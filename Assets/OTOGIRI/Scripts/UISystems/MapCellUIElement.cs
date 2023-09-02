using UnityEngine;
using UnityEngine.UI;

namespace OTOGIRI.UISystems
{
    public class MapCellUIElement : MonoBehaviour
    {
        [SerializeField]
        private RectTransform root;
        
        [SerializeField]
        private Image image;

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
        
        public RectTransform Root => this.root;

        public void SetColor(Define.CellType cellType)
        {
            this.image.color = cellType switch
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
