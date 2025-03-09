using UnityEngine;
using System.Collections;

public class Neil04Spike : Tile
{
    public TileTags OtherTargetWeDamage;
    public float damageForce;
    private Tile follow;
    private Coroutine desCor;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>() != null)
        {
            Tile tar = collision.gameObject.GetComponent<Tile>();
            if (tar != follow && tar.hasTag(OtherTargetWeDamage))
            {
                if (desCor != null)
                    StopCoroutine(desCor);
                tar.takeDamage(this, 1);
                Vector2 toOtherTile = (Vector2)tar.transform.position - (Vector2)transform.position;
                toOtherTile.Normalize();
                tar.addForce(damageForce * toOtherTile);
                follow = tar;
                transform.parent = follow.transform;
                Destroy(GetComponent<Rigidbody2D>());
                desCor = StartCoroutine(DestroySelf());
            }
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(5);
        base.die();
    }
}
