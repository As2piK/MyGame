using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour {

    public TurretBlueprint debugTurret;

    BuildManager buildManager;

    private void Start()
    {
        buildManager = BuildManager.instance;
    }

    public void selectDebugTurret()
    {
        buildManager.setTurretToBuild(debugTurret);
    }

}
