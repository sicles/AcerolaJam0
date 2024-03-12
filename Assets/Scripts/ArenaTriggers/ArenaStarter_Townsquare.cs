using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using LevelStateMachines;
using PlayerScript;
using UnityEngine;
using UnityEngine.Serialization;

// listen i know i shouldn't just have copypasted this script instead of inheritance, mistakes were made

namespace ArenaTriggers
{
    public class ArenaStarter_Townsquare : MonoBehaviour
    {
        [SerializeField] private List<PrototypeAI> enemiesToActivate;
        [SerializeField] private Animator entranceAnimator;
        [SerializeField] private Animator exitAnimator;
        [SerializeField] private GameObject exitLight;
        [FormerlySerializedAs("levelStateMachine")] [SerializeField] private LevelStateMachine_Paris levelStateMachineParis; 
        [SerializeField] private GameObject oldGeometry;
        [SerializeField] private GameObject arenaGeometry;
        [SerializeField] private GameObject newGeometry;
        [SerializeField] private bool arenaGeometryActiveOnStart;
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
            exitAnimator.SetBool(IsTriggered, true);
            if (newGeometry != null)
                newGeometry.SetActive(true);
            
            exitLight.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasEntered) return;
            if (other.gameObject.GetComponent<PlayerController>() == null)
                return;
        
            entranceAnimator.SetBool(IsTriggered, true);
            Invoke(nameof(DeactivateOldGeometry), 1f);
            
            levelStateMachineParis.TownSquareArenaQuip();;


            StartCoroutine(AlertingRoutine());

            _hasEntered = true;
        }

        private IEnumerator AlertingRoutine()
        {
            foreach (var enemy in enemiesToActivate)
            {
                enemy.SetAlert();
                yield return new WaitForSeconds(0.25f);
            }
        }

        private void DeactivateOldGeometry()
        {
            if (oldGeometry != null)
                oldGeometry.SetActive(false);
        }
    }
}
