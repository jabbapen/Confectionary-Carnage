using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ScoreModel
{
    public string name;
    public int score;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public static UnityEvent GameStart = new UnityEvent();
    [SerializeField] LevelSerializer levelSerializer;
    [SerializeField] LevelRequester levelRequester;
    [SerializeField] Transform levelParent;
    [SerializeField] PlayerController player;
    [SerializeField] NavMeshSurface navSurface;
    [SerializeField] bool useDebugList = false;
    [SerializeField] List<string> serializedLevels; 

    public LevelSerializer LevelSerializer { get { return levelSerializer; } }

    string scoreAPI = "https://lfrxfpetdl3dxlsfilviejg5kq0iruki.lambda-url.us-west-1.on.aws/leaderboard";
    int levelsBeat = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetupSystems());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SetupSystems()
    {
        yield return levelRequester.QueryAllLevels();
        SetupLevel();
        levelsBeat = 0;
    }

    public void EndGame()
    {
        StartCoroutine(TransitionOut());
    }

    IEnumerator TransitionOut()
    {
        Transition.Instance.HideScreen();
        yield return UploadScore();
        Transition.Instance.ShowScreen();
        SceneManager.LoadScene("GameOver");
    }

    public void SetupLevel(bool skipped = false)
    {
        UnloadLevel();
        if (!skipped)
            levelsBeat++;

        // Fetch level 
        LevelModel level = levelRequester.GetLevel();
        string levelString = level.serialized_level;

        if (useDebugList)
        {
            levelString = serializedLevels[Random.Range(0, serializedLevels.Count)];
        }
           
        LoadLevel(levelString); 
    }

    public void LoadLevel(string levelString)
    {
        // Call deserializer, passing in level parent and player transform
        levelSerializer.LoadField(levelString, levelParent.gameObject, player.transform);

        // Call nav surface to rebake the navmesh
        navSurface.BuildNavMesh();
        GameStart.Invoke();
    }

    public void UnloadLevel()
    {
        foreach (Transform child in levelParent)
        {
            // Destroy each child GameObject
            Destroy(child.gameObject);
        }
    }
    IEnumerator UploadScore()
    {
        ScoreModel scoreData = new ScoreModel
        {
            name = PlayerPrefs.GetString("Username", "Unity User"), 
            score = levelsBeat
        };
        string json = JsonUtility.ToJson(scoreData);

        // Create the POST request
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(scoreAPI, json))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Post data: " + www.downloadHandler.text);
            }
        }
    }

}
