using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers.Behaviours;

namespace OTOGIRI.ActorControllers.BehaviourInvokers
{
    public interface IActorBehaviourInvoker
    {
        /// <summary>
        /// <paramref name="actorModel"/>の状態を<paramref name="dungeonModel"/>に反映する
        /// </summary>
        /// <returns></returns>
        UniTask InvokeAsync(ActorModel actorModel, IActorBehaviour behaviour, DungeonModel dungeonModel, CancellationToken cancellationToken);
    }
}