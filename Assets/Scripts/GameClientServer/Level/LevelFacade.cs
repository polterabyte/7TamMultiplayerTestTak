using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using STamMultiplayerTestTak.Entities;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.Services;
using STamMultiplayerTestTak.UI;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.GameClientServer.Level
{
    public class LevelFacade : MonoBehaviour, IInitializable, ILevel
    {
        [SerializeField] public Transform leftSpawn, rightSpawn;
        [SerializeField] private BoxCollider2D area;
        [SerializeField] private List<BoxCollider2D> saveArea;
        
        
        private static LevelFacade _instance;
        private PlayerFacade.Factory _playerFactory;
        private Coin.MemoryPool _coinMemoryPool;
        private GameSetup _gameSetup;
        
        public List<PlayerFacade> players;

        private STamMultiplayerTestTak.Package.UI.Screen _screen;
        
        public IEnumerable<PlayerFacade> Players => players;
        public int PlayerWinActorNum { get; set; } = -1;
        public int InitialPlayerHeals => _gameSetup.playerHeals;
        public int TargetCoins => _gameSetup.coinsCountOnSession / 2;
        public int AreCoins => _coinMemoryPool.NumActive;
        public MatchStateEnum MatchState { get; set; }
        public IEnumerable<Vector3> SpawnPositions { get; private set; }

        [Inject]
        private void Construct(
            PlayerFacade.Factory playerFactory, 
            Coin.MemoryPool coinMemoryPool,
            GameSetup gameSetup,
            STamMultiplayerTestTak.Package.UI.Screen screen
            )
        {
            _playerFactory = playerFactory;
            _coinMemoryPool = coinMemoryPool;
            _gameSetup = gameSetup;
            _screen = screen;

            SpawnPositions = new[] { leftSpawn.position, rightSpawn.position };

            players = new List<PlayerFacade>();

            _instance = this;
        }
        
        public void Initialize()
        {
            _screen.Get<GameMainSite>().ObserveLevel(this);
        }
        
        public void CreateCoins(int coinsCount)
        {
            var size = area.size;
            var zero = area.offset - size / 2;
            var rect = new Rect(zero, size);

            for (var i = 0; i < coinsCount; i++)
            {
                var pos = new Vector3
                {
                    x = Random.Range(-rect.x, rect.x),
                    y = Random.Range(-rect.y, rect.y),
                };

                if (saveArea.All(x =>
                    {
                        var s = x.size;
                        var z = x.offset - s / 2;
                        var r = new Rect(z, s);
                        return !r.Contains(pos);
                    }))
                    _coinMemoryPool.Spawn(pos);
                else
                    i--;
            }
        }

        public static void PlacePlayer(GameObject go, bool isLocal, Color color)
        {
            go.transform.SetParent(_instance.transform);
            var player = _instance._playerFactory.Create(go, isLocal, color);
            _instance.players.Add(player);
        }

        public class Factory : PlaceholderFactory<ILevel>
        {
            
        }
    }
}