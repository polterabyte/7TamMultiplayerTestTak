using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Photon.Realtime;

namespace STamMultiplayerTestTak.Services
{
    public interface IPhotonService
    {
        public bool IsConnectedToServer { get; }
        public List<RoomInfo> Rooms { get; }
        public UniTask ConnectedToServerAsync();
        public UniTask<bool> TryCreateRoomAsync(string name);
        public UniTask JoinToRoomAsync(string name);
    }
}