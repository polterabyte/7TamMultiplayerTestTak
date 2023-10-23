using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using STamMultiplayerTestTak.GameClientServer.Level;
using STamMultiplayerTestTak.Package.UI;
using UnityEngine;
using Zenject;
using Screen = STamMultiplayerTestTak.Package.UI.Screen;

namespace STamMultiplayerTestTak.UI
{
    public class GameMainSite : MonoBehaviour, ISite
    {
        private Screen _screen;
        private ILevel _level;
        public bool IsShow => gameObject.activeSelf;
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ObserveLevel(ILevel level)
        {
            _level = level;
            
            UniTaskAsyncEnumerable
                .EveryValueChanged(level, l => l.MatchState)
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case MatchStateEnum.Pause:
                            _screen.ShowSplashScreen<AwaitGamePauseSplashScreen>();
                            break;
                        case MatchStateEnum.Run:
                            _screen.HideSplashScreen();
                            break;
                        case MatchStateEnum.End:
                            ExitAsync();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(state), state, null);
                    }
                })
                .AddTo(gameObject.GetCancellationTokenOnDestroy())
                ;
        }

        private async void ExitAsync()
        {
            await _screen.ShowMessageBoxAsync<InfoMsgBox, string>($"WINNER PLAYER NUM {_level.PlayerWinActorNum}");
            
            App.Quit();
        }

        [Inject]
        private void Construct(Screen screen)
        {
            _screen = screen;
        }
    }
}