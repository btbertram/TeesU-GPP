using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class WorldManager : MonoBehaviour
{
    Timer WorldUpdateTimer;
    GatheringPoint[] _gatheringPoints;

    // Start is called before the first frame update
    void Start()
    {
        _gatheringPoints = GameObject.FindObjectsOfType<GatheringPoint>();

        TimerSetup();
        WorldUpdateTimer.Start();
    }

    void TimerSetup()
    {
        WorldUpdateTimer = new Timer(10000);
        WorldUpdateTimer.AutoReset = true;
        WorldUpdateTimer.Elapsed += OnWorldUpdateTimerElapsed;
    }

    private void OnWorldUpdateTimerElapsed(object sender, ElapsedEventArgs e)
    {
        CheckGatheringPointGrowth();
    }

    private void CheckGatheringPointGrowth()
    {
        foreach(GatheringPoint point in _gatheringPoints)
        {
            point.AsyncCheckIfRegrown();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
