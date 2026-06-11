using System.Collections.Generic;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class ItemsUi : MonoBehaviour
    {
        public ItemUiCntPfb itemUiCntPfb;
        public GameObject root;
        public RectTransform contentRootTr;
        public Dictionary<CmsEntity, ItemUiCntPfb> instancesDic = new();
    
        public void Init()
        {
            foreach (var i in Items.All)
            {
                var inst = Instantiate(itemUiCntPfb, contentRootTr);
                instancesDic[i] = inst;
            }
        }

        public void Update()
        {
            foreach (var (k, v) in instancesDic)
            {
                v.text.text = $"{k.GetComponent<CmsNameComp>().name}: {Vars.items.GetStack(k).count.ToString("0")}";
            }
        }
    }
}