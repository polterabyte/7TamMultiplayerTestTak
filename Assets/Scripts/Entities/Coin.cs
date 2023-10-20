using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Photon.Pun;
using STamMultiplayerTestTak.Entities.Player;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities
{
    public class Coin : MonoBehaviour, IDisposable
    {
        private void Awake()
        {
            var ct = gameObject.GetCancellationTokenOnDestroy();
            gameObject
                .GetAsyncTriggerEnter2DTrigger()
                .Where(x=> x.GetComponent<CoinHandler>() != null)
                .Select(collision2D => collision2D.gameObject.GetComponent<CoinHandler>())
                .Subscribe(handler =>
                {
                    handler.TakeCoin(1);
                    Dispose();
                })
                .AddTo(ct)
                ;
        }
        public void Dispose()
        {
            PhotonNetwork.Destroy(gameObject);
        }
        public class Factory : PlaceholderFactory<Vector3, Coin>
        { }
    }
}