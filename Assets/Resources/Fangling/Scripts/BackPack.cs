using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : Tile
{
    private const int MaxCapacity = 4;
    private Dictionary<int, Tile> storedItems = new Dictionary<int, Tile>(); // 使用字典存储物品
    public float pickupRadius = 1.5f;
    private Tile currentItem = null;
    private int currentItemIndex = -1;

    public Image[] itemSlots;
    public GameObject itemUI;

    private bool isHoldingItem;
    private bool isInUse;

    private Tile tileHoldingUs;

    public ProxyTile[] proxyTiles;

    public Sprite defaultSprite;

    public override void init()
    {
        base.init();
        currentItem = null;
        isHoldingItem = false;
        isInUse = false;
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

        int emptySlot = GetFirstAvailableSlot();
        if (emptySlot == -1) return false;

        storedItems[emptySlot] = item;
        item.gameObject.SetActive(false);
        Debug.Log($"Item {item.name} stored in backpack at slot {emptySlot}.");
        return true;
    }

    public Tile RetrieveItem(int index)
    {
        if (!storedItems.ContainsKey(index))
        {
            Debug.Log("Invalid slot index!");
            return null;
        }

        Tile item = storedItems[index];
        item.gameObject.SetActive(true);
        item.pickUp(proxyTiles[index]);
        Debug.Log($"Item {item.name} retrieved from backpack.");
        return item;
    }

    private void StoreCurrentItem()
    {
        if (currentItem != null)
        {
            Debug.Log($"Storing back current item: {currentItem.name}");
            currentItem.gameObject.SetActive(false);
            currentItem = null;
        }
    }

    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        tileHoldingUs = tilePickingUsUp;
        for(int i = 0; i < MaxCapacity; i++)
        {
            proxyTiles[i].bodyParent = tilePickingUsUp;
        }
        isInUse = true;
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
        if(currentItem != null)
        {
            PutBackItem();
        }
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
                    
                    return;
                }
            }
        }

    
    }

    public void dropItem()
    {
        if (currentItemIndex == -1) return;

        Tile item = RetrieveItem(currentItemIndex);
        if (item != null)
        {
            storedItems.Remove(currentItemIndex);
            item.dropped(proxyTiles[currentItemIndex]);
            item.transform.parent = tileHoldingUs.transform.parent;
            currentItemIndex = -1;
            currentItem = null;
         
        }
    }

    void Update()
    {
        for(int i = 0; i < MaxCapacity; i++)
        {
            if(_tileHoldingUs != null)
            proxyTiles[i].aimDirection = _tileHoldingUs.aimDirection;
        }
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
        if(currentItem == null && currentItemIndex != -1){

            if(storedItems.ContainsKey(currentItemIndex)){
                storedItems.Remove(currentItemIndex);
                currentItemIndex = -1;
            }



        }

        UpdateUI();
        HandleItemSwitching();
    }

    private void HandleItemSwitching()
    {
        for (int i = 0; i < MaxCapacity; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (currentItemIndex == i)
                {
                    PutBackItem();
                    currentItemIndex = -1;
                }
                else if (storedItems.ContainsKey(i))
                {
                    currentItemIndex = i;
                    SwitchItem(i);
                }
            }
        }
    }

    private void SwitchItem(int newIndex)
    {
        StoreCurrentItem();
        currentItem = RetrieveItem(newIndex);
        if (currentItem != null)
        {
            isHoldingItem = true;
            Debug.Log($"Switched to item: {currentItem.name}");
        }
    }

    private void PutBackItem()
    {
        if (currentItem != null)
        {
            currentItem.gameObject.SetActive(false);
            storedItems[currentItemIndex] = currentItem;
            isHoldingItem = false;
            currentItem = null;
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (storedItems.ContainsKey(i) && storedItems[i] != null)
            {
                itemSlots[i].sprite = storedItems[i].sprite.sprite;
            }
            else
            {
                itemSlots[i].sprite = defaultSprite;
            }
        }
    }

    private int GetFirstAvailableSlot()
    {
        for (int i = 0; i < MaxCapacity; i++)
        {
            if (!storedItems.ContainsKey(i))
                return i;
        }
        return -1;
    }

    public override void useAsItem(Tile tileUsingUs) {
        if (currentItem != null)
		currentItem.useAsItem(proxyTiles[currentItemIndex]);
	}
}
