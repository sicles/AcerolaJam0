using LevelStateMachines;
using PlayerScript;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LevelStateMachine_Youth _levelStateMachineYouth;
    private bool _hasBeenTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasBeenTriggered) return;
        
        _hasBeenTriggered = true;
        
        playerController.ActivateGun(true);
        // TODO add an animation where you grab the gun and it looks very very cool
        
        // start tutorial 
        _levelStateMachineYouth.CallGunTutorial();
    }
}
