using UnityEngine;

namespace BoolPyeon.Dialogue
{
    // T3: 초상화 펀치 연출의 순수 판정·곡선 계산. DialogueBox(Assembly-CSharp)에서 값만 가져다 쓴다.
    public static class PortraitPunch
    {
        public const float DefaultDuration = 0.12f;
        public const float DefaultPeak = 1.1f;

        // 이전 줄과 새 줄의 스프라이트가 둘 다 있고 서로 다를 때만 펀치한다.
        // 대화 첫 줄(prev == null)이나 초상화가 없는 줄(next == null)은 대상이 아니다.
        public static bool ShouldPunch(Sprite prev, Sprite next)
        {
            return prev != null && next != null && prev != next;
        }

        // elapsed<=0 또는 elapsed>=duration 이거나 duration<=0 이면 1(기본 크기).
        // 앞 절반: 1 → peak, 뒤 절반: peak → 1 (삼각형 곡선).
        public static float EvaluateScale(float elapsed, float duration, float peak)
        {
            if (duration <= 0f || elapsed <= 0f || elapsed >= duration)
            {
                return 1f;
            }

            float half = duration / 2f;
            if (elapsed <= half)
            {
                return Mathf.Lerp(1f, peak, elapsed / half);
            }

            return Mathf.Lerp(peak, 1f, (elapsed - half) / half);
        }
    }
}
