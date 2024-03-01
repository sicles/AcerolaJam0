using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    private int _playerHealth;
    private int _playerMaxHealth;

    public void TakeDamage(int amount)
    {
        Debug.Log("takedamage has been called");
        _playerHealth -= amount;

        if (_playerHealth <= 0)
            PlayerDeath();
    }

    private void PlayerDeath()
    {
        Debug.Log("You are dead. This is not poggers :(");
    }
}
