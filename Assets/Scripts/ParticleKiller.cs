using UnityEngine;

public class ParticleKiller : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    
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
