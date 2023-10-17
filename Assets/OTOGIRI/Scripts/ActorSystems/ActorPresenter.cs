using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers;
using OTOGIRI.ActorControllers.Behaviours;
using OTOGIRI.GameSystems;

namespace OTOGIRI
{
    /// <summary>
    /// アクター行動処理を行うプレゼンター
    /// </summary>
    public sealed class ActorPresenter
    {
        public async UniTask ExecuteAsync(
            GameModel gameModel,
            ActorModel actorModel,
            IActorBehaviour actorBehaviour,
            CancellationToken cancellationToken
            )
        {
            await actorBehaviour.ExecuteAsync(actorModel, gameModel, cancellationToken);
        }
    }
}
