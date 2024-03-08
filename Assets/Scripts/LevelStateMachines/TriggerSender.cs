using UnityEngine;
using UnityEngine.Serialization;

namespace LevelStateMachines
{
    public class TriggerSender : MonoBehaviour
    {
        [FormerlySerializedAs("levelStateMachine")] [SerializeField] private LevelStateMachine_Paris levelStateMachineParis; 
    
        private void OnTriggerEnter(Collider other)
        {
            levelStateMachineParis.TownSquareMemory();
        }
    }
}
