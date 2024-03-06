﻿using System.Collections;
using UnityEngine;

namespace PlayerScript
{
    public partial class PlayerController
    {
        private Animator _animator;
        private static readonly int Walking = Animator.StringToHash("IsWalking");
        private static readonly int IsShooting = Animator.StringToHash("IsShooting");
        private static readonly int IsRecalling = Animator.StringToHash("IsRecalling");
        private static readonly int IsRacking = Animator.StringToHash("IsRacking");
        private static readonly int IsCatching = Animator.StringToHash("IsCatching");
        private static readonly int IsRacked = Animator.StringToHash("IsRacked");
        [SerializeField] private ParticleSystem bulletReadyParticle;
        private static readonly int Loaded = Animator.StringToHash("IsLoaded");
        private Coroutine _shootRoutine;
        private Coroutine _catchReloadCoroutine;

        private void SetBulletReadyParticleState(bool isActive)
        {
            bulletReadyParticle.gameObject.SetActive(isActive);
        }
        
        private void IsWalking()
        {
            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
                _animator.SetBool(Walking, true);
            else
                _animator.SetBool(Walking, false);
        }

        private void IsLoaded()
        {
            if (ammunition > 0)
                _animator.SetBool(Loaded, true);
            else
                _animator.SetBool(Loaded, false);
        }

        private void CallShootAnimation()
        {
            StartCoroutine(nameof(ShootAnimationRoutine));
        }

        private IEnumerator ShootAnimationRoutine()
        {
            _animator.SetBool(IsShooting, true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            _animator.SetBool(IsShooting, false);
            if (ammunition > 0) ammunition--;
            _gunIsRacked = false;
        }

        private void CallRecallAnimation()
        {
            StartCoroutine(nameof(RecallAnimationRoutine));
        }
        
        private IEnumerator RecallAnimationRoutine()
        {
            _animator.SetBool(IsRecalling, true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            _animator.SetBool(IsRecalling, false);
        }

        private void StartCatchReloadAnimation()
        {
            if (_catchReloadCoroutine == null)
            {
                _catchReloadCoroutine = StartCoroutine(CatchReloadAnimationRoutine(90));
            }
        }

        private void CheckIfReloading()
        {
            _reloadIsPlaying = (_catchReloadCoroutine != null);
        }
        
        private IEnumerator CatchReloadAnimationRoutine(int frames)     // set to animation frame amounts
        {
            StopRackAnimation();
            _rackIsReady = false;
            _animator.SetBool(IsCatching, true);
            
            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            _animator.SetBool(IsCatching, false);
            SetBulletReadyParticleState(true);

            yield return new WaitForSeconds(0.25f);
            
            _catchReloadCoroutine = null;
            ammunition++;
        }

        private void StartRackAnimation()
        {
            _animator.SetBool(IsRacking, true);
        }

        private void StopRackAnimation()
        {
            _animator.SetBool(IsRacking, false);
        }

        private void SetGunRacked()
        {
            if (_gunIsRacked)
                _animator.SetBool(IsRacked, true);
            else
                _animator.SetBool(IsRacked, false);
        }
        
    }
}