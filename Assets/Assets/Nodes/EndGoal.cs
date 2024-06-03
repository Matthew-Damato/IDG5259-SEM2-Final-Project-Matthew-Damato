using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGoal : MonoBehaviour
{
    public float life = 10;
    public TowerGeneration towerGenStats;
    public TowerCreationScript towerCreation;
    public Turret turretspawned;

    public GameManager GM;
    

    public string tagToFind = "tower";

    public float timer = 0;
    public bool isMonitering = false;
    private float Kills = 0;
    public float timeRemaining = 1200;
    private bool Activated = false;

    public float FirstContact = 0;
    public float LastContact = 0;
    private bool isFirstContact = false;

    // Start is called before the first frame update
    void Start()
    {
        towerGenStats = FindObjectOfType<TowerGeneration>();
        towerCreation = FindObjectOfType<TowerCreationScript>();
        if (towerCreation != null)
        {
            towerCreation.endFn = false;
            towerCreation.Gold = towerCreation.StartingGold;
            towerCreation.Timer = 0;
            towerCreation.TowerSpawned = false;
            towerCreation.TowerSpawned2 = false;
        }
        GM = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (life <= 0 && isMonitering == true && Activated == false)
        {
            //towerGenStats.AdaptRules( Kills, turretspawned.timeSpawned, turretspawned.enemyPassed, timer);
            //Destroy(gameObject);


            Debug.Log("Dead");
            Activated = true;
            if (towerGenStats != null)
            {
                towerGenStats.FailureTower();
            }
            LastContact = timer;
            GM.timer = 0;
            GM.conditionMet = 0;
            GM.gold = 10;
            StartCoroutine(ReloadFn());
        }
        else
        {
            timer += Time.deltaTime;
        }

        if (timer >= timeRemaining) 
        {
            //towerGenStats.AdaptRules(Kills, turretspawned.timeSpawned, turretspawned.enemyPassed, timer);
            //Destroy(gameObject);
            Debug.Log("Dead");
            Activated = true;
            if (towerGenStats != null)
            {
                towerGenStats.FailureTower();
            }
            LastContact = timer;
            GM.timer = 0;
            GM.conditionMet = 0;
            GM.gold = 10;
            StartCoroutine(ReloadFn());
        }

        if (life <= 0 && isMonitering == false && Activated == false)
        {
            Debug.Log("Dead");
            Activated = true;
            if (towerGenStats != null)
            {
                towerGenStats.FailureTower();
            }
            LastContact = timer;
            GM.timer = 0;
            GM.conditionMet = 0;
            GM.gold = 10;
            StartCoroutine(ReloadFn());
        }


        if (!isMonitering)
        {
            turretspawned = FindObjectOfType<Turret>();
        }
        if (turretspawned != null)
        {
            isMonitering=true;
        }

        
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");
        // Check if the collided object has the tag "Enemy"
        if (collision.gameObject.CompareTag("enemy"))
        {
            // Log the name of the enemy GameObject we collided with
            Debug.Log("Collided with Enemy: " + collision.gameObject.name);
            Destroy(collision.gameObject);
            life--;
            if (towerCreation != null)
            { 
                towerCreation.enemyPassedCounter += 2;
            }
            if(isFirstContact == false)
            {
                isFirstContact = true;
                FirstContact = timer;
            }
        }
    }

    IEnumerator ReloadFn()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tagToFind);
        foreach (GameObject obj in objectsWithTag)
        {
            Turret turretScript = obj.GetComponent<Turret>();
            if (turretScript != null)
            {
                turretScript.UpdateAi();
            }
        }
        if (towerGenStats != null)
        {
            towerGenStats.ID_Increment();
        }

        if (towerCreation != null)
        {
            towerCreation.FirstFunction(FirstContact, LastContact);
            towerCreation.ListClearAndGlobalReset();



        }

        yield return new WaitForSeconds(1);
        Debug.Log("Reload");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
