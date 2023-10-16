using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers.AISystems;
using UnityEngine;

namespace OTOGIRI.ActorControllers
{
    /// <summary>
    /// アクターのモデル
    /// </summary>
    public class ActorModel
    {
        /// <summary>
        /// アクターの名前を取得します。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// アクターの位置を取得または設定します。
        /// </summary>
        public Vector2Int Position
        {
            set => position.Value = value;
            get => position.Value;
        }

        /// <summary>
        /// アクターの位置プロパティを取得します。
        /// </summary>
        public IReadOnlyAsyncReactiveProperty<Vector2Int> PositionAsReactiveProperty() => position;

        /// <summary>
        /// アクターの位置を設定します。
        /// </summary>
        private AsyncReactiveProperty<Vector2Int> position = new AsyncReactiveProperty<Vector2Int>(Vector2Int.zero);

        /// <summary>
        /// アクターのAIを取得または設定します。
        /// </summary>
        public IActorAI AI { get; set; }

        /// <summary>
        /// アクターの行動力を取得または設定します。
        /// </summary>
        /// <remarks>
        /// この値が0になると行動できなくなる
        /// 基本的にアクションを行うと1ポイント消費する
        /// 鈍足状態の場合は2ポイント消費する
        /// ターンが終了した際に基本的に1ポイント回復する
        /// </remarks>
        public int ActionPoint { get; set; }

        /// <summary>
        /// アクターの回復行動力を取得または設定します。
        /// </summary>
        public int RecoveryActionPoint { get; set; } = 1;

        public ActorModel(string name, IActorAI ai, Vector2Int initialPosition)
        {
            Name = name;
            AI = ai;
            Position = initialPosition;
        }
    }
}