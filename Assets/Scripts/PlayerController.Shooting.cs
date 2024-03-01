using UnityEngine;

public partial class PlayerController
{
    [SerializeField] private float shootRange;
    [SerializeField] private int ammunition;
    [SerializeField] private int maxAmmunition = 1;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletBackcallThresholdDistance = 0.1f;
    [SerializeField] private float bulletTravelSpeed = 3;
    private bool _bulletIsTraveling;
    private readonly float _bulletRotationSpeed = 1000;
    public Vector3 BulletBackcallDirection { get; private set; }
    
    partial void Shoot();
    partial void Reload();
    partial void AbortInput();  // not implemented
    partial void PauseGame();   // not checked in update **yet** because this is really annoying while editing

    partial void Shoot()
    {
        if (ammunition == 0) return;
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

        Ray ray = new Ray(playerCamera.transform.position + playerCamera.transform.forward, playerCamera.transform.forward);
        Debug.Log("Shoot input!");
        
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            bullet.GetComponent<BulletCollision>().LastEnemy = hit.transform.gameObject;
            
            bullet.transform.SetParent(null, true);
            bullet.transform.GetComponent<MeshRenderer>().enabled = true;
            ammunition--;
            
            AttachBullet(hit);
            
            if (hit.collider.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
                enemyHealth.TakeDamage(50, hit.transform.position, hit.normal, playerCamera.transform.forward);
        }
    }

    /// <summary>
    /// Attach bullet to hit object.
    /// </summary>
    /// <param name="hit"></param>
    private void AttachBullet(RaycastHit hit)
    {
        bullet.transform.SetParent(hit.transform.gameObject.transform, true);
        bullet.transform.rotation = playerCamera.transform.rotation;
        bullet.transform.position = hit.point;
    }

    partial void Reload()
    {
        if (ammunition >= maxAmmunition) return;
        if (!Input.GetKeyDown(KeyCode.R)) return;

        BulletRecall();
    }

    /// <summary>
    /// Called once to start bullet travel back to player.
    /// </summary>
    private void BulletRecall()
    {
        bullet.transform.SetParent(null, true);
        _bulletIsTraveling = true;
    }

    /// <summary>
    /// Called every frame while recalling, moving bullet in increments.
    /// </summary>
    private void BulletTravel()
    {
        if (!_bulletIsTraveling) return;

        BulletBackcallDirection = -(bullet.transform.position - playerCamera.transform.position);
        if (BulletBackcallDirection.magnitude < bulletBackcallThresholdDistance)
        {
            _bulletIsTraveling = false;
            CatchBullet();
            return;
        }

        bullet.transform.position += BulletBackcallDirection.normalized * (bulletTravelSpeed * Time.deltaTime);
        
        float bulletRotation = bullet.transform.localRotation.eulerAngles.z;
        bulletRotation += _bulletRotationSpeed * Time.deltaTime;
        bullet.transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, bulletRotation);
    }

    /// <summary>
    /// On bullet with contact -> player gets ammunition, bullet goes inactive
    /// </summary>
    private void CatchBullet()
    {
        Debug.Log("player caught bullet!");
        ammunition++;
        bullet.transform.SetParent(transform, true);
        bullet.transform.GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// Set timescale to 0 when pausing
    /// Set timescale to 1 when unpausing
    /// </summary>
    partial void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (Time.timeScale < 1)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;
    }

}

