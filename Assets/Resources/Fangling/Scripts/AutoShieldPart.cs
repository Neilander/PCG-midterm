using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AutoShieldPart : Tile
{
    public Transform _player; 
    private float _orbitRadius = 1.5f;
    private float _orbitSpeed = 200f; 
    public float _angle; 
   
    public Sprite heldSprite;

    public override void init(){
        base.init();
        _sprite.sprite = heldSprite;
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
