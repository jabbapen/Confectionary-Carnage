using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] Transform tileCursor;
    [SerializeField] Transform tileMap;

    public Vector2 origin;
    public float tileSize;
    public Dictionary<Vector2, int> tileIDsMap = new Dictionary<Vector2, int>();
    public Dictionary<Vector2, Tile> tileObjMap = new Dictionary<Vector2, Tile>();

    public static LevelEditor self;
    // Start is called before the first frame update
    void Awake()
    {
        self = GetComponent<LevelEditor>();
        RequestBrush(DEFAULT_BRUSH);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (GameManager.Instance == null)
            {
                SetSerializedString();
            }
            else
            {
                GameManager.Instance.LevelSerializer.SaveField(tileMap.gameObject);
            }
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

    #region CONSTRAINT_MANAGER

    bool startPlaced = false, endPlaced = false;

    #endregion

    #region BRUSH_SELECTION
        
    const int TILE_START = 0, TILE_END = 1, TILE_BRICK = 2, TILE_WATER = 3, TILE_LAVA = 4;
    public int brushType;
    const int DEFAULT_BRUSH = 2;

    [SerializeField] GameObject cursorBrushObj;

    void RequestBrush(int id)
    {
        if (id == TILE_START)
        {
            if (startPlaced)
            {
                if (endPlaced) { return; }
                else { brushType = TILE_END; }
            }
            else
            {
                if (endPlaced) { brushType = TILE_START; }
                else if (brushType == TILE_START) { brushType = TILE_END; }                
                else { brushType = TILE_START; }
            }
        }
        else
        {
            brushType = id;
        }

        if (cursorBrushObj)
        {
            Destroy(cursorBrushObj);
            cursorBrushObj = Instantiate(tiles[brushType], tileCursor.position, Quaternion.identity);
            cursorBrushObj.transform.parent = tileCursor;
        }
    }

    #endregion

    #region DRAWING

    [SerializeField] GameObject[] tiles;
    void HandleDrawing()
    {
        Vector2 tileCursorPos = new Vector2(Mathf.RoundToInt(CursorObj.trfm.position.x / tileSize) * tileSize, Mathf.RoundToInt(CursorObj.trfm.position.y / tileSize) * tileSize);
        tileCursor.position = tileCursorPos;

        if (Input.GetMouseButton(0))
        {
            PlaceTile(tileCursorPos);
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
