using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Messsensor: MonoBehaviour
{

    public GameObject[] beacons;
    public GameObject[] beaconDirPivots;
    public GameObject[] beaconDirectionPoints;

    // Sensor Settings
    public int abtastrateInHz = 10;
    private float abtastInterval;
    private float timeAtLastMeasurement = 0f; 
    private int totalMeasurements = 0; 
    // Log File Paths
    private string simpleLogFilePath = Application.dataPath + "/simple-log.csv";
    private string extendedLogFilePath = Application.dataPath + "/extended-log.csv";

    // Heading
    private float heading;  // Winkeländerung zur Ursprungsausrichtung (Norden = 0°)
    private float headingDistorted; //
    public float headingSTD = 1f;
    
    // Angles (TODO: unbounded Angles)
    private float[] beaconAngles = new float[3];   //  in ships local modelspace, deg
    private float[] beaconAnglesDistorted = new float[3];  //   in ships local modelspace, deg
    
    // Directions (werden aus angles berechnet)
    private Vector2[] beaconDirections = new Vector2[3]; // in ships local modelspace  
    private Vector2[] beaconDirectionsDistorted = new Vector2[3]; // in ships local modelspace  

    // Entfernung (skalierte directions)
    private float[] beaconEntfernungen = new float[3];
    private float[] beaconEntfernungenDistorted = new float[3];

    public float[] beaconStds = new float[3]{1, 1, 1}; // verrauscht sämtliche Messdaten für jeweilges Beacon
    

    void Start()
    {
        abtastInterval = 1f / abtastrateInHz;
        timeAtLastMeasurement = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if ((Time.realtimeSinceStartup - timeAtLastMeasurement) >= abtastInterval)
        {
            PerformMeasurement();
            timeAtLastMeasurement = Time.realtimeSinceStartup;
        }
    }

    void PerformMeasurement()
    {
        CalcAndSetHeading();
        CalcAndSetBeaconAngles();
        CalcAndSetBeaconDirections();
        CalcAndSetEntfernungen();

        // Debug.Log("Beacon 0: " + beaconAnglesDistorted[0] + ": " + beaconDirectionsDistorted[0].ToString());
        // Debug.Log("Beacon 1: " + beaconAnglesDistorted[1] + ": "  + beaconDirectionsDistorted[1].ToString());
        // Debug.Log("Beacon 2: " + beaconAnglesDistorted[2] + ": "  + beaconDirectionsDistorted[2].ToString());

        WriteSimpleLog();
        WriteExtendedLog();

        totalMeasurements += 1;
    }

    void CalcAndSetHeading()
    {
      // East, weil transform.right in GetRelativeAngleToPosition verwendet wird
      Vector3 pointEastOfShip = transform.position + new Vector3(1,0,0);
      heading = GetRelativeAngleToPosition(pointEastOfShip);
      headingDistorted = GenerateRandomGaussian(heading, headingSTD);
    }

    void CalcAndSetBeaconAngles()
    {
        beaconAngles[0] = GetRelativeAngleToPosition(beacons[0].transform.position);
        beaconAngles[1] = GetRelativeAngleToPosition(beacons[1].transform.position);
        beaconAngles[2] = GetRelativeAngleToPosition(beacons[2].transform.position);

        beaconAnglesDistorted[0] = GenerateRandomGaussian(beaconAngles[0], beaconStds[0]);
        beaconAnglesDistorted[1] = GenerateRandomGaussian(beaconAngles[1], beaconStds[1]);
        beaconAnglesDistorted[2] = GenerateRandomGaussian(beaconAngles[2], beaconStds[2]);
    }

    void CalcAndSetBeaconDirections()
    {
        // FRAGE: Warum funktioniert das hier? 
        // FRAGE: ist das hier mit ein Grund, weshalb GetRelativeAngleToPosition transform.right verwendet?
        // undistorted directions
        float rad = 3.14159f * beaconAngles[0] / 180.0f;
        beaconDirections[0] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        rad = 3.14159f * beaconAngles[1] / 180.0f;
        beaconDirections[1] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        rad = 3.14159f * beaconAngles[2] / 180.0f;
        beaconDirections[2] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        // distorted directions
        rad = 3.14159f * beaconAnglesDistorted[0] / 180.0f;
        beaconDirectionsDistorted[0] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        rad = 3.14159f * beaconAnglesDistorted[1] / 180.0f;
        beaconDirectionsDistorted[1] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        rad = 3.14159f * beaconAnglesDistorted[2] / 180.0f;
        beaconDirectionsDistorted[2] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    void CalcAndSetEntfernungen()
    {
        // Berechnung der direkten Entfernungen zu den Beacons
        float distanceBeacon0 = Vector3.Distance(beacons[0].transform.position, transform.position);
        float distanceBeacon1 = Vector3.Distance(beacons[1].transform.position, transform.position);
        float distanceBeacon2 = Vector3.Distance(beacons[2].transform.position, transform.position);

        // Debug Log
        Debug.Log($"Distanz zu Beacon 0: {distanceBeacon0}");
        Debug.Log($"Distanz zu Beacon 1: {distanceBeacon1}");
        Debug.Log($"Distanz zu Beacon 2: {distanceBeacon2}");


        // Speichern der direkten Entfernungen (ohne Richtungsberechnung)
        beaconEntfernungen[0] = distanceBeacon0; // Direkte Entfernung zu Beacon 0
        beaconEntfernungen[1] = distanceBeacon1; // Direkte Entfernung zu Beacon 1
        beaconEntfernungen[2] = distanceBeacon2; // Direkte Entfernung zu Beacon 2

        // Verrauschte Entfernungen
        beaconEntfernungenDistorted[0] = distanceBeacon0 + GenerateRandomGaussian(0, beaconStds[0]);
        beaconEntfernungenDistorted[1] = distanceBeacon1 + GenerateRandomGaussian(0, beaconStds[1]);
        beaconEntfernungenDistorted[2] = distanceBeacon2 + GenerateRandomGaussian(0, beaconStds[2]);
    }


    float GetRelativeAngleToPosition(Vector3 position)
    {
        // FRAGE: Wieso right und nicht up?
        Vector2 xAxis = new Vector2(transform.right.x, transform.right.y);
        // float localAngle = transform.localRotation.eulerAngles.z;
        Vector3 direction = position - transform.position;
        // Project the direction vector onto the XY plane (ignoring the Z component)
        Vector2 direction2D = new Vector2(direction.x, direction.y);
        // float angle = Vector2.SignedAngle(Vector2.down, direction2D) - localAngle;
        float angle = Vector2.SignedAngle(xAxis, direction2D);
        // Ensure the angle is positive (between 0 and 360 degrees)
        // return (angle >= 0) ? angle : angle += 360f;
        return angle;
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


    /* The REAL CSV log functions.
    -----------------------------
    Simple log format:
    'Zeit_Index',
    'X_GT',  (X-Koordinate Ground Truth)
    'Y_GT',  (Y-Koordinate Ground Truth)
    'Richtung_X_B0', 'Richtung_Y_B0', 'STD_Grad_B0',  (Daten Beacon 0)
    'Richtung_X_B1', 'Richtung_Y_B1', 'STD_Grad_B1',  (Daten  Beacon 1)
    'Richtung_X_B2', 'Richtung_Y_B2', 'STD_Grad_B2'   (Daten für Beacon 2)
    */
    void WriteSimpleLog()
    {
        // TODO: runde auf 3 Dezimalstellen! 
        string logText = $"{totalMeasurements}\t{transform.position.x}\t{transform.position.y}";
        logText += $"\t{beaconDirectionsDistorted[0].x}\t{beaconDirectionsDistorted[0].y}\t{beaconStds[0]}";
        logText += $"\t{beaconDirectionsDistorted[1].x}\t{beaconDirectionsDistorted[1].y}\t{beaconStds[1]}";
        logText += $"\t{beaconDirectionsDistorted[2].x}\t{beaconDirectionsDistorted[2].y}\t{beaconStds[2]}";

        logText = logText.Replace(',', '.');

        using (StreamWriter writer = new StreamWriter(simpleLogFilePath, true))
        {
            writer.WriteLine(logText);
        }
    }

    /*
    Extended log format:

    -- Zeit
    [0]  'Zeit_Index',

    -- Position
    [1]  'X_GT',  (X-Koordinate Ground Truth)
    [2]  'Y_GT',  (Y-Koordinate Ground Truth)
    
    -- Heading
    [3]  'Heading_GT'  (Heading - Winkeländerung zu Ausgangsposition Norden)
    [4]  'Heading'  (Heading distorted)
    [5]  'Heading_STD' (STD für Heading)
    
    -- Angles 
    [6]  'angle_GT_B0', 'angleDistorted_B0', 'STD_Grad_B0',  (Daten Beacon 0)
    [9]  'angle_GT_B1', 'angleDistorted_B1', 'STD_Grad_B1',  (Daten Beacon 1)
    [12] 'angle_GT_B2', 'angleDistorted_B2', 'STD_Grad_B2',  (Daten Beacon 2)

    -- Directions
    [13] 'Richtung_GT_X_B0', 'Richtung_GT_Y_B0',
    [15] 'Richtung_X_B0', 'Richtung_Y_B0', 'STD_Grad_B0'
    [18] 'Richtung_GT_X_B1', 'Richtung_GT_Y_B1', 
    [20] 'Richtung_X_B1', 'Richtung_Y_B1', 'STD_Grad_B1'
    [23] 'Richtung_GT_X_B2', 'Richtung_GT_Y_B2',
    [25] 'Richtung_X_B2', 'Richtung_Y_B1', 'STD_Grad_B2' 
    
    -- Absolute Entfernungen 
    Direct distances to the beacons from the ship, noisy added distance and std
    [28] 'Entfernung_B0', 'EntfernungDistorted_B0', 'STD_Entfernung_B0',
    [31] 'Entfernung_B1', 'EntfernungDistorted_B1', 'STD_Entfernung_B1',
    [34] 'Entfernung_B2', 'EntfernungDistorted_B2', 'STD_Entfernung_B2'

    */

    void WriteExtendedLog()
    {
        // TODO: runde auf 3 Dezimalstellen! 
        // TODO: setze indixes händisch um Fehler zu vermeiden
        string logText = $"{totalMeasurements}";

        // position
        logText += $"\t{transform.position.x}\t{transform.position.y}";

        // heading
        logText += $"\t{heading}\t{headingDistorted}\t{headingSTD}";

        // angles
        logText += $"\t{beaconAngles[0]}\t{beaconAnglesDistorted[0]}\t{beaconStds[0]}";
        logText += $"\t{beaconAngles[1]}\t{beaconAnglesDistorted[1]}\t{beaconStds[1]}";
        logText += $"\t{beaconAngles[2]}\t{beaconAnglesDistorted[2]}\t{beaconStds[2]}";

        // directions
        logText += $"\t{beaconDirections[0].x}\t{beaconDirections[0].y}";
        logText += $"\t{beaconDirectionsDistorted[0].x}\t{beaconDirectionsDistorted[0].y}\t{beaconStds[0]}";
        logText += $"\t{beaconDirections[1].x}\t{beaconDirections[1].y}";
        logText += $"\t{beaconDirectionsDistorted[1].x}\t{beaconDirectionsDistorted[1].y}\t{beaconStds[1]}";
        logText += $"\t{beaconDirections[2].x}\t{beaconDirections[2].y}";
        logText += $"\t{beaconDirectionsDistorted[2].x}\t{beaconDirectionsDistorted[2].y}\t{beaconStds[2]}";
      
        // absolute distances to the beacons
        logText += $"\t{beaconEntfernungen[0]}\t{beaconEntfernungenDistorted[0]}\t{beaconStds[0]}";
        logText += $"\t{beaconEntfernungen[1]}\t{beaconEntfernungenDistorted[1]}\t{beaconStds[1]}";
        logText += $"\t{beaconEntfernungen[2]}\t{beaconEntfernungenDistorted[2]}\t{beaconStds[2]}";

        logText = logText.Replace(',', '.');

        using (StreamWriter writer = new StreamWriter(extendedLogFilePath, true))
        {
            writer.WriteLine(logText);
        }
    }
}
