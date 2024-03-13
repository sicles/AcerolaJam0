using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

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
        private bool _isWalking;

        private void IsWalking()
        {
            if (_agent.velocity.magnitude > 0)
            {
                _animator.SetBool(Walking, true);
                _isWalking = true;
            }
            else
            {
                _animator.SetBool(Walking, false);
                _isWalking = false;
            }
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

        private void DecideFootstepSound()
        {
            if (!Alive || _playerController.PlayerIsDead)
            {
                _footstepSound.stop(STOP_MODE.ALLOWFADEOUT);
                return;
            }
            
            RuntimeManager.AttachInstanceToGameObject(_footstepSound,  transform);
            
            _footstepSound.getPlaybackState(out var footstepSoundPlaybackState);
            
            if (_isWalking)
            {
                if (footstepSoundPlaybackState != PLAYBACK_STATE.PLAYING)
                    _footstepSound.start();

                return;
            }
            
            
            // if (!_isWalking && enemyIsBusy)
            // {
            //     if (footstepSoundPlaybackState == PLAYBACK_STATE.PLAYING)
            //         _footstepSound.stop(STOP_MODE.ALLOWFADEOUT);
            // }
        }

        public void StopDynamicSounds()
        {
            _footstepSound.stop(STOP_MODE.ALLOWFADEOUT);
            idleAlertSound.stop(STOP_MODE.ALLOWFADEOUT);
            idleUnalertSound.stop(STOP_MODE.ALLOWFADEOUT);
        }
        
        private void DecideIdleSound()
        {
            if (!Alive || _playerController.PlayerIsDead)
            {
                idleAlertSound.stop(STOP_MODE.ALLOWFADEOUT);    // not checking for playstate might lead to bugs if enemy is never alive
                idleUnalertSound.stop(STOP_MODE.ALLOWFADEOUT);
                return;
            }
            
            RuntimeManager.AttachInstanceToGameObject(idleAlertSound,  transform);
            RuntimeManager.AttachInstanceToGameObject(idleUnalertSound,  transform);
            
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