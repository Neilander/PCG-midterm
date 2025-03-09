using System;
using UnityEngine;

public class zt649PerlinEnemy : BasicAICreature
{
    [SerializeField] private float m_perlinNoiseMult = 100f;
    private void Update()
    {
        takeStep();
    }
    
    protected override void takeStep()
    {
        Vector2 _curPos = new Vector2(globalX, globalY);
        Vector2 _curGridPos = toGridCoord(globalX, globalY);
        Vector2 _deltaPos = new Vector2(Mathf.PerlinNoise(Time.time, Time.time),
            Mathf.PerlinNoise(Time.time + 100, Time.time + 100));
        
        /*_deltaPos = new Vector2(Mathf.PerlinNoise(_curPos.x, _curPos.y),
            Mathf.PerlinNoise(_curPos.x + 100, _curPos.y + 100));*/
        
        _deltaPos = (_deltaPos * 2 - Vector2.one) * 10;
        _curPos += _deltaPos;
        _curPos = toGridCoord(_curPos);
        _targetGridPos = _curPos;
    }
}