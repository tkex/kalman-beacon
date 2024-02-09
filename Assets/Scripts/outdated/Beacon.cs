using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Beacon : MonoBehaviour
{
    public int beaconID;
    public int broadcastFrequency = 30;
    private int framesSinceLastBroadcast = 0;

    public UnityEvent<long, int, Vector3> sendBroadcast;

    void FixedUpdate()
    {
        if (framesSinceLastBroadcast >= broadcastFrequency)
        {
            BroadcastSignal();
            framesSinceLastBroadcast = 0;
        }
        else
        {
            framesSinceLastBroadcast += 1;
        }
    }

    void BroadcastSignal()
    {
        // TODO: frequenz in millisekunden
        sendBroadcast.Invoke(DateTimeOffset.Now.ToUnixTimeMilliseconds(), beaconID, transform.position);
    }
}
