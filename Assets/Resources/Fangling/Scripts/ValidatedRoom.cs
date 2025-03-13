using UnityEngine;
using System.Collections.Generic;

public class ValidatedRoom : Room
{
    public bool hasUpExit;
    public bool hasDownExit;
    public bool hasLeftExit;
    public bool hasRightExit;

    public bool hasUpDownPath;
    public bool hasUpLeftPath;
    public bool hasUpRightPath;
    public bool hasDownLeftPath;
    public bool hasDownRightPath;
    public bool hasLeftRightPath;

    public GameObject movingWall;

    public bool Search(Vector2Int startingNode, Vector2Int targetNode)
    {
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closedSet = new List<Vector2Int>();
        Vector2Int currentNode;

        if (IsInBounds(startingNode) && IsEmpty(startingNode))
            openSet.Add(startingNode);

        while (openSet.Count > 0)
        {
            currentNode = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return true;

            //left
            Vector2Int neighbor = new Vector2Int(currentNode.x - 1, currentNode.y);
            if (IsInBounds(neighbor) && IsEmpty(neighbor)
                && openSet.Contains(neighbor) == false && closedSet.Contains(neighbor) == false)
            {
                openSet.Add(neighbor);
            }

            //right
            neighbor = new Vector2Int(currentNode.x + 1, currentNode.y);
            if (IsInBounds(neighbor) && IsEmpty(neighbor)
                && openSet.Contains(neighbor) == false && closedSet.Contains(neighbor) == false)
            {
                openSet.Add(neighbor);
            }

            //down
            neighbor = new Vector2Int(currentNode.x, currentNode.y - 1);
            if (IsInBounds(neighbor) && IsEmpty(neighbor)
                && openSet.Contains(neighbor) == false && closedSet.Contains(neighbor) == false)
            {
                openSet.Add(neighbor);
            }

            //up
            neighbor = new Vector2Int(currentNode.x, currentNode.y + 1);
            if (IsInBounds(neighbor) && IsEmpty(neighbor)
                && openSet.Contains(neighbor) == false && closedSet.Contains(neighbor) == false)
            {
                openSet.Add(neighbor);
            }
        }

        return false;
    }


    private static bool IsInBounds(Vector2Int node)
    {
        if (node.x >= 0 && node.x < LevelGenerator.ROOM_WIDTH &&
            node.y >= 0 && node.y < LevelGenerator.ROOM_HEIGHT)
        {
            return true;
        }
        else
            return false;
    }

    void ValidateExits()
        {
        LoadData();

        Vector2Int leftExit = new Vector2Int(0, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int rightExit = new Vector2Int(LevelGenerator.ROOM_WIDTH - 1, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int upExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, 0);
        Vector2Int downExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT - 1);

        hasLeftExit = IsEmpty(leftExit);
        hasRightExit = IsEmpty(rightExit);
        hasUpExit = IsEmpty(upExit);
        hasDownExit = IsEmpty(downExit);

        hasUpLeftPath = Search(upExit, leftExit);
        hasUpRightPath = Search(upExit, rightExit);
        hasUpDownPath = Search(upExit, downExit);
        hasLeftRightPath = Search(leftExit, rightExit);
        hasDownLeftPath = Search(downExit, leftExit);
        hasDownRightPath = Search(downExit, rightExit);
    }

    public bool meetsConstraint(ExitConstraint requiredExits)
    {
        ValidateExits();

        if (requiredExits.upExitRequired && hasUpExit == false)
            return false;

        if (requiredExits.downExitRequired && hasDownExit == false)
            return false;

        if (requiredExits.leftExitRequired && hasLeftExit == false)
            return false;

        if (requiredExits.rightExitRequired && hasRightExit == false)
            return false;

        if (requiredExits.upExitRequired && requiredExits.downExitRequired && hasUpDownPath == false)
            return false;

        if (requiredExits.upExitRequired && requiredExits.leftExitRequired && hasUpLeftPath == false)
            return false;

        if (requiredExits.upExitRequired && requiredExits.rightExitRequired && hasUpRightPath == false)
            return false;

        if (requiredExits.downExitRequired && requiredExits.leftExitRequired && hasDownLeftPath == false)
            return false;

        if (requiredExits.downExitRequired && requiredExits.rightExitRequired && hasDownRightPath == false)
            return false;

        if (requiredExits.leftExitRequired && requiredExits.rightExitRequired && hasLeftRightPath == false)
            return false;

        return true;
    }

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        LoadData();

        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                int tileIndex = indexGrid[i, j];
                GameObject tileToSpawn;
                if (tileIndex == 0)
                {
                    if (i == 0 || j == 0 || i == (LevelGenerator.ROOM_WIDTH - 1) || j == (LevelGenerator.ROOM_HEIGHT - 1))
                    {
                        if (Random.value > 0.7)
                        {
                            tileToSpawn = movingWall;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                        continue; // 0 is nothing.
                }
                else
                {
                    if (tileIndex < LevelGenerator.LOCAL_START_INDEX)
                    {
                        tileToSpawn = ourGenerator.globalTilePrefabs[tileIndex - 1];
                    }
                    else
                    {
                        int n = Random.Range(0, localTilePrefabs.Length);
                        tileToSpawn = localTilePrefabs[n];
                    }
                }
                Tile.spawnTile(tileToSpawn, transform, i, j);
            }
        }
    }
}
