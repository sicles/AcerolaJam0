using System;
using System.Collections.Generic;
using AI;
using FMOD.Studio;
using FMODUnity;
using LevelStateMachines;
using PlayerScript;
using UnityEngine;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

// listen i know i shouldn't just have copypasted this script instead of inheritance, mistakes were made

namespace ArenaTriggers
{
    public class ArenaStarter_Gallery : MonoBehaviour
    {
        [SerializeField] private List<PrototypeAI> enemiesToActivate;
        [SerializeField] private Animator entranceAnimator;
        [SerializeField] private Animator exitAnimator;
        [SerializeField] private LevelStateMachine_Youth levelStateMachineYouth; 
        [SerializeField] private GameObject oldGeometry;
        [SerializeField] private GameObject arenaGeometry;
        [SerializeField] private GameObject newGeometry;
        [SerializeField] private bool arenaGeometryActiveOnStart;
        
        [SerializeField] private GameObject paintingExit;
        [SerializeField] private GameObject paintingRamp;

        private bool _hasEntered;
        private static readonly int IsTriggered = Animator.StringToHash("IsTriggered");
        private bool _fightIsOver;

        private void Start()
        {
            arenaGeometry.SetActive(arenaGeometryActiveOnStart);
        }

        private void FixedUpdate()
        {
            CheckForSurvivors();
        }

        private void CheckForSurvivors()
        {
            if (!_fightIsOver)
            {
                foreach (var enemy in enemiesToActivate)
                {
                    if (enemy.Alive) return;
                }
            }

            _fightIsOver = true;
            ReleaseArena();
        }

        private void ReleaseArena()
        {
            // exitAnimator.SetBool(IsTriggered, true);
            if (newGeometry != null)
                newGeometry.SetActive(true);
            
            paintingExit.SetActive(false);
            paintingRamp.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasEntered) return;
            if (other.gameObject.GetComponent<PlayerController>() == null)
                return;
        
            levelStateMachineYouth.CallGalleryMemory2();
            
            entranceAnimator.SetBool(IsTriggered, true);
            Invoke(nameof(DeactivateOldGeometry), 1f);
            
            foreach (var enemy in enemiesToActivate)
            {
                enemy.SetAlert();
            }

            _hasEntered = true;
        }

        private void DeactivateOldGeometry()
        {
            oldGeometry.SetActive(false);
        }
    }
}
