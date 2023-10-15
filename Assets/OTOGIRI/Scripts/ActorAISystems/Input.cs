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
        public UniTask<IActorBehaviour> ThinkAsync(ActorModel actorModel, CancellationToken cancellationToken)
        {
            Debug.Log("Input");
            var completionSource = new UniTaskCompletionSource<IActorBehaviour>();
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
                        completionSource.TrySetResult(new Move(1, vector.ToDirection()));
                    }
                    else if (k.spaceKey.isPressed)
                    {
                        completionSource.TrySetResult(new Attack(1));
                    }
                })
                .AddTo(cancellationToken);

            return completionSource.Task;
        }
    }
}