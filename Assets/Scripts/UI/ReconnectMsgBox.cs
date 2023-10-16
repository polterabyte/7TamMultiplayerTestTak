using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using STamMultiplayerTestTak.Package.UI;
using UnityEngine;
using UnityEngine.UI;

namespace STamMultiplayerTestTak.UI
{
    public class ReconnectMsgBox : MonoBehaviour, IMessageBox<bool>
    {
        [SerializeField] private Button btOk, btCancel;
        public bool IsShow => gameObject.activeSelf;

        private void Start()
        {
            var ct = gameObject.GetCancellationTokenOnDestroy();

            btOk.OnClickAsAsyncEnumerable(ct)
                .Subscribe(_ =>
                {
                    InOutData = true;
                    Hide();
                });

            btCancel.OnClickAsAsyncEnumerable(ct)
                .Subscribe(_ =>
                {
                    InOutData = false;
                    Hide();
                });
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool Interactable { get; set; }
        public bool InOutData { get; set; }
    }
}