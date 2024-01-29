using UnityEngine;

public class Measurement
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

  public Measurement(
    long timestamp,
    int beaconId,
    Vector3 beaconPos,
    float angleGroundTruth,
    float angleDistorted,
    float sensorSTD,
    float headingAngleGroundTruth,
    float headingAngleDistorted,
    float compassSTD,
    int beaconFlag
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
  }

  public string GetCSVRepresentation()
  {
    string csvString = $"{this.beaconFlag}\t{this.timestamp}\t{this.beaconId}\t{this.beaconPos}\t{this.angleGroundTruth}\t{this.angleDistorted}\t{this.sensorSTD}\t{this.headingAngleGroundTruth}\t{this.headingAngleDistorted}\t{this.compassSTD}";
    
    return csvString;
  }
}