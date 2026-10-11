using System;
using UnityEngine;

namespace BoolPyeon.Input.Interaction
{
    /// <summary>
    /// 끌기형 입력. target(또는 그 자식)을 눌러 잡으면 포인터를 따라 움직이고,
    /// sampleWindow 마다 잰 이동 속도(월드 단위/초)를 Speed 로 제공한다.
    /// </summary>
    [Serializable]
    public class DragInteraction
    {
        public Transform target;
        public float sampleWindow = 0.1f;
        public bool ignoreOverUI = true;

        public bool IsDragging { get; private set; }
        public float Speed { get; private set; }

        private Vector2 sampleStartPos;
        private float sampleElapsed;

        public void Tick()
        {
            if (target == null)
            {
                return;
            }

            if (!IsDragging && PointerInput.Down)
            {
                RaycastHit2D hit = PointerInput.RaycastWorld2D(null, ignoreOverUI);
                if (hit.collider != null && (hit.transform == target || hit.transform.IsChildOf(target)))
                {
                    IsDragging = true;
                    sampleStartPos = target.position;
                    sampleElapsed = 0f;
                    Speed = 0f;
                }
            }

            if (IsDragging && (PointerInput.Up || !PointerInput.Held))
            {
                Release();
                return;
            }

            if (!IsDragging)
            {
                return;
            }

            Vector3 world = PointerInput.WorldPosition();
            target.position = new Vector3(world.x, world.y, target.position.z);

            sampleElapsed += Time.deltaTime;
            if (sampleElapsed >= sampleWindow && sampleElapsed > 0f)
            {
                Vector2 now = target.position;
                Speed = (now - sampleStartPos).magnitude / sampleElapsed;
                sampleStartPos = now;
                sampleElapsed = 0f;
            }
        }

        public void Release()
        {
            IsDragging = false;
            Speed = 0f;
            sampleElapsed = 0f;
        }
    }
}
