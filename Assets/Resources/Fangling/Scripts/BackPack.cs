using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : Tile
{
    private const int MaxCapacity = 4; 
    private List<Tile> storedItems = new List<Tile>();
    public float pickupRadius = 1.5f; 
    private Tile currentItem = null; 
    private int currentItemIndex = -1;

    public Image[] itemSlots;
    public GameObject itemUI;

    public override void init()
    {
        base.init();
        itemUI.SetActive(false);
        addTag(TileTags.CanBeHeld); 
    }

   
    public bool StoreItem(Tile item)
    {
        if (storedItems.Count >= MaxCapacity)
        {
            Debug.Log("Backpack is full!");
            return false;
        }

        storedItems.Add(item);

        item.gameObject.SetActive(false); 
        Debug.Log($"Item {item.name} stored in backpack.");
        return true;
    }

    
    public Tile RetrieveItem(int index)
    {
        if (index < 0 || index >= storedItems.Count)
        {
            Debug.Log("Invalid slot index!");
            return null;
        }

        Tile item = storedItems[index];
        item.gameObject.SetActive(true);
        item.pickUp(_tileHoldingUs);
        Debug.Log($"Item {item.name} retrieved from backpack.");
        return item;
    }

    
    private void StoreCurrentItem()
    {
        if (currentItem != null)
        {
            Tile item = currentItem;
            Debug.Log($"Storing back current item: {currentItem.name}");
            item.gameObject.SetActive(false); 
            currentItem = null;
        }
    }

   
    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        itemUI.SetActive(true);
        if (!hasTag(TileTags.CanBeHeld)) return;

        if (_body != null)
        {
            _body.linearVelocity = Vector2.zero;
            _body.bodyType = RigidbodyType2D.Kinematic;
        }

        transform.parent = tilePickingUsUp.transform;
        transform.localPosition = new Vector3(heldOffset.x, heldOffset.y, -0.1f);
        transform.localRotation = Quaternion.Euler(0, 0, heldAngle);
        removeTag(TileTags.CanBeHeld);
        tilePickingUsUp.tileWereHolding = this;
        _tileHoldingUs = tilePickingUsUp;
    }

  
    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
        itemUI.SetActive(false);
        if (_tileHoldingUs != tileDroppingUs) return;

        if (_body != null)
        {
            _body.bodyType = RigidbodyType2D.Dynamic;
        }

        transform.parent = null;
        addTag(TileTags.CanBeHeld);
        _tileHoldingUs.tileWereHolding = null;
        _tileHoldingUs = null;
        Debug.Log("Backpack dropped.");
    }


    public void pickUpNearbyItem()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);

        foreach (Collider2D col in colliders)
        {
            Tile item = col.GetComponent<Tile>();
            if (item != null && item.hasTag(TileTags.CanBeHeld))
            {
                if (StoreItem(item))
                {
                    Debug.Log($"Picked up {item.name}.");
                    return;
                }
            }
        }

        Debug.Log("No valid item found to pick up.");
    }

   
    public void dropItem()
    {

        Tile item = RetrieveItem(currentItemIndex);
        if (item != null)
        {
            item.dropped(_tileHoldingUs);
            storedItems.Remove(item);
            currentItem = null;
            Debug.Log($"Dropped {item.name}.");
        }
    }

  
    void Update()
    {
        if (_tileHoldingUs != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                pickUpNearbyItem();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                dropItem();
            }
        }
        if(currentItem == null && currentItemIndex != -1)
        {
            storedItems.RemoveAt(currentItemIndex);
            currentItemIndex = -1;
            
        }
        UpdateUI();
        HandleItemSwitching();
    }

    private void HandleItemSwitching()
    {
        int slotToRetrieve = -1;

        if (Input.GetKeyDown(KeyCode.Alpha1) && storedItems.Count >= 1)
        {
            if(currentItemIndex == 0)
            {
                PutBackItem();
                currentItemIndex = -1;
                slotToRetrieve = -1;
            }else{
                currentItemIndex = 0;
                slotToRetrieve = 0;
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && storedItems.Count >= 2)
        {
            if (currentItemIndex == 1)
            {
                PutBackItem();
                currentItemIndex = -1;
                slotToRetrieve = -1;
            }else{
                currentItemIndex = 1;
                slotToRetrieve = 1;
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && storedItems.Count >= 3)
        {
            if(currentItemIndex == 2)
            {
                PutBackItem();
                currentItemIndex = -1;
                slotToRetrieve = -1;
            }else{
                currentItemIndex = 2;
                slotToRetrieve = 2;
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && storedItems.Count >= 4)
        {
            if(currentItemIndex == 3)
            {
                currentItemIndex = -1;
                PutBackItem();
                slotToRetrieve = -1;
            }else{
                currentItemIndex = 3;
                slotToRetrieve = 3;
            }
        }

        if (slotToRetrieve != -1)
        {

            SwitchItem(slotToRetrieve);
        }
    }

   
    private void SwitchItem( int newIndex)
    {
       
        StoreCurrentItem();

        currentItem = RetrieveItem(newIndex);
        if (currentItem != null)
        {
            Debug.Log($"Switched to item: {currentItem.name}");
        }
    }

    private void PutBackItem()
    {
        if (currentItem != null)
        {
            currentItem.gameObject.SetActive(false);
            currentItem = null;
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < storedItems.Count)
            {
                itemSlots[i].sprite = storedItems[i].sprite.sprite;
            }
            else
            {
                itemSlots[i].sprite = null;
            }
        }

    }


}
