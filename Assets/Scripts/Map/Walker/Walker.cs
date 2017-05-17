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
                if (target == buildManager.map.getEndPixel())
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

    public void updatePath(List<INode> path)
    {
        this.path = path;
        if (target == null)
        {
            target = BuildManager.instance.map.getSpawnPixel();
        }

        if (path.Contains(target))
        {
            //New path but target still exists, just remove the points before the current target
            path.RemoveRange(0, path.IndexOf(target));
        }
        else
        {
            //New path hasn't the current target --> Recalculate a specific path for this walker
            
            AStar astar = new AStar(target, BuildManager.instance.map.getEndPixel());
            astar.Run();
            if (astar.Run() == State.Failed)
            {
                //If failed : no existing path --> Destroy
                WalkerManager.instance.WalkerDie(this);
                Debug.Log("No path found");
                return;
            }
            this.path = astar.GetPath();
        }

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
