using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.GameClientServer
{
    public class GameClient : MonoBehaviour, IOnEventCallback
    {
        private static GameClient _instance;
        
        public Transform leftSpawn, rightSpawn;

        private PlayerFacade.Factory _playerFactory;
        private IPhotonService _photonService;

        private List<PlayerFacade> _players;

        [Inject]
        private void Construct(PlayerFacade.Factory playerFactory, IPhotonService photonService)
        {
            _playerFactory = playerFactory;
            _photonService = photonService;

            _players = new List<PlayerFacade>();

            _instance = this;
        }
        
        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public static void PlacePlayer(GameObject go)
        {
            go.transform.SetParent(_instance.transform);
            var player = _instance._playerFactory.Create(go);
            _instance._players.Add(player);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)IPhotonService.Events.PlacePlayer)
            {
                var data = (object[])photonEvent.CustomData;
                var owner = (int)data[3];
                
                if (owner == _photonService.LocalPlayer.ActorNumber)
                {
                    var pos = (Vector3)data[0];
                    var rot = (Quaternion)data[1];
                    var color = (Color)data[2];
                    
                    var go = _photonService.InstantiatePlayer(pos, rot);
                    // go.transform.SetParent(transform);
                    // var player = _playerFactory.Create(go);
                    //player.Color = color;
                    // _players.Add(player);
                }
            }
        }
    }
}