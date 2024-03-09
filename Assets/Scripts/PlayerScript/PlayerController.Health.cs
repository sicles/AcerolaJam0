using FMODUnity;
using UnityEngine;

namespace PlayerScript
{
    public partial class PlayerController
    {
        [SerializeField] private UIManager uiManager;
        
        [SerializeField] private int playerHealth;
        private readonly int _playerMaxHealth = 100;
        private bool _playerIsDead;

        public void TakeDamage(int amount)
        {
            if (playerIsInvincible)
            {
                Debug.Log("player has dodged via iframes!");    // TODO may implement player feedback to this later
                return;
            }
            if (_playerIsDead) return;
            
            playerHealth -= amount;
            CallCameraShake(0.2f, amount);
            RuntimeManager.PlayOneShot("event:/OnPlayerEvents/Hurt");
            CallPlayerIFrames(15);   // give player some time to breath

            uiManager.UpdateHealthbar(amount);

            if (playerHealth <= 0)
                PlayerDeath();
        }

        private void PlayerDeath()
        {
            Debug.Log("You are dead. This is not poggers :(");
            _playerIsDead = true;
        }

        public int GetPlayerHealth() => playerHealth;
    }
}
