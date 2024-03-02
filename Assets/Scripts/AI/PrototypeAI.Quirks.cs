using System.Collections;
using UnityEngine;

namespace AI
{
    public partial class PrototypeAI
    {
        [SerializeField] float dodgeCooldown = 5f;
        [SerializeField] private int dodgeChance = 30;
        [SerializeField] private float dodgeTicker;
        [SerializeField] float tauntDuration = 2;
        [SerializeField] int tauntChance = 50;
        [SerializeField] private float dodgeDuration = 0.25f;
        private bool _dodgeIsActive;
        [SerializeField] float dodgeSpeed = 5;
        private float _direction;

        /// <summary>
        /// Performs a dodge if player is currently aiming at this enemy
        /// uses RNG
        /// </summary>
        private void ShouldDodge()
        {
            if (dodgeTicker < dodgeCooldown)
            {
                dodgeTicker += Time.deltaTime;
                return;
            }

            if (_playerController.GetPlayerViewDirection().transform.gameObject == transform.gameObject && !_enemyIsBusy)
            {
                if (Random.Range(0, 100) < dodgeChance)
                {
                    Debug.Log("wiggle wiggle");
                    StartCoroutine(DodgeRoutine());
                }
                else
                    Debug.Log("no wiggle :(");
            }
            
            dodgeTicker = 0;
        }

        private void Dodge()
        {
            if (!_dodgeIsActive) return;
            
            _agent.Move(_direction * transform.right * (dodgeSpeed * Time.deltaTime));
        }

        private IEnumerator DodgeRoutine()
        {
            // decide dodge direction
            _direction = Random.Range(-1f, 1f);
            _direction = _direction < 1 ? -1 : 1;

            _dodgeIsActive = true;
            _agent.isStopped = true;
            
            yield return new WaitForSeconds(dodgeDuration);

            _dodgeIsActive = false;
            _agent.isStopped = false;
        }

        /// <summary>
        /// Enemy stops to play a taunt animation
        /// Should be called on aggro and missed attacks
        /// uses RNG
        /// </summary>
        private void ShouldTaunt()
        {
            if (Random.Range(0, 100) < tauntChance)
                StartCoroutine(Taunt());
        }

        private IEnumerator Taunt()
        {
            var accelerationBuffer = _agent.acceleration;
            _agent.acceleration = 0;
            Debug.Log("huge taunt!");
            yield return new WaitForSeconds(tauntDuration);
            _agent.acceleration = accelerationBuffer;
        }
    }
}
