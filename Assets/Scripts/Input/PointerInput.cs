using UnityEngine;
using UnityEngine.EventSystems;

namespace BoolPyeon.Input
{
    /// <summary>
    /// 마우스와 터치를 하나의 포인터 입력으로 감싸는 정적 유틸리티.
    /// 구 Input Manager(Input.mousePosition / Input.touchCount) 기반.
    /// </summary>
    public static class PointerInput
    {
        private static Camera cachedCamera;

        public static bool Down
        {
            get
            {
                if (UnityEngine.Input.touchCount > 0)
                {
                    return UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began;
                }
                return UnityEngine.Input.GetMouseButtonDown(0);
            }
        }

        public static bool Held
        {
            get
            {
                if (UnityEngine.Input.touchCount > 0)
                {
                    TouchPhase phase = UnityEngine.Input.GetTouch(0).phase;
                    return phase == TouchPhase.Began || phase == TouchPhase.Moved || phase == TouchPhase.Stationary;
                }
                return UnityEngine.Input.GetMouseButton(0);
            }
        }

        public static bool Up
        {
            get
            {
                if (UnityEngine.Input.touchCount > 0)
                {
                    TouchPhase phase = UnityEngine.Input.GetTouch(0).phase;
                    return phase == TouchPhase.Ended || phase == TouchPhase.Canceled;
                }
                return UnityEngine.Input.GetMouseButtonUp(0);
            }
        }

        public static Vector2 ScreenPosition
        {
            get
            {
                if (UnityEngine.Input.touchCount > 0)
                {
                    return UnityEngine.Input.GetTouch(0).position;
                }
                return UnityEngine.Input.mousePosition;
            }
        }

        public static Vector3 WorldPosition(Camera cam = null)
        {
            Camera target = cam != null ? cam : GetCachedCamera();
            if (target == null)
            {
                return Vector3.zero;
            }
            return target.ScreenToWorldPoint(ScreenPosition);
        }

        public static RaycastHit2D RaycastWorld2D(Camera cam = null, bool ignoreOverUI = true)
        {
            if (ignoreOverUI && IsOverUI())
            {
                return default;
            }
            Vector3 worldPos = WorldPosition(cam);
            return Physics2D.Raycast(worldPos, Vector2.zero);
        }

        public static bool IsOverUI()
        {
            if (EventSystem.current == null)
            {
                return false;
            }
            if (UnityEngine.Input.touchCount > 0)
            {
                return EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(0).fingerId);
            }
            return EventSystem.current.IsPointerOverGameObject();
        }

        private static Camera GetCachedCamera()
        {
            if (cachedCamera == null || !cachedCamera.isActiveAndEnabled)
            {
                cachedCamera = Camera.main;
            }
            return cachedCamera;
        }
    }
}
