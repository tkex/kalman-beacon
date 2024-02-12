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

    // Log File Path
    private string extendedLogFilePath = Application.dataPath + "/log.csv";

    // Heading
    private float heading;  // Winkeländerung zur Ursprungsausrichtung (Norden = 0°)
    private float headingDistorted;
    public float headingSTD = 1f;
    
    // Angles
    private float[] beaconAngles = new float[3];   //  in ships local modelspace, deg
    private float[] beaconAnglesDistorted = new float[3];  //   in ships local modelspace, deg
    
    // Directions (werden aus Angles berechnet)
    private Vector2[] beaconDirections = new Vector2[3]; // in ships local modelspace  
    private Vector2[] beaconDirectionsDistorted = new Vector2[3]; // in ships local modelspace  

    // Entfernung (skalierte Directions)
    private float[] beaconEntfernungen = new float[3];
    private float[] beaconEntfernungenDistorted = new float[3];

    public float[] beaconStds = new float[3]{1, 1, 1}; // Verrauscht sämtliche Messdaten für jeweilges Beacon
    

    void Start()
    {
        abtastInterval = 1f / abtastrateInHz;
        timeAtLastMeasurement = Time.realtimeSinceStartup;

        DeleteInitialLogFile();
    }

    void Update()
    {
        if ((Time.realtimeSinceStartup - timeAtLastMeasurement) >= abtastInterval)
        {
            PerformMeasurement();
            timeAtLastMeasurement = Time.realtimeSinceStartup;
        }
    }

    void DeleteInitialLogFile()
    {

        // Make sure to delete old file
        File.Delete(extendedLogFilePath);
        //Debug.Log(extendedLogFilePath);

        /* 
         * *** ONLY THE FIRST ROW IN THE LOG ***
         * => 8 values for Python read in.
        */

        // Log proper beacon positions
        Vector3 beacon0_pos = beacons[0].transform.position;
        Vector3 beacon1_pos = beacons[1].transform.position;
        Vector3 beacon2_pos = beacons[2].transform.position;



        // *** LOGGING STARTS HERE ***

        // First columns for identifying
        string logText = "-1";

        /* 
         * Individual beacon positions:
         * beacon_0_pos_x, beacon_0_pos_y 
         * beacon_1_pos_x, beacon_1_pos_y 
         * beacon_2_pos_x, beacon_2_pos_y 
        */

        logText += $"\t{beacon0_pos.x}\t{beacon0_pos.y}";
        logText += $"\t{beacon1_pos.x}\t{beacon1_pos.y}";
        logText += $"\t{beacon2_pos.x}\t{beacon2_pos.y}";

        // Initial state of boat (x, y Position aka GT)
        // Position x, Position y
        logText += $"\t{transform.position.x}\t{transform.position.y}";
 
        logText = logText.Replace(',', '.');

        using (StreamWriter writer = new StreamWriter(extendedLogFilePath, true))
        {
            writer.WriteLine(logText);
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

    private void CalcAndSetBeaconAngles()
    {
        for (int i = 0; i < beaconAngles.Length; i++)
        {
            beaconAngles[i] = GetRelativeAngleToPosition(beacons[i].transform.position);
            beaconAnglesDistorted[i] = GenerateRandomGaussian(beaconAngles[i], beaconStds[i]);
        }
    }


    void CalcAndSetBeaconDirections()
    {
        // FRAGE: Warum funktioniert das hier?
        // FRAGE: ist das hier mit ein Grund, weshalb GetRelativeAngleToPosition transform.right verwendet?

        // undistorted directions
        float rad = Mathf.PI * beaconAngles[0] / 180.0f;
        beaconDirections[0] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        rad = Mathf.PI * beaconAngles[1] / 180.0f;
        beaconDirections[1] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        rad = Mathf.PI * beaconAngles[2] / 180.0f;
        beaconDirections[2] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        // distorted directions
        rad = Mathf.PI * beaconAnglesDistorted[0] / 180.0f;

        beaconDirectionsDistorted[0] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        rad = Mathf.PI * beaconAnglesDistorted[1] / 180.0f;
        beaconDirectionsDistorted[1] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        rad = Mathf.PI * beaconAnglesDistorted[2] / 180.0f;
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


    /*
    Extended log format:

    (ERST AB 2. ZEILE. ERSTE ZEILE IST SIEHE LogFile-Funktion)

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

        // Position
        logText += $"\t{transform.position.x}\t{transform.position.y}";

        // Heading
        logText += $"\t{heading}\t{headingDistorted}\t{headingSTD}";

        // Angles
        logText += $"\t{beaconAngles[0]}\t{beaconAnglesDistorted[0]}\t{beaconStds[0]}";
        logText += $"\t{beaconAngles[1]}\t{beaconAnglesDistorted[1]}\t{beaconStds[1]}";
        logText += $"\t{beaconAngles[2]}\t{beaconAnglesDistorted[2]}\t{beaconStds[2]}";

        // Directions
        logText += $"\t{beaconDirections[0].x}\t{beaconDirections[0].y}";
        logText += $"\t{beaconDirectionsDistorted[0].x}\t{beaconDirectionsDistorted[0].y}\t{beaconStds[0]}";
        logText += $"\t{beaconDirections[1].x}\t{beaconDirections[1].y}";
        logText += $"\t{beaconDirectionsDistorted[1].x}\t{beaconDirectionsDistorted[1].y}\t{beaconStds[1]}";
        logText += $"\t{beaconDirections[2].x}\t{beaconDirections[2].y}";
        logText += $"\t{beaconDirectionsDistorted[2].x}\t{beaconDirectionsDistorted[2].y}\t{beaconStds[2]}";
      
        // Absolute distances to the beacons
        logText += $"\t{beaconEntfernungen[0]}\t{beaconEntfernungenDistorted[0]}\t{beaconStds[0]}";
        logText += $"\t{beaconEntfernungen[1]}\t{beaconEntfernungenDistorted[1]}\t{beaconStds[1]}";
        logText += $"\t{beaconEntfernungen[2]}\t{beaconEntfernungenDistorted[2]}\t{beaconStds[2]}";


        // Replace comma with dots for proper log file seperation into columns
        logText = logText.Replace(',', '.');

        using (StreamWriter writer = new StreamWriter(extendedLogFilePath, true))
        {
            writer.WriteLine(logText);
        }
    }
}
