using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using STamMultiplayerTestTak.GameClientServer;
using STamMultiplayerTestTak.GameClientServer.Level;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Transform slidersContainer;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider coinSlider;

        private Vector3 _offset = new(0, 165);

        [Inject]
        private void Construct(CameraService cameraService, PlayerFacade player, ILevel levelFacade)
        {
            healthSlider.maxValue = levelFacade.InitialPlayerHeals;
            coinSlider.maxValue = levelFacade.TargetCoins;

            var ct = gameObject.GetCancellationTokenOnDestroy();
            
            UniTaskAsyncEnumerable
                .EveryValueChanged(player, p => p.Healths)
                .Subscribe(i =>
                {
                    healthSlider.value = i;
                })
                .AddTo(ct)
                ;
            UniTaskAsyncEnumerable
                .EveryValueChanged(player, p => p.Coins)
                .Subscribe(i =>
                {
                    coinSlider.value = i;
                })
                .AddTo(ct)
                ;

            gameObject
                .GetAsyncUpdateTrigger()
                .Subscribe(_ =>
                {
                    slidersContainer.transform.position = cameraService.WorldToScreenPoint(transform.position) + _offset;
                })
                .AddTo(ct)
                ;
        }
    }
}