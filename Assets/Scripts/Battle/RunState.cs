using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    // F-19: 방 사이(방 클리어 강화)의 진행 상태. 씬을 재로드해도 static Current 로 유지되고,
    // 패배·재시도·타이틀로 가면 null 로 초기화한다(BattleController 가 책임진다).
    public class RunState
    {
        public static RunState Current;

        // T2: 편성 순서(리더 포함, 최대 Squad.MaxMembers). 독립 스쿼드 전투(F-37)의 기준이 된다.
        public IReadOnlyList<CharacterDefinition> Members { get; }
        // 리더(=Members[0]). 기존 코드(스킬 목록 등)는 T3 전까지 계속 리더 기준으로 동작한다.
        public CharacterDefinition Character => Members[0];

        public int BonusMaxHp { get; private set; }
        public int BonusAttack { get; private set; }
        public int BonusDefense { get; private set; }
        public int EnergyGainBonus { get; private set; }
        public int EnergyCapBonus { get; private set; }
        // F-27: 영구 강화(시작 에너지). 전투 시작 시 CreateSquad 가 에너지 풀에 즉시 채운다.
        public int StartEnergyBonus { get; private set; }
        // 방이 끝날 때 멤버별 HP 를 저장해 둔다. 항목이 없으면(null) 가득 찬 상태로 시작한다.
        public List<int?> CurrentHps { get; private set; } = new List<int?>();
        // 옛 코드/세이브 호환용 읽기 전용 — 멤버0(리더) HP.
        public int? CurrentHp => CurrentHps.Count > 0 ? CurrentHps[0] : null;

        // T2: 런 하나에 미로 하나. 씬 재로드 사이에도 RunState.Current 와 함께 유지된다.
        public StageMaze Map { get; set; }
        public int Seed { get; set; }

        // T13: 현재 스테이지 번호. 다음 스테이지가 아직 없어 메모리 전용(기본 1)으로 두고
        // 세이브 스키마는 바꾸지 않는다. RunState.Current 가 null 이 되면 함께 사라진다.
        public int StageNumber { get; set; } = 1;

        // T2: 새 런의 시작 스테이지 = min(최고 클리어+1, 최대 스테이지), 최소 1.
        public static int ComputeStartStage(int highestCleared, int maxStage)
        {
            return Mathf.Clamp(highestCleared + 1, 1, Mathf.Max(1, maxStage));
        }

        // F-21: 이번 런에서 처치한 몬스터 수와 얻은 정화 결정. 결과 패널 표시·세이브 복원용.
        public int Kills { get; private set; }
        public int CrystalsEarned { get; private set; }

        public void AddKills(int n)
        {
            if (n > 0) Kills += n;
        }

        public void AddCrystalsEarned(int n)
        {
            if (n > 0) CrystalsEarned += n;
        }

        public void RestoreProgress(int kills, int crystalsEarned)
        {
            Kills = kills;
            CrystalsEarned = crystalsEarned;
        }

        // 습득·강화한 스킬(원본을 복제한 것). 원본 SO 자산은 건드리지 않는다.
        public List<SkillDefinition> Skills { get; } = new List<SkillDefinition>();
        readonly Dictionary<SkillDefinition, SkillDefinition> originalToClone = new Dictionary<SkillDefinition, SkillDefinition>();

        public List<UpgradeDefinition> Acquired { get; } = new List<UpgradeDefinition>();

        public RunState(CharacterDefinition character)
            : this(new List<CharacterDefinition> { character != null ? character : throw new ArgumentNullException(nameof(character)) })
        {
        }

        // T2: 편성 전원(SquadService 순서, 최대 Squad.MaxMembers). null 항목은 제외한다.
        public RunState(IReadOnlyList<CharacterDefinition> squad)
        {
            var members = new List<CharacterDefinition>();
            if (squad != null)
            {
                foreach (var c in squad)
                {
                    if (c == null) continue;
                    members.Add(c);
                    if (members.Count >= Squad.MaxMembers) break;
                }
            }
            if (members.Count == 0)
                throw new ArgumentException("squad 에 유효한 멤버가 없습니다.", nameof(squad));
            Members = members;

            if (Character.skills != null)
            {
                foreach (var s in Character.skills)
                {
                    if (s == null) continue;
                    var clone = ScriptableObject.Instantiate(s);
                    Skills.Add(clone);
                    originalToClone[s] = clone;
                }
            }
        }

        // F-37/T2: 영구·방 강화(스탯·시작 에너지)는 편성 전원에게 적용한다. 리더만 강해지면
        // 독립 스쿼드에서 나머지 멤버가 버티지 못하기 때문이다.
        public Squad CreateSquad()
        {
            var squad = new Squad(EnergyPool.DefaultCap + EnergyCapBonus);
            for (int i = 0; i < Members.Count; i++)
            {
                var def = Members[i];
                int maxHp = def.maxHp + BonusMaxHp;
                int? hp = i < CurrentHps.Count ? CurrentHps[i] : (int?)null;
                var combatant = new Combatant(def.displayName, maxHp, def.attack + BonusAttack,
                    def.defense + BonusDefense, hp);
                squad.TryAdd(def, combatant);
                if (StartEnergyBonus > 0) squad.Members[i].Energy.Add(StartEnergyBonus);
            }
            return squad;
        }

        // F-27: 영구 강화(허브에서 구매) 적용. 새 런을 만들 때와 저장된 런을 복원할 때 둘 다 부른다.
        public void ApplyPermanent(PermanentUpgradeKind kind, int total)
        {
            if (total == 0) return;
            switch (kind)
            {
                case PermanentUpgradeKind.MaxHp: BonusMaxHp += total; break;
                case PermanentUpgradeKind.Attack: BonusAttack += total; break;
                case PermanentUpgradeKind.Defense: BonusDefense += total; break;
                case PermanentUpgradeKind.StartEnergy: StartEnergyBonus += total; break;
                case PermanentUpgradeKind.EnergyAbsorb: EnergyGainBonus += total; break;
            }
        }

        // T2: 편성 전원 HP 를 순서대로 기록한다.
        public void CaptureHp(Squad squad)
        {
            if (squad == null || squad.Members.Count == 0) return;
            var hps = new List<int?>();
            foreach (var m in squad.Members) hps.Add(m.Combatant.Hp);
            CurrentHps = hps;
        }

        // F-20: 저장 복원용(옛 단일 HP 세이브 호환). 멤버0만 설정하고 나머지는 가득 찬 상태(null)로 남긴다.
        public void RestoreHp(int? hp)
        {
            CurrentHps = new List<int?> { hp };
        }

        // T2: 멤버별 HP 목록을 그대로 복원한다.
        public void RestoreHps(IReadOnlyList<int?> hps)
        {
            CurrentHps = hps != null ? new List<int?>(hps) : new List<int?>();
        }

        public int ModifyEnergyGain(int gained) => gained > 0 ? gained + EnergyGainBonus : gained;

        // F-18: 휴식 노드. 기록된 멤버 전원을 각자 최대 HP 대비 pct% 회복한다.
        // 아직 전투를 한 적이 없어 기록된 HP 가 없으면 무시한다(가득 찬 상태로 시작할 것이므로).
        public void HealPercent(int pct)
        {
            for (int i = 0; i < CurrentHps.Count; i++)
            {
                if (!CurrentHps[i].HasValue) continue;
                if (i >= Members.Count) continue;
                int maxHp = Members[i].maxHp + BonusMaxHp;
                int amount = maxHp * pct / 100;
                CurrentHps[i] = Math.Min(CurrentHps[i].Value + amount, maxHp);
            }
        }

        public bool IsApplicable(UpgradeDefinition def)
        {
            if (def == null) return false;
            switch (def.kind)
            {
                case UpgradeKind.LearnSkill:
                    return def.skill != null && !originalToClone.ContainsKey(def.skill);
                case UpgradeKind.SkillPower:
                    return Skills.Count > 0;
                default:
                    return true;
            }
        }

        public void Apply(UpgradeDefinition def)
        {
            if (def == null) return;
            switch (def.kind)
            {
                case UpgradeKind.Hp:
                    BonusMaxHp += def.value;
                    // T9: 사망(HP 0) 멤버는 회복하지 않는다 — 부활 수단은 휴식(HealPercent)만 허용한다.
                    // 최대 HP 보너스(BonusMaxHp)는 사망 멤버에게도 그대로 적용된다(다음에 부활하면 반영).
                    for (int i = 0; i < CurrentHps.Count; i++)
                    {
                        if (!CurrentHps[i].HasValue || i >= Members.Count) continue;
                        if (CurrentHps[i].Value <= 0) continue;
                        int maxHp = Members[i].maxHp + BonusMaxHp;
                        CurrentHps[i] = Math.Min(CurrentHps[i].Value + def.value, maxHp);
                    }
                    break;
                case UpgradeKind.Attack:
                    BonusAttack += def.value;
                    break;
                case UpgradeKind.Defense:
                    BonusDefense += def.value;
                    break;
                case UpgradeKind.EnergyAbsorb:
                    EnergyGainBonus += def.value;
                    break;
                case UpgradeKind.EnergyCap:
                    EnergyCapBonus += def.value;
                    break;
                case UpgradeKind.LearnSkill:
                    LearnSkill(def.skill);
                    break;
                case UpgradeKind.SkillPower:
                    UpgradeSkill(def.targetSkill, def.value);
                    break;
            }
            Acquired.Add(def);
        }

        public void LearnSkill(SkillDefinition skill)
        {
            if (skill == null || originalToClone.ContainsKey(skill)) return;
            var clone = ScriptableObject.Instantiate(skill);
            if (Skills.Count >= CharacterDefinition.MaxSkills)
            {
                var removed = Skills[0];
                Skills.RemoveAt(0);
                SkillDefinition removedOriginal = null;
                foreach (var kv in originalToClone)
                {
                    if (kv.Value == removed) { removedOriginal = kv.Key; break; }
                }
                if (removedOriginal != null) originalToClone.Remove(removedOriginal);
            }
            Skills.Add(clone);
            originalToClone[skill] = clone;
        }

        public void UpgradeSkill(SkillDefinition targetSkill, int amount)
        {
            SkillDefinition target;
            if (targetSkill != null)
            {
                target = originalToClone.TryGetValue(targetSkill, out var clone) ? clone : targetSkill;
            }
            else
            {
                target = Skills.Count > 0 ? Skills[0] : null;
            }
            if (target == null) return;
            foreach (var effect in target.effects)
            {
                if (effect == null) continue;
                if (effect.kind == SkillEffectKind.Damage || effect.kind == SkillEffectKind.Heal)
                    effect.amount += amount;
            }
            target.upgradeLevel += 1;
        }

        public List<UpgradeDefinition> RollChoices(IList<UpgradeDefinition> pool, int count, System.Random rng)
        {
            var result = new List<UpgradeDefinition>();
            if (pool == null) return result;
            rng ??= new System.Random();
            var applicable = new List<UpgradeDefinition>();
            foreach (var def in pool)
                if (IsApplicable(def)) applicable.Add(def);

            // Fisher-Yates 셔플
            for (int i = applicable.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (applicable[i], applicable[j]) = (applicable[j], applicable[i]);
            }
            int take = Math.Min(count, applicable.Count);
            for (int i = 0; i < take; i++) result.Add(applicable[i]);
            return result;
        }
    }
}
