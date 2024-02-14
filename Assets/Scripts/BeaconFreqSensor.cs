using UnityEngine;

public class BeaconFreqSensor : MessSensor
{
  protected override string logFilePath
  {
    get
    {
      return Application.dataPath + "/beacon_freq_log.csv";
    }
  }
  public float beaconVeryHighFallbackStd = 90f;

  protected new void Start()
  {
    base.Start();
    beaconStds = new float[3] { 1, 1, 1 }; // Verrauscht sämtliche Messdaten für jeweilges Beacon
    beaconStds = new float[3] { beaconVeryHighFallbackStd, beaconVeryHighFallbackStd, beaconVeryHighFallbackStd };

    // Listen to beacons
    for (int i = 0; i < beacons.Length; i++)
    {
      Beacon beaconScript = beacons[i].GetComponent<Beacon>();
      beaconScript.broadCastEvent.AddListener(SetBeaconFlagByBeaconId);
    }
  }

  protected override void PerformMeasurement()
  {
    base.PerformMeasurement();

    for (int i = 0; i < beaconStds.Length; i++)
    {
      beaconStds[i] = beaconVeryHighFallbackStd;
    }
  }

  void SetBeaconFlagByBeaconId(int beaconId, float beaconSTD)
  {
    beaconStds[beaconId] = beaconSTD;
  }
}