using UnityEngine;
using System.Collections.Generic;

public class zt649DFSRoom : Room
{
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        for (int r = 0; r < LevelGenerator.ROOM_HEIGHT; r++)
        {
            for (int c = 0; c < LevelGenerator.ROOM_WIDTH; c++)
            {
                /////////indexGrid[c, r] = 1;
            }
        }

        Vector2Int startNode = Vector2Int.zero;
        Vector2Int endNote = Vector2Int.zero;
        bool foundStart = false;

        foreach (Vector2Int exit in requiredExits.requiredExitLocations())
        {
            if (!foundStart)
            {
                startNode = exit;
                foundStart = true;
            }
                
            else
                endNote = exit;
        }

    }

    void DigPath(Vector2Int startingNode, Vector2Int targetNode)
    {
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closeSet = new List<Vector2Int>();
        Vector2Int currentNode;
        
        openSet.Add(startingNode);

        while (openSet.Count > 0)
        {
            currentNode = openSet[openSet.Count - 1];
            openSet.RemoveAt(openSet.Count - 1);
            closeSet.Add(currentNode);

            /////////indexGrid[currentNode.x, currentNode.y] = 0;

            if (currentNode == targetNode)
                return;

            Vector2Int _neighbor = new Vector2Int(currentNode.x - 1, currentNode.y);
            /////////if (IsWithinBound(_neighbor) && IsEmpty(_neighbor) && !openSet.Contains(_neighbor) && !closeSet.Contains(_neighbor))
                openSet.Add(_neighbor);
            
            _neighbor = new Vector2Int(currentNode.x + 1, currentNode.y);
            /////////if (IsWithinBound(_neighbor) && IsEmpty(_neighbor) && !openSet.Contains(_neighbor) && !closeSet.Contains(_neighbor))
                openSet.Add(_neighbor);
            
            _neighbor = new Vector2Int(currentNode.x, currentNode.y - 1);
            /////////if (IsWithinBound(_neighbor) && IsEmpty(_neighbor) && !openSet.Contains(_neighbor) && !closeSet.Contains(_neighbor))
                openSet.Add(_neighbor);
            
            _neighbor = new Vector2Int(currentNode.x, currentNode.y + 1);
            /////////if (IsWithinBound(_neighbor) && IsEmpty(_neighbor) && !openSet.Contains(_neighbor) && !closeSet.Contains(_neighbor))
                openSet.Add(_neighbor);
        }
    }
    
    public bool IsWithinBound(Vector2Int _node)
    {
        return
            _node.x >= 0 &&
            _node.x < LevelGenerator.ROOM_WIDTH &&
            _node.y >= 0 &&
            _node.y < LevelGenerator.ROOM_HEIGHT;
    }
}
