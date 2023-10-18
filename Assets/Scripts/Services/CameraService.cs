using UnityEngine;

namespace STamMultiplayerTestTak.Services
{
    public class CameraService
    {
        private Camera _camera;
        
        public CameraService()
        {
            _camera = Camera.main;
        }

        public bool CheckVisibility(Vector3 position)
        {
            var viewPos = _camera.WorldToViewportPoint(position);
            return viewPos.x is >= 0 and <= 1 && 
                   viewPos.y is >= 0 and <= 1 && 
                   viewPos.z > 0;
        }

        public Vector2 GetViewportRect()
        {
            return _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,0));
        }
    }
}