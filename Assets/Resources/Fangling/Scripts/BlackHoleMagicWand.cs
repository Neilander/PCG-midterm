using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackHoleMagicWand : Tile
{
   

    public GameObject blackHolePrefab;

    private void Update()
    {
        
    }

    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        
    }

    public override void useAsItem(Tile tileUsingUs)
    {
        base.useAsItem(tileUsingUs);
        if (_tileHoldingUs == tileUsingUs)
        {
            if (blackHolePrefab == null)
            {
                Debug.LogWarning("blackHolePrefab is not assigned!");
                return;
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider == null)
            {
                GameObject blackHole = Instantiate(blackHolePrefab, mousePosition, Quaternion.identity);
                blackHole.transform.parent = tileUsingUs.transform.parent;
                blackHole.transform.rotation = tileUsingUs.transform.rotation;
                blackHole.transform.localScale = tileUsingUs.transform.localScale;
            }
            else
            {
               
            }
		    Destroy(gameObject);

        }
    
    }

}
