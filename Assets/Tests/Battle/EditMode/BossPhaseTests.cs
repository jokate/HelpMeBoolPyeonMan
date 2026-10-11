using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoolPyeon.Battle.Tests
{
    // T11: 보스 2페이즈 전환(BattleState.UpdateBossPhases)
    public class BossPhaseTests
    {
        readonly List<Object> created = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var o in created) Object.DestroyImmediate(o);
            created.Clear();
        }

        MonsterDefinition Boss(int hp, float threshold, bool isBoss = true, MonsterActionDefinition phase2Action = null)
        {
            var m = ScriptableObject.CreateInstance<MonsterDefinition>();
            m.displayName = "Boss";
            m.maxHp = hp;
            m.attack = 3;
            m.isBoss = isBoss;
            m.phase2Threshold = threshold;
            var baseAction = new MonsterActionDefinition();
            baseAction.id = "base_attack";
            baseAction.kind = MonsterActionKind.Attack;
            baseAction.weight = 1f;
            baseAction.damageMultiplier = 1f;
            m.actions = new List<MonsterActionDefinition> { baseAction };
            if (phase2Action != null)
            {
                m.phase2Actions = new List<MonsterActionDefinition> { phase2Action };
            }
            created.Add(m);
            return m;
        }

        MonsterActionDefinition SpecialAction(float weight = 100f)
        {
            var a = new MonsterActionDefinition();
            a.id = "phase2_special";
            a.kind = MonsterActionKind.Special;
            a.weight = weight;
            a.damageMultiplier = 2f;
            return a;
        }

        static BattleState NewBattle(MonsterDefinition boss, int? seed = null)
        {
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", 50, 3, 0));
            var state = new BattleState(squad, new List<MonsterDefinition> { boss }, seed);
            state.StartBattle();
            return state;
        }

        [Test]
        public void NoTransition_WhenHpAboveThreshold()
        {
            var boss = Boss(hp: 100, threshold: 0.5f);
            var state = NewBattle(boss);
            state.Enemies[0].TakeDamage(10); // 90/100, above 50%
            state.UpdateBossPhases();
            Assert.IsFalse(state.IsPhase2(0));
            Assert.AreEqual(0, state.Phase2TransitionCount);
        }

        [Test]
        public void TransitionsOnce_EvenIfCheckedRepeatedly()
        {
            var boss = Boss(hp: 100, threshold: 0.5f);
            var state = NewBattle(boss);
            int eventCount = 0;
            state.Phase2Entered += _ => eventCount++;

            state.Enemies[0].TakeDamage(60); // 40/100, below 50%
            state.UpdateBossPhases();
            state.UpdateBossPhases();
            state.Enemies[0].TakeDamage(10); // still below threshold
            state.UpdateBossPhases();
            state.BeginTurn(); // BeginTurn also calls UpdateBossPhases internally

            Assert.IsTrue(state.IsPhase2(0));
            Assert.AreEqual(1, state.Phase2TransitionCount);
            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void NoTransition_WhenNotBoss()
        {
            var boss = Boss(hp: 100, threshold: 0.5f, isBoss: false);
            var state = NewBattle(boss);
            state.Enemies[0].TakeDamage(80);
            state.UpdateBossPhases();
            Assert.IsFalse(state.IsPhase2(0));
            Assert.AreEqual(0, state.Phase2TransitionCount);
        }

        [Test]
        public void NoTransition_WhenThresholdIsZero()
        {
            var boss = Boss(hp: 100, threshold: 0f);
            var state = NewBattle(boss);
            state.Enemies[0].TakeDamage(99);
            state.UpdateBossPhases();
            Assert.IsFalse(state.IsPhase2(0));
        }

        [Test]
        public void Phase2Actions_JoinCandidates_AfterTransition()
        {
            var phase2Action = SpecialAction(weight: 1000f); // 압도적으로 높은 가중치로 항상 뽑히게 함
            var boss = Boss(hp: 100, threshold: 0.5f, phase2Action: phase2Action);
            var state = NewBattle(boss, seed: 1);

            state.Enemies[0].TakeDamage(60); // 40/100, below 50%
            state.BeginTurn(); // UpdateBossPhases + PlanIntents 재계산

            Assert.IsTrue(state.IsPhase2(0));
            Assert.AreEqual(EnemyIntentKind.Special, state.Intents[0].Kind);
            Assert.AreSame(phase2Action, state.Intents[0].Action);
        }

        [Test]
        public void ConsumePhase2Cut_ReturnsTrueOnceThenFalse()
        {
            var boss = Boss(hp: 100, threshold: 0.5f);
            var state = NewBattle(boss);
            state.Enemies[0].TakeDamage(60);
            state.UpdateBossPhases();

            Assert.IsTrue(state.ConsumePhase2Cut());
            Assert.IsFalse(state.ConsumePhase2Cut());
        }

        [Test]
        public void BossIndex_FindsFirstIsBoss()
        {
            var notBoss = Boss(hp: 20, threshold: 0f, isBoss: false);
            var boss = Boss(hp: 100, threshold: 0.5f);
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", 50, 3, 0));
            var state = new BattleState(squad, new List<MonsterDefinition> { notBoss, boss });
            Assert.AreEqual(1, state.BossIndex);
        }
    }
}
