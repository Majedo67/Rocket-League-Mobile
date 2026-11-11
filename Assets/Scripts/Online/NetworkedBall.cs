using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Add NetworkObject, NetworkTransform and NetworkRigidbody to the ball prefab.
/// Server owns authoritative ball physics.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class NetworkedBall : NetworkBehaviour
{
    private Rigidbody body;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    public void ResetBallServer()
    {
        if (!IsServer) return;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.position = startingPosition;
        body.rotation = startingRotation;
    }
}
