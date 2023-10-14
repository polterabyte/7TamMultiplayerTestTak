using Zenject;

namespace MillstonesGame.UI
{
    public class ScreenInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ISplashScreen>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<IPanel>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<IPopup>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<ISite>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<Screen>().FromNew().AsSingle().NonLazy();
        }
    }
}