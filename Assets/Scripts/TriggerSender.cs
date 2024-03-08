using UnityEngine;

public class TriggerSender : MonoBehaviour
{
    [SerializeField] private LevelStateMachine levelStateMachine; 
    
    private void OnTriggerEnter(Collider other)
    {
        levelStateMachine.TownSquareMemory();
    }
}
