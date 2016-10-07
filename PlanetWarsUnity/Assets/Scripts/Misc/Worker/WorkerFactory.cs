using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorkerFactory : MonoBehaviour
{
    private static WorkerFactory instance;

    public static WorkerFactory Instance
    {
        get
        {
            return instance;
        }
    }

    public Queue<Worker> FreeWorkers;

    public List<Worker> RunningWorkers;

    public int InitWorkers = 10;

    private static Worker worker;
    public float workPerFrame = 1;

    [ReadOnly]
    public int CurrentFreeWorkerCount = 0;

    [ReadOnly]
    public float CurrentWork = 0;

    public float MinWorkPerFrame = 0.01f;
    public float MaxWorkPerFrame = 10;

    private float deltaTime = 0.0f;
    public float fps;

    private void Awake()
    {
        instance = this;

        FreeWorkers = new Queue<Worker>();
        for (int i = 0; i < InitWorkers; i++)
        {
            worker = new Worker();
            FinishWorker(worker);
        }
        worker = null;
        RunningWorkers = new List<Worker>();
    }

    private Worker GetFreeWorker()
    {
        if (FreeWorkers.Count > 0)
        {
            CurrentFreeWorkerCount = FreeWorkers.Count - 1;
            return FreeWorkers.Dequeue();
        }
        return new Worker();
    }

    public void AddWorker(Worker worker)
    {
        if (RunningWorkers.Contains(worker))
            return;
        RunningWorkers.Add(worker);
    }

    public void FinishWorker(Worker worker)
    {
        if (RunningWorkers.Contains(worker))
            RunningWorkers.Remove(worker);
        FreeWorkers.Enqueue(worker);
        CurrentFreeWorkerCount = FreeWorkers.Count;
    }

    public static void CreateWork(UnityAction callback)
    {
        worker = Instance.GetFreeWorker();
        worker.SetUpWork(callback);
        Instance.AddWorker(worker);
        worker = null;
    }

    public static void CreateWork(IEnumerator callback)
    {
        worker = Instance.GetFreeWorker();
        worker.SetUpWork(callback);
        Instance.AddWorker(worker);
        worker = null;
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;

        if (fps < 60)
            workPerFrame -= 0.01f;
        else if (fps > 60)
            workPerFrame += 0.01f;
        workPerFrame = Mathf.Clamp(workPerFrame, MinWorkPerFrame, MaxWorkPerFrame);

        CurrentWork += workPerFrame;
        while (CurrentWork > 1f)
        {
            DoWork();
            CurrentWork -= 1f;
        }
    }

    private void DoWork()
    {
        if (RunningWorkers.Count == 0)
            return;
        if (RunningWorkers[0].DoWork())
        {
            FinishWorker(RunningWorkers[0]);
        }
    }
}