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
        [SerializeField] private float reloadTimeSec = 0.5f;
        [SerializeField] private Transform gun;

        private JoystickInputService _inputService;
        private Bullet.Factory _bulletFactory;
        private PhotonView _photonView;

        private bool _reloadFlag;
        
        private List<Bullet> _bullets;

        [Inject]
        private void Construct(JoystickInputService joystickInputService, Bullet.Factory bulletFactory, PhotonView photonView)
        {
            _inputService = joystickInputService;
            _bulletFactory = bulletFactory;
            _photonView = photonView;
            
            _bullets = new List<Bullet>();

            if (!_photonView.IsMine) return;

            _inputService.Fire += Fire;
        }

        private void Fire()
        {
            if (_reloadFlag) return;

            var bullet = _bulletFactory.Create(gun.position, transform.right);
            _bullets.Add(bullet);

            StartCoroutine(Reloading());
        }

        private IEnumerator Reloading()
        {
            _reloadFlag = true;
            yield return new WaitForSeconds(reloadTimeSec);
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