using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

/// <summary>
/// Basic public matchmaking prototype using Unity Lobby + Relay + Netcode for GameObjects.
/// Requires the matching Unity packages and a NetworkManager with UnityTransport in the scene.
/// </summary>
public class UnityNetcodeMatchmaking : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 6;
    [SerializeField] private string lobbyName = "Rocket League Mobile";
    private Lobby currentLobby;
    private Coroutine heartbeat;

    public async void FindRandomMatch()
    {
        try
        {
            await InitializeServices();

            var query = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions
            {
                Count = 10,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            });

            foreach (var lobby in query.Results)
            {
                if (lobby.Data != null && lobby.Data.ContainsKey("RelayJoinCode"))
                {
                    try
                    {
                        currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
                        string code = currentLobby.Data["RelayJoinCode"].Value;
                        await JoinRelayMatch(code);
                        Debug.Log("Joined public match: " + currentLobby.Name);
                        return;
                    }
                    catch (LobbyServiceException)
                    {
                        // Lobby filled or expired while we were joining; try another.
                    }
                }
            }

            await CreatePublicMatch();
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Matchmaking failed: " + exception.Message);
        }
    }

    private async Task InitializeServices()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        if (NetworkManager.Singleton == null)
            throw new System.Exception("Add a NetworkManager with UnityTransport to the gameplay scene.");
    }

    private async Task CreatePublicMatch()
    {
        int capacity = Mathf.Clamp(maxPlayers, 2, 16);
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(capacity - 1);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null)
            throw new System.Exception("Add UnityTransport to the NetworkManager.");

        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.ConnectionData);

        currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, capacity,
            new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            });

        if (!NetworkManager.Singleton.StartHost())
            throw new System.Exception("Netcode could not start the host.");

        heartbeat = StartCoroutine(SendLobbyHeartbeat());
        Debug.Log("Public match created. Lobby ID: " + currentLobby.Id);
    }

    private async Task JoinRelayMatch(string joinCode)
    {
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null)
            throw new System.Exception("Add UnityTransport to the NetworkManager.");

        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData);

        if (!NetworkManager.Singleton.StartClient())
            throw new System.Exception("Netcode could not start the client.");
    }

    private IEnumerator SendLobbyHeartbeat()
    {
        while (currentLobby != null)
        {
            yield return new WaitForSeconds(15f);
            if (currentLobby != null)
                _ = LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        }
    }

    public async void LeaveMatch()
    {
        if (heartbeat != null)
        {
            StopCoroutine(heartbeat);
            heartbeat = null;
        }

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.Shutdown();

        if (currentLobby != null)
        {
            try
            {
                if (currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                    await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                else
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException exception)
            {
                Debug.LogWarning("Lobby leave notice: " + exception.Message);
            }

            currentLobby = null;
        }
    }

    private void OnDestroy()
    {
        if (heartbeat != null)
            StopCoroutine(heartbeat);
    }
}
