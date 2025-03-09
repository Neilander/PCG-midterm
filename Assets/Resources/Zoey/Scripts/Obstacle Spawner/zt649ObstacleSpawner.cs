using System;
using Unity.Mathematics;
using UnityEngine;

public class zt649ObstacleSpawner : Tile
{
    public GameObject[] obstaclePrefs;
    
    private bool m_pickUped;
    private int spawnedAmount = 0;
    
    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        m_pickUped = true;
    }
    
    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
        m_pickUped = false;
    }

    private void Start()
    {
        ShuffleObstacles();
    }

    private void Update()
    {
        
        if (m_pickUped && Input.GetMouseButtonDown(0))
        {
            Vector3 _obstacleDest = GetMouseTargetPos();

            Instantiate(obstaclePrefs[spawnedAmount], _obstacleDest, quaternion.identity);

            if (++spawnedAmount == obstaclePrefs.Length)
            {
                ShuffleObstacles();
                spawnedAmount = 0;
            }

        }
    }

    private void ShuffleObstacles()
    {
        GlobalFuncs.shuffle(obstaclePrefs);
    }

    private Vector3 GetMouseTargetPos()
    {
        Vector3 _mousePos = Input.mousePosition;
        Vector3 _target = Camera.main.ScreenToWorldPoint(_mousePos);
        _target.z = transform.position.z;
        return _target;
    }
}
