using System.Collections;
using System.Collections.Generic;
using LevelStateMachines;
using UnityEngine;

public class BedroomDoor : MonoBehaviour
{
    [SerializeField] private LevelStateMachine_Home levelStateMachineHome;
    [SerializeField] private EndingChant endingChant;
    [SerializeField] private GameObject subDoorLight;
    [SerializeField] private GameObject frontDoorLight;
    private static readonly int IsBroken = Animator.StringToHash("IsBroken");

    // good old copypasting scripts that should be interfaces, good job simon
    
    /// <summary>
    /// play destruction animation and sounds
    /// note that this does not actually destroy the object (and it shoudn't)
    /// </summary>
    public void DestroyAnimation()
    {
        frontDoorLight.SetActive(true);
        subDoorLight.SetActive(false);
        endingChant.StopChant();
        GetComponent<Animator>().SetBool(IsBroken, true);
        levelStateMachineHome.CallEndQuip();
    }
}
