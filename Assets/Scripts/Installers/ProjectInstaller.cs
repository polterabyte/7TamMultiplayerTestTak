using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Installers
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Debug.Log("!");
        }
    }
}