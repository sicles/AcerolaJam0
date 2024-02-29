using System.Collections;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    [SerializeField] private float shootRange;
    [SerializeField] private EnemyHealth _enemyHealth;
    partial void Shoot();
    partial void AbortInput();  // not implemented
    partial void PauseGame();   // not checked in update **yet** because this is really annoying while editing

    partial void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray r = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            Debug.DrawLine(playerCamera.transform.position, playerCamera.transform.forward * shootRange);
            Debug.Log("Shoot input!");
            
            if (Physics.Raycast(r, out RaycastHit hit, shootRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out EnemyHealth _enemyHealth))
                {
                    _enemyHealth.TakeDamage(10);
                    Debug.Log("Dealt damage to an enemy!");
                    return;
                }
                Debug.Log("Hit a non-enemy!");
                return;
            }
            
            Debug.Log("Missed!");
        }            
    }
    
    partial void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (Time.timeScale < 1)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;
    }

}

