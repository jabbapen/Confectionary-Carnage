using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LevelSerializer : MonoBehaviour
{
    [SerializeField] string resourcePath;
    [SerializeField] GameObject debugSerialize;
    [SerializeField] string debugSerialUpload;
    [SerializeField] GameObject debugTarget;
    [SerializeField] int playerSpawnIndex = -1;

    private string levelsAPI = "https://lfrxfpetdl3dxlsfilviejg5kq0iruki.lambda-url.us-west-1.on.aws/levels"; // CHANGE THIS TO AWS DEPLOYMENT

    private Dictionary<int, int> itemIdToIndexMap = new Dictionary<int, int>();
    ObjectIndex gameObjectList;

    /// <summary>
    /// Loads the GameObjectList from the Resources folder and creates a mapping
    /// of the Item component's id to the GameObject's index in the list.
    /// </summary>
    void LoadGameObjectList()
    {
        // Load the GameObjectList from Resources
        gameObjectList = Resources.Load<ObjectIndex>(resourcePath);

        if (gameObjectList == null)
        {
            Debug.LogError("GameObjectList not found at the specified path: " + resourcePath);
            return;
        }

        // Clear the dictionary in case this function is called more than once
        itemIdToIndexMap.Clear();

        // Iterate over the GameObjects in the list
        for (int i = 0; i < gameObjectList.objects.Count; i++)
        {
            GameObject obj = gameObjectList.objects[i];

            // Make sure the GameObject has an Item component
            Item item = obj.GetComponent<Item>();
            if (item != null)
            {
                // Add the mapping of the Item id to the index in the list
                itemIdToIndexMap[item.id] = i;
            }
            else
            {
                Debug.LogWarning($"GameObject at index {i} does not have an Item component: {obj.name}");
            }
        }

        Debug.Log("ObjectIndex loaded and mapping created.");
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadGameObjectList();
        if (debugSerialize) SaveField(debugSerialize);
        if (playerSpawnIndex == -1)
        {
            Debug.LogError("WARNING: playerSpawnIndex is set to -1, meaning no objects registered in our game object list is considered a spawnpoint - Player will not spawn!");
        }

        if (debugSerialUpload.Length > 0) StartCoroutine(UploadLevel(debugSerialUpload));
    }

    int GetIndexFromItemId(int itemId)
    {
        if (itemIdToIndexMap.TryGetValue(itemId, out int index))
        {
            return index;
        }

        Debug.LogWarning("Item with id " + itemId + " not found.");
        return -1;
    }

    public void SaveField(GameObject parent)
    {
        StringBuilder sb = new StringBuilder();
        // Get all child objects with the Item class
        foreach (Item obj in parent.GetComponentsInChildren<Item>())
        {
            // Serialize each object as id#x#y#z#rx#ry#rz#sx#sy#sz%
            // All values are rounded to int
            Transform tr = obj.transform;
            sb.Append(obj.id);
            sb.Append('#');
            sb.Append(Mathf.RoundToInt(tr.localPosition.x));
            sb.Append('#');
            sb.Append(Mathf.RoundToInt(tr.localPosition.y));
            sb.Append('#');
            sb.Append(Mathf.RoundToInt(tr.localPosition.z));
            sb.Append('#');
            sb.Append(Mathf.RoundToInt(tr.localRotation.eulerAngles.x));
            sb.Append('#');
            sb.Append(Mathf.RoundToInt(tr.localRotation.eulerAngles.y));
            sb.Append('#');
            sb.Append(Mathf.RoundToInt(tr.localRotation.eulerAngles.z));
            sb.Append('#');
            sb.Append(Mathf.RoundToInt(tr.localScale.x));
            sb.Append('#');
            sb.Append(Mathf.RoundToInt(tr.localScale.y));
            sb.Append('#');
            sb.Append(Mathf.RoundToInt(tr.localScale.z));
            sb.Append('%');
        }

        // Debug.Log(sb.ToString());
        StartCoroutine(UploadLevel(sb.ToString())); // uncomment this when you want to test uploading a level
    }

    public void LoadField(string data, GameObject parent, Transform playerTransform)
    {
        // Split the input string into individual object data blocks (split by '%')
        string[] objectDataBlocks = data.Split(new[] { '%' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string objectData in objectDataBlocks)
        {
            // Split the object data into individual components (split by '#')
            string[] fields = objectData.Split('#');

            // Ensure we have exactly 10 fields (id, position (x, y, z), rotation (x, y, z), scale (x, y, z))
            if (fields.Length == 10)
            {
                // Parse the id
                int id = int.Parse(fields[0]);

                // Look up the corresponding GameObject index from the id using the itemIdToIndexMap
                if (itemIdToIndexMap.TryGetValue(id, out int index))
                {
                    Transform tr;

                    if (id == playerSpawnIndex)  // Don't spawn if we have a player spawnpoint, move the player instead
                    {
                        tr = playerTransform;
                        tr.parent = parent.transform;
                    } else
                    {
                        // Instantiate the GameObject from the GameObjectList at the index
                        GameObject prefab = gameObjectList.objects[index];
                        GameObject newObj = Instantiate(prefab, parent.transform);

                        // If we spawned a spawnpoint, replace with the enemy
                        EnemySpawnpoint spawnpoint = newObj.GetComponent<EnemySpawnpoint>();
                        if (spawnpoint)
                        {
                            GameObject enemyObj = spawnpoint.GetEnemy();
                            Destroy(spawnpoint.gameObject);
                            newObj = Instantiate(enemyObj, parent.transform);
                        }

                        // Retrieve its Transform component
                        tr = newObj.transform;
                    }

                    

                    // Set local position, rotation, and scale based on the parsed data
                    tr.localPosition = new Vector3(
                        int.Parse(fields[1]),  // Position X
                        int.Parse(fields[2]),  // Position Y
                        int.Parse(fields[3])   // Position Z
                    );

                    tr.localRotation = Quaternion.Euler(
                        int.Parse(fields[4]),  // Rotation X
                        int.Parse(fields[5]),  // Rotation Y
                        int.Parse(fields[6])   // Rotation Z
                    );

                    tr.localScale = new Vector3(
                        int.Parse(fields[7]),  // Scale X
                        int.Parse(fields[8]),  // Scale Y
                        int.Parse(fields[9])   // Scale Z
                    );

                    if (id == playerSpawnIndex)  // deparent player
                    {
                        tr.parent = null;
                    }
                }
                else
                {
                    Debug.LogWarning($"Item with id {id} not found in the itemIdToIndexMap.");
                }
            }
            else
            {
                Debug.LogWarning("Invalid data format for object: " + objectData);
            }
        }
    }

    // API CALLS
    IEnumerator UploadLevel(string serializedLevel)
    {
        LevelModel levelData = new LevelModel
        {
            level_name = "Test Level",
            author = "UnityUser",
            serialized_level = serializedLevel 
        };
        string json = JsonUtility.ToJson(levelData);

        // Create the POST request
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(levelsAPI, json))
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

    IEnumerator GetLevelData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(levelsAPI))
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
            }
        }
    }
}
