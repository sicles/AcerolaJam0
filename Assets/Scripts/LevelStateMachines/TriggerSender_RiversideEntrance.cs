using UnityEngine;
using UnityEngine.Serialization;

namespace LevelStateMachines
{
    // ReSharper disable once InconsistentNaming
    public class TriggerSender_RiversideEntrance : MonoBehaviour
    {
        [FormerlySerializedAs("levelStateMachine")] [SerializeField] private LevelStateMachine_Paris levelStateMachineParis; 
    
        private void OnTriggerEnter(Collider other)
        {
            levelStateMachineParis.RiversideEntranceMemory();
        }
    }
}

