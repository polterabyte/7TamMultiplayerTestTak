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
            Container.BindInstance(gameSetup);
        }
    }

    [Serializable]
    public class GameSetup
    {
        public int version;
        public int maxPlayerInRoom = 5;
        public int serverTimeOut = 10;
        public int minimumPlayersForStartGame = 2;
    }
}