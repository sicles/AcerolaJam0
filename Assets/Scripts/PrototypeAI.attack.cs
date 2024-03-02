using System.Collections;
using UnityEngine;

public partial class PrototypeAI : MonoBehaviour
{
    [Header("Attacks")]
    [SerializeField] private float chargeRadius = 3f;
    [SerializeField] private float attackRadius = 1f;
    private bool _enemyIsBusy;
    private bool _attackRoutineActive;
    private Vector3 _chargeDirection;
    private bool _chargeIsActive;
    private bool _attackIsActive;
    [SerializeField] private float chargeSpeed = 3f;
    private readonly bool _feelsPain = true;
    private float _hurtDuration = 0.25f;

    [Header("Cooldowns")] 
    [SerializeField] private float chargeCooldown = 8f;
    private float _chargeTicker;
    [SerializeField] private float attackCooldown = 2f;
    private float _attackTicker;

    private void CooldownTick()
    {
        _chargeTicker += Time.fixedTime;
        _attackTicker += Time.fixedTime;
    }
        
    private void ShouldCharge()
    {
        if (_playerDistance.magnitude <= chargeRadius && !_enemyIsBusy)
            StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        _enemyIsBusy = true;

        
        _agent.isStopped = true;
        Debug.Log("CHARGE STARTED");
        yield return new WaitForSeconds(0.5f);
        _chargeDirection = player.transform.position - transform.position;
        _chargeIsActive = true;
        yield return new WaitForSeconds(0.5f);
        _chargeIsActive = true;
        _agent.isStopped = false;

        
        _enemyIsBusy = false;
    }

    private void Charge()
    {
        if (_chargeIsActive)
            _agent.Move(_chargeDirection * (chargeSpeed * Time.deltaTime));
    }
    
    private void ShouldAttack()
    {
        if (_playerDistance.magnitude < attackRadius && !_enemyIsBusy)
            StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        _attackRoutineActive = true;
        
        yield return new WaitForEndOfFrame();
        
        _attackRoutineActive = false;
    }

    /// <summary>
    /// Stuns NPC if taking damage.
    /// This aborts all coroutines on this object, effectively canceling attacks.
    /// </summary>
    private IEnumerator HurtState()
    {
        yield return new WaitForEndOfFrame();

        if (_feelsPain)
        {
            StopAllCoroutines();
            _chargeIsActive = false;
            _attackIsActive = false;
            _enemyIsBusy = true;
        
            yield return new WaitForSeconds(_hurtDuration);

            _enemyIsBusy = false;
        }
    }
}