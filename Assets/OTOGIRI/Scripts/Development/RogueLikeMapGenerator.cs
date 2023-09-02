using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    private int splitRandomRange;

    [SerializeField]
    private int roomRandomRange;

    [SerializeField]
    private RawImage image;

    [SerializeField]
    private bool isStepCorridorProcess;

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
            foreach (var neighborChunk in this.chunks)
            {
                if (chunk == neighborChunk)
                {
                    continue;
                }

                if (neighborChunk.neighbors.Find(x => x.chunk == chunk) != null)
                {
                    continue;
                }

                var isNeighbor = false;

                // 上方向に隣接しているか
                for (var x = chunk.raw.x; x < chunk.raw.x + chunk.raw.width; x++)
                {
                    var step = 1;
                    while (!isNeighbor)
                    {
                        var point = new Vector2Int(
                            x,
                            chunk.raw.y - step
                        );
                        step++;

                        if (point.x < 0 || point.x >= this.mapSize.x || point.y < 0 || point.y >= this.mapSize.y)
                        {
                            break;
                        }

                        var isDetectOtherChunk = false;
                        foreach (var otherChunk in this.chunks)
                        {
                            if (otherChunk.raw.Contains(point))
                            {
                                if (otherChunk == neighborChunk)
                                {
                                    chunk.neighbors.Add(new NeighborChunkData
                                    {
                                        chunk = neighborChunk,
                                        direction = Direction.Up,
                                    });
                                    isNeighbor = true;
                                    break;
                                }
                                else
                                {
                                    isDetectOtherChunk = true;
                                    break;
                                }
                            }
                        }
                        if (isDetectOtherChunk)
                        {
                            break;
                        }
                    }

                    if (isNeighbor)
                    {
                        break;
                    }
                }

                // 隣接している場合はもう計算しなくていい
                if (isNeighbor)
                {
                    break;
                }

                // 下方向に隣接しているか
                for (var x = chunk.raw.x; x < chunk.raw.x + chunk.raw.width; x++)
                {
                    var step = 1;
                    while (!isNeighbor)
                    {
                        var point = new Vector2Int(
                            x,
                            chunk.raw.y + chunk.raw.height + step
                        );
                        step++;

                        if (point.x < 0 || point.x >= this.mapSize.x || point.y < 0 || point.y >= this.mapSize.y)
                        {
                            break;
                        }

                        var isDetectOtherChunk = false;
                        foreach (var otherChunk in this.chunks)
                        {
                            if (otherChunk.raw.Contains(point))
                            {
                                if (otherChunk == neighborChunk)
                                {
                                    chunk.neighbors.Add(new NeighborChunkData
                                    {
                                        chunk = neighborChunk,
                                        direction = Direction.Down,
                                    });
                                    isNeighbor = true;
                                    break;
                                }
                                else
                                {
                                    isDetectOtherChunk = true;
                                    break;
                                }
                            }
                        }
                        if (isDetectOtherChunk)
                        {
                            break;
                        }
                    }

                    if (isNeighbor)
                    {
                        break;
                    }
                }

                // 隣接している場合はもう計算しなくていい
                if (isNeighbor)
                {
                    break;
                }

                // 左方向に隣接しているか
                for (var y = chunk.raw.y; y < chunk.raw.y + chunk.raw.height; y++)
                {
                    var step = 1;
                    while (!isNeighbor)
                    {
                        var point = new Vector2Int(
                            chunk.raw.x - step,
                            y
                        );
                        step++;

                        if (point.x < 0 || point.x >= this.mapSize.x || point.y < 0 || point.y >= this.mapSize.y)
                        {
                            break;
                        }

                        var isDetectOtherChunk = false;
                        foreach (var otherChunk in this.chunks)
                        {
                            if (otherChunk.raw.Contains(point))
                            {
                                if (otherChunk == neighborChunk)
                                {
                                    chunk.neighbors.Add(new NeighborChunkData
                                    {
                                        chunk = neighborChunk,
                                        direction = Direction.Left,
                                    });
                                    isNeighbor = true;
                                    break;
                                }
                                else
                                {
                                    isDetectOtherChunk = true;
                                    break;
                                }
                            }
                        }
                        if (isDetectOtherChunk)
                        {
                            break;
                        }
                    }

                    if (isNeighbor)
                    {
                        break;
                    }
                }

                // 隣接している場合はもう計算しなくていい
                if (isNeighbor)
                {
                    break;
                }

                // 右方向に隣接しているか
                for (var y = chunk.raw.y; y < chunk.raw.y + chunk.raw.height; y++)
                {
                    var step = 1;
                    while (!isNeighbor)
                    {
                        var point = new Vector2Int(
                            chunk.raw.x + chunk.raw.width + step,
                            y
                        );
                        step++;

                        if (point.x < 0 || point.x >= this.mapSize.x || point.y < 0 || point.y >= this.mapSize.y)
                        {
                            break;
                        }

                        var isDetectOtherChunk = false;
                        foreach (var otherChunk in this.chunks)
                        {
                            if (otherChunk.raw.Contains(point))
                            {
                                if (otherChunk == neighborChunk)
                                {
                                    chunk.neighbors.Add(new NeighborChunkData
                                    {
                                        chunk = neighborChunk,
                                        direction = Direction.Right,
                                    });
                                    isNeighbor = true;
                                    break;
                                }
                                else
                                {
                                    isDetectOtherChunk = true;
                                    break;
                                }
                            }
                        }
                        if (isDetectOtherChunk)
                        {
                            break;
                        }
                    }

                    if (isNeighbor)
                    {
                        break;
                    }
                }
            }
        }

        // 廊下を作る
        foreach (var chunk in this.chunks)
        {
            foreach (var neighbor in chunk.neighbors)
            {
                var d = neighbor.direction;
                var id = d switch
                {
                    Direction.Up => Direction.Down,
                    Direction.Down => Direction.Up,
                    Direction.Left => Direction.Right,
                    Direction.Right => Direction.Left,
                    _ => throw new ArgumentOutOfRangeException()
                };
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
                        currentPosition += currentDirection switch
                        {
                            Direction.Up => Vector2Int.down,
                            Direction.Down => Vector2Int.up,
                            Direction.Left => Vector2Int.left,
                            Direction.Right => Vector2Int.right,
                            _ => throw new ArgumentOutOfRangeException()
                        };
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
                        if (currentDirection == Direction.Up || currentDirection == Direction.Down)
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
        var isChunk = false;
        if (this.selectedChunk != null)
        {
            isChunk = this.selectedChunk.raw.Contains(new Vector2Int(x, y));
        }
        var color = cellType == CellType.Wall ? this.wallColor : isRoom ? this.roomColor : this.corridorColor;
        if (isChunk)
        {
            color *= this.chunkColor;
        }
        this.texture.SetPixel(x, this.mapSize.y - y - 1, color);
    }

    public enum CellType
    {
        Wall,
        Ground,
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
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
        /// 隣接しているチャンク
        /// </summary>
        public List<NeighborChunkData> neighbors = new();
    }

    public class NeighborChunkData
    {
        /// <summary>
        /// 隣接しているチャンク
        /// </summary>
        public Chunk chunk;

        /// <summary>
        /// 向き
        /// </summary>
        public Direction direction;
    }
}
