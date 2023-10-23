using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.GameClientServer.Level;
using STamMultiplayerTestTak.Services;

namespace STamMultiplayerTestTak.GameClientServer.Level
{
    public class LevelMatchStateEventCallbackObserver : IDisposable, IOnEventCallback
    {
        private readonly ILevel _level;

        public LevelMatchStateEventCallbackObserver(ILevel level)
        {
            _level = level;
            
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
                    _level.MatchState = state;

                    if (state == MatchStateEnum.End)
                        _level.PlayerWinActorNum = (int)data[1];
                }
            }
        }

        public void Dispose()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}