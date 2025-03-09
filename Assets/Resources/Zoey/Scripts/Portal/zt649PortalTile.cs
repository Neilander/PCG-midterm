using System;
using Unity.Mathematics;
using UnityEngine;

public class zt649PortalTile : Tile
{
    public GameObject portalDoorPref;
    public float portalCD = 1f;
    
    private bool m_pickUped;
    private Transform m_portal_1;
    private Transform m_portal_2;
    
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
    
    private void Update()
    {
        if (m_pickUped && Input.GetMouseButtonDown(0))
        {
            Vector3 _portalDest = GetMouseTargetPos();
            
            if (m_portal_1 == null)
            {
                m_portal_1 = Instantiate(portalDoorPref, _portalDest, quaternion.identity).transform;
            } else if (m_portal_2 == null)
            {
                m_portal_2 = Instantiate(portalDoorPref, _portalDest, quaternion.identity).transform;
                zt649PortalDoor _door_1 = m_portal_1.gameObject.GetComponent<zt649PortalDoor>();
                zt649PortalDoor _door_2 = m_portal_2.gameObject.GetComponent<zt649PortalDoor>();
                _door_1.SetOtherDoor(m_portal_2);
                _door_2.SetOtherDoor(m_portal_1);
                _door_1.SetCD(portalCD);
                _door_2.SetCD(portalCD);
            }
            else
            {
                Destroy(m_portal_1.gameObject);
                Destroy(m_portal_2.gameObject);
            }
            
        }
    }

    private Vector3 GetMouseTargetPos()
    {
        Vector3 _mousePos = Input.mousePosition;
        Vector3 _target = Camera.main.ScreenToWorldPoint(_mousePos);
        _target.z = transform.position.z;
        return _target;
    }
}
