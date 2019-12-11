using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bicho : MonoBehaviour
{

    public int group; //Number of bugs in this group

    private bool krab; //Its group has a krab or not

    private bool shooting;

    private bool chargedShooting;

    private float lastDecision = 1.0f;

    public GameObject bulletObj;

    private Vector3 target;

    public GameObject krabGO;

    public GameObject [] bugs;

    public Vector3 totalMovement = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.position;
        shooting = false;
        group = 4;
        krab = false;
        chargedShooting = false;
    }

    // Update is called once per frame
    void Update()
    {
        lastDecision -= Time.deltaTime;
        if (lastDecision <= 0.0){
            lastDecision = 1.0f;
            Action();
            
        }
        GetComponent<Rigidbody>().velocity = totalMovement * Time.deltaTime;
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

    void Action () {

        if (group >= 4){
            if (Random.Range(0, 10) <= 1){
                chargedShooting = true;
            }
        }
        if (Raycast()){
            if (chargedShooting){
                ChargedShoot();
            } else {
                Shoot();
            }
        }

        totalMovement = Vector3.zero;

        if (krab){
            totalMovement += krabGO.transform.forward;
        }
        if (!shooting){
            totalMovement += target;
        }

        //totalMovement += Flocking();

        totalMovement /= 20;

        

    }

    void ChargedShoot() {
        Vector3 direct = target - transform.position;
        Debug.Log("Die! Die! Dieeee!");
        Instantiate(bulletObj, transform.position + new Vector3(0,0,-4), transform.rotation);
        bulletObj.GetComponent<Bullet>().direction = direct;
        bulletObj.GetComponent<Bullet>().charged = true;
    }

    void Shoot () {
        Vector3 direct = target - transform.position;
        Debug.Log("Shoot bicho");
        Instantiate(bulletObj, transform.position + new Vector3(0,0,-8), transform.rotation);
        bulletObj.GetComponent<Bullet>().direction = direct;
    }

    Vector3 Flocking () {
        Vector3 theCenter = Vector3.zero;
        Vector3 theVelocity = Vector3.zero;
 
        foreach (GameObject boid in bugs)
        {
            theCenter = theCenter + boid.transform.localPosition;
            theVelocity = theVelocity + boid.GetComponent<Rigidbody>().velocity;
        }
 
        Vector3 flockCenter = theCenter/(group);
        Vector3 flockVelocity = theVelocity/(group);

        flockCenter = flockCenter - transform.localPosition;
        flockVelocity = flockVelocity - GetComponent<Rigidbody>().velocity;

        return flockCenter + flockVelocity + target*2;
    }
}
