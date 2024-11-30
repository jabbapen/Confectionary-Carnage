using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public static UnityEvent GameStart = new UnityEvent();
    [SerializeField] LevelSerializer levelSerializer;
    [SerializeField] Transform levelParent;
    [SerializeField] PlayerController player;
    [SerializeField] NavMeshSurface navSurface;

    [SerializeField] List<string> serializedLevels; 

    public LevelSerializer LevelSerializer { get { return levelSerializer; } }

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupLevel()
    {
        UnloadLevel();

        // Fetch serialized level string
        string levelString = serializedLevels[Random.Range(0, serializedLevels.Count)];

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

}
