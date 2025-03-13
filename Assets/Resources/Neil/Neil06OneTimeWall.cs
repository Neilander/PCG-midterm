using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Neil06OneTimeWall : Tile
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

    private bool isTriggered = false;
    private bool isAnimationed = false;
    private bool canDetect = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered&& !isAnimationed&& canDetect)
        {
            Collider2D[] detectedTiles = Physics2D.OverlapCircleAll(transform.position, 0.9f);
            List<Tile> tiles = new List<Tile>();

            foreach (Collider2D collider in detectedTiles)
            {
                Tile tile = collider.GetComponent<Tile>();
                if (tile != null&& tile != this)
                {
                    tiles.Add(tile);
                }
            }

            if (tiles.Count == 0)
                SpawnAnimation();
        }
    }

    public void SpawnAnimation()
    {
        if (gameObjectToMove != null)
        {
            isAnimationed = true;
            GetComponent<BoxCollider2D>().isTrigger = false;
            GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(MoveOverTime());
        }
    }

    private IEnumerator MoveOverTime()
    {
        gameObjectToMove.SetActive(true);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isTriggered = true;
        Invoke("startDetect", 0.1f);
        
    }

    private void startDetect()
    {
        canDetect = true;
    }
}
