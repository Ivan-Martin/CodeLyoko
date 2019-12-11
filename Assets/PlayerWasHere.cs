using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWasHere : MonoBehaviour
{
    public bool playerHasPassed = false;

    void OnTriggerEnter(Collider collider){
        if(collider.tag == "Player"){
            playerHasPassed = true;
        }
    }
}
