using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Beacon : MonoBehaviour
{
    public int beaconID;
    public int broadcastFrequency = 30;
    private int framesSinceLastBroadcast = 0;

    public UnityEvent<System.DateTime, int, Vector3> signalBroadcast;

    void Update()
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
        signalBroadcast.Invoke(System.DateTime.Now, beaconID, transform.position);
    }
}
