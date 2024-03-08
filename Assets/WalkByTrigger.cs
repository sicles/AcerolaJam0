using AI;
using UnityEngine;

public class WalkByTrigger : MonoBehaviour
{
    [SerializeField] private PrototypeAI enemy;
    [SerializeField] private Transform walkToTarget; 
    
    private void OnTriggerEnter(Collider other)
    {
        enemy.CallWalkTo(walkToTarget.position, true);
    }
}
