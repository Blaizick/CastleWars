

using System;

namespace Proj21
{
    public class CastleHealthComp : HealthComp
    {
        [NonSerialized] public Castle castle;

        public void Set1(Castle castle)
        {
            this.castle = castle;
        }

        public override bool CanBeDamaged(object source)
        {
            return castle.buildings.Count == 0;
        }
    }
}