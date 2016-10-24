using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.RTSActions;

public class TownCenter : MonoBehaviour,IRTSActionProvider
{
    public float WorkerBuildSpeed = 3;

    private int numQueuedWorkers;

	// Use this for initialization
	void Start () {
	
	}

    private float workerBuildProgress = 0;
	// Update is called once per frame
	void Update () {
	    if (numQueuedWorkers == 0)
	    {
	        workerBuildProgress = 0;
            return;
	    }

	    workerBuildProgress += Time.deltaTime*(1/WorkerBuildSpeed);
	    if (workerBuildProgress >= 1)
	    {
	        numQueuedWorkers--;
            SpawnWorker();
            workerBuildProgress -= 1;
	        if (workerBuildProgress > 4) workerBuildProgress = 0;
	    }
	    

	}

    private void SpawnWorker()
    {
        Debug.Log("Workerworkweork");
    }

    public IEnumerable<IRTSAction> GetActions()
    {
        return new[] {new CreateWorkerAction()};
    }

    public void QueueWorker()
    {
        numQueuedWorkers ++;
    }
}