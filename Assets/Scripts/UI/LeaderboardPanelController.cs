using System.Collections.Generic;
using Common;
using UnityEngine;


public class LeaderboardPanelController : MonoBehaviour
{
    [SerializeField] private GameObject leaderBoardItemPrefab;
    [SerializeField] private RectTransform content;
    
    private List<LeaderboardItemController> leaderboardItems = new List<LeaderboardItemController>();
    private void Start()
    {
        GetRanking();
    }
    
    public void GetRanking()
    {
        StartCoroutine(NetworkManager.Instance.GetRanking(
                (userInfo) =>
                {
                    var ranking = userInfo.ranking;
                    for (int i = 0; i < ranking.Length; i++)
                    {
                        AddRankingItem(ranking[i]);
                    }
                    var currentUserIndex = FindCurrentUserIndex();
                    Debug.Log(currentUserIndex);
                    ScrollTo(currentUserIndex);
                },
                () =>
                {
                    // 로그인 화면 띄우기
                    Debug.Log("GetRanking fail");
                }));
    }

    public void AddRankingItem(ScoreResult scoreResult)
    {
        var leaderboardItem = Instantiate(leaderBoardItemPrefab, content);
        var leaderboardItemController = leaderboardItem.GetComponent<LeaderboardItemController>();
        leaderboardItemController.SetLeaderboardItem(scoreResult);
        leaderboardItems.Add(leaderboardItemController);
        content.sizeDelta += leaderboardItem.GetComponent<RectTransform>().sizeDelta;
    }

    public int FindCurrentUserIndex()
    {
        var username = PlayerPrefs.GetString("currentUsername");

        // Use LINQ to find the index of the leaderboard item with the current username
        var index = leaderboardItems.FindIndex(item => item.GetUsername() == username);

        return index >= 0 ? index : -1; // Return the index if found, otherwise return -1 indicating not found
    }


    public void ScrollTo(int index)
    {
        float cellHeight = 150f;
        content.anchoredPosition = new Vector2(0,  index * cellHeight);
    }

    public void OnClickCloseButton()
    {
        Destroy(gameObject);
    }
}
