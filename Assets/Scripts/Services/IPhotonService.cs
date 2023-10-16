using System.Collections.Generic;
using Photon.Realtime;

namespace STamMultiplayerTestTak.Services
{
    public interface IPhotonService
    {
        public bool IsConnectedToServer { get; }
        public List<RoomInfo> Rooms { get; }
        public void ConnectedToServer();
        public bool TryCreateRoom(string name);
    }
}