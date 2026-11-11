using UnityEngine;
using UnityEngine.Events;

public class GoalTrigger : MonoBehaviour
{
    [Tooltip("Use 0 for the blue team's score and 1 for the orange team's score.")]
    public int scoringTeam;
    public UnityEvent<int> onGoalScored;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
            onGoalScored?.Invoke(scoringTeam);
    }
}
