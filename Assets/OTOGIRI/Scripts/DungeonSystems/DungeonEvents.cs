using HK.Framework.MessageSystems;
using MessagePipe;

namespace OTOGIRI
{
    /// <summary>
    /// ダンジョンに関するイベント
    /// </summary>
    public static class DungeonEvents
    {
        public sealed class SetMap : Message<SetMap, Define.CellType[,]>
        {
            public Define.CellType[,] Map => this.Param1;
        }

        public static void RegisterEvents(BuiltinContainerBuilder builder)
        {
            builder.AddMessageBroker<DungeonModel, SetMap>();
        }
    }
}
