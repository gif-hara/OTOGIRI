using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class BSPDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    private RawImage dungeonImage;

    [Header("Colors")]
    [SerializeField]
    private Color wallColor = Color.black;
    [SerializeField]
    private Color roomColor = Color.white;
    [SerializeField]
    private Color corridorColor = Color.gray;

    [Header("Sizes")]
    [SerializeField]
    private Vector2Int dungeonSize = new Vector2Int(100, 100);

    [Header("Split Settings")]
    [SerializeField]
    private int maxSplitDepth = 4; // 分割の最大回数

    [SerializeField]
    private int splitRandomRange = 10;

    private Texture2D dungeonTexture;

    private void Start()
    {
        Generate(dungeonSize.x, dungeonSize.y);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Generate(dungeonSize.x, dungeonSize.y);
        }
    }

    public BSPTree Generate(int width, int height)
    {
        dungeonTexture = new Texture2D(width, height)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                dungeonTexture.SetPixel(x, y, wallColor);
            }
        }

        BSPTree root = Split(null, new RectInt(0, 0, width, height));
        CreateRooms(root);
        ConnectRooms(root);
        DrawCorridors(root);
        DrawRooms(root);

        dungeonTexture.Apply();
        dungeonImage.texture = dungeonTexture;

        return root;
    }

    private BSPTree Split(BSPTree parent, RectInt area, int depth = 0)
    {
        BSPTree node = new()
        {
            Parent = parent,
            Room = area
        };

        // 既に深さが最大値に達している場合、分割を終了
        if (depth >= maxSplitDepth)
            return node;

        bool splitH = Random.value > 0.5f;

        if (area.width > area.height && (float)area.width / area.height >= 1.5)
            splitH = false;
        else if (area.height > area.width && (float)area.height / area.width >= 1.5)
            splitH = true;

        int max = (splitH ? area.height : area.width) / 2;
        int split = Random.Range(max - this.splitRandomRange, max + this.splitRandomRange);
        node.splitIndex = split;
        node.isSplitHorizontal = splitH;

        // 子ノードの分割時には現在の深さを+1して渡す
        if (splitH)
        {
            node.LeftChild = Split(node, new RectInt(area.x, area.y, area.width, split), ++depth);
            node.RightChild = Split(node, new RectInt(area.x, area.y + split, area.width, area.height - split), ++depth);
        }
        else
        {
            node.LeftChild = Split(node, new RectInt(area.x, area.y, split, area.height), ++depth);
            node.RightChild = Split(node, new RectInt(area.x + split, area.y, area.width - split, area.height), ++depth);
        }

        return node;
    }

    private void CreateRooms(BSPTree node)
    {
        if (node == null)
        {
            return;
        }

        if (node.LeftChild != null || node.RightChild != null)
        {
            // First, recurse and create rooms for children.
            CreateRooms(node.LeftChild);
            CreateRooms(node.RightChild);
        }

        RectInt r = node.Room;
        node.Room = new RectInt(r.x + 3, r.y + 3, r.width - 6, r.height - 6);
    }

    private void ConnectRooms(BSPTree node)
    {
        if (node == null)
        {
            return;
        }

        ConnectRooms(node.LeftChild, node.RightChild, node.splitIndex, node.isSplitHorizontal, node.Corridors);
        // if (node.Parent != null)
        // {
        //     ConnectRooms(node.Parent, node.Parent.LeftChild, node.splitIndex, node.isSplitHorizontal, node.Corridors);
        //     ConnectRooms(node.Parent, node.Parent.RightChild, node.splitIndex, node.isSplitHorizontal, node.Corridors);
        // }
        if (node.LeftChild != null) ConnectRooms(node.LeftChild);
        if (node.RightChild != null) ConnectRooms(node.RightChild);
    }

    private void ConnectRooms(BSPTree left, BSPTree right, int splitIndex, bool isSplitHorizontal, List<RectInt> corridors)
    {
        if (left == null || right == null)
        {
            return;
        }

        Vector2Int leftCenter = left.GetCenter();
        Vector2Int rightCenter = right.GetCenter();
        if (isSplitHorizontal)
        {
            var leftX = leftCenter.x;
            corridors.Add(new RectInt(
                leftX,
                leftCenter.y > splitIndex ? splitIndex : leftCenter.y,
                1,
                Mathf.Abs(splitIndex - leftCenter.y) + 1
                ));
            var rightX = rightCenter.x;
            corridors.Add(new RectInt(
                rightX,
                rightCenter.y > splitIndex ? splitIndex : rightCenter.y,
                1,
                Mathf.Abs(splitIndex - rightCenter.y) + 1
                ));
            corridors.Add(new RectInt(
                    leftX > rightX ? rightX : leftX,
                    splitIndex,
                    Mathf.Abs(leftX - rightX) + 1,
                    1
                    ));
        }
        else
        {
            var leftY = leftCenter.y;
            corridors.Add(new RectInt(
                leftCenter.x > splitIndex ? splitIndex : leftCenter.x,
                leftY,
                Mathf.Abs(splitIndex - leftCenter.x) + 1,
                1
                ));
            var rightY = rightCenter.y;
            corridors.Add(new RectInt(
                rightCenter.x > splitIndex ? splitIndex : rightCenter.x,
                rightY,
                Mathf.Abs(splitIndex - rightCenter.x) + 1,
                1
                ));
            corridors.Add(new RectInt(
                    splitIndex,
                    leftY > rightY ? rightY : leftY,
                    1,
                    Mathf.Abs(leftY - rightY) + 1
                    ));
        }
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
            DrawRooms(node.LeftChild);
            DrawRooms(node.RightChild);
        }
    }

    private void DrawCorridors(BSPTree node)
    {
        if (node == null) return;

        if (node.LeftChild == null && node.RightChild == null)
        {
            return;
        }

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

        DrawCorridors(node.LeftChild);
        DrawCorridors(node.RightChild);
    }
}

public class BSPTree
{
    public BSPTree Parent;
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
