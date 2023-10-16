using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.Installers;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Services
{
    public class PhotonService : MonoBehaviourPunCallbacks, IPhotonService
    {

        public bool IsConnectedToServer => PhotonNetwork.IsConnected;
        public List<RoomInfo> Rooms { get; private set; } = new();

        private GameSetup _gameSetup;
        
        [Inject]
        private void Construct(GameSetup gameSetup)
        {
            _gameSetup = gameSetup;
            
            if (!int.TryParse(PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion, out var version) || version != _gameSetup.version)
                throw new Exception($"App version must be equals PUN server seating version. {version}:{_gameSetup.version}");
        }
        
        public async UniTask ConnectedToServerAsync()
        {
            PhotonNetwork.ConnectUsingSettings();

            var breakDate = DateTime.Now + TimeSpan.FromSeconds(_gameSetup.serverTimeOut);
            while (!IsConnectedToServer || DateTime.Now <= breakDate)
            {
                await Task.Yield();
            }
        }

        public async UniTask<bool> TryCreateRoomAsync(string name)
        {
            if (!IsConnectedToServer) return false;

            if (Rooms.Any(x => x.Name == name)) return false;

            var roomOption = new RoomOptions
            {
                MaxPlayers = _gameSetup.maxPlayerInRoom
            };
            PhotonNetwork.CreateRoom(name, roomOption);

            var breakDate = DateTime.Now + TimeSpan.FromSeconds(_gameSetup.serverTimeOut);
            while (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Name != name || DateTime.Now <= breakDate)
            {
                await Task.Yield();
            }
            
            return true;
        }

        public async UniTask JoinToRoomAsync(string name)
        {
            if (!IsConnectedToServer || PhotonNetwork.CurrentRoom == null) return;

            if (!PhotonNetwork.IsMasterClient)
                PhotonNetwork.JoinRoom(name);
            
            while (IsConnectedToServer && PhotonNetwork.CurrentRoom.PlayerCount < _gameSetup.minimumPlayersForStartGame)
            {
                await Task.Yield();
            }
            
            if (PhotonNetwork.CurrentRoom.PlayerCount >= _gameSetup.minimumPlayersForStartGame)
                PhotonNetwork.LoadLevel("Game");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Rooms = roomList;
        }
    }
}