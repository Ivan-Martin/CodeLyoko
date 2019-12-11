using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class XANA : MonoBehaviour
{

    private MovementControler player;
    private GameObject [] spawnPoints;

    private int [] timesPassed;

    private GameObject [] enemies;
    // Start is called before the first frame update

    private GameObject tower;

    private float PlayerDificulty;

    private float cooldown = 5f;

    private System.Random random;

    public Cangrejo krab;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementControler>();

        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");

        for (int i = 0; i < spawnPoints.Length; i++){
            PlayerPrefs.GetInt("SpawnPoint" + i, 0);
        }

        tower = GameObject.FindGameObjectWithTag("Tower");

        random = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown <= 0){
            CalculateDificulty();
            cooldown = UnityEngine.Random.Range(30f, 30+PlayerDificulty*20);
            //cooldown = 5f;
            TakeDecision();
        } else {
            cooldown -= Time.deltaTime;
        }
        
    }

    void CalculateDificulty() {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float distanceToTower = Vector3.Distance(player.transform.position, tower.transform.position);
        float towerDif = 1 - (distanceToTower/500);
        
        int enemyNumber = enemies.Length;
        int totalLife = 0;
        for (int i = 0; i < enemyNumber; i++){
            totalLife += enemies[i].GetComponent<Life>().life;
        }

        float enemyDif = totalLife/(100*enemyNumber);

        float enemyNumberDif = 1 - (enemyNumber/10);

        float playerLife = 1 - (player.life/100);

        PlayerDificulty = (towerDif + enemyDif + playerLife + enemyNumberDif) / 4;
        
    }

    void TakeDecision () {
        float n1 = (float) random.NextDouble();
        float n2 = (float) random.NextDouble();

        Debug.Log(n1 + " " + n2);

        if (n1 >= PlayerDificulty && n2 >= PlayerDificulty){
            
            GenerateEnemy();
            Debug.Log("Genero bicho");
        } else {
            Debug.Log("No genero bicho");
        }
    }

    void GenerateEnemy() {

        //MEMORY
        //CHOOSES RANDOM SPAWN POINT WHILE TAKING INTO ACCOUNT WHERE THE PLAYER HAS PASSED
        int [] passed = new int [spawnPoints.Length];
        int total = 0;
        for (int i = 0; i < spawnPoints.Length; i++){
            bool holi = spawnPoints[i].GetComponent<PlayerWasHere>().playerHasPassed;
            if (!holi) PlayerPrefs.SetInt("SpawnPoint" + i, PlayerPrefs.GetInt("SpawnPoint" + i, 0));
            else PlayerPrefs.SetInt("SpawnPoint" + i, PlayerPrefs.GetInt("SpawnPoint" + i, 0)+1);
            passed[i] = PlayerPrefs.GetInt("SpawnPoint" + i, 0)+1;
            total += passed[i];
        }
        float [] passedFloats = new float[passed.Length];
        for (int i = 0; i < passedFloats.Length; i++){
            passedFloats[i] = total / passed[i];
        }

        float [] accumulative = new float [passed.Length];
        accumulative[0] = passedFloats[0];
        for (int i = 1; i < accumulative.Length; i++){
            accumulative[i] = accumulative[i-1] + passedFloats[i];
        }

        float spawnPointelected = (float) random.NextDouble();

        int selected = 0;
        while (accumulative[selected] < spawnPointelected && selected < accumulative.Length-1){
            selected++;
        }
        GameObject mySpawn = spawnPoints[selected];

        Instantiate(krab, mySpawn.transform.position, mySpawn.transform.rotation);
    }
}
