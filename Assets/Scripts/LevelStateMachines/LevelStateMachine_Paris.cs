using System.Collections;
using UnityEngine;

namespace LevelStateMachines
{
    // ReSharper disable once InconsistentNaming
    public class LevelStateMachine_Paris : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        private bool _townSquareMemoryHasBeenCalled;
        private bool _townSquareArenaQuipHasBeenCalled;
        private bool _riversideEntranceMemoryHasBeenCalled;
        private bool _riversideArenaQuipHasBeenCalled;

        private void Start()
        {
            Invoke(nameof(Greeting), 3f);
            FadeIn();
        }

        private void Greeting()
        {
            uiManager.CallSendMessage("The music... the wine... the dancing...", 3f);

        }

        private void FadeIn()
        {
            uiManager.Blackout();
            uiManager.Unblackout(3f, 2f);
        }

        public void TownSquareMemory()
        {
            if (_townSquareMemoryHasBeenCalled) return;

            _townSquareMemoryHasBeenCalled = true;
            uiManager.CallSendMessage("I was too embarassed to ask.", 3f);
        }

        public void TownSquareArenaQuip()
        {
            if (_townSquareArenaQuipHasBeenCalled) return;

            _townSquareArenaQuipHasBeenCalled = true;
            uiManager.CallSendMessage("But then... you did.", 3f);    
        }

        public void RiversideEntranceMemory()
        {
            if (_riversideEntranceMemoryHasBeenCalled) return;

            _riversideEntranceMemoryHasBeenCalled = true;
            uiManager.CallSendMessage("Then, by the iridescent river...", 3f);
        }

        public void RiversideArenaQuip()
        {
            if (_riversideArenaQuipHasBeenCalled) return;

            _riversideArenaQuipHasBeenCalled = true;
            uiManager.CallSendMessage("...watching you.", 3f);
        }
    }
}
