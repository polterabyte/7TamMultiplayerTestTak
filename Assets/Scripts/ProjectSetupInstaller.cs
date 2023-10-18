using System;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak
{
    [CreateAssetMenu(fileName = "GameSetupInstaller", menuName = "STamMultiplayerTestTak/Installers/GameSetupInstaller")]
    public class ProjectSetupInstaller : ScriptableObjectInstaller<ProjectSetupInstaller>
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
        public int playerHeals = 100;
        public int coinsCount = 100;
        public string playerPrefabName = "Player";
        public string coinPrefabName = "Coin";
        public string bulletPrefabName = "Bullet";
    }
}