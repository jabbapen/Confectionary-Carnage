using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class ScoreResponse
{
    public int statusCode;
    public string message;
    public List<ScoreModel> data;
}

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI leaderboardText; // TextMeshPro text field to display the leaderboard.

    string scoreAPI = "https://lfrxfpetdl3dxlsfilviejg5kq0iruki.lambda-url.us-west-1.on.aws/leaderboard";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetLeaderboardData(10));
    }

    IEnumerator GetLeaderboardData(int amt)
    {
        string query = $"{scoreAPI}?limit={amt}";
        using (UnityWebRequest www = UnityWebRequest.Get(query))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                string fieldData = www.downloadHandler.text;
                Debug.Log("Get Data: " + fieldData);
                ScoreResponse data = JsonUtility.FromJson<ScoreResponse>(fieldData);
                UpdateLeaderboardDisplay(data.data);
            }
        }
    }

    private void UpdateLeaderboardDisplay(List<ScoreModel> scores)
    {
        leaderboardText.text = "Leaderboard\n\n";

        int rank = 1;
        foreach (ScoreModel score in scores)
        {
            leaderboardText.text += $"{rank}. {score.name}: {score.score}\n";
            rank++;
        }
    }
}
