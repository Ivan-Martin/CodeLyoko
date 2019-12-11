using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloque : MonoBehaviour
{

    private Vector3 target;
    //Following target if block is going to move.

    private float lastDecision = 0.0f;

    private float lastTimePlayerSeen = 0.0f;

    private enum State {
        FREEZE, SHOOT, DEFEND, SEARCH
    }

    private State myState;

    private int cooldown;

    [SerializeField]
    //private float life = 100.0f;
    private GameObject spawnPoint;

    public float speed = 100.0f;

    public float shortDistance = 200f;
    public float longDistance = 300f;

    public GameObject bulletObj;

    private UnityEngine.AI.NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        myState = State.DEFEND;
    }

    // Update is called once per frame
    void Update()
    {
        if (myState == State.DEFEND || myState == State.SEARCH){
            Move();
        }
        lastDecision += Time.deltaTime;
        lastTimePlayerSeen += Time.deltaTime;
        if (lastDecision >= 1.0){
            Decision();
            lastDecision = 0.0f;
        }
    }

    bool Raycast (float distance, Vector3 direction) {
        Vector3 forwardVector = transform.TransformDirection(direction);
        RaycastHit choque = new RaycastHit();
        
        if (Physics.Raycast(transform.position, forwardVector, out choque, distance)){
            Debug.DrawRay(transform.position, forwardVector * choque.distance, Color.red);
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

    bool Raycast(float distance){
        bool found = false;
        Vector3 [] vectores = new Vector3 [4];
        vectores[0] = Vector3.forward;
        vectores[1] = Vector3.left;
        vectores[2] = Vector3.back;
        vectores[3] = Vector3.right;
        for (int i = 0; i < 4; i++){
            found = found || Raycast(distance, vectores[i]);
        }
        return found;
    }

    void Decision () {
        /*
        CHANGE STATES ACCORDING TO STATE MACHINE
        */
        switch (myState){
            case State.DEFEND:
                //When defending, don't move away from tower. Wait the player to approach block
                if (gameObject.GetComponent<Life>().life < 50 && Raycast(longDistance)){
                    this.myState = State.FREEZE;
                } else if (Raycast(longDistance)){
                    this.myState = State.SHOOT;
                }
            break;
            case State.FREEZE:
                //Shoot a freeze shot has a cooldown of 5 seconds. Then proceed to defend
                if (cooldown < 5){
                    cooldown++;
                } else {
                    cooldown = 0;
                    this.myState = State.DEFEND;
                }
            break;
            case State.SHOOT:
                //When Block shoots normal, continue shooting until you can't see the player
                //Then, search it
                if (!Raycast(shortDistance)){
                    this.myState = State.SEARCH;
                }
            break;
            case State.SEARCH:
                //Search the player for a maximum of 5 seconds and shoot. If not found, defend Tower
                if (cooldown >= 5){
                    cooldown = 0;
                    this.myState = State.DEFEND;
                } else {
                    if (Raycast(shortDistance)){
                        this.myState = State.SHOOT;
                        cooldown = 0;
                    } else {
                        cooldown++;
                    }
                }
            break;
        }

        /*
        EXECUTE ACTION ACCORDING TO ACTUAL STATE
        */

        switch (myState){
            case State.DEFEND:
                target = spawnPoint.transform.position;
                Move();
            break;
            case State.SHOOT:
                //Shoot();
            break;
            case State.SEARCH:
                Move();
            break;
            case State.FREEZE:
                //ShootFreeze();
            break;
        }


    }

    void Move() {
        //transform.position = Vector3.MoveTowards(transform.position, target, step);
        agent.SetDestination(target);
        agent.speed = speed;
    }

    void Shoot () {
        genericShoot();
        agent.SetDestination(transform.position);
    }

    void ShootFreeze (){
        genericShoot();
        bulletObj.GetComponent<Bullet>().freeze = true;
        agent.SetDestination(transform.position);
        Debug.Log("Shooteando congelasio");
    }

    void genericShoot(){
        Vector3 direct = target - transform.position;
        Debug.Log("Die! Die! Dieeee!");
        Instantiate(bulletObj, transform.position + new Vector3(0,0,-4), transform.rotation);
        bulletObj.GetComponent<Bullet>().direction = direct;
    }
}
