using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cangrejo : MonoBehaviour
{

    private bool playerSeen;
    //Whether krab has seen player or not this frame
    private Vector3 target;
    //Following target if krab is going to move.

    private Transform playerTransform;
    //Reference to the player Transform

    [SerializeField]
    private float lastTimePlayerSeen;
    //Time between this frame and last time the krab has seen the player

    [SerializeField]
    private float life;
    //life of this krab

    [SerializeField]
    private int nearBugs;

    [SerializeField]
    private GameObject bulletObj;

    private enum Action {
        SEARCH, SHOOT, BACK, WAIT
    }

    private Action nextAction; //Next action to do

    private float lastDecision = 0.0f; //time between the last decision was taken and now

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        life = 100.0f;
        nearBugs = 0;
        lastTimePlayerSeen = 5.0f;
        playerSeen = false;
    }

    // Update is called once per frame
    void Update()
    {
        lastDecision += Time.deltaTime;
        lastTimePlayerSeen += Time.deltaTime;
        if (lastDecision >= 1.0){
            Decision();
            lastDecision = 0.0f;
        }

    }

    bool Raycast () {
        Vector3 forwardVector = transform.TransformDirection(Vector3.forward);
        RaycastHit choque = new RaycastHit();
        
        if (Physics.Raycast(transform.position, forwardVector, out choque, 200f)){
            Debug.DrawRay(transform.position, forwardVector * choque.distance, Color.red);
            return (choque.collider.gameObject.tag == "Player");
        } else {
            return false;
        }
    }

    void Decision () {
        /*
        SENSORS:
            - Player seen in this frame
            - Life (>= 25 or <25)
            - Number of near bugs (>=3 or <3)
            - Last time player was seen (>5.0s or <= 5.0s)
        */
        playerSeen = Raycast();

        if (playerSeen){
            lastTimePlayerSeen = 0.0f;
        }

        //Numero de bichillos SE HACE CON COLISIONES uwu

        /*
        ACTIONS:
            - Shoot the player straight away (SHOOT)
            - Go back and restore some health (BACK)
            - Actively search the player (SEARCH)
            - Wait for the player or for bugs (WAIT)
        */
        if (lastTimePlayerSeen > 5.0f && nearBugs >= 3){
            nextAction = Action.SEARCH;
        } else if (lastTimePlayerSeen > 5.0f && nearBugs < 3){
            nextAction = Action.WAIT;
        } else if (playerSeen && nearBugs >= 3){
            nextAction = Action.SHOOT;
        } else if (playerSeen && nearBugs < 3 && life < 25){
            nextAction = Action.BACK;
        } else if (playerSeen && nearBugs < 3 && life >= 25){
            nextAction = Action.SHOOT;
        } else if (lastTimePlayerSeen <= 5.0f && nearBugs >= 3){
            nextAction = Action.SHOOT;
        } else if (lastTimePlayerSeen <= 5.0f && life < 25 && nearBugs < 3){
            nextAction = Action.BACK;
        } else if (lastTimePlayerSeen <= 5.0f && life >= 25 && nearBugs < 3){
            nextAction = Action.SHOOT;
        }

        /*
        EXECUTORS
        */

        switch (nextAction){
            case Action.WAIT:
                Debug.Log("waiting");
                //Do nothing
            break;
            case Action.SHOOT:
                Shoot();
            break;
            case Action.BACK:
                Back();
            break;
            case Action.SEARCH:
                Search();
            break;
        }
    }

    void Shoot () {
        RaycastHit hit;

        Vector3 direct = target - transform.position;
        if(Physics.Raycast(transform.position, direct,out hit,100f)){

        }
        
        Debug.Log("Die! Die! Dieeee!");
        Instantiate(bulletObj, transform.position, transform.rotation);
    }

    void Back () {
        Debug.Log("I go back!");

    }

    void Search () {
        Debug.Log("Searching for some players to kill...");
    }
}
