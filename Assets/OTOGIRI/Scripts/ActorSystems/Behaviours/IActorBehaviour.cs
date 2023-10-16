namespace OTOGIRI.ActorControllers.Behaviours
{
    public interface IActorBehaviour
    {
        Define.ActorBehaviourType BehaviourType { get; }

        int ConsumeActionPoint { get; }
        
        void Execute(ActorModel actorModel, DungeonModel dungeonModel);
    }
}