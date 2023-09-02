using System.Collections.Generic;
using UnityEngine;

namespace OTOGIRI.DungeonSystems
{
    public class Corridor
    {
        public List<Vector2Int> Cells { get; }

        public Corridor(List<Vector2Int> cells)
        {
            this.Cells = cells;
        }
    }
}
