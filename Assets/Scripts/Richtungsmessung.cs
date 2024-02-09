using UnityEngine;

public class Richtungsmessung
{
  public long timestamp;
  public int beaconId;
  public Vector3 beaconPos;
  public float angleGroundTruth;
  public float angleDistorted;
  public float sensorSTD;
  public float headingAngleGroundTruth;
  public float headingAngleDistorted;
  public float compassSTD;
  public int beaconFlag;
  public float propulsion;
  public float rudderAngle;

  public Richtungsmessung(
    long timestamp,
    int beaconId,
    Vector3 beaconPos,
    float angleGroundTruth,
    float angleDistorted,
    float sensorSTD,
    float headingAngleGroundTruth,
    float headingAngleDistorted,
    float compassSTD,
    int beaconFlag,
    float propulsion,
    float rudderAngle
    )
  {
    this.timestamp = timestamp;
    this.beaconId = beaconId;
    this.beaconPos = beaconPos;
    this.angleGroundTruth = angleGroundTruth;
    this.angleDistorted = angleDistorted;
    this.sensorSTD = sensorSTD;
    this.headingAngleGroundTruth = headingAngleGroundTruth;
    this.headingAngleDistorted = headingAngleDistorted;
    this.compassSTD = compassSTD;
    this.beaconFlag = beaconFlag;
    this.propulsion = propulsion;
    this.rudderAngle = rudderAngle;
  }

  public string GetCSVRepresentation()
  {
    string csvString = $"{this.beaconFlag};{this.timestamp};{this.beaconId};{this.beaconPos};{this.angleGroundTruth};{this.angleDistorted};{this.sensorSTD};{this.headingAngleGroundTruth};{this.headingAngleDistorted};{this.compassSTD};{this.propulsion};{this.rudderAngle}";
    
    return csvString;
  }
}
