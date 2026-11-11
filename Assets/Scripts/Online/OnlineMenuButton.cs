using UnityEngine;

/// <summary>
/// Connect this to the Online Play and Leave Match buttons in the Unity UI.
/// </summary>
public class OnlineMenuButton : MonoBehaviour
{
    [SerializeField] private OnlineMatchBootstrap onlineMatch;

    public void OnFindMatchPressed()
    {
        if (onlineMatch == null)
        {
            Debug.LogError("Assign OnlineMatchBootstrap in the Inspector.");
            return;
        }

        onlineMatch.FindMatch();
    }

    public void OnLeaveMatchPressed()
    {
        if (onlineMatch != null)
            onlineMatch.LeaveMatch();
    }
}
