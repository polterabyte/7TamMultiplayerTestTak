using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace STamMultiplayerTestTak.Entities
{
    public class GameField : MonoBehaviour, IInitializable, IOnEventCallback
    {
        [SerializeField] private Transform leftSpawn, rightSpawn;

        private List<PlayerFacade> _players;
        private List<Coin> _coins;
        private PlayerFacade.Factory _playerFactory;
        private Coin.Factory _coinFactory;
        private GameSetup _gameSetup;
        private CameraService _cameraService;
        private IPhotonService _photonService;

        [Inject]
        private void Construct(GameSetup gameSetup, IPhotonService photonService, CameraService cameraService, PlayerFacade.Factory playerFactory, Coin.Factory coinFactory)
        {
            _players = new List<PlayerFacade>();
            _coins = new List<Coin>(gameSetup.coinsCount);
            _playerFactory = playerFactory;
            _coinFactory = coinFactory;
            _gameSetup = gameSetup;
            _cameraService = cameraService;
            _photonService = photonService;
            
            if (photonService.IsMasterClient)
            {
                UniTaskAsyncEnumerable
                    .EveryValueChanged(_players, list => list.Count)
                    .Where(i => i > 1)
                    .Subscribe(i =>
                    {
                        CreateCoins();
                    })
                    .AddTo(gameObject.GetCancellationTokenOnDestroy())
                    ;
            }
        }

        public void Initialize()
        {
            var pos =  _photonService.IsMasterClient ? leftSpawn.position : rightSpawn.position;
            var rot =  _photonService.IsMasterClient ? 0 : 180;
            
            var player = _playerFactory.Create(pos, (Quaternion.Euler(0, 0, rot)), transform, -1);
            _players.Add(player);
            // var raiseOpt = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            // var sendOpt = new SendOptions();
            // PhotonNetwork.RaiseEvent((byte)IPhotonService.Events.OnPlayerAddToRoom, $"{pos.x} {pos.y} {rot}", raiseOpt, sendOpt);
        }
        
                
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)IPhotonService.Events.OnPlayerAddToRoom)
            {
                var data = (object[]) photonEvent.CustomData;

                var player = _playerFactory.Create((Vector3) data[0], (Quaternion) data[1], transform, (int) data[2]);
                _players.Add(player);
            }
        }

        private void CreateCoins()
        {
            var rect = _cameraService.GetViewportRect();
            for (var i = 0; i < _gameSetup.coinsCount; i++)
            {
                var pos = new Vector3
                {
                    x = Random.Range(-rect.x, rect.x),
                    y = Random.Range(-rect.y, rect.y),
                };

                _coins.Add(_coinFactory.Create(pos));
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

    }
}