using UnityEngine;

public class UIRoomSelectionController : MonoBehaviour
{
    [SerializeField] GameObject[] roomPrefabs;

    public void SetSelectedRoom(int roomIndex)
    {
        FindFirstObjectByType<GridPlacementManager>().buildingObj = roomPrefabs[roomIndex];
    }
}
