using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

public class EnemyHitboxPart : MonoBehaviour
{
    [SerializeField] private PrototypeAI parentAI;

    // the things you have to do when unity's meshcollider stops working
    
    public void HasBeenHit(int damage, Vector3 position, Vector3 rotation, Vector3 bulletDirection)
    {
        parentAI.TakeDamage(damage, position, rotation, bulletDirection);
    }
}
