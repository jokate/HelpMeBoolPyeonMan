using UnityEngine;

namespace BoolPyeon.UI
{
    /// <summary>
    /// 기준 영역(17.78 x 10)이 항상 보이도록 orthographicSize 를 화면 비율에 맞춘다.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFit : MonoBehaviour
    {
        public const float ReferenceWidth = 17.78f;
        public const float ReferenceHeight = 10f;
        public const float MinOrthographicSize = 5f;

        private Camera targetCamera;
        private int lastScreenWidth;
        private int lastScreenHeight;

        private void Awake()
        {
            targetCamera = GetComponent<Camera>();
            Fit();
        }

        private void Update()
        {
            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                Fit();
            }
        }

        private void Fit()
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            float aspect = (float)Screen.width / Screen.height;
            float halfWidth = ReferenceWidth * 0.5f;
            targetCamera.orthographicSize = Mathf.Max(MinOrthographicSize, halfWidth / aspect);
        }
    }
}
