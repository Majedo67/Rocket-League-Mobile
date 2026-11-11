using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Optional helper for the NetworkManager Player Prefab.
/// Assign a camera root and local-only camera. Only the owning player enables it.
/// </summary>
public class NetworkedPlayerSetup : NetworkBehaviour
{
    [SerializeField] private GameObject localCamera;
    [SerializeField] private Behaviour[] localOnlyBehaviours;

    public override void OnNetworkSpawn()
    {
        bool local = IsOwner;
        if (localCamera != null) localCamera.SetActive(local);

        if (localOnlyBehaviours != null)
        {
            foreach (var behaviour in localOnlyBehaviours)
                if (behaviour != null) behaviour.enabled = local;
        }
    }
}
