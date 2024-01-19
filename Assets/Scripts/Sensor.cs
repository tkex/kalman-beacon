using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Sensor : MonoBehaviour
{

    public float sensorAccStandardDeviation = 3f;
    private GameObject[] beacons;
    private string logFilePath = Application.dataPath + "/log.txt";

    void Start()
    {
        //
        beacons = GameObject.FindGameObjectsWithTag("Beacon");
        foreach (GameObject beacon in beacons)
        {
            var beaconScript = beacon.GetComponent<Beacon>();
            beaconScript.signalBroadcast.AddListener(ReceiveBeaconBroadcast);
        }

    }

    void ReceiveBeaconBroadcast(System.DateTime sentTimestamp, int beaconId, Vector3 beaconPos)
    {
        float angleGroundTruth = GetZAngleBetweenSensorAndPosition(beaconPos);
        float distortedAngle = GenerateRandomGaussian(angleGroundTruth);
        int beaconFlag = 1;
        LogBroadcast(timestamp: sentTimestamp, beaconId: beaconId, beaconPos: beaconPos, angleGroundTruth: angleGroundTruth, angleDistorted: distortedAngle, std: sensorAccStandardDeviation, beaconFlag: beaconFlag);

    }

    float GenerateRandomGaussian(float mean)
    {
        float u1 = 1f - UnityEngine.Random.value; // Uniform random value from 0 to 1
        float u2 = 1f - UnityEngine.Random.value; // Another uniform random value

        // Box-Muller transform to get two independent standard normal random variables
        float z0 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);

        // Scale and shift to get the desired mean and standard deviation
        float gaussianValue = mean + z0 * sensorAccStandardDeviation;

        return gaussianValue;
    }

    float GetZAngleBetweenSensorAndPosition(Vector3 position)
    {
        // Calculate the direction vector from position1 to position2
        Vector3 direction = transform.position - position;

        // Project the direction vector onto the XY plane (ignoring the Z component)
        Vector2 direction2D = new Vector2(direction.x, direction.y);

        // Calculate the angle in degrees
        float angle = Vector2.SignedAngle(Vector2.up, direction2D);

        // Ensure the angle is positive (between 0 and 360 degrees)
        return (angle >= 0) ? angle : angle += 360f;
    }

    void LogBroadcast(System.DateTime timestamp, int beaconId, Vector3 beaconPos, float angleGroundTruth, float angleDistorted, float std, int beaconFlag)
    {
        string logText = $"{beaconFlag}\t{timestamp}\t{beaconId}\t{beaconPos}\t{angleGroundTruth}\t{angleDistorted}\t{std}";
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine(logText);
        }
    }
}
