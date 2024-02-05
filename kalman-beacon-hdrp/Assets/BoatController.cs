using UnityEngine;

public class BoatController : MonoBehaviour
{

    // Speed of the boat
    public float speed = 10f;

    // Turn rotation speed of the boat
    public float turnSpeed = 50f; // Drehgeschwindigkeit des Bootes

    // Effect var how strong the ruder is applying to the rotation (100%)
    public float ruderEffectiveness = 1f;

    // Ruder reference
    public Transform ruder; // Referenz auf das Ruderobjekt

    // Max. degree of the ruder (oar) that is possible
    public float maxRuderAngle = 30f;

    private Rigidbody rb;

    // Actual position of the ruder (oar) (-1 to 1)
    private float ruderPosition = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Updates rudder position based on horizontal axis inputs (A and D buttons)
        float rudderInput = Input.GetAxis("Horizontal");
        ruderPosition = Mathf.Clamp(ruderPosition + rudderInput * Time.deltaTime, -1f, 1f);

        // Turn the ruder if not null
        if (ruder != null)
        {
            ruder.localEulerAngles = new Vector3(ruder.localEulerAngles.x, ruderPosition * maxRuderAngle, ruder.localEulerAngles.z);
        }
    }

    private void FixedUpdate()
    {
        // W and S buttons
        float moveVertical = Input.GetAxis("Vertical");
        rb.AddForce(transform.forward * moveVertical * speed);

        // Only turn ruder if boat is moving
        if (moveVertical != 0)
        {
            // Adjusting the turning force based on the speed of the boat
            // Speed factor (normaliized to max speed possible of the boat)
            float speedFactor = rb.velocity.magnitude / speed;
            rb.AddTorque(0f, ruderPosition * turnSpeed * ruderEffectiveness * speedFactor * moveVertical, 0f);
        }

        /*         
        // Rotation ONLY based on the ruder position
        if (moveVertical != 0)
        {
            rb.AddTorque(0f, rudderPosition * turnSpeed * rudderEffectiveness * moveVertical, 0f);
        }
        */

        // Debug.Log("Ruderposition: " + rudderPosition);
        Debug.Log("Ruderwinkel: " + ruder.localEulerAngles);
    }
}
