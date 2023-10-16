using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Installers
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PhotonService>().FromNewComponentOn(gameObject).AsSingle();
        }
    }
}