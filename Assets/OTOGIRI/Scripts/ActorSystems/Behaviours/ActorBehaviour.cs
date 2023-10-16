using OTOGIRI.GameSystems;

namespace OTOGIRI.ActorControllers.Behaviours
{
    public abstract class ActorBehaviour : IActorBehaviour
    {
        public Define.ActorBehaviourType BehaviourType { get; }

        public int ConsumeActionPoint { get; }

        public abstract void Execute(ActorModel actorModel, GameModel gameModel);

        public ActorBehaviour(Define.ActorBehaviourType behaviourType, int consumeActionPoint)
        {
            this.BehaviourType = behaviourType;
            this.ConsumeActionPoint = consumeActionPoint;
        }
    }
}