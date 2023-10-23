using STamMultiplayerTestTak.GameClientServer.Level;
using STamMultiplayerTestTak.Services;
using UnityEngine;

namespace STamMultiplayerTestTak.GameClientServer.Server
{
    public class RemoteGameServer : BaseGameServer
    {
        public RemoteGameServer(IPhotonService photonService, LevelFacade.Factory leveFactory, GameSetup gameSetup) : base(photonService, leveFactory, gameSetup)
        {
        }
    }
}