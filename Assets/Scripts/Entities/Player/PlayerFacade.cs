using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.GameClientServer;
using STamMultiplayerTestTak.GameClientServer.Level;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerFacade : MonoBehaviour, IPunObservable
    {
        [SerializeField] public float reloadTimeSec = 0.5f;
        [SerializeField] public Transform gun;
        public int Healths => _damageHandler.heals;
        public int Coins => _coinHandler.coins;
        public Color Color => _sprite.color;
        public bool IsEnableControl { get; set; }

        private DamageHandler _damageHandler;
        private CoinHandler _coinHandler;
        private SpriteRenderer _sprite;
        
        private Color _initColor;
        private bool _blinkFlag;

        [Inject]
        private void Construct(
            LevelFacade levelFacade,
            SpriteRenderer spriteRenderer, 
            PhotonView photonView, 
            DamageHandler damageHandler,
            CoinHandler coinHandler,
            Color color
            )
        {
            _sprite = spriteRenderer;
            _damageHandler = damageHandler;
            _coinHandler = coinHandler;
            _initColor = color;

            _sprite.color = _initColor;
            
            if (!photonView.ObservedComponents.Contains(this))
                photonView.ObservedComponents.Add(this);

            var ct = gameObject.GetCancellationTokenOnDestroy();
            UniTaskAsyncEnumerable
                    .EveryValueChanged(this, facade => facade.Healths)
                    .Subscribe(i =>
                    {
                        if (i > 0)
                        {
                            Blink();
                            Debug.Log($"HEALS {i}");
                        }
                    })
                    .AddTo(ct)
                ;
            UniTaskAsyncEnumerable
                    .EveryValueChanged(this, facade => facade.Coins)
                    .Subscribe(i =>
                    {
                        if (i > levelFacade.TargetCoins)
                        {
                            Debug.Log($"COINS {i}");
                        }
                    })
                    .AddTo(ct)
                ;
        }
        
        private async void Blink()
        {
            _sprite.color = Color.red;
            await UniTask.Delay(300);
            _sprite.color = _initColor;
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_coinHandler.coins);
                stream.SendNext(_damageHandler.heals);
                //stream.SendNext(IPhotonService.SerializeColor(_sprite.color));
            }
            else
            {
                _coinHandler.coins = (int)stream.ReceiveNext();
                _damageHandler.heals = (int)stream.ReceiveNext();
                //_sprite.color = (Color)IPhotonService.DeserializeColor(stream.PeekNext());
            }
        }
        
        /// <summary>
        /// GameObject - префаб
        /// bool - локальныый
        /// </summary>
        public class Factory : PlaceholderFactory<GameObject, bool, Color, PlayerFacade>
        {
        }
    }
}