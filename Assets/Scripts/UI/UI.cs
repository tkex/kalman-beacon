using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    // index = id. Achtung: Mögliche Fehlerquelle
    public BeaconStats[] beaconStatsScripts;
    public SensorStats sensorStats;
    public BoatStats boatStats;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var boatScript = player.GetComponent<Boat>();
        boatScript.sendBoatInfo.AddListener(UpdateBoatInfo);
    }
    
    // TODO: implementiere wieder mit neuen Messungen
    void UpdateStatsWithMeasurement(Measurement measurement)
    {
        if (beaconStatsScripts[measurement.beaconId])
        {
            beaconStatsScripts[measurement.beaconId].SetStatsWithMeasurement(measurement);
        }
        else
        {
            Debug.LogWarning($"Kein BeaconStatsScript mit ID {measurement.beaconId}");
        }
    }

    void UpdateSensorInfo(float sensorSTD, float compassSTD, float headingGT)
    {
        this.sensorStats.SetStats(sensorSTD, compassSTD, headingGT);
    }

    void UpdateBoatInfo(float propulsion, float rudderAngle)
    {
        this.boatStats.SetStats( propulsion, rudderAngle);
    }

}
