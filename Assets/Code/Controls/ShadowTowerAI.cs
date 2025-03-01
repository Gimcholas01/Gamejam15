using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowTowerAI : BaseObject
{
    // simulate gold etc...
    public int gold;
    private int income;
    private int minionCost;

    private bool canSpawnMelee = true;
    private float CD;

    public GameObject baseMinion;
    [SerializeField]
    private int percentageChanceToSpawnSpecial = 10;
    public GameObject[] specialMinions;

    void Start() {

        GameMan.Shadow.Tower = this;
        
        // copy player stats

        // Inventory playerInventory = GameMan.GetPlayerInventory();
        // gold = playerInventory.gold;
        // income = playerInventory.GetIncome();

        Tower_Spawner playerTower = GameObject.Find("Player Tower").GetComponent<Tower_Spawner>();
        minionCost = playerTower.minionCost;
        CD = playerTower.CD;

        // scale with level
        percentageChanceToSpawnSpecial *= GameMan.currentLevel;
        gold = GameMan.currentLevel * 10;
        // income = GameMan.currentLevel; // scaling with seconds is kinda OP
        income = 2; // if income is 3 then the game is literally un-winnable

        StartCoroutine(IncomeOverTime());

    }

    void Update() {
        if (canSpawnMelee && gold > minionCost && spawnAreaClear){
            
            StartCoroutine(MeleeCountdown());

            GameObject newUnit;
            var randomNumber = Random.Range(0, 100);
            if (randomNumber < percentageChanceToSpawnSpecial)
            {
                // Remainder of random number / amount of special minions is sorta random index
                var theUnitToSpawn = specialMinions[randomNumber % specialMinions.Length];
                newUnit = Instantiate(specialMinions[randomNumber % specialMinions.Length], GameMan.CalculateSpawnPosition(gameObject.transform, theUnitToSpawn), Quaternion.identity);
            }
            else
            {
                newUnit = Instantiate(baseMinion, GameMan.CalculateSpawnPosition(this.gameObject.transform, baseMinion), Quaternion.identity);
            }
            GameMan.Shadow.AddUnit(newUnit.GetComponent<BaseMovingUnit>());
            gold -= minionCost;
        }
        spawnAreaClear = true;
    }

    IEnumerator IncomeOverTime(){
        while(true){
        gold+=income;
        yield return new WaitForSeconds(1);
        }
    }

    // countdown
    IEnumerator MeleeCountdown(){
        canSpawnMelee = false;
        yield return new WaitForSeconds(CD);
        canSpawnMelee = true;
    }

    protected override void HandleDestruction()
    {
        // play sounds
        // play animations
        // instantiate vfx
        // destroy this unit at the very end
        GameMan.TowerDestroyed(thisUnitSide);
        Object.Destroy(this.gameObject);
    }

    private bool spawnAreaClear = true;

    public void OnTriggerStay2D(Collider2D collision)
    {
        // Make sure it's a unit blocking the exit
        if (collision.GetComponent<BaseUnit>() != null && collision.GetComponent<BaseUnit>().thisUnitSide == thisUnitSide)
        {
            spawnAreaClear = false;
        }
    }
}
