using System;
using UnityEngine;

namespace BoolPyeon.Battle
{
    // Utility AI 판단 요소: 0~1 입력을 커브로 배율로 바꾼다. 행동 점수 = weight × 모든 배율의 곱
    [Serializable]
    public class AIConsideration
    {
        public AIInputKind input;
        [Tooltip("TargetHasStatus 일 때 확인할 상태이상")]
        public StatusEffectType status = StatusEffectType.DamageTakenUp;
        [Tooltip("TurnsSinceUsed 일 때 이 턴 수가 지나면 입력 1")]
        [Min(1)] public int normalizeTurns = 5;
        [Tooltip("가로: 입력(0~1), 세로: 배율(0 이상)")]
        public AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        // 커브가 없거나 키가 없으면 배율 1
        public float Evaluate(float input01)
        {
            if (curve == null || curve.length == 0) return 1f;
            float v = curve.Evaluate(Mathf.Clamp01(input01));
            return v > 0f ? v : 0f;
        }
    }
}
