using AI;
using LevelStateMachines;
using UnityEngine;

public class DashTutorialTrigger : MonoBehaviour
{
    [SerializeField] private LevelStateMachine_Youth levelStateMachineYouth;
    private bool _hasBeenTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasBeenTriggered) return;

        _hasBeenTriggered = true;
        levelStateMachineYouth.CallDashTutorial();
    }
}
