using System.Threading;
using Cysharp.Threading.Tasks;

namespace OTOGIRI.ActorControllers.AISystems
{
    public interface IActorAI
    {
        /// <summary>
        /// ターンの行動を決定する
        /// </summary>
        UniTask ThinkAsync(ActorModel actorModel, CancellationToken cancellationToken);
    }
}