using UnityEngine;

public class BoatController : MonoBehaviour
{
    // Rb of the boat
    private Rigidbody rb;

    // Ruder reference
    public Transform ruder;

    // Max. degree of the ruder (oar) that is possible
    public float maxRuderAngle = 30f;

    // Speed of the boat
    public float speed = 10f;

    // Turn rotation speed of the boat
    public float turnSpeed = 50f;

    // Effect var how strong the ruder is applying to the rotation (100%)
    public float ruderEffectiveness = 1f;

    // Speed at which the ruder returns to center when no input is given
    public float ruderReturnSpeed = 1f;

    // Actual position of the ruder (oar) (-1 to 1)
    private float ruderPosition = 0f;

    // Target in Z-Axis -- set in inspector
    [Range(0, 1000)]
    public float zTarget = 100f;

    // Control modi
    public enum ControlMode
    {
        Manual, // Manual control (WASD)
        Straight // Straight line
    }

    public ControlMode controlMode = ControlMode.Manual;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (controlMode)
        {
            case ControlMode.Manual:
                ManualControl();
                break;
            case ControlMode.Straight:
                StraightLine();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (controlMode == ControlMode.Manual)
        {
            ManualPhysicsForManualModus();
        }
    }

    void ManualPhysicsForManualModus()
    {
        float moveVertical = Input.GetAxis("Vertical");
        rb.AddForce(transform.forward * moveVertical * speed);

        if (moveVertical > 0)
        {
            float speedFactor = rb.velocity.magnitude / speed;
            rb.AddTorque(0f, ruderPosition * turnSpeed * ruderEffectiveness * speedFactor * moveVertical, 0f);
        }
    }

    void ManualControl()
    {

        float ruderInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(ruderInput) > 0f)
        {
            ruderPosition += ruderInput * Time.deltaTime;
        }
        else
        {
            if (ruderPosition != 0)
            {
                float returnSign = Mathf.Sign(ruderPosition);
                ruderPosition -= returnSign * ruderReturnSpeed * Time.deltaTime;
                ruderPosition = Mathf.Clamp(ruderPosition, -1f, 1f);
            }
        }
        ruderPosition = Mathf.Clamp(ruderPosition, -1f, 1f);

        if (ruder != null)
        {
            ruder.localEulerAngles = new Vector3(ruder.localEulerAngles.x, ruderPosition * maxRuderAngle, ruder.localEulerAngles.z);
        }
    }

    void StraightLine()
    {
        if (transform.position.z != zTarget)
        {
            // Easy movement to position of Z-value
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, zTarget), step);
        }
    }
}
