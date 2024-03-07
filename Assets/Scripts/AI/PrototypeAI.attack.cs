using System.Collections;
using FMODUnity;
using UnityEngine;

namespace AI
{
    public partial class PrototypeAI
    {
        [Header("Attacks")]
        [SerializeField] bool enemyIsBusy = false;
        [SerializeField] private float hurtDuration = 0.5f;
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
        [SerializeField] private bool chargeReadyOnAlert;
        private readonly float _chargeCollisionRadius = 2;
        private float _chargeChance;
        private static readonly int IsHurt = Animator.StringToHash("IsHurt");


        private void ChargeCollisonCheck()
        {
            if (_chargeIsActive && _playerDistanceRaw.magnitude <= _chargeCollisionRadius && !_chargeHasHit)
            {
                _chargeHasHit = true;
                _playerController.TakeDamage(25);
                _playerController.CallCameraShake(0.2f, 30);
            }
        }
    
        private void CooldownTick()
        {
            if (_chargeTicker < chargeCooldown)
                _chargeTicker += Time.deltaTime;
        
            if (_attackTicker < attackCooldown)
                _attackTicker += Time.deltaTime;
        }
        
        private void FixedUpdate()
        {
            CalculateChargeChance();
        }
        
        private void CalculateChargeChance()
        {
            _chargeChance = Random.Range(0f, 1f);
        }
        
        private void ShouldCharge()
        {
            if (_chargeChance > 0.01f)   // TODO chance is frame dependent, cringe
                return;
            
            if (_playerDistanceRaw.magnitude <= chargeRadius 
                && _playerDistanceRaw.magnitude >= attackRadius
                && !enemyIsBusy && _chargeTicker >= chargeCooldown)
                StartCoroutine(ChargeRoutine());
        }

        private IEnumerator ChargeRoutine()
        {
            // Phase 1: charge is charged (haha)
            RuntimeManager.PlayOneShotAttached("event:/OnEnemyEvents/Charge", gameObject);
            _chargeHasHit = false;
            enemyIsBusy = true;
            StartChargingAnimation();
            _chargeTicker = 0;
            _chargeDirection = player.transform.position - transform.position;  // charge direction is decided at this point
            
            _agent.isStopped = true;
            yield return new WaitForSeconds(chargeWindupTime);
        
            // Phase 2: charge is executed
            _chargeIsActive = true;
            yield return new WaitForSeconds(chargeActiveTime);

            // Phase 3: enemy recovers from charge
        
            _chargeIsActive = false;

        
            yield return new WaitForSeconds(chargeRecoverTime);
        
            // Phase 4: enemy may taunt (RNG) OBSOLETE: taunting after charge is redundant (animation already includes a recovery)
            // ShouldTaunt();
            
            // Phase 5: enemy behaviour resets
            enemyIsBusy = false;
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
            if (_playerDistanceRaw.magnitude < attackRadius && !enemyIsBusy && _attackTicker >= attackCooldown)
                StartCoroutine(AttackRoutine());
        }

        /// <summary>
        /// Standard melee attack. Tracks player.
        /// </summary>
        /// <returns></returns>
        private IEnumerator AttackRoutine()
        {
            RuntimeManager.PlayOneShotAttached("event:/OnEnemyEvents/Attack", gameObject);
            enemyIsBusy = true;
            _attackTicker = 0;
            StartAttackAnimation();

            // Phase 1: Windup

            yield return new WaitForSeconds(attackWindupTime);
            
            // Phase 2: Attack

            if (Physics.Raycast(transform.position, _playerDistanceRaw, attackRadius, 1 << 7))
            {
                Debug.DrawLine(transform.position, 
                        player.transform.position - transform.position * attackRange,
                            Color.red);
                _playerController.TakeDamage(25);
                _playerController.CallCameraShake(0.2f, 20);
            }
            
            // Phase 3: Recovery

            _agent.isStopped = true;

            yield return new WaitForSeconds(attackRecoveryTime);

            // Phase 4: Resume default behaviour
            
            _agent.isStopped = false;
            enemyIsBusy = false;
        }

        private void CallHurtState()
        {
            StopAllCoroutines();
            StartCoroutine(HurtState());
        }
    
        /// <summary>
        /// Stuns NPC if taking damage.
        /// </summary>
        private IEnumerator HurtState()
        {
            ResetAllAnimationBools();
            RuntimeManager.PlayOneShotAttached("event:/OnEnemyEvents/Hurt", transform.gameObject);
            enemyIsBusy = true;
            _agent.isStopped = true;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            
            _animator.SetBool(IsHurt, true);

    
            yield return new WaitForSeconds(hurtDuration);

            _animator.SetBool(IsHurt, false);
            _agent.isStopped = false;
            enemyIsBusy = false;
        }
    }
}