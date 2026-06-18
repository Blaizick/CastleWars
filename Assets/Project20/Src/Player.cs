using System;
using System.Linq;
using UnityEngine;

namespace Proj21
{
    public class Player : MonoBehaviour
    {
        public PlayerCastle Castle => Vars.teams.ally.castles.castles.Count > 0 ? Vars.teams.ally.castles.castles.First() as PlayerCastle : null;

        public void Init()
        {
            
        }
    }
}