using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

namespace STamMultiplayerTestTak.Package.UI
{
    public class Screen //: IScreen
    {
        private readonly List<ISplashScreen> _splashScreens;
        private readonly List<IPopup> _popups;
        private readonly List<ISite> _sites;

        private readonly Stack<KeyValuePair<ISite, ISite>> _callSiteStack;
        private readonly Stack<KeyValuePair<IPopup, IPopup>> _callMessageBoxesStack;

        private ISplashScreen _currentSplashScreen;

        public Screen(List<ISplashScreen> splashScreens, List<IPopup> popups, List<ISite> sites)
        {
            _splashScreens = splashScreens;
            _popups = popups;
            _sites = sites;

            if (_sites.Count(x => x.IsShow) > 1)
                throw new Exception("Стартовая страница должна быть одна !");
            
            var mainSite = _sites.FirstOrDefault(x => x.IsShow);
            _callSiteStack = new Stack<KeyValuePair<ISite, ISite>>();
            if (mainSite != null)
                _callSiteStack.Push(new KeyValuePair<ISite, ISite>(null, mainSite));
            
            _callMessageBoxesStack = new Stack<KeyValuePair<IPopup, IPopup>>();
        }
        
        public void ShowMessageBox(Type popup)
        {
            var nextPopup = _popups.First(x => x.GetType() == popup);
            ShowMessageBox(nextPopup);
        }
        
        public void ShowMessageBox(IPopup nextPopup)
        {
            IPopup currentPopup = null;
            if (_callMessageBoxesStack.TryPeek(out var pair))
            {
                currentPopup = pair.Value;

                currentPopup.Interactable = false;
            }
            _callMessageBoxesStack.Push(new KeyValuePair<IPopup, IPopup>(currentPopup, nextPopup));
            
            nextPopup.Interactable = true;
            nextPopup.Show();

            var token = new CancellationToken();
            UniTaskAsyncEnumerable.EveryValueChanged(nextPopup, popup1 => popup1.IsShow)
                .Where(b => b == false)
                //.Skip(1)
                .Subscribe(_ =>
                {
                    token.ThrowIfCancellationRequested();
                    if (_callMessageBoxesStack.Count == 0) return;

                    var (backPopup, popup) = _callMessageBoxesStack.Pop();
                    popup.Interactable = false;
                    if (backPopup != null)
                        backPopup.Interactable = true;
                })
                .AddTo(token)
                ;
        }
        
        public void ShowMessageBox<T>() where T : IPopup
        {
            var msgBox = _popups.OfType<T>().First();
            ShowMessageBox(msgBox);
        }
        
        public void ShowMessageBox<T, TR>(TR data) where T : IMessageBox<TR>
        {
            var msgBox = _popups.OfType<T>().First();
            msgBox.InOutData = data;
            ShowMessageBox(msgBox);
        }
        
        public async UniTask<TR> ShowMessageBoxAsync<TR>(IMessageBox<TR> msgBox)// where T : IMessageBox<TR>
        {
            ShowMessageBox(msgBox);
            
            while (msgBox is { IsShow: true })
            {
                await UniTask.Yield();
            }

            return msgBox.InOutData;
        }
        
        public async UniTask<TR> ShowMessageBoxAsync<T, TR>() where T : IMessageBox<TR>
        {
            var msgBox = _popups.OfType<T>().First();
            ShowMessageBox(msgBox);
            
            while (msgBox is { IsShow: true })
            {
                await UniTask.Yield();
            }

            return msgBox.InOutData;
        }

        public async UniTask<TR> ShowMessageBoxAsync<T, TR>(TR data) where T : IMessageBox<TR>
        {
            var msgBox = _popups.OfType<T>().First();
            msgBox.InOutData = data;
            ShowMessageBox(msgBox);
            
            while (msgBox is { IsShow: true })
            {
                await UniTask.Yield();
            }

            return msgBox.InOutData;
        }

        public T Get<T>() where T : ISite
        {
            return (T)Get(typeof(T));
        }
        public void SwitchSite<T>() where T : ISite
        {
            SwitchSite(typeof(T));
        }

        public ISite Get(Type site)
        {
            return _sites.First(x => x.GetType() == site);
        }
        public void SwitchSite(Type site)
        {
            var nextSite = Get(site);

            if (_callSiteStack.TryPeek(out var pair))
            {
                var currentSite = pair.Value;
            
                currentSite?.Hide();
                _callSiteStack.Push(new KeyValuePair<ISite, ISite>(currentSite, nextSite));
                nextSite.Show();
            }
            else
            {
                _callSiteStack.Push(new KeyValuePair<ISite, ISite>(null, nextSite));
            }
        }

        public void ToBackSite()
        {
            if (_callSiteStack.Count == 1) return;
            
            var (backSite, currentSite) = _callSiteStack.Pop();
            currentSite.Hide();
            backSite.Show();
        }

        public void ShowSplashScreen()
        {
            ShowSplashScreen(_splashScreens.First());
        }
        public void ShowSplashScreen(ISplashScreen splashScreen)
        {
            _currentSplashScreen?.Hide();
            _currentSplashScreen = splashScreen;
            _currentSplashScreen.Show();
        }
        public void ShowSplashScreen<T>() where T : ISplashScreen
        {
            ShowSplashScreen(_splashScreens.OfType<T>().First());
        }

        public void HideSplashScreen()
        {
            _currentSplashScreen?.Hide();
        }

        public async Task ShowSplashScreenAsync(Task awaitTask)
        {
            ShowSplashScreen();
            
            await awaitTask;
            
            HideSplashScreen();
        }
        
        public async Task ShowSplashScreenAsync<T>(Task awaitTask) where T : ISplashScreen
        {
            ShowSplashScreen<T>();

            await awaitTask;
            
            HideSplashScreen();
        }
    }
}