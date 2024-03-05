using System.Collections;
using UnityEngine;

namespace AI
{
    public partial class PrototypeAI
    {
        private Animator _animator;
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        private static readonly int Walking = Animator.StringToHash("IsWalking");
        private static readonly int IsCharging = Animator.StringToHash("IsCharging");
        private static readonly int IsTaunting = Animator.StringToHash("IsTaunting");
        private static readonly int IdleState = Animator.StringToHash("IdleState");
        private static readonly int Dodging = Animator.StringToHash("IsDodging");
        private static readonly int DodgeDirection = Animator.StringToHash("DodgeDirection");

        private void IsWalking()
        {
            if (_agent.velocity.magnitude > 0)
                _animator.SetBool(Walking, true);
            else
                _animator.SetBool(Walking, false);
        }

        private void IsDodging()
        {
            if (_dodgeIsActive)
            {
                _animator.SetFloat(DodgeDirection, _dodgeDirection);
                _animator.SetBool(Dodging, true);
            }
            else
                _animator.SetBool(Dodging, false);
        }

        private void DecideIdleState()
        {
            // TODO it's overkill to call this every frame
            int newState = Random.Range(0, 100);
            _animator.SetInteger(IdleState, newState);
        }

        private void StartAttackAnimation()
        {
            StartCoroutine(AttackAnimationRoutine());
        }

        private IEnumerator AttackAnimationRoutine()
        {
            _animator.SetBool(IsAttacking, true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            _animator.SetBool(IsAttacking, false);
        }

        private void StartChargingAnimation()
        {
            StartCoroutine(ChargingAnimationRoutine());
        }
        
        private IEnumerator ChargingAnimationRoutine()
        {
            _animator.SetBool(IsCharging, true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            _animator.SetBool(IsCharging, false);
        }

        private void StartTauntingAnimation()
        {
            StartCoroutine(TauntingAnimationRoutine());
        }
        
        private IEnumerator TauntingAnimationRoutine()
        {
            enemyIsBusy = true;
            _animator.SetBool(IsTaunting, true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            _animator.SetBool(IsTaunting, false);
            enemyIsBusy = true;
        }

        private void ResetAllAnimationBools()
        {
            ResetAllBehaviours();
            
            _animator.SetBool(IsTaunting, false);
            _animator.SetBool(IsCharging, false);
            _animator.SetBool(IsAttacking, false);
            _animator.SetBool(Walking, false);
            _animator.SetBool(Dodging, false);
        }

        private void ResetAllBehaviours()
        {
            _dodgeIsActive = false;
            _chargeIsActive = false;
        }
    }
}