using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostTile : Tile
{
    public float speedMultiplier = 2f; // 默认加速倍数
    public float boostDuration = 5f; // 持续时间（秒），如果为 0 则永久生效

    private Dictionary<Tile, float> originalSpeeds = new Dictionary<Tile, float>(); // 存储原始速度

    public override void useAsItem(Tile tileUsingUs)
    {
        List<Tile> enemyTiles = FindAllEnemyTiles();

        if (enemyTiles.Count == 0)
        {
            Debug.Log("No enemies found to boost.");
            return;
        }

        foreach (Tile enemy in enemyTiles)
        {
            ApplySpeedBoost(enemy);
        }

        Debug.Log($"Boosted {enemyTiles.Count} enemies with {speedMultiplier}x speed.");
    }

    // 查找所有带有 Enemy 标签的 Tile
    private List<Tile> FindAllEnemyTiles()
    {
        Tile[] allTiles = FindObjectsOfType<Tile>(); // 获取所有 Tile
        List<Tile> enemyTiles = new List<Tile>();

        foreach (Tile tile in allTiles)
        {
            if (tile.hasTag(TileTags.Enemy))
            {
                enemyTiles.Add(tile);
            }
        }

        return enemyTiles;
    }

    // 应用加速效果
    private void ApplySpeedBoost(Tile enemy)
    {
        Rigidbody2D enemyBody = enemy.body;
        if (enemyBody == null) return;

        float currentSpeed = enemyBody.linearVelocity.magnitude;
        if (!originalSpeeds.ContainsKey(enemy))
        {
            originalSpeeds[enemy] = currentSpeed; // 记录原始速度
        }

        enemyBody.linearVelocity *= speedMultiplier; // 速度乘以倍数

        // 如果有持续时间，恢复原速
        if (boostDuration > 0)
        {
            StartCoroutine(ResetSpeedAfterDuration(enemy, boostDuration));
        }
    }

    // 还原原始速度
    private IEnumerator ResetSpeedAfterDuration(Tile enemy, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (originalSpeeds.ContainsKey(enemy))
        {
            Rigidbody2D enemyBody = enemy.body;
            if (enemyBody != null)
            {
                enemyBody.linearVelocity = enemyBody.linearVelocity.normalized * originalSpeeds[enemy]; // 恢复速度
            }
            originalSpeeds.Remove(enemy);
        }
    }
}
