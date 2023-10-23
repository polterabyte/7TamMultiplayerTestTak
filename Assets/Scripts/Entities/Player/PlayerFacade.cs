using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Photon.Pun;
using STamMultiplayerTestTak.GameClientServer.Level;
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
        public bool IsEnableControl { get;  private set; }
        public int ActorNum => _photonView.ControllerActorNr;

        private DamageHandler _damageHandler;
        private CoinHandler _coinHandler;
        private SpriteRenderer _sprite;
        private PhotonView _photonView;
        
        private Color _initColor;
        private bool _blinkFlag;

        [Inject]
        private void Construct(
            ILevel level,
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
            _photonView = photonView;
            _initColor = color;

            _sprite.color = _initColor;
            
            if (!photonView.ObservedComponents.Contains(this))
                photonView.ObservedComponents.Add(this);

            var ct = gameObject.GetCancellationTokenOnDestroy();
            
            UniTaskAsyncEnumerable
                .EveryValueChanged(level, l => l.MatchState)
                .Subscribe(i =>
                {
                    IsEnableControl = i == MatchStateEnum.Run;
                })
                .AddTo(ct)
                ;
            
            UniTaskAsyncEnumerable
                    .EveryValueChanged(this, facade => facade.Healths)
                    .Subscribe(i =>
                    {
                        Blink();
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
            }
            else
            {
                _coinHandler.coins = (int)stream.ReceiveNext();
                _damageHandler.heals = (int)stream.ReceiveNext();
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