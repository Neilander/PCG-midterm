using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class NeilRoomPlus : Room
{
    public List<TextAsset> designedRoomFiles;
    private List<SubRoomData> bag = new List<SubRoomData>();

    private void fillBag()
    {
        bag.Clear(); // 先清空 bag，避免重复添加

        foreach (TextAsset file in designedRoomFiles)
        {
            // 解析文件，获取 10x8 的 indexGrid
            int[,] indexGrid = ParseDesignedRoomFile(file, "RoomGenerator");

            // 生成 4 个子房间
            bag.Add(new SubRoomData(indexGrid, ifUp: true, ifRight: false));  // 左上
            bag.Add(new SubRoomData(indexGrid, ifUp: true, ifRight: true));   // 右上
            bag.Add(new SubRoomData(indexGrid, ifUp: false, ifRight: false)); // 左下
            bag.Add(new SubRoomData(indexGrid, ifUp: false, ifRight: true));  // 右下
        }

        Debug.Log($"Filled bag with {bag.Count} sub-rooms from {designedRoomFiles.Count} files.");
    }

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        fillBag();
        int maxAttempts = 20; // 最多尝试 20 次
        int[,] newRoomGrid = null;
        bool isValid = false;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // 打乱 bag
            List<SubRoomData> shuffledBag = new List<SubRoomData>(bag);
            ShuffleBySorting(shuffledBag);

            newRoomGrid = SubRoomData.RecomposeRoom(shuffledBag, requiredExits);

            // 检查是否连通
            if (CanReachAllExits(newRoomGrid, ourGenerator, requiredExits))
            {
                isValid = true;
                break; // 找到合格房间，跳出循环
            }
            
        }

        // 20 次都失败，报错
        if (!isValid)
        {
            throw new UnityException("Failed to generate a valid room after 20 attempts.");
        }

        // 这里可以用 newRoomGrid 进行后续房间构建逻辑
        Debug.Log("Successfully generated a valid room.");
        //base.fillRoom(ourGenerator, requiredExits);
        //Debug.Log(CanReachAllExits(ParseDesignedRoomFile(designedRoomFiles[0], "Neil"), ourGenerator, matrix.ToExitConstraint()));
        
        // Secret Room
        
    }

    public static bool CanReachAllExits(int[,] indexGrid, LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        int width = indexGrid.GetLength(0);
        int height = indexGrid.GetLength(1);
        bool[,] visited = new bool[width, height];

        // 获取所有必需的出口位置
        List<Vector2Int> requiredExitPositions = new List<Vector2Int>(requiredExits.requiredExitLocations());

        if (requiredExitPositions.Count == 0) return true; // 没有出口要求，视为连通

        // 选择一个出口作为 BFS 起点
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> reachedExits = new HashSet<Vector2Int>();

        Vector2Int startPos = requiredExitPositions[0]; // 任选一个出口作为起点
        queue.Enqueue(startPos);
        visited[startPos.x, startPos.y] = true;

        // BFS 方向数组 (上、右、下、左)
        Vector2Int[] directions = {
            new Vector2Int(0, 1),  // 上
            new Vector2Int(1, 0),  // 右
            new Vector2Int(0, -1), // 下
            new Vector2Int(-1, 0)  // 左
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // 如果当前点是必需出口之一，加入已到达集合
            if (requiredExitPositions.Contains(current))
            {
                reachedExits.Add(current);
                if (reachedExits.Count == requiredExitPositions.Count)
                {
                    return true; // 所有出口都能互相连通
                }
            }

            // 尝试向四个方向扩展
            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                // 检查是否越界
                if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                    continue;

                // 已访问过，跳过
                if (visited[next.x, next.y])
                    continue;

                // 获取 TileIndex 并检查是否是墙壁
                int tileIndex = indexGrid[next.x, next.y];
                if (tileIndex == 0) // 0 代表空地，可以通过
                {
                    queue.Enqueue(next);
                    visited[next.x, next.y] = true;
                }
                else
                {
                    // 通过 tileIndex 查找 Tile，判断是否是墙
                    GameObject tilePrefab = ourGenerator.globalTilePrefabs[tileIndex - 1];
                    Tile tile = tilePrefab.GetComponent<Tile>();

                    if (tile != null && !tile.hasTag(TileTags.Wall)) // 不是墙壁
                    {
                        queue.Enqueue(next);
                        visited[next.x, next.y] = true;
                    }
                }
            }
        }

        return false; // BFS 结束仍未能连接所有出口
    }

    public static int[,] ParseDesignedRoomFile(TextAsset designedRoomFile, string roomAuthor)
    {
        if (designedRoomFile == null)
        {
            throw new UnityException("Designed room file is null.");
        }

        // 读取文本内容并按行拆分
        string initialGridString = designedRoomFile.text;
        string[] rows = initialGridString.Trim().Split('\n');

        // 计算宽度和高度
        int width = rows[0].Trim().Split(',').Length;
        int height = rows.Length;

        // 校验高度
        if (height != LevelGenerator.ROOM_HEIGHT)
        {
            throw new UnityException(string.Format(
                "Error in room by {0}. Wrong height, Expected: {1}, Got: {2}",
                roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
        }

        // 校验宽度
        if (width != LevelGenerator.ROOM_WIDTH)
        {
            throw new UnityException(string.Format(
                "Error in room by {0}. Wrong width, Expected: {1}, Got: {2}",
                roomAuthor, LevelGenerator.ROOM_WIDTH, width));
        }

        // 创建 Grid 并填充
        int[,] indexGrid = new int[width, height];

        for (int r = 0; r < height; r++)
        {
            string row = rows[height - r - 1]; // 反向读取，使得 0,0 在左下角
            string[] cols = row.Trim().Split(',');

            for (int c = 0; c < width; c++)
            {
                indexGrid[c, r] = int.Parse(cols[c]);
            }
        }

        return indexGrid;
    }

    private void ShuffleBySorting<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        list.Sort((a, b) => rng.Next(-1, 2)); // -1, 0, 1 进行随机排序
    }
}

public struct SubRoomData2
{
    public int[,] grid; // 5x4 的 int 网格数据
    public bool topLeftCornerPassable;
    public bool topRightCornerPassable;
    public bool bottomLeftCornerPassable;
    public bool bottomRightCornerPassable;

    // 构造函数：从 10x8 的 `indexGrid` 提取 5x4
    public SubRoomData2(int[,] fullGrid, bool ifUp, bool ifRight)
    {
        grid = new int[5, 4];

        int startX = ifRight ? 5 : 0;  // ifRight 为 true 取右半部分，否则取左半部分
        int startY = ifUp ? 4 : 0;     // ifUp 为 true 取上半部分，否则取下半部分

        // 填充 5x4 子区域，从 `fullGrid`（10x8）中截取
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                grid[x, y] = fullGrid[startX + x, startY + y];
            }
        }

        // 角落通行判断：当 grid[x, y] == 0 时，表示可通过
        topLeftCornerPassable = grid[0, 3] == 0;  // 左上角
        topRightCornerPassable = grid[4, 3] == 0; // 右上角
        bottomLeftCornerPassable = grid[0, 0] == 0;  // 左下角
        bottomRightCornerPassable = grid[4, 0] == 0; // 右下角
    }

    // 调试输出
    public void DebugSubRoom()
    {
        Debug.Log("SubRoom Data:");
        Debug.Log($"Top Left Passable: {topLeftCornerPassable}");
        Debug.Log($"Top Right Passable: {topRightCornerPassable}");
        Debug.Log($"Bottom Left Passable: {bottomLeftCornerPassable}");
        Debug.Log($"Bottom Right Passable: {bottomRightCornerPassable}");
    }

    public static int[,] RecomposeRoom(List<SubRoomData2> shuffledSubRooms, ExitConstraint requiredExit)
    {
        if (shuffledSubRooms == null || shuffledSubRooms.Count < 4)
        {
            throw new UnityException("Not enough SubRoomData2 to generate a new room.");
        }

        List<SubRoomData2> selectedSubRooms = new List<SubRoomData2>(4); // 存储选中的4个片段

        // **确定每个 SubRoom 需要符合的通行条件**
        bool leftUp_ReqRight = requiredExit.upExitRequired;   // 左上：需要右上角可通
        bool leftUp_ReqDown = requiredExit.leftExitRequired;  // 左上：需要左下角可通

        bool rightUp_ReqLeft = requiredExit.upExitRequired;   // 右上：需要左上角可通
        bool rightUp_ReqDown = requiredExit.rightExitRequired; // 右上：需要右下角可通

        bool leftDown_ReqUp = requiredExit.leftExitRequired;  // 左下：需要左上角可通
        bool leftDown_ReqRight = requiredExit.downExitRequired; // 左下：需要右下角可通

        bool rightDown_ReqUp = requiredExit.rightExitRequired; // 右下：需要右上角可通
        bool rightDown_ReqLeft = requiredExit.downExitRequired; // 右下：需要左下角可通

        // **遍历所有 SubRoomData2，按需求选择**
        SubRoomData2? leftUp = null, rightUp = null, leftDown = null, rightDown = null;

        foreach (var subRoom in shuffledSubRooms)
        {
            if (!leftUp.HasValue && subRoom.topRightCornerPassable == leftUp_ReqRight && subRoom.bottomLeftCornerPassable == leftUp_ReqDown)
            {
                leftUp = subRoom;
            }
            else if (!rightUp.HasValue && subRoom.topLeftCornerPassable == rightUp_ReqLeft && subRoom.bottomRightCornerPassable == rightUp_ReqDown)
            {
                rightUp = subRoom;
            }
            else if (!leftDown.HasValue && subRoom.topLeftCornerPassable == leftDown_ReqUp && subRoom.bottomRightCornerPassable == leftDown_ReqRight)
            {
                leftDown = subRoom;
            }
            else if (!rightDown.HasValue && subRoom.topRightCornerPassable == rightDown_ReqUp && subRoom.bottomLeftCornerPassable == rightDown_ReqLeft)
            {
                rightDown = subRoom;
            }

            // 如果 4 片都找到，停止遍历
            if (leftUp.HasValue && rightUp.HasValue && leftDown.HasValue && rightDown.HasValue)
            {
                break;
            }
        }

        // **检查是否成功找到 4 片**
        if (!leftUp.HasValue || !rightUp.HasValue || !leftDown.HasValue || !rightDown.HasValue)
        {
            throw new UnityException("Failed to find suitable SubRoomData2 pieces to match the required exits.");
        }

        // **拼接 10x8 的房间**
        int[,] newRoomGrid = new int[10, 8];

        // 左上填充 (0,4)
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                newRoomGrid[x, y + 4] = leftUp.Value.grid[x, y];
            }
        }

        // 右上填充 (5,4)
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                newRoomGrid[x + 5, y + 4] = rightUp.Value.grid[x, y];
            }
        }

        // 左下填充 (0,0)
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                newRoomGrid[x, y] = leftDown.Value.grid[x, y];
            }
        }

        // 右下填充 (5,0)
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                newRoomGrid[x + 5, y] = rightDown.Value.grid[x, y];
            }
        }

        return newRoomGrid;
    }
}
