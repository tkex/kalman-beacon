using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeaconStats : MonoBehaviour
{

    public TextMeshProUGUI id;
    public TextMeshProUGUI position;
    public TextMeshProUGUI timestamp;
    public TextMeshProUGUI flag;
    public TextMeshProUGUI angle;
    public TextMeshProUGUI angleDistorted;

    // TODO: Measurement is deprecated
    // public void SetStatsWithMeasurement(Measurement measurement)
    // {
    //     this.id.text = measurement.beaconId.ToString();
    //     this.position.text = measurement.beaconPos.ToString();
    //     this.timestamp.text = measurement.timestamp.ToString();
    //     this.flag.text = measurement.beaconFlag.ToString();
    //     this.angle.text = measurement.angleGroundTruth.ToString();
    //     this.angleDistorted.text = measurement.angleDistorted.ToString();
    // }
}
