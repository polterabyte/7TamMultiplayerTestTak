using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STamMultiplayerTestTak.Package.UI
{
    public class InfoMsgBox : Popup, IMessageBox<string>
    {
        [SerializeField] private Button btClose;
        [SerializeField] private TMP_Text message;
        private void Awake()
        {
            btClose.OnClickAsAsyncEnumerable().Subscribe(_ =>
                {
                    Hide();
                })
                .AddTo(gameObject.GetCancellationTokenOnDestroy())
                ;
        }
        public override void Show()
        {
            message.text = InOutData;
            base.Show();
        }

        public string InOutData { get; set; }
    }
}