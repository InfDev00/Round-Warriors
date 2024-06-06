using UnityEngine;

namespace Utils
{
    public class CameraResolution : MonoBehaviour
    {
        private void Awake()
        {
            var cam = GetComponent<Camera>();
            var viewportRect = cam.rect;
            
            var screenAspectRatio = (float)Screen.width / Screen.height;
            const float targetAspectRatio = 16f / 9f; // 원하는 고정 비율 설정 (예: 16:9)
            
            if (screenAspectRatio < targetAspectRatio)
            {
                viewportRect.height = screenAspectRatio / targetAspectRatio;
                viewportRect.y = (1f - viewportRect.height) / 2f;
            }
            else
            {
                viewportRect.width = targetAspectRatio / screenAspectRatio;
                viewportRect.x = (1f - viewportRect.width) / 2f;
            }
            
            cam.rect = viewportRect;
        }
    }
}