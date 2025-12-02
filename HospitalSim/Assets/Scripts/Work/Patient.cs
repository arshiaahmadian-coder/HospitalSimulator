using UnityEngine;
using UnityEngine.AI;

public class Patient : MonoBehaviour
{
    public RoomType[] RequiredServices;
    public bool searchingForRoom = true;
    public int currentServiceIndex = 0;
    [SerializeField] NavMeshAgent agent;
    public bool isGoingToRoom = false;
    private WorkableRoom serviceRoom;

    private void FixedUpdate()
    {
        print(isGoingToRoom);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
            agent.velocity.sqrMagnitude < 0.01f &&
            isGoingToRoom) {
            agent.isStopped = true;
            isGoingToRoom = false;
            serviceRoom.PatientArrived();
        }

        if(!searchingForRoom) return;
        WorkableRoom[] workableRooms = FindObjectsByType<WorkableRoom>(FindObjectsSortMode.None);
        if(workableRooms.Length == 0) return;

        foreach(WorkableRoom room in workableRooms)
        {
            if(room.hasWorker == true && room.roomType == RequiredServices[currentServiceIndex])
            {
                room.AddPatientToQueue(this);
                searchingForRoom = false;
                break;
            }
        }
    }

    public void ComeToServiceRoom(Transform roomPos, WorkableRoom serviceRoom)
    {
        agent.SetDestination(roomPos.position);
        agent.isStopped = false;
        this.serviceRoom = serviceRoom;
        isGoingToRoom = true;
        print("ComeToServiceRoom");
    }

    public void EndOfService()
    {
        searchingForRoom = true;
        currentServiceIndex++;
        Debug.Log("patient is ok now");
        Destroy(gameObject);
    }
}
