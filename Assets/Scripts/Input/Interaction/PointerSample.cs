using UnityEngine;

namespace BoolPyeon.Input.Interaction
{
    /// <summary>
    /// 한 프레임의 포인터 상태를 값으로 담은 구조체. 실제 입력(FromPointerInput)과
    /// 합성 입력(셀프체크) 양쪽에서 인터랙션의 Step()에 넣을 수 있다.
    /// </summary>
    public struct PointerSample
    {
        public bool down;
        public bool held;
        public bool up;
        public bool overUI;
        public Vector2 world;
        public float time;
        public GameObject hit;

        public static PointerSample FromPointerInput(bool ignoreOverUI = true)
        {
            RaycastHit2D raycast = PointerInput.RaycastWorld2D(null, ignoreOverUI);
            return new PointerSample
            {
                down = PointerInput.Down,
                held = PointerInput.Held,
                up = PointerInput.Up,
                overUI = PointerInput.IsOverUI(),
                world = PointerInput.WorldPosition(),
                time = Time.time,
                hit = raycast.collider != null ? raycast.collider.gameObject : null
            };
        }
    }
}
