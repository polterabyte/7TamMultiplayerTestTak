using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerInstaller : Installer<GameObject, PlayerInstaller>
    {
        private const string BulletContainerName = "bullets";

        private readonly GameObject _gameObject;

        [InjectOptional] private IPhotonService _photonService;
        [InjectOptional] private CameraService _cameraService;
        [InjectOptional] private JoystickInputService _inputService;

        public PlayerInstaller(GameObject gameObject)
        {
            _gameObject = gameObject;
            Object.Instantiate(new GameObject(BulletContainerName), _gameObject.transform);
        }
        
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<PlayerFacade>()
                .FromNew()
                .AsSingle()
                ;
            Container.Bind<Transform>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<Rigidbody2D>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<SpriteRenderer>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<PlayerGun>().FromComponentOn(_gameObject).AsSingle().NonLazy();
            Container.Bind<PhotonView>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<PlayerFacadePunObservable>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<DamageHandler>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<CoinHandler>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<PlayerMoveController>().FromComponentOn(_gameObject).AsSingle().NonLazy();

            Container
                .BindFactory<Vector3,Vector3, Bullet, Bullet.Factory>()
                .FromMethod(Method)
                ;
            
            Container.InjectGameObject(_gameObject);
        }

        private Bullet Method(DiContainer arg1, Vector3 arg2, Vector3 arg3)
        {
            var bullet = _photonService.InstantiateBullet().GetComponent<Bullet>();
            bullet.transform.position = arg2;
            bullet.direction = arg3;
            bullet.transform.SetParent(arg1.Resolve<Transform>().Find(BulletContainerName));
            arg1.Inject(bullet);
            return bullet;
        }
    }
}