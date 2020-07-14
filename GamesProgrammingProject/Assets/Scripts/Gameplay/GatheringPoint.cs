using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GatheringPoint : MonoBehaviour
{
    int _pointID;
    EGatherPointType _type;
    Vector3 _pos;
    bool _isActive;


    public void LoadPoint(int ID, EGatherPointType type, Vector3 pos)
    {
        _pointID = ID;
        _type = type;
        _pos = pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
