using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.GameSystems;

namespace OTOGIRI.ActorControllers.Behaviours
{
    public abstract class ActorBehaviour : IActorBehaviour
    {
        public Define.ActorBehaviourType BehaviourType { get; }

        public int ConsumeActionPoint { get; }

        public abstract UniTask ExecuteAsync(ActorModel actorModel, GameModel gameModel, CancellationToken cancellationToken);

        public ActorBehaviour(Define.ActorBehaviourType behaviourType, int consumeActionPoint)
        {
            this.BehaviourType = behaviourType;
            this.ConsumeActionPoint = consumeActionPoint;
        }
    }
}