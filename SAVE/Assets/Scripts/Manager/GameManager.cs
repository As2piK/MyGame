using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {
    
    [Header("Debug")]
    public Material pathMaterial;

    public GameObject turretToBuild;

    public static GameManager instance;
    
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

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public GameObject getTurretToBuild()
    {
        return turretToBuild;
    }
}
