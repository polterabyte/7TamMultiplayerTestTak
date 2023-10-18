using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerFacade : IDisposable
    {
        public int Heals => _damageHandler.heals;
        public int Coins => _coinHandler.coins;
        
        [Inject] private DamageHandler _damageHandler;
        [Inject] private CoinHandler _coinHandler;
        [Inject] private Transform _transform;
        [Inject] private PhotonView _photonView;
        
        public class Factory : PlaceholderFactory<Vector3, Quaternion, Transform, int, PlayerFacade>
        {
            public override PlayerFacade Create(Vector3 param1, Quaternion param2, Transform param3, int param4)
            {
                //var facade = base.Create(param1, param2, param3, param4);
                var facade = base.CreateInternal(new List<TypeValuePair>());
            
                facade._transform.position = param1;
                facade._transform.rotation = param2;
                facade._transform.SetParent(param3);
            
                //https://doc.photonengine.com/pun/current/gameplay/instantiation#manual_instantiation
                #region Manual Instantiation
                
                if (param4 == -1)
                {
                    if (PhotonNetwork.AllocateViewID(facade._photonView))
                    {
                        param4 = facade._photonView.ViewID;
                        var data = new object[]
                        {
                            param1, param2, param4
                        };
            
                        var raiseEventOptions = new RaiseEventOptions
                        {
                            Receivers = ReceiverGroup.Others,
                            CachingOption = EventCaching.AddToRoomCache
                        };
            
                        var sendOptions = new SendOptions
                        {
                            Reliability = true
                        };
            
                        PhotonNetwork.RaiseEvent((byte)IPhotonService.Events.OnPlayerAddToRoom, data, raiseEventOptions, sendOptions);
                    }
                    else
                    {
                        Debug.LogError("Failed to allocate a ViewId.");
            
                        //Destroy(player);
                    }
                }
                
                facade._photonView.ViewID = param4;
            
                #endregion
                
                return facade;
            }
        }

        public void Dispose()
        {
            Object.Destroy(_transform.gameObject);
        }
    }
}