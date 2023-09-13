using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers.Behaviours;

namespace OTOGIRI.ActorControllers.AISystems
{
    public class Random : IActorAI
    {
        public UniTask ThinkAsync(ActorModel actorModel, CancellationToken cancellationToken)
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                actorModel.NextBehaviour = new Move(1, Define.Direction.Up);
                return UniTask.CompletedTask;
            }
            else
            {
                actorModel.NextBehaviour = new Attack(1);
                return UniTask.CompletedTask;
            }
        }
    }
}