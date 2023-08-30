using System.Collections.Generic;
using OTOGIRI.Scripts;
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

        public void Print()
        {
            var sb = new System.Text.StringBuilder();
            for (var y = 0; y < this.Cells.GetLength(0); y++)
            {
                for (var x = 0; x < this.Cells.GetLength(1); x++)
                {
                    sb.Append(this.Cells[y, x] == Define.CellType.Ground ? "■" : "□");
                }
                sb.AppendLine();
            }
            // クリップボードにコピーする
            GUIUtility.systemCopyBuffer = sb.ToString();
            Debug.Log(sb.ToString());
        }
    }
}
