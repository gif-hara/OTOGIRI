using UnityEngine;

namespace OTOGIRI.DungeonSystems
{
    public struct Room
    {
        public Rect Rect { get; }
        
        public Room(Rect rect)
        {
            this.Rect = rect;
        }
        
        public Room(int x, int y, int width, int height)
        {
            this.Rect = new Rect(x, y, width, height);
        }
    }
}
