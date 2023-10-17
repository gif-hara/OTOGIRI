namespace OTOGIRI.DungeonSystems
{
    public class DungeonModel
    {
        public Define.CellType[,] Map { get; private set; }

        public void SetMap(Define.CellType[,] map)
        {
            this.Map = map;
            DungeonEvents.SetMap.PublishWithKey(this, map);
        }
    }
}