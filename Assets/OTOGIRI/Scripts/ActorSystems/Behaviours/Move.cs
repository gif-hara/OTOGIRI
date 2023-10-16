using OTOGIRI.GameSystems;

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

        public override void Execute(ActorModel actorModel, GameModel gameModel)
        {
            actorModel.Position += this.direction.ToVector2Int();
        }
    }
}