using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItemController: MonoBehaviour
{
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image backgroundImage;
    private ScoreResult _scoreResult;
    
    public void SetLeaderboardItem(ScoreResult scoreResult)
    {
        _scoreResult = scoreResult; 
        nicknameText.text = _scoreResult.nickname;
        scoreText.text = _scoreResult.score.ToString();
        HighlightIfCurrentUser();
    }

    public string GetUsername()
    {
        return _scoreResult.username;
    }

    private void HighlightIfCurrentUser()
    {
        string currentUsername = PlayerPrefs.GetString("currentUsername", string.Empty); // Get the current user's username

        if (_scoreResult.username == currentUsername)
        {
            // Change the background color to highlight the current user
            backgroundImage.color = Color.magenta;
        }
        else
        {
            // Set the background to a default color if not the current user
            backgroundImage.color = new Color(0.172549f, 0.2156863f, 0.3490196f, 1f);  // Or any default color
        }
    }
}
