using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerGun : MonoBehaviour
    {
        private JoystickInputService _inputService;
        private Bullet.Factory _bulletFactory;
        private PhotonView _photonView;
        private PlayerFacade _player;
        private bool _reloadFlag;
        
        private List<Bullet> _bullets;

        [Inject]
        private void Construct(JoystickInputService joystickInputService, Bullet.Factory bulletFactory, PhotonView photonView, PlayerFacade player)
        {
            _inputService = joystickInputService;
            _bulletFactory = bulletFactory;
            _photonView = photonView;
            _player = player;
            
            _bullets = new List<Bullet>();

            if (!_photonView.IsMine) return;

            _inputService.Fire += Fire;
        }

        private void Fire()
        {
            if (_reloadFlag || !_player.IsEnableControl) return;

            var bullet = _bulletFactory.Create(_player.gun.position, transform.right);
            _bullets.Add(bullet);

            StartCoroutine(Reloading());
        }

        private IEnumerator Reloading()
        {
            _reloadFlag = true;
            yield return new WaitForSeconds(_player.reloadTimeSec);
            _reloadFlag = false;
        }

        private void OnDestroy()
        {
            if (_inputService != null)
                _inputService.Fire -= Fire;
            StopAllCoroutines();
            foreach (var bullet in _bullets?.Where(x=> x != null)!)
            {
                PhotonNetwork.Destroy(bullet.gameObject);
            }
                
        }
    }
}