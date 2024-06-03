/*
 * The way the tower defense section will function:
 * 
 * 
 * Two global variables:
 *  Global damage 
 *  Global rate
 *  
 * Both can be increased using Gold
 * 
 * Gold is used to determine the cost of the tower, as well as its stats.
 * As one increases gold, the tower's power increases, but it takes longer to spawn.
 * 
 *  There are also two possible spawn points. 
 * Point A and B. 
 * 
 * All parameters are decided prior to the spawning of the tower. The AI then just waits till the gold requirements are reached
 * 
 * 
 * In terms of Feedback:
 * 
 *  Gold
 *  Total Kills
 *  Time lasted in the round
 *  Time till first tower spawned.
 *  Number of enemies passed total.
 *  Number of enemies that passed prior to tower spawning.
 * 
 */



using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCreationScript : MonoBehaviour
{

    public List<TurretScriptTrue> turretScriptHold;

    [Header("Shared Settings")]
    public float Gold = 20f;
    public float GlobalDamage = 0;
    public float GlobalRate = 0;
    public float GlobalSpecial = 0;
    public float GlobalRange = 0;

    [Header("Base Settings")]
    public float BaseDamage = 50f;
    public float BaseRate = 100f;
    public float BaseRange = 3f;
    public float AdjustedDamage;
    public float AdjustedRate;

    [Header("Misc")]
    public float goldGainRate = 1f;
    public GameObject Area1;
    public GameObject Area2;
    public GameObject Tower;
    public float maxGold = 500;
    public float minGold = 1;
    public float StartingGold = 20f;
    public float GoldHold = 20f;

    //The First value is for Spawn point A.The Second is for spawn point B

    [Header("SpawnLocations")]
    private GameObject FirstSpawnLoc;
    private GameObject SecondSpawnLoc;
    public bool TowerSpawned = false;
    public bool TowerSpawned2 = false;
    public float[] maxGoldFirstTower = { 500, 500 };
    public float[] maxGoldSecondTower = { 500, 500 };


    [Header("Past tower data")]
    public float[] TowerDamageCost;
    public float[] TowerDamageDmg;
    public float[] TowerDamageRat;
    public float[] TowerDamagekill;





    //Tower 1 is the damage tower
    private float Tower1;
    //Tower 2 is the rate of fire tower,
    private float Tower2;



    public float Timer;

    // Path and data to save results in:
    private string directoryPath = @"C:\Users\Owner\Documents\Unity projects\Tower Defence AI\Assets\Results";
    private string fileName = "results.txt";
    private string filePath;
    private int loopNo = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Area1 == null)
        {
            Area1 = GameObject.FindWithTag("pointA");
        }
        if (Area2 == null)
        {
            Area2 = GameObject.FindWithTag("pointB");
        }

        Timer += Time.deltaTime;

    }

    void FixedUpdate()
    {
        GoldGeneration();
        GoldToPowerAdjust();

        if (Gold >= maxGoldFirstTower[0] && TowerSpawned == false && endFn == false)
        {
            //Spawn the first tower in area A
            GameObject TowerClone1 = Instantiate(Tower, Area1.transform.position, Area1.transform.rotation);
            
            //This is a damage Tower
            TurretScriptTrue Tst = TowerClone1.GetComponent<TurretScriptTrue>();
            Tst.AvilableEvolutionPaths.Add("damage");
            Tst.damage = GlobalDamage/100;
            Tst.fireRate = BaseRate/100;
            Tst.frange = BaseRange;
            Tst.UID = "A";
            turretScriptHold.Add(Tst);

            NoTowerSpawned = false;
            TowerSpawned = true;
            Gold = Gold - maxGoldFirstTower[0];
        }

        if(Gold >= maxGoldSecondTower[0] && TowerSpawned2 == false && endFn == false)
        {
            GameObject TowerClone2 = Instantiate(Tower, Area2.transform.position, Area2.transform.rotation);
            
            TurretScriptTrue Tst2 = TowerClone2.GetComponent<TurretScriptTrue>();
            Tst2.AvilableEvolutionPaths.Add("rate");
            Tst2.damage = BaseDamage / 100;
            Tst2.fireRate = GlobalRate / 100;
            Tst2.frange = BaseRange;
            Tst2.UID = "B";
            turretScriptHold.Add(Tst2);


            NoSecondTowerSpawned = false;
            TowerSpawned2 = true;
            Gold = Gold - (maxGoldSecondTower[0]);
            goldGainRate = 2;
        }




        if (TowerSpawned == true && TowerSpawned2 == true)
        {
            
            if((Mathf.Round(Gold) % 100 == 0) && CanEvolve == true)
            {
                Debug.Log("Show the Gold: " + Mathf.Round(Gold));
                TowerUpgrade();
                Debug.Log("Time to evolve");
                CanEvolve = false;
            }
        }

        if (CanEvolve == false)
        {
            StartCoroutine(ReactivateEvolve());
        }
    }
    private bool CanEvolve = true;

    public void GoldGeneration()
    {
        Debug.Log("Check: " + Gold);
        Gold = Gold + (Time.deltaTime*goldGainRate);
        //Gold = Mathf.Round(Gold);
    }

    IEnumerator ReactivateEvolve()
    {
        yield return new WaitForSeconds(2);
        CanEvolve = true ;
    }




    void GoldToPowerAdjust()
    {
        //We need to have an algorithm where a gold price is decided, and points are then allocated accodingly.
        if (Gold > maxGold)
        {
            Gold = maxGold;
        }

        
        //float GoldToSpend = Random.Range(minGold, maxGold);
        //DistributeGold(GoldToSpend);
        GlobalDamage = BaseDamage + maxGoldFirstTower[0];
        GlobalRate = BaseRate + maxGoldSecondTower[0];
        //You can easily check the price of each tower by checking the value of Tower 1 and Tower 2.


    }






    //Now spawn the towers
    void TowerSpawn()
    {
        GameObject PosSpawn;
        if (Tower1 <= maxGoldFirstTower[0])
        {
            PosSpawn = Area1;
            //Spawn the damage tower first at one of the 2 positions.
        }
        else if (Tower2 <= maxGoldFirstTower[0])
        {
            PosSpawn = Area1;
            //Spawn the rate tower first.
        }
        else if (Tower1 <= maxGoldFirstTower[1])
        {
            PosSpawn = Area2;
            //Spawn the damage tower first.
        }
        else if (Tower2 <= maxGoldFirstTower[1])
        {
            PosSpawn = Area2;
            //Spawn the rate tower first.
        }
        else
        {
            //Both towers are invalid to spawn.
        }


    }

    void DistributeGold(float amount)
    {
        Tower1 = Random.Range(0, amount);
        Tower2 = amount- Tower1;
    }


    //Adjust the amount of gold that can be allocated for the towers.
    void MaxGoldAdjust()
    {
        //If the damagetower didn't spawn:
        if(TowerSpawned == false)
        { 
            if (FirstSpawnLoc == Area1)
            {
                maxGoldFirstTower[0] -= 10;
            }
            else
            {
                maxGoldFirstTower[1] -= 10;
            }

        }
        else if(TowerSpawned2 == false)
        {
            if (SecondSpawnLoc == Area1)
            {
                maxGoldSecondTower[0]--;
            }
            else
            {
                maxGoldSecondTower[1]--;
            }
        }
    }





















    public bool NoTowerSpawned = true;
    public bool NoSecondTowerSpawned = true;

    public bool FirstFnSatisfied = false;
    public bool endFn = false;

    public int enemyPassedCounter;

    //The purpose of this algorithm is to find the maximum gold that can be used to spawn a tower at each location.
    //For now, we'll ignore the differant spawn points.
    public void FirstFunction(float firstContact, float lastContact)
    {
        //If no tower spawned, Set the max gold for 
        if (NoTowerSpawned == true)
        {
            endFn = true;
            maxGoldFirstTower[0] = Gold - (lastContact - firstContact) - (enemyPassedCounter);
            maxGoldFirstTower[1] = Gold - (lastContact - firstContact) - (enemyPassedCounter);
        }

        else if (NoSecondTowerSpawned == true)
        {
            endFn = true;
            maxGoldSecondTower[0] = Gold - (lastContact - firstContact) - (enemyPassedCounter * 2);
            maxGoldSecondTower[1] = Gold - (lastContact - firstContact) - (enemyPassedCounter * 2);
        }
        else
        {
            FirstFnSatisfied = true;
            maxGold = maxGoldFirstTower[0] + maxGoldSecondTower[0]*10;
        }
    }

    //If FirstFnSatisfied is true, that means the gold has been decided

    //Now to tune the values further
    //This function is called once the first is confirmed true. 
    //It will start from the max gold of each, and increase 1 while decreasing the other.
    public void SecondFunction()
    {
        
        //Start by setting the stats:
        GlobalDamage = BaseDamage + maxGoldFirstTower[0];
        GlobalRate = BaseRate + maxGoldSecondTower[0];



    }






    int RoundToNearestTen(int number)
    {
        return Mathf.RoundToInt(number / 10.0f) * 10;
    }
    int DivideAndRoundToNearestWholeNumber(int number)
    {
        return Mathf.RoundToInt(number / 10.0f);
    }


    //So this will adjust the rates to adjust the 2 towers
    public void AdjustFn1()
    {

        int temp;
        
        if (Timer < 580)
        {
            temp = 600 - (int)Mathf.Round(Timer);
            temp = RoundToNearestTen(temp);
            temp = DivideAndRoundToNearestWholeNumber(temp);


            if ((maxGoldFirstTower[0] - temp) < 10) 
            {
                maxGoldFirstTower[0] = 10;
                maxGoldSecondTower[0] = maxGoldSecondTower[0] + (maxGoldFirstTower[0] - 10);
                Timer = 0;
            }

            else
            {
                maxGoldFirstTower[0] -= temp;
                maxGoldSecondTower[0] += temp;
                Timer = 0;
            }
            
        }
    }


    //Costs need to be arrays for the 2 towers
    //Slow Slows down all  hit targets
    public float specialStatSlow;
    public float[] specialStatSlow_Cost = { 50, 50};
    //multi shoots more bullets
    public float specialStatMulti;
    public float[] specialStatMulti_Cost = { 150, 150 };
    //Burst shoots 1 instakill every so often
    public float specialStatBurst;
    public float[] specialStatBurst_Cost = { 100, 100 };

    public float[] DmgUpgrade_Cost = { 50, 50 };
    public float[] RngUpgrade_Cost = { 50, 50 };
    public float[] RateUpgrade_Cost = { 50, 50 };
    public float[] specialStatGold = {50, 50};

    List<bool> potentialTowers;
    public List<int> PossibleTowers;

    //Implement a buy and upgrade function for the towers:
    public void TowerUpgrade() 
    {
        Debug.Log("Time to grow");
        //Choose a tower to upgrade:
        int towerToUpgrade = 0;
        CanEvolve = false;
        /*
        foreach (TurretScriptTrue TST in turretScriptHold)
        {
            bool temp = TST.NeedsEvolution();
            Debug.Log("Called NeedsEvo "+ temp);
            potentialTowers.Add(temp);
        }
        */

        bool temp1 = turretScriptHold[0].NeedsEvolution();
        bool temp2 = turretScriptHold[1].NeedsEvolution();
        //potentialTowers.Add(temp1);
        //potentialTowers.Add(temp2);







        TowerUpgradeSetter(temp1, temp2);

        int _temp = Random.Range(0, PossibleTowers.Count);
        towerToUpgrade = PossibleTowers[_temp];

        //int towerToUpgrade = Random.Range(1, 3);
        /*Decide on an upgrade for the tower:
         * To do this, We need to get the current upgrade cost for that path for the tower,
         * and the number of mutations.
         * 
         * Each tower has a limited number of mutations assigned on creation.
         */

        //Get the mutation rate of the tower picked:
        float mutation = turretScriptHold[towerToUpgrade].MutationSpaces;


        if (towerToUpgrade == 1) 
        {
            Debug.Log("Problem tower");
        }

        //If there are no mutation slots left, upgrade what's there already. 
        //I need a way to save whatever's been selected so that it doesn't get replaced


        //If no mutation, check that there is enough  gold. then upgrade
        if (mutation == 0)
        {
            AddValuesToFileBefore(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount);
            switch (turretScriptHold[towerToUpgrade].AvilableEvolutionPaths[Random.Range(0, turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Count)])
            {
                case "damage":
                    if (Mathf.Round(Gold) >= DmgUpgrade_Cost[towerToUpgrade])
                    {
                        Gold -= DmgUpgrade_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].Gld += DmgUpgrade_Cost[towerToUpgrade];
                        //turretScriptHold[towerToUpgrade].damage += 50/100;
                        turretScriptHold[towerToUpgrade].DmgIncrease();
                        DmgUpgrade_Cost[towerToUpgrade] = DmgUpgrade_Cost[towerToUpgrade] * 2;
                        turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                        AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "damage");
                        AddValuesToFileClose();
                        break;
                    }
                    else
                    {
                        AddValuesToFileFailedUpgradeString("damage");
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                    
                case "rate":
                    if (Mathf.Round(Gold) >= RateUpgrade_Cost[towerToUpgrade])
                    {
                        Gold -= RateUpgrade_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].Gld += RateUpgrade_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].RateIncrease();
                        RateUpgrade_Cost[towerToUpgrade] = RateUpgrade_Cost[towerToUpgrade] * 2;
                        turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                        AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "rate");
                        AddValuesToFileClose();
                        break;
                    }
                    else
                    {
                        AddValuesToFileFailedUpgradeString("rate");
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                    
                case "range":
                    if (Mathf.Round(Gold) >= RngUpgrade_Cost[towerToUpgrade])
                    {
                        Gold -= RngUpgrade_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].Gld += RngUpgrade_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].frange += 0.5f;
                        RngUpgrade_Cost[towerToUpgrade] = RngUpgrade_Cost[towerToUpgrade] * 2;
                        turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                        AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "range");
                        AddValuesToFileClose();
                        break;
                    }
                    else
                    {
                        AddValuesToFileFailedUpgradeString("range");
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                    
                case "slow":
                    if (Mathf.Round(Gold) >= specialStatSlow_Cost[towerToUpgrade])
                    {
                        Gold -= specialStatSlow_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].Gld += specialStatSlow_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].specialStatSlow += 1;
                        specialStatSlow_Cost[towerToUpgrade] = specialStatSlow_Cost[towerToUpgrade] * 2;
                        turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                        AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "slow");
                        AddValuesToFileClose();
                        break;
                    }
                    else
                    {
                        AddValuesToFileFailedUpgradeString("slow");
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                    
                case "multi":
                    if (Mathf.Round(Gold) >= specialStatMulti_Cost[towerToUpgrade]) 
                    {
                        Gold -= specialStatMulti_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].Gld += specialStatMulti_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].specialStatMulti += 1;
                        specialStatMulti_Cost[towerToUpgrade] = specialStatMulti_Cost[towerToUpgrade] * 2;
                        turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                        AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "multi");
                        AddValuesToFileClose();
                        break; 
                    }
                    else
                    {
                        AddValuesToFileFailedUpgradeString("multi");
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                    
                case "burst":
                    if (Mathf.Round(Gold) >= specialStatBurst_Cost[towerToUpgrade])
                    {
                        Gold -= specialStatBurst_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].Gld += specialStatBurst_Cost[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].specialStatBurst += 1;
                        specialStatBurst_Cost[towerToUpgrade] = specialStatBurst_Cost[towerToUpgrade] * 2;
                        turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                        AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "burst");
                        AddValuesToFileClose();
                        break;
                    }
                    else
                    {
                        AddValuesToFileFailedUpgradeString("burst");
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                case "gold":
                    if (turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Contains("Gold"))
                    {
                        Gold -= specialStatGold[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].Gld += specialStatGold[towerToUpgrade];
                        turretScriptHold[towerToUpgrade].specialStatGold += 1;
                        specialStatGold[towerToUpgrade] = specialStatGold[towerToUpgrade] * 2;
                        goldGainRate = goldGainRate + 1;
                        turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                        AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "gold-digger");
                        AddValuesToFileClose();
                        break;
                    }
                    else
                    {
                        AddValuesToFileFailedUpgradeString("Gold");
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                default:
                    Debug.Log("Upgrade failed - " + turretScriptHold[towerToUpgrade].AvilableEvolutionPaths[Random.Range(0, turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Count)]);
                    break;
            }
            
        }


        //else, randomly select an area to upgrade, and if it's a mutation, subtract a mutation marker.

        else
        {
            AddValuesToFileBefore(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount);
            int area_To_Upgrade = Random.Range(0,7);
            switch (area_To_Upgrade)
            {
                case 0:
                    Debug.Log("Check prices: " + Mathf.Round(Gold) + " Upgrade: " + specialStatBurst_Cost[towerToUpgrade]);
                    if (Mathf.Round(Gold) >= DmgUpgrade_Cost[towerToUpgrade])
                    {
                        if (turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Contains("damage"))
                        {
                            Gold -= DmgUpgrade_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += DmgUpgrade_Cost[towerToUpgrade];
                            //turretScriptHold[towerToUpgrade].damage = turretScriptHold[towerToUpgrade].damage + (50 /100);
                            turretScriptHold[towerToUpgrade].DmgIncrease();

                            DmgUpgrade_Cost[towerToUpgrade] = DmgUpgrade_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "damage");
                            AddValuesToFileClose();
                            break;
                        }
                        else
                        {
                            Gold -= DmgUpgrade_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += DmgUpgrade_Cost[towerToUpgrade];
                            //turretScriptHold[towerToUpgrade].damage = turretScriptHold[towerToUpgrade].damage + (50 /100);
                            turretScriptHold[towerToUpgrade].DmgIncrease();

                            DmgUpgrade_Cost[towerToUpgrade] = DmgUpgrade_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Add("damage");
                            turretScriptHold[towerToUpgrade].MutationSpaces--;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "damage");
                            AddValuesToFileClose();
                            break;
                        }
                    }
                    else
                    {
                        AddValuesToFileFailedUpgrade(area_To_Upgrade);
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                    
                case 1:
                    Debug.Log("Check prices: " + Mathf.Round(Gold) + " Upgrade: " + specialStatBurst_Cost[towerToUpgrade]);
                    if (Mathf.Round(Gold) >= RateUpgrade_Cost[towerToUpgrade])
                    {
                        if (turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Contains("rate"))
                        {
                            Gold -= RateUpgrade_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += RateUpgrade_Cost[towerToUpgrade];
                            //turretScriptHold[towerToUpgrade].fireRate = turretScriptHold[towerToUpgrade].fireRate + 0.5f;
                            turretScriptHold[towerToUpgrade].RateIncrease();

                            RateUpgrade_Cost[towerToUpgrade] = RateUpgrade_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "rate");
                            AddValuesToFileClose();
                            break;
                        }
                        else
                        {
                            Gold -= RateUpgrade_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += RateUpgrade_Cost[towerToUpgrade];
                            //turretScriptHold[towerToUpgrade].fireRate = turretScriptHold[towerToUpgrade].fireRate + 0.5f;
                            turretScriptHold[towerToUpgrade].RateIncrease();

                            RateUpgrade_Cost[towerToUpgrade] = RateUpgrade_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Add("rate");
                            turretScriptHold[towerToUpgrade].MutationSpaces--;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "rate");
                            AddValuesToFileClose();
                            break;
                        }
                    }
                    else
                    {
                        AddValuesToFileFailedUpgrade(area_To_Upgrade);
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                    
                case 2:
                    Debug.Log("Check prices: " + Mathf.Round(Gold) + " Upgrade: " + specialStatBurst_Cost[towerToUpgrade]);
                    if (Mathf.Round(Gold) >= RngUpgrade_Cost[towerToUpgrade])
                    {
                        if (turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Contains("range"))
                        {
                            Gold -= RngUpgrade_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += RngUpgrade_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].frange += 0.5f;
                            RngUpgrade_Cost[towerToUpgrade] = RngUpgrade_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "range");
                            AddValuesToFileClose();
                            break;
                        }
                        else
                        {
                            Gold -= RngUpgrade_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += RngUpgrade_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].frange += 0.5f;
                            RngUpgrade_Cost[towerToUpgrade] = RngUpgrade_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Add("range");
                            turretScriptHold[towerToUpgrade].MutationSpaces--;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "range");
                            AddValuesToFileClose();
                            break;
                        }
                    }
                    else
                    {
                        AddValuesToFileFailedUpgrade(area_To_Upgrade);
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                    
                case 3:
                    Debug.Log("Check prices: " + Mathf.Round(Gold) + " Upgrade: " + specialStatBurst_Cost[towerToUpgrade]);
                    if (Mathf.Round(Gold) >= specialStatSlow_Cost[towerToUpgrade])
                    {
                        if (turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Contains("slow"))
                        {
                            Gold -= specialStatSlow_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += specialStatSlow_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].specialStatSlow += 1;
                            specialStatSlow_Cost[towerToUpgrade] = specialStatSlow_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "slow");
                            AddValuesToFileClose();
                            break;
                        }
                        else
                        {
                            Gold -= specialStatSlow_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += specialStatSlow_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].specialStatSlow += 1;
                            specialStatSlow_Cost[towerToUpgrade] = specialStatSlow_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Add("slow");
                            turretScriptHold[towerToUpgrade].MutationSpaces--;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "slow");
                            AddValuesToFileClose();
                            break;
                        }
                    }
                    else
                    {
                        AddValuesToFileFailedUpgrade(area_To_Upgrade);
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                    
                case 4:
                    Debug.Log("Check prices: " + Mathf.Round(Gold) + " Upgrade: " + specialStatBurst_Cost[towerToUpgrade]);
                    if (Mathf.Round(Gold) >= specialStatMulti_Cost[towerToUpgrade])
                    {
                        if (turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Contains("multi"))
                        {
                            Gold -= specialStatMulti_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += specialStatMulti_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].specialStatMulti += 1;
                            specialStatMulti_Cost[towerToUpgrade] = specialStatMulti_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].damage -= 10/100;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "multi");
                            AddValuesToFileClose();
                            break;
                        }
                        else
                        {
                            Gold -= specialStatMulti_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += specialStatMulti_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].specialStatMulti += 1;
                            specialStatMulti_Cost[towerToUpgrade] = specialStatMulti_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].damage -= 25/100;
                            turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Add("multi");
                            turretScriptHold[towerToUpgrade].MutationSpaces--;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "multi");
                            AddValuesToFileClose();
                            break;
                        }
                    }
                    else
                    {
                        AddValuesToFileFailedUpgrade(area_To_Upgrade);
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }

                case 5:
                    Debug.Log("Check prices: " + Mathf.Round(Gold) + " Upgrade: " + specialStatBurst_Cost[towerToUpgrade]);
                    if (Mathf.Round(Gold) >= specialStatBurst_Cost[towerToUpgrade])
                    {
                        if (turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Contains("burst"))
                        {
                            Gold -= specialStatBurst_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += specialStatBurst_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].specialStatBurst += 1;
                            specialStatBurst_Cost[towerToUpgrade] = specialStatBurst_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "burst");
                            AddValuesToFileClose();
                            break;
                        }
                        else
                        {
                            Gold -= specialStatBurst_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += specialStatBurst_Cost[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].specialStatBurst += 1;
                            specialStatBurst_Cost[towerToUpgrade] = specialStatBurst_Cost[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Add("burst");
                            turretScriptHold[towerToUpgrade].MutationSpaces--;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "burst");
                            AddValuesToFileClose();
                            break;
                        }
                    }
                    else
                    {
                        AddValuesToFileFailedUpgrade(area_To_Upgrade);
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                case 6:
                    Debug.Log("Check prices: " + Mathf.Round(Gold) + " Upgrade: " + specialStatGold[towerToUpgrade]);
                    if (Mathf.Round(Gold) >= specialStatGold[towerToUpgrade])
                    {
                        if (turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Contains("Gold"))
                        {
                            Gold -= specialStatGold[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += specialStatGold[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].specialStatGold += 1;
                            specialStatGold[towerToUpgrade] = specialStatGold[towerToUpgrade] * 2;
                            goldGainRate = goldGainRate + 1;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "gold-digger");
                            AddValuesToFileClose();
                            break;
                        }
                        else
                        {
                            Gold -= specialStatGold[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].Gld += specialStatGold[towerToUpgrade];
                            turretScriptHold[towerToUpgrade].specialStatGold += 1;
                            specialStatGold[towerToUpgrade] = specialStatGold[towerToUpgrade] * 2;
                            turretScriptHold[towerToUpgrade].AvilableEvolutionPaths.Add("gold");
                            turretScriptHold[towerToUpgrade].MutationSpaces--;
                            goldGainRate = goldGainRate + 1;
                            turretScriptHold[towerToUpgrade].upgradedAmount += 1;
                            AddValuesToFileAfter(towerToUpgrade, turretScriptHold[towerToUpgrade].damage, turretScriptHold[towerToUpgrade].frange, turretScriptHold[towerToUpgrade].fireRate, turretScriptHold[towerToUpgrade].specialStatSlow, turretScriptHold[towerToUpgrade].specialStatMulti, turretScriptHold[towerToUpgrade].specialStatBurst, turretScriptHold[towerToUpgrade].specialStatGold, turretScriptHold[towerToUpgrade].Gld, turretScriptHold[towerToUpgrade].timeSpawned, turretScriptHold[towerToUpgrade].upgradedAmount, "Gold-digger");
                            AddValuesToFileClose();
                            break;
                        }
                    }
                    else
                    {
                        AddValuesToFileFailedUpgrade(area_To_Upgrade);
                        Debug.Log("Upgrade failed - Not enough Gold");
                        break;
                    }
                default:
                    AddValuesToFileFailedUpgrade(area_To_Upgrade);
                    Debug.Log("Upgrade failed - "+ area_To_Upgrade);
                    break;

            }
            
        }


        //At this point, the tower has been upgraded and set.

        //Clear the list:
        //potentialTowers.Clear();
        PossibleTowers.Clear();



    }



    //The Below function will check which towers to upgrade:
    public void TowerUpgradeSetter(bool temp1, bool temp2)
    {
        Debug.Log("printing everything related to the error: "+temp1 + " " + temp2);
        if (temp1 == false)
        {
            if (temp2 == false)
            {
                PossibleTowers.Add(0);
                PossibleTowers.Add(1);
            }
            if (temp2 == true)
            {
                PossibleTowers.Add(1);
            }
        }
        if (temp1 == true)
        {
            PossibleTowers.Add(0);
            if (temp2 == true)
            {
                PossibleTowers.Add(1);
            }
        }
    }




    //Create a function to save the data.
    public void PathCreate()
    {
        loopNo++;
        //Prep by creating a file to save results in.
        string fileNameCombined = loopNo.ToString() +" "+ fileName;
        filePath = Path.Combine(directoryPath, fileNameCombined);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log("Directory created at: " + directoryPath);
        }
        // Check if the file exists, if not create it
        if (!File.Exists(filePath))
        {
            CreateFile();
        }
    }
    void CreateFile()
    {
        // Create a new file and close it immediately
        File.Create(filePath).Close();
        Debug.Log("File created at: " + filePath);
    }
    //Add values to the file:
    void AddValuesToFileBefore(float TowerNo, float _damage, float _range, float _rate, float _slow, float _multi, float _burst, float _gldUpgrade , float _gold, float _time, float _upgradeAmount)
    {
        // Create the content to write
        string content = $"Before upgrading: \nTower number = {TowerNo}\nDamage = {_damage}\nRange = {_range}\nRate = {_rate}\nSlow = {_slow}\nMulti = {_multi}\nBurst = {_burst}\nGold-digger level: {_gldUpgrade}\nGold = {_gold}\nTime = {_time}\nUpgrade No. = {_upgradeAmount}\n\n";


        // Write the content to the file
        File.AppendAllText(filePath, content);
        Debug.Log("Values added to file: " + filePath);
    }
    void AddValuesToFileAfter(float TowerNo, float _damage, float _range, float _rate, float _slow, float _multi, float _burst, float _gldUpgrade , float _gold, float _time, float _upgradeAmount, string _path)
    {
        // Create the content to write
        string content = $"After upgrading: \nTower number = {TowerNo}\nDamage = {_damage}\nRange = {_range}\nRate = {_rate}\nSlow = {_slow}\nMulti = {_multi}\nBurst = {_burst}\nGold-digger level: {_gldUpgrade}\nGold = {_gold}\nTime = {_time}\nUpgrade No. = {_upgradeAmount}\nLast upgrade: {_path}\n";


        // Write the content to the file
        File.AppendAllText(filePath, content);
        Debug.Log("Values added to file: " + filePath);
    }
    void FinalValues(float TowerNo, float _damage, float _range, float _rate, float _slow, float _multi, float _burst, float _gldUpgrade , float _gold, float _time, float _upgradeAmount, float _kills)
    {
        // Create the content to write
        string content = $"Final Values: \nTower number = {TowerNo}\nDamage = {_damage}\nRange = {_range}\nRate = {_rate}\nSlow = {_slow}\nMulti = {_multi}\nBurst = {_burst}\nGold-digger level: {_gldUpgrade}\nGold = {_gold}\nTime = {_time}\nUpgrade No. = {_upgradeAmount}\nKills = {_kills}\n\n\n___________________________\n\n";

        // Write the content to the file
        File.AppendAllText(filePath, content);
        Debug.Log("Values added to file: " + filePath);
    }
    void FinalReport()
    {
        // Create the content to write
        string content = $"Time Taken for this attempt: {Timer} \n\n___________________________\n\n";

        // Write the content to the file
        File.AppendAllText(filePath, content);
        Debug.Log("Values added to file: " + filePath);
    }



    void AddValuesToFileClose()
    {
        // Create the content to write
        string content = $"Upgrade Concluded. \n\n___________________________\n\n";

        // Write the content to the file
        File.AppendAllText(filePath, content);
        Debug.Log("Values added to file: " + filePath);
    }

    void AddValuesToFileFailedUpgrade(float ValToUpgrade)
    {
        // Create the content to write
        string content = $"Upgrade Concluded. No upgrade Occured. Value Attempted: {ValToUpgrade} \n\n___________________________\n\n";
        // Write the content to the file
        File.AppendAllText(filePath, content);
        Debug.Log("Values added to file: " + filePath);
    }
    void AddValuesToFileFailedUpgradeString(string ValToUpgrade)
    {
        // Create the content to write
        string content = $"Upgrade Concluded. No upgrade Occured. Value Attempted: {ValToUpgrade} \n\n___________________________\n\n";
        // Write the content to the file
        File.AppendAllText(filePath, content);
        Debug.Log("Values added to file: " + filePath);
    }


    public void ListClearAndGlobalReset()
    {
        
        if (turretScriptHold.Count >0)
        {
            FinalValues(0, turretScriptHold[0].damage, turretScriptHold[0].frange, turretScriptHold[0].fireRate, turretScriptHold[0].specialStatSlow, turretScriptHold[0].specialStatMulti, turretScriptHold[0].specialStatBurst, turretScriptHold[0].specialStatGold, turretScriptHold[0].Gld, turretScriptHold[0].timeSpawned, turretScriptHold[0].upgradedAmount, turretScriptHold[0].TotalKills);
            FinalReport();
        }

        if (turretScriptHold.Count > 1)
        {
            FinalValues(1, turretScriptHold[1].damage, turretScriptHold[1].frange, turretScriptHold[1].fireRate, turretScriptHold[1].specialStatSlow, turretScriptHold[1].specialStatMulti, turretScriptHold[1].specialStatBurst, turretScriptHold[1].specialStatGold, turretScriptHold[1].Gld, turretScriptHold[1].timeSpawned, turretScriptHold[1].upgradedAmount, turretScriptHold[1].TotalKills);
            FinalReport();
        }
        


        turretScriptHold.Clear();
        BaseDamage = 50;
        BaseRange = 3;
        BaseRate = 100;

        GlobalDamage = 0;
        GlobalRange = 0;
        GlobalRate = 0;
        GlobalSpecial = 0;

        specialStatSlow_Cost[0] = 50;
        specialStatSlow_Cost[1] = 50;

        specialStatMulti_Cost[0] = 150;
        specialStatMulti_Cost[1] = 150;

        specialStatBurst_Cost[0] = 100;
        specialStatBurst_Cost[1] = 100;

        DmgUpgrade_Cost[0] = 50;
        DmgUpgrade_Cost[1] = 50;

        RngUpgrade_Cost[0] = 50;
        RngUpgrade_Cost[1] = 50;

        RateUpgrade_Cost[0] = 50;
        RateUpgrade_Cost[1] = 50;

        goldGainRate = 1;


        PathCreate();
    }
}
