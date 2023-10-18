using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using STamMultiplayerTestTak.Entities.Player;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities
{
    public class Bullet : MonoBehaviour, IPoolable<Vector3, IMemoryPool>, IDisposable
    {
        [SerializeField] private int kSpeed = 5;
        public int damage = 5;
        
        private IMemoryPool _memoryPool;
        private Vector3 _dir;

        private CameraService _cameraService;
        
        [Inject]
        private void Construct(CameraService cameraService)
        {
            _cameraService = cameraService;
            
            var ct = gameObject.GetCancellationTokenOnDestroy();

            gameObject.GetAsyncUpdateTrigger().Subscribe(_ =>
                {
                    transform.position += _dir * (Time.deltaTime * kSpeed);

                    if (!_cameraService.CheckVisibility(transform.position))
                        Dispose();
                })
                .AddTo(ct)
                ;
            
            gameObject
                .GetAsyncTriggerEnter2DTrigger()
                .Where(x=> x.GetComponent<DamageHandler>() != null)
                .Select(collision2D => collision2D.gameObject.GetComponent<DamageHandler>())
                .Subscribe(handler =>
                {
                    handler.TakeDamage(damage);
                    Dispose();
                })
                .AddTo(ct)
                ;
            
        }
        
        public void OnDespawned()
        {
            _memoryPool = null;
        }

        public void OnSpawned(Vector3 p1, IMemoryPool p2)
        {
            _dir = p1;
            _memoryPool = p2;
        }

        public void Dispose()
        {
            _memoryPool?.Despawn(this);
        }

        public class Factory : PlaceholderFactory<Vector3, Bullet>
        {
            
        }
    }
}