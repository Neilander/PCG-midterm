using UnityEngine;
using System.Collections;

public class YoYo : Tile
{
    private bool isOnHand = true;  
    private SpriteRenderer _spriteRenderer;
    public GameObject _yoYoModel;  
    private Tile _tileUsingUs;
    private GameObject yoYo;
    private bool isReturning = false;  
    private float throwForce = 5f;  
    private float maxThrowDistance = 8f;  
    private Rigidbody2D yoYoRb; 
    private DistanceJoint2D yoYoJoint; 
    private LineRenderer lineRenderer; 

    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        isOnHand = true;
    }

    public override void useAsItem(Tile tileUsingUs)
    {
        _tileUsingUs = tileUsingUs;
        base.useAsItem(tileUsingUs);

        if (isOnHand)
        {
            if (tileUsingUs.hasTag(TileTags.Friendly))
            {
                Vector2 direction = tileUsingUs.aimDirection;
                yoYo = Instantiate(_yoYoModel);
                yoYo.transform.parent = null;
                yoYo.transform.rotation = transform.rotation;
                yoYo.transform.position = tileUsingUs.transform.position;
                yoYo.GetComponent<Tile>().init();

            
                yoYoRb = yoYo.GetComponent<Rigidbody2D>();
                yoYoRb.linearVelocity = direction.normalized * throwForce;

              
                yoYoJoint = yoYo.AddComponent<DistanceJoint2D>();
                yoYoJoint.connectedBody = tileUsingUs.GetComponent<Rigidbody2D>();
                yoYoJoint.autoConfigureDistance = false;
                yoYoJoint.distance = maxThrowDistance;
     

           
                lineRenderer = yoYo.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.05f;
                lineRenderer.endWidth = 0.05f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
                lineRenderer.positionCount = 2;

                _spriteRenderer = GetComponent<SpriteRenderer>();
                _spriteRenderer.enabled = false;
                isOnHand = false;

                StartCoroutine(UpdateLineRenderer());
                StartCoroutine(ReturnYoYoAfterTime());
            }
        }
    }

    private IEnumerator UpdateLineRenderer()
    {
        while (yoYo != null && _tileUsingUs != null)
        {
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, _tileUsingUs.transform.position);
                lineRenderer.SetPosition(1, yoYo.transform.position);
            }
            yield return null;
        }
    }

    private IEnumerator ReturnYoYoAfterTime()
    {
    
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ReturnYoYo());
    }

    private IEnumerator ReturnYoYo()
    {
        isReturning = true;

        
        while (yoYoJoint != null && yoYo != null)
        {
            yoYoJoint.distance = Mathf.Lerp(yoYoJoint.distance, 0f, Time.deltaTime * 3f);

       
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, _tileUsingUs.transform.position);
                lineRenderer.SetPosition(1, yoYo.transform.position);
            }

    
            if (yoYoJoint.distance < 0.1f)
            {
                Destroy(yoYo);
                isReturning = false;
                isOnHand = true;
                _spriteRenderer.enabled = true;
                break;
            }
            yield return null;
        }
    }
}
