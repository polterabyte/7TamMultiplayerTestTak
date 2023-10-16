using STamMultiplayerTestTak.Package.UI;
using UnityEngine;

namespace STamMultiplayerTestTak.UI
{
    public abstract class BaseSplashScreen : MonoBehaviour, ISplashScreen
    {
        public bool IsShow => gameObject.activeSelf;
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}