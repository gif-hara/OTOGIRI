using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers.AISystems;
using OTOGIRI.ActorControllers.Behaviours;
using UnityEngine;

namespace OTOGIRI.ActorControllers
{
    public class ActorModel
    {
        public string Name { get; }
        
        public Vector2Int Position { get; set; }
        
        /// <summary>
        /// ターン中に移動したルートのリスト
        /// </summary>
        public List<Define.Direction> MovedRoutes { get; } = new();
        
        public IActorAI AI { get; set; }

        /// <summary>
        /// 行動力
        /// </summary>
        /// <remarks>
        /// この値が0になると行動できなくなる
        /// 基本的にアクションを行うと1ポイント消費する
        /// 鈍足状態の場合は2ポイント消費する
        /// ターンが終了した際に基本的に1ポイント回復する
        /// </remarks>
        public int ActionPoint { get; set; }

        /// <summary>
        /// 回復する行動力
        /// </summary>
        public int RecoveryActionPoint { get; set; } = 1;
        
        public bool IsBusy { get; set; }
        
        public IActorBehaviour NextBehaviour { get; set; }
        
        /// <summary>
        /// アクションを実行したかどうか
        /// </summary>
        public bool HasPerformedAction { get; set; }
        
        public ActorModel(string name, IActorAI ai)
        {
            this.Name = name;
            this.AI = ai;
        }
        
        /// <summary>
        /// アイドル状態になるまで待機する
        /// </summary>
        public UniTask WaitUntilIdleAsync()
        {
            if (!this.IsBusy)
            {
                return UniTask.CompletedTask;
            }
            return UniTask.WaitUntil(() => !this.IsBusy);
        }
    }
}