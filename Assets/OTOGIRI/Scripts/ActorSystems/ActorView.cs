using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace OTOGIRI.ActorControllers
{
    public class ActorView : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();

        public ActorView(ActorModel actorModel)
        {
            actorModel.PositionAsReactiveProperty()
                .Subscribe(position =>
                {
                    Debug.Log($"{actorModel.Name}: {position}");
                })
                .AddTo(cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
        }
    }
}
