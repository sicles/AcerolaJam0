using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;

    public void TakeDamage(int damage)
    {
        if (health - damage > 0)
            health -= damage;
        else
            Die();
    }

    private void Die()
    {
        Debug.Log("Ouch!");
    }
}
