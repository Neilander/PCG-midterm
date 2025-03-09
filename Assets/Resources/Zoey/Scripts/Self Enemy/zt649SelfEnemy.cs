using System;
using System.Collections;
using UnityEngine;

public class zt649SelfEnemy : apt283AStarEnemy
{
    public TileTags enemyStateTags = 0;
    public float m_lifeDuration = 5f;
    
    private bool m_isEnemy = false;

    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
        ChangeToEnemy();
    }

    public override void Update()
    {
        if(m_isEnemy)
            base.Update();
    }

    public override void FixedUpdate()
    {
        if(m_isEnemy)
            base.FixedUpdate();
    }

    protected override void takeStep()
    {
        if(m_isEnemy)
            base.takeStep();
    }

    private void ChangeToEnemy()
    {
        m_isEnemy = true;
        tags = enemyStateTags;
        GetComponent<CircleCollider2D>().isTrigger = false;
        StartCoroutine(DestroySelf());
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(m_lifeDuration);
        Destroy(gameObject);
    }
}
