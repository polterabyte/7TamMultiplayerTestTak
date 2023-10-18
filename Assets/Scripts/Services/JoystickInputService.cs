using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace STamMultiplayerTestTak.Services
{
    public class JoystickInputService : MonoBehaviour
    {
        [SerializeField] private Joystick joystick;
        [SerializeField] private Button btFire;

        public float Vertical => joystick.Vertical;
        public float Horizontal => joystick.Horizontal;
        public Action Fire { get; set; } = () => { };

        private void Awake()
        {
            btFire
                .OnClickAsAsyncEnumerable()
                .Subscribe(_ => Fire())
                .AddTo(gameObject.GetCancellationTokenOnDestroy())
                ;
        }
    }
}