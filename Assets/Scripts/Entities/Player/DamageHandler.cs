using System;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class DamageHandler : MonoBehaviour
    {
        public int heals;
        
        [Inject]
        private void Construct(GameSetup gameSetup)
        {
            heals = gameSetup.playerHeals;
        }
        
        public void TakeDamage(int damage)
        {
            Debug.Log("DAMAGE");

            heals = Math.Clamp(heals - damage, 0, int.MaxValue);
        }
    }
}