using STamMultiplayerTestTak.Entities;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak
{
    public class GameSceneContextInstaller : MonoInstaller
    {
        [SerializeField] private GameObject playerPrefab;
        
        [SerializeField] private GameField gameField;
        [SerializeField] private JoystickInputService joystick;

        [InjectOptional] private IPhotonService _photonService;
        public override void InstallBindings()
        {
            Container.Bind<JoystickInputService>().FromInstance(joystick).AsSingle();
            Container.Bind<CameraService>().AsSingle();
            Container
                .BindFactory<Vector3, Quaternion, Transform, int, PlayerFacade, PlayerFacade.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<PlayerInstaller>(playerPrefab)//Installer<PlayerInstaller>()
                ;

            Container
                .BindFactory<Vector3, Coin, Coin.Factory>()
                .FromMethod(Method)
                ;
            Container.BindInterfacesTo<GameField>().FromInstance(gameField).AsSingle();
        }

        private Coin Method(DiContainer arg1, Vector3 arg2)
        {
            var coin =_photonService.InstantiateCoin().GetComponent<Coin>();
            coin.transform.position = arg2;
            arg1.Inject(coin);
            return coin;
        }
    }
}