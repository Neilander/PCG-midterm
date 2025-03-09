using System;
using System.Collections;
using UnityEngine;

public class zt649BulletRing : Tile
{
    [Header("Bullet Ring General")]
    [SerializeField] private GameObject m_bulletPrefb;
    [SerializeField] private int m_bulletAmount;
    [SerializeField] private float m_bulletForce;
    
    [Header("Bullet Ring Radius")]
    [SerializeField] private float m_ringRadius;
    [SerializeField] private float m_radiusOffset;
    [SerializeField] private float m_radiusNoiseScale = 2f;
    
    [Header("Bullet Ring Angle")]
    [SerializeField] [Range(-90,90)] private float m_initRotAngleOffset;
    [SerializeField] private float m_rotAngleChangeRate = .1f;
    
    [Header("Bullet Ring Spawn")]
    [SerializeField] private float m_spawnRate = 0.2f;
    [SerializeField] private float m_spawnRateOffset = 0.02f;
    [SerializeField] private float m_spawnTrackerIncre = 0.1f;
    [SerializeField] private int m_maxSpawnNum = 100;
    
    
    private bool m_firing = false;
    private float m_timer = 0;
    private int m_bulletIndexThisRound = 0;
    private float _m_spawnRateOffset;
    
    // Trackers used as parameters for procedural effects
    private float m_spawnTracker = 0;
    private float m_rotAngleTracker = 0;
    
    // Bag for performance
    private GameObject[] m_bulletBag;
    private int m_bulletIndex = 0;

    private void Start()
    {
        m_bulletBag = new GameObject[m_maxSpawnNum];
        _m_spawnRateOffset = m_spawnRateOffset;
    }

    private void Update()
    {
        if (!m_firing) return;
        m_timer+=Time.deltaTime;
        if (m_timer >= m_spawnRate)
        {
            SpawnBullet();
            m_timer = 0;
        }

        if (m_spawnRate / m_bulletAmount < _m_spawnRateOffset)
            _m_spawnRateOffset = m_spawnRate / m_bulletAmount;
        else
            _m_spawnRateOffset = m_spawnRateOffset;
    }

    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        m_firing = true;
    }

    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
        m_firing = false;
        StopAllCoroutines();
    }

    private void SpawnBullet()
    {
        m_spawnTracker += m_spawnTrackerIncre;
        m_rotAngleTracker += m_rotAngleChangeRate;
        
        float _ringRadius = m_ringRadius + (2 * Mathf.PerlinNoise1D(m_spawnTracker) - 1) * m_radiusNoiseScale;
        
        float _rotAngle = 360f / m_bulletAmount;
        float _initRotAngle = m_initRotAngleOffset + m_rotAngleTracker;

        StartCoroutine(SpawnSingleBullet(_ringRadius, _rotAngle, _initRotAngle));
    }

    private IEnumerator SpawnSingleBullet(float _ringRadius, float _rotAngle, float _initRotAngle)
    {
        yield return new WaitForSeconds(m_spawnRateOffset);
        Vector3 _localSpawnPos = new Vector3(0, _ringRadius + m_radiusOffset*m_bulletIndexThisRound, 0);
        _localSpawnPos = Quaternion.Euler(0, 0, _rotAngle * m_bulletIndexThisRound + _initRotAngle) * _localSpawnPos;
        GameObject _bullet = Instantiate(m_bulletPrefb, transform);
        _bullet.transform.localPosition = _localSpawnPos;
        _bullet.GetComponent<Rigidbody2D>().AddForce(Vector3.Normalize(_localSpawnPos)*m_bulletForce, ForceMode2D.Impulse);
            
        // Record bullet
        if (m_bulletBag[m_bulletIndex] != null)
        {
            Destroy(m_bulletBag[m_bulletIndex]);
        }
        m_bulletBag[m_bulletIndex] = _bullet;
        if (++m_bulletIndex >= m_maxSpawnNum)
            m_bulletIndex = 0;
        
        if (++m_bulletIndexThisRound >= m_bulletAmount)
        {
            m_bulletIndexThisRound = 0;
            yield break;
        }
        
        StartCoroutine(SpawnSingleBullet(_ringRadius, _rotAngle, _initRotAngle));
    }
}
