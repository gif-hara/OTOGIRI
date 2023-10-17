
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers;
using UnityEngine;

namespace OTOGIRI.GameSystems
{
    /// <summary>
    /// ゲームのビュー
    /// </summary>
    public sealed class GameView : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();

        private readonly Dictionary<ActorModel, ActorView> actorViews = new();

        public GameView(GameModel gameModel)
        {
            GameEvents.AddedActorModel
                .SubscribeWithKey(
                    (IActorModelHolder)gameModel,
                    x => this.actorViews.Add(x.ActorModel, new ActorView(x.ActorModel))
                    )
                .AddTo(this.cancellationTokenSource.Token);

            GameEvents.RemovedActorModel
                .SubscribeWithKey(
                    (IActorModelHolder)gameModel,
                    x =>
                    {
                        this.actorViews[x.ActorModel].Dispose();
                        this.actorViews.Remove(x.ActorModel);
                    })
                .AddTo(this.cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            foreach (var actorView in this.actorViews)
            {
                actorView.Value.Dispose();
            }
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource?.Dispose();
        }
    }
}
