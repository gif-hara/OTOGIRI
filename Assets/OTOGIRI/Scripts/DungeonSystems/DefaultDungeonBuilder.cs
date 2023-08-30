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
            
            // 最初は全て壁に設定する
            for (var y = 0; y < this.dungeonSize.y; y++)
            {
                for (var x = 0; x < this.dungeonSize.x; x++)
                {
                    tiles[y, x] = Define.CellType.Wall;
                }
            }
            
            // 部屋を作成する
            var rooms = CreateRooms();
            
            // tilesに書き込む
            foreach (var room in rooms)
            {
                for (var y = 0; y < room.Rect.height; y++)
                {
                    for (var x = 0; x < room.Rect.width; x++)
                    {
                        var position = new Vector2Int((int)room.Rect.x + x, (int)room.Rect.y + y);
                        tiles[position.y, position.x] = Define.CellType.Ground;
                    }
                }
            }
            
            return new Dungeon(tiles, rooms);
        }

        private List<Room> CreateRooms()
        {
            var rooms = new List<Room>();
            for (var i = 0; i < this.roomCount; i++)
            {
                var width = Random.Range(this.roomSizeMin.x, this.roomSizeMax.x);
                var height = Random.Range(this.roomSizeMin.y, this.roomSizeMax.y);
                var x = Random.Range(0, this.dungeonSize.x - width);
                var y = Random.Range(0, this.dungeonSize.y - height);
                var newRoom = new Room(x, y, width, height);
                if(!IsRoomsOverlap(rooms, newRoom))
                {
                    rooms.Add(newRoom);
                }
            }

            return rooms;
        }
        
        private static bool IsRoomsOverlap(List<Room> rooms, Room room)
        {
            var roomRect = room.Rect;
            roomRect.x--;
            roomRect.y--;
            roomRect.width += 2;
            roomRect.height += 2;
            foreach (var r in rooms)
            {
                if (r.Rect.Overlaps(roomRect))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
