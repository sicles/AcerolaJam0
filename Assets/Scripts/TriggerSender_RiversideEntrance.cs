using UnityEngine;

public class TriggerSender_RiversideEntrance : MonoBehaviour
{
    [SerializeField] private LevelStateMachine levelStateMachine; 
    
    private void OnTriggerEnter(Collider other)
    {
        levelStateMachine.RiversideEntranceMemory();
    }
}

