using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Services
{
    public class PhotonService : MonoBehaviourPunCallbacks, IPhotonService
    {
        public bool IsMasterClient => PhotonNetwork.IsMasterClient;
        public bool IsConnectedToServer => PhotonNetwork.IsConnected;
        public List<RoomInfo> Rooms { get; private set; } = new();
        public Room CurrentRoom => PhotonNetwork.CurrentRoom; 
        public List<Player> PlayersInRooms { get; } = new();
        public Player LocalPlayer => PhotonNetwork.LocalPlayer;

        private GameSetup _gameSetup;
        
        [Inject]
        private void Construct(GameSetup gameSetup)
        {
            
            _gameSetup = gameSetup;

            PhotonPeer.RegisterType(typeof(Color), (byte)IPhotonService.Events.SerializeColor, IPhotonService.SerializeColor, IPhotonService.DeserializeColor);
            
            if (!int.TryParse(PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion, out var version) || version != _gameSetup.version)
                throw new Exception($"App version must be equals PUN server seating version. {version}:{_gameSetup.version}");
        }
        
        public async UniTask ConnectedToServerAsync()
        {
            PhotonNetwork.ConnectUsingSettings();

            var breakDate = DateTime.Now + TimeSpan.FromSeconds(_gameSetup.serverTimeOut);
            while (!IsConnectedToServer && DateTime.Now <= breakDate)
            {
                await Task.Yield();
            }
        }

        public async UniTask JoinToLobbyAsync()
        {
            if (!IsConnectedToServer) return;
            
            var breakDate = DateTime.Now + TimeSpan.FromSeconds(_gameSetup.serverTimeOut);
            while (!PhotonNetwork.InLobby && DateTime.Now <= breakDate)
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
            while (PhotonNetwork.CurrentRoom == null && DateTime.Now <= breakDate)
            {
                await Task.Yield();
            }

            return PhotonNetwork.CurrentRoom != null && DateTime.Now <= breakDate;
        }

        public async UniTask JoinToRoomAsync(string name)
        {
            if (!IsConnectedToServer) return;
            
            if (!PhotonNetwork.IsMasterClient)
                PhotonNetwork.JoinRoom(name);
            

            while (PhotonNetwork.CurrentRoom == null || IsConnectedToServer && PhotonNetwork.CurrentRoom.PlayerCount < _gameSetup.minimumPlayersForStartGame)
            {
                await Task.Yield();
            }
            
            if (PhotonNetwork.CurrentRoom.PlayerCount >= _gameSetup.minimumPlayersForStartGame)
                PhotonNetwork.LoadLevel("Game");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            PlayersInRooms.Add(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayersInRooms.Remove(otherPlayer);
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Rooms = roomList; 
        }
    }
}