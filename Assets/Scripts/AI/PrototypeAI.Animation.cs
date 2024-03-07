using System.Collections;
using FMOD.Studio;
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
        private int _lastPositionIdleAlertSound;
        private int _lastPositionIdleUnalertSound1;
        
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

        private void DecideIdleSound()
        {
            if (!Alive)
            {
                idleAlertSound.stop(STOP_MODE.ALLOWFADEOUT);    // not checking for playstate might lead to bugs if enemy is never alive
                idleUnalertSound.stop(STOP_MODE.ALLOWFADEOUT);
                return;
            }
            
            if (isAlert)
            {
                idleAlertSound.getPaused(out var idleAlertSoundisPaused);
                idleAlertSound.getPlaybackState(out var idleAlertPlaybackState);
                if (idleAlertPlaybackState != PLAYBACK_STATE.PLAYING)
                {
                    idleAlertSound.start();
                }
                
                if (!idleAlertSoundisPaused)
                {
                    idleAlertSound.setTimelinePosition(_lastPositionIdleAlertSound);

                    idleAlertSound.setPaused(false);
                }

                else if (enemyIsBusy)
                {
                    idleAlertSound.getTimelinePosition(out _lastPositionIdleAlertSound);

                    idleAlertSound.setPaused(true);
                }
                
            }
            else
            {
                idleUnalertSound.getPaused(out var idleUnalertSoundisPaused);
                idleUnalertSound.getPlaybackState(out var idleUnalertPlaybackState);
                if (idleUnalertPlaybackState != PLAYBACK_STATE.PLAYING)
                    idleUnalertSound.start();

                if (!idleUnalertSoundisPaused)
                {
                    idleUnalertSound.setTimelinePosition(_lastPositionIdleAlertSound);

                    idleUnalertSound.setPaused(false);
                }
                else if (enemyIsBusy)
                {
                    idleUnalertSound.getTimelinePosition(out _lastPositionIdleAlertSound);

                    idleUnalertSound.setPaused(true);
                }
            }
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