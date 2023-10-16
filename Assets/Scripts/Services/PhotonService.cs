using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace STamMultiplayerTestTak.Services
{
    public class PhotonService : MonoBehaviourPunCallbacks, IPhotonService
    {
        public bool IsConnectedToServer { get; private set; }
        public List<RoomInfo> Rooms { get; private set; } = new();

        public void ConnectedToServer()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public bool TryCreateRoom(string name)
        {
            if (!IsConnectedToServer) return false;

            if (Rooms.Any(x => x.Name == name)) return false;

            var roomOption = new RoomOptions
            {
                MaxPlayers = 5
            };
            PhotonNetwork.CreateRoom(name, roomOption);

            return true;
        }

        public override void OnConnectedToMaster()
        {
            IsConnectedToServer = true;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            IsConnectedToServer = false;
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Rooms = roomList;
        }
    }
}