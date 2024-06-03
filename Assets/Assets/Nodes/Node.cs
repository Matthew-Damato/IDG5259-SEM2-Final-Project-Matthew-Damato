using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Color hoverColor;


    private GameObject turret;

    private Renderer rend;
    private Color startColor;
    void OnMouseEnter()
    {
        rend.material.color = hoverColor;
    }
    void OnMouseExit()
    {
        rend.material.color = startColor;
    }

    void OnMouseDown()
    {
        if (turret != null)
        {
            Debug.Log("Can't build here");
            return;
        }
        else
        {
            //build the turret
            GameObject turretToBuild = BuildManager.Instance.getTurretToBuild();
            turret = (GameObject)Instantiate(turretToBuild, transform.position, transform.rotation);
        }


    }
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
