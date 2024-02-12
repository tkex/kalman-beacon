using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Richtungssensor : MonoBehaviour
{

    public GameObject[] beacons;
    public GameObject[] beaconDirPivots;
    public GameObject[] beaconDirectionPoints;
    public int abtastrateInHz = 10;
    private float abtastInterval;
    private float time = 0f;
    private int totalMeasurements = 0;

    public float STD = 1f;

    private float[] beaconAngles = new float[3];   //  in ships local modelspace, deg
    private float[] beaconAnglesDistorted = new float[3];  //   in ships local modelspace, deg
    private Vector2[] beaconDirectionsDistorted = new Vector2[3]; // in ships local modelspace

    private string logFilePath = Application.dataPath + "/log.csv";


    void Start()
    {
        abtastInterval = 1f / abtastrateInHz;
        beaconAngles = new float[3];
        beaconAnglesDistorted = new float[3];
        beaconDirectionsDistorted = new Vector2[3];
        time = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if ((Time.realtimeSinceStartup - time) >= abtastInterval)
        {
            PerformMeasurement();
            //VisualizeLatestMeasurement();
            time = Time.realtimeSinceStartup;
        }
    }

    void PerformMeasurement()
    {
        beaconAngles[0] = GetRelativeAngleToPosition(beacons[0].transform.position);
        beaconAngles[1] = GetRelativeAngleToPosition(beacons[1].transform.position);
        beaconAngles[2] = GetRelativeAngleToPosition(beacons[2].transform.position);

        beaconAnglesDistorted[0] = GenerateRandomGaussian(beaconAngles[0], STD);
        beaconAnglesDistorted[1] = GenerateRandomGaussian(beaconAngles[1], STD);
        beaconAnglesDistorted[2] = GenerateRandomGaussian(beaconAngles[2], STD);

        float rad = 3.14159f * beaconAnglesDistorted[0] / 180.0f;
        beaconDirectionsDistorted[0] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        
        rad = 3.14159f * beaconAnglesDistorted[1] / 180.0f;
        beaconDirectionsDistorted[1] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        
        rad = 3.14159f * beaconAnglesDistorted[2] / 180.0f;
        beaconDirectionsDistorted[2] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        
        Debug.Log("Beacon 0: " + beaconAnglesDistorted[0] + ": " + beaconDirectionsDistorted[0].ToString());
        Debug.Log("Beacon 1: " + beaconAnglesDistorted[1] + ": "  + beaconDirectionsDistorted[1].ToString());
        Debug.Log("Beacon 2: " + beaconAnglesDistorted[2] + ": "  + beaconDirectionsDistorted[2].ToString());

        //SetBeaconDirPivotsByDistortedAngles();
        //SetBeaconDirections();

        WriteLog();

        totalMeasurements += 1;
    }

    float GetRelativeAngleToPosition(Vector3 position)
    {
        Vector2 xAxis = new Vector2(transform.right.x, transform.right.y);
        // float localAngle = transform.localRotation.eulerAngles.z;
        Vector3 direction = position - transform.position;
        // Project the direction vector onto the XY plane (ignoring the Z component)
        Vector2 direction2D = new Vector2(direction.x, direction.y);
        // float angle = Vector2.SignedAngle(Vector2.down, direction2D) - localAngle;
        float angle = Vector2.SignedAngle(xAxis, direction2D);
        // Ensure the angle is positive (between 0 and 360 degrees)
        return (angle >= 0) ? angle : angle += 360f;
    }

    void SetBeaconDirPivotsByDistortedAngles()
    {
        beaconDirPivots[0].transform.eulerAngles = new Vector3(0, 0, beaconAnglesDistorted[0]);
        beaconDirPivots[1].transform.eulerAngles = new Vector3(0, 0, beaconAnglesDistorted[1]);
        beaconDirPivots[2].transform.eulerAngles = new Vector3(0, 0, beaconAnglesDistorted[2]);
    }

    void SetBeaconDirections()
    {
        Vector3 dirBeacon0 = beaconDirectionPoints[0].transform.position - transform.position;
        Vector3 dirBeacon1 = beaconDirectionPoints[1].transform.position - transform.position;
        Vector3 dirBeacon2 = beaconDirectionPoints[2].transform.position - transform.position;
        dirBeacon0.Normalize();
        dirBeacon1.Normalize();
        dirBeacon2.Normalize();
        beaconDirectionsDistorted[0] = new Vector2(dirBeacon0.x, dirBeacon0.y);
        beaconDirectionsDistorted[1] = new Vector2(dirBeacon1.x, dirBeacon1.y);
        beaconDirectionsDistorted[2] = new Vector2(dirBeacon2.x, dirBeacon2.y);
    }

    void VisualizeLatestMeasurement()
    {
        Debug.Log($"### MEASUREMENT {totalMeasurements} ###");
        // Debug.Log("-- angles GT");
        // Debug.Log($"BEACON-0: {beaconAnglesGT[0]}");
        // Debug.Log($"BEACON-1: {beaconAnglesGT[1]}");
        // Debug.Log($"BEACON-2: {beaconAnglesGT[2]}");
        // Debug.Log("-- angles Distorted");
        // Debug.Log($"BEACON-0: {beaconAnglesDistorted[0]}");
        // Debug.Log($"BEACON-1: {beaconAnglesDistorted[1]}");
        // Debug.Log($"BEACON-2: {beaconAnglesDistorted[2]}");
        Debug.Log("-- beacon directions distorted");
        Debug.Log($"BEACON-0: {beaconDirectionsDistorted[0]}");
        Debug.Log($"BEACON-1: {beaconDirectionsDistorted[1]}");
        Debug.Log($"BEACON-2: {beaconDirectionsDistorted[2]}");
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


    /* The REAL CSV log function.
    -----------------------------
    'Zeit_Index',
    'X_GT',  (X-Koordinate Ground Truth)
    'Y_GT',  (Y-Koordinate Ground Truth)
    'Richtung_X_B0', 'Richtung_Y_B0', 'STD_Grad_B0',  (Daten Beacon 0)
    'Richtung_X_B1', 'Richtung_Y_B1', 'STD_Grad_B1',  (Daten  Beacon 1)
    'Richtung_X_B2', 'Richtung_Y_B2', 'STD_Grad_B2'   (Daten für Beacon 2)
    */
    void WriteLog()
    {
        // runde auf 3 Dezimalstellen! 
        string logText = $"{totalMeasurements}\t{transform.position.x}\t{transform.position.y}";

        logText += $"\t{beaconDirectionsDistorted[0].x}\t{beaconDirectionsDistorted[0].y}\t{STD}";
        logText += $"\t{beaconDirectionsDistorted[1].x}\t{beaconDirectionsDistorted[1].y}\t{STD}";
        logText += $"\t{beaconDirectionsDistorted[2].x}\t{beaconDirectionsDistorted[2].y}\t{STD}";

        logText = logText.Replace(',', '.');

        //Debug.Log("Log: " + logText);

        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine(logText);
        }
    }
}