using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float accSpeed = 2.5f; 
    public float propulsion = 0f;

    public float maxOarAngle = 45f;
    public float oarTurnSpeed = 30f;
    public float oarAngle = 0f;

    // factor for how much oar turns boat
    public float oarBoatTurnFactor = 0.3f;
    // Turn rotation speed of the boat
    // public float turnSpeed = 50f;

    // public float ruderReturnSpeed = 1f;


    void Update()
    {
      float verticalInput = Input.GetAxis("Vertical");
      float newPropulsion = propulsion + (verticalInput * accSpeed * Time.deltaTime);
      propulsion = Mathf.Clamp(newPropulsion, 0, maxSpeed);

      // inverse horizontal input s.t. right/left input leads to rigth/left rotation
      float horizontalInput = -Input.GetAxis("Horizontal");
      float newOarAngle = oarAngle + (horizontalInput * oarTurnSpeed * Time.deltaTime);
      oarAngle = Mathf.Clamp(newOarAngle, -maxOarAngle, maxOarAngle);

      // Gradually returns the ruder to center if there is no input
            // if (ruderPosition != 0)
                // float returnSign = Mathf.Sign(ruderPosition);
                // ruderPosition -= returnSign * ruderReturnSpeed * Time.deltaTime;
                // ruderPosition = Mathf.Clamp(ruderPosition, -1f, 1f); // Ensures ruderPosition stays within bounds
        // ruderPosition = Mathf.Clamp(ruderPosition, -1f, 1f);
    }

  void FixedUpdate(){

      // advance boat depending on propulsion
      var trajectory = GetCurrentTrajectory();
      var positionChange = trajectory * propulsion;
      transform.position += positionChange * Time.deltaTime;
      // turn boat depending on oar and "movespeed" of boat 
      var oarStirFactor = oarAngle * oarBoatTurnFactor * positionChange.magnitude;
      var rotationChange = Vector3.forward * oarStirFactor;
      transform.eulerAngles += rotationChange * Time.deltaTime; 
  }

    Vector3 GetCurrentTrajectory(){
      var heading = transform.localRotation.eulerAngles.z; 
      var trajectory = Quaternion.AngleAxis(heading, Vector3.forward) * Vector3.up;
      return trajectory;
    }

    void MoveBoatAlongYAxis()
    {
        transform.position += Vector3.up * maxSpeed;
    }
}
