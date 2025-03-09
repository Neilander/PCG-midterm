using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoShield : Tile
{
    public Collider2D onGroundCollider;
    public Collider2D heldCollider;
    public Sprite heldSprite;
    public Sprite onGroundSprite;

    private Transform _player; 
    private float _orbitRadius = 1.5f;
    private float _orbitSpeed = 200f; 
    private float _angle; 
    private float _initialAngle; 

    public GameObject shieldPartPrefab;
    private List<GameObject> _extraShields = new List<GameObject>();

  

    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);

        if (tilePickingUsUp == null)
        {
            return;
        }

        if (_tileHoldingUs == tilePickingUsUp)
        {
            transform.parent = null;
            _sprite.sprite = heldSprite;
            onGroundCollider.enabled = false;
            heldCollider.enabled = true;

            _player = tilePickingUsUp.transform;
            _angle = 0f; 

            if (shieldPartPrefab == null)
            {
                Debug.LogWarning("shieldPrefab is not assigned!");
                return;
            }

            float angleStep = 90f; 
        
            for (int i = 1; i <= 3; i++) 
            {
                float spawnAngle = i * angleStep; 
                GameObject shield = Instantiate(shieldPartPrefab, _player.position, Quaternion.identity);
                shield.transform.parent = tilePickingUsUp.transform.parent;
                AutoShieldPart shieldScript = shield.GetComponent<AutoShieldPart>();
                if (shieldScript != null)
                {
                    shieldScript._angle = spawnAngle;
                    shieldScript._player = _player;
                    _extraShields.Add(shield); 
                }
        
            }
        }


    }
    


    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
         foreach (GameObject shield in _extraShields)
                {
                    Destroy(shield);
                }
                _extraShields.Clear();

        if (_tileHoldingUs == null)
        {
            _body.bodyType = RigidbodyType2D.Kinematic;
            _body.linearVelocity = Vector2.zero;
            _body.angularVelocity = 0;

            heldCollider.enabled = false;
            onGroundCollider.enabled = true;
            _sprite.sprite = onGroundSprite;

            if (tileDroppingUs != null)
            {
                transform.parent = tileDroppingUs.transform.parent;
                transform.position = tileDroppingUs.transform.position;
               
            }

            _player = null;
        }
    }

    private void Update()
    {
        if (_player == null) return;

        _angle += _orbitSpeed * Time.deltaTime;
        float radian = _angle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * _orbitRadius;
        transform.position = (Vector2)_player.position + offset;

        Vector2 directionToPlayer = _player.position - transform.position;
        float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleToPlayer + 180);
    }
}
