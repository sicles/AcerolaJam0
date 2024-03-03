using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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
        private float _dodgeDirection;
        private bool _isLookedAt;

        private void OnMouseEnter()
        {
            _isLookedAt = true;
        }

        private void OnMouseExit()
        {
            _isLookedAt = false;
        }

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

            if (_isLookedAt && !_enemyIsBusy)
            {
                if (Random.Range(0, 100) < dodgeChance)
                {
                    StartCoroutine(DodgeRoutine());
                }
            }
            
            dodgeTicker = 0;
        }

        private void Dodge()
        {
            if (!_dodgeIsActive) return;
            
            _agent.Move(_dodgeDirection * transform.right * (dodgeSpeed * Time.deltaTime));
        }

        private IEnumerator DodgeRoutine()
        {
            // decide dodge direction
            _dodgeDirection = Random.Range(-1f, 1f);
            _dodgeDirection = _dodgeDirection < 1 ? -1 : 1;

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
