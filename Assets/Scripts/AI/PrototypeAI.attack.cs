using System.Collections;
using UnityEngine;

namespace AI
{
    public partial class PrototypeAI
    {
        [Header("Attacks")]
        private readonly bool _feelsPain = true;
        private bool _enemyIsBusy;
        private readonly float _hurtDuration = 0.5f;
        [SerializeField] private float chargeRadius = 3f;
        [SerializeField] private float attackRadius = 1f;
        private Vector3 _chargeDirection;
        private bool _chargeIsActive;
        private bool _chargeHasHit;
        [SerializeField] private float chargeSpeed = 3f;
    
        [Header("Cooldowns")] 
        [SerializeField] private float chargeCooldown = 8f;
        [SerializeField] private float _chargeTicker;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float _attackTicker;
        [SerializeField] private float chargeWindupTime = 1;
        [SerializeField] private float chargeActiveTime = 0.5f;
        [SerializeField] private float chargeRecoverTime = 1;
        [SerializeField] private float attackWindupTime = 0.75f;
        [SerializeField] private float attackRecoveryTime = 1f;
        [SerializeField] float attackRange = 3;


        private void ChargeCollisonCheck()
        {
            if (_chargeIsActive && _playerDistance.magnitude <= 1 && !_chargeHasHit)
            {
                Debug.Log("charge collision happened!");
                _chargeHasHit = true;
                _playerController.TakeDamage(25);
            }
        }
    
        private void CooldownTick()
        {
            if (_chargeTicker < chargeCooldown)
                _chargeTicker += Time.deltaTime;
        
            if (_attackTicker < attackCooldown)
                _attackTicker += Time.deltaTime;
        }
        
        private void ShouldCharge()
        {
            if (_playerDistance.magnitude <= chargeRadius && !_enemyIsBusy && _chargeTicker >= chargeCooldown)
                StartCoroutine(ChargeRoutine());
        }

        private IEnumerator ChargeRoutine()
        {
            // Phase 1: charge is charged (haha)
            _chargeHasHit = false;
            _enemyIsBusy = true;
            _chargeTicker = 0;
            _chargeDirection = player.transform.position - transform.position;  // charge direction is decided at this point
            
            _agent.isStopped = true;
            Debug.Log("CHARGE STARTED");
            yield return new WaitForSeconds(chargeWindupTime);
        
            // Phase 2: charge is executed
            _chargeIsActive = true;
            yield return new WaitForSeconds(chargeActiveTime);

            // Phase 3: enemy recovers from charge
        
            _chargeIsActive = false;
            _enemyIsBusy = false;

        
            yield return new WaitForSeconds(chargeRecoverTime);
        
            // Phase 4: enemy may taunt (RNG)
            ShouldTaunt();
            
            // Phase 5: enemy behaviour resets
            _agent.isStopped = false;

        }

        /// <summary>
        /// Gapclosing attack. Does not track player.
        /// </summary>
        private void Charge()
        {
            if (_chargeIsActive)
                _agent.Move(_chargeDirection.normalized * (chargeSpeed * Time.deltaTime));
        }
    
        private void ShouldAttack()
        {
            if (_playerDistance.magnitude < attackRadius && !_enemyIsBusy && _attackTicker >= attackCooldown)
                StartCoroutine(Attack());
        }

        /// <summary>
        /// Standard melee attack. Tracks player.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Attack()
        {
            Debug.Log("Attack was started!");
            _enemyIsBusy = true;
            _attackTicker = 0;

            // Phase 1: Windup

            yield return new WaitForSeconds(attackWindupTime);
            
            // Phase 2: Attack

            if (Physics.Raycast(transform.position, _playerDistance, attackRadius, 1 << 7))
            {
                Debug.Log("player has been hit!");
                Debug.DrawLine(transform.position, 
                        player.transform.position - transform.position * attackRange,
                            Color.red);
                _playerController.TakeDamage(25);
            }
            
            // Phase 3: Recovery

            _agent.isStopped = true;

            yield return new WaitForSeconds(attackRecoveryTime);

            // Phase 4: Resume default behaviour
            
            _agent.isStopped = false;
            _enemyIsBusy = false;
        }

        private void CallHurtState()
        {
            if (!_alive) return;
            
            StopAllCoroutines();
            StartCoroutine(HurtState());
        }
    
        /// <summary>
        /// Stuns NPC if taking damage.
        /// </summary>
        private IEnumerator HurtState()
        {
            yield return new WaitForEndOfFrame();

            if (_feelsPain)
            {
                _enemyIsBusy = true;
                _agent.isStopped = true;
        
                yield return new WaitForSeconds(_hurtDuration);

                _agent.isStopped = false;
                _enemyIsBusy = false;
            }
        }
    }
}