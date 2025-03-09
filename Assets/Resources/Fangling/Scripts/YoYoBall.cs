using UnityEngine;

public class YoYoBall : Tile
{

    public float damageForce = 1000;
    public float damageThreshold = 14;
   
    void Start()
    {
        
    }

    public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType) {
		if (damageType == DamageType.Explosive) {
			base.takeDamage(tileDamagingUs, amount, damageType);
		}
	}


 
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.GetComponent<Tile>() != null && collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Enemy)) {
			Tile otherTile = collision.gameObject.GetComponent<Tile>();
			otherTile.takeDamage(this, 5);
			otherTile.addForce(_body.linearVelocity.normalized*damageForce * 100);
		}
	}
}
