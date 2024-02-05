using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boat : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float accSpeed = 2.5f; 
    public float propulsion = 0f;

    public float rudderMaxAngle = 45f;
    public float rudderTurnSpeed = 25f;
    public float rudderReturnSpeed = 10f;
    public float rudderAngle = 0f;
    // TODO: refactor into UI or seperate Visualizer class 
    public GameObject rudderDisplayPivot;

    // factor for how much rudder turns boat
    public float rudderBoatTurnFactor = 0.3f;

    public UnityEvent<float, float> sendBoatInfo;

    void Update()
    {
      HandlePropulsionInput();
      HandleStirringInput();
      sendBoatInfo.Invoke(propulsion, rudderAngle);
    }

    void HandlePropulsionInput()
    {
      float verticalInput = Input.GetAxis("Vertical");
      float newPropulsion = propulsion + (verticalInput * accSpeed * Time.deltaTime);
      propulsion = Mathf.Clamp(newPropulsion, 0, maxSpeed);
    }
    
    void HandleStirringInput()
    {
      // inverse horizontal input s.t. right/left input leads to rigth/left rotation
      float horizontalInput = -Input.GetAxis("Horizontal");
      float newRudderAngle = rudderAngle + (horizontalInput * rudderTurnSpeed * Time.deltaTime);

      // Gradually returns the rudderAngle to zero if there is no input
      // FRAGE: soll sich Ruder auch nur während der Fahrt zurück setzen?
      if (horizontalInput == 0){
        float returnSign = Mathf.Sign(rudderAngle);
        newRudderAngle = rudderAngle - returnSign * rudderReturnSpeed * Time.deltaTime;
      }
      rudderAngle = Mathf.Clamp(newRudderAngle, -rudderMaxAngle, rudderMaxAngle);
      rudderAngle = Mathf.Round(rudderAngle * 100f) / 100f;
      // Visualize rudder
      rudderDisplayPivot.transform.localEulerAngles = Vector3.forward * rudderAngle;
    }
    
    void FixedUpdate(){

      // advance boat depending on propulsion
      var trajectory = GetCurrentTrajectory();
      var positionChange = trajectory * propulsion;
      transform.position += positionChange * Time.deltaTime;
      // turn boat depending on rudder and "movespeed" of boat 
      // könnte sein, dass magnitude hier laggs verursacht
      var rudderStirFactor = rudderAngle * rudderBoatTurnFactor * positionChange.magnitude;
      var rotationChange = Vector3.forward * rudderStirFactor;
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
