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
        [SerializeField] int kMove = 6;
        [SerializeField] int kRotate = 120;
        
        private JoystickInputService _inputService;
        private PhotonView _photonView;

        [Inject]
        private void Construct(JoystickInputService joystickInputService, PhotonView photonView, Rigidbody2D rigidbody2D)
        {
            _inputService = joystickInputService;
            _photonView = photonView;
            
            
            if (!_photonView.IsMine) return;
            
            gameObject
                .GetAsyncUpdateTrigger()
                .Subscribe(_ =>
                {
                    var v = _inputService.Vertical;
                    var h = _inputService.Horizontal;
                    var dir = new Vector2() { x = transform.right.x, y = transform.right.y };
                    rigidbody2D.velocity = dir * v * kMove;
                    //rigidbody2D.MovePosition(rigidbody2D.position + dir * (kMove * (Time.deltaTime * v)));
                    //transform.position += transform.right * (kMove * (Time.deltaTime * v));
                    transform.Rotate(0, 0, kRotate * (Time.deltaTime * -h));
                })
                .AddTo(gameObject.GetCancellationTokenOnDestroy())
                ;
        }
    }
}