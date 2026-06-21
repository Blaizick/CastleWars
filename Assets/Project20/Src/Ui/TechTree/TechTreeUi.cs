using System;
using System.Collections.Generic;
using Blaze.Runtime;
using Blaze.Runtime.Cms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Proj21
{
    public class TechTreeUi : MonoBehaviour
    {
        public TechNodeUiCntPfb pfb;

        public Transform contentRootTr;
        public GameObject root;

        public TMP_Text quintEssenceText;

        public Dictionary<CmsEntity, TechNodeUiCntPfb> instancesDic = new();
        public Button backBtn;

        public void Init()
        {
            foreach (var i in ResearchTypes.All)
            {
                var inst = Instantiate(pfb, contentRootTr);
                ((RectTransform)inst.transform).anchoredPosition = i.GetComponent<CmsPosComp>().position;
                foreach (var state in inst.AllStates)
                {
                    state.btn.onClick.AddListener(() =>
                    {
                        if (!Vars.researches.IsResearched(i) && Vars.researches.IsAwailable(i))
                        {
                            Vars.researches.Research(i);
                        }
                    });
                    state.nameText.text = i.GetComponent<CmsNameComp>().name;
                }
                instancesDic[i] = inst;
            }

            foreach (var (research, view) in instancesDic)
            {
                foreach (var req in research.GetAllComponentsOfType<CmsRequiredTechComp>())
                {
                    var go = new GameObject("Line", typeof(RectTransform));
                    go.transform.SetParent(view.linesRootTr);
                    go.transform.localScale = Vector3.one;
                    ((RectTransform)go.transform).anchoredPosition = Vector3.zero;
                    ((RectTransform)go.transform).sizeDelta = Vector2.zero;
                    var lineRenderer = go.AddComponent<UILineRenderer>();
                    lineRenderer.points = new Vector2[]
                    {
                        Vector2.zero,
                        ((RectTransform)instancesDic[req.tech.GetCmsEntity()].transform).anchoredPosition - ((RectTransform)view.root.transform).anchoredPosition,
                    };
                    if (view.root.transform.GetSiblingIndex() > instancesDic[req.tech.GetCmsEntity()].root.transform.GetSiblingIndex())
                    {
                        instancesDic[req.tech.GetCmsEntity()].root.transform.SetSiblingIndex(view.root.transform.GetSiblingIndex());
                    }
                }
            }

            backBtn.onClick.AddListener(() =>
            {
                Hide();
            });

            root.SetActive(false);
        }

        public void Show()
        {
            if (Vars.ui.sceneTransitionScreen.tween == null)
            {
                Time.timeScale = 0.0f;
                gameObject.SetActive(true);
            }
        }
        public void Hide()
        {
            if (Vars.ui.sceneTransitionScreen.tween == null)
            {
                Time.timeScale = 1.0f;
                gameObject.SetActive(false);
            }
        }

        public void Update()
        {
            foreach (var (research, view) in instancesDic)
            {
                view.awailableState.root.SetActive(!Vars.researches.IsResearched(research) && Vars.researches.IsAwailable(research));
                view.notAwailableState.root.SetActive(!Vars.researches.IsResearched(research) && !Vars.researches.IsAwailable(research));
                view.researchedState.root.SetActive(Vars.researches.IsResearched(research));
            }

            quintEssenceText.text = $"Quintessence: {Vars.quintEssence.quintEssence}";
        }
    }

    [Serializable]
    public class CmsPosComp : CmsComponent
    {
        public Vector2 position;
    }


    public static class ResearchTypes
    {
        public static CmsEntity Root => Cms.GetEntity("RootTech");
        public static CmsEntity Mender => Cms.GetEntity("MenderTech");
        public static CmsEntity Wall => Cms.GetEntity("WallTech");
        public static CmsEntity SniperTurret => Cms.GetEntity("SniperTurretTech");

        public static List<CmsEntity> All => new() {Root, Mender, Wall, SniperTurret, };
    }

    public class QuintEssenceSystem
    {
        public int quintEssence;

        public void Add(int count)
        {
            quintEssence += count;
            quintEssence = Math.Clamp(quintEssence, 0, int.MaxValue);
        }

        public void Remove(int count)
        {
            quintEssence -= count;
            quintEssence = Math.Clamp(quintEssence, 0, int.MaxValue);
        }

        public bool Has(int  count ) => quintEssence >= count;
    }

    public class ResearchSystem
    {
        public List<CmsEntity> researched = new();

        public void Init()
        {
            foreach (var i in ResearchTypes.All)
            {
                if (i.HasComponent<CmsAlwaysResearchedTag>())
                {
                    researched.Add(i);
                }
            }
        }

        public bool IsResearched(CmsEntity tech) => researched.Contains(tech);

        public bool IsAwailable(CmsEntity tech)
        {
            foreach (var  i in tech.GetAllComponentsOfType<CmsRequiredTechComp>())
            {
                if (!IsResearched(i.tech.GetCmsEntity()))
                {
                    return false;
                }
            }
            return true;
        }

        public void Research (CmsEntity tech)
        {
            researched.Add(tech);
        }
    }
    [Serializable]
    public class CmsRequiredTechComp : CmsComponent
    {
        public CmsEntityPfb tech;
    }
    [Serializable]
    public class CmsAlwaysResearchedTag : CmsComponent
    {
        
    }
}