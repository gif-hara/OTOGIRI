using System.Collections.Generic;
using OTOGIRI.ActorControllers;

namespace OTOGIRI.GameSystems
{
    /// <summary>
    /// ゲームの主要なデータを管理するクラス
    /// </summary>
    public class GameModel
    {
        /// <summary>
        /// ダンジョンモデルのインスタンスを取得します。
        /// </summary>
        public DungeonModel DungeonModel { get; } = new();

        /// <summary>
        /// アクターモデルのリストを取得します。
        /// </summary>
        public IReadOnlyList<ActorModel> ActorModels => actorModels;

        /// <summary>
        /// アクターモデルのプライベートリスト
        /// </summary>
        private readonly List<ActorModel> actorModels = new();

        public void AddActorModel(ActorModel model)
        {
            this.actorModels.Add(model);
            GameEvents.AddedActorModel.Publish(model);
        }

        public void RemoveActorModel(ActorModel model)
        {
            this.actorModels.Remove(model);
            GameEvents.RemovedActorModel.Publish(model);
        }
    }
}