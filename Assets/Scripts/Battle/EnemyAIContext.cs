using System;

namespace BoolPyeon.Battle
{
    // EnemyAI 가 점수를 매길 때 보는 전투 상황
    public class EnemyAIContext
    {
        public Combatant Self { get; }
        public Combatant Target { get; }
        public EnergyPool Energy { get; }
        public int Turn { get; }
        readonly Func<MonsterActionDefinition, int> lastUsedTurnOf;

        public EnemyAIContext(Combatant self, Combatant target, EnergyPool energy, int turn,
            Func<MonsterActionDefinition, int> lastUsedTurnOf = null)
        {
            Self = self;
            Target = target;
            Energy = energy;
            Turn = turn;
            this.lastUsedTurnOf = lastUsedTurnOf;
        }

        // 이 행동을 마지막으로 쓴 턴. 쓴 적 없으면 -1
        public int LastUsedTurn(MonsterActionDefinition action)
        {
            if (action == null || lastUsedTurnOf == null) return -1;
            return lastUsedTurnOf(action);
        }
    }
}
