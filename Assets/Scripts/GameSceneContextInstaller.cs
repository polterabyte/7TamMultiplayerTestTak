using STamMultiplayerTestTak.Entities;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.GameClientServer;
using STamMultiplayerTestTak.GameClientServer.Level;
using STamMultiplayerTestTak.GameClientServer.Server;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace STamMultiplayerTestTak
{
    public class GameSceneContextInstaller : MonoInstaller
    {
        [SerializeField] private JoystickInputService joystick;

        [Inject] private IPhotonService _photonService;

        [InjectOptional] private string _patchToSelectedLevelPrefab = "Prefabs/Level 1";
        
        public override void InstallBindings()
        {
            Container.Bind<JoystickInputService>().FromInstance(joystick).AsSingle();
            Container.Bind<CameraService>().FromNew().AsSingle();
            
            Container
                .BindFactory<LevelFacade, LevelFacade.Factory>()
                .FromSubContainerResolve()
                .ByNewContextPrefabResource(_patchToSelectedLevelPrefab)
                ;
            
            if (_photonService.IsMasterClient)
            {
                Container
                    .BindInterfacesAndSelfTo<MasterGameServer>()
                    .FromNew()
                    .AsSingle()
                    .NonLazy()
                    ;
            }
            else
            {
                Container
                    .BindInterfacesAndSelfTo<RemoteGameServer>()
                    .FromNew()
                    .AsSingle()
                    .NonLazy()
                    ;
            }
        }
    }
}