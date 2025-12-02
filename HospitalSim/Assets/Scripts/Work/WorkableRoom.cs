using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
    Reception,
    DoctorOffice,
}
public class WorkableRoom : MonoBehaviour
{
    public WorkerType workerType;
    public bool hasWorker = false;
    public bool hasPatient = false;
    [SerializeField] float serviceTime;
    public List<Patient> patientsQueue;
    public Transform workerPos;
    public Transform patientPos;

    public RoomType roomType;

    private void Start()
    {
        FindWorker();
    }

    public void FindWorker() {
        Worker[] workers = FindObjectsByType<Worker>(FindObjectsSortMode.None);
        if(workers.Length == 0)
        {
            Debug.Log("there is no worker here");
            return;
        }

        foreach (Worker worker in workers)
        {
            if(worker.isUnemployed && worker.workerType == workerType)
            {
                worker.GoToWork(workerPos, this);
                break;
            }
        }
    }

    public void CallNextPatient()
    {
        if(patientsQueue.Count > 0)
        {
            patientsQueue[0].ComeToServiceRoom(patientPos, this);
        }
    }

    public void WorkerArrived()
    {
        // now patients can come here
        hasWorker = true;
        Invoke(nameof(CallNextPatient), 0.8f);
    }

    public void EndOfTheService()
    {
        Patient patient = patientsQueue[0];
        patient.EndOfService();
        patientsQueue.Remove(patient);
        hasPatient = false;
    }

    public void PatientArrived()
    {
        hasPatient = true;
        Invoke(nameof(EndOfTheService), serviceTime);
        print("patient arrived");
    }

    public void AddPatientToQueue(Patient patient)
    {
        patientsQueue.Add(patient);
    }
}
