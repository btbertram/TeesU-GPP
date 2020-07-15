﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

/// <summary>
/// A script attached to a gameobject, which defines the game object as a Gathering Point
/// for the player to interact with and harvest resouces from.
/// </summary>
public class GatheringPoint : MonoBehaviour
{
    int _pointID;
    EGatherPointType _type;
    long _respawnTimer;
    Vector3 _pos;
    bool _isActive;
    GatheringConnection gatheringPointConneciton;

    public void LoadPoint(int ID, EGatherPointType type, Vector3 pos)
    {
        _pointID = ID;
        _type = type;
        _pos = pos;
    }

    public Vector3 GetPos()
    {
        return _pos;
    }

    public async void AsyncCheckIfRegrown()
    {
        if (!_isActive)
        {
            long currenttime = await ConnectionManager.AsyncQueryTimeNow();
            long lastGatheredTime = await gatheringPointConneciton.AsyncQueryGatherTime(_pointID);

            if(currenttime - lastGatheredTime >= _respawnTimer)
            {
                _isActive = true;
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gatheringPointConneciton = FindObjectOfType<GatheringConnection>();

        switch (_type)
        {
            case EGatherPointType.GoldGatherType:
            {
                _respawnTimer = 60000;
                break;
            }

            default:
            {
                break;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
