using System;
using System.Linq;
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

            while (Level.Players.Count() < GameSetup.minimumPlayersForStartGame && !token.IsCancellationRequested)
            {
                await Task.Yield();
            }
            
            Level.CreateCoins(GameSetup.coinsCountOnSession);
            
            RiseEvent_OnStartMatch();

            AwaitGameComplete(token);
        }

        private async void AwaitGameComplete(CancellationToken token)
        {
            while (Level.Players.Count() > 1 &&
                   Level.Players.All(x=> x.Coins < Level.TargetCoins) && 
                   Level.Players.All(x=> x.Healths > 0) && 
                   Level.AreCoins > 0 && 
                   !token.IsCancellationRequested)
            {
                await Task.Yield();
            }
            
            if (token.IsCancellationRequested)
               Dispose();
            else if (Level.Players.Count() == 1)
                RiseEvent_OnEndMatch(Level.Players.First().ActorNum);
            else if (Level.Players.Any(x=> x.Coins >= Level.TargetCoins))
                RiseEvent_OnEndMatch(Level.Players.First(x=> x.Coins >= Level.TargetCoins).ActorNum);

            RiseEvent_OnEndMatch(Level.Players.OrderByDescending(x=> x.Coins).First().ActorNum);
        }

        private void PlacePlayer()
        {
            var materColor = Color.blue;
            foreach (var (_, player) in PhotonService.CurrentRoom.Players)
            {
                if (player.IsMasterClient)
                {
                    RiseEvent_OnInstallPlayer(Level.SpawnPositions.First(), Quaternion.identity, materColor, player.ActorNumber);
                }
                else
                {
                    var color = Random.ColorHSV();

                    while (color.Equals(materColor))
                    {
                        color = Random.ColorHSV();
                    }

                    RiseEvent_OnInstallPlayer(Level.SpawnPositions.Last(), Quaternion.Euler(0, 0, 180), color, player.ActorNumber);
                }
            }
        }

        private static void RiseEvent_OnInstallPlayer(Vector3 pos, Quaternion rot, Color color, int actor)
        {
            var content = new object[] {pos, rot, color, actor }; 
            var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)IPhotonService.Events.PlacePlayer, content, options, SendOptions.SendReliable);
        }
        
        private static void RiseEvent_OnStartMatch()
        {
            var content = new object[] { MatchStateEnum.Run.ToString()};
            var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)IPhotonService.Events.StartMatch, content, options, SendOptions.SendReliable);
        }
        private static void RiseEvent_OnPauseMatch()
        {
            var content = new object[] { MatchStateEnum.Pause.ToString()};
            var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)IPhotonService.Events.StartMatch, content, options, SendOptions.SendReliable);
        }
        private static void RiseEvent_OnEndMatch(int actorWin)
        {
            var content = new object[] { MatchStateEnum.End.ToString(), actorWin};
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