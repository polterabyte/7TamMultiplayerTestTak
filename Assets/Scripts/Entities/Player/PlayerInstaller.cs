using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerInstaller : Installer<GameObject, bool, Color, PlayerInstaller>
    {
        private const string BulletContainerName = "bullets";

        private readonly GameObject _gameObject;
        private readonly bool _isLocal;
        private readonly Color _color;

        [Inject] private IPhotonService _photonService;
        [Inject] private GameSetup _gameSetup;

        public PlayerInstaller(GameObject gameObject, bool isLocal, Color color)
        {
            _gameObject = gameObject;
            _isLocal = isLocal;
            _color = color;
            Object.Instantiate(new GameObject(BulletContainerName), _gameObject.transform);
        }
        
        public override void InstallBindings()
        {
            Container.BindInstance(_color);
            Container.Bind<PhotonView>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<PunInstantiateMagicCallbackImplementor>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<SpriteRenderer>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<PlayerUI>().FromComponentOn(_gameObject).AsSingle().NonLazy();
            Container.Bind<PlayerFacade>().FromComponentOn(_gameObject).AsSingle();

            Container.Bind<PlayerMatchStateObserver>().FromNew().AsSingle().NonLazy();

            Container.Bind<DamageHandler>().FromNewComponentOn(_gameObject).AsSingle().WithArguments(_gameSetup.playerHeals).NonLazy();
            Container.Bind<CoinHandler>().FromNewComponentOn(_gameObject).AsSingle().NonLazy();
            
            Container
                .BindFactory<Vector3,Vector3, Bullet, Bullet.Factory>()
                .FromMethod(Method)
                ;
            
            if (_isLocal) InstallLocalPlayer();

            Container.InjectGameObject(_gameObject);
        }

        private void InstallLocalPlayer()
        {
            Container.Bind<Transform>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<Rigidbody2D>().FromComponentOn(_gameObject).AsSingle();
            Container.Bind<PlayerGun>().FromNewComponentOn(_gameObject).AsSingle().NonLazy();
            Container.Bind<PlayerMoveController>().FromNewComponentOn(_gameObject).AsSingle().NonLazy();
        }
        
        private Bullet Method(DiContainer arg1, Vector3 arg2, Vector3 arg3)
        {
            var bullet = PhotonNetwork.Instantiate(_gameSetup.bulletPrefabName, arg2, Quaternion.identity).GetComponent<Bullet>();
            bullet.direction = arg3;
            bullet.transform.SetParent(arg1.Resolve<Transform>().Find(BulletContainerName));
            arg1.Inject(bullet);
            return bullet;
        }
    }
}