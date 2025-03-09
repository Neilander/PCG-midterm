using UnityEngine;
using System.Collections;

public class Neil03Rocker : Tile
{
    private Tile tileUsingUs;
    private Rigidbody2D tileBody;
    private float recordRotate;
    public float moveSpeed = 5f;
    public float turnSpeed = 200f;
    private bool ifDropped = false;
    private Vector2 recordDir;
    private bool ifUsed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (tileUsingUs != null && tileBody != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - tileBody.position).normalized;

            
            Vector2 currentVelocity = tileBody.linearVelocity.normalized;

          
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float currentAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

            float rotationStep = turnSpeed * Time.deltaTime;
            float clampedTargetAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationStep);

            
            Vector2 newDirection = new Vector2(Mathf.Cos(clampedTargetAngle * Mathf.Deg2Rad), Mathf.Sin(clampedTargetAngle * Mathf.Deg2Rad));

            
            tileBody.linearVelocity = newDirection * moveSpeed;

            tileBody.rotation = clampedTargetAngle;
            recordDir = newDirection;
        }
        if (ifDropped)
        {
            moveViaVelocity(recordDir, moveSpeed, 100);
        }

    }

    //can't be damaged!
    public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
    {
        Debug.Log("Rocket can't be damaged!");
        return;
    }

    public override void useAsItem(Tile tileUsingUs)
    {
        this.tileUsingUs = tileUsingUs;
        tileBody = tileUsingUs.GetComponent<Rigidbody2D>();
        recordRotate = tileBody.rotation;
        ifUsed = true;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ifDropped)
        {
            StopAllCoroutines();
            if (collision.GetComponent<Tile>() != null)
                collision.GetComponent<Tile>().takeDamage(this, 5,DamageType.Explosive);
            base.die();
        }
    }

    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
        tileUsingUs = null;
        tileDroppingUs.body.rotation = recordRotate;
        tileBody = null;

        if (ifUsed)
        {
            ifDropped = true;
            removeTag(TileTags.CanBeHeld);
            moveViaVelocity(recordDir, moveSpeed, 100);
            StartCoroutine(destroySelf());
        }
    }

    IEnumerator destroySelf()
    {
        yield return new WaitForSeconds(1);
        base.die();

    }
}
