using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.GameSystems;

namespace OTOGIRI.ActorControllers.Behaviours
{
    public interface IActorBehaviour
    {
        Define.ActorBehaviourType BehaviourType { get; }

        int ConsumeActionPoint { get; }

        UniTask ExecuteAsync(ActorModel actorModel, GameModel gameModel, CancellationToken cancellationToken);
    }
}