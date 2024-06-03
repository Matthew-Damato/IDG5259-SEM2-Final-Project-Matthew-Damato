using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    void Awake()
    {
        Instance = this;
    }
    public GameObject standardTurret;

    void Start()
    {
        turretToBuild = standardTurret;
    }



    private GameObject turretToBuild;

    public GameObject getTurretToBuild() 
    {  
        return turretToBuild; 
    }
}
