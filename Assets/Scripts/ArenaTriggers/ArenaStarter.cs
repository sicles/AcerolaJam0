using System.Collections.Generic;
using AI;
using PlayerScript;
using UnityEngine;

namespace ArenaTriggers
{
    public class ArenaStarter : MonoBehaviour
    {
        [SerializeField] private List<PrototypeAI> enemiesToActivate;
        [SerializeField] private Animator entranceAnimator;
        [SerializeField] private Animator exitAnimator;
        [SerializeField] private GameObject oldGeometry;
        private bool _hasEntered;
        private static readonly int IsTriggered = Animator.StringToHash("IsTriggered");
        private bool _fightIsOver;


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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasEntered) return;
            if (other.gameObject.GetComponent<PlayerController>() == null)
                return;
        
            entranceAnimator.SetBool(IsTriggered, true);
            Invoke(nameof(DeactivateOldGeometry), 1f);
        
            foreach (var enemy in enemiesToActivate)
            {
                enemy.SetAlert(true);
            }

            _hasEntered = true;
        }

        private void DeactivateOldGeometry()
        {
            oldGeometry.SetActive(false);
        }
    }
}
