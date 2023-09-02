using HK.Framework.UISystems;
using OTOGIRI.DungeonSystems;
using UnityEngine;
using UnityEngine.UI;

namespace OTOGIRI.UISystems
{
    public class MapUIView : UIView<MapUIView>
    {
        [SerializeField]
        private RawImage mapImage;
        
        [SerializeField]
        private Color abyssColor;
        
        [SerializeField]
        private Color waterwayColor;
        
        [SerializeField]
        private Color stepColor;
        
        [SerializeField]
        private Color wallColor;
        
        [SerializeField]
        private Color corridorColor;
        
        [SerializeField]
        private Color roomColor;
        
        private Texture2D mapTexture;
        
        public void SetTexture(Texture2D texture)
        {
            this.mapTexture = texture;
            this.mapImage.texture = this.mapTexture;
        }
        
        public void CreateCell(Dungeon dungeon)
        {
            if (this.mapTexture != null)
            {
                Destroy(this.mapTexture);
            }
            this.mapTexture = new Texture2D(
                dungeon.Cells.GetLength(1),
                dungeon.Cells.GetLength(0),
                TextureFormat.RGBA32,
                false,
                true
                )
                {
                    filterMode = FilterMode.Point
                };
            this.mapImage.texture = this.mapTexture;
            for (var y = 0; y < dungeon.Cells.GetLength(0); y++)
            {
                for (var x = 0; x < dungeon.Cells.GetLength(1); x++)
                {
                    var cell = dungeon.Cells[y, x];
                    var position = new Vector2Int(x, y);
                    this.mapTexture.SetPixel(x, dungeon.Cells.GetLength(0) - 1 - y, GetColor(dungeon, position, cell));
                }
            }
            this.mapTexture.Apply();
        }
        
        public void UpdateCell(Dungeon dungeon, Vector2Int position, Define.CellType cellType)
        {
            this.mapTexture.SetPixel(position.x, dungeon.Cells.GetLength(0) - 1 - position.y, GetColor(dungeon, position, cellType));
            this.mapTexture.Apply();
        }

        private Color GetColor(Dungeon dungeon, Vector2Int position, Define.CellType cellType)
        {
            return cellType switch
            {
                Define.CellType.Abyss => this.abyssColor,
                Define.CellType.Waterway => this.waterwayColor,
                Define.CellType.Ground => dungeon.IsRoom(position) ? this.roomColor : this.corridorColor,
                Define.CellType.Step => this.stepColor,
                Define.CellType.Wall => this.wallColor,
                _ => throw new System.NotImplementedException($"{cellType}は未実装です")
            };
        }
    }
}
