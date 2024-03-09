using System;
using System.Collections.Generic;
using AI;
using LevelStateMachines;
using PlayerScript;
using UnityEngine;

// listen i know i shouldn't just have copypasted this script instead of inheritance, mistakes were made

namespace ArenaTriggers
{
    public class ArenaStarter_Apartment : MonoBehaviour
    {
        [SerializeField] private List<PrototypeAI> enemiesToActivate;
        [SerializeField] private Animator exitAnimator;
        [SerializeField] private LevelStateMachine_Home levelStateMachineHome; 
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
            
            // levelStateMachineHome.CallArenaEndQuip();
        }

        public void ActivateHomeArena()
        {
            foreach (var enemy in enemiesToActivate)
            {
                enemy.SetAlert();
            }
        }
    }
}
