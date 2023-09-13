using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using OTOGIRI.ActorControllers.Behaviours;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OTOGIRI.ActorControllers.AISystems
{
    public class Input : IActorAI
    {
        public async UniTask ThinkAsync(ActorModel actorModel, CancellationToken cancellationToken)
        {
            var completionSource = new UniTaskCompletionSource();
            AsyncTriggerGameObject.GetAsyncUpdateTrigger()
                .Subscribe(_ =>
                {
                    var k = Keyboard.current;
                    var vector = new Vector2Int
                    {
                        x = k.rightArrowKey.isPressed ? 1 : k.leftArrowKey.isPressed ? -1 : 0,
                        y = k.upArrowKey.isPressed ? 1 : k.downArrowKey.isPressed ? -1 : 0
                    };

                    if (vector != Vector2Int.zero)
                    {
                        actorModel.NextBehaviour = new Move(1, vector.ToDirection());
                        completionSource.TrySetResult();
                    }
                    else if (k.spaceKey.isPressed)
                    {
                        actorModel.NextBehaviour = new Attack(1);
                        completionSource.TrySetResult();
                    }
                })
                .AddTo(cancellationToken);
            
            await completionSource.Task;
        }
    }
}