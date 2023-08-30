using System;
using UnityEngine;

namespace KWC.Utils
{
    public class AspectUtility : MonoBehaviour
    {
 
        public int x;
        public int y;

        private static float _wantedAspectRatio;
        private static Camera _cam;
        private static Camera _backgroundCam;

        private void Awake()
        {
 
            _cam = GetComponent<Camera>();
            if (!_cam)
            {
                _cam = Camera.main;
            }
            if (!_cam)
            {
                Debug.LogError("No camera available");
                return;
            }
            _wantedAspectRatio = (float)x / y;
            SetCamera();
        }
 
        public static void SetCamera()
        {
            float currentAspectRatio = (float)Screen.width / Screen.height;
            // If the current aspect ratio is already approximately equal to the desired aspect ratio,
            // use a full-screen Rect (in case it was set to something else previously)
 
      
            // Floating Point Comparison
            if (Math.Abs((int)(currentAspectRatio * 100) / 100.0f - (int)(_wantedAspectRatio * 100) / 100.0f) < 0.01f)
            {
                _cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                if (_backgroundCam)
                {
                    Destroy(_backgroundCam.gameObject);
                }
 
                return;
            }
            
            // Pillarbox
            if (currentAspectRatio > _wantedAspectRatio)
            {
                var inset = 1.0f - _wantedAspectRatio / currentAspectRatio;
                //Debug.Log(new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f));
                _cam.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
            }
            // Letterbox
            else
            {
                var inset = 1.0f - currentAspectRatio / _wantedAspectRatio;
                _cam.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
            }
 
            if (!_backgroundCam)
            {
                // Make a new camera behind the normal camera which displays black; otherwise the unused space is undefined
                _backgroundCam = new GameObject("BackgroundCam", typeof(Camera)).GetComponent<Camera>();
                _backgroundCam.depth = int.MinValue;
                _backgroundCam.clearFlags = CameraClearFlags.SolidColor;
                _backgroundCam.backgroundColor = Color.black;
                _backgroundCam.cullingMask = 0;
            }
        }
 
        public static int ScreenHeight
        {
            get
            {
                return (int)(Screen.height * _cam.rect.height);
            }
        }
 
        public static int ScreenWidth
        {
            get
            {
                return (int)(Screen.width * _cam.rect.width);
            }
        }
 
        public static int XOffset
        {
            get
            {
                return (int)(Screen.width * _cam.rect.x);
            }
        }
 
        public static int YOffset
        {
            get
            {
                return (int)(Screen.height * _cam.rect.y);
            }
        }
 
        public static Rect ScreenRect
        {
            get
            {
                return new Rect(_cam.rect.x * Screen.width, _cam.rect.y * Screen.height, _cam.rect.width * Screen.width, _cam.rect.height * Screen.height);
            }
        }
 
        public static Vector3 MousePosition
        {
            get
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.y -= (int)(_cam.rect.y * Screen.height);
                mousePos.x -= (int)(_cam.rect.x * Screen.width);
                return mousePos;
            }
        }
 
        public static Vector2 GUIMousePosition
        {
            get
            {
                Vector2 mousePos = Event.current.mousePosition;
                mousePos.y = Mathf.Clamp(mousePos.y, _cam.rect.y * Screen.height, _cam.rect.y * Screen.height + _cam.rect.height * Screen.height);
                mousePos.x = Mathf.Clamp(mousePos.x, _cam.rect.x * Screen.width, _cam.rect.x * Screen.width + _cam.rect.width * Screen.width);
                return mousePos;
            }
        }
    }
}