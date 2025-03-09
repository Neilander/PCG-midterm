using System.Collections.Generic;
using UnityEngine;

public class zt649ValidatedRoom : Room
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

    public bool Search(Vector2Int startingNode, Vector2Int targetNode)
    {
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closeSet = new List<Vector2Int>();
        Vector2Int currentNode;
        
        openSet.Add(startingNode);

        while (openSet.Count > 0)
        {
            currentNode = openSet[0];
            openSet.RemoveAt(0);
            closeSet.Add(currentNode);

            if (currentNode == targetNode)
                return true;

            Vector2Int _neighbor = new Vector2Int(currentNode.x - 1, currentNode.y);
            if (IsWithinBound(_neighbor) && IsEmpty(_neighbor) && !openSet.Contains(_neighbor) && !closeSet.Contains(_neighbor))
                openSet.Add(_neighbor);
            
            _neighbor = new Vector2Int(currentNode.x + 1, currentNode.y);
            if (IsWithinBound(_neighbor) && IsEmpty(_neighbor) && !openSet.Contains(_neighbor) && !closeSet.Contains(_neighbor))
                openSet.Add(_neighbor);
            
            _neighbor = new Vector2Int(currentNode.x, currentNode.y - 1);
            if (IsWithinBound(_neighbor) && IsEmpty(_neighbor) && !openSet.Contains(_neighbor) && !closeSet.Contains(_neighbor))
                openSet.Add(_neighbor);
            
            _neighbor = new Vector2Int(currentNode.x, currentNode.y + 1);
            if (IsWithinBound(_neighbor) && IsEmpty(_neighbor) && !openSet.Contains(_neighbor) && !closeSet.Contains(_neighbor))
                openSet.Add(_neighbor);
        }

        return true;
    }

    public bool IsEmpty(Vector2Int _node)
    {
        return true;
    }
    
    public bool IsWithinBound(Vector2Int _node)
    {
        return
            _node.x >= 0 &&
            _node.x < LevelGenerator.ROOM_WIDTH &&
            _node.y >= 0 &&
            _node.y < LevelGenerator.ROOM_HEIGHT;
    }

    void ValidateExits()
    {
        /////////LoadData();
        
        Vector2Int leftExit = new Vector2Int(0, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int rightExit = new Vector2Int(LevelGenerator.ROOM_WIDTH - 1, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int upExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, 0);
        Vector2Int downExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT - 1);
        
        hasLeftExit = IsEmpty(leftExit);
        hasRightExit = IsEmpty(rightExit);
        hasUpExit = IsEmpty(upExit);
        hasDownExit = IsEmpty(downExit);

        hasUpLeftPath    = Search(upExit, leftExit);
        hasUpRightPath   = Search(upExit, rightExit);
        hasUpDownPath    = Search(upExit, rightExit);
        hasDownLeftPath  = Search(upExit, rightExit);
        hasDownRightPath = Search(upExit, rightExit);
        hasLeftRightPath = Search(upExit, rightExit);
    }
    
    public bool meetsConstraint(ExitConstraint requiredExits)
    {
        if (requiredExits.upExitRequired && hasUpExit == false)
            return false;
        
        if (requiredExits.downExitRequired && hasDownExit == false)
            return false;
        
        if (requiredExits.rightExitRequired && hasRightExit == false)
            return false;
        
        if (requiredExits.leftExitRequired && hasLeftExit == false)
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
}
