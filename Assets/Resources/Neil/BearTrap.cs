using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BearTrap : Tile
{
    public SpriteRenderer targetRenderer; // 目标组件，需要在 Inspector 中手动赋值
    public Color targetColor = Color.red; // 目标颜色
    public float transitionDuration = 1f; // 颜色变换持续时间
    public float detectionRadius = 2f; // 检测范围的半径

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Tile>() != null && collision.GetComponent<Tile>().hasTag(TileTags.Creature))
        {
           
            if (targetRenderer != null)
                StartCoroutine(ChangeColor(targetRenderer.color, targetColor, transitionDuration));
            else
                Debug.LogWarning("targetRenderer 未赋值！");
        }
    }

    private IEnumerator ChangeColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            targetRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetRenderer.color = endColor; // 确保最终颜色精准
        Debug.Log("颜色变换完成");
        DetectTilesInRange();
    }

    private void DetectTilesInRange()
    {
        Collider2D[] detectedTiles = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        List<Tile> tiles = new List<Tile>();

        foreach (Collider2D collider in detectedTiles)
        {
            Tile tile = collider.GetComponent<Tile>();
            if (tile != null)
            {
                tiles.Add(tile);
            }
        }

        Debug.Log($"检测到 {tiles.Count} 个 Tile");

        // 这里你可以补充处理这些 Tile 的逻辑
        ProcessTiles(tiles);
        die();
        
    }

    private void ProcessTiles(List<Tile> tiles)
    {
        // TODO: 你可以在这里补充逻辑
        foreach (Tile t in tiles)
        {
            if (t!=this && t.hasTag(TileTags.Creature))
                t.takeDamage(this,1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 画出检测范围，在 Scene 视图中可见
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
