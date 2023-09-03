﻿namespace OTOGIRI
{
    public static class Define
    {
        /// <summary>
        /// マップのセルの種類
        /// </summary>
        public enum CellType
        {
            Abyss = -2,
            Waterway = -1,
            Ground = 0,
            Step = 1,
            Wall = 2,
        }
        
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }
    }
}