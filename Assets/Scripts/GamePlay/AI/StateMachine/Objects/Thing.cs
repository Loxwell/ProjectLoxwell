using Platformer.Player.Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thing : MonoBehaviour
{
    public void TakeDamage(Damager damager, Damageable damageable)
    {
        Debug.Log("TakeDamage");
    }
}
