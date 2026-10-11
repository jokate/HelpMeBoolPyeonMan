using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoolPyeon.Battle.Tests
{
    public class BattleStateTests
    {
        readonly List<Object> created = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var o in created) Object.DestroyImmediate(o);
            created.Clear();
        }

        MonsterDefinition Monster(int hp = 8, int attack = 3, int specialInterval = 0,
            StatusEffectType penalty = StatusEffectType.DamageTakenUp, params MonsterActionKind[] pattern)
        {
            var m = ScriptableObject.CreateInstance<MonsterDefinition>();
            m.displayName = "M";
            m.maxHp = hp;
            m.attack = attack;
            m.specialInterval = specialInterval;
            m.specialDamageMultiplier = 2f;
            m.penalty = penalty;
            m.penaltyTurns = 1;
            m.actionPattern = pattern.Length > 0
                ? new List<MonsterActionKind>(pattern)
                : new List<MonsterActionKind> { MonsterActionKind.Attack };
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

        static MonsterActionDefinition Act(string id, MonsterActionKind kind, float weight, int cooldown = 0,
            params AIConsideration[] considerations)
        {
            return new MonsterActionDefinition
            {
                id = id,
                kind = kind,
                weight = weight,
                cooldownTurns = cooldown,
                considerations = new List<AIConsideration>(considerations)
            };
        }

        static BattleState NewSeeded(int playerHp, int seed, params MonsterDefinition[] wave)
        {
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", playerHp, 3, 0));
            var state = new BattleState(squad, wave, seed);
            state.StartBattle();
            return state;
        }

        // T3: 3인 스쿼드 헬퍼 — 멤버 순서·타겟팅·전멸 테스트용
        static BattleState NewSquad3Battle(int memberHp, params MonsterDefinition[] wave)
        {
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P1", memberHp, 3, 0));
            squad.TryAdd(null, new Combatant("P2", memberHp, 3, 0));
            squad.TryAdd(null, new Combatant("P3", memberHp, 3, 0));
            var state = new BattleState(squad, wave);
            state.StartBattle();
            return state;
        }

        static BattleState NewSeededSquad3(int memberHp, int seed, params MonsterDefinition[] wave)
        {
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P1", memberHp, 3, 0));
            squad.TryAdd(null, new Combatant("P2", memberHp, 3, 0));
            squad.TryAdd(null, new Combatant("P3", memberHp, 3, 0));
            var state = new BattleState(squad, wave, seed);
            state.StartBattle();
            return state;
        }

        static void NextTurn(BattleState s)
        {
            RunEnemyPhase(s);
            s.EndTurn();
            s.BeginTurn();
        }

        [Test]
        public void AI_SpecialAction_FailedDefense_ReservesAllStatuses()
        {
            var m = Monster();
            var sp = Act("sp", MonsterActionKind.Special, 1f);
            sp.statuses = new List<StatusEffectSpec>
            {
                new StatusEffectSpec { type = StatusEffectType.DamageTakenUp, turns = 1, magnitude = 0.5f },
                new StatusEffectSpec { type = StatusEffectType.NoEnergyReserve, turns = 2, magnitude = 0f }
            };
            m.actions = new List<MonsterActionDefinition> { sp };
            var s = NewSeeded(50, 1, m);
            Assert.AreEqual(EnemyIntentKind.Special, s.Intents[0].Kind);
            Assert.AreSame(sp, s.Intents[0].Action);
            RunEnemyPhase(s);
            Assert.AreEqual(2, s.PendingPenalties.Count);
            Assert.AreEqual(StatusEffectType.DamageTakenUp, s.PendingPenalties[0].Type);
            Assert.AreEqual(StatusEffectType.NoEnergyReserve, s.PendingPenalties[1].Type);
        }

        [Test]
        public void AI_SameSeed_SameSequence()
        {
            List<string> Run()
            {
                var m = Monster();
                m.actions = new List<MonsterActionDefinition>
                {
                    Act("atk", MonsterActionKind.Attack, 1f),
                    Act("rest", MonsterActionKind.Charge, 1f),
                    Act("sp", MonsterActionKind.Special, 1f)
                };
                var s = NewSeeded(999, 42, m);
                var seq = new List<string>();
                for (int t = 0; t < 10; t++)
                {
                    seq.Add(s.Intents[0].Action.id);
                    NextTurn(s);
                }
                return seq;
            }
            CollectionAssert.AreEqual(Run(), Run());
        }

        [Test]
        public void AI_ZeroCurveConsideration_NeverPicked()
        {
            var m = Monster();
            var zero = new AIConsideration { input = AIInputKind.SelfHpRatio, curve = AnimationCurve.Constant(0f, 1f, 0f) };
            m.actions = new List<MonsterActionDefinition>
            {
                Act("never", MonsterActionKind.Attack, 100f, 0, zero),
                Act("rest", MonsterActionKind.Charge, 1f)
            };
            var s = NewSeeded(999, 7, m);
            for (int t = 0; t < 20; t++)
            {
                Assert.AreEqual("rest", s.Intents[0].Action.id);
                Assert.AreEqual(EnemyIntentKind.Wait, s.Intents[0].Kind);
                NextTurn(s);
            }
        }

        [Test]
        public void AI_Cooldown_BlocksReselection()
        {
            var m = Monster();
            m.randomness = 0f;
            m.actions = new List<MonsterActionDefinition>
            {
                Act("big", MonsterActionKind.Attack, 1000f, 2),
                Act("small", MonsterActionKind.Attack, 0.001f)
            };
            var s = NewSeeded(999, 3, m);
            var seq = new List<string>();
            for (int t = 0; t < 4; t++)
            {
                seq.Add(s.Intents[0].Action.id);
                NextTurn(s);
            }
            CollectionAssert.AreEqual(new[] { "big", "small", "small", "big" }, seq);
        }

        [Test]
        public void AI_ZeroRandomness_PicksHighestScore()
        {
            var m = Monster();
            m.randomness = 0f;
            m.actions = new List<MonsterActionDefinition>
            {
                Act("a", MonsterActionKind.Attack, 1f),
                Act("b", MonsterActionKind.Special, 5f),
                Act("c", MonsterActionKind.Charge, 2f)
            };
            var s = NewSeeded(999, 11, m);
            for (int t = 0; t < 5; t++)
            {
                Assert.AreEqual("b", s.Intents[0].Action.id);
                NextTurn(s);
            }
        }

        [Test]
        public void ThreeMonsters_AtMostTwoAttack()
        {
            var s = NewBattle(30, Monster(), Monster(), Monster());
            int attackers = 0;
            foreach (var it in s.Intents)
                if (it.Kind != EnemyIntentKind.Wait) attackers++;
            Assert.AreEqual(BattleState.MaxAttackersPerTurn, attackers);
            RunEnemyPhase(s);
            Assert.AreEqual(24, s.Player.Hp);
        }

        [Test]
        public void Guard_HalvesDamage()
        {
            var s = NewBattle(30, Monster(attack: 4));
            s.AddEnergy(1);
            s.BeginCommandPhase();
            Assert.IsTrue(s.TryCommand(Skill(1, SkillTargetType.Self, SkillEffectKind.Guard), null));
            s.EndPlayerPhase();
            Assert.AreEqual(2, s.ResolveEnemyIntent(0));
        }

        [Test]
        public void EnemyGuard_ReducesDamageTaken()
        {
            var m = Monster(attack: 4);
            m.actions = new List<MonsterActionDefinition> { Act("guard", MonsterActionKind.Guard, 1f) };
            var s = NewBattle(30, m);
            RunEnemyPhase(s); // Guard 해석: 0 피해, Guarded 부여
            Assert.IsTrue(s.Enemies[0].HasStatus(StatusEffectType.Guarded));
            s.EndTurn();
            s.BeginTurn();

            // 이번 턴 인텐트가 무엇이든, 적의 CalculateDamage 는 Guarded 로 절반이 된다
            Assert.AreEqual(2, s.Enemies[0].CalculateDamage(4));
        }

        [Test]
        public void Magic_IgnoresDefense()
        {
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", 30, 3, 5)); // Defense=5
            var m = Monster(attack: 4);
            m.actions = new List<MonsterActionDefinition> { Act("magic", MonsterActionKind.Magic, 1f) };
            m.actions[0].damageMultiplier = 1f;
            var state = new BattleState(squad, new List<MonsterDefinition> { m });
            state.StartBattle();
            RunEnemyPhase(state);
            Assert.AreEqual(30 - 4, state.Player.Hp);
        }

        [Test]
        public void Magic_AppliesStatus()
        {
            var m = Monster(attack: 4);
            var magic = Act("magic", MonsterActionKind.Magic, 1f);
            magic.statuses = new List<StatusEffectSpec>
            {
                new StatusEffectSpec { type = StatusEffectType.Stun, turns = 1, magnitude = 0f }
            };
            m.actions = new List<MonsterActionDefinition> { magic };
            var s = NewBattle(30, m);
            RunEnemyPhase(s);
            Assert.AreEqual(1, s.PendingPenalties.Count);
            s.EndTurn();
            s.BeginTurn();
            Assert.IsTrue(s.Player.HasStatus(StatusEffectType.Stun));
        }

        [Test]
        public void Magic_Defended_NoDamageNoStatus()
        {
            var m = Monster(attack: 4);
            var magic = Act("magic", MonsterActionKind.Magic, 1f);
            magic.statuses = new List<StatusEffectSpec>
            {
                new StatusEffectSpec { type = StatusEffectType.Stun, turns = 1, magnitude = 0f }
            };
            m.actions = new List<MonsterActionDefinition> { magic };
            var s = NewBattle(30, m);
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            int dealt = s.ResolveEnemyIntent(0, true);
            Assert.AreEqual(0, dealt);
            Assert.AreEqual(30, s.Player.Hp);
            Assert.AreEqual(0, s.PendingPenalties.Count);
        }

        [Test]
        public void EnemyHeal_RestoresLowestAlly()
        {
            var healer = Monster(attack: 6);
            var healAction = Act("heal", MonsterActionKind.Heal, 1f);
            healAction.targetsAllies = true;
            healer.actions = new List<MonsterActionDefinition> { healAction };
            var hurt = Monster(hp: 20);
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", 30, 0, 0));
            var state = new BattleState(squad, new List<MonsterDefinition> { healer, hurt });
            state.StartBattle();
            // hurt 를 미리 다치게 한다
            state.Enemies[1].TakeDamage(10);
            int hpBefore = state.Enemies[1].Hp;
            RunEnemyPhase(state);
            Assert.Greater(state.Enemies[1].Hp, hpBefore);
        }

        [Test]
        public void EnemyBuff_IncreasesOutgoingDamage()
        {
            var buffer = Monster(attack: 1);
            var buffAction = Act("buff", MonsterActionKind.Buff, 1f);
            buffAction.targetsAllies = true;
            buffer.actions = new List<MonsterActionDefinition> { buffAction };
            var attacker = Monster(attack: 4);
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", 30, 0, 0));
            var state = new BattleState(squad, new List<MonsterDefinition> { buffer, attacker });
            state.StartBattle();
            RunEnemyPhase(state); // buffer 는 Buff, attacker 는 Wait(둘 다 카운트되어 2/2)
            state.EndTurn();
            state.BeginTurn();
            Assert.IsTrue(state.Enemies[0].HasStatus(StatusEffectType.AttackUp));
            int buffed = state.Enemies[0].ModifyOutgoingDamage(4);
            Assert.Greater(buffed, 4);
        }

        [Test]
        public void PlanIntents_SupportActionsCountTowardCap2()
        {
            var m1 = Monster(attack: 1);
            m1.actions = new List<MonsterActionDefinition> { Act("g1", MonsterActionKind.Guard, 1f) };
            var m2 = Monster(attack: 1);
            m2.actions = new List<MonsterActionDefinition> { Act("g2", MonsterActionKind.Guard, 1f) };
            var m3 = Monster(attack: 1);
            m3.actions = new List<MonsterActionDefinition> { Act("g3", MonsterActionKind.Guard, 1f) };
            var s = NewBattle(30, m1, m2, m3);
            Assert.AreEqual(EnemyIntentKind.Guard, s.Intents[0].Kind);
            Assert.AreEqual(EnemyIntentKind.Guard, s.Intents[1].Kind);
            Assert.AreEqual(EnemyIntentKind.Wait, s.Intents[2].Kind);
        }

        [Test]
        public void Stunned_CommandRejected()
        {
            var s = NewBattle(30, Monster());
            s.Player.AddStatus(StatusEffectType.Stun, 1, 0f);
            s.AddEnergy(5);
            s.BeginCommandPhase();
            Assert.IsTrue(s.IsPlayerStunned);
            Assert.IsFalse(s.TryCommand(Skill(1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage), null));
            Assert.AreEqual(5, s.Energy.Current);
        }

        [Test]
        public void NoEnergyReserve_KeepsReserveAtTurnEnd()
        {
            var s = NewBattle(30, Monster(attack: 0));
            s.Player.AddStatus(StatusEffectType.NoEnergyReserve, 1, 0f);
            s.AddEnergy(4);
            RunEnemyPhase(s);
            s.EndTurn();
            Assert.AreEqual(4, s.Energy.Current);
        }

        [Test]
        public void NoEnergyReserve_SealedTurnGainsZero_NextTurnCollects()
        {
            var s = NewBattle(30, Monster(attack: 0));
            s.AddEnergy(3);
            s.Player.AddStatus(StatusEffectType.NoEnergyReserve, 1, 0f);
            Assert.IsTrue(s.IsEnergySealed);
            Assert.AreEqual(0, s.CollectEnergy(5));
            Assert.AreEqual(3, s.Energy.Current);
            RunEnemyPhase(s);
            s.EndTurn();
            s.BeginTurn();
            Assert.IsFalse(s.IsEnergySealed);
            Assert.AreEqual(3, s.Energy.Current);
            Assert.AreEqual(5, s.CollectEnergy(5));
            Assert.AreEqual(8, s.Energy.Current);
        }

        [Test]
        public void NoEnergyReserve_PenaltySealsNextTurn()
        {
            var m = Monster(attack: 3, specialInterval: 1, penalty: StatusEffectType.NoEnergyReserve);
            var s = NewBattle(30, m);
            s.AddEnergy(2);
            Assert.IsFalse(s.IsEnergySealed);
            RunEnemyPhase(s);
            s.EndTurn();
            s.BeginTurn();
            Assert.IsTrue(s.IsEnergySealed);
            Assert.AreEqual(0, s.CollectEnergy(4));
            Assert.AreEqual(2, s.Energy.Current);
        }

        [Test]
        public void EnergyKeptWithoutPenalty()
        {
            var s = NewBattle(30, Monster(attack: 0));
            s.AddEnergy(4);
            RunEnemyPhase(s);
            s.EndTurn();
            Assert.AreEqual(4, s.Energy.Current);
        }

        [Test]
        public void AllEnemiesDead_Victory()
        {
            var s = NewBattle(30, Monster(hp: 3));
            s.AddEnergy(1);
            s.BeginCommandPhase();
            Assert.IsTrue(s.TryCommand(Skill(1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage), null));
            Assert.AreEqual(BattlePhase.Victory, s.Phase);
            Assert.AreEqual(BattlePhase.Victory, s.Outcome);
        }

        [Test]
        public void PlayerHpZero_Defeat()
        {
            var s = NewBattle(5, Monster(attack: 3), Monster(attack: 3));
            RunEnemyPhase(s);
            Assert.IsTrue(s.Player.IsDead);
            Assert.AreEqual(BattlePhase.Defeat, s.Phase);
        }

        [Test]
        public void Special_OnIntervalAndPenaltyAppliedNextTurn()
        {
            var s = NewBattle(30, Monster(attack: 3, specialInterval: 2, penalty: StatusEffectType.Stun));
            Assert.AreEqual(EnemyIntentKind.Attack, s.Intents[0].Kind);
            RunEnemyPhase(s);
            s.EndTurn();
            s.BeginTurn();
            Assert.AreEqual(2, s.Turn);
            Assert.AreEqual(EnemyIntentKind.Special, s.Intents[0].Kind);
            Assert.AreEqual(6, s.Intents[0].Damage);
            RunEnemyPhase(s);
            Assert.IsFalse(s.Player.HasStatus(StatusEffectType.Stun));
            s.EndTurn();
            s.BeginTurn();
            Assert.IsTrue(s.IsPlayerStunned);
        }

        [Test]
        public void SuccessfulCommand_StaysInCommandPhase()
        {
            var s = NewBattle(30, Monster(hp: 20));
            s.AddEnergy(2);
            s.BeginCommandPhase();
            Assert.AreEqual(BattlePhase.Command, s.Phase);
            var attack = Skill(1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 1);
            Assert.IsTrue(s.TryCommand(attack, null));
            Assert.AreEqual(BattlePhase.Command, s.Phase);
            Assert.IsTrue(s.TryCommand(attack, null));
            Assert.AreEqual(BattlePhase.Command, s.Phase);
            Assert.AreEqual(0, s.Energy.Current);
            Assert.IsFalse(s.TryCommand(attack, null));
        }

        [Test]
        public void CanAnyCommand_FalseWhenEnergyExhausted()
        {
            var s = NewBattle(30, Monster(hp: 20));
            s.AddEnergy(1);
            s.BeginCommandPhase();
            var skill = Skill(1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 1);
            Assert.IsTrue(s.TryCommand(skill, null));
            Assert.IsFalse(s.CanAnyCommand(new[] { skill }));
            s.AddEnergy(1);
            Assert.IsTrue(s.CanAnyCommand(new[] { skill }));
        }

        [Test]
        public void CommandPhase_AllSkillsUsableWhileEnergyLasts()
        {
            var s = NewBattle(30, Monster(hp: 20));
            s.AddEnergy(3);
            s.BeginCommandPhase();
            var attack = Skill(1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 1);
            var guard = Skill(1, SkillTargetType.Self, SkillEffectKind.Guard);
            var skill = Skill(1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 1);

            Assert.IsTrue(s.CanCommand(attack));
            Assert.IsTrue(s.CanCommand(guard));
            Assert.IsTrue(s.CanCommand(skill));
            Assert.IsTrue(s.TryCommand(attack, null));
            Assert.AreEqual(BattlePhase.Command, s.Phase);
        }

        [Test]
        public void EnemyPhase_BecomesEnemySpecialOnMonsterSpecialInterval()
        {
            var s = NewBattle(30, Monster(attack: 3, specialInterval: 3));
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            Assert.AreEqual(BattlePhase.Enemy, s.Phase);
            for (int i = 0; i < s.Intents.Count; i++) s.ResolveEnemyIntent(i);
            s.EndTurn();
            s.BeginTurn();

            s.BeginCommandPhase();
            s.EndPlayerPhase();
            Assert.AreEqual(BattlePhase.Enemy, s.Phase);
            for (int i = 0; i < s.Intents.Count; i++) s.ResolveEnemyIntent(i);
            s.EndTurn();
            s.BeginTurn();

            Assert.AreEqual(3, s.Turn);
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            Assert.AreEqual(BattlePhase.EnemySpecial, s.Phase);
        }

        [Test]
        public void SpecialIntent_Defended_NoDamageNoPenalty()
        {
            var s = NewBattle(30, Monster(attack: 3, specialInterval: 1, penalty: StatusEffectType.Stun));
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            Assert.AreEqual(BattlePhase.EnemySpecial, s.Phase);
            int dealt = s.ResolveEnemyIntent(0, true);
            Assert.AreEqual(0, dealt);
            Assert.AreEqual(30, s.Player.Hp);
            Assert.AreEqual(0, s.PendingPenalties.Count);
        }

        [Test]
        public void SpecialIntent_NotDefended_DamageAndPenalty()
        {
            var s = NewBattle(30, Monster(attack: 3, specialInterval: 1, penalty: StatusEffectType.Stun));
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            Assert.AreEqual(BattlePhase.EnemySpecial, s.Phase);
            int dealt = s.ResolveEnemyIntent(0, false);
            Assert.AreEqual(6, dealt);
            Assert.AreEqual(24, s.Player.Hp);
            Assert.AreEqual(1, s.PendingPenalties.Count);
        }

        [Test]
        public void NormalAttack_Evaded_NoDamage()
        {
            var s = NewBattle(30, Monster(attack: 4));
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            Assert.AreEqual(BattlePhase.Enemy, s.Phase);
            Assert.AreEqual(0, s.ResolveEnemyIntent(0, true));
            Assert.AreEqual(30, s.Player.Hp);
        }

        [Test]
        public void NormalAttack_NotEvaded_FullDamage()
        {
            var s = NewBattle(30, Monster(attack: 4));
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            Assert.AreEqual(4, s.ResolveEnemyIntent(0, false));
            Assert.AreEqual(26, s.Player.Hp);
        }

        [Test]
        public void NormalAttack_NotEvaded_Guarding_HalfDamage()
        {
            var s = NewBattle(30, Monster(attack: 4));
            s.AddEnergy(1);
            s.BeginCommandPhase();
            Assert.IsTrue(s.TryCommand(Skill(1, SkillTargetType.Self, SkillEffectKind.Guard), null));
            s.EndPlayerPhase();
            Assert.AreEqual(2, s.ResolveEnemyIntent(0, false));
            Assert.AreEqual(28, s.Player.Hp);
        }

        [Test]
        public void Grunt_TenTurns_MixesActions_AndHeavyScoresHigherWithStatus()
        {
            // Grunt.asset 을 흉내 낸 5개 행동. 시드 고정, 10턴 동안 2종 이상이 나오는지 확인한다.
            var m = Monster(hp: 18);
            var heavyCurve = new AnimationCurve(new Keyframe(0f, 0.6f), new Keyframe(1f, 2.0f));
            var restCurve = new AnimationCurve(new Keyframe(0f, 2.0f), new Keyframe(1f, 0.1f));
            var weakenTurnsCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1.5f));
            var weakenStatusCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.2f));
            var drainCurve = new AnimationCurve(new Keyframe(0f, 0.3f), new Keyframe(1f, 1.5f));

            var basic = new MonsterActionDefinition { id = "basic", kind = MonsterActionKind.Attack, weight = 1.0f, damageMultiplier = 1.0f };
            var heavy = new MonsterActionDefinition
            {
                id = "heavy", kind = MonsterActionKind.Attack, weight = 0.6f, damageMultiplier = 1.7f, cooldownTurns = 1,
                considerations = new List<AIConsideration> { new AIConsideration { input = AIInputKind.TargetHasStatus, status = StatusEffectType.DamageTakenUp, curve = heavyCurve } }
            };
            var rest = new MonsterActionDefinition
            {
                id = "rest", kind = MonsterActionKind.Charge, weight = 0.4f, cooldownTurns = 2,
                considerations = new List<AIConsideration> { new AIConsideration { input = AIInputKind.SelfHpRatio, curve = restCurve } }
            };
            var specialWeaken = new MonsterActionDefinition
            {
                id = "special_weaken", kind = MonsterActionKind.Special, weight = 0.8f, damageMultiplier = 1.5f, cooldownTurns = 2,
                statuses = new List<StatusEffectSpec> { new StatusEffectSpec { type = StatusEffectType.DamageTakenUp, turns = 2, magnitude = 0.5f } },
                considerations = new List<AIConsideration>
                {
                    new AIConsideration { input = AIInputKind.TurnsSinceUsed, normalizeTurns = 3, curve = weakenTurnsCurve },
                    new AIConsideration { input = AIInputKind.TargetHasStatus, status = StatusEffectType.DamageTakenUp, curve = weakenStatusCurve }
                }
            };
            var specialDrain = new MonsterActionDefinition
            {
                id = "special_drain", kind = MonsterActionKind.Special, weight = 0.6f, damageMultiplier = 1.0f, cooldownTurns = 3,
                statuses = new List<StatusEffectSpec> { new StatusEffectSpec { type = StatusEffectType.NoEnergyReserve, turns = 2, magnitude = 0f } },
                considerations = new List<AIConsideration> { new AIConsideration { input = AIInputKind.PlayerEnergyRatio, curve = drainCurve } }
            };
            m.actions = new List<MonsterActionDefinition> { basic, heavy, rest, specialWeaken, specialDrain };
            m.randomness = 0.7f;

            var s = NewSeeded(999, 2024, m);
            var seen = new HashSet<string>();
            for (int t = 0; t < 10 && !s.Player.IsDead; t++)
            {
                if (s.Intents[0].Action != null) seen.Add(s.Intents[0].Action.id);
                NextTurn(s);
            }
            Assert.GreaterOrEqual(seen.Count, 2, "10턴 동안 2종 이상의 행동이 나와야 한다.");

            // heavy 점수: 대상이 DamageTakenUp 이면 배율이 오른다 (10턴 루프의 잔여 상태이상과 분리된 새 대상으로 비교)
            var freshTarget = new Combatant("P2", 30, 3, 0);
            var ctxNoStatus = new EnemyAIContext(s.Enemies[0], freshTarget, s.Energy, 5, _ => -1);
            float scoreWithout = EnemyAI.Score(heavy, ctxNoStatus);
            freshTarget.AddStatus(StatusEffectType.DamageTakenUp, 2, 0.5f);
            var ctxWithStatus = new EnemyAIContext(s.Enemies[0], freshTarget, s.Energy, 5, _ => -1);
            float scoreWith = EnemyAI.Score(heavy, ctxWithStatus);
            Assert.Greater(scoreWith, scoreWithout, "DamageTakenUp 이 걸리면 heavy 점수가 올라가야 한다.");
        }

        [Test]
        public void ChargePattern_IsWait()
        {
            var s = NewBattle(30, Monster(pattern: new[] { MonsterActionKind.Charge, MonsterActionKind.Attack }));
            Assert.AreEqual(EnemyIntentKind.Wait, s.Intents[0].Kind);
            RunEnemyPhase(s);
            Assert.AreEqual(30, s.Player.Hp);
            s.EndTurn();
            s.BeginTurn();
            Assert.AreEqual(EnemyIntentKind.Attack, s.Intents[0].Kind);
        }

        // ---- T3: 멤버 턴·타겟팅 ----

        [Test]
        public void MemberOrder_AdvancesThenResetsNextTurn()
        {
            var s = NewSquad3Battle(30, Monster(attack: 0));
            Assert.AreEqual(0, s.ActiveMemberIndex);
            Assert.IsFalse(s.AllMembersActed);
            Assert.IsTrue(s.AdvanceMember());
            Assert.AreEqual(1, s.ActiveMemberIndex);
            Assert.IsTrue(s.AdvanceMember());
            Assert.AreEqual(2, s.ActiveMemberIndex);
            Assert.IsFalse(s.AdvanceMember());
            Assert.IsTrue(s.AllMembersActed);

            RunEnemyPhase(s);
            s.EndTurn();
            s.BeginTurn();
            Assert.AreEqual(0, s.ActiveMemberIndex);
            Assert.IsFalse(s.AllMembersActed);
        }

        [Test]
        public void MemberOrder_SkipsDeadMember()
        {
            var s = NewSquad3Battle(30, Monster(attack: 0));
            while (!s.Members[1].Combatant.IsDead) s.Members[1].Combatant.TakeDamage(9999);
            s.EndTurn();
            s.BeginTurn();
            Assert.AreEqual(0, s.ActiveMemberIndex);
            Assert.IsTrue(s.AdvanceMember());
            Assert.AreEqual(2, s.ActiveMemberIndex);
            Assert.IsFalse(s.AdvanceMember());
        }

        [Test]
        public void EnemyIntent_TargetsAliveMember_AcrossSeeds()
        {
            for (int seed = 1; seed <= 5; seed++)
            {
                var s = NewSeededSquad3(30, seed, Monster(attack: 1));
                Assert.AreEqual(EnemyIntentKind.Attack, s.Intents[0].Kind);
                int t = s.Intents[0].TargetMemberIndex;
                Assert.GreaterOrEqual(t, 0, $"seed={seed}");
                Assert.Less(t, 3, $"seed={seed}");
                Assert.IsFalse(s.Members[t].Combatant.IsDead, $"seed={seed}");
            }
        }

        [Test]
        public void ResolveEnemyIntent_Retargets_WhenPlannedTargetDiesFirst()
        {
            var s = NewSquad3Battle(30, Monster(attack: 4));
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            int intended = s.Intents[0].TargetMemberIndex;
            while (!s.Members[intended].Combatant.IsDead) s.Members[intended].Combatant.TakeDamage(9999);

            int dealt = s.ResolveEnemyIntent(0);
            Assert.Greater(dealt, 0);
            bool otherHit = false;
            for (int i = 0; i < s.Members.Count; i++)
            {
                if (i == intended) continue;
                if (!s.Members[i].Combatant.IsDead && s.Members[i].Combatant.Hp < 30) otherHit = true;
            }
            Assert.IsTrue(otherHit, "다른 살아있는 멤버가 대신 피해를 입어야 한다");
        }

        [Test]
        public void PartialMemberDeath_IsNotDefeat()
        {
            var s = NewSquad3Battle(30, Monster(attack: 0));
            while (!s.Members[0].Combatant.IsDead) s.Members[0].Combatant.TakeDamage(9999);
            while (!s.Members[1].Combatant.IsDead) s.Members[1].Combatant.TakeDamage(9999);
            Assert.IsFalse(s.Squad.IsDefeated);
            s.EndTurn();
            s.BeginTurn();
            Assert.AreNotEqual(BattlePhase.Defeat, s.Phase);
        }

        [Test]
        public void AllMembersDead_CausesDefeat()
        {
            var s = NewSquad3Battle(30, Monster(attack: 0));
            foreach (var m in s.Members)
                while (!m.Combatant.IsDead) m.Combatant.TakeDamage(9999);
            s.EndTurn();
            s.BeginTurn();
            Assert.IsTrue(s.Squad.IsDefeated);
            Assert.AreEqual(BattlePhase.Defeat, s.Phase);
        }

        [Test]
        public void PendingPenalty_HitsOnlyTargetedMember()
        {
            var s = NewSquad3Battle(30, Monster(attack: 3, specialInterval: 1, penalty: StatusEffectType.Stun));
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            int targetIdx = s.Intents[0].TargetMemberIndex;
            s.ResolveEnemyIntent(0);
            s.EndTurn();
            s.BeginTurn();

            Assert.IsTrue(s.Members[targetIdx].Combatant.HasStatus(StatusEffectType.Stun));
            for (int i = 0; i < s.Members.Count; i++)
            {
                if (i == targetIdx) continue;
                Assert.IsFalse(s.Members[i].Combatant.HasStatus(StatusEffectType.Stun), $"member {i}");
            }
        }

        [Test]
        public void CollectEnergy_OnlyAffectsActiveMember_SealJudgedByActiveMember()
        {
            var s = NewSquad3Battle(30, Monster(attack: 0));
            Assert.AreEqual(0, s.ActiveMemberIndex);
            s.CollectEnergy(3);
            Assert.AreEqual(3, s.Members[0].Energy.Current);
            Assert.AreEqual(0, s.Members[1].Energy.Current);

            Assert.IsFalse(s.IsEnergySealed);
            s.Members[0].Combatant.AddStatus(StatusEffectType.NoEnergyReserve, 1, 0f);
            Assert.IsTrue(s.IsEnergySealed);
            Assert.AreEqual(0, s.CollectEnergy(5));
            Assert.AreEqual(3, s.Members[0].Energy.Current);
        }
    }
}
