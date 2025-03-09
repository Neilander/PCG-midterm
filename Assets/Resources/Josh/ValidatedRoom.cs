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

    public bool meetsConstraint(ExitConstraint requiredExits)
    {
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

    public bool Search(Vector2Int startingNode, Vector2Int targetNode)
    {
        //LoadData();
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closeSet = new List<Vector2Int>();
        Vector2Int currentNode;

        openSet.Add(startingNode);
        return false;
        while (openSet.Count > 0)
        {
            currentNode = openSet[0];
            openSet.RemoveAt(0);
            closeSet.Add(currentNode);
            if (currentNode == targetNode)
                return true;

            Vector2Int leftNeighbor = new Vector2Int(currentNode.x - 1, currentNode.y);
            Vector2Int rightNeighbor = new Vector2Int(currentNode.x + 1, currentNode.y);
            Vector2Int UpNeighbor = new Vector2Int(currentNode.x, currentNode.y+1);
            Vector2Int DownNeighbor = new Vector2Int(currentNode.x, currentNode.y-1);
            SafeAdd(openSet,closeSet, leftNeighbor);
            SafeAdd(openSet, closeSet, rightNeighbor);
            SafeAdd(openSet, closeSet, UpNeighbor);
            SafeAdd(openSet, closeSet, DownNeighbor);

        }
    }

    private void SafeAdd(List<Vector2Int> openSet,List<Vector2Int>closeSet, Vector2Int toAdd)
    {
        if (toAdd.x >= 0 && toAdd.x < LevelGenerator.ROOM_WIDTH && toAdd.y >= 0 && toAdd.y < LevelGenerator.ROOM_HEIGHT )
        {
            if(!openSet.Contains(toAdd)&&!closeSet.Contains(toAdd))
                openSet.Add(toAdd);
        }
    }
}
