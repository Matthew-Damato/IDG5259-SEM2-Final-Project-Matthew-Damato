using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointChild : MonoBehaviour
{
    public EndGoal GM;
    //public Turret turretStore;
    public GameObject[] objectsWithTag;


    public EnemyPointAI AIparent;


    public GameObject[] EntityNodes;
    private int numberOfEntities;
    private bool TargetReached = false;
    private int currentTarget = 0;
    public float stoppingDistance = 1f;

    public float Health = 1;
    private string LastHit_UID;

    public float MovSpeed = 5f;

    public bool RandomOrFixed = false;
    private bool Upgrade1 = false;
    private bool Upgrade2 = false;
    private bool Upgrade3 = false;
    private bool immuneToSlow = false;  

    // Start is called before the first frame update
    void Start()
    {
        AIparent = FindObjectOfType<EnemyPointAI>();
        EntityNodes = AIparent.EntityNodes;
        GM = FindObjectOfType<EndGoal>();
        


        if ((GM.timer >= 200))
        {
            Health += Mathf.Round(GM.timer/200);
        }

        if (GM.timer >= 300 && Upgrade1 == false)
        {
            MovSpeed = MovSpeed + 0.1f;
            Upgrade1 = true;
        }
        else if ((GM.timer >= 900) && Upgrade2 == false)
        {
            MovSpeed = MovSpeed +0.2f;
            Upgrade2 = true;
        }
        else if (GM.timer >= 1100 && Upgrade3 == false)
        {
            MovSpeed = MovSpeed+0.3f;
            Upgrade3 = true;
        }
        //UID = (int)Health;
        numberOfEntities = EntityNodes.Length;
    }

    // Update is called once per frame
    void Update()
    {
        objectsWithTag = GameObject.FindGameObjectsWithTag("tower");
        //turretStore = FindObjectOfType<Turret>();
        /*
        if (Health <= 0)
        {

            turretStore.UpdateKill();
            Destroy(this.gameObject);
        }
        */


        if(MovSpeed < 0)
        {
            MovSpeed = 0.1f;
            immuneToSlow = true;
        }
    }

    void FixedUpdate()
    {
        Seek();
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
        if (currentTarget < numberOfEntities - 1)
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
        Debug.Log("Shot" + Health);
        // Check if the collided object has the specified tag
        if (collision.gameObject.CompareTag("bullet"))
        {
            Bullet blt = collision.gameObject.GetComponent<Bullet>();
            LastHit_UID = blt.bulletID;
            // Call the function you want to execute
            
            Health = Health - blt.damageValue;
            if(blt.SlowDown > 0 && immuneToSlow == false)
            {
                MovSpeed = MovSpeed - blt.SlowDown;
            }
            Destroy(collision.gameObject);
            if (Health <= 0)
            {
                foreach (GameObject obj in objectsWithTag)
                {
                    
                    Turret turretScript = obj.GetComponent<Turret>();
                    if (turretScript != null)
                    {
                        if (turretScript.UID == LastHit_UID)
                        {
                            turretScript.UpdateKill();
                            Destroy(this.gameObject);
                        }
                    }

                    TurretScriptTrue ts = obj.GetComponent<TurretScriptTrue>();
                    if (ts != null)
                    {
                        if (ts.UID == LastHit_UID)
                        {
                            ts.UpdateKill();
                            //Destroy(this.gameObject);
                        }
                    }




                }
                Destroy(this.gameObject);
            }
        }
    }




}

