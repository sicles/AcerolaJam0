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
        Debug.Log("Particle destroyed itself!");
        Destroy(this.transform.gameObject);
    }
}
