using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class RogueLikeMapGenerator : MonoBehaviour
{
    [SerializeField]
    private Vector2Int mapSize;

    [SerializeField]
    private Vector2Int roomCountRange;

    [SerializeField]
    private Color wallColor;

    [SerializeField]
    private Color roomColor;

    [SerializeField]
    private Color corridorColor;

    [SerializeField]
    private Color chunkColor;

    [SerializeField]
    private Color searchNeighborChunkColor;

    [SerializeField]
    private Color searchNeighborChunkPointColor;

    [SerializeField]
    private Color detectedNeighborChunkColor;

    [SerializeField]
    private int splitRandomRange;

    [SerializeField]
    private int roomRandomRange;

    [SerializeField]
    private RawImage image;

    [SerializeField]
    private bool isStepCorridorProcess;

    [SerializeField]
    private bool isStepNeighborChunkProcess;

    [SerializeField]
    private Camera uiCamera;

    private List<Chunk> chunks = new();

    private Chunk selectedChunk;

    private CellType[,] cells;

    /// <summary>
    /// 作成した部屋のリスト
    /// </summary>
    private readonly List<RectInt> rooms = new();

    /// <summary>
    /// ダンジョンデータを描画するテクスチャ
    /// </summary>
    private Texture2D texture;

    /// <summary>
    /// 生成中のCancellationTokenSource
    /// </summary>
    private CancellationTokenSource generateCancellationTokenSource;

    void Start()
    {
        this.Generate().Forget();
    }

    private void Update()
    {
        if (Keyboard.current[Key.Q].wasPressedThisFrame || Keyboard.current[Key.W].isPressed)
        {
            this.Generate().Forget();
        }

        if (Keyboard.current[Key.E].wasPressedThisFrame)
        {
            var x = 0;
            var y = 0;
            this.cells[y, x] = this.cells[y, x] == CellType.Wall ? CellType.Ground : CellType.Wall;
            this.UpdateTexture(x, y, cells[y, x]);
            this.texture.Apply();
        }

        // テクスチャをクリックしたらその場所の座標を表示する
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var rectTransform = this.image.rectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, this.uiCamera, out var localPoint))
            {
                // 左上を原点とする
                localPoint += new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);
                // Y座標は反転させる
                localPoint.y = rectTransform.rect.height - localPoint.y;
                // mapSizeに合わせて正規化する
                localPoint.x /= rectTransform.rect.width;
                localPoint.y /= rectTransform.rect.height;
                localPoint.x *= this.mapSize.x;
                localPoint.y *= this.mapSize.y;
                var x = Mathf.FloorToInt(localPoint.x);
                var y = Mathf.FloorToInt(localPoint.y);
                this.selectedChunk = this.chunks.Find(c => c.raw.Contains(new Vector2Int(x, y)));
                this.SetTexture();
            }
        }
    }

    private async UniTask Generate()
    {
        this.generateCancellationTokenSource?.Cancel();
        this.generateCancellationTokenSource?.Dispose();
        this.generateCancellationTokenSource = new CancellationTokenSource();

        this.rooms.Clear();
        this.cells = new CellType[this.mapSize.y, this.mapSize.x];
        this.selectedChunk = null;
        this.chunks = new List<Chunk>
        {
            new()
            {
                raw = new RectInt(0, 0, this.mapSize.x, this.mapSize.y),
            }
        };

        var roomCount = Random.Range(this.roomCountRange.x, this.roomCountRange.y + 1);
        for (var i = 0; i < roomCount - 1; i++)
        {
            var maxSize = 0;
            var targetIndex = -1;
            for (var j = 0; j < this.chunks.Count; j++)
            {
                var size = this.chunks[j].raw.width * this.chunks[j].raw.height;
                if (size > maxSize)
                {
                    maxSize = size;
                    targetIndex = j;
                }
            }

            var targetChunk = this.chunks[targetIndex];
            var isSplitHorizontal = targetChunk.raw.width < targetChunk.raw.height;
            var chunkA = new Chunk
            {
                raw = new RectInt(
                    targetChunk.raw.x,
                    targetChunk.raw.y,
                    isSplitHorizontal ? targetChunk.raw.width : targetChunk.raw.width / 2 + Random.Range(-this.splitRandomRange, this.splitRandomRange + 1),
                    isSplitHorizontal ? targetChunk.raw.height / 2 + Random.Range(-this.splitRandomRange, this.splitRandomRange + 1) : targetChunk.raw.height
                ),
            };
            var chunkB = new Chunk
            {
                raw = new RectInt(
                    isSplitHorizontal ? targetChunk.raw.x : chunkA.raw.x + chunkA.raw.width,
                    isSplitHorizontal ? chunkA.raw.y + chunkA.raw.height : targetChunk.raw.y,
                    isSplitHorizontal ? targetChunk.raw.width : targetChunk.raw.width - chunkA.raw.width,
                    isSplitHorizontal ? targetChunk.raw.height - chunkA.raw.height : targetChunk.raw.height
                ),
            };
            this.chunks.RemoveAt(targetIndex);
            this.chunks.Add(chunkA);
            this.chunks.Add(chunkB);
        }

        foreach (var chunk in this.chunks)
        {
            var xRange = Random.Range(0, this.roomRandomRange);
            var yRange = Random.Range(0, this.roomRandomRange);
            chunk.room = new RectInt(
                chunk.raw.x + 3 + xRange,
                chunk.raw.y + 3 + yRange,
                chunk.raw.width - 6 - xRange * 2 - Random.Range(0, this.roomRandomRange),
                chunk.raw.height - 6 - yRange * 2 - Random.Range(0, this.roomRandomRange)
            );
        }

        // 小さすぎる部屋は削除する
        this.chunks.RemoveAll(x => x.room.width < 4 || x.room.height < 4);

        // 部屋を作る
        foreach (var chunk in this.chunks)
        {
            this.rooms.Add(chunk.room);
            for (var y = chunk.room.y; y < chunk.room.y + chunk.room.height; y++)
            {
                for (var x = chunk.room.x; x < chunk.room.x + chunk.room.width; x++)
                {
                    cells[y, x] = CellType.Ground;
                }
            }
        }

        // 隣接しているチャンクを探す
        foreach (var chunk in this.chunks)
        {
            // 上方向に隣接しているチャンクを探す
            var upRect = new RectInt(chunk.raw.x, 0, chunk.raw.width, chunk.raw.y);
            if (upRect.width > 0 && upRect.height > 0)
            {
                var neighborChunks = new List<Chunk>();
                var minDistance = float.MaxValue;
                Chunk currentChunk = null;
                foreach (var c in this.chunks)
                {
                    if (c.raw.Overlaps(upRect))
                    {
                        var distance = (chunk.raw.center - c.raw.center).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            currentChunk = c;
                        }
                    }
                }
                if (currentChunk != null)
                {
                    chunk.neighborChunkDatas.Add(new NeighborChunkData
                    {
                        chunk = currentChunk,
                        direction = Direction.Up
                    });
                }
            }

            // 下方向に隣接しているチャンクを探す
            var downRect = new RectInt(chunk.raw.x, chunk.raw.y + chunk.raw.height, chunk.raw.width, this.mapSize.y - chunk.raw.y - chunk.raw.height);
            if (downRect.width > 0 && downRect.height > 0)
            {
                var neighborChunks = new List<Chunk>();
                var minDistance = float.MaxValue;
                Chunk currentChunk = null;
                foreach (var c in this.chunks)
                {
                    if (c.raw.Overlaps(downRect))
                    {
                        var distance = (chunk.raw.center - c.raw.center).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            currentChunk = c;
                        }
                    }
                }
                if (currentChunk != null)
                {
                    chunk.neighborChunkDatas.Add(new NeighborChunkData
                    {
                        chunk = currentChunk,
                        direction = Direction.Down
                    });
                }
            }

            // 左方向に隣接しているチャンクを探す
            var leftRect = new RectInt(0, chunk.raw.y, chunk.raw.x, chunk.raw.height);
            if (leftRect.width > 0 && leftRect.height > 0)
            {
                var neighborChunks = new List<Chunk>();
                var minDistance = float.MaxValue;
                Chunk currentChunk = null;
                foreach (var c in this.chunks)
                {
                    if (c.raw.Overlaps(leftRect))
                    {
                        var distance = (chunk.raw.center - c.raw.center).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            currentChunk = c;
                        }
                    }
                }
                if (currentChunk != null)
                {
                    chunk.neighborChunkDatas.Add(new NeighborChunkData
                    {
                        chunk = currentChunk,
                        direction = Direction.Left
                    });
                }
            }

            // 右方向に隣接しているチャンクを探す
            var rightRect = new RectInt(chunk.raw.x + chunk.raw.width, chunk.raw.y, this.mapSize.x - chunk.raw.x - chunk.raw.width, chunk.raw.height);
            if (rightRect.width > 0 && rightRect.height > 0)
            {
                var neighborChunks = new List<Chunk>();
                var minDistance = float.MaxValue;
                Chunk currentChunk = null;
                foreach (var c in this.chunks)
                {
                    if (c.raw.Overlaps(rightRect))
                    {
                        var distance = (chunk.raw.center - c.raw.center).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            currentChunk = c;
                        }
                    }
                }
                if (currentChunk != null)
                {
                    chunk.neighborChunkDatas.Add(new NeighborChunkData
                    {
                        chunk = currentChunk,
                        direction = Direction.Right
                    });
                }
            }
        }

        // 廊下を作る
        foreach (var chunk in this.chunks)
        {
            foreach (var neighbor in chunk.neighborChunkDatas)
            {
                var d = neighbor.direction;
                var id = d.Inverse();
                var start = new Vector2Int(
                    d switch
                    {
                        Direction.Left => chunk.room.x,
                        Direction.Right => chunk.room.x + chunk.room.width - 1,
                        _ => chunk.room.x + Random.Range(1, chunk.room.width - 1)
                    },
                    d switch
                    {
                        Direction.Up => chunk.room.y,
                        Direction.Down => chunk.room.y + chunk.room.height - 1,
                        _ => chunk.room.y + Random.Range(1, chunk.room.height - 1)
                    }
                );
                var currentPosition = start;
                var currentDirection = d;
                var wayPoints = new List<Vector2Int>
                {
                    // 自分のChunkの境界線
                    new(
                        d switch
                        {
                            Direction.Up => currentPosition.x,
                            Direction.Down => currentPosition.x,
                            Direction.Right => chunk.raw.x + chunk.raw.width - 1,
                            Direction.Left => chunk.raw.x,
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        d switch
                        {
                            Direction.Up => chunk.raw.y,
                            Direction.Down => chunk.raw.y + chunk.raw.height - 1,
                            Direction.Right => currentPosition.y,
                            Direction.Left => currentPosition.y,
                            _ => throw new ArgumentOutOfRangeException()
                        }
                    ),
                    new (
                        id switch
                        {
                            Direction.Left => neighbor.chunk.room.x - 1,
                            Direction.Right => neighbor.chunk.room.x + neighbor.chunk.room.width,
                            _ => neighbor.chunk.room.x + Random.Range(1, neighbor.chunk.room.width - 1)
                        },
                        id switch
                        {
                            Direction.Up => neighbor.chunk.room.y  -1,
                            Direction.Down => neighbor.chunk.room.y + neighbor.chunk.room.height,
                            _ => neighbor.chunk.room.y + Random.Range(1, neighbor.chunk.room.height - 1)
                        }
                    )
                };
                var currentWayPointIndex = 0;
                var currentWayPoint = wayPoints[currentWayPointIndex];
                while (currentWayPointIndex < wayPoints.Count)
                {
                    var canMove = true;
                    var isHorizontal = currentDirection == Direction.Left || currentDirection == Direction.Right;
                    var isVertical = currentDirection == Direction.Up || currentDirection == Direction.Down;
                    if (isHorizontal && currentPosition.x == currentWayPoint.x)
                    {
                        canMove = false;
                    }
                    else if (isVertical && currentPosition.y == currentWayPoint.y)
                    {
                        canMove = false;
                    }

                    if (canMove)
                    {
                        currentPosition += currentDirection.ToVector2Int();
                    }
                    if (currentPosition.x < 0 || currentPosition.x >= this.mapSize.x || currentPosition.y < 0 || currentPosition.y >= this.mapSize.y)
                    {
                        Debug.LogException(new Exception("Out of Range"));
                    }
                    cells[currentPosition.y, currentPosition.x] = CellType.Ground;
                    if (isVertical && currentPosition.y == currentWayPoint.y)
                    {
                        currentDirection = currentPosition.x < currentWayPoint.x ? Direction.Right : Direction.Left;
                    }
                    else if (isHorizontal && currentPosition.x == currentWayPoint.x)
                    {
                        currentDirection = currentPosition.y < currentWayPoint.y ? Direction.Down : Direction.Up;
                    }

                    if (currentPosition == currentWayPoint)
                    {
                        currentWayPointIndex++;
                        if (currentWayPointIndex >= wayPoints.Count)
                        {
                            break;
                        }
                        currentWayPoint = wayPoints[currentWayPointIndex];
                        if (currentDirection.IsVertical())
                        {
                            currentDirection = currentPosition.y < currentWayPoint.y ? Direction.Down : Direction.Up;
                        }
                        else
                        {
                            currentDirection = currentPosition.x < currentWayPoint.x ? Direction.Right : Direction.Left;
                        }
                    }

                    if (this.isStepCorridorProcess)
                    {
                        this.SetTexture();
                        await UniTask.WaitWhile(() => !Keyboard.current[Key.Space].isPressed, cancellationToken: this.generateCancellationTokenSource.Token);
                    }
                }
            }
        }
        this.SetTexture();
    }

    private void SetTexture()
    {
        // 描画
        this.texture = new Texture2D(this.mapSize.x, this.mapSize.y, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        for (var y = 0; y < this.mapSize.y; y++)
        {
            for (var x = 0; x < this.mapSize.x; x++)
            {
                this.UpdateTexture(x, y, cells[y, x]);
            }
        }

        this.texture.Apply();
        this.image.texture = this.texture;
    }

    private void UpdateTexture(int x, int y, CellType cellType)
    {
        var isRoom = false;
        foreach (var room in this.rooms)
        {
            if (room.Contains(new Vector2Int(x, y)))
            {
                isRoom = true;
                break;
            }
        }
        var isSelectedChunkPoint = false;
        if (this.selectedChunk != null)
        {
            isSelectedChunkPoint = this.selectedChunk.raw.Contains(new Vector2Int(x, y));
        }
        var color = cellType == CellType.Wall ? this.wallColor : isRoom ? this.roomColor : this.corridorColor;
        if (isSelectedChunkPoint)
        {
            color *= this.chunkColor;
        }
        this.UpdateTexture(x, y, color);
    }

    private void UpdateTexture(int x, int y, Color color)
    {
        this.texture.SetPixel(x, this.mapSize.y - y - 1, color);
    }

    private Color GetPixel(int x, int y)
    {
        return this.texture.GetPixel(x, this.mapSize.y - y - 1);
    }

    public enum CellType
    {
        Wall,
        Ground,
    }

    public class Dungeon
    {
        public List<Chunk> chunks = new();

        public List<RectInt> rooms = new();

        public CellType[,] cells;

        public Dungeon(List<Chunk> chunks, List<RectInt> rooms, CellType[,] cells)
        {
            this.chunks = chunks;
            this.rooms = rooms;
            this.cells = cells;
        }
    }

    public class Chunk
    {
        /// <summary>
        /// このチャンクの範囲
        /// </summary>
        public RectInt raw;

        /// <summary>
        /// 部屋の範囲
        /// </summary>
        public RectInt room;

        /// <summary>
        /// 隣接しているチャンクの情報
        /// </summary>
        public List<NeighborChunkData> neighborChunkDatas = new();
    }

    /// <summary>
    /// 隣接しているチャンクの情報
    /// </summary>
    public class NeighborChunkData
    {
        public Chunk chunk;

        public Direction direction;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

public enum Axis
{
    Horizontal,
    Vertical,
}

public static class Extensions
{
    public static Vector2Int ToVector2Int(this Direction direction) => direction switch
    {
        Direction.Up => Vector2Int.down,
        Direction.Down => Vector2Int.up,
        Direction.Left => Vector2Int.left,
        Direction.Right => Vector2Int.right,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    public static bool IsHorizontal(this Direction direction) => direction == Direction.Left || direction == Direction.Right;

    public static bool IsVertical(this Direction direction) => direction == Direction.Up || direction == Direction.Down;

    public static bool IsPositive(this Direction direction) => direction == Direction.Down || direction == Direction.Right;

    public static bool IsNegative(this Direction direction) => direction == Direction.Up || direction == Direction.Left;

    public static Direction Inverse(this Direction direction) => direction switch
    {
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    public static Direction RandomDirection(this Axis axis)
    {
        return axis == Axis.Horizontal ? Random.Range(0, 2) == 0 ? Direction.Left : Direction.Right : Random.Range(0, 2) == 0 ? Direction.Up : Direction.Down;
    }

    public static void Raycast(this Vector2Int position, Direction direction, int max, Func<Vector2Int, bool> predicate)
    {
        var i = direction.IsHorizontal() ? position.x : position.y;
        while (i >= 0 && i <= max)
        {
            var currentPosition = direction.IsHorizontal() ? new Vector2Int(i, position.y) : new Vector2Int(position.x, i);
            if (predicate(currentPosition))
            {
                break;
            }
            i += direction.IsPositive() ? 1 : -1;
        }
    }
}

