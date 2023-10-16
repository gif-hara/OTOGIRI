using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers;
using UnityEngine;

namespace OTOGIRI.SceneControllers
{
    public class GameSceneController : MonoBehaviour
    {
        private void Start()
        {
            BeginGameLoopAsync(this.destroyCancellationToken)
                .Forget();
        }

        private static async UniTask BeginGameLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                var dungeonModel = new DungeonModel();
                var playerModel = new ActorModel(
                    "Player", new ActorControllers.AISystems.Input(), new Vector2Int(0, 0)
                    );
                dungeonModel.SetPlayerModel(playerModel);

                // 敵の生成をループで行う
                for (int i = 1; i <= 3; i++)
                {
                    dungeonModel.AddOtherModel(
                        new ActorModel($"Enemy{i}", new ActorControllers.AISystems.Random(), new Vector2Int(0, 0))
                        );
                }

                var actorBehaviourInvoker = new ActorControllers.BehaviourInvokers.Log();

                while (!cancellationToken.IsCancellationRequested)
                {
                    // すべてのActorのターン処理を行う
                    foreach (var model in dungeonModel.AllModels)
                    {
                        using (var actorScope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                        {
                            var actorBehaviour = await model.AI.ThinkAsync(model, actorScope.Token);
                            actorScope.Cancel();
                            await actorBehaviourInvoker.InvokeAsync(
                                model,
                                actorBehaviour,
                                dungeonModel,
                                cancellationToken
                                );
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}