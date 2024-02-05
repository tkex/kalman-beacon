using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Floater : MonoBehaviour
{
    // Rb of the boat or floating point (empty go)
    public Rigidbody rb;

    //  Depth before submersion at which the boat starts to experience buoyancy
    public float depthBefSub;

    // Amount of water displacement caused by the boat
    public float displacementAmt;

    // Number of points on the boat contributing to buoyancy -- used to distribute force
    public int floaters;

    // Drag coefficient in water -- affecting the boats resistance to movement in water.
    public float waterDrag;

    // Angular drag coefficient in water -- affecting the boats resistance to rotational movement in water
    public float waterAngularDrag;

    // Water params
    public WaterSurface water;
    WaterSearchParameters Search;
    WaterSearchResult SearchResult;


    private void FixedUpdate()
    {

        rb.AddForceAtPosition(Physics.gravity / floaters, transform.position, ForceMode.Acceleration);

        Search.startPosition = transform.position;

        water.FindWaterSurfaceHeight(Search, out SearchResult);

        if (transform.position.y < SearchResult.height)
        {

            float displacementMulti = Mathf.Clamp01(SearchResult.height - transform.position.y / depthBefSub) * displacementAmt;

            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMulti, 0f), transform.position, ForceMode.Acceleration);
            rb.AddForce(displacementMulti * -rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rb.AddTorque(displacementMulti * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

}