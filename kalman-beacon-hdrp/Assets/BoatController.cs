using UnityEngine;

public class BoatController : MonoBehaviour
{
    // Speed of the boat
    public float speed = 10f;

    // Turn rotation speed of the boat
    public float turnSpeed = 50f;

    // Effect var how strong the ruder is applying to the rotation (100%)
    public float ruderEffectiveness = 1f;

    // Ruder reference
    public Transform ruder;

    // Max. degree of the ruder (oar) that is possible
    public float maxRuderAngle = 30f;

    // Speed at which the ruder returns to center when no input is given
    public float ruderReturnSpeed = 1f;

    private Rigidbody rb;

    // Actual position of the ruder (oar) (-1 to 1)
    private float ruderPosition = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Updates ruder position based on horizontal axis inputs (A and D buttons)
        float ruderInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(ruderInput) > 0f)
        {
            // Adjusts the ruder based on input
            ruderPosition += ruderInput * Time.deltaTime;
        }
        else
        {
            // Gradually returns the ruder to center if there is no input
            if (ruderPosition != 0)
            {
                float returnSign = Mathf.Sign(ruderPosition);
                ruderPosition -= returnSign * ruderReturnSpeed * Time.deltaTime;
                ruderPosition = Mathf.Clamp(ruderPosition, -1f, 1f); // Ensures ruderPosition stays within bounds
            }
        }
        ruderPosition = Mathf.Clamp(ruderPosition, -1f, 1f);

        // Rotate the ruder object if it's not null
        if (ruder != null)
        {
            ruder.localEulerAngles = new Vector3(ruder.localEulerAngles.x, ruderPosition * maxRuderAngle, ruder.localEulerAngles.z);
        }
    }

    private void FixedUpdate()
    {
        // Forward and backward movement (W and S buttons)
        float moveVertical = Input.GetAxis("Vertical");
        rb.AddForce(transform.forward * moveVertical * speed);

        // Only apply ruder turning effect if there is forward movement
        if (moveVertical > 0)
        {
            // Adjusting the turning force based on the speed of the boat
            float speedFactor = rb.velocity.magnitude / speed;
            rb.AddTorque(0f, ruderPosition * turnSpeed * ruderEffectiveness * speedFactor * moveVertical, 0f);
        }
    }
}
