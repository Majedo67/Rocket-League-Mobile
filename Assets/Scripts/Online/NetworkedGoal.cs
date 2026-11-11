using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Put on each goal trigger. Assign the scene NetworkedMatchState and ball.
/// scoringTeam 0 = blue score, 1 = orange score.
/// </summary>
public class NetworkedGoal : MonoBehaviour
{
    [SerializeField] private int scoringTeam;
    [SerializeField] private NetworkedMatchState matchState;
    [SerializeField] private NetworkedBall ball;
    private bool goalLocked;

    private void OnTriggerEnter(Collider other)
    {
        if (goalLocked || NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            return;

        if (!other.CompareTag("Ball")) return;

        goalLocked = true;
        if (matchState != null) matchState.AddGoalServer(scoringTeam);
        if (ball != null) ball.ResetBallServer();
        Invoke(nameof(UnlockGoal), 1.25f);
    }

    private void UnlockGoal()
    {
        goalLocked = false;
    }
}
