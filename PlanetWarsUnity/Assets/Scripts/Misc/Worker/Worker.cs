using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Worker
{
    public UnityAction Callback;
    public IEnumerator Coroutine;

    private void Reset()
    {
        Callback = null;
        Coroutine = null;
    }

    public void SetUpWork(UnityAction callback)
    {
        Reset();
        Callback = callback;
    }

    public void SetUpWork(IEnumerator coroutine)
    {
        Reset();
        Coroutine = coroutine;
    }

    public bool DoWork()
    {
        if (Callback != null)
            Callback.Invoke();
        if (Coroutine != null)
            WorkerFactory.Instance.StartCoroutine(Coroutine);

        return true;
    }
}