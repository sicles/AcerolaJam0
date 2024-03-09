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

        [SerializeField] private List<Light> lightsToSwitchOff;
        [SerializeField] private List<Light> lightsToSwitchOn;
        
        private void Start()
        {
            StartCoroutine(OpeningSequence());
        }

        private IEnumerator OpeningSequence()
        {
            yield return new WaitForSeconds(3f);
            StartCoroutine(LightSwitch());
            uiManager.CallSendMessage("Our apartment...", 5f);
            yield return new WaitForSeconds(5f);
            uiManager.CallSendMessage("It was more than just 'our place'.", 5f);
            yield return new WaitForSeconds(5f);
            uiManager.CallSendMessage("It was home.", 3f);
            yield return new WaitForSeconds(5f);
            uiManager.CallSendMessage("And then, it was something else", 3f);
            yield return new WaitForSeconds(5f);
            _arenaStarterApartment.ActivateHomeArena();
            }

        private IEnumerator LightSwitch()
        {
            yield return new WaitForSeconds(5f);

            foreach (var lightOff in lightsToSwitchOff)
            {
                lightOff.gameObject.SetActive(false);
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(5f);

            foreach (var lightOn in lightsToSwitchOn)
            {
                lightOn.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.25f);
            }
        }

        public void CallArenaEndQuip()
        {
            StartCoroutine(ArenaEndQuipRoutine());
        }
        
        private IEnumerator ArenaEndQuipRoutine()
        {
            yield return new WaitForSeconds(5f);
            uiManager.CallSendMessage("I am waiting for you.", 5f);
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
                uiManager.CallSendMessage("You will never be alone again.", 5f);
        }
    }
}
