using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantGameObject : MonoBehaviour
{
    public GameManager GM;
    private static PersistantGameObject instance;
    void Awake()
    {
        // Find all objects with the same tag as this GameObject
        GameObject[] objectsWithSameTag = GameObject.FindGameObjectsWithTag(this.tag);

        // If there is more than one, destroy the new instance
        if (objectsWithSameTag.Length > 1)
        {
            Debug.Log("Reloaded");
            GM.startConditions();
            Destroy(gameObject);
        }
        else
        {
            // If this is the only instance, make it persistent
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
