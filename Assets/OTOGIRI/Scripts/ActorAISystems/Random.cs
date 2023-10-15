using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers.Behaviours;

namespace OTOGIRI.ActorControllers.AISystems
{
    public class Random : IActorAI
    {
        public UniTask<IActorBehaviour> ThinkAsync(ActorModel actorModel, CancellationToken cancellationToken)
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                return UniTask.FromResult<IActorBehaviour>(new Move(1, Define.Direction.Up));
            }
            else
            {
                return UniTask.FromResult<IActorBehaviour>(new Attack(1));
            }
        }
    }
}