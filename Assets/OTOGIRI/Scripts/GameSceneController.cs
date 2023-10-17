using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OTOGIRI.ActorControllers;
using OTOGIRI.GameSystems;
using UnityEngine;

namespace OTOGIRI.SceneControllers
{
    public class GameSceneController : MonoBehaviour
    {
        private void Start()
        {
            BeginGameLoopAsync(this.destroyCancellationToken)
                .Forget();
        }

        private static async UniTask BeginGameLoopAsync(CancellationToken cancellationToken)
        {
            var gameModel = new GameModel();
            using var gameView = new GameView(gameModel);
            var gamePresenter = new GamePresenter();
            try
            {
                gameModel.AddActorModel(
                    new ActorModel("Player", new ActorControllers.AISystems.Input(), new Vector2Int(0, 0))
                );
                for (int i = 1; i <= 3; i++)
                {
                    gameModel.AddActorModel(
                        new ActorModel($"Enemy{i}", new ActorControllers.AISystems.Random(), new Vector2Int(0, 0))
                    );
                }
                var map = new Define.CellType[10, 10];
                for (var y = 0; y < map.GetLength(0); y++)
                {
                    for (var x = 0; x < map.GetLength(1); x++)
                    {
                        map[x, y] = Define.CellType.Ground;
                    }
                }
                gameModel.DungeonModel.SetMap(map);

                await gamePresenter.BeginGameLoopAsync(gameModel, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}