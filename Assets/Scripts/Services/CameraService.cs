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

        public bool CheckVisibility(Vector3 position)
        {
            var viewPos = Camera.WorldToViewportPoint(position);
            return viewPos.x is >= 0 and <= 1 && 
                   viewPos.y is >= 0 and <= 1 && 
                   viewPos.z > 0;
        }

        public Vector2 GetViewportRect()
        {
            return Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,0));
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPos)
        {
            return Camera.WorldToScreenPoint(worldPos);
        }
    }
}