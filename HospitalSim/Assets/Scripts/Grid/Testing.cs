using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    Grid grid;
    [SerializeField] GameObject buildingObj;
    [SerializeField] GameObject buildingPreviewObj;
    [SerializeField] GameObject baseObj;
    private void Start()
    {
        grid = new Grid(10, 10, 10f, baseObj, true, true, buildingPreviewObj);
        
    }

    private void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        if(Input.GetMouseButtonDown(0))
        {
            grid.PlaceBuilding(mouseWorldPos, buildingObj, 2);
        }

        grid.RePositionPreview(mouseWorldPos);
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // y=0 plane
        float enter;
        if (groundPlane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }
}
