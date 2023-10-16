using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using STamMultiplayerTestTak.Misc;
using STamMultiplayerTestTak.Package.UI;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using STamMultiplayerTestTak.Services;
using TMPro;
using Zenject;

namespace STamMultiplayerTestTak.UI
{
    public class LobbyMainSite : MonoBehaviour, ISite
    {
        [SerializeField] private TMP_InputField roomName;
        [SerializeField] private Button btCreateRoom;
        [SerializeField] private Transform roomItemViewContainer;
        [SerializeField] private RoomItemView roomItemViewPrefab;

        private List<RoomItemView> _rooms = new();
        private IPhotonService _photonService;
        
        public bool IsShow => gameObject.activeSelf;
        public void Show()
        {
            
        }

        public void Hide()
        {
            
        }

        [Inject]
        private void Construct(IPhotonService photonService)
        {
            _photonService = photonService;

            var ct = gameObject.GetCancellationTokenOnDestroy();
            UniTaskAsyncEnumerable
                .EveryValueChanged(_photonService, service => service.Rooms)
                .Subscribe(RefreshRoomList)
                .AddTo(ct)
                ;

            roomName
                .OnValueChangedAsAsyncEnumerable(ct)
                .Subscribe(s =>
                {
                    if (string.IsNullOrEmpty(s) || _photonService.Rooms.Any(x => x.Name == s))
                        btCreateRoom.interactable = false;
                    else
                        btCreateRoom.interactable = true;
                })
                ;
            
            btCreateRoom.interactable = false;
            btCreateRoom
                .OnClickAsAsyncEnumerable(ct)
                .Subscribe(_ =>
                {
                    _photonService.TryCreateRoom(roomName.text);
                })
                ;
        }

        private void RefreshRoomList(List<RoomInfo> roomList)
        {
            Debug.Log("RefreshRoomList");
            foreach (var room in _rooms)
            {
                Destroy(room);
            }
            
            _rooms.Clear();

            foreach (var room in roomList)
            {
                var roomView = Instantiate(roomItemViewPrefab, roomItemViewContainer);
                
                roomView.Init(room);
            }
        }
    }
}