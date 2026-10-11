using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Input.Interaction
{
    /// <summary>
    /// 연타형 입력. target 을 누를 때마다 1회 세고, window 초 동안의 탭 수(Count)와
    /// 탭 속도(Rate = Count / window)를 제공한다.
    /// </summary>
    [Serializable]
    public class MashInteraction
    {
        public Transform target;
        public float window = 1f;
        public bool ignoreOverUI = true;

        public int Count { get; private set; }
        public int TotalCount { get; private set; }
        public float Rate { get; private set; }

        private readonly Queue<float> taps = new Queue<float>();

        public void Step(PointerSample s)
        {
            if (target == null)
            {
                return;
            }

            if (s.down && s.hit != null && (s.hit.transform == target || s.hit.transform.IsChildOf(target)))
            {
                taps.Enqueue(s.time);
                TotalCount++;
            }

            while (taps.Count > 0 && s.time - taps.Peek() > window)
            {
                taps.Dequeue();
            }

            Count = taps.Count;
            Rate = window > 0f ? Count / window : 0f;
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
            taps.Clear();
            Count = 0;
            TotalCount = 0;
            Rate = 0f;
        }
    }
}
