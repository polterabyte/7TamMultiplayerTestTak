using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Photon.Realtime;
using UnityEngine;

namespace STamMultiplayerTestTak.Services
{
    public interface IPhotonService
    {
        public enum Events : byte
        {
            SerializeColor = 199,
            PlacePlayer = 198,
        }
        public bool IsMasterClient { get; }
        public bool IsConnectedToServer { get; }
        public List<RoomInfo> Rooms { get; }
        public Room CurrentRoom { get; }
        public List<Player> PlayersInRooms { get; }
        public Player LocalPlayer { get; }
        public UniTask ConnectedToServerAsync();
        public UniTask<bool> TryCreateRoomAsync(string name);
        public UniTask JoinToRoomAsync(string name);
        public GameObject InstantiatePlayer(Vector3 pos, Quaternion rot);
        public GameObject InstantiateCoin();
        public GameObject InstantiateBullet();

        public static object DeserializeColor(byte[] data)
        {
            var retVal = new Color();

            const int floatSize = sizeof(float);
            retVal.a = BitConverter.ToSingle(data, 0);
            retVal.r = BitConverter.ToSingle(data, floatSize);
            retVal.g = BitConverter.ToSingle(data, floatSize * 2);
            retVal.b = BitConverter.ToSingle(data, floatSize * 3);

            return retVal;
        }
        public static object DeserializeColor(object o)
        {
            return DeserializeColor((byte[])o);
        }

        public static byte[] SerializeColor(object o)
        {
            const int floatSize = sizeof(float);
            var color = (Color)o;
            var retVal = new byte[floatSize*4];

            BitConverter.GetBytes(color.a).CopyTo(retVal, 0);
            BitConverter.GetBytes(color.r).CopyTo(retVal, floatSize);
            BitConverter.GetBytes(color.g).CopyTo(retVal, floatSize * 2);
            BitConverter.GetBytes(color.b).CopyTo(retVal, floatSize * 3);

            return retVal;
        }

    }
}