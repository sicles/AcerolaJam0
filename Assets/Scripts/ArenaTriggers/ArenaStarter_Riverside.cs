using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using LevelStateMachines;
using PlayerScript;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArenaTriggers
{
    public class ArenaStarterRiverside : MonoBehaviour
    {
        [FormerlySerializedAs("enemiesToActivate")] [SerializeField] private List<PrototypeAI> firstWaveToActivate;
        [SerializeField] private List<PrototypeAI> secondWaveToActivate; 
        [SerializeField] private Animator entranceAnimator;
        [FormerlySerializedAs("levelStateMachine")] [SerializeField] private LevelStateMachine_Paris levelStateMachineParis;
        [SerializeField] private Animator exitAnimator;
        [SerializeField] private List<GameObject> oldGeometry;
        [SerializeField] private Material waterMaterial;
        [SerializeField] private Color startColor = new Color(0.2235293f, 0.5372549f, 0.7254902f, 0.6470588f);
        [SerializeField] private Color targetColor = new Color(0.4823529f, 0.05490194f, 0.01176471f, 0.6470588f);
        [SerializeField] private float startMetallic = 0.3f;
        [SerializeField] private float targetMetallic = 0.95f;
        [SerializeField] private List<Light> riverLights;
        [SerializeField] private List<Light> exitLights;
        [SerializeField] private GameObject arenaGeometry;
        [SerializeField] private bool arenaActiveOnStart;
        private bool _hasEntered;
        private static readonly int IsTriggered = Animator.StringToHash("IsTriggered");
        private bool _fightIsOver;
        private static readonly int WaterColor = Shader.PropertyToID("_Water_Color");
        private static readonly int Metallic = Shader.PropertyToID("_Metallic");
        private static readonly int Transparency = Shader.PropertyToID("_Transparency");

        private void Start()
        {
            arenaGeometry.gameObject.SetActive(arenaActiveOnStart);
            waterMaterial.SetColor(WaterColor, new Color(0.2235293f, 0.5372549f, 0.7254902f, 0.6470588f));
        }

        private bool CheckForSurvivors(int wave)
        {
            if (wave == 1)
            {
                if (!_fightIsOver)
                {
                    foreach (var enemy in firstWaveToActivate)
                    {
                        if (enemy.Alive) return false;
                    }

                    return true;
                }
            }
            
            if (wave == 2)
            {
                if (!_fightIsOver)
                {
                    foreach (var enemy in secondWaveToActivate)
                    {
                        if (enemy.Alive) return false;
                    }
                    
                    _fightIsOver = true;
                    return true;
                }
            }

            return false;
        }

        private void ReleaseArena()
        {
            foreach (var lighty in riverLights)
            {
                lighty.color = Color.white;
            }

            foreach (var shiny in exitLights)
            {
                shiny.gameObject.SetActive(true);
            }
            
            waterMaterial.SetColor(WaterColor, startColor);
            waterMaterial.SetFloat(Metallic, 0.3f);
            waterMaterial.SetFloat(Transparency, 0.995f);
            
            exitAnimator.SetBool(IsTriggered, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasEntered) return;
            if (other.gameObject.GetComponent<PlayerController>() == null)
                return;
        
            entranceAnimator.SetBool(IsTriggered, true);
            Invoke(nameof(DeactivateOldGeometry), 1f);
            levelStateMachineParis.RiversideArenaQuip();
            ThrowTheTea();

            _hasEntered = true;
        }

        private void StartArena()
        {
            StartCoroutine(RiversideFightRoutine());
        }

        private IEnumerator RiversideFightRoutine()
        {
            foreach (var enemy in firstWaveToActivate)
            {
                enemy.SetAlert();
            }

            yield return new WaitUntil(() => CheckForSurvivors(1));

            foreach (var enemy in secondWaveToActivate)
            {
                enemy.SetAlert();
            }
            
            yield return new WaitUntil(() => CheckForSurvivors(2));
            
            ReleaseArena();
        }
        
        private void ThrowTheTea()
        {
            StartCoroutine(ThrowTheTeaRoutine());
        }

        private IEnumerator ThrowTheTeaRoutine()
        {
            for (int i = 1; i < 21; i++)
            {
                yield return new WaitForSeconds(1f / i);
                waterMaterial.SetColor(WaterColor, startColor);
                waterMaterial.SetFloat(Metallic, startMetallic);
                waterMaterial.SetFloat(Transparency, 0.995f);
                foreach (var lighty in riverLights)
                {
                    lighty.color = Color.white;
                }
                
                yield return new WaitForSeconds(1f / i);
                waterMaterial.SetColor(WaterColor, targetColor);
                waterMaterial.SetFloat(Metallic, targetMetallic);
                waterMaterial.SetFloat(Transparency, 1f);
                foreach (var light in riverLights)
                {
                    light.color = targetColor;
                }
                
                startMetallic = Mathf.Lerp(startMetallic, targetMetallic, 0.1f);
            }

            StartArena();
        }

        private void DeactivateOldGeometry()
        {
            foreach (var mesh in oldGeometry)
            {
                mesh.SetActive(false);
            }
        }
    }
}
