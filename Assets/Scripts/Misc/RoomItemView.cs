using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STamMultiplayerTestTak.Misc
{
    public class RoomItemView : MonoBehaviour
    {
        [SerializeField] private Button btJoin;
        [SerializeField] private TMP_Text roomName;
        [SerializeField] private TMP_Text roomInfo;

        public void Init(RoomInfo room)
        {
            roomName.text = room.Name;
            roomInfo.text = $"{room.PlayerCount} / {room.MaxPlayers}";
            
            btJoin
                .OnClickAsAsyncEnumerable()
                .Subscribe(_ =>
                {

                })
                .AddTo(gameObject.GetCancellationTokenOnDestroy())
                ;
        }
    }
}