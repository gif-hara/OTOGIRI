using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using OTOGIRI.DungeonSystems;
using OTOGIRI.Scripts;
using OTOGIRI.UISystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OTOGIRI.Development
{
    public class TestSceneController : MonoBehaviour
    {
        [SerializeField]
        private DungeonBuilderScriptableObject dungeonBuilder;
        
        [SerializeField]
        private MapUIView mapUIViewPrefab;
        
        private async void Start()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: this.destroyCancellationToken);
            
            var dungeon = this.dungeonBuilder.Build();
            dungeon.Print();
            
            var mapUIPresenter = new MapUIPresenter(this.mapUIViewPrefab);
            mapUIPresenter.PresentationAsync(this.destroyCancellationToken)
                .Forget();
            
            mapUIPresenter.CreateMap(dungeon);

            this.GetAsyncUpdateTrigger()
                .Subscribe(_ =>
                {
                    if (Keyboard.current[Key.Q].wasPressedThisFrame)
                    {
                        const int x = 3;
                        const int y = 4;
                        dungeon.Cells[y, x] = dungeon.Cells[y, x] == Define.CellType.Ground ? Define.CellType.Wall : Define.CellType.Ground;
                        mapUIPresenter.UpdateCell(new Vector2Int(x, y), dungeon.Cells[y, x]);
                    }
                    if (Keyboard.current[Key.W].wasPressedThisFrame)
                    {
                        dungeon = this.dungeonBuilder.Build();
                        mapUIPresenter.CreateMap(dungeon);
                    }
                })
                .AddTo(this.destroyCancellationToken);
        }
    }
}
