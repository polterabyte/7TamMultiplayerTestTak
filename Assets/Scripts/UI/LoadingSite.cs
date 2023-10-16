using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using STamMultiplayerTestTak.Package.UI;
using STamMultiplayerTestTak.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace STamMultiplayerTestTak.UI
{
    public class LoadingSite : MonoBehaviour, ISite
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text info;
        public bool IsShow => gameObject.activeSelf;

        private IPhotonService _photonService;
        
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
            
            LoadAsync();
        }

        private async void LoadAsync()
        {
            info.text = "Fake loading operation 1";
            progressBar.value = 0.33f;
            await Task.Delay(100);
            
            info.text = "Fake loading operation 2";
            progressBar.value = 0.66f;
            await Task.Delay(100);
            
            info.text = "Connect to server";
            _photonService.ConnectedToServer();

            var breakDate = DateTime.Now + TimeSpan.FromSeconds(10);
            while (!_photonService.IsConnectedToServer || DateTime.Now <= breakDate)
            {
                await Task.Yield();
            }
            progressBar.value = 1f;

            SceneManager.LoadSceneAsync("Lobby");
        }
    }
}