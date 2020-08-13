using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class WorldManager : MonoBehaviour
{
    Timer WorldUpdateTimer;
    List<GatheringPoint> _gatheringPoints;

    IEnumerator WorldUpdateRate()
    {
        for(; ; )
        {
            CheckGatheringPointGrowth();            
            yield return new WaitForSeconds(3);
        }
    }

    private void CheckGatheringPointGrowth()
    {
        foreach (GatheringPoint point in _gatheringPoints)
        {
            point.CheckIfRegrownAsync();
        }
    }

    public void AddToGatheringPointsList(GatheringPoint point)
    {
        _gatheringPoints.Add(point);
    }

    // Start is called before the first frame update
    void Start()
    {
        _gatheringPoints = new List<GatheringPoint>();
        StartCoroutine(WorldUpdateRate());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
