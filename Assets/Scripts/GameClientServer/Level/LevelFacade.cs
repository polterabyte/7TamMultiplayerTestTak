using System.Collections.Generic;
using Photon.Pun;
using STamMultiplayerTestTak.Entities;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.GameClientServer.Level
{
    public class LevelFacade : MonoBehaviour
    {
        [SerializeField] public Transform leftSpawn, rightSpawn;

        private static LevelFacade _instance;
        private CameraService _cameraService;
        private PlayerFacade.Factory _playerFactory;
        private Coin.Factory _coinFactory;
        private GameSetup _gameSetup;
        
        private List<Coin> _coins;
        public List<PlayerFacade> Players;
        public int InitialPlayerHeals => _gameSetup.playerHeals;
        public int TargetCoins => _gameSetup.coinsCountOnSession;

        [Inject]
        private void Construct(
            PlayerFacade.Factory playerFactory, 
            Coin.Factory coinFactory,
            CameraService cameraService,
            GameSetup gameSetup
            )
        {
            _playerFactory = playerFactory;
            _coinFactory = coinFactory;
            _cameraService = cameraService;
            _gameSetup = gameSetup;

            Players = new List<PlayerFacade>();
            _coins = new List<Coin>();

            _instance = this;
        }
        
        public void CreateCoins(int coinsCount)
        {
            var rect = _cameraService.GetViewportRect();
            for (var i = 0; i < coinsCount; i++)
            {
                var pos = new Vector3
                {
                    x = Random.Range(-rect.x, rect.x),
                    y = Random.Range(-rect.y, rect.y),
                };

                _coins.Add(_coinFactory.Create(pos));
            }
        }

        public static void PlacePlayer(GameObject go, bool issLocal, Color color)
        {
            go.transform.SetParent(_instance.transform);
            var player = _instance._playerFactory.Create(go, issLocal, color);
            _instance.Players.Add(player);
        }

        public class Factory : PlaceholderFactory<LevelFacade>
        {
            
        }
    }
}