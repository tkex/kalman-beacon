using UnityEngine;

public class BasicSensor : MessSensor
{
  protected override string logFilePath
  {
    get
    {
      return Application.dataPath + "/basic_log.csv";
    }
  }

  protected override void PerformMeasurement()
  {
    base.PerformMeasurement();
  }
}