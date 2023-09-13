using UnityEngine;

namespace OTOGIRI
{
    public static class Extensions
    {
        public static Define.Direction ToDirection(this Vector2Int self)
        {
            if(self.y <= -1)
            {
                if(self.x <= -1)
                {
                    return Define.Direction.UpLeft;
                }
                if(self.x >= 1)
                {
                    return Define.Direction.UpRight;
                }
                
                return Define.Direction.Up;
            }
            if(self.y >= 1)
            {
                if(self.x <= -1)
                {
                    return Define.Direction.DownLeft;
                }
                if(self.x >= 1)
                {
                    return Define.Direction.DownRight;
                }
                else
                {
                    return Define.Direction.Down;
                }
            }
            else
            {
                if(self.x <= -1)
                {
                    return Define.Direction.Left;
                }
                else if(self.x >= 1)
                {
                    return Define.Direction.Right;
                }
                
                throw new System.ArgumentException("Vector2Int is zero");
            }
        }
        
        public static Vector2Int ToVector2Int(this Define.Direction self)
        {
            return self switch
            {
                Define.Direction.Up => new Vector2Int(0, -1),
                Define.Direction.UpRight => new Vector2Int(1, -1),
                Define.Direction.Right => new Vector2Int(1, 0),
                Define.Direction.DownRight => new Vector2Int(1, 1),
                Define.Direction.Down => new Vector2Int(0, 1),
                Define.Direction.DownLeft => new Vector2Int(-1, 1),
                Define.Direction.Left => new Vector2Int(-1, 0),
                Define.Direction.UpLeft => new Vector2Int(-1, -1),
                _ => throw new System.ArgumentException("Invalid Direction")
            };
        }
    }
}