using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerFacadePunObservable : MonoBehaviour, IPunObservable
    {
        private DamageHandler _damageHandler;
        private CoinHandler _coinHandler;
        
        [Inject]
        private void Construct(DamageHandler damageHandler, CoinHandler coinHandler, PhotonView photonView)
        {
            _damageHandler = damageHandler;
            _coinHandler = coinHandler;
            
            
            if (!photonView.ObservedComponents.Contains(this))
                photonView.ObservedComponents.Add(this);
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_damageHandler.heals);
                stream.SendNext(_coinHandler.coins);
            }
            else
            {
                _damageHandler.heals = (int)stream.PeekNext();
                _coinHandler.coins = (int)stream.PeekNext();
            }
        }
    }
}