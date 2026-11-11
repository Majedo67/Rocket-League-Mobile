using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Optional legacy-UI HUD. Assign three Text labels and the scene match state.
/// </summary>
public class NetworkedMatchHUD : MonoBehaviour
{
    [SerializeField] private NetworkedMatchState matchState;
    [SerializeField] private Text blueScoreText;
    [SerializeField] private Text orangeScoreText;
    [SerializeField] private Text timerText;

    private void Update()
    {
        if (matchState == null || !matchState.IsSpawned) return;

        if (blueScoreText != null) blueScoreText.text = matchState.BlueScore.Value.ToString();
        if (orangeScoreText != null) orangeScoreText.text = matchState.OrangeScore.Value.ToString();

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(matchState.SecondsRemaining.Value);
            timerText.text = (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
        }
    }
}
