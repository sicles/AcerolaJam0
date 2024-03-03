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
            if (_playerIsDead) return;
            
            Debug.Log("takedamage has been called");
            playerHealth -= amount;

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
