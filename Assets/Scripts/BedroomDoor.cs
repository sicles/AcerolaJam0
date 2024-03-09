using System.Collections;
using System.Collections.Generic;
using LevelStateMachines;
using UnityEngine;

public class BedroomDoor : MonoBehaviour
{
    [SerializeField] private LevelStateMachine_Home levelStateMachineHome;
    
    // good old copypasting scripts that should be interfaces, good job simon
    
    /// <summary>
    /// play destruction animation and sounds
    /// note that this does not actually destroy the object (and it shoudn't)
    /// </summary>
    public void DestroyAnimation()
    {
        transform.position += transform.right * 5;    // this is just for testing
        levelStateMachineHome.CallEndQuip();
    }
}
