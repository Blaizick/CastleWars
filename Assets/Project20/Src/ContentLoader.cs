using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class ContentLoader : MonoBehaviour
    {
        public static ContentLoader loader = null;

        public void Init()
        {
            if (loader)
            {
                return;
            }
            loader = this;
            DontDestroyOnLoad(this);
            Cms.LoadAll("Content");
        }
    }
}