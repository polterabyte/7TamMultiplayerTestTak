using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using STamMultiplayerTestTak.Package.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STamMultiplayerTestTak.UI
{
    public class InfoMsgBox : MonoBehaviour, IMessageBox<string>
    {
        [SerializeField] private TMP_Text info;
        [SerializeField] private Button btOk;
        public bool IsShow => gameObject.activeSelf;

        private void Start()
        {
            var ct = gameObject.GetCancellationTokenOnDestroy();

            btOk.OnClickAsAsyncEnumerable(ct)
                .Subscribe(_ =>
                {
                    Hide();
                });
        }

        public void Show()
        {
            info.text = InOutData;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool Interactable { get; set; }
        public string InOutData { get; set; }
    }
}