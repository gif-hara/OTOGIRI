using System.Threading;
using Cysharp.Threading.Tasks;

namespace OTOGIRI.GameSystems
{
    /// <summary>
    /// ゲームのプレゼンター
    /// </summary>
    public sealed class GamePresenter
    {
        public async UniTask BeginGameLoopAsync(GameModel gameModel, CancellationToken cancellationToken)
        {
            var actorBehaviourInvoker = new ActorControllers.BehaviourInvokers.Log();

            while (!cancellationToken.IsCancellationRequested)
            {
                // すべてのActorのターン処理を行う
                foreach (var actorModel in gameModel.ActorModels)
                {
                    using (var actorScope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                    {
                        var actorBehaviour = await actorModel.AI.ThinkAsync(actorModel, actorScope.Token);
                        actorScope.Cancel();
                        await actorBehaviourInvoker.InvokeAsync(
                            actorModel,
                            actorBehaviour,
                            gameModel,
                            cancellationToken
                            );
                    }
                }
            }
        }
    }
}
