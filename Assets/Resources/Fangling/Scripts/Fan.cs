using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fan: Tile
{
    public float pushRadius = 5f;
    public float pushForceEnemy = 30f;
    private GameObject tileHoldingUs;
    private Animator _animator;

    private List<Tile> affectedCreatures = new List<Tile>(); 

    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        _body.bodyType = RigidbodyType2D.Dynamic;
		Joint2D ourJoint = GetComponent<Joint2D>();
		ourJoint.connectedBody = _tileHoldingUs.body;
		ourJoint.enabled = true;
        tileHoldingUs = tilePickingUsUp.gameObject;
        _animator = GetComponent<Animator>();
        _animator.SetBool("isPickUp", true);
       
    }

    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
        _body.bodyType = RigidbodyType2D.Kinematic;
        _body.linearVelocity = Vector2.zero;
        _body.angularVelocity = 0;
        Joint2D ourJoint = GetComponent<Joint2D>();
        ourJoint.enabled = false;
        ourJoint.connectedBody = null;
        tileHoldingUs = null;
    }
    private void PushCreatures()
    {
        affectedCreatures.Clear();
        if(tileHoldingUs == null)
        {
            return;
        }
        Vector2 fanDirection = transform.position - tileHoldingUs.transform.position;
        float maxAngle = 30;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pushRadius);

        foreach (Collider2D col in colliders)
        {
            Tile tile = col.GetComponent<Tile>();
            
            if (tile != null && tile.hasTag(TileTags.Creature) && tile.hasTag(TileTags.Enemy) && !affectedCreatures.Contains(tile))
            {
                Vector2 directionToTile = (tile.transform.position - tileHoldingUs.transform.position).normalized;
                if(Vector2.Angle(fanDirection, directionToTile) < maxAngle)
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

                if (distance < pushRadius)  
                {
                    direction.Normalize(); 
                    
                    float forceMultiplier = Mathf.Clamp01(1 - (distance / pushRadius)); 
   
                    creatureRb.AddForce(-direction * pushForceEnemy * forceMultiplier , ForceMode2D.Force);
                    
                }
            }
        }
    }


    
	void FixedUpdate() {
		if (_tileHoldingUs != null) {
			float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x)*Mathf.Rad2Deg;
 			_body.MoveRotation(aimAngle);
		}

	}

    private void Update()
    {
        PushCreatures();
    }



    
    


}
