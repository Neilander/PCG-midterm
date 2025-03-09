using System;
using UnityEngine;

public class zt649PortalDoor : MonoBehaviour
{
    public LayerMask playerLayer;
    
    private Transform m_otherDoor;
    private float m_cd;
    private float m_cdTimer = 0;

    public void SetOtherDoor(Transform _otherDoor)
    {
        m_otherDoor = _otherDoor;
    }

    public void SetCD(float _cd)
    {
        m_cd = _cd;
    }

    public void StartCD()
    {
        m_cdTimer = m_cd;
    }

    private void Update()
    {
        if (m_cdTimer > 0)
        {
            m_cdTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_cdTimer > 0) return;
        
        bool _isPlayer = false;

        if ((playerLayer.value & (1 << other.transform.gameObject.layer)) > 0)
            _isPlayer = true;
        
        if (_isPlayer && m_otherDoor != null)
        {
            Vector3 _newPos = m_otherDoor.transform.position;
            _newPos.z = other.transform.position.z;
            other.transform.position = _newPos;
            
            // Set cd
            m_otherDoor.GetComponent<zt649PortalDoor>().StartCD();
            StartCD();
        }
    }
}
