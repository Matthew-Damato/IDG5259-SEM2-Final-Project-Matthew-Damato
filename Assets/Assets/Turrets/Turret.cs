using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameManager GM;
    //AddValuesToFile(float KpS, float Time, float timeBeforespawn, float kills)
    public int killCount = 0;
    public int[] EnemyTypes;

    public Transform target;

    public TowerGeneration towerGenStats;

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


    [Header("Set up")]
    public Transform partToRotate;
    public float RotateSpeed = 5;
    public GameObject Bullet;
    public Transform SpawnPoint;

    public string enemyTag = "enemy";


    public float timeSpawned = 0;
    public float enemyPassed = 0;
    // Start is called before the first frame update
    void Start()
    {
        GameObject TowerGeneratorGameObject = GameObject.FindWithTag("GameManager");
        if (TowerGeneratorGameObject != null ) { Debug.Log("Found!"); }
        towerGenStats = TowerGeneratorGameObject.GetComponent<TowerGeneration>();
        GM = FindObjectOfType<GameManager>();



        //towerGenStats.TowerGenerate();
        frange = towerGenStats.RangeReturn();
        fireRate = towerGenStats.RateReturn();
        damage = towerGenStats.DamageReturn();
        Gld = towerGenStats.GldReturn();
        path = towerGenStats.focus;

        towerGenStats.stringBuilder();
        UID = towerGenStats.UID;

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
            if(distanceToEnemy < shortestdistance)
            {
                shortestdistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if(nearestEnemy != null && shortestdistance <= frange)
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

        if(fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate; 
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        
        shotsFired++;
        GameObject bClone = (GameObject)Instantiate(Bullet, SpawnPoint.position, SpawnPoint.rotation);
        Bullet bullet = bClone.GetComponent<Bullet>();
        bullet.Seek(target);
        bullet.bulletID = UID;
        bullet.damageValue = damage;

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, frange);
    }


    public void UpdateKill()
    {
        killCount++;
    }

    public void UpdateAi()
    {
        Debug.Log(killCount + " "+ timeSpawned + " " + numberOfEnemies + " " + GM.timer + " " + path + " " + fireRate + " " + frange + " " + damage + " " + (int)Gld + " " + shotsFired + " " + UID);
        towerGenStats.TowerImprovementAlgorithm(killCount, timeSpawned, numberOfEnemies, GM.timer, path, fireRate, frange, damage, (int)Gld, shotsFired, UID);
    }
}
