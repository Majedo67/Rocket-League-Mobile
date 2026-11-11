using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Place on a scene GameObject with NetworkObject. Server updates score and clock;
/// NetworkVariables replicate the values to every player.
/// </summary>
public class NetworkedMatchState : NetworkBehaviour
{
    public NetworkVariable<int> BlueScore = new NetworkVariable<int>(0);
    public NetworkVariable<int> OrangeScore = new NetworkVariable<int>(0);
    public NetworkVariable<float> SecondsRemaining = new NetworkVariable<float>(180f);
    public NetworkVariable<bool> MatchFinished = new NetworkVariable<bool>(false);

    [SerializeField] private float matchLengthSeconds = 180f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            BlueScore.Value = 0;
            OrangeScore.Value = 0;
            SecondsRemaining.Value = matchLengthSeconds;
            MatchFinished.Value = false;
        }
    }

    private void Update()
    {
        if (!IsServer || MatchFinished.Value) return;

        SecondsRemaining.Value = Mathf.Max(0f, SecondsRemaining.Value - Time.deltaTime);
        if (SecondsRemaining.Value <= 0f)
            MatchFinished.Value = true;
    }

    public void AddGoalServer(int team)
    {
        if (!IsServer || MatchFinished.Value) return;

        if (team == 0) BlueScore.Value++;
        else if (team == 1) OrangeScore.Value++;
    }
}
