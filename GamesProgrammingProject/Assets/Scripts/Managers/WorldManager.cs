using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class WorldManager : MonoBehaviour
{
    Timer WorldUpdateTimer;
    List<GatheringPoint> _gatheringPoints;

    // Start is called before the first frame update
    void Start()
    {
        _gatheringPoints = new List<GatheringPoint>();
        //foreach(GatheringPoint x in _gatheringPoints)
        //{
        //    Debug.Log(x.gameObject.name);
        //}

        TimerSetup();
        WorldUpdateTimer.Start();
        Debug.Log("World Started: " + WorldUpdateTimer.Enabled);
    }

    void TimerSetup()
    {
        WorldUpdateTimer = new Timer(1000);
        WorldUpdateTimer.AutoReset = true;
        WorldUpdateTimer.Elapsed += OnWorldUpdateTimerElapsed;
    }

    private void OnWorldUpdateTimerElapsed(object sender, ElapsedEventArgs e)
    {
        //Debug.Log("Ding!");
        CheckGatheringPointGrowth();
    }

    private void CheckGatheringPointGrowth()
    {

        foreach (GatheringPoint point in _gatheringPoints)
        {
            point.AsyncCheckIfRegrown();
        }
    }

    public void AddToGatheringPointsList(GatheringPoint point)
    {
        _gatheringPoints.Add(point);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
