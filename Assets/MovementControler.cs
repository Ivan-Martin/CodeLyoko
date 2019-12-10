using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControler : MonoBehaviour
{
    CharacterController characterController;
    public float movementSpeed = 500.0f;
    private Vector3 moveDirection = Vector3.zero;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    void Update () {
        
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection = moveDirection * movementSpeed;
        
        moveDirection.y -= 1000f * Time.deltaTime;
        //Gravity
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
