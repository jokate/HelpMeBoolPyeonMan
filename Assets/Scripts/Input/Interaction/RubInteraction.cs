using System;
using UnityEngine;

namespace BoolPyeon.Input.Interaction
{
    public enum RubAxis
    {
        Horizontal,
        Vertical,
        Any
    }

    /// <summary>
    /// 문지르기형 입력. target 위에서 눌러 좌우(또는 상하)로 왕복하면
    /// 방향 반전 횟수와 누적 이동 거리를 제공한다. target 밖으로 나가도
    /// 누르고 있는 동안은 계속 센다.
    /// </summary>
    [Serializable]
    public class RubInteraction
    {
        public Transform target;
        public RubAxis axis = RubAxis.Horizontal;
        public float minStroke = 0.3f;
        public bool ignoreOverUI = true;

        public bool IsRubbing { get; private set; }
        public int DirectionChanges { get; private set; }
        public float Distance { get; private set; }

        private Vector2 lastPos;
        private Vector2 strokeStartPos;
        private int lastSign;

        public void Step(PointerSample s)
        {
            if (target == null)
            {
                return;
            }

            if (!IsRubbing && s.down)
            {
                if (s.hit != null && (s.hit.transform == target || s.hit.transform.IsChildOf(target)))
                {
                    IsRubbing = true;
                    lastPos = s.world;
                    strokeStartPos = lastPos;
                    lastSign = 0;
                }
            }

            if (IsRubbing && (s.up || !s.held))
            {
                IsRubbing = false;
                return;
            }

            if (!IsRubbing)
            {
                return;
            }

            Vector2 pos = s.world;
            Distance += Vector2.Distance(pos, lastPos);
            lastPos = pos;

            Vector2 stroke = pos - strokeStartPos;
            float component = axis == RubAxis.Vertical ? stroke.y : stroke.x;
            if (axis == RubAxis.Any)
            {
                component = stroke.magnitude >= minStroke ? Mathf.Sign(Vector2.Dot(stroke, stroke)) * stroke.magnitude : 0f;
            }

            if (Mathf.Abs(component) >= minStroke)
            {
                int sign = component > 0f ? 1 : -1;
                if (lastSign != 0 && sign != lastSign)
                {
                    DirectionChanges++;
                }
                lastSign = sign;
                strokeStartPos = pos;
            }
        }

        public void Tick()
        {
            if (target == null)
            {
                return;
            }

            Step(PointerSample.FromPointerInput(ignoreOverUI));
        }

        public void Reset()
        {
            IsRubbing = false;
            DirectionChanges = 0;
            Distance = 0f;
            lastSign = 0;
        }
    }
}
