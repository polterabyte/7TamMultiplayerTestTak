using STamMultiplayerTestTak.Services;
using Zenject;

namespace STamMultiplayerTestTak
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PhotonService>().FromNewComponentOn(gameObject).AsSingle();
        }
    }
}