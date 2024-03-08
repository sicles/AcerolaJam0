using System.Collections;
using System.Collections.Generic;
using PlayerScript;
using UnityEngine;

namespace LevelStateMachines
{
    // ReSharper disable once InconsistentNaming
    public class LevelStateMachine_Youth : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private List<GameObject> hatch;
        [SerializeField] private PlayerController playerController;
        
        private void Start()
        {
            StartCoroutine(OpeningSequence());
        }

        private IEnumerator OpeningSequence()
        {
            yield return new WaitForSeconds(3f);
            foreach (var hatchpart in hatch)
            {
                hatchpart.SetActive(false);
            }
            Invoke(nameof(DropSequence), 2f);
        }

        private void DropSequence()
        {
            uiManager.CallSendMessage("You are not alone anymore.", 3f);
        }

        public void CallGunTutorial()
        {
            StartCoroutine(GunTutorialRoutine());
        }
        
        private IEnumerator GunTutorialRoutine()
        {
            uiManager.CallSendTutorial("Press 'R' to recall your bullet.");
            yield return new WaitUntil(() => playerController.Ammunition > 0);
            uiManager.ClearTutorial();
            StartCoroutine(ReloadTutorialRoutine());
        }

        private IEnumerator ReloadTutorialRoutine()
        {
            uiManager.CallSendTutorial("Hold 'Right Mouse' to pull back the hammer.");
            yield return new WaitUntil(() => playerController.GunIsRacked);
            uiManager.ClearTutorial();
            StartCoroutine(ShootTutorialRoutine());
        }

        private IEnumerator ShootTutorialRoutine()
        {
            uiManager.CallSendTutorial("Press 'Left Mouse' to fire.");
            yield return new WaitUntil(() => playerController.Ammunition == 0);
            uiManager.ClearTutorial();
        }
    }
}
