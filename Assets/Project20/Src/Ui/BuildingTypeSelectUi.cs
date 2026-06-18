using System;
using System.Collections.Generic;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class BuildingTypeSelectUi : MonoBehaviour
    {
        public BuildingTypeSelectUiCntPfb buildingTypeSelectUiCntPfb;

        public Transform contentRootTr;
        public GameObject root;

        [NonSerialized] public List<BuildingTypeSelectUiCntPfb> instances = new();
        [NonSerialized] public Dictionary<CmsEntity, BuildingTypeSelectUiCntPfb> instancesDic = new(); 

        public void Init()
        {
            Rebuild();
        }

        public void Update()
        {
            foreach (var (k, v) in instancesDic)
            {
                v.selectedState.root.SetActive(Vars.input.sBuild == k);
                v.notSelectedState.root.SetActive(Vars.input.sBuild != k);
            }
        }

        public void Rebuild()
        {
            instances.ForEach(i => Destroy(i.gameObject));
            instances = new();
            instancesDic = new();

            foreach (var bt in BuildingTypes.All)
            {
                var inst = Instantiate(buildingTypeSelectUiCntPfb, contentRootTr);
                foreach (var state in inst.AllStates)
                {
                    state.nameText.text = bt.GetComponent<CmsNameComp>().name;
                    state.image.sprite = bt.GetComponent<CmsSpriteComp>().sprite;
                    state.btn.onClick.AddListener(() =>
                    {
                        if (Vars.input.sBuild == bt)
                        {
                            Vars.input.UnsetSBuilding();
                        }
                        else
                        {
                            Vars.input.SetSBuilding(bt);                        
                        }
                    });
                    state.root.SetActive(false);
                }
                inst.notSelectedState.root.SetActive(true);
                instancesDic[bt] = inst;
                instances.Add(inst);
            }
        }
    }

    [Serializable]
    public class CmsNameComp : CmsComponent
    {
        public string name;
    }

    public static class BuildingTypes
    {
        public static CmsEntity Turret => Cms.GetEntity("Turret");
        public static CmsEntity Collector => Cms.GetEntity("Collector");
        public static CmsEntity SniperTurret => Cms.GetEntity("SniperTurret");
        public static CmsEntity Wall => Cms.GetEntity("Wall");
        public static CmsEntity Mender => Cms.GetEntity("Mender");

        public static List<CmsEntity> All => new(){Turret, Collector, SniperTurret, Wall, Mender,};
    }
}