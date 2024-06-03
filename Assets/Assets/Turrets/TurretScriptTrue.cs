using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScriptTrue : MonoBehaviour
{
    public float killCount = 0;
    public int[] EnemyTypes;

    public Transform target;

    public TowerCreationScript towerCreation;

    [Header("Attributes")]
    public float frange = 15;
    public float fireRate = 1f;
    public float fireCountdown = 0f;
    public float damage = 1f;
    public float numberOfEnemies = 0f;
    public float Gld = 0f;
    public string path;
    public float shotsFired = 0;
    public string UID;


    public float specialStatSlow = 0;
    public float specialStatMulti = 0;
    public float specialStatBurst = 0;
    public float specialStatGold = 0;

    public float  MutationSpaces;

    public List<string> AvilableEvolutionPaths;

    [Header("Set up")]
    public Transform partToRotate;
    public float RotateSpeed = 5;
    public GameObject Bullet;
    public Transform SpawnPoint;
    public float timeSpawned = 0;
    public float enemyEnteredRange = 0;

    private float timeTillBurst = 0;
    public float upgradedAmount = 0;


    public float TotalKills = 0;

    public string enemyTag = "enemy";
    // Start is called before the first frame update
    void Start()
    {
        MutationSpaces = Random.Range(0, 4);
        towerCreation = FindObjectOfType<TowerCreationScript>();

        //frange = 5; 
        //fireRate = towerCreation.BaseRate/100;
        //damage = towerCreation.BaseDamage/100;
        Gld = towerCreation.Gold;

        InvokeRepeating("UpdateTarget", 0, 1f);

    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestdistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestdistance)
            {
                shortestdistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestdistance <= frange)
        {
            numberOfEnemies++;
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSpawned += Time.deltaTime;
        if (target == null)
        {
            return;
        }

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);

        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * RotateSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (fireCountdown <= 0f)
        {
            if (specialStatMulti > 0)
            {
                for (int i = 0; i < specialStatMulti; i++)
                {
                    Shoot();
                }
                fireCountdown = 1f / fireRate;
            }
            else
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            
        }
        if (specialStatBurst > 0)
        {
            if(timeTillBurst >= 5)
            {
                ShootBurst();
            }
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        Debug.Log("Shooting");
        shotsFired++;
        GameObject bClone = (GameObject)Instantiate(Bullet, SpawnPoint.position, SpawnPoint.rotation);
        Bullet bullet = bClone.GetComponent<Bullet>();
        bullet.Seek(target);
        bullet.bulletID = UID;
        bullet.damageValue = damage;
        bullet.SlowDown = specialStatSlow;

    }



    void ShootBurst()
    {
        Debug.Log("Shooting");
        shotsFired++;
        GameObject bClone = (GameObject)Instantiate(Bullet, SpawnPoint.position, SpawnPoint.rotation);
        Bullet bullet = bClone.GetComponent<Bullet>();
        bullet.Seek(target);
        bullet.bulletID = UID;
        bullet.damageValue = damage+specialStatBurst;
        //bullet.SlowDown = specialStatSlow;

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, frange);
    }


    public void UpdateKill()
    {
        killCount++;
        TotalKills++;
    }

    public void DmgIncrease()
    {
        Debug.Log("Damage Increase");
        damage = damage + 0.5f;
    }
    public void RateIncrease()
    {
        Debug.Log("Rate Increase");
        fireRate = fireRate + 0.5f;
    }








    public float Efficiency;
    public void EfficiencyCalc()
    {
        if (killCount > 0 && enemyEnteredRange > 0)
        {
            Efficiency = killCount / enemyEnteredRange;
        }
    }

    public bool NeedToEvolve = true;
    public bool NeedsEvolution()
    {
        Debug.Log("Called NeedsEvo");
        EfficiencyCalc();
        killCount = 0;
        enemyEnteredRange = 0;
        if (Efficiency > 0.9f)
        {
            return NeedToEvolve = false;
        }
        return NeedToEvolve = true;
    }
}
