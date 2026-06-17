using System;
using System.Collections.Generic;
using Blaze.Runtime.Cms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Proj21
{
    public class DesktopInput : MonoBehaviour
    {
        public InputSystem_Actions actions;

        [NonSerialized] public Vector2 mousePos;
        [NonSerialized] public Vector2 mouseWorldPos;
        [NonSerialized] public Vector2Int mouseGridPos;
        [NonSerialized] public Vector2Int buildGridPos;

        [NonSerialized] public float cameraTargetZoom;

        public Transform sPlanTr;
        public SpriteRenderer sPlanSpriteRenderer;
        public CmsEntity sBuild;

        [NonSerialized] public bool pointerOverGameObject;

        public GameObject breakSelectionRoot;
        public RectTransform breakSelectionTr;

        [NonSerialized] public Vector2Int breakSelectionPos0;
        [NonSerialized] public Vector2Int breakSelectionPos1;
        [NonSerialized] public bool breaking;

        [NonSerialized] public bool cameraFollowCastle;

        [NonSerialized] public bool castlesMode;

        public void Init()
        {
            actions = new();
            actions.Enable();

            cameraTargetZoom = Vars.camera.Lens.OrthographicSize;
        }

        public void OnDestroy()
        {
            actions.Disable();
            actions.Dispose();            
        }

        public void Restart()
        {
            sBuild = null;
            castlesMode = false;
            cameraFollowCastle = false;
            breaking = false;
        }

        public void _Update()
        {
            if (actions.Player.CameraFollowCastle.WasPerformedThisFrame())
            {
                cameraFollowCastle = !cameraFollowCastle;
            }
            
            if (cameraFollowCastle)
            {
                if (Vars.player && Vars.player.castle)
                {
                    Vector2 pos = Vector2.MoveTowards(Vars.camera.transform.position, Vars.player.castle.CenterPosition, 100.0f * Time.unscaledDeltaTime);
                    Vars.camera.transform.position = new Vector3(pos.x, pos.y, -10.0f);
                }
            }
            else
            {
                if (actions.Player.MouseWheel.IsPressed())
                {
                    Vector2 delta = actions.Player.MouseDelta.ReadValue<Vector2>() / 80.0f * (Vars.camera.Lens.OrthographicSize / 8.0f);
                    Vector2 pos = (Vector2)Vars.camera.transform.position - delta;
                    Vars.camera.transform.position = new Vector3(pos.x, pos.y, -10.0f);
                }
            }

            float zoomDt = actions.Player.Zoom.ReadValue<float>() * -0.5f;
            cameraTargetZoom = Mathf.Clamp(cameraTargetZoom + zoomDt, 1.0f, 20.0f);
            Vars.camera.Lens.OrthographicSize = Mathf.Lerp(Vars.camera.Lens.OrthographicSize, cameraTargetZoom, 30.0f * Time.unscaledDeltaTime);

            mousePos = actions.Player.MousePos.ReadValue<Vector2>();
            mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            if (Vars.player && Vars.player.castle)
            {
                mouseGridPos = Vars.player.castle.WorldToGridPosition(mouseWorldPos);
            }

            pointerOverGameObject = EventSystem.current.IsPointerOverGameObject();

            if (actions.Player.SwitchCastlesMode.WasPerformedThisFrame())
            {
                castlesMode = !castlesMode;
            }

            if (castlesMode)
            {
                if (sBuild != null)
                {
                    UnsetSBuilding();
                }
                if (breaking)
                {
                    breaking = false;
                }
            }
            if (Vars.player && Vars.player.castle)
            {
                Vars.player.castle.selectSprite.gameObject.SetActive(castlesMode);
                Vars.player.castle.pathEndSprite.gameObject.SetActive(castlesMode && Vars.player.castle.moving);
                Vars.player.castle.pathLineRenderer.gameObject.SetActive(castlesMode && Vars.player.castle.moving);
            }

            sPlanTr.gameObject.SetActive(sBuild != null);

            if (sBuild != null && Vars.player && Vars.player.castle)
            {
                int size = sBuild.GetComponent<CmsSquareSizeComp>().size;

                var tmp = mouseWorldPos;
                if (size % 2 == 0)
                {
                    tmp -= Vector2.one / 2;
                }
                buildGridPos = Vars.player.castle.WorldToGridPosition(tmp);

                sPlanSpriteRenderer.sprite = sBuild.GetComponent<CmsSpriteComp>().sprite;
                sPlanTr.transform.position = Vars.player.castle.GetRectPosition(buildGridPos, size);            
                if (Vars.player.castle.CanPlaceBuilding(buildGridPos, size))
                {
                    sPlanSpriteRenderer.color = new Color(0.25f, 0.25f, 0.65f, 0.65f);
                }
                else
                {
                    sPlanSpriteRenderer.color = new Color(0.65f, 0.25f, 0.25f, 0.65f);
                }
            }

            if (actions.Player.Place.IsPressed())
            {
                if (!castlesMode)
                {
                    if (sBuild != null && Vars.player && Vars.player.castle && !pointerOverGameObject)
                    {
                        if (Vars.player.castle.CanPlaceBuilding(buildGridPos, sBuild.GetComponent<CmsSquareSizeComp>().size))
                        {
                            bool success = true;
                            foreach (var c in sBuild.GetAllComponentsOfType<CmsBuildCostComp>())
                            {
                                if (!Vars.items.Has(c.cost.AsItemStack()))
                                {
                                    success = false;
                                    break;
                                }
                            }
                            if (success)
                            {
                                foreach (var c in sBuild.GetAllComponentsOfType<CmsBuildCostComp>())
                                {
                                    Vars.items.Remove(c.cost.AsItemStack());
                                }
                                Vars.player.castle.StartConstructingBuilding(sBuild, buildGridPos);
                                // Vars.player.castle.CreateBuilding(sBuild, buildGridPos);
                            }
                        }
                    }
                }
            }


            if (actions.Player.Break.WasPerformedThisFrame())
            {
                if (Vars.player && Vars.player.castle)
                {
                    if (castlesMode)
                    {
                        Vars.player.castle.MoveTo(mouseWorldPos);
                    }
                    else
                    {
                        if (sBuild != null)
                        {
                            UnsetSBuilding();
                        }
                        else
                        {
                            if (!pointerOverGameObject)
                            {
                                breaking = true;
                                breakSelectionPos0 = mouseGridPos;
                            }
                        }
                    }
                }
            }

            breakSelectionRoot.gameObject.SetActive(breaking);

            if (!castlesMode && Vars.player && Vars.player.castle)
            {   
                breakSelectionPos1 = mouseGridPos;

                Vector2Int min = new Vector2Int(Math.Min(breakSelectionPos0.x, breakSelectionPos1.x), Math.Min(breakSelectionPos0.y, breakSelectionPos1.y));
                Vector2Int max = new Vector2Int(Math.Max(breakSelectionPos0.x, breakSelectionPos1.x), Math.Max(breakSelectionPos0.y, breakSelectionPos1.y));
    
                Vector2 minW = Vars.player.castle.GridToWorldPosition(min) - (Vector2.one / 2);
                Vector2 maxW = Vars.player.castle.GridToWorldPosition(max) + (Vector2.one / 2);
                
                Vector2 minS = Camera.main.WorldToScreenPoint(minW);
                Vector2 maxS = Camera.main.WorldToScreenPoint(maxW);

                breakSelectionTr.anchoredPosition = (minS + maxS) / 2;
                breakSelectionTr.sizeDelta = maxS - minS;

                if (actions.Player.Break.WasReleasedThisFrame())
                {
                    if (breaking)
                    {
                        foreach (var t in Vars.player.castle.GetTilesInRect(new RectInt(min, max - min + Vector2Int.one)))
                        {
                            if (t.building)
                            {
                                Vars.player.castle.DestroyBuilding(t.building);
                            }
                        }
                        breaking = false;
                    }
                }
            }

            if (actions.Player.Pause.WasPerformedThisFrame())
            {
                if (Vars.ui.sceneTransitionScreen.tween == null)
                {
                    if (Vars.ui.pauseMenu.root.activeInHierarchy)
                    {
                        Vars.ui.pauseMenu.Hide();
                    }
                    else
                    {
                        Vars.ui.pauseMenu.Show();
                    }    
                }
            }
        }

        public void SetSBuilding(CmsEntity build)
        {
            sBuild = build;
        }
        public void UnsetSBuilding()
        {
            sBuild = null;
        }

        void OnDrawGizmos()
        {
            if (Vars.player && Vars.player.castle)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(Vars.player.castle.GridToWorldPosition(mouseGridPos), Vector3.one);
            }
        }
    }
}