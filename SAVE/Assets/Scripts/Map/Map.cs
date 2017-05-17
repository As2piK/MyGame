using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    [Header("Map")]
    public Pixel[,] map;
    public Pixel pixel;
    public int mapSizeX;
    public int mapSizeY;

    [Header("Spawns")]
    public Transform spawn;
    public Transform end;
    public int spawnX;
    public int spawnY;
    public int endX;
    public int endY;
    

    private AStar astar;
    private List<INode> path;

    private void Start()
    {
        map = new Pixel[mapSizeX, mapSizeX];

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                Vector3 pos = new Vector3(i, 0, j);

                Pixel p = Instantiate(pixel, pos, transform.rotation);
                p.x = i;
                p.y = j;
                map[i, j] = p;
            }
        }

        //Generate spawn and end
        Vector3 spawnPos = map[spawnX, spawnY].transform.position;
        map[spawnX, spawnY].canBuild = false;
        spawnPos.y += spawn.transform.localScale.y / 2f + pixel.transform.localScale.y / 2f;
        Instantiate(spawn, spawnPos, transform.rotation);
        Vector3 endPos = map[endX, endY].transform.position;
        map[endX, endY].canBuild = false;
        endPos.y += end.transform.localScale.y / 2f + pixel.transform.localScale.y / 2f;
        Instantiate(end, endPos, transform.rotation);
        
    }

    //TODO Remove
    private void addRandomTurret()
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < 1500; i++)
        {
            int rx = rand.Next(mapSizeX);
            int ry = rand.Next(mapSizeY);
            if (map[rx, ry].canBuild)
            {
                if (map[rx, ry].turret == null)
                {
                    map[rx, ry].turret = Instantiate(GameManager.instance.getTurretToBuild(), map[rx, ry].getBuildPosition(), transform.rotation);
                }
                map[map[rx, ry].x, map[rx, ry].y].turret = GameManager.instance.turretToBuild;
            }
        }
    }

    public bool generatePath()
    {
        //Reset Open and Closed list from pixels
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                map[i, j].SetOpenList(false);
                map[i, j].SetClosedList(false);
            }
        }

        //Run A*
        astar = new AStar(map[spawnX, spawnY], map[endX, endY]);
        if (astar.Run() == State.Failed)
        {
            //If failed : no path
            return false;
        }

        //Clear old path
        if (path != null)
        {
            foreach (INode item in path)
            {
                ((Pixel)item).GetComponent<Renderer>().material.color = ((Pixel)item).defaultColor;
                ((Pixel)item).currentColor = ((Pixel)item).defaultColor;
            }
        }

        //Get path
        path = astar.GetPath();
        //Debug.Log("Path length : " + path.Count);

        //Color new path
        foreach (INode item in path)
        {
            ((Pixel)item).GetComponent<Renderer>().material.color = Color.red;
            ((Pixel)item).currentColor = Color.red;
        }

        return true;
    }

    public bool addTurretTo(GameObject t, Pixel p)
    {
        GameObject oldTurret = map[p.x, p.y].turret;
        map[p.x, p.y].turret = t;
        if (generatePath())
        {
            return true;
        }

        map[p.x, p.y].turret = oldTurret;
        return false;
    }
}
