using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {

    BuildManager buildManager;

    public TurretBlueprint debugTurret;

    private void Start()
    {
        buildManager = BuildManager.instance;
    }

    public void addRandomTurret()
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < 1500; i++)
        {
            int rx = rand.Next(buildManager.map.mapSizeX);
            int ry = rand.Next(buildManager.map.mapSizeY);
            if (buildManager.map.map[rx, ry].canBuild)
            {
                buildManager.map.map[rx, ry].buildTurret(debugTurret);
            }
        }
    }
}
