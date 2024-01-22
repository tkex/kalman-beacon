using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    // index = id. Achtung: Mögliche Fehlerquelle
    public BeaconStats[] beaconStatsScripts;
    public SensorStats sensorStats;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var sensorScript = player.GetComponent<Sensor>();
        sensorScript.conductedMeasurement.AddListener(UpdateStatsWithMeasurement);
        sensorScript.sendSensorInfo.AddListener(UpdateSensorInfo);
    }

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

}
