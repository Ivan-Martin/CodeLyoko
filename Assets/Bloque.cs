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

    private float life = 100.0f;

    private Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        
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

    bool Raycast (float distance) {
        Vector3 forwardVector = transform.TransformDirection(Vector3.forward);
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

    void Decision () {
        /*
        CHANGE STATES ACCORDING TO STATE MACHINE
        */
        switch (myState){
            case State.DEFEND:
                if (life < 50 && Raycast(300f)){
                    this.myState = State.FREEZE;
                } else if (Raycast(300f)){
                    this.myState = State.SHOOT;
                }
            break;
            case State.FREEZE:
                if (cooldown < 5){
                    cooldown++;
                } else {
                    cooldown = 0;
                    this.myState = State.DEFEND;
                }
            break;
            case State.SHOOT:
                if (!Raycast(200f)){
                    this.myState = State.SEARCH;
                }
            break;
            case State.SEARCH:
                if (cooldown >= 5){
                    cooldown = 0;
                    this.myState = State.DEFEND;
                } else {
                    if (Raycast(200)){
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
                target = spawnPoint;
                Move();
            break;
            case State.SHOOT:
                Shoot();
            break;
            case State.SEARCH:
                Move();
            break;
            case State.FREEZE:
                ShootFreeze();
            break;
        }


    }
}
