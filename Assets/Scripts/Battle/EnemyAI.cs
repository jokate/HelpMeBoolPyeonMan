using System;
using System.Collections.Generic;

namespace BoolPyeon.Battle
{
    // Utility AI: 행동마다 점수(weight × 판단 요소 배율의 곱)를 내고 점수 비율대로 무작위로 고른다.
    public static class EnemyAI
    {
        public static float Score(MonsterActionDefinition action, EnemyAIContext ctx)
        {
            if (action == null || ctx == null) return 0f;
            int last = ctx.LastUsedTurn(action);
            if (action.cooldownTurns > 0 && last >= 0 && ctx.Turn - last <= action.cooldownTurns) return 0f;

            float score = Math.Max(0f, action.weight);
            if (action.considerations != null)
            {
                foreach (var c in action.considerations)
                {
                    if (c == null) continue;
                    score *= c.Evaluate(Input(c, action, ctx));
                    if (score <= 0f) return 0f;
                }
            }
            return score;
        }

        public static float Input(AIConsideration c, MonsterActionDefinition action, EnemyAIContext ctx)
        {
            switch (c.input)
            {
                case AIInputKind.SelfHpRatio:
                    return Ratio(ctx.Self);
                case AIInputKind.TargetHpRatio:
                    return Ratio(ctx.Target);
                case AIInputKind.TargetHasStatus:
                    return ctx.Target != null && ctx.Target.HasStatus(c.status) ? 1f : 0f;
                case AIInputKind.TargetGuarding:
                    return ctx.Target != null && ctx.Target.IsGuarding ? 1f : 0f;
                case AIInputKind.PlayerEnergyRatio:
                    return ctx.Energy != null && ctx.Energy.Cap > 0 ? Clamp01((float)ctx.Energy.Current / ctx.Energy.Cap) : 0f;
                case AIInputKind.TurnsSinceUsed:
                    int last = ctx.LastUsedTurn(action);
                    if (last < 0) return 1f;
                    return Clamp01((float)(ctx.Turn - last) / Math.Max(1, c.normalizeTurns));
                default:
                    return 0f;
            }
        }

        // randomness 0: 최고점, 1: 점수 비율 룰렛, 그 사이: 점수^(1/r) 룰렛. 모두 0점이면 null
        public static MonsterActionDefinition Pick(IList<MonsterActionDefinition> actions, EnemyAIContext ctx,
            Random rng, float randomness)
        {
            if (actions == null || actions.Count == 0) return null;
            var scores = new float[actions.Count];
            float max = 0f;
            int best = -1;
            for (int i = 0; i < actions.Count; i++)
            {
                scores[i] = Score(actions[i], ctx);
                if (scores[i] > max) { max = scores[i]; best = i; }
            }
            if (best < 0) return null;
            if (randomness <= 0f || rng == null) return actions[best];

            double exponent = randomness >= 1f ? 1.0 : 1.0 / randomness;
            double total = 0;
            var w = new double[scores.Length];
            for (int i = 0; i < scores.Length; i++)
            {
                w[i] = scores[i] > 0f ? Math.Pow(scores[i] / max, exponent) : 0.0; // max 로 나눠 오버플로 방지
                total += w[i];
            }
            if (total <= 0) return actions[best];

            double roll = rng.NextDouble() * total;
            for (int i = 0; i < w.Length; i++)
            {
                if (w[i] <= 0) continue;
                roll -= w[i];
                if (roll < 0) return actions[i];
            }
            return actions[best];
        }

        static float Ratio(Combatant c) => c != null && c.MaxHp > 0 ? Clamp01((float)c.Hp / c.MaxHp) : 0f;
        static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);
    }
}
