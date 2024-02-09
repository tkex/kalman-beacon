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
      // Konvertieren beaconPos (Vector3) in  x, y, und z (für Python)
      string beaconPosX = this.beaconPos.x.ToString();
      string beaconPosY = this.beaconPos.y.ToString();
      string beaconPosZ = this.beaconPos.z.ToString();
      
      // Erstellen des CSV-Strings
      string csvString = $"{this.beaconFlag}\t{this.timestamp}\t{this.beaconId}\t{beaconPosX}\t{beaconPosY}\t{beaconPosZ}\t{this.angleGroundTruth}\t{this.angleDistorted}\t{this.sensorSTD}\t{this.headingAngleGroundTruth}\t{this.headingAngleDistorted}\t{this.compassSTD}\t{this.propulsion}\t{this.rudderAngle}";
      
      return csvString;
  }


  /*
  For Semicolon-delimiter:

  public string GetCSVRepresentation()
  {
      // Konvertieren des Vector3 beaconPos in drei separate Werte für x, y, und z
      string beaconPosX = this.beaconPos.x.ToString();
      string beaconPosY = this.beaconPos.y.ToString();
      string beaconPosZ = this.beaconPos.z.ToString();
      
      // Erstellen des CSV-Strings mit den separaten Werten
      string csvString = $"{this.beaconFlag};{this.timestamp};{this.beaconId};{beaconPosX};{beaconPosY};{beaconPosZ};{this.angleGroundTruth};{this.angleDistorted};{this.sensorSTD};{this.headingAngleGroundTruth};{this.headingAngleDistorted};{this.compassSTD};{this.propulsion};{this.rudderAngle}";
      
      return csvString;
  }
  */

}
