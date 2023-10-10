using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BSPDungeonGenerator : MonoBehaviour
{
    [SerializeField] private RawImage dungeonImage;
    
    [Header("Colors")]
    [SerializeField] private Color wallColor = Color.black;
    [SerializeField] private Color roomColor = Color.white;
    [SerializeField] private Color corridorColor = Color.gray;

    [Header("Sizes")]
    [SerializeField] private Vector2Int roomSize = new Vector2Int(6, 12);
    [SerializeField] private Vector2Int dungeonSize = new Vector2Int(100, 100);
    
    [Header("Split Settings")]
    [SerializeField] private int maxSplitDepth = 4; // 分割の最大回数

    private Texture2D dungeonTexture;

    private void Start()
    {
        Generate(dungeonSize.x, dungeonSize.y);
    }

    public BSPTree Generate(int width, int height)
    {
        dungeonTexture = new Texture2D(width, height);
        dungeonTexture.filterMode = FilterMode.Point;
        dungeonTexture.wrapMode = TextureWrapMode.Clamp;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                dungeonTexture.SetPixel(x, y, wallColor);
            }
        }

        BSPTree root = Split(new RectInt(0, 0, width, height));
        CreateRooms(root);
        DrawRooms(root);

        dungeonTexture.Apply();
        dungeonImage.texture = dungeonTexture;

        return root;
    }

    private BSPTree Split(RectInt area, int depth = 0) // 現在の深さを追加
    {
        BSPTree node = new BSPTree { Room = area };

        // 既に深さが最大値に達している場合、分割を終了
        if (depth >= maxSplitDepth)
            return node;

        bool splitH = Random.value > 0.5f;

        if (area.width > area.height && area.width / area.height >= 1.5)
            splitH = false;
        else if (area.height > area.width && area.height / area.width >= 1.5)
            splitH = true;

        int max = (splitH ? area.height : area.width) - roomSize.x;
        if (max <= roomSize.x)
            return node;

        int split = Random.Range(roomSize.x, max);
        node.splitIndex = split;
        node.isSplitHorizontal = splitH;

        // 子ノードの分割時には現在の深さを+1して渡す
        if (splitH)
        {
            node.LeftChild = Split(new RectInt(area.x, area.y, area.width, split), depth + 1);
            node.RightChild = Split(new RectInt(area.x, area.y + split, area.width, area.height - split), depth + 1);
        }
        else
        {
            node.LeftChild = Split(new RectInt(area.x, area.y, split, area.height), depth + 1);
            node.RightChild = Split(new RectInt(area.x + split, area.y, area.width - split, area.height), depth + 1);
        }

        return node;
    }

    private void CreateRooms(BSPTree node)
    {
        if (node == null)
            return;

        RectInt room = node.Room;

        if (node.LeftChild == null && node.RightChild == null)
        {
            int roomWidth = Random.Range(roomSize.x, Mathf.Min(room.width, roomSize.y));
            int roomHeight = Random.Range(roomSize.x, Mathf.Min(room.height, roomSize.y));
            int roomX = Random.Range(room.x, room.x + room.width - roomWidth);
            int roomY = Random.Range(room.y, room.y + room.height - roomHeight);
            node.Room = new RectInt(roomX, roomY, roomWidth, roomHeight);
        }
        else
        {
            CreateRooms(node.LeftChild);
            CreateRooms(node.RightChild);
            if (node.LeftChild != null && node.RightChild != null)
            {
                ConnectRooms(node);
            }
        }
    }

    private void ConnectRooms(BSPTree node)
    {
        if (node.LeftChild != null && node.RightChild != null)
        {
            Debug.Log($"splitIndex: {node.splitIndex}, isSplitHorizontal: {node.isSplitHorizontal}");
            Vector2Int leftCenter = node.LeftChild.GetCenter();
            Vector2Int rightCenter = node.RightChild.GetCenter();
            if (node.isSplitHorizontal)
            {
                node.Corridors.Add(new RectInt(
                    leftCenter.x,
                    leftCenter.y > node.splitIndex ? node.splitIndex : leftCenter.y,
                    1,
                    Mathf.Abs(node.splitIndex - leftCenter.y)
                    ));
                node.Corridors.Add(new RectInt(
                    rightCenter.x,
                    rightCenter.y > node.splitIndex ? node.splitIndex : rightCenter.y,
                    1,
                    Mathf.Abs(node.splitIndex - rightCenter.y)
                    ));
                node.Corridors.Add(new RectInt(
                        leftCenter.x > rightCenter.x ? rightCenter.x : leftCenter.x,
                        node.splitIndex,
                        Mathf.Abs(leftCenter.x - rightCenter.x),
                        1
                        ));
            }
            else
            {
                node.Corridors.Add(new RectInt(
                    leftCenter.x > node.splitIndex ? node.splitIndex : leftCenter.x,
                    leftCenter.y,
                    Mathf.Abs(node.splitIndex - leftCenter.x),
                    1
                    ));
                node.Corridors.Add(new RectInt(
                    rightCenter.x > node.splitIndex ? node.splitIndex : rightCenter.x,
                    rightCenter.y,
                    Mathf.Abs(node.splitIndex - rightCenter.x),
                    1
                    ));
                node.Corridors.Add(new RectInt(
                        node.splitIndex,
                        leftCenter.y > rightCenter.y ? rightCenter.y : leftCenter.y,
                        1,
                        Mathf.Abs(leftCenter.y - rightCenter.y)
                        ));
            }
        }

        if (node.LeftChild != null) ConnectRooms(node.LeftChild);
        if (node.RightChild != null) ConnectRooms(node.RightChild);
    }

    private void DrawRooms(BSPTree node)
    {
        if (node == null) return;

        if (node.LeftChild == null && node.RightChild == null)
        {
            for (int x = node.Room.x; x < node.Room.x + node.Room.width; x++)
            {
                for (int y = node.Room.y; y < node.Room.y + node.Room.height; y++)
                {
                    dungeonTexture.SetPixel(x, y, roomColor);
                }
            }
        }
        else
        {
            foreach (var corridor in node.Corridors)
            {
                for (int x = corridor.x; x < corridor.x + corridor.width; x++)
                {
                    for (int y = corridor.y; y < corridor.y + corridor.height; y++)
                    {
                        dungeonTexture.SetPixel(x, y, corridorColor);
                    }
                }
            }

            DrawRooms(node.LeftChild);
            DrawRooms(node.RightChild);
        }
    }
}

public class BSPTree
{
    public RectInt Room;
    public List<RectInt> Corridors = new List<RectInt>();
    public BSPTree LeftChild;
    public BSPTree RightChild;
    public int splitIndex = -1;
    public bool isSplitHorizontal;
    
    public Vector2Int GetCenter()
    {
        return new Vector2Int(Room.x + Room.width / 2, Room.y + Room.height / 2);
    }
}
