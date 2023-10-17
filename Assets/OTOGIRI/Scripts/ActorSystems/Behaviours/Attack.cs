using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.GameSystems;
using UnityEngine;

namespace OTOGIRI.ActorControllers.Behaviours
{
    public class Attack : ActorBehaviour
    {
        public Attack(int consumeActionPoint)
            : base(Define.ActorBehaviourType.Action, consumeActionPoint)
        {
        }

        public override UniTask ExecuteAsync(ActorModel actorModel, GameModel gameModel, CancellationToken cancellationToken)
        {
            // TODO: Implement attack logic
            Debug.Log($"{actorModel.Name}: Attack");
            return UniTask.CompletedTask;
        }
    }
}