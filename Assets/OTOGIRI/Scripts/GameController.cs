using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers.Behaviours;
using UnityEngine;
using UnityEngine.Assertions;

namespace OTOGIRI.ActorControllers
{
    public class GameController : MonoBehaviour
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
                var playerModel = new ActorModel("Player", new AISystems.Input());
                dungeonModel.SetPlayerModel(playerModel);
                dungeonModel.AddOtherModel(new ActorModel("Enemy1", new AISystems.Random()));
                dungeonModel.AddOtherModel(new ActorModel("Enemy2", new AISystems.Random()));
                dungeonModel.AddOtherModel(new ActorModel("Enemy3", new AISystems.Random()));
                var actionBehaviours = new List<(ActorModel model, IActorBehaviour behaviour)>();
                CancellationTokenSource actorScope = null;
                var actorBehaviourInvoker = new BehaviourInvokers.Log();
                
                while (true)
                {
                    actionBehaviours.Clear();
                    
                    // 行動力を回復する
                    foreach (var model in dungeonModel.AllModels)
                    {
                        model.ActionPoint += model.RecoveryActionPoint;
                    }
                    
                    // プレイヤーの行動を処理する
                    while (playerModel.ActionPoint > 0)
                    {
                        await playerModel.WaitUntilIdleAsync();
                        CreateScope();
                        await playerModel.AI.ThinkAsync(playerModel, actorScope.Token);
                        ClearScope();
                        var behaviour = playerModel.NextBehaviour;
                        Assert.IsNotNull(behaviour, "behaviour != null");
                        playerModel.ActionPoint -= behaviour.ConsumeActionPoint;
                        var task = actorBehaviourInvoker.InvokeAsync(
                            playerModel,
                            behaviour,
                            dungeonModel,
                            cancellationToken
                        );
                        if (behaviour.BehaviourType == Define.ActorBehaviourType.Move)
                        {
                            task.Forget();
                        }
                        else
                        {
                            await task;
                        }
                    }
                        
                    // 全ての他アクターの行動を処理する
                    foreach (var model in dungeonModel.OtherModels)
                    {
                        while (model.ActionPoint > 0)
                        {
                            CreateScope();
                            await model.AI.ThinkAsync(model, actorScope.Token);
                            ClearScope();
                            var behaviour = model.NextBehaviour;
                            Assert.IsNotNull(behaviour, "behaviour != null");
                            model.ActionPoint -= behaviour.ConsumeActionPoint;
                            if (behaviour.BehaviourType == Define.ActorBehaviourType.Move)
                            {
                                Assert.IsTrue(!model.HasPerformedAction, "!model.HasPerformedAction");
                                actorBehaviourInvoker.InvokeAsync(
                                        model,
                                        behaviour,
                                        dungeonModel,
                                        cancellationToken
                                    )
                                    .Forget();
                            }
                            // アクションだった場合はリストに追加しておく
                            else
                            {
                                model.HasPerformedAction = true;
                                actionBehaviours.Add((model, behaviour));
                            }
                        }
                    }

                    // 移動が終わるのを待つ
                    await UniTask.WhenAll(
                        dungeonModel.AllModels.Select(x => x.WaitUntilIdleAsync())
                    );
                    
                    // アクションを実行する
                    foreach (var (model, behaviour) in actionBehaviours)
                    {
                        await actorBehaviourInvoker.InvokeAsync(
                            model,
                            behaviour,
                            dungeonModel,
                            cancellationToken
                        );
                    }

                    // ターン終了処理
                    {
                        foreach (var model in dungeonModel.AllModels)
                        {
                            model.MovedRoutes.Clear();
                        }
                    
                        Debug.Log("Turn End");
                    }
                }
                
                void CreateScope()
                {
                    actorScope?.Cancel();
                    actorScope?.Dispose();
                    actorScope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                }
                
                void ClearScope()
                {
                    actorScope?.Cancel();
                    actorScope?.Dispose();
                    actorScope = null;
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