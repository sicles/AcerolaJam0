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
    public class ArenaStarter_Classroom : MonoBehaviour
    {
        [SerializeField] private List<PrototypeAI> enemiesToActivate;
        [SerializeField] private Animator entranceAnimator;
        [SerializeField] private Animator exitAnimator;
        [SerializeField] private LevelStateMachine_Youth levelStateMachineYouth; 
        [SerializeField] private GameObject oldGeometry;
        [SerializeField] private GameObject arenaGeometry;
        [SerializeField] private GameObject newGeometry;
        [SerializeField] private bool arenaGeometryActiveOnStart;
        private bool _hasEntered;
        private static readonly int IsTriggered = Animator.StringToHash("IsTriggered");
        private bool _fightIsOver;

        [SerializeField] private Material cleanMat;
        [SerializeField] private Material leftWarningMat;
        [SerializeField] private Material rightWarningMat;
        [SerializeField] private MeshRenderer leftBlackboard;
        [SerializeField] private MeshRenderer rightBlackboard;
        

        private void Start()
        {
            arenaGeometry.SetActive(arenaGeometryActiveOnStart);
            leftBlackboard.material = cleanMat;
            rightBlackboard.material = cleanMat;
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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasEntered) return;
            if (other.gameObject.GetComponent<PlayerController>() == null)
                return;
            
            levelStateMachineYouth.CallClassroomMemory2();
        
            entranceAnimator.SetBool(IsTriggered, true);
            Invoke(nameof(DeactivateOldGeometry), 1f);

            StartCoroutine(BlackboardFlicker());
            
            foreach (var enemy in enemiesToActivate)
            {
                enemy.SetAlert();
            }

            _hasEntered = true;
        }

        private IEnumerator BlackboardFlicker()
        {
            yield return new WaitForSeconds(2f);
            
            for (int i = 0; i < 40; i++)
            {
                leftBlackboard.material = leftWarningMat;
                rightBlackboard.material = rightWarningMat;
                
                yield return new WaitForSeconds(0.1f);
                
                leftBlackboard.material = cleanMat;
                rightBlackboard.material = cleanMat;
            }
        }

        private void DeactivateOldGeometry()
        {
            oldGeometry.SetActive(false);
        }
    }
}
