using System;
using UnityEngine;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class CoinHandler : MonoBehaviour
    {
        public int coins;

        public void TakeCoin(int coin)
        {
            coins = Math.Clamp(coins + coin, 0, int.MaxValue);
        }


    }
}