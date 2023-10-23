using System;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.GameClientServer.Level;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


namespace STamMultiplayerTestTak.GameClientServer.Server
{
    public class MasterGameServer : BaseGameServer
    {
        private readonly CancellationToken _ct = new();

        public MasterGameServer(IPhotonService photonService, LevelFacade.Factory leveFactory, GameSetup gameSetup) : base(photonService, leveFactory, gameSetup)
        {
            StartGameAsync(_ct);
        }


        private async void StartGameAsync(CancellationToken token)
        {
            while (PhotonService.PlayersInRooms.Count < GameSetup.minimumPlayersForStartGame-1 && !token.IsCancellationRequested)
            {
                await Task.Yield(); 
            }
            if (token.IsCancellationRequested) return;
            
            PlacePlayer();

            while (Level.Players.Count < GameSetup.minimumPlayersForStartGame && !token.IsCancellationRequested)
            {
                await Task.Yield();
            }
            
            Level.CreateCoins(GameSetup.coinsCountOnSession);
            
            RiseEvent_OnStartMatch(MatchStateEnum.Run);
            Level.Players.ForEach(x=> x.IsEnableControl = true);
        }

        private void PlacePlayer()
        {
            var materColor = Color.blue;
            foreach (var (_, player) in PhotonService.CurrentRoom.Players)
            {
                if (player.IsMasterClient)
                {
                    RiseEvent_OnInstallPlayer(Level.leftSpawn.position, Quaternion.identity, materColor, player.ActorNumber);
                }
                else
                {
                    var color = Random.ColorHSV();

                    while (color.Equals(materColor))
                    {
                        color = Random.ColorHSV();
                    }

                    RiseEvent_OnInstallPlayer(Level.rightSpawn.position, Quaternion.Euler(0, 0, 180), color, player.ActorNumber);
                }
            }
        }

        private static void RiseEvent_OnInstallPlayer(Vector3 pos, Quaternion rot, Color color, int actor)
        {
            Debug.Log($"RiseEvent_OnInstallPlayer {pos} {rot}");
            var content = new object[] {pos, rot, color, actor }; 
            var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)IPhotonService.Events.PlacePlayer, content, options, SendOptions.SendReliable);
        }
        
        private static void RiseEvent_OnStartMatch(MatchStateEnum state)
        {
            Debug.Log($"RiseEvent_OnStartMatch");
            var content = new object[] { state.ToString() };//{pos, rot, color, actor }; 
            var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)IPhotonService.Events.StartMatch, content, options, SendOptions.SendReliable);
        }

        public override void Dispose()
        {
            _ct.ThrowIfCancellationRequested();
            base.Dispose();
        }
    }
}