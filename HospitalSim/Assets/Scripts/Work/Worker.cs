using UnityEngine;
using UnityEngine.AI;

public enum WorkerType
{
    Doctor,
    Nurse,
}
public class Worker : MonoBehaviour
{
    public bool isUnemployed = true;
    public WorkerType workerType;
    [SerializeField] NavMeshAgent agent;
    public WorkableRoom workRoom;

    public void GoToWork(Transform workPos, WorkableRoom workRoom)
    {
        agent.SetDestination(workPos.position);
        isUnemployed = false;
        this.workRoom = workRoom;
        agent.isStopped = false;
    }

    private void Update()
    {
        if(isUnemployed) return;
        if(workRoom.hasWorker == true) return;

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            workRoom.WorkerArrived();
            agent.isStopped = true;
        }
    }
}
