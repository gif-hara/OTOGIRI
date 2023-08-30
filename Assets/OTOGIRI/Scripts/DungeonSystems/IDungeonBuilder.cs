namespace OTOGIRI.DungeonSystems
{
    /// <summary>
    /// ダンジョン生成のインターフェース
    /// </summary>
    public interface IDungeonBuilder
    {
        /// <summary>
        /// ダンジョンを生成する
        /// </summary>
        Dungeon Build();
    }
}
