using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OTOGIRI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace OTOGIRI.DungeonSystems
{
    [Serializable]
    public class DefaultDungeonBuilder : IDungeonBuilder
    {
        [SerializeField]
        private Vector2Int dungeonSize;
        
        [SerializeField]
        private int roomCount;
        
        [SerializeField]
        private int splitRandomRange;
        
        [SerializeField]
        private int roomRandomRange;

        private List<Room> CreateRooms(Define.CellType[,] cellTypes)
        {
            var rooms = new List<Room>();
            var splitDungeons = new List<RectInt>
            {
                new(0, 0, this.dungeonSize.x, this.dungeonSize.y)
            };
            var connectRoomIndex = new List<(int roomAIndex, int roomBIndex)>();
            for (var i = 0; i < this.roomCount - 1; i++)
            {
                var maxSize = 0;
                var targetIndex = -1;
                for (var j = 0; j < splitDungeons.Count; j++)
                {
                    var size = splitDungeons[j].width * splitDungeons[j].height;
                    if (size > maxSize)
                    {
                        maxSize = size;
                        targetIndex = j;
                    }
                }
                Assert.IsTrue(targetIndex >= 0);
                var targetRect = splitDungeons[targetIndex];
                var isSplitHorizontal = targetRect.width < targetRect.height;
                var splitRectA = new RectInt(
                    targetRect.x,
                    targetRect.y,
                    isSplitHorizontal ? targetRect.width : targetRect.width / 2 - 1 + Random.Range(-this.splitRandomRange, this.splitRandomRange + 1),
                    isSplitHorizontal ? targetRect.height / 2 - 1 + Random.Range(-this.splitRandomRange, this.splitRandomRange + 1) : targetRect.height
                    );
                var splitRectB = new RectInt(
                    isSplitHorizontal ? targetRect.x : splitRectA.xMax + 1,
                    isSplitHorizontal ? splitRectA.yMax + 1 : targetRect.y,
                    isSplitHorizontal ? targetRect.width : targetRect.width - splitRectA.width - 1,
                    isSplitHorizontal ? targetRect.height - splitRectA.height - 1 : targetRect.height
                    );
                connectRoomIndex.RemoveAll(x => x.roomAIndex == targetIndex || x.roomBIndex == targetIndex);
                splitDungeons[targetIndex] = splitRectA;
                splitDungeons.Add(splitRectB);
                connectRoomIndex.Add((targetIndex, splitDungeons.Count - 1));
            }
            
            foreach (var splitDungeon in splitDungeons)
            {
                var xRandomRange = Random.Range(0, this.roomRandomRange + 1);
                var yRandomRange = Random.Range(0, this.roomRandomRange + 1);
                var x = splitDungeon.x + 1 + xRandomRange;
                var y = splitDungeon.y + 1 + yRandomRange;
                var width = splitDungeon.width - 2 - xRandomRange - Random.Range(0, this.roomRandomRange + 1);
                var height = splitDungeon.height - 2 - yRandomRange - Random.Range(0, this.roomRandomRange + 1);
                if (width < 5 || height < 5)
                {
                    continue;
                }
                rooms.Add(new Room(x, y, width, height));
            }
            
            // connect rooms
            var connectRoomIndexListCount = connectRoomIndex.Count;
            for (var i = 0; i < connectRoomIndexListCount; i++)
            {
                var (roomAIndex, roomBIndex) = connectRoomIndex[i];
                var roomA = rooms[roomAIndex];
                var roomB = rooms[roomBIndex];
                var roomAPoint = roomA.GetRandomPointInsideRoom();
                var roomBPoint = roomB.GetRandomPointInsideRoom();
                while (roomAPoint.x != roomBPoint.x)
                {
                    if (roomAPoint.x < roomBPoint.x)
                    {
                        roomAPoint.x++;
                    }
                    else
                    {
                        roomAPoint.x--;
                    }
                    cellTypes[roomAPoint.y, roomAPoint.x] = Define.CellType.Ground;
                }
                while(roomAPoint.y != roomBPoint.y)
                {
                    if (roomAPoint.y < roomBPoint.y)
                    {
                        roomAPoint.y++;
                    }
                    else
                    {
                        roomAPoint.y--;
                    }
                    cellTypes[roomAPoint.y, roomAPoint.x] = Define.CellType.Ground;
                }
            }
            
            return rooms;
        }
        
        private List<Vector2Int> CreateCorridors(List<Room> rooms)
        {
            var corridors = new List<Vector2Int>();

            return corridors;
        }

        public Dungeon Build()
        {
            var cells = new Define.CellType[dungeonSize.y, dungeonSize.x];

            for (var y = 0; y < dungeonSize.y; y++)
            {
                for (var x = 0; x < dungeonSize.x; x++)
                {
                    cells[y, x] = Define.CellType.Wall;
                }
            }

            var rooms = CreateRooms(cells);
            foreach (var room in rooms)
            {
                for (var y = room.Rect.y; y < room.Rect.yMax; y++)
                {
                    for (var x = room.Rect.x; x < room.Rect.xMax; x++)
                    {
                        cells[y, x] = Define.CellType.Ground;
                    }
                }
            }

            var corridors = CreateCorridors(rooms);
            foreach (var point in corridors)
            {
                cells[point.y, point.x] = Define.CellType.Ground;
            }

            return new Dungeon(cells, rooms);
        }
    }
}
