using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class LevelModel
{
    public string level_name;
    public string author;
    public string serialized_level;
}

[System.Serializable]
public class LevelResponse
{
    public int statusCode;
    public string message;
    public List<LevelModel> data;
}

public class LevelRequester : MonoBehaviour
{
    Queue<LevelModel> levelQueue = new Queue<LevelModel>();
    [SerializeField] int preloadAmount = 3;
    List<LevelModel> serializedLevels = new List<LevelModel>();  // previously loadeed levels, used in case we can't load fast enough

    private string levelsAPI = "https://jtxj7s3d3tz2ii2dv7ric3xqbi0ljjls.lambda-url.us-west-1.on.aws/levels"; // CHANGE THIS TO AWS DEPLOYMENT
    bool fetchingLevel = false;

    private void Start()
    {
        // StartCoroutine(GetLevelData(2));
    }

    public IEnumerator QueryAllLevels()
    {
        // populate queue
        yield return GetLevelData(preloadAmount);

        StartCoroutine(CheckQuery());
        yield return null;
    }

    IEnumerator CheckQuery()
    {
        // check once per frame if the queue is emptying
        while (true)
        {
            yield return new WaitForEndOfFrame();
            // if yes, query another
            if (levelQueue.Count < preloadAmount && !fetchingLevel)
            {
                StartCoroutine(GetLevelData(1));
            }
        }

    }

    IEnumerator GetLevelData(int amt)
    {
        fetchingLevel = true;
        string query = $"{levelsAPI}?limit={amt}";
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
                LevelResponse data = JsonUtility.FromJson<LevelResponse>(fieldData);
                foreach (var s in data.data)
                {
                    Debug.Log(s.level_name);
                    levelQueue.Enqueue(s);
                }
            }
        }
        fetchingLevel = false;
    }

    public LevelModel GetLevel()
    {
        if (levelQueue.Count > 0)
        {
            LevelModel cur = levelQueue.Dequeue();
            serializedLevels.Add(cur);
            return cur;
        } else
        {
            return serializedLevels.Count > 0 ? serializedLevels[Random.Range(0, serializedLevels.Count)] : new LevelModel();
        }
        
    }
}
