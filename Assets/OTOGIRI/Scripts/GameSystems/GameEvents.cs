using HK.Framework.MessageSystems;
using MessagePipe;
using OTOGIRI.ActorControllers;

namespace OTOGIRI.GameSystems
{
    public static class GameEvents
    {
        /// <summary>
        /// アクターが追加された時のメッセージ
        /// </summary>
        public class AddedActorModel : Message<AddedActorModel, ActorModel>
        {
            public ActorModel ActorModel => this.Param1;
        }

        /// <summary>
        /// アクターが削除された時のメッセージ
        /// </summary>
        public class RemovedActorModel : Message<RemovedActorModel, ActorModel>
        {
            public ActorModel ActorModel => this.Param1;
        }

        public static void RegisterEvents(BuiltinContainerBuilder builder)
        {
            builder.AddMessageBroker<AddedActorModel>();
            builder.AddMessageBroker<RemovedActorModel>();
        }
    }
}
