

using UnityEngine;

namespace Proj21
{
    public class EffectsSystem : MonoBehaviour
    {
        public Effect buildingDestroyEffect;

        public void CreateEffect(Effect effect, Vector2 position)
        {
            var e = Instantiate(effect, position, Quaternion.identity);
            Vars.restart.destroyOnRestart.Add(e.gameObject);
            e.Init();
        }
    }
}