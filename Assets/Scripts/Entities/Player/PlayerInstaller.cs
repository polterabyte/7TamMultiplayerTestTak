using Photon.Pun;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerInstaller : Installer<PlayerInstaller>
    {
        [InjectOptional] private IPhotonService _photonService;
        [InjectOptional] private CameraService _cameraService;

        public override void InstallBindings()
        {
            //Container.BindInstances(_player, _bullet);
            Container.BindInstances(_cameraService);
            Container.Bind<PlayerFacade>().FromNew().AsSingle();
            Container.Bind<Transform>().FromComponentOnRoot().AsSingle();
            Container.Bind<PlayerGun>().FromComponentOnRoot().AsSingle();
            Container.Bind<PhotonView>().FromComponentOnRoot().AsSingle();
            Container.Bind<PlayerFacadePunObservable>().FromComponentOnRoot().AsSingle();
            Container.Bind<DamageHandler>().FromNewComponentOnRoot().AsSingle();
            Container.Bind<CoinHandler>().FromNewComponentOnRoot().AsSingle();
            Container.Bind<PlayerMoveController>().FromNewComponentOnRoot().AsSingle().NonLazy();

            Container
                .BindFactory<Vector3, Bullet, Bullet.Factory>()
                .FromMonoPoolableMemoryPool(binder => binder.FromMethod(BulletBuilder))
                ;
        }
        private Bullet BulletBuilder(DiContainer arg)
        {
            var bullet = _photonService.InstantiateBullet().GetComponent<Bullet>();
            arg.Inject(bullet);
            return bullet;
        }
    }
}