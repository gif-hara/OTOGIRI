using HK.Framework.UISystems;
using OTOGIRI.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace OTOGIRI.UISystems
{
    public class MapUIView : UIView<MapUIView>
    {
        [SerializeField]
        private RectTransform mapRoot;
        
        [SerializeField]
        private RawImage mapImage;
        
        [SerializeField]
        private Color abyssColor;
        
        [SerializeField]
        private Color waterwayColor;
        
        [SerializeField]
        private Color groundColor;
        
        [SerializeField]
        private Color stepColor;
        
        [SerializeField]
        private Color wallColor;
        
        private Texture2D mapTexture;
        
        public void CreateCell(Define.CellType[,] cells)
        {
            if (this.mapTexture != null)
            {
                Destroy(this.mapTexture);
            }
            this.mapTexture = new Texture2D(
                cells.GetLength(1),
                cells.GetLength(0),
                TextureFormat.RGBA32,
                false,
                true
                )
                {
                    filterMode = FilterMode.Point
                };
            this.mapImage.texture = this.mapTexture;
            for (var y = 0; y < cells.GetLength(0); y++)
            {
                for (var x = 0; x < cells.GetLength(1); x++)
                {
                    var cell = cells[y, x];
                    this.mapTexture.SetPixel(x, y, GetColor(cell));
                }
            }
            this.mapTexture.Apply();
        }
        
        public void UpdateCell(Vector2Int position, Define.CellType cellType)
        {
            this.mapTexture.SetPixel(position.x, position.y, GetColor(cellType));
            this.mapTexture.Apply();
        }

        private Color GetColor(Define.CellType cellType)
        {
            return cellType switch
            {
                Define.CellType.Abyss => this.abyssColor,
                Define.CellType.Waterway => this.waterwayColor,
                Define.CellType.Ground => this.groundColor,
                Define.CellType.Step => this.stepColor,
                Define.CellType.Wall => this.wallColor,
                _ => throw new System.NotImplementedException($"{cellType}は未実装です")
            };
        }
    }
}
