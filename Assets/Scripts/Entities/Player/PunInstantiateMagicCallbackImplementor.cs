using System;
using Photon.Pun;
using STamMultiplayerTestTak.GameClientServer;
using STamMultiplayerTestTak.GameClientServer.Level;
using UnityEngine;

namespace STamMultiplayerTestTak.Entities.Player
{
    [RequireComponent(typeof(PhotonView))]
    public class PunInstantiateMagicCallbackImplementor : MonoBehaviour, IPunInstantiateMagicCallback
    {
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            var color = (Color)info.photonView.InstantiationData[0];
            LevelFacade.PlacePlayer(gameObject,info.photonView.IsMine, color);
        }
    }
}