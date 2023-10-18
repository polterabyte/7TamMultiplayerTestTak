using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Photon.Realtime;
using UnityEngine;

namespace STamMultiplayerTestTak.Services
{
    public interface IPhotonService
    {
        public enum Events : byte
        {
            OnPlayerAddToRoom = 0,
        }
        public bool IsMasterClient { get; }
        public bool IsConnectedToServer { get; }
        public List<RoomInfo> Rooms { get; }
        public Photon.Realtime.Player[] OtherPlayers { get; }
        public Photon.Realtime.Player LocalPlayer { get; }
        public UniTask ConnectedToServerAsync();
        public UniTask<bool> TryCreateRoomAsync(string name);
        public UniTask JoinToRoomAsync(string name);
        public GameObject InstantiatePlayer();
        public GameObject InstantiateCoin();
        public GameObject InstantiateBullet();
    }
}