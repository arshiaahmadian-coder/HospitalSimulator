using UnityEngine;

/// <summary>
/// Grid placement system for Unity (works at runtime). Attach to an empty GameObject in the scene.
/// Features:
/// - configurable cell size and plane (XZ or XY)
/// - preview "ghost" object that follows mouse and snaps to grid
/// - place prefab with Left Click, remove with Right Click
/// - rotate preview with Q/E (or use your own keys)
/// - stores grid coordinates in placed objects via GridPlacedObject component
/// 
/// Note: put your placeable prefab into the "prefab" slot in inspector. The prefab should have a collider
/// so deletion raycasts can hit it (or use a custom tag/layer). If you want editor-only placement, adapt the code
/// to run in editor scripts instead.
/// </summary>
public class GridPlacementSystem : MonoBehaviour
{
    public enum Plane { XZ, XY }

    [Header("Grid")]
    public Plane gridPlane = Plane.XZ;
    public Vector2 cellSize = new Vector2(1, 1);
    public Vector3 gridOrigin = Vector3.zero; // origin of grid

    [Header("Placement")]
    public GameObject prefab; // prefab to place
    public LayerMask placementLayerMask = ~0; // where raycast hits (default: everything)
    public Transform previewInstance; // optional: assign a prefab for preview (will be instantiated if null)

    [Header("Controls")]
    public KeyCode rotateCW = KeyCode.E;
    public KeyCode rotateCCW = KeyCode.Q;
    public int rotationStepDegrees = 90;

    Camera cam;
    GameObject currentPreview;
    Quaternion previewRotation = Quaternion.identity;

    void Start()
    {
        cam = Camera.main;
        if (cam == null) Debug.LogWarning("GridPlacementSystem: no Camera tagged MainCamera found. Mouse raycasts may not work.");

        if (previewInstance == null && prefab != null)
        {
            // create a copy of prefab to use as preview
            currentPreview = Instantiate(prefab);
            MakePreviewTransparent(currentPreview);
        }
        else if (previewInstance != null)
        {
            currentPreview = Instantiate(previewInstance.gameObject);
            MakePreviewTransparent(currentPreview);
        }

        if (currentPreview != null) currentPreview.SetActive(false);
    }

    void Update()
    {
        if (prefab == null) return; // nothing to place

        HandleRotationInput();

        Vector3? hitPoint = GetMouseWorldPoint();
        if (!hitPoint.HasValue)
        {
            if (currentPreview) currentPreview.SetActive(false);
            return;
        }

        Vector3 snapped = SnapToGrid(hitPoint.Value);

        if (currentPreview)
        {
            currentPreview.SetActive(true);
            currentPreview.transform.position = snapped;
            currentPreview.transform.rotation = previewRotation;
        }

        if (Input.GetMouseButtonDown(0)) // left click => place
        {
            PlaceAt(snapped, previewRotation);
        }

        if (Input.GetMouseButtonDown(1)) // right click => remove
        {
            TryRemoveAt(hitPoint.Value);
        }
    }

    void HandleRotationInput()
    {
        if (Input.GetKeyDown(rotateCW))
        {
            previewRotation = previewRotation * Quaternion.Euler(0, rotationStepDegrees, 0);
        }
        if (Input.GetKeyDown(rotateCCW))
        {
            previewRotation = previewRotation * Quaternion.Euler(0, -rotationStepDegrees, 0);
        }
    }

    Vector3 SnapToGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - gridOrigin;
        if (gridPlane == Plane.XZ)
        {
            float x = Mathf.Round(local.x / cellSize.x) * cellSize.x;
            float z = Mathf.Round(local.z / cellSize.y) * cellSize.y;
            return gridOrigin + new Vector3(x, worldPos.y - gridOrigin.y, z);
        }
        else // XY
        {
            float x = Mathf.Round(local.x / cellSize.x) * cellSize.x;
            float y = Mathf.Round(local.y / cellSize.y) * cellSize.y;
            return gridOrigin + new Vector3(x, y, worldPos.z - gridOrigin.z);
        }
    }

    Vector3Int WorldToGridCoordinates(Vector3 worldPos)
    {
        Vector3 local = worldPos - gridOrigin;
        if (gridPlane == Plane.XZ)
        {
            int gx = Mathf.RoundToInt(local.x / cellSize.x);
            int gz = Mathf.RoundToInt(local.z / cellSize.y);
            return new Vector3Int(gx, 0, gz);
        }
        else
        {
            int gx = Mathf.RoundToInt(local.x / cellSize.x);
            int gy = Mathf.RoundToInt(local.y / cellSize.y);
            return new Vector3Int(gx, gy, 0);
        }
    }

    void PlaceAt(Vector3 position, Quaternion rotation)
    {
        GameObject go = Instantiate(prefab, position, rotation);
        GridPlacedObject placed = go.GetComponent<GridPlacedObject>();
        if (placed == null) placed = go.AddComponent<GridPlacedObject>();
        placed.gridCoordinates = WorldToGridCoordinates(position);
    }

    void TryRemoveAt(Vector3 worldPoint)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayerMask))
        {
            GridPlacedObject placed = hit.collider.GetComponentInParent<GridPlacedObject>();
            if (placed != null)
            {
                Destroy(placed.gameObject);
            }
            else
            {
                // optional: try by tag
                // if (hit.collider.CompareTag("Placeable")) Destroy(hit.collider.gameObject);
            }
        }
    }

    Vector3? GetMouseWorldPoint()
    {
        if (cam == null) return null;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // For simplicity we'll raycast against placementLayerMask; in many scenes you might want
        // to raycast against a ground plane instead.
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayerMask))
        {
            return hit.point;
        }

        // If no collider hit, do not allow placement
        return null;
    }

    void MakePreviewTransparent(GameObject preview)
    {
        // Walk all renderers and set material to a transparent variant so preview looks ghost-like
        Renderer[] rends = preview.GetComponentsInChildren<Renderer>();
        foreach (var r in rends)
        {
            // Create a simple transparent material instance so we don't modify the original prefab's material
            Material mat = new Material(Shader.Find("Standard"));
            if (r.sharedMaterial != null && r.sharedMaterial.mainTexture != null)
                mat.mainTexture = r.sharedMaterial.mainTexture;
            mat.SetFloat("_Mode", 3); // Transparent mode
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            Color c = r.sharedMaterial != null ? r.sharedMaterial.color : Color.white;
            c.a = 0.45f;
            mat.color = c;
            r.material = mat;
        }
    }
}

/// <summary>
/// Small component stored on placed objects so we can identify/remove them and record their grid coords.
/// </summary>
public class GridPlacedObject : MonoBehaviour
{
    public Vector3Int gridCoordinates;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 world = transform.position;
        Gizmos.DrawWireCube(world + Vector3.up * 0.01f, new Vector3(0.9f, 0.01f, 0.9f));

        #if UNITY_EDITOR
        UnityEditor.Handles.Label(world + Vector3.up * 0.1f, gridCoordinates.ToString());
        #endif
    }
}
