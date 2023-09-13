namespace OTOGIRI
{
    public static class Define
    {
        public enum ActorBehaviourType
        {
            Move,
            Action
        }

        public enum ActorBehaviourResult
        {
            Success,
            Abort,
        }
        
        public enum ActorState
        {
            Idle,
            Move,
            Action,
        }
        
        public enum Direction
        {
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpLeft,
        }
        
        public enum CellType
        {
            Ground,
            Wall,
        }
    }
}