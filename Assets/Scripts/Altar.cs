using System.Collections.Generic;
using LevelStateMachines;
using PlayerScript;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LevelStateMachine_Youth _levelStateMachineYouth;
    [SerializeField] private GameObject detachedGun;
    [SerializeField] private List<Light> altarLight;
    [SerializeField] private List<GameObject> uiElementsToActivate;
    private bool _hasBeenTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasBeenTriggered) return;
        
        _hasBeenTriggered = true;
        
        playerController.ActivateGun(true);
        detachedGun.SetActive(false);

        // foreach (var lighty in altarLight)
        // {
        //     lighty.gameObject.SetActive(false);
        // }

        foreach (var element in uiElementsToActivate)
        {
            element.SetActive(true);
        }
        
        // TODO add an animation where you grab the gun and it looks very very cool
        
        // start tutorial 
        _levelStateMachineYouth.CallGunTutorial();
    }
}
