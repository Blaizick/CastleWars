

using System;
using System.Text;
using Blaze.Runtime.Cms;
using TMPro;
using UnityEngine;

namespace Proj21
{
    public class BuildingTypeTooltip : MonoBehaviour
    {
        public TMP_Text titleText;
        public TMP_Text descText;
        public TMP_Text buildCostText;
        public GameObject root;

        public void Update()
        {
            if (Vars.input.sBuild != null)
            {
                titleText.text = $"{Vars.input.sBuild.GetComponent<CmsNameComp>().name}";                
                descText.text = $"{Vars.input.sBuild.GetComponent<CmsDescComp>().desc}";
                StringBuilder sb = new();
                foreach (var i in Vars.input.sBuild.GetAllComponentsOfType<CmsBuildCostComp>())
                {
                    sb.AppendLine($"{i.cost.AsItemStack().item.GetComponent<CmsNameComp>().name}: {i.cost.count}");
                }
                buildCostText.text = sb.ToString();
            }
        }
    }

    [Serializable]
    public class CmsDescComp : CmsComponent
    {
        public string desc;
    }
}