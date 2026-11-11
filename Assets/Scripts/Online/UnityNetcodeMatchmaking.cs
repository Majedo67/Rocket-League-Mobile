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
/// Public matchmaking using Unity Lobby + Relay + Netcode for GameObjects.
/// Requires a NetworkManager with UnityTransport in the scene.
/// </summary>
public class UnityNetcodeMatchmaking : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 6;
    [SerializeField] private string lobbyName = "Rocket League Mobile";
    private Lobby currentLobby;
    private Coroutine heartbeat;
    private bool starting;

    public async void FindRandomMatch()
    {
        if (starting || (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening))
            return;

        starting = true;
        try
        {
            await InitializeServices();
            var query = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions
            {
                Count = 20,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            });

            foreach (var lobby in query.Results)
            {
                if (lobby.Data == null || !lobby.Data.ContainsKey("RelayJoinCode"))
                    continue;

                try
                {
                    currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
                    await JoinRelayMatch(currentLobby.Data["RelayJoinCode"].Value);
                    Debug.Log("Joined public match: " + currentLobby.Name);
                    return;
                }
                catch (System.Exception joinError)
                {
                    Debug.LogWarning("Could not join lobby " + lobby.Id + ": " + joinError.Message);
                    if (currentLobby != null)
                    {
                        try
                        {
                            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
                        }
                        catch { }
                        currentLobby = null;
                    }
                }
            }

            await CreatePublicMatch();
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Matchmaking failed: " + exception.Message);
        }
        finally
        {
            starting = false;
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

    private UnityTransport GetTransport()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null)
            throw new System.Exception("Add UnityTransport to the NetworkManager.");
        return transport;
    }

    private async Task CreatePublicMatch()
    {
        int capacity = Mathf.Clamp(maxPlayers, 2, 16);
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(capacity - 1);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        GetTransport().SetRelayServerData(
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
        {
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
            currentLobby = null;
            throw new System.Exception("Netcode could not start the host.");
        }

        heartbeat = StartCoroutine(SendLobbyHeartbeat());
        Debug.Log("Public match created. Lobby ID: " + currentLobby.Id);
    }

    private async Task JoinRelayMatch(string joinCode)
    {
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        GetTransport().SetRelayServerData(
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
