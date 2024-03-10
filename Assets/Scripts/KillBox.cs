using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using PlayerScript;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    
    // TODO this will literally destroy the player
    private void OnTriggerEnter(Collider other)
    {
        player.BulletRecall();
        
        Destroy(other);
        Debug.Log(other + " has been destroyed due to ooob");
        return;
    }
}
