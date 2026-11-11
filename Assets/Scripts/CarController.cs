using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Driving")]
    public float acceleration = 24f;
    public float reverseAcceleration = 12f;
    public float turnSpeed = 95f;
    public float maxSpeed = 22f;
    public float grip = 5f;

    [Header("Boost")]
    public float boostForce = 38f;
    public float boostFuel = 3f;
    public float boostRecharge = 0.35f;

    [Header("Mobile input")]
    [Range(-1f, 1f)] public float steering;
    [Range(-1f, 1f)] public float throttle;
    public bool boostPressed;

    private Rigidbody body;
    private float fuel;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = new Vector3(0f, -0.45f, 0f);
        fuel = boostFuel;
    }

    void FixedUpdate()
    {
        float forwardSpeed = Vector3.Dot(body.linearVelocity, transform.forward);

        if (throttle > 0f && forwardSpeed < maxSpeed)
            body.AddForce(transform.forward * throttle * acceleration, ForceMode.Acceleration);
        else if (throttle < 0f)
            body.AddForce(transform.forward * throttle * reverseAcceleration, ForceMode.Acceleration);

        float speedFactor = Mathf.Clamp01(body.linearVelocity.magnitude / 3f);
        float direction = forwardSpeed >= -0.5f ? 1f : -1f;
        float turn = steering * turnSpeed * speedFactor * direction * Time.fixedDeltaTime;
        body.MoveRotation(body.rotation * Quaternion.Euler(0f, turn, 0f));

        Vector3 sideways = Vector3.Project(body.linearVelocity, transform.right);
        body.AddForce(-sideways * grip, ForceMode.Acceleration);

        if (boostPressed && fuel > 0f)
        {
            body.AddForce(transform.forward * boostForce, ForceMode.Acceleration);
            fuel = Mathf.Max(0f, fuel - Time.fixedDeltaTime);
        }
        else
        {
            fuel = Mathf.Min(boostFuel, fuel + boostRecharge * Time.fixedDeltaTime);
        }
    }

    public float BoostAmount => boostFuel <= 0f ? 0f : fuel / boostFuel;
}
