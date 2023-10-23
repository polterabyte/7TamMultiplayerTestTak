using Photon.Pun;
using STamMultiplayerTestTak.Entities;
using STamMultiplayerTestTak.Entities.Player;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.GameClientServer.Level
{
    public class LevelInstaller : MonoInstaller<LevelInstaller>
    {
        [SerializeField] private Transform coinContainer;

        [Inject] private GameSetup _gameSetup;
        //private Transform _coinsParent;


        public override void InstallBindings()
        {
            Container
                .BindFactory<Vector3, Coin, Coin.Factory>()
                .FromMethod(Method)
                ;
            
            Container
                .BindFactory<GameObject, bool, Color, PlayerFacade, PlayerFacade.Factory>()
                .FromSubContainerResolve()
                .ByInstaller<PlayerInstaller>()
                ;
            
            Container
                .Bind<LevelFacade>()
                .FromComponentOnRoot()
                .AsSingle()
                ;
        }
        
        private Coin Method(DiContainer arg1, Vector3 arg2)
        {
            // _coinsParent = Object.Instantiate(new GameObject(CoinsContainerName), arg1.Resolve<Transform>()).transform;
            var go = PhotonNetwork.Instantiate(_gameSetup.coinPrefabName, arg2, Quaternion.identity);
            go.transform.SetParent(coinContainer);
            return arg1.InjectGameObjectForComponent<Coin>(go);
        }
    }
}