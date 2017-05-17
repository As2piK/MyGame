using UnityEngine;
using System.Collections.Generic;
using System;

public class BuildManager : MonoBehaviour {
    
    [Header("Debug")]
    public Material pathMaterial;

    private TurretBlueprint turretToBuild;

    public static BuildManager instance;
    
    public Map map;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("GameManager is being instanciate too many times !");
            return;
        }
        instance = this;
    }

    public TurretBlueprint getTurretToBuild()
    {
        return turretToBuild;
    }

    public void setTurretToBuild(TurretBlueprint blueprint)
    {
        turretToBuild = blueprint;
    }
}
