using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    [SerializeField] private float shootRange;
    [SerializeField] private int ammunition;
    [SerializeField] private int maxAmmunition = 1;
    [SerializeField] private GameObject bullet;
    partial void Shoot();
    partial void Reload();
    partial void AbortInput();  // not implemented
    partial void PauseGame();   // not checked in update **yet** because this is really annoying while editing

    partial void Shoot()
    {
        if (ammunition == 0) return;
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

        ammunition--;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.Log("Shoot input!");
        
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            Debug.DrawRay(hit.collider.transform.position, playerCamera.transform.forward);
            MoveBullet(hit);
            if (hit.collider.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(10, hit.transform.position, hit.normal, playerCamera.transform.forward);
                Debug.Log("Dealt damage to an enemy!");
                return;
            }
            Debug.Log("Hit a non-enemy!");
            return;
        }
            
        Debug.Log("Missed!");
    }

    private void MoveBullet(RaycastHit hit)
    {
        // bullet.transform.parent = hit.transform.gameObject.transform;
        bullet.transform.rotation = hit.transform.rotation;
        bullet.transform.position = hit.point;
    }

    partial void Reload()
    {
        if (ammunition >= maxAmmunition) return;
        if (!Input.GetKeyDown(KeyCode.R)) return;
        
        ammunition++;
        Debug.Log("Reload successful");
        // supposed to call back your bullet - for now, just adds one
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

