using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public TowerGeneration towerGenStats;

    public float interval = 1f; // Interval in seconds
    public float timer = 0f; // Timer to track the interval


    public int gold = 10;

    public List<GameObject> path;
    public GameObject towerPrefab; 
    public float maxDistance = 5f; 
    public string nodeTag = "Node"; 
    public int conditionMet = 0;
    public string focusVal;
    public bool towerJustSpawned = false;
    // Start is called before the first frame update



    void Start()
    {
        startConditions();
    }
    public void startConditions()
    {
        conditionMet = 0;
        maxDistance = towerGenStats.RangeReturn();
        towerGenStats.GoldPrice();
        if (conditionMet == 0)
        {
            towerGenStats.GoldPriceFirst();
        }
        path = new List<GameObject>(GameObject.FindGameObjectsWithTag("Paths"));
    }

    bool ContainsValidGameObjects(List<GameObject> list)
    {
        if (list == null || list.Count == 0)
        {
            return false;
        }
        foreach (GameObject obj in list)
        {
            if (obj != null)
            {
                return true; // Found at least one valid GameObject
            }
        }
        return false; // No valid GameObjects found
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log(path.Count);
        if (path.Count == 0 || !ContainsValidGameObjects(path))
        {
            startConditions();
        }

        if (((towerGenStats.LoopNo+1) % 2) == 0)
        {
            focusVal = "damage";
            if (conditionMet == 1)
            {
                focusVal = "rate";
            }
        }
        else
        {
            focusVal = "rate";
            if (conditionMet == 1)
            {
                focusVal = "damage";
            }
        }

        if (conditionMet == 2)
        {
            focusVal = "range";
        }


        timer += Time.deltaTime;
        if (timer >= interval)
        {
            // Add gold
            gold += 1;

            // Reset the timer
            timer = 0f;

        }

        if (towerJustSpawned == true)
        {
            towerGenStats.GoldPrice();
            if (conditionMet == 0)
            {
                towerGenStats.GoldPriceFirst();
            }
            towerJustSpawned = false;
        }

        if (gold >= towerGenStats.GoldBuy && conditionMet < 3)
        {
            Debug.Log("Activated");
            towerGenStats.TowerGenerate(focusVal);
            maxDistance = towerGenStats.RangeReturn();
            SpawnTowerOnClosestNode();
            conditionMet++;
            towerJustSpawned = true;
            gold = gold - towerGenStats.GoldBuy;
        }

    }


    void SpawnTowerOnClosestNode()
    {
        // Find all nodes in the scene
        GameObject[] nodes = GameObject.FindGameObjectsWithTag(nodeTag);
        List<GameObject> validNodes = new List<GameObject>();

        // Check each node to see if it's within the maxDistance of any path GameObject
        foreach (GameObject node in nodes)
        {
            foreach (GameObject pathPoint in path)
            {
                if (Vector3.Distance(node.transform.position, pathPoint.transform.position) <= maxDistance)
                {
                    validNodes.Add(node);
                    break; // No need to check other path points for this node
                }
            }
        }

        // If there are valid nodes, spawn a tower on one of them
        if (validNodes.Count > 0)
        {
            GameObject selectedNode = validNodes[Random.Range(0, validNodes.Count)];
            Instantiate(towerPrefab, selectedNode.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No valid nodes found within the specified distance from the path.");
        }
    }


}
