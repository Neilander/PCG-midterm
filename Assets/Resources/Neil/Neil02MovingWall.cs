using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Neil02MovingWall : Tile
{

    [Header("Movement Settings")]
    public GameObject gameObjectToMove; 
    public Vector3 startPosition; 
    public Vector3 targetPosition; 
    public AnimationCurve moveCurve; 
    public float moveDuration = 2f;

    [Header("Spawn Settings")]
    public GameObject newObjectPrefab;
    public float checkInterval = 1f;
    private float checkTimer = 0f;

    [Header("Reverse Movement Settings")]
    public AnimationCurve reverseMoveCurve;
    public float reverseMoveDuration = 1.5f; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            TrySpawnAndDestroy();
        }
    }

    public void SpawnAnimation()
    {
        if (gameObjectToMove != null)
        {
            StartCoroutine(MoveOverTime());
        }
    }

    private IEnumerator MoveOverTime()
    {
        gameObjectToMove.transform.localPosition = startPosition;
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            float curveValue = moveCurve.Evaluate(t);
            gameObjectToMove.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObjectToMove.transform.localPosition = targetPosition;
    }

    private void TrySpawnAndDestroy()
    {
        Vector2 currentPos = transform.position;
        Vector2[] directions = { Vector2.up * 2, Vector2.down * 2, Vector2.left * 2, Vector2.right * 2 };
        List<Vector2> emptySpots = new List<Vector2>();

        foreach (Vector2 dir in directions)
        {
            if (!HasTileAtPosition(currentPos + dir))
            {
                emptySpots.Add(currentPos + dir);
            }
        }

        if (emptySpots.Count > 0)
        {
            Vector2 spawnPos = emptySpots[Random.Range(0, emptySpots.Count)];
            SpawnNewObjectAndDestroySelf(spawnPos);
        }
    }

    private bool HasTileAtPosition(Vector2 position)
    {
        return Tile.tileAtPoint(position, (TileTags)~0) != null;
    }

    private void SpawnNewObjectAndDestroySelf(Vector2 spawnPosition)
    {
        if (newObjectPrefab != null)
        {
            GameObject newObj = Instantiate(newObjectPrefab, spawnPosition, Quaternion.identity, transform.parent);
        }
        StartCoroutine(MoveBackAndDestroy());
    }

    private IEnumerator MoveBackAndDestroy()
    {
        float elapsedTime = 0f;
        while (elapsedTime < reverseMoveDuration)
        {
            float t = elapsedTime / reverseMoveDuration;
            float curveValue = reverseMoveCurve.Evaluate(t);
            gameObjectToMove.transform.localPosition = Vector3.Lerp(targetPosition, startPosition, curveValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObjectToMove.transform.localPosition = startPosition;
        Destroy(gameObject);
    }

}
