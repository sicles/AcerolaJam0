using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private GameObject bloodDecal;
    
    void Start()
    {
        Invoke(nameof(CleanUp), lifeTime);   
    }

    void CleanUp()
    {
        Destroy(this.transform.gameObject);
    }
}
