using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.GameClientServer.Level;
using STamMultiplayerTestTak.Services;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerMatchStateObserver : IDisposable, IOnEventCallback
    {
        private readonly PlayerFacade _player;

        public PlayerMatchStateObserver(PlayerFacade player)
        {
            _player = player;
            
            PhotonNetwork.AddCallbackTarget(this);
        }
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)IPhotonService.Events.StartMatch)
            {
                var data = (object[])photonEvent.CustomData;
                var stateStr = (string)data[0];

                if (Enum.TryParse(stateStr, out MatchStateEnum state))
                {
                    _player.IsEnableControl = state == MatchStateEnum.Run;
                }
            }
        }

        public void Dispose()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}