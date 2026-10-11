using System;

namespace BoolPyeon.Battle
{
    public class BattleEvents
    {
        public event Action OnBattleStart;
        public event Action<int> OnTurnStart;
        public event Action OnPlayerPhaseEnd;
        public event Action OnEnemyPhaseEnd;
        public event Action<Combatant, SkillDefinition> OnSkillUsed;
        public event Action<Combatant, int> OnDamageDealt;

        public void RaiseBattleStart() => OnBattleStart?.Invoke();
        public void RaiseTurnStart(int turn) => OnTurnStart?.Invoke(turn);
        public void RaisePlayerPhaseEnd() => OnPlayerPhaseEnd?.Invoke();
        public void RaiseEnemyPhaseEnd() => OnEnemyPhaseEnd?.Invoke();
        public void RaiseSkillUsed(Combatant user, SkillDefinition skill) => OnSkillUsed?.Invoke(user, skill);
        public void RaiseDamageDealt(Combatant target, int amount) => OnDamageDealt?.Invoke(target, amount);
    }
}
