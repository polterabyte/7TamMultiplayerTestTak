using UnityEngine;

namespace MillstonesGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Popup : MonoBehaviour, IPopup
    {
        public bool IsShow => gameObject.activeSelf;
        public virtual void Show()
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            transform.SetAsFirstSibling();
            gameObject.SetActive(false);
        }

        public bool Interactable
        {
            get => GetComponent<CanvasGroup>().interactable;
            set => GetComponent<CanvasGroup>().interactable = value;
        }
    }
}