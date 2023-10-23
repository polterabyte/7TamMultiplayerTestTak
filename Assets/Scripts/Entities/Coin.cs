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
    [RequireComponent(typeof(PhotonView))]
    public class Coin : MonoBehaviour, IDisposable
    {
        private PhotonView _photonView;
        private void Awake()
        {
            _photonView = PhotonView.Get(gameObject);
        }

        private void OnDisable()
        {
            if (_photonView.IsMine)
                _photonView.RPC(nameof(RPCOnDisable), RpcTarget.Others);
        }
        
        [PunRPC]
        private void RPCOnDisable()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (_photonView.IsMine)
                _photonView.RPC(nameof(RPCOnEnable), RpcTarget.Others);
        }
        
        [PunRPC]
        private void RPCOnEnable()
        {
            gameObject.SetActive(true);
        }

        public void Dispose()
        {
            PhotonNetwork.Destroy(gameObject);
        }

        public class MemoryPool : MonoMemoryPool<Vector3, Coin>
        {
            protected override void OnDestroyed(Coin item)
            {
                item.Dispose();
            }

            protected override void Reinitialize(Vector3 p1, Coin item)
            {
                item.transform.position = p1;
            }

            protected override void OnSpawned(Coin item)
            {
                if (!PhotonView.Get(item).IsMine) return;

                var ct = item.gameObject.GetCancellationTokenOnDestroy();
                item.gameObject
                    .GetAsyncTriggerEnter2DTrigger()
                    .Where(x => x.GetComponent<CoinHandler>() != null)
                    .Select(collision2D => collision2D.gameObject.GetComponent<CoinHandler>())
                    .Subscribe(handler =>
                    {
                        handler.TakeCoin(1);
                        Despawn(item);
                    })
                    .AddTo(ct)
                    ;
                
                base.OnSpawned(item);
            }
        }
    }
}