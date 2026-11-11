using UnityEngine;
using UnityEngine.Events;

public class MatchManager : MonoBehaviour
{
    public int blueScore;
    public int orangeScore;
    public float matchLengthSeconds = 180f;
    public UnityEvent onMatchEnded;

    private float timeRemaining;
    private bool matchEnded;

    void Start()
    {
        timeRemaining = matchLengthSeconds;
    }

    void Update()
    {
        if (matchEnded) return;

        timeRemaining = Mathf.Max(0f, timeRemaining - Time.deltaTime);
        if (timeRemaining <= 0f)
        {
            matchEnded = true;
            onMatchEnded?.Invoke();
        }
    }

    public void AddGoal(int team)
    {
        if (matchEnded) return;

        if (team == 0) blueScore++;
        else if (team == 1) orangeScore++;
    }

    public float TimeRemaining => timeRemaining;
}
