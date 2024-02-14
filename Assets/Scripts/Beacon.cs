using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Beacon : MonoBehaviour
{
    public int beaconID;
    public float broadcastFrequencyInHz = 10;
    private float broadcastInterval;
    private float timeAtLastBroadcast = 0f;
    public float beaconStd = 1;

    public UnityEvent<int, float> broadCastEvent;

    void Start()
    {
        broadcastInterval = 1f / broadcastFrequencyInHz;
    }

    void Update()
    {
        if ((Time.realtimeSinceStartup - timeAtLastBroadcast) >= broadcastInterval)
        {
            BroadcastSignal();
            timeAtLastBroadcast = Time.realtimeSinceStartup;
        }
    }

    void BroadcastSignal()
    {
        // TODO: frequenz in millisekunden
        broadCastEvent.Invoke(beaconID, beaconStd);
    }
}
