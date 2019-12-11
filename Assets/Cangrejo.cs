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
    //life of this krab

    [SerializeField]
    private int nearBugs;

    [SerializeField]
    public GameObject bulletObj;

    private enum Action {
        SEARCH, SHOOT, BACK, WAIT
    }

    private Action nextAction; //Next action to do

    private float lastDecision = 0.0f; //time between the last decision was taken and now

    private bool search = false;

    private float RotationSpeed = 10;

    private bool rightBool = false;

    private Vector3 to = new Vector3 (0,180,0);

     private UnityEngine.AI.NavMeshAgent agent;

     private float speed = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        nearBugs = 0;
        lastTimePlayerSeen = 5.0f;
        playerSeen = false;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (search) {
             Rotate();
        }
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
            Debug.DrawRay(transform.position, forwardVector * choque.distance, Color.red, 1.0f);
            if((choque.collider.gameObject.tag == "Player")){
                target = choque.collider.gameObject.transform.position;
                return true;
            } else {
                return false;
            }
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
        float life = gameObject.GetComponent<Life>().life;
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
                //Do nothing
                search = true;
            break;
            case Action.SHOOT:
                search = false;
                Shoot();
            break;
            case Action.BACK:
                search = false;
                Back();
            break;
            case Action.SEARCH:
                search = true;
                Search();
            break;
        }
    }

    void Shoot () {
        Vector3 direct = target - transform.position;
        Debug.Log("Die! Die! Dieeee!");
        Instantiate(bulletObj, transform.position + new Vector3(0,0,-4), transform.rotation);
        bulletObj.GetComponent<Bullet>().direction = direct;
    }

    void Back () {
        gameObject.GetComponent<Life>().life += 10;
        target = transform.position + new Vector3 (-10, 0, -10);
        agent.SetDestination(target);
        agent.speed = speed;

    }

    void Search () {
        agent.SetDestination(target);
        agent.speed = speed;
    }

    void Rotate () {
        //Debug.Log(rightBool);
        Vector3 right = new Vector3(0, 135, 0);
        Vector3 left = new Vector3(0, 225, 0);
         if (Vector3.Distance(transform.eulerAngles, to) > 0.01f)
         {
             transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, to, Time.deltaTime);
             
         }
         else
         {
             transform.eulerAngles = to;
             if (rightBool){
                 to = left;
             } else {
                 to = right;
             }
             rightBool = !rightBool;
         }
    }
}
