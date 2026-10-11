using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoolPyeon.Battle.Tests
{
    // T7: 새 상태이상 4종(Weaken/Rush/CostUp/Burn) — 적용, 지속 턴 감소, 해제
    public class StatusEffectTests
    {
        readonly List<Object> created = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var o in created) Object.DestroyImmediate(o);
            created.Clear();
        }

        MonsterDefinition Monster(int hp = 8, int attack = 3, int specialInterval = 0,
            StatusEffectType penalty = StatusEffectType.Weaken, float penaltyMagnitude = 0.25f)
        {
            var m = ScriptableObject.CreateInstance<MonsterDefinition>();
            m.displayName = "M";
            m.maxHp = hp;
            m.attack = attack;
            m.specialInterval = specialInterval;
            m.specialDamageMultiplier = 2f;
            m.penalty = penalty;
            m.penaltyTurns = 1;
            m.penaltyMagnitude = penaltyMagnitude;
            m.actionPattern = new List<MonsterActionKind> { MonsterActionKind.Attack };
            created.Add(m);
            return m;
        }

        SkillDefinition Skill(int cost, SkillTargetType target, SkillEffectKind kind, int amount = 0)
        {
            var s = ScriptableObject.CreateInstance<SkillDefinition>();
            s.energyCost = cost;
            s.targetType = target;
            s.effects = new List<SkillEffect> { new SkillEffect { kind = kind, amount = amount } };
            created.Add(s);
            return s;
        }

        SkillDefinition StatusSkill(int cost, SkillTargetType target, StatusEffectType status, int turns, float mag)
        {
            var s = ScriptableObject.CreateInstance<SkillDefinition>();
            s.energyCost = cost;
            s.targetType = target;
            s.effects = new List<SkillEffect>
            {
                new SkillEffect { kind = SkillEffectKind.ApplyStatus, status = status, statusTurns = turns, statusMagnitude = mag }
            };
            created.Add(s);
            return s;
        }

        static BattleState NewBattle(int playerHp, params MonsterDefinition[] wave)
        {
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", playerHp, 3, 0));
            var state = new BattleState(squad, wave);
            state.StartBattle();
            return state;
        }

        static void RunEnemyPhase(BattleState s)
        {
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            for (int i = 0; i < s.Intents.Count; i++) s.ResolveEnemyIntent(i);
        }

        // ---- Weaken ----

        [Test]
        public void Weaken_ReducesOutgoingDamage()
        {
            var c = new Combatant("A", 20, 0, 0);
            c.AddStatus(StatusEffectType.Weaken, 2, 0.5f);
            Assert.AreEqual(5, c.ModifyOutgoingDamage(10));
        }

        [Test]
        public void Weaken_TicksDownAndExpires()
        {
            var c = new Combatant("A", 20, 0, 0);
            c.AddStatus(StatusEffectType.Weaken, 1, 0.5f);
            Assert.AreEqual(5, c.ModifyOutgoingDamage(10));
            c.EndTurn();
            Assert.IsFalse(c.HasStatus(StatusEffectType.Weaken));
            Assert.AreEqual(10, c.ModifyOutgoingDamage(10));
        }

        [Test]
        public void Weaken_FromMonsterSpecialPenalty_ReducesPlayerOutgoingDamageNextTurn()
        {
            var s = NewBattle(30, Monster(hp: 20, attack: 4, specialInterval: 1, penalty: StatusEffectType.Weaken, penaltyMagnitude: 0.5f));
            RunEnemyPhase(s); // 특수 공격 적중 -> pendingPenalties 에 Weaken 예약
            s.EndTurn();
            s.BeginTurn(); // BeginTurn 에서 Player 에게 Weaken 부여
            Assert.IsTrue(s.Player.HasStatus(StatusEffectType.Weaken));
            s.AddEnergy(1);
            s.BeginCommandPhase();
            var attack = Skill(1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 5);
            Assert.IsTrue(s.TryCommand(attack, null));
            // raw = 5 + Attack(3) = 8 -> Weaken 0.5 -> ceil(4)=4
            Assert.AreEqual(20 - 4, s.Enemies[0].Hp);
        }

        [Test]
        public void Weaken_FromSkill_ReducesUserDamageOnTarget()
        {
            var s = NewBattle(30, Monster(hp: 20));
            s.Player.AddStatus(StatusEffectType.Weaken, 2, 0.5f);
            s.AddEnergy(1);
            s.BeginCommandPhase();
            var attack = Skill(1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 5);
            Assert.IsTrue(s.TryCommand(attack, null));
            // raw = amount(5) + Attack(3) = 8 -> Weaken 0.5 -> ceil(4)=4 -> def0 -> 4
            Assert.AreEqual(20 - 4, s.Enemies[0].Hp);
        }

        // ---- Rush ----

        [Test]
        public void Rush_TimeScale_AppliedAndExpires()
        {
            var s = NewBattle(30, Monster(attack: 0));
            Assert.AreEqual(1f, s.EnergyCollectTimeScale);
            s.Player.AddStatus(StatusEffectType.Rush, 1, 0f);
            Assert.AreEqual(BattleState.RushTimeScale, s.EnergyCollectTimeScale);
            RunEnemyPhase(s);
            s.EndTurn();
            s.BeginTurn();
            Assert.AreEqual(1f, s.EnergyCollectTimeScale);
        }

        // ---- CostUp ----

        [Test]
        public void CostUp_IncreasesEffectiveCost()
        {
            var c = new Combatant("A", 20, 0, 0);
            var skill = Skill(2, SkillTargetType.Self, SkillEffectKind.Guard);
            Assert.AreEqual(2, SkillResolver.EffectiveCost(skill, c));
            c.AddStatus(StatusEffectType.CostUp, 2, 1f);
            Assert.AreEqual(3, SkillResolver.EffectiveCost(skill, c));
        }

        [Test]
        public void CostUp_BlocksCommandWhenExactEnergy_ThenExpires()
        {
            var s = NewBattle(30, Monster(hp: 20));
            s.Player.AddStatus(StatusEffectType.CostUp, 1, 1f);
            s.AddEnergy(1);
            s.BeginCommandPhase();
            var attack = Skill(1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 1);
            Assert.IsFalse(s.CanCommand(attack));
            Assert.IsFalse(s.TryCommand(attack, null));

            RunEnemyPhase(s);
            s.EndTurn();
            s.BeginTurn();
            Assert.IsFalse(s.Player.HasStatus(StatusEffectType.CostUp));
            s.BeginCommandPhase();
            Assert.IsTrue(s.CanCommand(attack));
        }

        // ---- Burn ----

        [Test]
        public void Burn_TickDeals_IgnoringDefenseAndGuard()
        {
            var c = new Combatant("A", 20, 0, 10);
            c.SetGuard(true);
            c.AddStatus(StatusEffectType.Burn, 2, 3f);
            Assert.AreEqual(3, c.TickBurn());
            Assert.AreEqual(17, c.Hp);
        }

        [Test]
        public void Burn_AppliedAtTurnStart_ThenExpires()
        {
            // RemainingTurns 는 EndTurn 에서만 줄고, 피해는 BeginTurn 에서만 나간다.
            // 부여 시점이 턴 중간이라 turns=2 인 Burn 은 다음 BeginTurn 에 한 번만 적용된다.
            // 몬스터 일반 공격의 최소 1 피해가 섞이지 않도록 EndTurn/BeginTurn 만으로 턴을 진행한다.
            var s = NewBattle(30, Monster(attack: 0));
            s.Player.AddStatus(StatusEffectType.Burn, 2, 4f);
            s.EndTurn();
            s.BeginTurn(); // Burn 적용
            Assert.AreEqual(26, s.Player.Hp);
            Assert.IsTrue(s.Player.HasStatus(StatusEffectType.Burn)); // 아직 만료 전(다음 EndTurn 에 만료)

            s.EndTurn(); // 여기서 만료·제거
            s.BeginTurn();
            Assert.IsFalse(s.Player.HasStatus(StatusEffectType.Burn));
            Assert.AreEqual(26, s.Player.Hp); // 더 이상 깎이지 않음
        }

        [Test]
        public void Burn_KillsPlayer_CausesDefeat()
        {
            var s = NewBattle(3, Monster(attack: 0));
            s.Player.AddStatus(StatusEffectType.Burn, 2, 5f);
            s.EndTurn();
            s.BeginTurn();
            Assert.IsTrue(s.Player.IsDead);
            Assert.AreEqual(BattlePhase.Defeat, s.Phase);
        }

        [Test]
        public void Burn_FromSkill_DealsDamageToEnemyNextTurn()
        {
            var s = NewBattle(30, Monster(hp: 20, attack: 0));
            s.AddEnergy(1);
            s.BeginCommandPhase();
            var burnSkill = StatusSkill(1, SkillTargetType.SingleEnemy, StatusEffectType.Burn, 2, 4f);
            Assert.IsTrue(s.TryCommand(burnSkill, null));
            RunEnemyPhase(s);
            s.EndTurn();
            s.BeginTurn();
            Assert.AreEqual(16, s.Enemies[0].Hp);
        }
    }
}
