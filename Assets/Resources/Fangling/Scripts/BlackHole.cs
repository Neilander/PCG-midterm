using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackHole : Tile
{
    public float pullRadius = 2f;
    public float pullForceEnemy = 30f;
    public float pullForceFriendly = 20f;

    private List<Tile> affectedCreatures = new List<Tile>(); 
    private void PullCreatures()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pullRadius);

        foreach (Collider2D col in colliders)
        {
            Tile tile = col.GetComponent<Tile>();
            if (tile != null && tile.hasTag(TileTags.Creature) && !affectedCreatures.Contains(tile))
            {
                affectedCreatures.Add(tile);
            }
        }

        for (int i = affectedCreatures.Count - 1; i >= 0; i--)
        {
            Tile creature = affectedCreatures[i];

            if (creature == null)
            {
                affectedCreatures.RemoveAt(i);
                continue;
            }

            Rigidbody2D creatureRb = creature.GetComponent<Rigidbody2D>();
            if (creatureRb != null)
            {
                Vector2 direction = (transform.position - creature.transform.position);
                float distance = direction.magnitude;

                if (distance < pullRadius)  
                {
                    direction.Normalize(); 
                    
                    float forceMultiplier = Mathf.Clamp01(1 - (distance / pullRadius)); 

                    if (creature.hasTag(TileTags.Friendly))
                    {
                        creatureRb.AddForce(direction * pullForceFriendly * forceMultiplier, ForceMode2D.Force);
                    }
                    else
                    {
                        creatureRb.linearVelocity = direction * pullForceEnemy * forceMultiplier;
                    }
                }
            }
        }
    }

    
    

    private void Update()
    {
        PullCreatures();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Tile>() != null)
        {
            Tile creature = other.GetComponent<Tile>();
            if(creature.hasTag(TileTags.Player))
            {
                int currentHealth = creature.GetComponent<Player>().health;
                creature.takeDamage(this, currentHealth, DamageType.Normal);
                Color newColor = creature.sprite.color;
                newColor.a = 0f; 
                creature.sprite.color = newColor; 
            }
            else
            {
                Destroy(other.gameObject);
            }
            if(affectedCreatures.Contains(creature))
            affectedCreatures.RemoveAt(affectedCreatures.IndexOf(creature));
        }else return;
                      
    }
}
