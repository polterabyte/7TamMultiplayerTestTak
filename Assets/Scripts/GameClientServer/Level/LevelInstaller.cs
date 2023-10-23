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


        public override void InstallBindings()
        {
            Container
                .BindMemoryPool<Coin, Coin.MemoryPool>()
                .FromMethod(container =>
                {
                    var go = PhotonNetwork.Instantiate(_gameSetup.coinPrefabName, Vector3.zero, Quaternion.identity);
                    go.transform.SetParent(coinContainer);
                    return container.InjectGameObjectForComponent<Coin>(go);
                })
                //.FromMethod(Method)
                ;
            
            Container
                .BindFactory<GameObject, bool, Color, PlayerFacade, PlayerFacade.Factory>()
                .FromSubContainerResolve()
                .ByInstaller<PlayerInstaller>()
                ;
            Container.Bind<LevelMatchStateEventCallbackObserver>().FromNew().AsSingle().NonLazy();

            Container
                .BindInterfacesTo<LevelFacade>()
                .FromComponentOnRoot()
                .AsSingle()
                ;
        }
        
        private Coin Method(DiContainer arg1, Vector3 arg2)
        {
            var go = PhotonNetwork.Instantiate(_gameSetup.coinPrefabName, arg2, Quaternion.identity);
            go.transform.SetParent(coinContainer);
            return arg1.InjectGameObjectForComponent<Coin>(go);
        }
    }
}