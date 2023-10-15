using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers.Behaviours;

namespace OTOGIRI.ActorControllers.AISystems
{
    public interface IActorAI
    {
        /// <summary>
        /// ターンの行動を決定する
        /// </summary>
        UniTask<IActorBehaviour> ThinkAsync(ActorModel actorModel, CancellationToken cancellationToken);
    }
}