using System;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    [RequireComponent(typeof(PhotonView))]
    public class CoinHandler : MonoBehaviour
    {
        public int coins;

        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = PhotonView.Get(gameObject);
        }

        public void TakeCoin(int coin)
        {
            if (_photonView.IsMine)
                RPCTakeCoin(coin);
            else
                _photonView.RPC(nameof(RPCTakeCoin), RpcTarget.Others, coin);
        }
        
        [PunRPC]
        private void RPCTakeCoin(int coin)
        {
            coins = Math.Clamp(coins + coin, 0, int.MaxValue);
        }
    }
}