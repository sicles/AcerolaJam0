using System.Collections;
using System.Collections.Generic;
using PlayerScript;
using UnityEngine;

namespace LevelStateMachines
{
    // ReSharper disable once InconsistentNaming
    public class LevelStateMachine_Home : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private PlayerController playerController;
        
        private void Start()
        {
            StartCoroutine(OpeningSequence());
        }

        private IEnumerator OpeningSequence()
        {
            yield return new WaitForSeconds(3f);
        }
    }
}
