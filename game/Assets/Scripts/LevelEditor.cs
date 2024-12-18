using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the editor scene, including selecting the tile to paint,
/// serialization, and making sure levels are of a valid format (eg. tiles must
/// not overlap). Implements Unity's `Start`, `Update`, and `FixedUpdate` to
/// interact with the user per-frame
/// </summary>
public class LevelEditor : MonoBehaviour
{
    [SerializeField] Transform tileCursor;
    [SerializeField] Transform tileMap;
    [SerializeField] LevelSerializer serializer;

    public Vector2 origin;
    public float tileSize;

    /// <summary>
    /// Maps positions in the scene to Tile IDs for serialization
    /// </summary>
    public Dictionary<Vector2, int> tileIDsMap = new Dictionary<Vector2, int>();
    /// <summary>
    /// Maps positions in the scene to GameObjects to control game state
    /// </summary>
    public Dictionary<Vector2, Tile> tileObjMap = new Dictionary<Vector2, Tile>();

    public static LevelEditor self;
    // Start is called before the first frame update
    void Awake()
    {
        self = GetComponent<LevelEditor>();        
    }

    private void Start()
    {
        RequestBrush(DEFAULT_BRUSH);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && startPlaced && endPlaced)
        {
            if (serializer == null)
            {
                SetSerializedString();
            }
            else
            {
                // TODO: Verification
                StartCoroutine(TransitionOut());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (brushType == TILE_START) { RequestBrush(TILE_END); }            
            else if (brushType == TILE_END) { RequestBrush(TILE_START); }
        }
                
        if (Input.GetKeyDown(KeyCode.Alpha1)) { RequestBrush(TILE_BRICK); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { RequestBrush(TILE_WATER); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { RequestBrush(TILE_LAVA); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { RequestBrush(TILE_END); }

        if (Input.GetKeyDown(KeyCode.Alpha0)) { RequestBrush(TILE_START); }
    }

    void FixedUpdate()
    {
        HandleDrawing();
    }

    IEnumerator TransitionOut()
    {
        Transition.Instance.ShowScreen();
        // GameManager.Instance.LevelSerializer.SaveField(tileMap.gameObject);
        serializer.SaveField(tileMap.gameObject);
        yield return new WaitForSeconds(0.5f);
        Transition.Instance.HideScreen();
        SceneManager.LoadScene("ChooseMode");

    }

    #region CONSTRAINT_MANAGER

    bool startPlaced = false, endPlaced = false;
    Vector2 start, end;

    void RemoveStart()
    {
        if (!startPlaced) { startPlaced = true; return; }        
        DeleteTile(start);
    }
    void RemoveEnd()
    {
        if (!endPlaced) { endPlaced = true; return; }        
        DeleteTile(end);
    }

    #endregion

    #region BRUSH_SELECTION

    public const int TILE_START = 0, TILE_END = 1, TILE_BRICK = 2, TILE_WATER = 3, TILE_LAVA = 4, TILE_ENEMY = 5;
    public int brushType;
    const int DEFAULT_BRUSH = 2;

    [SerializeField] GameObject cursorObj;
    [SerializeField] GameObject[] cursorObjs;

    public void RequestBrush(int id)
    {
        brushType = id;

        if (cursorObj) { Destroy(cursorObj); }
        cursorObj = Instantiate(cursorObjs[brushType], tileCursor.transform.position, Quaternion.identity);
        cursorObj.transform.parent = tileCursor.transform;
    }

    #endregion

    #region DRAWING

    [SerializeField] GameObject[] tiles;
    public static bool paletteHover;
    void HandleDrawing()
    {
        Vector2 tileCursorPos = new Vector2(Mathf.RoundToInt(CursorObj.trfm.position.x / tileSize) * tileSize, Mathf.RoundToInt(CursorObj.trfm.position.y / tileSize) * tileSize);
        tileCursor.position = tileCursorPos;

        if (Input.GetMouseButton(0) && !paletteHover)
        {            
            if (brushType == TILE_START)
            {
                RemoveStart();
                PlaceTile(tileCursorPos);
                start = tileCursorPos;                
            } else if (brushType == TILE_END)
            {
                RemoveEnd();
                PlaceTile(tileCursorPos);
                end = tileCursorPos;
            } else
            {
                PlaceTile(tileCursorPos);
            }
        }
        else if (Input.GetMouseButton(1))
        {
            DeleteTile(tileCursorPos);
        }
    }

    void PlaceTile(Vector2 tileCursorPos)
    {
        if (!tileIDsMap.ContainsKey(tileCursorPos))
        {
            tileObjMap.Add(tileCursorPos, Instantiate(tiles[brushType], tileCursorPos, Quaternion.identity).GetComponent<Tile>());
            tileObjMap[tileCursorPos].trfm.parent = tileMap;
            tileIDsMap.Add(tileCursorPos, brushType);
        }
        else if (tileIDsMap[tileCursorPos] != brushType)
        {
            Destroy(tileObjMap[tileCursorPos].gameObject);
            tileObjMap[tileCursorPos] = Instantiate(tiles[brushType], tileCursorPos, Quaternion.identity).GetComponent<Tile>();
            tileIDsMap[tileCursorPos] = brushType;
        }
        
    }

    void DeleteTile(Vector2 tileCursorPos)
    {
        if (tileIDsMap.ContainsKey(tileCursorPos))
        {
            tileIDsMap.Remove(tileCursorPos);
            Destroy(tileObjMap[tileCursorPos].gameObject);
            tileObjMap.Remove(tileCursorPos);
        }        
    }

    Vector2 calculateCell()
    {
        return new Vector2(Mathf.RoundToInt((CursorObj.trfm.position.x - origin.x)/tileSize), Mathf.RoundToInt(CursorObj.trfm.position.y));
    }

    #endregion

    #region SERIALIZATION

    public string mapSerialization = "";
    void SetSerializedString()
    {
        List<Vector2> tileList = new List<Vector2>(tileIDsMap.Keys);
        Debug.Log(tileList);

        mapSerialization = "";
        //mapSerialization += origin.ToString() + '\n';

        foreach (Vector2 tileCoord in tileList)
        {
            mapSerialization += tileCoord.ToString() + ": "+ tileIDsMap[tileCoord] + '\n';
        }
    }

    #endregion
}
