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
    public class Bullet : MonoBehaviour, IDisposable
    {
        [SerializeField] private int kSpeed = 5;
        public int damage = 5;

        public Vector3 direction;
        //private CameraService _cameraService;
        //private PhotonView _photonView;

        private void Awake()
        {
            //_photonView = PhotonView.Get(gameObject);

            var rb = GetComponent<Rigidbody2D>();
            var ct = gameObject.GetCancellationTokenOnDestroy();

            gameObject.GetAsyncUpdateTrigger().Subscribe(_ =>
                {
                    var dir = new Vector2() { x = direction.x, y = direction.y };
                    rb.velocity = dir * kSpeed;
                    //transform.position += direction * (Time.deltaTime * kSpeed);

                    // if (!_cameraService.CheckVisibility(transform.position))
                    //     Dispose();
                })
                .AddTo(ct)
                ;
            
            gameObject
                .GetAsyncCollisionEnter2DTrigger()
                //.Where(x=> x.gameObject.GetComponent<DamageHandler>() != null)
                //.Select(collision2D => collision2D.gameObject.GetComponent<DamageHandler>())
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
            PhotonNetwork.Destroy(gameObject);
        }

        public class Factory : PlaceholderFactory<Vector3, Vector3, Bullet>
        {
        }
    }
}