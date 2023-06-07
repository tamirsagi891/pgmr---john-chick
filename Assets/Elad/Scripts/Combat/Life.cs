using System;
using Elad.Scripts.Combat;
using UnityEngine;

namespace Elad.Scripts
{
    public class Life : MonoBehaviour
    {
        [SerializeField] private int healAmount = 10;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController pC = other.GetComponent<PlayerController>();
            if (pC)
            {
                Damageable dam = other.GetComponent<Damageable>();
                dam.AddLife(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
