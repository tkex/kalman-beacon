using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public float speed = 0.02f;
    void Update()
    {
        MoveBoatAlongYAxis();
    }

    void MoveBoatAlongYAxis()
    {
        transform.position += Vector3.up * speed;
    }
}
