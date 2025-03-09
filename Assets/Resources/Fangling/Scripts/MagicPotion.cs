using UnityEngine;
using System.Collections;

public class MagicPotion : Tile
{
    public enum PotionType
    {
        SpeedUp,
        Invulnerable,

    }

    public PotionType potionType;
    private float duration = 5f;
    private float probility;

    private GameObject _tileUsingUs;

    private SpriteRenderer _spriteRenderer;

 

    private void Start(){
        _spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public override void pickUp(Tile tilePickingUsUp){
        base.pickUp(tilePickingUsUp);
        probility = Random.Range(0f, 1f);
        if(probility < 0.5f){
            potionType = PotionType.SpeedUp;
            _spriteRenderer.color = Color.red;
        }else{
            potionType = PotionType.Invulnerable;
            _spriteRenderer.color = Color.blue;
        }
    }

    public override void useAsItem(Tile tileUsingUs){
        base.useAsItem(tileUsingUs);
        _tileUsingUs = tileUsingUs.gameObject;
        switch(potionType){
            case PotionType.SpeedUp:
                StartCoroutine(SpeedUp());
                break;
            case PotionType.Invulnerable:
                Invulnerable();
                break;
        }
        
        _spriteRenderer.enabled = false;
    }


    private IEnumerator SpeedUp(){
        Player player = _tileUsingUs.GetComponent<Player>();
        if(player == null){
            yield break;
        }

        player.moveSpeed += 2;
 
        yield return new WaitForSeconds(duration);
        player.moveSpeed -= 2;
      
        Destroy(gameObject);

    }

    private void Invulnerable(){

        Player player = _tileUsingUs.GetComponent<Player>();
        if(player == null){
            return;
        }
        _tileUsingUs.GetComponent<Collider2D>().enabled = false;
               _tileUsingUs.GetComponent<SpriteRenderer>().color =  new Color(1, 1,1,0.4f);
        StartCoroutine(InvulnerableTimer(player));
        
        
    }

    private IEnumerator InvulnerableTimer(Player player){
        yield return new WaitForSeconds(duration);
        _tileUsingUs.GetComponent<Collider2D>().enabled = true;
          _tileUsingUs.GetComponent<SpriteRenderer>().color =  new Color(1, 1,1,1);
        Destroy(gameObject);
    }



    
}
