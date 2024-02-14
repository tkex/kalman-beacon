using UnityEngine;

public class BeaconFlagFreqSensor : MessSensor
{
  protected override string logFilePath
  {
    get
    {
      return Application.dataPath + "/beacon_flag_AND_freq_log.csv";
    }
  }
  public float beaconVeryHighFallbackStd = 90f;
  protected Beacon[] beaconScripts;


  protected new void Start()
  {
    base.Start();
    beaconStds = new float[3] { 1, 1, 1 }; // Verrauscht sämtliche Messdaten für jeweilges Beacon
    beaconStds = new float[3] { beaconVeryHighFallbackStd, beaconVeryHighFallbackStd, beaconVeryHighFallbackStd };
    beaconScripts = new Beacon[3];

    for (int i = 0; i < beacons.Length; i++)
    {
      Beacon beaconScript = beacons[i].GetComponent<Beacon>();
      beaconScripts[i] = beaconScript;
      // Listen to beacons
      beaconScript.broadCastEvent.AddListener(SetBeaconFlagByBeaconId);
    }
  }

  protected override void PerformMeasurement()
  {
    for (int i = 0; i < beaconScripts.Length; i++)
    {
      if (!beaconScripts[i].beaconIsActive)
      {
        beaconStds[i] = beaconVeryHighFallbackStd;
      }
    }

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