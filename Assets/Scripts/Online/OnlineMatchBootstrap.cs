using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Starts or joins a public Photon Fusion session.
/// Add this to a GameObject in the gameplay scene and assign a NetworkRunner prefab.
/// Requires Photon Fusion 2 and a configured Photon App ID.
/// </summary>
public class OnlineMatchBootstrap : MonoBehaviour
{
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private int maxPlayers = 6;

    private NetworkRunner runner;

    public async void FindMatch()
    {
        if (runner != null)
            return;

        if (runnerPrefab == null)
        {
            Debug.LogError("Assign a NetworkRunner prefab before starting online play.");
            return;
        }

        runner = Instantiate(runnerPrefab);
        runner.name = "OnlineNetworkRunner";
        runner.ProvideInput = true;

        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = null, // Fusion matchmaking selects/creates a session.
            PlayerCount = Mathf.Clamp(maxPlayers, 2, 16),
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        });

        if (!result.Ok)
        {
            Debug.LogError("Online match failed: " + result.ShutdownReason);
            Destroy(runner.gameObject);
            runner = null;
            return;
        }

        Debug.Log("Connected to an online match.");
    }

    public async void LeaveMatch()
    {
        if (runner == null)
            return;

        var oldRunner = runner;
        runner = null;
        await oldRunner.Shutdown();
        Destroy(oldRunner.gameObject);
    }

    private async void OnDestroy()
    {
        if (runner != null)
        {
            var oldRunner = runner;
            runner = null;
            await oldRunner.Shutdown();
        }
    }
}
