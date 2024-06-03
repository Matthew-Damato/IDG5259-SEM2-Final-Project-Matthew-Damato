using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnScript : MonoBehaviour
{
    public EndGoal GM;

    public GameObject objectToInstantiate;
    public float spawnInterval = 3f;
    private float waitTime = 5;
    private bool IncreaseA = false;
    private bool IncreaseB = false;
    private bool IncreaseC = false;


    // Start is called before the first frame update
    void Start()
    {
        GM = FindObjectOfType<EndGoal>();
        StartCoroutine(SpawnObjectCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.timer > 200 && IncreaseA == false)
        {
            spawnInterval = 2f;
            IncreaseA = true;
        }

        if (GM.timer >= 500 && IncreaseB == false)
        {
            spawnInterval = 1.5f;
            IncreaseB = true;
        }

        if (GM.timer >= 1000 && IncreaseC == false)
        {
            spawnInterval = 1.25f;
            IncreaseC = true;
        }


    }

    private IEnumerator SpawnObjectCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        while (true)
        {
            // Instantiate the object
            Instantiate(objectToInstantiate, transform.position, Quaternion.identity);

            // Wait for the specified interval before spawning the next object
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
