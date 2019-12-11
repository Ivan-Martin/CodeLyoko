using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 75f;
    private Rigidbody rigidbody;

    public bool freeze = false;

    public Vector3 direction; 

    public float life =5f;

    public bool launchedByPlayer = false;

    public bool charged = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * 60;
        freeze = false;
        launchedByPlayer = false;
        charged = false;
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity = direction*speed;
        if(life <= 0){
            Destroy(this);
        }else{
            life -= Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision){
        if(collision.collider.tag == "Player"){
            if(freeze){
                collision.collider.GetComponent<MovementControler>().canMove= false;
            }else if (!charged){
                collision.collider.GetComponent<MovementControler>().RemoveLife();
                
            } else {
                collision.collider.GetComponent<MovementControler>().RemoveLife();
                collision.collider.GetComponent<MovementControler>().RemoveLife();
            }
            
        } else if (collision.collider.tag == "Enemy" && !launchedByPlayer){
            collision.collider.GetComponent<Life>().life -= 10;
        }
        Destroy(this.gameObject);
    }
}