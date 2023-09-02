using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using OTOGIRI.DungeonSystems;
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
        
        private void Start()
        {
            var dungeon = this.dungeonBuilder.Build();
            
            var mapUIPresenter = new MapUIPresenter(this.mapUIViewPrefab);
            mapUIPresenter.PresentationAsync(this.destroyCancellationToken)
                .Forget();
            
            mapUIPresenter.CreateMap(dungeon);

            this.GetAsyncUpdateTrigger()
                .Subscribe(_ =>
                {
                    if (Keyboard.current[Key.Q].wasPressedThisFrame)
                    {
                        const int x = 0;
                        const int y = 0;
                        dungeon.Cells[y, x] = dungeon.Cells[y, x] == Define.CellType.Ground ? Define.CellType.Wall : Define.CellType.Ground;
                        mapUIPresenter.UpdateCell(dungeon, new Vector2Int(x, y), dungeon.Cells[y, x]);
                    }
                    if (Keyboard.current[Key.W].isPressed)
                    {
                        dungeon = this.dungeonBuilder.Build();
                        mapUIPresenter.CreateMap(dungeon);
                    }
                })
                .AddTo(this.destroyCancellationToken);
        }
    }
}
