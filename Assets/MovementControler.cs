using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovementControler : MonoBehaviour
{
    CharacterController characterController;
    
    public bool canMove = true;

    public GameObject bulletObj;

    private float timeBlocked = 3f;

    private float shootColdown = 1f;
    public float life = 100f;
    public float movementSpeed = 500.0f;
    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    void Update () {
        //MOVEMENT START
        if(canMove){
            
            float xMove = Input.GetAxis("Horizontal");
            float zMove = Input.GetAxis("Vertical");
            Vector3 directionZ = transform.forward * zMove;
            Vector3 directionX = transform.right * xMove;
            velocity = (directionZ + directionX) * movementSpeed;

        }else{
            timeBlocked -= Time.deltaTime;
            if(timeBlocked <=0){
                canMove = true;
                timeBlocked = 3f;
            }
        }
        //Gravity
        velocity.y -= 100f * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
        //MOVEMENT END

        //SHOOT CONTROLER START
        if(Input.GetKeyDown(KeyCode.Space)){
            if(shootColdown<=0){
                Shoot();
                shootColdown = 1f;
            }
        }
        if(shootColdown >= 0){
            shootColdown -= Time.deltaTime;
        }
        //SHOOT CONTROLER END
    }
    void Shoot(){
        Instantiate(bulletObj, transform.position + new Vector3(0,0,4f), transform.rotation);
        bulletObj.GetComponent<Bullet>().direction = transform.forward;
        bulletObj.GetComponent<Rigidbody>().velocity = transform.forward*10;

    }

    public void RemoveLife() {
        life-=5;
        if (life <= 0){
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}
