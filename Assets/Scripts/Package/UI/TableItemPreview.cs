using UnityEngine;
using UnityEngine.UI;

namespace STamMultiplayerTestTak.Package.UI
{
    public abstract class TableItemPreview<T> : MonoBehaviour
        //where T : IEquatable<T>
    {
        [SerializeField] public Button button;

        private T _ref;
        public T RefOfTableItem
        {
            get => _ref;
            set
            {
                _ref = value;
                OnSetRef();
            }
        }

        protected abstract void OnSetRef();

        // public void SetName(string n) => text.text = n;
        //
        // public void SetActionOnClick(Action onClick)
        // {
        //     button.onClick.RemoveAllListeners();
        //     
        //     button.onClick.AddListener(() => onClick?.Invoke());
        // }
        public void SetSelected(bool isSelected)
        {
            button.image.color = isSelected? Color.green : Color.gray;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}