using UnityEngine;
using System.Collections.Generic;

public class CircleRoom : Room
{

    public GameObject[] treasurePrefab;
    public GameObject[] enemyPrefab;

	public int minNumEnemies = 1, maxNumEnemies = 2;

    List<Vector2Int> pathNodes = new List<Vector2Int>(); 
    HashSet<Vector2Int> wallNodes = new HashSet<Vector2Int>(); 

  

    private void Start()
    {
    
    }
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

		
        //DigCircularPath(new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT / 2), 5, 2);
        GenerateLayeredMaze(new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT / 2), 5);
        DigPath(startNode, endNode);
   


        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                int tileIndex = indexGrid[i, j];
                if (tileIndex == 0)
                {
                    continue; 
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
        GenerateEnemies(Random.Range(minNumEnemies, maxNumEnemies + 1));
        GenerateTreasure();
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

    void DigPath(Vector2Int startNode, Vector2Int targetNode)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        stack.Push(startNode);
        cameFrom[startNode] = startNode;

        while (stack.Count > 0)
        {
            Vector2Int currentNode = stack.Pop();
            indexGrid[currentNode.x, currentNode.y] = 0;

            if (currentNode == targetNode)
            {
                return;
            }

            List<Vector2Int> neighbors = new List<Vector2Int>
            {
                new Vector2Int(currentNode.x - 1, currentNode.y),
                new Vector2Int(currentNode.x + 1, currentNode.y),
                new Vector2Int(currentNode.x, currentNode.y - 1),
                new Vector2Int(currentNode.x, currentNode.y + 1)
            };

    
            neighbors.Sort((a, b) => (a - targetNode).sqrMagnitude.CompareTo((b - targetNode).sqrMagnitude));

            foreach (var neighbor in neighbors)
            {
                if (IsInBounds(neighbor) && !cameFrom.ContainsKey(neighbor))
                {
                    stack.Push(neighbor);
                    cameFrom[neighbor] = currentNode;
                }
            }
        }
        }

    void DigCircularPath(Vector2Int center, int radius, int thickness)
    {
        for (float angle = 0; angle < 2 * Mathf.PI; angle += 0.1f)
        {
            for (int t = -thickness / 2; t < thickness / 2; t++)
            {
                int x = Mathf.RoundToInt(center.x + (radius + t) * Mathf.Cos(angle));
                int y = Mathf.RoundToInt(center.y + (radius + t) * Mathf.Sin(angle));
                if (IsInBounds(new Vector2Int(x, y)))
                {
                    indexGrid[x, y] = 0;
                }
            }
        }
    }
    void GenerateLayeredMaze(Vector2Int center, int maxLayers)
    {



        SetCenterAsPath(center, pathNodes);

        for (int layer = 2; layer <= maxLayers; layer++) 
        {
            bool isPath = (layer % 2 == 1); 
            int size = layer -1 ; 

        
            for (int i = -size; i <= size; i++)
            {
                PlaceTile(center.x + i, center.y - size, isPath, pathNodes, wallNodes);
                PlaceTile(center.x + i, center.y + size, isPath, pathNodes, wallNodes);
                PlaceTile(center.x - size, center.y + i, isPath, pathNodes, wallNodes);
                PlaceTile(center.x + size, center.y + i, isPath, pathNodes, wallNodes);
            }

            if (!isPath)
            {
                EnsureWallHasExits(center, size, pathNodes, wallNodes);
            }
        }

        AddExtraConnections(pathNodes);

    }


    void SetCenterAsPath(Vector2Int center, List<Vector2Int> pathNodes)
    {
        Vector2Int centerPos = center;
        indexGrid[centerPos.x, centerPos.y] = 0; 
        pathNodes.Add(centerPos);
    }

    void PlaceTile(int x, int y, bool isPath, List<Vector2Int> pathNodes, HashSet<Vector2Int> wallNodes)
    {
        Vector2Int pos = new Vector2Int(x, y);
        if (IsInBounds(pos))
        {
            indexGrid[x, y] = isPath ? 0 : 1;
            if (isPath)
                pathNodes.Add(pos);
            else
                wallNodes.Add(pos);
        }
    }

    void EnsureWallHasExits(Vector2Int center, int size, List<Vector2Int> pathNodes, HashSet<Vector2Int> wallNodes)
    {
        int numExits = Random.Range(2, 4);
        List<Vector2Int> wallList = new List<Vector2Int>(wallNodes);
        
        for (int i = 0; i < numExits; i++)
        {
            Vector2Int nearestPath = pathNodes[Random.Range(0, pathNodes.Count)];
            Vector2Int doorPos = FindClosestWall(center, size, nearestPath, wallList);

            if (IsInBounds(doorPos))
            {
                indexGrid[doorPos.x, doorPos.y] = 0; 
                pathNodes.Add(doorPos); 
                wallNodes.Remove(doorPos);
            }
        }
    }

    Vector2Int FindClosestWall(Vector2Int center, int size, Vector2Int target, List<Vector2Int> wallList)
    {
        GlobalFuncs.shuffle(wallList); 
        foreach (var pos in wallList)
        {
            if (Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y) == 1)
            {
                return pos;
            }
        }
        return target;
    }

    void AddExtraConnections(List<Vector2Int> pathNodes)
    {
        int extraConnections = pathNodes.Count / 10; 
        for (int i = 0; i < extraConnections; i++)
        {
            Vector2Int nodeA = pathNodes[Random.Range(0, pathNodes.Count)];
            Vector2Int nodeB = pathNodes[Random.Range(0, pathNodes.Count)];

            if (Mathf.Abs(nodeA.x - nodeB.x) + Mathf.Abs(nodeA.y - nodeB.y) == 1)
            {
                indexGrid[nodeB.x, nodeB.y] = 0; 
            }
        }
    }

    void GenerateEnemies(int numEnemies)
    {
        List<Vector2> possibleSpawnPositions = new List<Vector2>(LevelGenerator.ROOM_WIDTH * LevelGenerator.ROOM_HEIGHT);
        
    
        for (int i = 0; i < numEnemies; i++)
        {
            possibleSpawnPositions.Clear();
            
            
            for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
            {
                for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
                {
                    if (wallNodes.Contains(new Vector2Int(x, y)))
                    {
                        continue;
                    }

                    
                    possibleSpawnPositions.Add(new Vector2(x, y));
                }
            }

           
            if (possibleSpawnPositions.Count > 0)
            {
                
                Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
                int enemyIndex = Random.Range(0, enemyPrefab.Length);
                Tile.spawnTile(enemyPrefab[enemyIndex], transform, (int)spawnPos.x, (int)spawnPos.y);
                
                
            }
        }
    }

    void GenerateTreasure(){
        int n = Random.Range(0, treasurePrefab.Length);
        Tile.spawnTile(treasurePrefab[n], transform, LevelGenerator.ROOM_WIDTH/2, LevelGenerator.ROOM_HEIGHT/2);
    }


}
