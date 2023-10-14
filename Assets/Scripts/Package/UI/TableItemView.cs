using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace STamMultiplayerTestTak.Package.UI
{
    public abstract class TableItemView<T> : Popup, IMessageBox<T> //where T : IDatabaseData<T>
    {
        [SerializeField] private Button btOk;
        [SerializeField] private Button btCancel;
        private void Awake()
        {
            var goct = gameObject.GetCancellationTokenOnDestroy();
            btOk.OnClickAsAsyncEnumerable(goct).Subscribe(_ =>
                {
                    Hide();
                })
                ;
            btCancel.OnClickAsAsyncEnumerable(goct).Subscribe(_ =>
                {
                    InOutData = default;
                    Hide();
                })
                ;
        }
        public T InOutData { get; set; }
    }
}