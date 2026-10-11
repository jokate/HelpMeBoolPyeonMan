using System;
using UnityEngine;

namespace BoolPyeon.Input.Interaction
{
    /// <summary>
    /// 긋기형 입력. path 로 잡은 꺾은선의 시작점 근처에서 눌러 끝점까지 끌면
    /// 완료된다. 경로 이탈 거리와 진행률을 제공한다.
    /// </summary>
    [Serializable]
    public class SwipeInteraction
    {
        public Transform[] path;
        public float startRadius = 0.5f;
        public float finishRadius = 0.5f;
        public bool ignoreOverUI = true;

        public bool IsTracing { get; private set; }
        public float MaxDeviation { get; private set; }
        public float Progress { get; private set; }
        public bool Completed { get; private set; }

        public event Action<bool, float> Finished;

        private float[] cumulative;
        private float totalLength;

        public void Step(PointerSample s)
        {
            if (path == null || path.Length < 2)
            {
                return;
            }

            EnsureLengths();

            if (!IsTracing && s.down)
            {
                if (ignoreOverUI && s.overUI)
                {
                    return;
                }
                Vector2 world = s.world;
                if (Vector2.Distance(world, path[0].position) <= startRadius)
                {
                    IsTracing = true;
                    MaxDeviation = 0f;
                    Progress = 0f;
                    Completed = false;
                }
            }

            if (!IsTracing)
            {
                return;
            }

            if (s.up || !s.held)
            {
                Vector2 world = s.world;
                bool completed = Vector2.Distance(world, path[path.Length - 1].position) <= finishRadius;
                Completed = completed;
                IsTracing = false;
                Finished?.Invoke(completed, MaxDeviation);
                return;
            }

            Vector2 pos = s.world;
            float minDist = float.MaxValue;
            float bestProgress = 0f;

            for (int i = 0; i < path.Length - 1; i++)
            {
                Vector2 a = path[i].position;
                Vector2 b = path[i + 1].position;
                Vector2 ab = b - a;
                float segLen = ab.magnitude;
                float t = segLen > 0f ? Mathf.Clamp01(Vector2.Dot(pos - a, ab) / (segLen * segLen)) : 0f;
                Vector2 closest = a + ab * t;
                float dist = Vector2.Distance(pos, closest);
                if (dist < minDist)
                {
                    minDist = dist;
                    bestProgress = totalLength > 0f ? (cumulative[i] + segLen * t) / totalLength : 0f;
                }
            }

            if (minDist > MaxDeviation)
            {
                MaxDeviation = minDist;
            }
            if (bestProgress > Progress)
            {
                Progress = bestProgress;
            }
        }

        public void Tick()
        {
            Step(PointerSample.FromPointerInput(ignoreOverUI));
        }

        public void Reset()
        {
            IsTracing = false;
            MaxDeviation = 0f;
            Progress = 0f;
            Completed = false;
        }

        private void EnsureLengths()
        {
            if (cumulative != null && cumulative.Length == path.Length)
            {
                return;
            }

            cumulative = new float[path.Length];
            totalLength = 0f;
            for (int i = 1; i < path.Length; i++)
            {
                totalLength += Vector2.Distance(path[i - 1].position, path[i].position);
                cumulative[i] = totalLength;
            }
        }
    }
}
