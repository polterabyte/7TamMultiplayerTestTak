using System.Collections.Generic;
using STamMultiplayerTestTak.Entities.Player;
using UnityEngine;

namespace STamMultiplayerTestTak.GameClientServer.Level
{
    public interface ILevel
    {
        public int PlayerWinActorNum { get; set; }
        public int InitialPlayerHeals { get; }
        public int TargetCoins  { get; }
        public int AreCoins  { get; }
        public MatchStateEnum MatchState { get; set; }
        public IEnumerable<Vector3> SpawnPositions { get; }
        public IEnumerable<PlayerFacade> Players { get; }
        public void CreateCoins(int coinsCount);
    }
}