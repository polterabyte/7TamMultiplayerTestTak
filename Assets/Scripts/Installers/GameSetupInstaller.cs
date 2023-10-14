using System;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Installers
{
    [CreateAssetMenu(fileName = "GameSetupInstaller", menuName = "STamMultiplayerTestTak/Installers/GameSetupInstaller")]
    public class GameSetupInstaller : ScriptableObjectInstaller<GameSetupInstaller>
    {
        [SerializeField] private GameSetup gameSetup;
        
        public override void InstallBindings()
        {
        }
    }

    [Serializable]
    public class GameSetup
    {
        public int version;
    }
}