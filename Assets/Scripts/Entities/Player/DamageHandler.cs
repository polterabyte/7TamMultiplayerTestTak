using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class DamageHandler : MonoBehaviour
    {
        public int heals;
        
        private SpriteRenderer _sprite;

        [Inject]
        private void Construct(GameSetup gameSetup)
        {
            heals = gameSetup.playerHeals;
        }
        
        public void TakeDamage(int damage)
        {
            heals = Math.Clamp(heals - damage, 0, int.MaxValue);
        }
    }
}