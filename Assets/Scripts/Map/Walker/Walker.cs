using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour {

    BuildManager buildManager;

    public bool isEnemy;
    public float defaultSpeed;
    public float health;
    public float worth;

    private List<INode> path;
    private Pixel target;

    private float speed;
    
    private void Start()
    {
        buildManager = BuildManager.instance;
        path = buildManager.map.getPath();
    }

    private void Update()
    {
        if (path != null)
        {
            Vector3 dir = target.transform.position + target.positionOffset - transform.position;
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

            //Debug.Log("Distance : " + Vector3.Distance(transform.position, target.transform.position));

            if (Vector3.Distance(transform.position, target.transform.position) <= target.positionOffset.magnitude + (speed * Time.deltaTime))
            {
                transform.position = transform.position;
                if (path.Count == 0)
                {
                    EndPath();
                    return;
                }
                else
                {
                    GetNextWaypoint();
                }
            }

            speed = defaultSpeed;
        }
    }

    public void updatePath(List<INode> p)
    {
        
        this.path = p;
        if (target == null)
        {
            target = BuildManager.instance.map.getSpawnPixel();
        }

        if (path.Contains(target))
        {
            Debug.Log("yay");
            //New path but target still exists, just remove the points before the current target
            path.RemoveRange(0, path.IndexOf(target));
        }
        
        //New path hasn't the current target --> Recalculate a specific path for this walker
        AStar astar = AStar.getInstance(target, BuildManager.instance.map.getEndPixel());
        Debug.Log("Nouveau chemin depuis " + target.x + ":" + target.y);
        path = astar.GetPath();

        //Debug.Log(" ");
        //Debug.Log(" ");
        //Debug.Log(BuildManager.instance.map.getEndPixel());
        //Debug.Log(" ");
        //Debug.Log(" ");
        //foreach (INode n in path)
        //{
        //    Debug.Log((Pixel)n);
        //}

        GetNextWaypoint();
    }

    private void GetNextWaypoint()
    {
        target = (Pixel)path[0];
        path.RemoveAt(0);
    }

    private void EndPath()
    {
        WalkerManager.instance.WalkerDie(this);
    }
}
