using UnityEngine;

public class HealthKit : Tile
{
    public AudioClip heartSound;
    public int healthAmount = 1;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>() != null) {
			Tile otherTile = collision.gameObject.GetComponent<Tile>();
            if(otherTile.hasTag(TileTags.Player))
            {
                otherTile.takeDamage(this, -healthAmount);
                AudioManager.playAudio(heartSound);
                Destroy(gameObject);
            }
			
		}
    }


}
