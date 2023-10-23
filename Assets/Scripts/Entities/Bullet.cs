using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Photon.Pun;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities
{
    [RequireComponent(typeof(PhotonView))]
    public class Bullet : MonoBehaviour, IDisposable
    {
        [SerializeField] private int kSpeed = 5;
        public int damage = 5;

        public Vector3 direction;

        private PhotonView _photonView;
        private void Awake()
        {
            var rb = GetComponent<Rigidbody2D>();
            var go = gameObject;
            var ct = go.GetCancellationTokenOnDestroy();
            
            _photonView = PhotonView.Get(go);
            
            go.GetAsyncUpdateTrigger().Subscribe(_ =>
                {
                    var dir = new Vector2 { x = direction.x, y = direction.y };
                    rb.velocity = dir * kSpeed;
                })
                .AddTo(ct)
                ;

            go
                .GetAsyncCollisionEnter2DTrigger()
                .Subscribe(c =>
                {
                    if (c.gameObject.GetComponent<DamageHandler>() != null)
                        c.gameObject.GetComponent<DamageHandler>().TakeDamage(damage);
                    
                    Dispose();
                })
                .AddTo(ct)
                ;
            
        }

        public void Dispose()
        {
            if (_photonView.IsMine)
                RPCDispose();
            else
                _photonView.RPC(nameof(RPCDispose), RpcTarget.Others);
        }
        
        [PunRPC]
        private void RPCDispose()
        {
            PhotonNetwork.Destroy(gameObject);
        }

        public class Factory : PlaceholderFactory<Vector3, Vector3, Bullet>
        {
        }
    }
}