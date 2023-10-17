using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OTOGIRI.DungeonSystems
{
    /// <summary>
    /// ダンジョンのビュー
    /// </summary>
    public sealed class DungeonView : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();

        public DungeonView(DungeonModel dungeonModel)
        {
            DungeonEvents.SetMap
                .SubscribeWithKey(
                    dungeonModel,
                    message =>
                    {
                        var sb = new StringBuilder();
                        for (var y = 0; y < message.Map.GetLength(0); y++)
                        {
                            for (var x = 0; x < message.Map.GetLength(1); x++)
                            {
                                sb.Append((int)message.Map[x, y]);
                            }
                            sb.AppendLine();
                        }

                        Debug.Log(sb.ToString());
                    }
                )
                .AddTo(this.cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
        }
    }
}
