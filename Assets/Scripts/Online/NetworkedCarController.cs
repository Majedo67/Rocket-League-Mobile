using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Put on the car Player Prefab with NetworkObject, NetworkTransform,
/// NetworkRigidbody and Rigidbody. The server simulates movement.
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
    private float localThrottle;
    private float localSteering;
    private bool localBoost;
    private float serverThrottle;
    private float serverSteering;
    private bool serverBoost;
    private float boostFuel;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = new Vector3(0f, -0.45f, 0f);
        boostFuel = boostCapacity;
    }

    private void Update()
    {
        if (!IsOwner) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        localThrottle = Input.GetAxisRaw("Vertical");
        localSteering = Input.GetAxisRaw("Horizontal");
        localBoost = Input.GetKey(KeyCode.Space);
#endif
        SubmitInputServerRpc(localThrottle, localSteering, localBoost);
    }

    public void SetMobileThrottle(float value)
    {
        if (!IsOwner) return;
        localThrottle = Mathf.Clamp(value, -1f, 1f);
        SubmitInputServerRpc(localThrottle, localSteering, localBoost);
    }

    public void SetMobileSteering(float value)
    {
        if (!IsOwner) return;
        localSteering = Mathf.Clamp(value, -1f, 1f);
        SubmitInputServerRpc(localThrottle, localSteering, localBoost);
    }

    public void SetMobileBoost(bool value)
    {
        if (!IsOwner) return;
        localBoost = value;
        SubmitInputServerRpc(localThrottle, localSteering, localBoost);
    }

    [ServerRpc]
    private void SubmitInputServerRpc(float throttle, float steering, bool boost)
    {
        serverThrottle = Mathf.Clamp(throttle, -1f, 1f);
        serverSteering = Mathf.Clamp(steering, -1f, 1f);
        serverBoost = boost;
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        float forwardSpeed = Vector3.Dot(body.linearVelocity, transform.forward);
        if (serverThrottle > 0f && forwardSpeed < maxSpeed)
            body.AddForce(transform.forward * serverThrottle * acceleration, ForceMode.Acceleration);
        else if (serverThrottle < 0f)
            body.AddForce(transform.forward * serverThrottle * reverseAcceleration, ForceMode.Acceleration);

        float speedFactor = Mathf.Clamp01(body.linearVelocity.magnitude / 3f);
        float direction = forwardSpeed >= -0.5f ? 1f : -1f;
        float turn = serverSteering * turnSpeed * speedFactor * direction * Time.fixedDeltaTime;
        body.MoveRotation(body.rotation * Quaternion.Euler(0f, turn, 0f));

        Vector3 sideways = Vector3.Project(body.linearVelocity, transform.right);
        body.AddForce(-sideways * grip, ForceMode.Acceleration);

        if (serverBoost && boostFuel > 0f)
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
