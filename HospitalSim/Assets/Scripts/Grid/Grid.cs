using UnityEngine;

public class Grid {
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    private GameObject baseObj;
    private GameObject previewObj;
    private bool showPreview;

    public Grid(
        int width,
        int height,
        float cellSize,
        GameObject baseObj,
        bool showDebuggingLines,
        bool showPreview,
        GameObject previewObj
    ) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.baseObj = baseObj;
        this.showPreview = showPreview;
        this.previewObj = previewObj;

        if (showDebuggingLines) ShowDebugingLines();
    }

    private void ShowDebugingLines()
    {
        gridArray = new int[width, height];
        
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                Object.Instantiate(baseObj, GetWorldPosition(x, z), Quaternion.identity);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    private Vector3 GetWorldPosition(int x, int z)
    {
        // return new Vector3(x, 0, z) * cellSize;
        return new Vector3((x + 0.5f) * cellSize, 0, (z + 0.5f) * cellSize);
    }

    private void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.RoundToInt(worldPosition.x / cellSize - 0.5f);
        z = Mathf.RoundToInt(worldPosition.z / cellSize - 0.5f);
    }

    public void SetValue(int x, int z, int value)
    {
        gridArray[x, z] = value;
        Debug.Log(gridArray[x, z]);
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetValue(x, z, value);
    }

    public int GetValue(int x, int z)
    {
        return gridArray[x, z];
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return gridArray[x, z];
    }

    public void PlaceBuilding(Vector3 worldPosition, GameObject buildingObj, int buildingCode = 1)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);

        // you cant place in here. building already placed
        if(GetValue(x, z) != 0) return;
        
        SetValue(x, z, buildingCode);
        Object.Instantiate(buildingObj, GetWorldPosition(x, z), previewObj.transform.rotation);
    }

    public void RePositionPreview(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        
        previewObj.transform.position = GetWorldPosition(x, z);
    }
}
