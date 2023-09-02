using System.Collections.Generic;
using UnityEngine;

namespace OTOGIRI.DungeonSystems
{
    public class Dungeon
    {
        public Define.CellType[,] Cells { get; }

        public List<Room> Rooms { get; }
        
        public Vector2Int Size => new Vector2Int(this.Cells.GetLength(1), this.Cells.GetLength(0));
        
        public Dungeon(Define.CellType[,] cells, List<Room> rooms)
        {
            this.Cells = cells;
            this.Rooms = rooms;
        }
        
        public bool IsRoom(Vector2Int position)
        {
            foreach (var room in this.Rooms)
            {
                if (room.Rect.Contains(position))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
