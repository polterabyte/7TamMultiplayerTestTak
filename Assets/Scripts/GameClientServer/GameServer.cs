using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.Entities;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace STamMultiplayerTestTak.GameClientServer
{
    public class GameServer : IDisposable
    {
        private List<Coin> _coins = new();
        private Coin.Factory _coinFactory;
        private GameSetup _gameSetup;
        private CameraService _cameraService;
        private IPhotonService _photonService;
        private GameClient _gameClient;

        private List<IDisposable> _disposables = new();
        private CancellationToken _ct = new();

        public GameServer(IPhotonService photonService, GameClient gameClient, CameraService cameraService, GameSetup gameSetup, Coin.Factory coinFactory, PlayerFacade.Factory playerFactory)
        {
            _photonService = photonService;
            _gameClient = gameClient;
            _cameraService = cameraService;
            _gameSetup = gameSetup;
            _coinFactory = coinFactory;
            
            StartGameAsync(_ct);
        }

        private async void StartGameAsync(CancellationToken token)
        {
            while (_photonService.PlayersInRooms.Count < _gameSetup.minimumPlayersForStartGame-1 && !token.IsCancellationRequested)
            {
                await Task.Yield(); 
            }
            if (token.IsCancellationRequested) return;
            
            CreateCoins();
            CreatePlayers();
        }

        private void CreateCoins()
        {
            var rect = _cameraService.GetViewportRect();
            for (var i = 0; i < _gameSetup.coinsCountOnSession; i++)
            {
                var pos = new Vector3
                {
                    x = Random.Range(-rect.x, rect.x),
                    y = Random.Range(-rect.y, rect.y),
                };

                _coins.Add(_coinFactory.Create(pos));
            }
        }

        private void CreatePlayers()
        {
            var masterPlayerColor = Color.blue;
            
            foreach (var (_, player) in _photonService.CurrentRoom.Players)
            {
                if (player.IsMasterClient)
                {
                    RiseEvent_OnInstallPlayer(_gameClient.leftSpawn.position, Quaternion.identity, masterPlayerColor, player.ActorNumber);
                }
                else
                {
                    var color = Random.ColorHSV();

                    while (color.Equals(masterPlayerColor))
                    {
                        color = Random.ColorHSV();
                    }
                    
                    RiseEvent_OnInstallPlayer(_gameClient.rightSpawn.position, (Quaternion.Euler(0, 0, 180)), color, player.ActorNumber);
                }
            }
        }
        
        private static void RiseEvent_OnInstallPlayer(Vector3 pos, Quaternion rot, Color color, int playerActorNumber)
        {
            var content = new object[] {pos, rot, color, playerActorNumber }; 
            var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)IPhotonService.Events.PlacePlayer, content, options, SendOptions.SendReliable);
        }

        public void Dispose()
        {
            _ct.ThrowIfCancellationRequested();
            _disposables.ForEach(x=> x.Dispose());
        }
    }
}