using STamMultiplayerTestTak.Entities;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.GameClientServer;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace STamMultiplayerTestTak
{
    public class GameSceneContextInstaller : MonoInstaller
    {
        private const string CoinsContainerName = "coins";

        [SerializeField] private GameClient gameClient;
        [SerializeField] private JoystickInputService joystick;

        [InjectOptional] private IPhotonService _photonService;
        public override void InstallBindings()
        {
            Container.Bind<JoystickInputService>().FromInstance(joystick).AsSingle();
            Container.Bind<CameraService>().FromNew().AsSingle();
            Container
                .BindFactory<GameObject, PlayerFacade, PlayerFacade.Factory>()
                .FromSubContainerResolve()
                .ByInstaller<PlayerInstaller>()
                ;

            Container
                .BindFactory<Vector3, Coin, Coin.Factory>()
                .FromMethod(Method)
                ;

            Container.BindInterfacesAndSelfTo<GameClient>().FromInstance(gameClient).AsSingle();
            
            if (_photonService.IsMasterClient)
            {
                Container
                    .BindInterfacesAndSelfTo<GameServer>()
                    .FromNew()
                    .AsSingle()
                    .NonLazy()
                    ;
            }
        }

        private Coin Method(DiContainer arg1, Vector3 arg2)
        {
            var parent = gameClient.transform.Find(CoinsContainerName);
            if (parent == null)
                Instantiate(new GameObject(CoinsContainerName), gameClient.transform);

            var coin =_photonService.InstantiateCoin().GetComponent<Coin>();
            coin.transform.position = arg2;
            coin.transform.SetParent(parent);
            arg1.Inject(coin);
            return coin;
        }
    }
}