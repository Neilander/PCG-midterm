using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class zt649DFSRoom : Room
{
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
		for (int r = 0; r < LevelGenerator.ROOM_HEIGHT; r++)
		{
			for (int c = 0; c < LevelGenerator.ROOM_WIDTH; c++)
			{
				indexGrid[c, r] = 1;
			}
		}

		Vector2Int startNode = Vector2Int.zero;
		Vector2Int endNode = Vector2Int.zero;
		bool foundStart = false;

		foreach (Vector2Int exit in requiredExits.requiredExitLocations())
		{
			if (foundStart == false) { 
				startNode = exit;
				foundStart = true;
			}
			else
				endNode = exit;
        }

		DigPath(startNode, endNode);
        //DigPath(startNode, new Vector2Int(0, 4));
        //DigPath(startNode, new Vector2Int(LevelGenerator.ROOM_WIDTH-1, 4));

        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                int tileIndex = indexGrid[i, j];
                if (tileIndex == 0)
                {
                    continue; // 0 is nothing.
                }
                GameObject tileToSpawn;
                if (tileIndex < LevelGenerator.LOCAL_START_INDEX)
                {
                    tileToSpawn = ourGenerator.globalTilePrefabs[tileIndex - 1];
                }
                else
                {
                    tileToSpawn = localTilePrefabs[tileIndex - LevelGenerator.LOCAL_START_INDEX];
                }
                Tile.spawnTile(tileToSpawn, transform, i, j);
            }
        }
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

    void DigPath(Vector2Int startingNode, Vector2Int targetNode)
    {
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closedSet = new List<Vector2Int>();
        Vector2Int currentNode;

        if (IsInBounds(startingNode))
            openSet.Add(startingNode);

        while (openSet.Count > 0)
        {
            currentNode = openSet[openSet.Count -1];
            openSet.RemoveAt(openSet.Count - 1);
            closedSet.Add(currentNode);

            indexGrid[currentNode.x, currentNode.y] = 0;

            if (currentNode == targetNode)
                return;


            List<Vector2Int> neighbors = new List<Vector2Int>();
            //left
            Vector2Int neighbor = new Vector2Int(currentNode.x - 1, currentNode.y);
            if (IsInBounds(neighbor)
                && openSet.Contains(neighbor) == false && closedSet.Contains(neighbor) == false)
            {
                neighbors.Add(neighbor);
            }

            //right
            neighbor = new Vector2Int(currentNode.x + 1, currentNode.y);
            if (IsInBounds(neighbor)
                && openSet.Contains(neighbor) == false && closedSet.Contains(neighbor) == false)
            {
                neighbors.Add(neighbor);
            }

            //down
            neighbor = new Vector2Int(currentNode.x, currentNode.y - 1);
            if (IsInBounds(neighbor)
                && openSet.Contains(neighbor) == false && closedSet.Contains(neighbor) == false)
            {
                neighbors.Add(neighbor);
            }

            //up
            neighbor = new Vector2Int(currentNode.x, currentNode.y + 1);
            if (IsInBounds(neighbor)
                && openSet.Contains(neighbor) == false && closedSet.Contains(neighbor) == false)
            {
                neighbors.Add(neighbor);
            }
            
            // iterate through neighbors, sort them by their distance to the target, go to the furthest route
            SortedDictionary<float, Vector2Int> _sortedNeighbors = new SortedDictionary<float, Vector2Int>();
            foreach (var n in neighbors)
            {
                float _dist = Vector2Int.Distance(n, targetNode);
                while (_sortedNeighbors.ContainsKey(_dist))
                    _dist += 0.01f;
                _sortedNeighbors.Add(_dist, n);
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                neighbors[i] = _sortedNeighbors.ElementAt(neighbors.Count-i-1).Value;
            }

            //GlobalFuncs.shuffle(neighbors);

            openSet.AddRange(neighbors);
        }

    }
}
