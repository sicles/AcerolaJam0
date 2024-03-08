using UnityEngine;

namespace LevelStateMachines
{
    // ReSharper disable once InconsistentNaming
    public class LevelStateMachine_Youth : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;

        private void Start()
        {
            Invoke(nameof(Greeting), 3f);
        }

        private void Greeting()
        {
            uiManager.CallSendMessage("You are not alone anymore.", 3f);

        }
    }
}
