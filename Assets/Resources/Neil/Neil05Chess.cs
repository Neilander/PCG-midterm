using UnityEngine;
using System.Collections;

public class Neil05Chess : BasicAICreature
{
    public float damageForce = 1000;
    public int damageAmount = 1;

    protected float _nextMoveCounter;
    public float timeBetweenMovesMin = 1.5f;
    public float timeBetweenMovesMax = 3f;

    public override void Start()
    {
        _targetGridPos = Tile.toGridCoord(globalX, globalY);
        _nextMoveCounter = Random.Range(timeBetweenMovesMin, timeBetweenMovesMax);
    }

    void Update()
    {
        // Update our counters.
        if (_nextMoveCounter > 0)
        {
            _nextMoveCounter -= Time.deltaTime;
        }

        // When it's time to try a new move.
        if (_nextMoveCounter <= 0)
        {
            takeStep();
        }

        updateSpriteSorting();
    }

    protected override void takeStep()
    {
        // Try to move to one of our neighboring positions if it is empty.
        _neighborPositions.Clear();

        // We test neighbor locations by casting in specific directions. 

        
        Vector2 upFarthest = FindFarthestReachable(_targetGridPos, Vector2.up);
        Vector2 rightFarthest = FindFarthestReachable(_targetGridPos, Vector2.right);
        Vector2 downFarthest = FindFarthestReachable(_targetGridPos, Vector2.down);
        Vector2 leftFarthest = FindFarthestReachable(_targetGridPos, Vector2.left);

        if (Vector2.Distance(upFarthest, toWorldCoord(transform.position)) > 1f)
            _neighborPositions.Add(upFarthest);
        if (Vector2.Distance(rightFarthest, toWorldCoord(transform.position)) > 1f)
            _neighborPositions.Add(rightFarthest);
        if (Vector2.Distance(downFarthest, toWorldCoord(transform.position)) > 1f)
            _neighborPositions.Add(downFarthest);
        if (Vector2.Distance(leftFarthest, toWorldCoord(transform.position)) > 1f)
            _neighborPositions.Add(leftFarthest);

        // If there's an empty neighbor, choose one randomly.
        if (_neighborPositions.Count > 0)
        {
            _targetGridPos = GlobalFuncs.randElem(_neighborPositions);
            _nextMoveCounter = Random.Range(timeBetweenMovesMin, timeBetweenMovesMax);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Tile otherTile = collision.gameObject.GetComponent<Tile>();
        if (otherTile != null && otherTile.hasTag(tagsWeChase))
        {
            otherTile.takeDamage(this, damageAmount);
            Vector2 toOtherTile = (Vector2)otherTile.transform.position - (Vector2)transform.position;
            toOtherTile.Normalize();
            otherTile.addForce(damageForce * toOtherTile);
        }
    }

    private Vector2 FindFarthestReachable(Vector2 startPos, Vector2 direction)
    {
        Vector2 currentPos = startPos;
        Vector2 nextPos = currentPos + direction;


        while (pathIsClear(toWorldCoord(nextPos)))
        {
            currentPos = nextPos;
            nextPos += direction;
        }

        return currentPos;
    }
}
