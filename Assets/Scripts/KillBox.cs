using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using PlayerScript;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
        {
            if (other.GetComponent<PrototypeAI>() != null)    // recall bullet so it doesn't get destroyed with object
                player.BulletRecall();
            
            Destroy(other);
            Debug.Log(other + " has been destroyed due to ooob");
            return;
        }
        
        Debug.Log("Wait, you're not supposed to be down here.");
    }
}
