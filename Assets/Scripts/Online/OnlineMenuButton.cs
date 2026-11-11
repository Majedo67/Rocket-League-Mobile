using UnityEngine;

/// <summary>
/// Connect the Online Play and Leave buttons to these methods in the Inspector.
/// </summary>
public class OnlineMenuButton : MonoBehaviour
{
    [SerializeField] private UnityNetcodeMatchmaking matchmaking;

    public void OnFindMatchPressed()
    {
        if (matchmaking == null)
        {
            Debug.LogError("Assign UnityNetcodeMatchmaking in the Inspector.");
            return;
        }

        matchmaking.FindRandomMatch();
    }

    public void OnLeaveMatchPressed()
    {
        if (matchmaking != null)
            matchmaking.LeaveMatch();
    }
}
