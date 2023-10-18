using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Photon.Pun;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerMoveController : MonoBehaviour
    {   
        [SerializeField] int kMove = 3;
        [SerializeField] int kRotate = 60;
        
        private JoystickInputService _inputService;
        
        [Inject]
        private void Construct(JoystickInputService inputService, PhotonView photonView)
        {
            if (!photonView.IsMine) return;
            
            _inputService = inputService;

            gameObject
                .GetAsyncUpdateTrigger()
                .Subscribe(_ =>
                {
                    var v = _inputService.Vertical;
                    var h = _inputService.Horizontal;
                    transform.position += transform.right * (kMove * (Time.deltaTime * v));
                    transform.Rotate(0, 0, kRotate * (Time.deltaTime * -h));
                })
                .AddTo(gameObject.GetCancellationTokenOnDestroy())
                ;
        }
    }
}