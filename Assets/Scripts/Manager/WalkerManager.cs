using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerManager : MonoBehaviour {

    BuildManager buildManager;

    public static WalkerManager instance;

    public List<Walker> walkers;

    public GameObject pathInspectorPrefab;

    public float pathInspectorWaitTime = 3f;
    public float pathInspectorCooldown = 0f;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("WalkerManager is being instanciate too many times !");
            return;
        }
        walkers = new List<Walker>();
        instance = this;
    }

    private void Start()
    {
        buildManager = BuildManager.instance;
    }

    private void Update()
    {
        //Spawn Path Inspector
        if (pathInspectorCooldown <= 0f)
        {
            SpawnPathInspector();
            pathInspectorCooldown = pathInspectorWaitTime;
        }
        pathInspectorCooldown -= Time.deltaTime;
    }

    public void addWalker(Walker w)
    {
        walkers.Add(w);
    }

    public void updatePath(List<INode> path)
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            walkers[i].updatePath(path);
        }
    }

    void SpawnPathInspector()
    {
        GameObject w = Instantiate(
            pathInspectorPrefab,
            buildManager.map.getSpawnPixel().transform.position + buildManager.map.getSpawnPixel().positionOffset,
            buildManager.map.getSpawnPixel().transform.rotation);
        w.transform.GetComponent<Walker>().updatePath(buildManager.map.getPath());
        addWalker(w.transform.GetComponent<Walker>());
    }

    public void WalkerDie(Walker w) {
        walkers.Remove(w);
        if (w != null)
        {
            Destroy(w.gameObject);
        }
    }

}
