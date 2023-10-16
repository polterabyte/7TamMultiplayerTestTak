using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

namespace STamMultiplayerTestTak
{
    public static class App
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InitUniTaskLoop()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            Cysharp.Threading.Tasks.PlayerLoopHelper.Initialize(ref loop);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            //SystemSetups();
#if UNITY_EDITOR
            SceneManager.LoadScene("Loading");
#endif
        }

        static void SystemSetups()
        {
            Screen.SetResolution(1080, 1920, true);
            Application.targetFrameRate = -1;
            Time.fixedDeltaTime = 1 / 32f;
        }

        public static void Quit()
        {
            Application.Quit();
        }
    }

}