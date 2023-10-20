using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerFacade : IDisposable
    {
        private readonly GameSetup _gameSetup;
        private readonly DamageHandler _damageHandler;
        private readonly CoinHandler _coinHandler;
        private readonly Transform _transform;
        private readonly SpriteRenderer _spriteRenderer;
        public int Heals => _damageHandler.heals;
        public int Coins => _coinHandler.coins;
        public Color Color
        {
            get => _spriteRenderer.color;
            set => _spriteRenderer.color = value;
        }


        private List<IDisposable> _disposables = new();

        private Color _initColor;
        private bool _blinkFlag;

        public PlayerFacade(
            GameSetup gameSetup, 
            DamageHandler damageHandler, 
            CoinHandler coinHandler, 
            Transform transform, 
            SpriteRenderer spriteRenderer
            )
        {
            _gameSetup = gameSetup;
            _damageHandler = damageHandler;
            _coinHandler = coinHandler;
            _transform = transform;
            _spriteRenderer = spriteRenderer;
            
            _initColor = _spriteRenderer.color;

            var a = UniTaskAsyncEnumerable
                    .EveryValueChanged(this, facade => facade.Heals)
                    .Subscribe(i =>
                    {
                        if (i > 0)
                        {
                            Blink();
                            Debug.Log($"HEALS {i}");
                        }
                    })
                ;
            var b = UniTaskAsyncEnumerable
                    .EveryValueChanged(this, facade => facade.Coins)
                    .Subscribe(i =>
                    {
                        // TODO !!!!!
                        if (i > _gameSetup.coinsCountOnSession)
                        {
                            Debug.Log($"COINS {i}");
                        }
                    })
                ;

            _disposables.AddRange(new[] { a, b });
        }

        private async void Blink()
        {
            _spriteRenderer.color = Color.red;
            await UniTask.Delay(300);
            _spriteRenderer.color = _initColor;
        }

        public void Dispose()
        {
            _disposables.ForEach(x=> x.Dispose());
            PhotonNetwork.Destroy(_transform.gameObject);
        }
        
        public class Factory : PlaceholderFactory<GameObject, PlayerFacade>
        {
        }
    }
}