using System.Collections.Generic;

namespace BoolPyeon.Battle
{
    // 공격·방어·스킬 모두 이 한 경로로 처리한다 (D-5)
    public static class SkillResolver
    {
        public static bool CanUse(SkillDefinition skill, Combatant user, EnergyPool energy)
        {
            if (skill == null || user == null || energy == null) return false;
            if (user.IsDead || user.HasStatus(StatusEffectType.Stun)) return false;
            return energy.CanSpend(EffectiveCost(skill, user));
        }

        // T7: CostUp 이 걸린 사용자는 스킬 원가 + CostIncrease 만큼 든다. SkillDefinition 원본 값은 바꾸지 않는다.
        public static int EffectiveCost(SkillDefinition skill, Combatant user)
        {
            if (skill == null) return 0;
            int increase = user != null ? user.CostIncrease : 0;
            return skill.energyCost + increase;
        }

        public static bool TryUse(SkillDefinition skill, Combatant user, EnergyPool energy,
            IReadOnlyList<Combatant> enemies, BattleEvents events = null)
        {
            return TryUse(skill, user, energy, enemies, null, events);
        }

        public static bool TryUse(SkillDefinition skill, Combatant user, EnergyPool energy,
            IReadOnlyList<Combatant> enemies, Combatant target, BattleEvents events = null)
        {
            if (!CanUse(skill, user, energy)) return false;

            var targets = ResolveTargets(skill.targetType, user, enemies, target);
            if (skill.targetType != SkillTargetType.Self && targets.Count == 0) return false;

            energy.TrySpend(EffectiveCost(skill, user));
            events?.RaiseSkillUsed(user, skill);

            if (skill.effects == null) return true;
            foreach (var effect in skill.effects)
            {
                if (effect == null) continue;
                foreach (var t in targets)
                    Apply(effect, user, t, events);
            }
            return true;
        }

        // F-28: 모듈 효과(선제 타격)용 — 에너지 확인·소모 없이 스킬 효과만 적용한다. CanUse 검사도 생략한다
        // (죽음·기절 확인은 호출부 책임).
        public static bool TryUseFree(SkillDefinition skill, Combatant user, IReadOnlyList<Combatant> enemies,
            Combatant target, BattleEvents events = null)
        {
            if (skill == null || user == null) return false;
            var targets = ResolveTargets(skill.targetType, user, enemies, target);
            if (skill.targetType != SkillTargetType.Self && targets.Count == 0) return false;

            events?.RaiseSkillUsed(user, skill);

            if (skill.effects == null) return true;
            foreach (var effect in skill.effects)
            {
                if (effect == null) continue;
                foreach (var t in targets)
                    Apply(effect, user, t, events);
            }
            return true;
        }

        public static List<Combatant> ResolveTargets(SkillTargetType type, Combatant user,
            IReadOnlyList<Combatant> enemies, Combatant explicitTarget)
        {
            var result = new List<Combatant>();
            switch (type)
            {
                case SkillTargetType.Self:
                    result.Add(user);
                    break;
                case SkillTargetType.SingleEnemy:
                    if (explicitTarget != null && !explicitTarget.IsDead)
                    {
                        result.Add(explicitTarget);
                        break;
                    }
                    if (enemies == null) break;
                    foreach (var e in enemies)
                        if (e != null && !e.IsDead) { result.Add(e); break; }
                    break;
                case SkillTargetType.AllEnemies:
                    if (enemies == null) break;
                    foreach (var e in enemies)
                        if (e != null && !e.IsDead) result.Add(e);
                    break;
            }
            return result;
        }

        static void Apply(SkillEffect effect, Combatant user, Combatant target, BattleEvents events)
        {
            switch (effect.kind)
            {
                case SkillEffectKind.Damage:
                    int dealt = target.TakeDamage(user.ModifyOutgoingDamage(effect.amount + user.Attack));
                    events?.RaiseDamageDealt(target, dealt);
                    break;
                case SkillEffectKind.Guard:
                    target.SetGuard(true);
                    break;
                case SkillEffectKind.Heal:
                    target.Heal(effect.amount);
                    break;
                case SkillEffectKind.ApplyStatus:
                    target.AddStatus(effect.status, effect.statusTurns, effect.statusMagnitude);
                    break;
            }
        }
    }
}
