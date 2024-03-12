using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerScript
{
    public partial class PlayerController
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private CallOfDuty callOfDuty;
        
        [SerializeField] private int playerHealth;
        private readonly int _playerMaxHealth = 100;
        private bool _playerIsDead;

        public bool PlayerIsDead => _playerIsDead;

        public void TakeDamage(int amount)
        {
            if (playerIsInvincible)
            {
                Debug.Log("player has dodged via iframes!");    // TODO may implement player feedback to this later
                return;
            }
            if (PlayerIsDead) return;
            
            playerHealth -= amount;
            callOfDuty.FlashScreen();
            CallCameraShake(0.2f, amount);
            RuntimeManager.PlayOneShot("event:/OnPlayerEvents/Hurt");
            CallPlayerIFrames(60);   // give player some time to breath

            uiManager.UpdateHealthbar(amount);

            if (playerHealth <= 0)
                PlayerDeath();
        }

        private void PlayerDeath()
        {
            PlayFootsteps(false);

            StartCoroutine(StopAllSounds());
            
            RuntimeManager.PlayOneShot("event:/OnPlayerEvents/Death");
            
            _playerIsDead = true;

            // for camera tilt logic see PlayerTilt()
            
            StartCoroutine(PlayerDeathRoutine());
        }

        private IEnumerator StopAllSounds()
        {
            yield return new WaitForSeconds(5f);
            RuntimeManager.PauseAllEvents(true);
        }
        
        private IEnumerator PlayerDeathRoutine()
        {
            foreach (var mesh in firstPersonMeshes)
            {
                mesh.SetActive(false);
            }
            
            
            uiManager.SlowBlackout(3f);
            yield return new WaitForSeconds(6f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public int GetPlayerHealth() => playerHealth;
    }
}
