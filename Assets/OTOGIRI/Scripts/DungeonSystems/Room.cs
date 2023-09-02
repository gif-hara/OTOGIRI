using UnityEngine;

namespace OTOGIRI.DungeonSystems
{
    public readonly struct Room
    {
        public RectInt Rect { get; }
        
        public Room(int x, int y, int width, int height)
        {
            this.Rect = new RectInt(x, y, width, height);
        }

        public Vector2Int GetRandomPointInsideRoom()
        {
            var x = Random.Range(Rect.xMin, Rect.xMax);
            var y = Random.Range(Rect.yMin, Rect.yMax);
            return new Vector2Int(x, y);
        }
    }
}
