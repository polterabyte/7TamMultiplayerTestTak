using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.GameClientServer.Level;
using STamMultiplayerTestTak.Services;
using UnityEngine;

namespace STamMultiplayerTestTak.GameClientServer.Server
{
    public abstract class BaseGameServer : IServer, IDisposable, IOnEventCallback
    {
        protected readonly IPhotonService PhotonService;
        protected readonly GameSetup GameSetup;
        protected readonly LevelFacade Level;
        protected BaseGameServer(IPhotonService photonService, LevelFacade.Factory leveFactory, GameSetup gameSetup)
        {
            PhotonService = photonService;
            GameSetup = gameSetup;
            Level = leveFactory.Create();
            
            PhotonNetwork.AddCallbackTarget(this);
        }
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)IPhotonService.Events.PlacePlayer)
            {
                var data = (object[])photonEvent.CustomData;
                var actor = (int)data[3];

                if (actor == PhotonService.LocalPlayer.ActorNumber)
                {
                    var pos = (Vector3)data[0];
                    var rot = (Quaternion)data[1];
                    var color = (Color)data[2];
                    
                    PhotonNetwork.Instantiate(GameSetup.playerPrefabName, pos, rot, 0, new object[]{ color });
                }
            }
        }

        public virtual void Dispose()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}