using TMPro;
using UnityEngine;

public class GridPlacementManager : MonoBehaviour
{
    Grid grid;
    public GameObject buildingObj;
    [SerializeField] GameObject buildingPreviewObj;
    [SerializeField] GameObject baseObj;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] GameObject[] roomPrefabs;

    private bool isInBuildingState = false;
    private void Start()
    {
        grid = new Grid(10, 10, 10f, baseObj, true, true, buildingPreviewObj);
        buildingPreviewObj.SetActive(isInBuildingState);
    }

    private void Update()
    {
        if(!isInBuildingState) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(Input.GetMouseButtonDown(1))
            grid.PlaceBuilding(mouseWorldPos, buildingObj, 1);

        if (scroll > 0f)
        {
            buildingPreviewObj.transform.Rotate(0, 90, 0);
        } else if (scroll < 0f)
        {
            buildingPreviewObj.transform.Rotate(0, -90, 0);
        }

        grid.RePositionPreview(mouseWorldPos);
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); 
        float enter;
        if (groundPlane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }

    public void SwichBuildingSate()
    {
        isInBuildingState = !isInBuildingState;
        buildingPreviewObj.SetActive(isInBuildingState);
        roomNameText.text = buildingObj.name;
    }

    public void SetSelectedRoom(int roomIndex)
    {
        buildingObj = roomPrefabs[roomIndex];
        roomNameText.text = buildingObj.name;
    }
}
