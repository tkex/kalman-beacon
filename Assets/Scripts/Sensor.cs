using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class Sensor : MonoBehaviour
{

    public float sensorAccStandardDeviation = 3f;
    public float compassAccStandardDeviation = 1f;

    private GameObject[] beacons;
    private string logFilePath = Application.dataPath + "/log.csv";

    public UnityEvent<Measurement> conductedMeasurement;
    public UnityEvent<float, float, float> sendSensorInfo;

    public UnityEvent<Measurement> onNewMeasurement;

    void Start()
    {
        beacons = GameObject.FindGameObjectsWithTag("Beacon");
        foreach (GameObject beacon in beacons)
        {
            var beaconScript = beacon.GetComponent<Beacon>();
            beaconScript.sendBroadcast.AddListener(ReceiveBeaconBroadcast);
        }
    }

    void ReceiveBeaconBroadcast(long sentTimestamp, int beaconId, Vector3 beaconPos)
    {
        float headingAngle = transform.localRotation.eulerAngles.z;
        float distortedHeadingAngle = GenerateRandomGaussian(headingAngle, compassAccStandardDeviation);
        float angleGroundTruth = GetZAngleBetweenSensorAndPosition(headingAngle, beaconPos);
        float distortedAngle = GenerateRandomGaussian(angleGroundTruth, sensorAccStandardDeviation);
        int beaconFlag = 1;

        Measurement measurement = new Measurement(timestamp: sentTimestamp, beaconId: beaconId, beaconPos: beaconPos, angleGroundTruth: angleGroundTruth, angleDistorted: distortedAngle, sensorSTD: sensorAccStandardDeviation, headingAngleGroundTruth: headingAngle, headingAngleDistorted: distortedHeadingAngle, compassSTD: compassAccStandardDeviation, beaconFlag: beaconFlag);
        LogMeasurement(measurement);

        // Trigger event for Websocket (see WebSocketClient).
        onNewMeasurement.Invoke(measurement);

        conductedMeasurement.Invoke(measurement);
        sendSensorInfo.Invoke(sensorAccStandardDeviation, compassAccStandardDeviation, headingAngle);
    }

    float GenerateRandomGaussian(float mean, float std)
    {
        float u1 = 1f - UnityEngine.Random.value; // Uniform random value from 0 to 1
        float u2 = 1f - UnityEngine.Random.value; // Another uniform random value

        // Box-Muller transform to get two independent standard normal random variables
        float z0 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);

        // Scale and shift to get the desired mean and standard deviation
        float gaussianValue = mean + z0 * std;

        return gaussianValue;
    }

    float GetZAngleBetweenSensorAndPosition(float localAngle, Vector3 position)
    {
        // Calculate the direction vector from position1 to position2
        Vector3 direction = transform.position - position;

        // Project the direction vector onto the XY plane (ignoring the Z component)
        Vector2 direction2D = new Vector2(direction.x, direction.y);

        float angle = Vector2.SignedAngle(Vector2.down, direction2D) - localAngle;

        // Ensure the angle is positive (between 0 and 360 degrees)
        return (angle >= 0) ? angle : angle += 360f;
    }

    void LogMeasurement(Measurement measurement)
    {
        string logText = measurement.GetCSVRepresentation();
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine(logText);
        }
    }
}
