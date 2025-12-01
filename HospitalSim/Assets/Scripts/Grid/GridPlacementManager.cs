using UnityEngine;

public class GridPlacementManager : MonoBehaviour
{
    Grid grid;
    [SerializeField] GameObject buildingObj;
    [SerializeField] GameObject buildingPreviewObj;
    [SerializeField] GameObject baseObj;

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
        if(Input.GetMouseButtonDown(0))
            grid.PlaceBuilding(mouseWorldPos, buildingObj, 2);

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
    }
}
