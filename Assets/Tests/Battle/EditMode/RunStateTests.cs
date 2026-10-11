using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoolPyeon.Battle.Tests
{
    public class RunStateTests
    {
        readonly List<Object> created = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var o in created) Object.DestroyImmediate(o);
            created.Clear();
        }

        SkillDefinition Skill(string name, int damage = 5)
        {
            var s = ScriptableObject.CreateInstance<SkillDefinition>();
            s.displayName = name;
            s.effects = new List<SkillEffect> { new SkillEffect { kind = SkillEffectKind.Damage, amount = damage } };
            created.Add(s);
            return s;
        }

        CharacterDefinition Character(params SkillDefinition[] skills)
        {
            var c = ScriptableObject.CreateInstance<CharacterDefinition>();
            c.displayName = "P";
            c.maxHp = 30;
            c.attack = 3;
            c.defense = 0;
            c.skills = new List<SkillDefinition>(skills);
            created.Add(c);
            return c;
        }

        UpgradeDefinition Upgrade(UpgradeKind kind, int value = 0, SkillDefinition skill = null, SkillDefinition targetSkill = null)
        {
            var u = ScriptableObject.CreateInstance<UpgradeDefinition>();
            u.displayName = kind.ToString();
            u.kind = kind;
            u.value = value;
            u.skill = skill;
            u.targetSkill = targetSkill;
            created.Add(u);
            return u;
        }

        [Test]
        public void CreateSquad_AppliesHpAttackDefenseBonus()
        {
            var character = Character();
            var run = new RunState(character);
            run.Apply(Upgrade(UpgradeKind.Hp, 5));
            run.Apply(Upgrade(UpgradeKind.Attack, 2));
            run.Apply(Upgrade(UpgradeKind.Defense, 1));

            var squad = run.CreateSquad();
            var combatant = squad.Members[0].Combatant;

            Assert.AreEqual(35, combatant.MaxHp);
            Assert.AreEqual(5, combatant.Attack);
            Assert.AreEqual(1, combatant.Defense);
        }

        [Test]
        public void CaptureHp_CarriesOverToNextSquad()
        {
            var character = Character();
            var run = new RunState(character);
            var squad = run.CreateSquad();
            squad.Members[0].Combatant.TakeDamage(12);

            run.CaptureHp(squad);
            var nextSquad = run.CreateSquad();

            Assert.AreEqual(18, nextSquad.Members[0].Combatant.Hp);
        }

        [Test]
        public void LearnSkill_AddsToSkillsAsClone_WithoutTouchingOriginal()
        {
            var newSkill = Skill("HeavyBlow");
            var character = Character();
            var run = new RunState(character);

            run.Apply(Upgrade(UpgradeKind.LearnSkill, skill: newSkill));

            Assert.AreEqual(1, run.Skills.Count);
            Assert.AreNotSame(newSkill, run.Skills[0]);
            Assert.AreEqual("HeavyBlow", run.Skills[0].displayName);
        }

        [Test]
        public void LearnSkill_WhenFull_ReplacesOldestSlot()
        {
            var skills = new SkillDefinition[CharacterDefinition.MaxSkills];
            for (int i = 0; i < skills.Length; i++) skills[i] = Skill($"S{i}");
            var character = Character(skills);
            var run = new RunState(character);

            var extra = Skill("Extra");
            run.Apply(Upgrade(UpgradeKind.LearnSkill, skill: extra));

            Assert.AreEqual(CharacterDefinition.MaxSkills, run.Skills.Count);
            Assert.IsFalse(run.Skills.Exists(s => s.displayName == "S0"));
            Assert.IsTrue(run.Skills.Exists(s => s.displayName == "Extra"));
        }

        [Test]
        public void UpgradeSkill_IncreasesDamageAndLevel_OnClone()
        {
            var original = Skill("Slash", damage: 5);
            var character = Character(original);
            var run = new RunState(character);

            run.Apply(Upgrade(UpgradeKind.SkillPower, 2, targetSkill: original));

            Assert.AreEqual(0, original.effects[0].amount == 5 ? 0 : 1); // 원본은 그대로 5
            Assert.AreEqual(7, run.Skills[0].effects[0].amount);
            Assert.AreEqual(1, run.Skills[0].upgradeLevel);
        }

        [Test]
        public void RollChoices_ExcludesAlreadyLearnedSkill_AndSkillPowerWithoutSkills()
        {
            var owned = Skill("Owned");
            var newSkill = Skill("New");
            var character = Character(); // 보유 스킬 없음
            var run = new RunState(character);
            run.Apply(Upgrade(UpgradeKind.LearnSkill, skill: owned));

            var pool = new List<UpgradeDefinition>
            {
                Upgrade(UpgradeKind.Hp, 5),
                Upgrade(UpgradeKind.LearnSkill, skill: owned),   // 이미 보유 → 제외
                Upgrade(UpgradeKind.LearnSkill, skill: newSkill), // 미보유 → 포함 가능
                Upgrade(UpgradeKind.SkillPower, 2), // 보유 스킬 있음(owned) → 포함 가능
            };

            var choices = run.RollChoices(pool, 3, new System.Random(1));

            Assert.IsFalse(choices.Exists(u => u.kind == UpgradeKind.LearnSkill && u.skill == owned));
            Assert.LessOrEqual(choices.Count, 3);
        }

        // T2: 새 런의 시작 스테이지 계산 = min(highestCleared+1, maxStage), 최소 1.
        [Test]
        public void ComputeStartStage_ClampsToRange()
        {
            Assert.AreEqual(1, RunState.ComputeStartStage(0, 2));
            Assert.AreEqual(2, RunState.ComputeStartStage(1, 2));
            Assert.AreEqual(2, RunState.ComputeStartStage(2, 2));
            Assert.AreEqual(1, RunState.ComputeStartStage(5, 0));
        }

        [Test]
        public void StageNumber_DefaultsToOne()
        {
            var character = Character(Skill("S"));
            var run = new RunState(character);
            Assert.AreEqual(1, run.StageNumber);
        }

        // T2: CreateSquad 는 편성 순서대로(최대 3명) 전원을 스탯+영구/방 강화 보너스로 만든다.
        [Test]
        public void CreateSquad_AllMembersInOrder()
        {
            var a = Character(); a.displayName = "A"; a.maxHp = 20; a.attack = 2; a.defense = 1;
            var b = Character(); b.displayName = "B"; b.maxHp = 25; b.attack = 3; b.defense = 0;
            var c = Character(); c.displayName = "C"; c.maxHp = 30; c.attack = 1; c.defense = 2;
            var run = new RunState(new List<CharacterDefinition> { a, b, c });
            run.Apply(Upgrade(UpgradeKind.Hp, 5));
            run.Apply(Upgrade(UpgradeKind.Attack, 1));

            var squad = run.CreateSquad();

            Assert.AreEqual(3, squad.Members.Count);
            Assert.AreEqual("A", squad.Members[0].Combatant.Name);
            Assert.AreEqual("B", squad.Members[1].Combatant.Name);
            Assert.AreEqual("C", squad.Members[2].Combatant.Name);
            Assert.AreEqual(25, squad.Members[0].Combatant.MaxHp); // 20+5
            Assert.AreEqual(3, squad.Members[0].Combatant.Attack); // 2+1
            Assert.AreEqual(30, squad.Members[1].Combatant.MaxHp); // 25+5
            Assert.AreEqual(4, squad.Members[1].Combatant.Attack); // 3+1
            Assert.AreEqual(35, squad.Members[2].Combatant.MaxHp); // 30+5
        }

        // T2: CaptureHp 는 편성 전원의 HP 를 순서대로 기록한다.
        [Test]
        public void CaptureHp_PerMember()
        {
            var a = Character(); a.maxHp = 20;
            var b = Character(); b.maxHp = 20;
            var run = new RunState(new List<CharacterDefinition> { a, b });
            var squad = run.CreateSquad();
            squad.Members[0].Combatant.TakeDamage(5);
            squad.Members[1].Combatant.TakeDamage(8);

            run.CaptureHp(squad);
            var nextSquad = run.CreateSquad();

            Assert.AreEqual(15, nextSquad.Members[0].Combatant.Hp);
            Assert.AreEqual(12, nextSquad.Members[1].Combatant.Hp);
        }

        // T9: 방 사이 규칙 — 사망(HP 0) 멤버는 다음 방에서도 HP 0 로 시작한다(자동 부활 없음).
        [Test]
        public void CaptureHp_DeadMemberStaysDeadInNextRoom()
        {
            var a = Character(); a.maxHp = 20;
            var b = Character(); b.maxHp = 20;
            var run = new RunState(new List<CharacterDefinition> { a, b });
            var squad = run.CreateSquad();
            squad.Members[0].Combatant.TakeDamage(20); // 사망
            squad.Members[1].Combatant.TakeDamage(5);

            run.CaptureHp(squad);
            var nextSquad = run.CreateSquad();

            Assert.AreEqual(0, nextSquad.Members[0].Combatant.Hp);
            Assert.AreEqual(15, nextSquad.Members[1].Combatant.Hp);
        }

        // T9: 휴식 노드는 사망 멤버도 pct% 회복시켜 부활시킨다.
        [Test]
        public void HealPercent_PerMember_RevivesDead()
        {
            var a = Character(); a.maxHp = 30;
            var b = Character(); b.maxHp = 30;
            var c = Character(); c.maxHp = 30;
            var run = new RunState(new List<CharacterDefinition> { a, b, c });
            run.RestoreHps(new List<int?> { 10, 0, null });

            run.HealPercent(50);

            Assert.AreEqual(25, run.CurrentHps[0]);
            Assert.AreEqual(15, run.CurrentHps[1]);
            Assert.IsNull(run.CurrentHps[2]);
        }

        // T9: HP 업그레이드는 살아 있는 멤버만 회복시킨다(사망 멤버는 건너뛴다). BonusMaxHp 는 전원 적용.
        [Test]
        public void ApplyHpUpgrade_SkipsDeadMember()
        {
            var a = Character(); a.maxHp = 30;
            var b = Character(); b.maxHp = 30;
            var run = new RunState(new List<CharacterDefinition> { a, b });
            run.RestoreHps(new List<int?> { 10, 0 });

            run.Apply(Upgrade(UpgradeKind.Hp, 5));

            Assert.AreEqual(15, run.CurrentHps[0]);
            Assert.AreEqual(0, run.CurrentHps[1]);
            Assert.AreEqual(5, run.BonusMaxHp);
        }

        // T9: 방 클리어(Capture) → 휴식(HealPercent) → 업그레이드(Apply) → 다음 방(CreateSquad) 순서로
        // 멤버별 HP 가 규칙대로 이어지는지 확인한다.
        [Test]
        public void RoomClear_Rest_Upgrade_Chain()
        {
            var a = Character(); a.maxHp = 30;
            var b = Character(); b.maxHp = 30;
            var run = new RunState(new List<CharacterDefinition> { a, b });
            var squad = run.CreateSquad();
            squad.Members[0].Combatant.TakeDamage(30); // 사망
            squad.Members[1].Combatant.TakeDamage(10);

            run.CaptureHp(squad); // {0, 20}
            run.HealPercent(50);  // {15, 35->30}
            run.Apply(Upgrade(UpgradeKind.Hp, 5)); // 멤버0 생존(15>0) -> +5, 멤버1도 +5(상한 35)

            var nextSquad = run.CreateSquad();
            Assert.AreEqual(20, nextSquad.Members[0].Combatant.Hp);
            Assert.AreEqual(35, nextSquad.Members[1].Combatant.Hp);
        }

        // T2: 옛(단일 HP) 세이브 복원 경로 — 멤버0만 복원되고 나머지는 가득 찬 상태(null)로 시작한다.
        [Test]
        public void RestoreHp_LegacySingle_OnlyRestoresLeader()
        {
            var a = Character(); a.maxHp = 20;
            var b = Character(); b.maxHp = 20;
            var run = new RunState(new List<CharacterDefinition> { a, b });

            run.RestoreHp(9);
            var squad = run.CreateSquad();

            Assert.AreEqual(9, squad.Members[0].Combatant.Hp);
            Assert.AreEqual(20, squad.Members[1].Combatant.Hp);
        }
    }
}
