using System.Threading;
using Cysharp.Threading.Tasks;
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

        public override UniTask ExecuteAsync(ActorModel actorModel, GameModel gameModel, CancellationToken cancellationToken)
        {
            actorModel.Position += this.direction.ToVector2Int();
            return UniTask.CompletedTask;
        }
    }
}