using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers.Behaviours;
using UnityEngine;

namespace OTOGIRI.ActorControllers.BehaviourInvokers
{
    public class Log : IActorBehaviourInvoker
    {
        public async UniTask InvokeAsync(ActorModel actorModel, IActorBehaviour behaviour, DungeonModel dungeonModel, CancellationToken cancellationToken)
        {
            if (behaviour.BehaviourType == Define.ActorBehaviourType.Move)
            {
                Debug.Log($"{actorModel.Name}: Move");
                behaviour.Execute(actorModel, dungeonModel);
                await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: cancellationToken);
            }
            else
            {
                Debug.Log($"{actorModel.Name}: Action");
                behaviour.Execute(actorModel, dungeonModel);
                await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: cancellationToken);
            }
        }
    }
}