using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointAI : MonoBehaviour
{
    public EndGoal GM;
    public Turret turretStore;


    public GameObject[] EntityNodes;
    private int numberOfEntities;
    private bool TargetReached = false;
    private int currentTarget = 0;
    public float stoppingDistance = 1f;

    private float Health = 1;
    private int UID = 0;

    public float MovSpeed = 5f; 

    public bool RandomOrFixed = false; 

    // Start is called before the first frame update
    void Start()
    {
        GM = FindObjectOfType<EndGoal>();
        turretStore = FindObjectOfType<Turret>();

        if ( (GM.timer%10) == 0)
        {
            Health += Random.Range(0, 2);
        }
        UID = (int)Health;
        numberOfEntities = EntityNodes.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
        {
            turretStore.UpdateKill();
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        //Seek();
    }


    void Seek()
    {
        
        if (EntityNodes.Length == 0)
        {
            return;
        }
        
        if (EntityNodes[currentTarget] == null)
        {
            return;
        }
        
        Vector3 position = EntityNodes[currentTarget].transform.position; 
        Vector3 direction = EntityNodes[currentTarget].transform.position - transform.position;

        if (direction.magnitude > stoppingDistance)
        {
            direction.Normalize();
            transform.position += direction * MovSpeed * Time.deltaTime;
        }
        else
        {
            if (RandomOrFixed == false)
            {
                IncreaseFixed();
            }
            if (RandomOrFixed == true)
            {
                IncreaseRandom();
            }
            
        } 
    }

    void IncreaseFixed()
    {
        if (currentTarget < numberOfEntities-1)
        {
            currentTarget++;
        }
        else
        {
            currentTarget = 0;
        }
    }

    void IncreaseRandom()
    {
        int tempHold = currentTarget;
        bool loop = true;
        while (loop == true)
        {
            if (currentTarget == tempHold)
            {
                currentTarget = Random.Range(0, numberOfEntities);
            }
            else
            {
                loop = false;
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the specified tag
        if (collision.gameObject.CompareTag("bullet"))
        {
            // Call the function you want to execute
            Health = Health - turretStore.damage;
        }
    }




    }
