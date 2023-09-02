using System.Threading;
using Cysharp.Threading.Tasks;
using HK.Framework.UISystems;
using OTOGIRI.DungeonSystems;
using UnityEngine;

namespace OTOGIRI.UISystems
{
    public class MapUIPresenter : UIPresenter
    {
        private readonly MapUIView mapUIViewPrefab;
        
        private MapUIView view;
        
        public MapUIPresenter(MapUIView mapUIViewPrefab)
        {
            this.mapUIViewPrefab = mapUIViewPrefab;
        }

        protected override async UniTask PresentationInternalAsync(CancellationToken cancellationToken)
        {
            this.view = this.mapUIViewPrefab.Register();
            
            this.view.ShowAsync().Forget();
            
            await UniTask.WaitUntilCanceled(cancellationToken);
            
            this.view.Unregister();
        }
        
        public void SetTexture(Texture2D texture)
        {
            this.view.SetTexture(texture);
        }

        public void CreateMap(Dungeon dungeon)
        {
            this.view.CreateCell(dungeon);
        }
        
        public void UpdateCell(Dungeon dungeon, Vector2Int position, Define.CellType cellType)
        {
            this.view.UpdateCell(dungeon, position, cellType);
        }
    }
}
