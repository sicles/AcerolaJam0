using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using PlayerScript;
using TMPro;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace LevelStateMachines
{
    // ReSharper disable once InconsistentNaming
    public class LevelStateMachine_Youth : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private List<GameObject> hatch;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Animator bedroomDoor;
        [SerializeField] private TextMeshProUGUI creditsTMP;

        [SerializeField] private Light tutorialDoorLight;
        private static readonly int IsBroken = Animator.StringToHash("IsBroken");
        private EventInstance _knockSound;
        private bool _knocking;

        private void Start()
        {
            StartCoroutine(OpeningSequence());
        }

        private IEnumerator OpeningSequence()
        {
            playerController.PlayerControlsAreOn = false;
            uiManager.Blackout();

            yield return new WaitForSeconds(5f);
            
            _knocking = true;
            KnockKnock();
            uiManager.Unblackout(10f);
            
            yield return new WaitForSeconds(10f);

            playerController.PlayerControlsAreOn = true;
            creditsTMP.gameObject.SetActive(true);

            yield return new WaitUntil(() => bedroomDoor.GetBool(IsBroken));

            RuntimeManager.PlayOneShotAttached("event:/BreakDoor", bedroomDoor.transform.gameObject);
            _knocking = false;
            _knockSound.stop(STOP_MODE.ALLOWFADEOUT);
            creditsTMP.gameObject.SetActive(false);
            
            yield return new WaitForSeconds(4f);
            
            foreach (var hatchpart in hatch)
            {
                hatchpart.SetActive(false);
            }
            
            Invoke(nameof(DropSequence), 2f);
        }

        private void KnockKnock()
        {
            _knockSound = RuntimeManager.CreateInstance("event:/KnockKnock");
            RuntimeManager.AttachInstanceToGameObject(_knockSound,  bedroomDoor.transform);

            StartCoroutine(KnockRoutine());
        }

        private IEnumerator KnockRoutine()
        {
            while (_knocking)
            {
                _knockSound.start();
                yield return new WaitForSeconds(Random.Range(4f, 7f));
            }
        }

        private void DropSequence()
        {
            StartCoroutine(DropSequenceRoutine());
        }

        private IEnumerator DropSequenceRoutine()
        {
            uiManager.CallSendMessage("You are not alone anymore.", 3f);
            yield return new WaitForSeconds(5f);
            uiManager.CallSendMessage("And no one will hurt you ever again.", 3f);
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
            tutorialDoorLight.gameObject.SetActive(true);
            yield return new WaitUntil(() => playerController.Ammunition == 0);
            uiManager.ClearTutorial();
        }

        public void CallClassroomMemory()
        {
            uiManager.CallSendMessage("I remember the first time you smiled at me.", 5f);
        }

        public void CallClassroomMemory2()
        {
            uiManager.CallSendMessage("I said something stupid, you turned around - and there it was.", 5f);
        }
        
        public void CallGalleryMemory()
        {
            uiManager.CallSendMessage("Beauty... well, I can't exactly say I get it.", 5f);
        }
        
        public void CallGalleryMemory2()
        {
            uiManager.CallSendMessage("But I know it when I see it.", 5f);
        }

        public void CallDashTutorial()
        {
            StartCoroutine(DashTutorialRoutine());
        }

        private IEnumerator DashTutorialRoutine()
        {
            uiManager.CallSendTutorial("Press 'Left Shift' to dash in any direction.");
            yield return new WaitUntil(() => playerController.DashIsActive);
            uiManager.ClearTutorial();
        }
    }
}
