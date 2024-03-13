using System.Collections;
using System.Collections.Generic;
using ArenaTriggers;
using PlayerScript;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelStateMachines
{
    // ReSharper disable once InconsistentNaming
    public class LevelStateMachine_Home : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private ArenaStarter_Apartment _arenaStarterApartment;

        [SerializeField] private GameObject window;
        [SerializeField] private Material windowReplacementMat;
        
        [SerializeField] private List<Light> lightsToSwitchOff;
        [SerializeField] private List<GameObject> lightsToSwitchOn;
        [SerializeField] private GameObject fightOverLight;
        
        private void Start()
        {
            StartCoroutine(OpeningSequence());
            FadeIn();
        }
        
        private void FadeIn()
        {
            uiManager.Blackout();
            uiManager.Unblackout(3f, 2f);
        }

        private IEnumerator OpeningSequence()
        {
            yield return new WaitForSeconds(5f);
            StartCoroutine(LightSwitch());
            uiManager.CallSendMessage("Our apartment...", 5f);
            yield return new WaitForSeconds(5f);
            uiManager.CallSendMessage("It was more than just 'our place'.", 5f);
            yield return new WaitForSeconds(5f);
            uiManager.CallSendMessage("It was home.", 3f);
            yield return new WaitForSeconds(5f);
            uiManager.CallSendMessage("And then, it was something else", 3f);
            yield return new WaitForSeconds(5f);    // 25 seconds
            _arenaStarterApartment.ActivateHomeArena();     // 24 seconds
            }

        private IEnumerator LightSwitch()
        {
            foreach (var lightOff in lightsToSwitchOff)
            {
                lightOff.gameObject.SetActive(false);
                yield return new WaitForSeconds(5f);
            }

            foreach (var lightOn in lightsToSwitchOn)
            {
                lightOn.SetActive(true);
                yield return new WaitForSeconds(6f);
            }

            window.GetComponent<MeshRenderer>().material = windowReplacementMat;
        }

        public void CallArenaEndQuip()
        {
            // disabled due to bug
            // StartCoroutine(ArenaEndQuipRoutine());
        }
        
        private IEnumerator ArenaEndQuipRoutine()
        {
            fightOverLight.SetActive(true);
            yield return new WaitForSeconds(3f);
            uiManager.CallSendMessage("I am waiting.", 5f);
        }

        public void CallEndQuip()
        {
            StopAllCoroutines();
            StartCoroutine(EndQuip());
        }

        private IEnumerator EndQuip()
        {
            uiManager.CallSendMessage("You can scratch, kick, and bite all you want.", 5f);
            yield return new WaitForSeconds(5f);
            if (!playerController.IsEnd)    // ifs in case the player goes in guns blazing
                uiManager.CallSendMessage("I am a part of you now and forever.", 5f);
            yield return new WaitForSeconds(5f);
            if (!playerController.IsEnd)
                uiManager.CallSendMessage("You will never be alone again.", 5000f);
        }
    }
}
