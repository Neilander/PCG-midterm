using UnityEngine;
using System.Collections;

public class Neil01StormStone : Tile
{

    // Sound that's played when we're thrown.
    public AudioClip throwSound;

    // How much force to add when thrown
    public float throwForce = 3000f;
    public float pushForceToUser = 1000f;

    // How slow we need to be going before we consider ourself "on the ground" again
    public float onGroundThreshold = 0.8f;

    // How much relative velocity we need with a target on a collision to cause damage.
    public float damageThreshold = 14;
    // How much force we apply to a target when we deal damage. 
    public float damageForce = 1000;

    // We keep track of the tile that threw us so we don't collide with it immediately.
    protected Tile _tileThatThrewUs = null;

    // Keep track of whether we're in the air and whether we were JUST thrown
    protected bool _isInAir = false;
    protected float _afterThrowCounter;
    public float afterThrowTime = 0.2f;

    public float pullForce;
    public float pullRadius;

    public float expandDuration = 0.5f;
    public AnimationCurve expandCurve;
    public float creatureForceTime;
    public Vector3 targetScale = new Vector3(5f, 5f, 1f);
    public GameObject rangeObject;
    public int canBeUsedTime = 3;
    public GameObject selfPrefab;

    private Vector3 initialScale;

    //Storm stone can't be damaged!
    public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
    {
        Debug.Log("Storm stone can't be damaged!");
        return;
    }

    public override void useAsItem(Tile tileUsingUs)
    {
        if (_tileHoldingUs != tileUsingUs)
        {
            return;
        }
        if (onTransitionArea())
        {
            return; // Don't allow us to be thrown while we're on a transition area.
        }
        AudioManager.playAudio(throwSound);

        if (canBeUsedTime > 1)
        {
            GameObject newStorm = Instantiate(selfPrefab, transform.parent);
            newStorm.transform.position = transform.position;
            newStorm.transform.rotation = transform.rotation;
            newStorm.GetComponent<Tile>().init();
            newStorm.GetComponent<Neil01StormStone>().getThrowed(tileUsingUs, _tileHoldingUs,false);
            canBeUsedTime -= 1;
        }
        else
        {
            getThrowed(tileUsingUs, _tileHoldingUs, true);
        }

        

    }

    public void getThrowed(Tile tileUsingUs, Tile holding, bool ifRelease)
    {
        canBeUsedTime = 1;
        //_sprite.transform.localPosition = Vector3.zero;
        _tileHoldingUs = holding;
        _tileThatThrewUs = tileUsingUs;
        _isInAir = true;

        // We use IgnoreCollision to turn off collisions with the tile that just threw us.
        if (_tileThatThrewUs.GetComponent<Collider2D>() != null)
        {
            Physics2D.IgnoreCollision(_tileThatThrewUs.GetComponent<Collider2D>(), _collider, true);
        }

        // We're thrown in the aim direction specified by the object throwing us.
        Vector2 throwDir = _tileThatThrewUs.aimDirection.normalized;
        _tileHoldingUs.addForce(-throwDir * pushForceToUser);
        // Have to do some book keeping similar to when we're dropped.
        _body.bodyType = RigidbodyType2D.Dynamic;
        transform.parent = tileUsingUs.transform.parent;
        if(ifRelease)_tileHoldingUs.tileWereHolding = null;
        _tileHoldingUs = null;

        _collider.isTrigger = false;

        // Since we're thrown so fast, we switch to continuous collision detection to avoid tunnelling
        // through walls.
        _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Finally, here's where we get the throw force.
        _body.AddForce(throwDir * throwForce);


        _afterThrowCounter = afterThrowTime;
    }

    protected virtual void Update()
    {
        if (_isInAir)
        {
            if (_afterThrowCounter > 0)
            {
                _afterThrowCounter -= Time.deltaTime;
            }
            // If we've been in the air long enough, need to check if it's time to consider ourself "on the ground"
            else if (_body.linearVelocity.magnitude <= onGroundThreshold)
            {
                _body.linearVelocity = Vector2.zero;
                if (_afterThrowCounter <= 0 && _tileThatThrewUs != null && _tileThatThrewUs.GetComponent<Collider2D>() != null)
                {
                    Physics2D.IgnoreCollision(_tileThatThrewUs.GetComponent<Collider2D>(), _collider, false);
                }
                _body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
                _collider.isTrigger = true;
                addTag(TileTags.CanBeHeld);
                _isInAir = false;
            }
        }


        if (_tileHoldingUs != null)
        {
            // We aim the rock behind us.
            _sprite.transform.localPosition = new Vector3(-0.5f, 0, 0);
            float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0, 0, aimAngle);
        }
        else
        {
            _sprite.transform.localPosition = Vector3.zero;
        }


        updateSpriteSorting();
    }

    // When we collide with something in the air, we try to deal damage to it.
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isInAir && collision.gameObject.GetComponent<Tile>() != null)
        {
            float impact = collisionImpactLevel(collision);
            // First, make sure we're going fast enough to do damage
            if (impact <= damageThreshold)
            {
                return;
            }

            if (rangeObject != null)
            {
                rangeObject.SetActive(true);
                StartCoroutine(ExpandAndPull());
            }
        }
    }

    private IEnumerator ExpandAndPull()
    {
        initialScale = rangeObject.transform.localScale;
        float elapsedTime = 0f;
        while (elapsedTime < expandDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / expandDuration;
            rangeObject.transform.localScale = Vector3.Lerp(initialScale, targetScale, expandCurve.Evaluate(t));
            yield return null;
        }

       
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, pullRadius);
        foreach (Collider2D nearbyCollider in nearbyColliders)
        {
            Tile tile = nearbyCollider.GetComponent<Tile>();
            if (tile == this)
            {
                continue; 
            }
            if (tile != null)
            {
                
                Vector2 pullDirection = (transform.position - tile.transform.position).normalized;
                tile.addForce(pullDirection * pullForce*(tile.hasTag(TileTags.Creature)?creatureForceTime:1));
            }
        }


        base.die();
    }
}
