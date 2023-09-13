namespace OTOGIRI.ActorControllers.Behaviours
{
    public class Move : ActorBehaviour
    {
        private readonly Define.Direction direction;

        public Move(int consumeActionPoint, Define.Direction direction)
            : base(Define.ActorBehaviourType.Move, consumeActionPoint)
        {
            this.direction = direction;
        }

        public override void Execute(ActorModel actorModel, DungeonModel dungeonModel)
        {
            actorModel.MovedRoutes.Add(this.direction);
            actorModel.Position += this.direction.ToVector2Int();
        }
    }
}