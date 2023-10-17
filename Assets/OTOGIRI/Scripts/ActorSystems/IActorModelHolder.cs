using System.Collections.Generic;

namespace OTOGIRI.ActorControllers
{
    /// <summary>
    /// <see cref="ActorModel"/>のホルダー
    /// </summary>
    public interface IActorModelHolder
    {
        /// <summary>
        /// アクターモデルのリストを取得します。
        /// </summary>
        IReadOnlyList<ActorModel> ActorModels { get; }
    }
}