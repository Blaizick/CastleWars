

using UnityEngine;

namespace Proj21
{
    public class Effect : MonoBehaviour
    {
        public void Init()
        {
            Destroy(gameObject, 10.0f);
        }
        public void OnDestroy()
        {
            Vars.restart.destroyOnRestart.Remove(gameObject);
        }
    }
}