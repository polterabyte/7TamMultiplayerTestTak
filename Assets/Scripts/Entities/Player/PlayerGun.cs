using Photon.Pun;
using STamMultiplayerTestTak.Services;
using UnityEngine;
using Zenject;

namespace STamMultiplayerTestTak.Entities.Player
{
    public class PlayerGun : MonoBehaviour
    {
        [SerializeField] private Transform gun;

        [Inject]
        private void Construct(JoystickInputService inputService, Bullet.Factory bulletFactory, PhotonView photonView)
        {
            if (!photonView.IsMine) return;
            
            inputService.Fire += () =>
            {
                var bullet = bulletFactory.Create(transform.right);
                bullet.transform.position = gun.position;
            };
        }

    }
}