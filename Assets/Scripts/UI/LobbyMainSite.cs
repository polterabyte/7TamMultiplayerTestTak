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
using Screen = STamMultiplayerTestTak.Package.UI.Screen;

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
        private Screen _screen;
        
        public bool IsShow => gameObject.activeSelf;
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        [Inject]
        private void Construct(IPhotonService photonService, Screen screen)
        {
            _photonService = photonService;
            _screen = screen;

            var ct = gameObject.GetCancellationTokenOnDestroy();
            UniTaskAsyncEnumerable
                .EveryValueChanged(_photonService, service => service.Rooms)
                .Subscribe(RefreshRoomList)
                .AddTo(ct)
                ;
            
            UniTaskAsyncEnumerable
                .EveryValueChanged(_photonService, service => service.IsConnectedToServer)
                .Subscribe(async b =>
                {
                    if (b) return;
                    await Reconnect();
                })
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
                .Subscribe(async _=>
                {
                    var name = roomName.text;
                    screen.ShowSplashScreen<AwaitCreateRoomSplashScreen>();
                    var result = await _photonService.TryCreateRoomAsync(name);
                    if (result)
                    {
                        JoinToRoom(name);
                    }
                    else
                    {
                        screen.HideSplashScreen();
                        screen.ShowMessageBox<InfoMsgBox, string>("ROOM NOT CREATED !!");
                    }
                })
                ;
        }

        private void RefreshRoomList(List<RoomInfo> roomList)
        {
            foreach (var room in _rooms)
            {
                Destroy(room);
            }
            
            _rooms.Clear();

            foreach (var room in roomList)
            {
                var roomView = Instantiate(roomItemViewPrefab, roomItemViewContainer);
                
                roomView.Init(room, () => JoinToRoom(room.Name));
            }
        }

        private async UniTask Reconnect()
        {
            _screen.HideSplashScreen();
            var result = await _screen.ShowMessageBoxAsync<ReconnectMsgBox, bool>();
            if (result)
            {
                _screen.ShowSplashScreen<AwaitConnectingSplashScreen>();
                await _photonService.ConnectedToServerAsync();
                _screen.HideSplashScreen();

                if (!_photonService.IsConnectedToServer)
                    await Reconnect();
            }
            else
            {
                App.Quit();
            }
        }

        private async void JoinToRoom(string roomName)
        {
            _screen.ShowSplashScreen<AwaitOtherPlayerSplashScreen>();
            await _photonService.JoinToRoomAsync(roomName);
            _screen.HideSplashScreen();
        }
    }
}