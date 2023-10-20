using System;
using Photon.Pun;
using STamMultiplayerTestTak.GameClientServer;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerFacadePunObservable : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
    {
        private DamageHandler _damageHandler;
        private CoinHandler _coinHandler;
        private SpriteRenderer _sprite;

        private bool _isInitialize;
        [Inject]
        private void Construct(DamageHandler damageHandler, CoinHandler coinHandler, SpriteRenderer spriteRenderer, PhotonView photonView)
        {
            _damageHandler = damageHandler;
            _coinHandler = coinHandler;
            _sprite = spriteRenderer;
            
            if (!photonView.ObservedComponents.Contains(this))
                photonView.ObservedComponents.Add(this);

            _isInitialize = true;
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!_isInitialize) return;
            
            if (stream.IsWriting)
            {
                stream.SendNext(_damageHandler.heals);
                stream.SendNext(_coinHandler.coins);
                //stream.SendNext(IPhotonService.SerializeColor(_sprite.color));
            }
            else
            {
                _damageHandler.heals = (int)stream.PeekNext();
                _coinHandler.coins = (int)stream.PeekNext();
                //_sprite.color = (Color)IPhotonService.DeserializeColor(stream.PeekNext());
            }
        }
        
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            GameClient.PlacePlayer(gameObject);
        }
    }
}