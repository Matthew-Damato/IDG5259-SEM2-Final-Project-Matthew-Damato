using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGeneration : MonoBehaviour
{
    public static TowerGeneration GenInstance;

    public string UID;
    public float LoopNo;
    public float ID;

    //
    public float damageValue = 50;
    public float range = 50;
    public float rateOfFire = 50;

    private float DamageValueStart = 100;
    private float RangeValueStart = 100;
    private float RateOfFireValueStart = 100;


    // The amount decreased in range for every unit increase in damage or rate of fire
    private float rangeDecrease = 2;
    // The same for damage
    private float damageDecrease = 2;
    // The same for rate of fire
    private float rateDecrease = 2;

    //rate increases:
    private float rangeIncrease = 2;
    private float damageIncrease = 2;
    private float rateIncrease = 2;



    private float DmgAdjustDamage = 1;
    private float RngAdjustDamage = 1;
    private float RatAdjustDamage = 1;

    private float DmgAdjustRange = 1;
    private float RngAdjustRange = 1;
    private float RatAdjustRange = 1;

    private float DmgAdjustRate = 1;
    private float RngAdjustRate = 1;
    private float RatAdjustRate = 1;



    public string focus = null;
    public string[] focusValues = { "damage", "range", "rate" };

    private int repeatCounterDamage = 1;
    private int repeatCounterRange = 1;
    private int repeatCounterRate = 1;


    private float[] damageArray;
    private float[] rangeArray;
    private float[] rateArray;



    private bool isValid = true;




    public float MaxValDamage = 100;
    public float MaxRangeVal = 100;
    public float MaxRateVal = 1000;
    private int GoldSell = 10;
    public int GoldBuy = 10;
    private float Goldadjustment;
    public int firstTowerGoldLimit = 100;

    private bool SpecialEffect = false;



    // Start is called before the first frame update
    void Start()
    {
        //Clamp the values
        damageValue = Mathf.Clamp(damageValue, 0.0f, 200.0f);
        range = Mathf.Clamp(range, 0.0f, 200.0f);
        rateOfFire = Mathf.Clamp(rateOfFire, 0.0f, 200.0f);

        //TowerGenerate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TowerGenerate(string focusVal)
    {
        focus = focusVal;
        //Start by delcaring an area of focus.
        /*
        if(focus == null) 
        {
            int randomIndex = Random.Range(0, focusValues.Length);
            focus = focusValues[randomIndex];
        }
        else
        {
            int randomIndex = Random.Range(0, focusValues.Length);
            focus = focusValues[randomIndex];
        }
        */

        //BaseAdjust(focus);

        /*Now that we have an area of focus:
         * 
         * We need to create 3 general focus paths.
         * These take the values we started with, and:
         * - add to the area of focus
         * - decrease the other areas accordingly
         * - store the 3 values in an array
         * - check that these values have not been stored already
         * - if they have, adjust.
         * 
         * 
         * Finally, the generated tower is presented. 
         */
        float valueShiftDamage = Random.Range(1f, 2f);
        float valueShiftRange = Random.Range(1f, 2f);
        float valueShiftRate = Random.Range(1f, 2f);

        do
        {
            if (focus == "damage")
            {
                damageValue = DamageValueStart + (valueShiftDamage * DmgAdjustDamage);
                range = RangeValueStart - (RngAdjustDamage * valueShiftRange);
                rateOfFire = RateOfFireValueStart - (RatAdjustDamage * valueShiftRate);
                //These need to be adjusted after checks, not before.

                if (damageValue > BaseAdjust(focus))
                {
                    damageValue = BaseAdjust(focus);
                }
                if (range > MaxRangeVal)
                {
                    range = MaxRangeVal;
                }
                if (rateOfFire > MaxRateVal)
                {
                    rateOfFire = MaxRateVal;
                }
                if (damageValue < 1)
                {
                    damageValue = 1;
                }
                if (range < 1)
                {
                    range = 1;
                }
                if (rateOfFire < 1)
                {
                    rateOfFire = 1;
                }

                isValid = IsValueAccurate(damageValue, range, rateOfFire);
                
                //isValid = CheckIfNew(damageValue, range, rateOfFire);

                if(isValid == true)
                {
                    repeatCounterDamage++;
                    repeatCounterRange = 1;
                    repeatCounterRate = 1;

                    //StoreInArrays(damageValue, range, rateOfFire);
                }
                
            }
            else if (focus == "range")
            {
                damageValue = DamageValueStart - (valueShiftDamage * DmgAdjustRange);
                range = RangeValueStart + (RngAdjustRange * valueShiftRange);
                rateOfFire = RateOfFireValueStart - (RatAdjustRange * valueShiftRate);


                if (damageValue > MaxValDamage)
                {
                    damageValue = MaxValDamage;
                }
                if (range > BaseAdjust(focus))
                {
                    range = BaseAdjust(focus); 
                }
                if (rateOfFire > MaxRateVal)
                {
                    rateOfFire = MaxRateVal;
                }
                if (damageValue < 1)
                {
                    damageValue = 1;
                }
                if (range < 1)
                {
                    range = 1;
                }
                if (rateOfFire < 1)
                {
                    rateOfFire = 1;
                }


                isValid = IsValueAccurate(range, damageValue, rateOfFire);
                //isValid = CheckIfNew(damageValue, range, rateOfFire);

                if(isValid == true)
                {
                    repeatCounterDamage = 1;
                    repeatCounterRange++;
                    repeatCounterRate = 1;

                    //StoreInArrays(damageValue, range, rateOfFire);
                }
                
            }
            else if (focus == "rate")
            {
                damageValue = DamageValueStart - (valueShiftDamage * DmgAdjustRate);
                range = RangeValueStart - (RngAdjustRate * valueShiftRange);
                rateOfFire = RateOfFireValueStart + (RatAdjustRate * valueShiftRate);

                if (damageValue > MaxValDamage)
                {
                    damageValue = MaxValDamage;
                }
                if (range > MaxRangeVal)
                {
                    range = MaxRangeVal;
                }
                if (rateOfFire > BaseAdjust(focus))
                {
                    rateOfFire = BaseAdjust(focus); 
                }
                if (damageValue < 1)
                {
                    damageValue = 1;
                }
                if (range < 1)
                {
                    range = 1;
                }
                if (rateOfFire < 1)
                {
                    rateOfFire = 1;
                }




                isValid = IsValueAccurate(rateOfFire, damageValue, range);
                //isValid = CheckIfNew(damageValue, range, rateOfFire);

                if (isValid == true)
                {
                    repeatCounterDamage = 1;
                    repeatCounterRange = 1;
                    repeatCounterRate++;

                    //StoreInArrays(damageValue, range, rateOfFire);
                }
                
            }            
        } while (isValid == false);



    }






    //Cause a slight decay in the increase of the last selected tower type, and an increase in the others
    

    bool IsValueAccurate(float mainStat, float subStat1, float subStat2)
    {
        //Perform a check that that the main stat is not smaller then the sub-stats.
        if(mainStat < subStat1 || mainStat < subStat2)
        {
            return false;
        }
        
        
        //Loop through the array, and make sure there are no repeats


        return true;
    }

    bool CheckIfNew(float damage, float range, float rate)
    {
        int ArrayLen = damageArray.Length;
        for (int i = 0; i < ArrayLen; i++ )
        {
            if (damage == damageArray[i] && range == rangeArray[i] && rate == rateArray[i])
            {
                return false;
            }
            
        }
        return true;
    }

    //measured for novilty. 
    void StoreInArrays(float D, float R, float RoF)
    {
        if(damageArray.Length == 0)
        {
            damageArray[0] = D;
            rangeArray[0] = R;
            rateArray[0] = RoF;
        }
        else
        {
            damageArray[damageArray.Length+1] = D;
            rangeArray[rangeArray.Length+1] = R;
            rateArray[rateArray.Length+1] = RoF;
        }
    }







    // Path and data to save results in:
    private string directoryPath = @"C:\Users\Owner\Documents\Unity projects\Tower Defence AI\Assets\Results";
    private string fileName = "results.txt";
    private string filePath;




    public void FailureTower()
    {
        firstTowerGoldLimit = (int)firstTowerGoldLimit - 10;
        if (firstTowerGoldLimit < 10)
        {
            firstTowerGoldLimit = 10;
        }
    }




    //Evolve the tower:
    public void TowerImprovementAlgorithm(float Kills, float Time, float EnemiesPassed, float TotalTime, string AreaOfFocus, float RoF, float Rng, float Dmg, int Gld, float ShotsFired, string TowerIdentifier)
    {
        if ((TotalTime - Time) < 60)
        {
            if (EnemiesPassed > 5)
            {
                firstTowerGoldLimit = (int)firstTowerGoldLimit - (int)(10*EnemiesPassed);
                if (firstTowerGoldLimit < 10)
                {
                    firstTowerGoldLimit = 10;
                }
            }
            else
            {
                firstTowerGoldLimit = (int)firstTowerGoldLimit + Mathf.RoundToInt(Kills/Time);
            }
            //If the tower isn't getting a kill every 5 seconds, 
            if (Kills/Time < 5) 
            {
                ValUpdate(KillRateCalc(Kills, Time), efficiencyCalc(Kills, EnemiesPassed), AreaOfFocus, BulletEfficiency(Kills, ShotsFired), ShotsOverLife(ShotsFired, Time));
                /*
                if (AreaOfFocus == "damage") 
                {
                    damageIncrease = damageIncrease + 10;
                    rangeIncrease = rangeIncrease + 5;
                    rateIncrease = rateIncrease + 5;
                }

                if (AreaOfFocus == "range")
                {
                    damageIncrease = damageIncrease + 5;
                    rangeIncrease = rangeIncrease + 10;
                    rateIncrease = rateIncrease + 5;
                }

                if (AreaOfFocus == "rate")
                {
                    damageIncrease = damageIncrease + 5;
                    rangeIncrease = rangeIncrease + 5;
                    rateIncrease = rateIncrease + 10;
                }
                */
            }
            // This tower is doing very well
            else if (Kills/Time > 0.8)
            {
                GoldBuy = Gld;
                damageValue = Dmg;
                range = Rng;
                rateOfFire = RoF;
            }
        }

        else if (Kills / Time > 0.95 || EnemiesPassed/Kills > 0.9)
        {
            GoldBuy = Gld;
            damageValue = Dmg;
            range = Rng;
            rateOfFire = RoF;
        }

        else
        {
            ValUpdate(KillRateCalc(Kills, Time), efficiencyCalc(Kills, EnemiesPassed), AreaOfFocus, BulletEfficiency(Kills, ShotsFired), ShotsOverLife(ShotsFired, Time));
            //Call the value to update







            /*
            if (Kills < 5)
            {
                if (AreaOfFocus == "damage")
                {
                    damageIncrease = damageIncrease + 5;
                    rateDecrease = rateDecrease - 5;
                    rangeDecrease = rangeDecrease - 5;
                }

                if (AreaOfFocus == "range")
                {
                    damageDecrease = damageDecrease - 5;
                    rangeIncrease = rangeIncrease + 5;
                    rateDecrease -= 5;
                }

                if (AreaOfFocus == "rate")
                {
                    damageDecrease -= 5;
                    rangeDecrease -= 5;
                    rateIncrease += 5;
                }
            }
            else
            {
                damageIncrease += Random.Range(-5, 6);
                rangeIncrease += Random.Range(-5, 6);
                rateIncrease += Random.Range(-5, 6);
            }
            */

        }
        AdaptRules(Kills, Time, EnemiesPassed, TotalTime, AreaOfFocus, RoF, Rng, Dmg, Gld, ShotsFired,  TowerIdentifier);
    }












    //Save data
    public void AdaptRules(float Kills, float Time, float EnemiesPassed, float TotalTime, string AreaOfFocus, float RoF, float Rng, float Dmg, int Gld, float ShotsFired, string TowerIdentifier)
    {
        //Detect score based on how many kills and time spent on field
        float KpS = Kills/Time; //Finds how many enemies are killed per second
        float TimeTillGameOver = Time; //The time the tower stayed present in the game scene
        float TimeBeforeSpawn = TotalTime - Time; //Time before spawn.
         //Now to create the text file and pass the variables:
        filePath = Path.Combine(directoryPath, fileName);
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
        AddValuesToFile(Kills, Time, EnemiesPassed, TotalTime, AreaOfFocus, RoF, Rng, Dmg, Gld, ShotsFired,  TowerIdentifier);



    }


    float Decay(float valToDecay, int decayIncrease)
    {
        valToDecay = valToDecay - (Random.Range(0f, 1f) * decayIncrease);
        return valToDecay;
    }








    //Return the range, rate, and damage.

    public float RangeReturn()
    {
        float rangeToSend = range / 10;
        if(rangeToSend < 1f)
        {
            Debug.Log("Range Too Low");
            return 1; //Replace with special effect tower depending on other attributes.
        }
        return rangeToSend;
    } 

    public float RateReturn()
    {
        return  100/ rateOfFire;
    }
    public float DamageReturn() { return damageValue/100; }
    public float GldReturn() { return GoldBuy; }













    //Handles the adjustment of gold. 
    //Generate the price
    public void GoldPrice()
    {
        int temp = Random.Range(1, 10) * 10;
        GoldBuy = temp;
        GoldAdjustmentRate();
    }
    public void GoldPriceFirst()
    {
        if(GoldBuy > firstTowerGoldLimit)
        {
            GoldBuy = firstTowerGoldLimit;
        }
        GoldAdjustmentRate();
    }
    //Adjust the rate for the price
    void GoldAdjustmentRate()
    {
        float result = GoldBuy / 10f;
        int resultRound = Mathf.RoundToInt(result);
        Goldadjustment = (float)resultRound*10;
    }


    //Adjust the base values for the price
    float BaseAdjust(string type)
    {
        float MaxValDmg = 0;
        float MaxValRng = 0;
        float MaxValRat = 0;

        switch(type)
        {
            case "damage":
                MaxValDmg = MaxValDamage + Goldadjustment; 
                return MaxValDmg; break;
            case "range":
                MaxValRng = MaxRangeVal + Goldadjustment; 
                return MaxValRng; break;
            case "rate":
                MaxValRat = MaxRateVal + Goldadjustment; 
                return MaxValRat; break;
        }
        return 0;
    }





    //Functions for adding values to files:
    void CreateFile()
    {
        // Create a new file and close it immediately
        File.Create(filePath).Close();
        Debug.Log("File created at: " + filePath);
    }

    void AddValuesToFile(float Kills, float Time, float EnemiesPassed, float TotalTime, string AreaOfFocus, float RoF, float Rng, float Dmg, int Gld, float ShotsFired, string TowerIdentifier)
    {
        // Create the content to write
        string content = $"ID = {TowerIdentifier}\nPath = {AreaOfFocus}\nRate of Fire = {RoF}\nRange = {Rng}\nDamage = {Dmg}\nPrice = {Gld}\nTotal Kills = {Kills}\nTime = {Time}\nTime Total = {TotalTime}\nShots fired = {ShotsFired}\nEnemies entered range = {EnemiesPassed}\n___________________________\n\n";

        // Write the content to the file
        //File.WriteAllText(filePath, content);
        File.AppendAllText(filePath, content);
        Debug.Log("Values added to file: " + filePath);
    }




    public void ID_Increment()
    {
        Debug.Log("LoopNo = " + LoopNo);
        LoopNo = LoopNo + 1;
        ID = 0;
    }



    public void stringBuilder()
    {
        if (LoopNo < 100 && LoopNo > 9) 
        {
            if (ID < 10)
            {
                UID = "0" + (LoopNo.ToString()) + "0" + (ID.ToString());
                ID++;
            }
            else
            {
                UID = "0" + (LoopNo.ToString()) + (ID.ToString());
                ID++;
            }
        }
        if (LoopNo < 100 && LoopNo < 10)
        {
            if (ID < 10)
            {
                UID = "00" + (LoopNo.ToString()) + "0" + (ID.ToString());
                ID++;
            }
            else
            {
                UID = "00" + (LoopNo.ToString()) + (ID.ToString());
                ID++;
            }
        }
        if (LoopNo > 99)
        {
            if (ID < 10)
            {
                UID = (LoopNo.ToString()) + "0" + (ID.ToString());
                ID++;
            }
            else
            {
                UID = (LoopNo.ToString()) + (ID.ToString());
                ID++;
            }
        }

    }
















    //Get the kills per second
    public float KillRateCalc(float kills, float time)
    {
        if (kills > 0)
            return kills/time;
        else return 1;
    }
    //Get the efficiency of the tower
    public float efficiencyCalc(float kills, float enemiesPassed)
    {
        if(kills >= 1)
            return kills/ enemiesPassed;
        else return 1;
    }
    //get how many shots it takes on average to get 1 kill.
    public float BulletEfficiency(float kills, float ShotsFired)
    {
        if (kills >= 1 && ShotsFired >= 1)
            return ShotsFired/kills;
        else return 1;
    }
    //get shots per second
    public float ShotsOverLife(float ShotsFired, float time)
    {
        if (ShotsFired >= 1)
            return ShotsFired/time;
        else return 1;
    }

    public void ValUpdate(float killRate, float Efficiency, string type, float bulletEff, float shotsPerSecond)
    {
        switch (type)
        {
            case "damage":
                //Low shots fired and high kills:
                /*
                if (shotsPerSecond > killRate)
                {
                    float ValToUse = (DmgAdjustDamage / killRate);
                    DmgAdjustDamage = ValToUse / Efficiency;
                    RatAdjustDamage = RatAdjustDamage - RatAdjustDamage*(killRate - shotsPerSecond);
                    break;
                }
                else
                {
                    DmgAdjustDamage = (DmgAdjustDamage/killRate);
                    DmgAdjustDamage = ValToUse/Efficiency;
                    RatAdjustDamage = RatAdjustDamage/killRate;
                    RatAdjustDamage = RatAdjustDamage/Efficiency;
                }
                */
                if (Efficiency < 0.8)
                {
                    ID = 0;
                    //Debug.Log("Improving");
                    //Debug.Log("Time to change:" + DmgAdjustDamage + " " + killRate + " " + Efficiency);
                    DmgAdjustDamage = (DmgAdjustDamage + killRate);
                    //Debug.Log("Time to change: first Result: " + DmgAdjustDamage);
                    DmgAdjustDamage = DmgAdjustDamage / Efficiency;
                    //Debug.Log("Time to change: second Result: " + DmgAdjustDamage);
                    //DmgAdjustDamage = DmgAdjustDamage / (1/bulletEff);
                    //RatAdjustDamage = RatAdjustDamage / killRate;
                    RatAdjustDamage = RatAdjustDamage - Efficiency;
                    DmgAdjustDamage = DmgAdjustDamage + (1 - MinorAdj(killRate, shotsPerSecond));

                    DmgAdjustDamage = DmgAdjustDamage + Random.Range(-0.5f, 2f);
                    RatAdjustDamage = RatAdjustDamage + Random.Range(-1f, 2f);
                }
                break;




            case "range":
                ID = 0;
                RngAdjustRange = RngAdjustRange/Efficiency;
                //Adjust to increase range to maximize number of enemies.
                break;




            case "rate":
                if ((Efficiency < 0.8))
                {
                    Debug.Log("Improving Rate: "+ RatAdjustRate);
                    RatAdjustRate = RatAdjustRate + shotsPerSecond;
                    RatAdjustRate = RatAdjustRate / Efficiency;
                    RatAdjustRate += 100;
                    //DmgAdjustRate = DmgAdjustRate/shotsPerSecond;
                    DmgAdjustRate = DmgAdjustRate - Efficiency;
                    //RatAdjustRate = RatAdjustRate + MinorAdj(killRate, shotsPerSecond);

                    DmgAdjustRate = DmgAdjustRate + Random.Range(-1f, 2f);
                    RatAdjustRate = RatAdjustRate + Random.Range(0f, 2f);
                }
                ID = 0;
                //Increase rate of fire, and decrease damage accordingly
                

                break;




        }
    }

    private float MinorAdj(float killRate, float shotsPerSecond)
    {
        // Divide the kills per second by the shots per second.
        return killRate / shotsPerSecond;
        // If they are equal, it will be close to 1. 


    }
}





/*
 * Rules:
 * 
 * A tower is made up of 3 parts, highlighted by the 3 identified factors 
 * These factors also effect the appearance of the tower
 * 
 * The tower will start with 3 hard focuses, and these will adjust over time. 
 *  Example:
 *      At the start, focus on offense will increase damage by a lot, and the other 2 options will fall conversly.
 *      If the player chooses to balance this more, they will adjust accordingly.
 *  The towers will be tested for novilty while remaining within the bounds of their rules.
 *  
 *  Towers will also be tested on effectiveness (A balloon passing them in a decrease.)
 *  
 *  The algorthm of tower creation will evolve over time.
 * 
 * 
 */
