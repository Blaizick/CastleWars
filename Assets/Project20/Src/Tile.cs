

using System;
using UnityEngine;

namespace Proj21
{
    public class Tile : MonoBehaviour
    {
        [NonSerialized] public Castle castle;
        [NonSerialized] public Vector2Int pos;
        [NonSerialized] public Building building = null;
    }
}