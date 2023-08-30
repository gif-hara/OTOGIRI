using System;
using System.Collections.Generic;
using OTOGIRI.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OTOGIRI.DungeonSystems
{
    [Serializable]
    public class DefaultDungeonBuilder : IDungeonBuilder
    {
        [SerializeField]
        private Vector2Int dungeonSize;
        
        [SerializeField]
        private Vector2Int roomSizeMin;
        
        [SerializeField]
        private Vector2Int roomSizeMax;
        
        [SerializeField]
        private int roomCount;
        
        
        public Dungeon Build()
        {
            var tiles = new Define.CellType[this.dungeonSize.y, this.dungeonSize.x];
            
            // 部屋を作成する
            var rooms = new List<Room>();
            for (var i = 0; i < this.roomCount; i++)
            {
                var width = Random.Range(this.roomSizeMin.x, this.roomSizeMax.x);
                var height = Random.Range(this.roomSizeMin.y, this.roomSizeMax.y);
                var x = Random.Range(1, this.dungeonSize.x - width - 1);
                var y = Random.Range(1, this.dungeonSize.y - height - 1);
                var newRoom = new Room(x, y, width, height);
                if(!IsRoomsOverlap(rooms, newRoom))
                {
                    rooms.Add(newRoom);
                    // tilesに書き込む
                    for (var j = y; j < y + height; j++)
                    {
                        for (var k = x; k < x + width; k++)
                        {
                            tiles[j, k] = Define.CellType.Wall;
                        }
                    }
                }
            }
            
            return new Dungeon(tiles, rooms);
        }
        
        private static bool IsRoomsOverlap(List<Room> rooms, Room room)
        {
            foreach (var r in rooms)
            {
                if (r.Rect.Overlaps(room.Rect))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
