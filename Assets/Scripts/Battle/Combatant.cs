using System;
using System.Collections.Generic;

namespace BoolPyeon.Battle
{
    public class Combatant
    {
        public const float GuardMultiplier = 0.5f;
        public const float DefaultGuardedMagnitude = 0.5f;
        public const float DefaultAttackUpMagnitude = 0.3f;

        public string Name { get; }
        public int MaxHp { get; }
        public int Hp { get; private set; }
        public int Attack { get; }
        public int Defense { get; }
        public bool IsGuarding { get; private set; }
        public bool IsDead => Hp <= 0;

        readonly List<StatusEffect> statuses = new List<StatusEffect>();
        public IReadOnlyList<StatusEffect> Statuses => statuses;

        // (대상, 실제 피해)
        public event Action<Combatant, int> Damaged;

        public Combatant(string name, int maxHp, int attack, int defense, int? startHp = null)
        {
            Name = name;
            MaxHp = Math.Max(1, maxHp);
            // T9: startHp==0 은 방 사이에 사망 상태를 그대로 넘기는 경우(RunState.CreateSquad)다.
            // 최소 1로 올려버리면 죽은 멤버가 자동으로 살아나므로 0은 그대로 허용한다.
            Hp = startHp.HasValue ? Math.Clamp(startHp.Value, 0, MaxHp) : MaxHp;
            Attack = attack;
            Defense = defense;
        }

        public static Combatant FromCharacter(CharacterDefinition def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            return new Combatant(def.displayName, def.maxHp, def.attack, def.defense);
        }

        // T1: hpMul/atkMul 은 StageDefinition 의 몬스터 배율. 기본 1f 라 기존 호출은 그대로 동작한다.
        public static Combatant FromMonster(MonsterDefinition def, float hpMul = 1f, float atkMul = 1f)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            int hp = Math.Max(1, (int)Math.Round(def.maxHp * hpMul, MidpointRounding.AwayFromZero));
            int atk = Math.Max(1, (int)Math.Round(def.attack * atkMul, MidpointRounding.AwayFromZero));
            return new Combatant(def.displayName, hp, atk, def.defense);
        }

        // 피해 = max(1, raw [- 방어력]) → DamageTakenUp ×(1+mag) → Guarded ×(1-mag) → 방어(IsGuarding) ×0.5, 각 단계 올림
        // ignoreDefense=true 면 방어력을 무시한다(T4: Magic).
        public int CalculateDamage(int raw, bool ignoreDefense = false)
        {
            double dmg = ignoreDefense ? Math.Max(1, raw) : Math.Max(1, raw - Defense);
            var up = FindStatus(StatusEffectType.DamageTakenUp);
            if (up != null)
                dmg = Math.Ceiling(dmg * (1.0 + up.Magnitude));
            var guarded = FindStatus(StatusEffectType.Guarded);
            if (guarded != null)
            {
                float mag = guarded.Magnitude > 0f ? guarded.Magnitude : DefaultGuardedMagnitude;
                dmg = Math.Ceiling(dmg * (1.0 - mag));
            }
            if (IsGuarding)
                dmg = Math.Ceiling(dmg * GuardMultiplier);
            return (int)dmg;
        }

        public int TakeDamage(int raw, bool ignoreDefense = false)
        {
            if (IsDead) return 0;
            int dmg = CalculateDamage(raw, ignoreDefense);
            int applied = Math.Min(dmg, Hp);
            Hp -= applied;
            Damaged?.Invoke(this, applied);
            return applied;
        }

        // T7: 나가는 피해에 Weaken 을 적용한다. mag<=0 이면 기본값(0.25)을 쓴다.
        public const float DefaultWeakenMagnitude = 0.25f;

        public int ModifyOutgoingDamage(int raw)
        {
            var buff = FindStatus(StatusEffectType.AttackUp);
            if (buff != null)
            {
                float bmag = buff.Magnitude > 0f ? buff.Magnitude : DefaultAttackUpMagnitude;
                raw = (int)Math.Ceiling(raw * (1.0 + bmag));
            }
            var weaken = FindStatus(StatusEffectType.Weaken);
            if (weaken == null) return raw;
            float mag = weaken.Magnitude > 0f ? weaken.Magnitude : DefaultWeakenMagnitude;
            return Math.Max(1, (int)Math.Ceiling(raw * (1.0 - mag)));
        }

        // T7: Burn — 방어력·가드를 무시하고 매 턴 시작 시 고정 피해를 준다. 없으면 0.
        public int TickBurn()
        {
            var burn = FindStatus(StatusEffectType.Burn);
            if (burn == null || IsDead) return 0;
            int dmg = Math.Max(1, (int)Math.Round(burn.Magnitude, MidpointRounding.AwayFromZero));
            int applied = Math.Min(dmg, Hp);
            Hp -= applied;
            Damaged?.Invoke(this, applied);
            return applied;
        }

        // T7: CostUp 이 있으면 커맨드 코스트에 더할 값(최소 1). 없으면 0.
        public int CostIncrease
        {
            get
            {
                var up = FindStatus(StatusEffectType.CostUp);
                if (up == null) return 0;
                return Math.Max(1, (int)up.Magnitude);
            }
        }

        public int Heal(int amount)
        {
            if (IsDead || amount <= 0) return 0;
            int healed = Math.Min(amount, MaxHp - Hp);
            Hp += healed;
            return healed;
        }

        public void SetGuard(bool guarding) => IsGuarding = guarding;

        public void AddStatus(StatusEffectType type, int turns, float magnitude)
        {
            if (turns <= 0) return;
            var existing = FindStatus(type);
            if (existing != null)
            {
                existing.RemainingTurns = Math.Max(existing.RemainingTurns, turns);
                existing.Magnitude = magnitude;
                return;
            }
            statuses.Add(new StatusEffect(type, turns, magnitude));
        }

        public bool HasStatus(StatusEffectType type) => FindStatus(type) != null;

        public StatusEffect FindStatus(StatusEffectType type)
        {
            for (int i = 0; i < statuses.Count; i++)
                if (statuses[i].Type == type) return statuses[i];
            return null;
        }

        public void EndTurn()
        {
            for (int i = statuses.Count - 1; i >= 0; i--)
            {
                statuses[i].Tick();
                if (statuses[i].IsExpired) statuses.RemoveAt(i);
            }
            IsGuarding = false;
        }
    }
}
