using System;
using UnityEngine;

namespace BoolPyeon.Input.Interaction
{
    /// <summary>
    /// 돌리기형 입력. center 를 기준으로 누른 채 원을 그리면 프레임 간
    /// 부호 있는 각도 차를 누적해 AccumulatedAngle(도, 반시계 +)과
    /// Turns(=|각|/360)을 제공한다.
    /// </summary>
    [Serializable]
    public class RotateInteraction
    {
        public Transform center;
        public float grabRadius = 0f;
        public bool ignoreOverUI = true;

        public bool IsRotating { get; private set; }
        public float AccumulatedAngle { get; private set; }
        public float Turns => Mathf.Abs(AccumulatedAngle) / 360f;

        private Vector2 lastDir;

        public void Step(PointerSample s)
        {
            if (center == null)
            {
                return;
            }

            if (!IsRotating && s.down)
            {
                if (ignoreOverUI && s.overUI)
                {
                    return;
                }
                Vector2 world = s.world;
                Vector2 centerPos = center.position;
                float dist = Vector2.Distance(world, centerPos);
                if (grabRadius <= 0f || dist <= grabRadius)
                {
                    if (dist > 0.1f)
                    {
                        IsRotating = true;
                        lastDir = (world - centerPos).normalized;
                    }
                }
            }

            if (IsRotating && (s.up || !s.held))
            {
                IsRotating = false;
                return;
            }

            if (!IsRotating)
            {
                return;
            }

            Vector2 pos = s.world;
            Vector2 centerPosNow = center.position;
            Vector2 dir = pos - centerPosNow;
            if (dir.magnitude <= 0.1f)
            {
                return;
            }
            dir.Normalize();

            float delta = Vector2.SignedAngle(lastDir, dir);
            AccumulatedAngle += delta;
            lastDir = dir;
        }

        public void Tick()
        {
            Step(PointerSample.FromPointerInput(ignoreOverUI));
        }

        public void Reset()
        {
            IsRotating = false;
            AccumulatedAngle = 0f;
            lastDir = Vector2.zero;
        }
    }
}
