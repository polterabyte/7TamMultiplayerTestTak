using UnityEngine;

namespace STamMultiplayerTestTak.Services
{
    public class CameraService
    {
        public readonly Camera Camera;
        
        public CameraService()
        {
            Camera = Camera.main;
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPos)
        {
            return Camera.WorldToScreenPoint(worldPos);
        }
    }
}