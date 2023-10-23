using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    [RequireComponent(typeof(PhotonView))]
    public class DamageHandler : MonoBehaviour
    {
        public int heals;
        
        private SpriteRenderer _sprite;
        private PhotonView _photonView;

        [Inject]
        private void Construct(int heal)
        {
            heals = heal;
            
            _photonView = PhotonView.Get(gameObject);
        }
        
        public void TakeDamage(int damage)
        {
            if (_photonView.IsMine)
                RPCTakeDamage(damage);
            else
                _photonView.RPC(nameof(RPCTakeDamage), RpcTarget.Others, damage);
        }
        [PunRPC]
        private void RPCTakeDamage(int damage)
        {
            Debug.Log("RPCTakeDamage");
            heals = Math.Clamp(heals - damage, 0, int.MaxValue);
        }
    }
}