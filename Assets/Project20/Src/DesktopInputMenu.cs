

using System;
using UnityEngine;

namespace Proj21
{
    public class DesktopInputMenu : MonoBehaviour
    {
        public InputSystem_Actions actions;

        [NonSerialized] public float cameraTargetZoom;
        [NonSerialized] public Vector2 mousePos;

        public void Init()
        {
            actions = new();
            actions.Enable();
        }

        public void OnDestroy()
        {
            actions.Disable();
            actions.Dispose();
        }

        public void Update()
        {
            if (Vars.uiMenu.levelsMapRoot.activeInHierarchy)
            {
                if (actions.Player.MouseWheel.IsPressed() || actions.Player.Place.IsPressed())
                {
                    Vector2 delta = actions.Player.MouseDelta.ReadValue<Vector2>() / 80.0f * (Vars.camera.Zoom / 8.0f);
                    Vector2 pos = (Vector2)Vars.camera.transform.position - delta;
                    Vars.camera.transform.position = new Vector3(pos.x, pos.y, -10.0f);
                }

                // float zoomDt = actions.Player.Zoom.ReadValue<float>() * -0.5f;
                // cameraTargetZoom = Mathf.Clamp(cameraTargetZoom + zoomDt, 1.0f, 20.0f);
                // Vars.camera.Lens.OrthographicSize = Mathf.Lerp(Vars.camera.Lens.OrthographicSize, cameraTargetZoom, 30.0f * Time.unscaledDeltaTime);
            }

            mousePos = actions.Player.MousePos.ReadValue<Vector2>();
            if (Vars.uiMenu.mainUiRoot.activeInHierarchy)
            {
                Vector2 centerPos = new Vector2(Screen.width, Screen.height) / 2.0f;
                Vector2 dif = (mousePos - centerPos) / 500.0f;
                Vars.camera.transform.position = new Vector3(dif.x, dif.y, -10.0f);
            }
        }
    }
}