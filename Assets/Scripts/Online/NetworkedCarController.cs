using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Put on the car prefab with NetworkObject, NetworkTransform and Rigidbody.
/// The server simulates the car; each owning client sends its mobile input.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class NetworkedCarController : NetworkBehaviour
{
    [Header("Driving")]
    [SerializeField] private float acceleration = 24f;
    [SerializeField] private float reverseAcceleration = 12f;
    [SerializeField] private float turnSpeed = 95f;
    [SerializeField] private float maxSpeed = 22f;
    [SerializeField] private float grip = 5f;

    [Header("Boost")]
    [SerializeField] private float boostForce = 38f;
    [SerializeField] private float boostCapacity = 3f;
    [SerializeField] private float boostRecharge = 0.35f;

    private Rigidbody body;
    private float throttleInput;
    private float steeringInput;
    private bool boostInput;
    private float boostFuel;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = new Vector3(0f, -0.45f, 0f);
        boostFuel = boostCapacity;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        float throttle = 0f;
        float steering = 0f;
        bool boost = false;

        // Keyboard fallback for editor testing; mobile buttons can call the public setters.
#if UNITY_EDITOR || UNITY_STANDALONE
        throttle = Input.GetAxisRaw("Vertical");
        steering = Input.GetAxisRaw("Horizontal");
        boost = Input.GetKey(KeyCode.Space);
#endif
        SubmitInputServerRpc(throttle, steering, boost);
    }

    public void SetMobileThrottle(float value)
    {
        if (IsOwner) SubmitInputServerRpc(Mathf.Clamp(value, -1f, 1f), steeringInput, boostInput);
    }

    public void SetMobileSteering(float value)
    {
        if (IsOwner) SubmitInputServerRpc(throttleInput, Mathf.Clamp(value, -1f, 1f), boostInput);
    }

    public void SetMobileBoost(bool value)
    {
        if (IsOwner) SubmitInputServerRpc(throttleInput, steeringInput, value);
    }

    [ServerRpc]
    private void SubmitInputServerRpc(float throttle, float steering, bool boost)
    {
        throttleInput = Mathf.Clamp(throttle, -1f, 1f);
        steeringInput = Mathf.Clamp(steering, -1f, 1f);
        boostInput = boost;
    }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        float forwardSpeed = Vector3.Dot(body.linearVelocity, transform.forward);
        if (throttleInput > 0f && forwardSpeed < maxSpeed)
            body.AddForce(transform.forward * throttleInput * acceleration, ForceMode.Acceleration);
        else if (throttleInput < 0f)
            body.AddForce(transform.forward * throttleInput * reverseAcceleration, ForceMode.Acceleration);

        float speedFactor = Mathf.Clamp01(body.linearVelocity.magnitude / 3f);
        float direction = forwardSpeed >= -0.5f ? 1f : -1f;
        float turn = steeringInput * turnSpeed * speedFactor * direction * Time.fixedDeltaTime;
        body.MoveRotation(body.rotation * Quaternion.Euler(0f, turn, 0f));

        Vector3 sideways = Vector3.Project(body.linearVelocity, transform.right);
        body.AddForce(-sideways * grip, ForceMode.Acceleration);

        if (boostInput && boostFuel > 0f)
        {
            body.AddForce(transform.forward * boostForce, ForceMode.Acceleration);
            boostFuel = Mathf.Max(0f, boostFuel - Time.fixedDeltaTime);
        }
        else
        {
            boostFuel = Mathf.Min(boostCapacity, boostFuel + boostRecharge * Time.fixedDeltaTime);
        }
    }
}
