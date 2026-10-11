using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    // T-Battle: 방어 정책 (S1~S6 시나리오에서 쓴다). 러너를 부르지 않고 결과만 정한다.
    public enum ScenarioDefensePolicy
    {
        AlwaysSuccess,
        AlwaysFail,
        Alternate
    }

    // F-14 검증: 전투 한 판(들)을 사람 입력 없이 끝까지 돌려 규칙 값을 판정한다.
    // BattleController 와 달리 러너·씬을 쓰지 않고 BattleState 를 직접 구동한다(EditMode·RunCommand 양쪽에서 실행 가능).
    public class ScenarioReport
    {
        public string Name;
        public bool Pass = true;
        public readonly List<string> Reasons = new List<string>();
        public int Turns;

        public void Fail(string reason)
        {
            Pass = false;
            Reasons.Add(reason);
        }

        public override string ToString()
        {
            string status = Pass ? "PASS" : "FAIL";
            string detail = Reasons.Count > 0 ? " (" + string.Join("; ", Reasons) + ")" : "";
            return $"[Scenario] {Name} {status} turns={Turns}{detail}";
        }
    }

    public static class BattleScenarioSimulator
    {
        public const int TurnCap = 50; // 넘으면 무한 루프 의심으로 FAIL

        static MonsterDefinition Monster(string name, int hp, int attack, int specialInterval = 0,
            float specialMul = 2f, StatusEffectType penalty = StatusEffectType.DamageTakenUp, int penaltyTurns = 1)
        {
            var m = ScriptableObject.CreateInstance<MonsterDefinition>();
            m.displayName = name;
            m.maxHp = hp;
            m.attack = attack;
            m.specialInterval = specialInterval;
            m.specialDamageMultiplier = specialMul;
            m.penalty = penalty;
            m.penaltyTurns = penaltyTurns;
            m.actionPattern = new List<MonsterActionKind> { MonsterActionKind.Attack };
            return m;
        }

        static SkillDefinition Skill(string name, int cost, SkillTargetType target, SkillEffectKind kind, int amount = 0)
        {
            var s = ScriptableObject.CreateInstance<SkillDefinition>();
            s.displayName = name;
            s.energyCost = cost;
            s.targetType = target;
            s.effects = new List<SkillEffect> { new SkillEffect { kind = kind, amount = amount } };
            return s;
        }

        static BattleState NewBattle(int playerHp, int seed, params MonsterDefinition[] wave)
        {
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("Player", playerHp, 3, 0));
            var state = new BattleState(squad, wave, seed);
            state.StartBattle();
            return state;
        }

        static bool NextDefense(ScenarioDefensePolicy policy, ref bool toggle)
        {
            switch (policy)
            {
                case ScenarioDefensePolicy.AlwaysSuccess: return true;
                case ScenarioDefensePolicy.AlwaysFail: return false;
                default:
                    toggle = !toggle;
                    return toggle;
            }
        }

        // 적 턴 전체를 처리한다. forecastDamage 는 방어 결과와 무관한 예고 피해 합(D-12 판정용).
        // T6: 일반 공격은 미니게임이 없어 항상 defended=false. 특수 공격만 방어 정책을 적용한다.
        static (int executed, int damageTaken, int forecastDamage) RunEnemyTurn(
            BattleState s, ScenarioDefensePolicy policy, ref bool toggle)
        {
            s.EndPlayerPhase();
            int forecast = 0;
            foreach (var it in s.Intents)
                if (it.Kind != EnemyIntentKind.Wait) forecast += it.Damage;

            int executed = 0, damage = 0;
            for (int i = 0; i < s.Intents.Count && !s.IsOver; i++)
            {
                var intent = s.Intents[i];
                if (intent.Kind == EnemyIntentKind.Wait || s.Enemies[intent.MonsterIndex].IsDead) continue;
                bool defended = BattleState.IsEvadable(intent.Kind) ? NextDefense(policy, ref toggle) : false;
                damage += s.ResolveEnemyIntent(i, defended);
                executed++;
            }
            return (executed, damage, forecast);
        }

        // S1: 1마리 웨이브, 일반 공격은 미니게임 없이 항상 그대로 맞는다 → Victory, 누적 피해 > 0
        public static ScenarioReport S1_OneMonster_NormalAttackHits()
        {
            var report = new ScenarioReport { Name = "S1" };
            var s = NewBattle(30, 1, Monster("Grunt", 20, 4));
            var attack = Skill("Attack", 1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 1); // dealt = 1+atk3 = 4
            bool toggle = false;
            int turn = 0;
            int totalDamage = 0;
            while (!s.IsOver && turn < TurnCap)
            {
                turn++;
                s.AddEnergy(3);
                s.BeginCommandPhase();
                while (s.CanCommand(attack)) s.TryCommand(attack, null);
                if (!s.IsOver)
                {
                    var (_, damage, _) = RunEnemyTurn(s, ScenarioDefensePolicy.AlwaysSuccess, ref toggle);
                    totalDamage += damage;
                }
                if (!s.IsOver) { s.EndTurn(); s.BeginTurn(); }
            }
            report.Turns = turn;
            if (turn >= TurnCap && !s.IsOver) report.Fail("턴 상한(50) 초과");
            if (s.Phase != BattlePhase.Victory) report.Fail($"Victory 아님: {s.Phase}");
            if (totalDamage <= 0) report.Fail($"일반 공격 피해가 들어오지 않음(T6): totalDamage={totalDamage}");
            if (s.Player.Hp != 30 - totalDamage) report.Fail($"Player.Hp {s.Player.Hp} != 30 - {totalDamage}");
            return report;
        }

        // S2: 3마리 웨이브 → 실행되는 적 행동 ≤2(MaxAttackersPerTurn), 예고 총 피해 ≤ 플레이어 최대 HP 의 25%(D-12)
        public static ScenarioReport S2_ThreeMonsters_AlwaysFailEvade()
        {
            var report = new ScenarioReport { Name = "S2" };
            var s = NewBattle(30, 2, Monster("Grunt", 6, 3), Monster("Grunt", 6, 3), Monster("Grunt", 6, 3));
            var attack = Skill("Attack", 1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 1); // dealt=4
            bool toggle = false;
            int turn = 0;
            float cap = s.Player.MaxHp * 0.25f;
            while (!s.IsOver && turn < TurnCap)
            {
                turn++;
                s.AddEnergy(3);
                s.BeginCommandPhase();
                while (s.CanCommand(attack)) s.TryCommand(attack, null);
                if (!s.IsOver)
                {
                    var (executed, _, forecast) = RunEnemyTurn(s, ScenarioDefensePolicy.AlwaysFail, ref toggle);
                    if (executed > BattleState.MaxAttackersPerTurn)
                        report.Fail($"턴{turn}: 실행된 적 행동 {executed} > {BattleState.MaxAttackersPerTurn}");
                    if (forecast > cap)
                        report.Fail($"턴{turn}: 예고 피해 {forecast} > 최대HP25%({cap})");
                }
                if (!s.IsOver) { s.EndTurn(); s.BeginTurn(); }
            }
            report.Turns = turn;
            if (turn >= TurnCap && !s.IsOver) report.Fail("턴 상한(50) 초과");
            if (!s.IsOver) report.Fail($"전투가 끝나지 않음: {s.Phase}");
            return report;
        }

        // S3: 방어 커맨드(Guard) 후 일반 공격 피격 → 피해가 방어하지 않았을 때의 50%(D-14)
        public static ScenarioReport S3_GuardCommand_HalvesDamage()
        {
            var report = new ScenarioReport { Name = "S3" };
            var guard = Skill("Guard", 1, SkillTargetType.Self, SkillEffectKind.Guard);

            var withoutGuard = NewBattle(30, 3, Monster("Grunt", 10, 5));
            withoutGuard.BeginCommandPhase();
            withoutGuard.EndPlayerPhase();
            int rawDamage = withoutGuard.ResolveEnemyIntent(0, false);

            var withGuard = NewBattle(30, 3, Monster("Grunt", 10, 5));
            withGuard.AddEnergy(1);
            withGuard.BeginCommandPhase();
            withGuard.TryCommand(guard, null);
            withGuard.EndPlayerPhase();
            int guardedDamage = withGuard.ResolveEnemyIntent(0, false);

            report.Turns = 1;
            int expected = (int)Math.Ceiling(rawDamage * 0.5);
            if (guardedDamage != expected)
                report.Fail($"방어 피해 {guardedDamage} != 기대(raw {rawDamage}의 50%, {expected})");
            return report;
        }

        // S4: 에너지 — 커맨드 코스트만큼 차감, 턴 종료 때 남은 에너지 비축, 상한 10(D-3·D-5)
        public static ScenarioReport S4_EnergyCostsAndCapAndCarryOver()
        {
            var report = new ScenarioReport { Name = "S4" };
            var attack = Skill("Attack", 1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 0);
            var guard = Skill("Guard", 1, SkillTargetType.Self, SkillEffectKind.Guard);
            var bigSkill = Skill("Big", 3, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 1);

            var s = NewBattle(30, 4, Monster("Grunt", 999, 0));
            report.Turns = 1;
            s.AddEnergy(12);
            if (s.Energy.Current != EnergyPool.DefaultCap)
                report.Fail($"에너지 상한 10 초과: {s.Energy.Current}");

            s.BeginCommandPhase();
            if (!s.TryCommand(attack, null)) report.Fail("공격 커맨드 실패");
            if (s.Energy.Current != 9) report.Fail($"공격 후 에너지 {s.Energy.Current} != 9");
            if (!s.TryCommand(guard, null)) report.Fail("방어 커맨드 실패");
            if (s.Energy.Current != 8) report.Fail($"방어 후 에너지 {s.Energy.Current} != 8");
            if (!s.TryCommand(bigSkill, null)) report.Fail("스킬 커맨드 실패");
            if (s.Energy.Current != 5) report.Fail($"스킬 후 에너지 {s.Energy.Current} != 5");

            s.EndPlayerPhase();
            s.ResolveEnemyIntent(0, true);
            s.EndTurn();
            if (s.Energy.Current != 5)
                report.Fail($"턴 종료 후 에너지가 비축되지 않음: {s.Energy.Current} != 5");
            return report;
        }

        // S5: 특수 공격 — N턴 주기로 예고, 방어 실패 시 패널티 예약(D-6·D-7)
        public static ScenarioReport S5_SpecialInterval_PenaltyReserved()
        {
            var report = new ScenarioReport { Name = "S5" };
            const int interval = 3;
            var s = NewBattle(30, 5, Monster("Boss", 999, 3, specialInterval: interval, penalty: StatusEffectType.Stun));
            while (s.Turn < interval && !s.IsOver)
            {
                if (s.Intents[0].Kind == EnemyIntentKind.Special)
                    report.Fail($"턴{s.Turn}에서 이미 특수 예고됨 (interval={interval})");
                s.BeginCommandPhase();
                s.EndPlayerPhase();
                s.ResolveEnemyIntent(0, true); // 예고 전 턴은 방어 성공 처리(플레이어 상태에 영향 없이 지나간다)
                s.EndTurn();
                s.BeginTurn();
            }
            report.Turns = s.Turn;
            if (s.Intents[0].Kind != EnemyIntentKind.Special)
                report.Fail($"턴{s.Turn}(=interval)에 Special 예고가 아님: {s.Intents[0].Kind}");
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            s.ResolveEnemyIntent(0, false); // 방어 실패
            if (s.PendingPenalties.Count != 1)
                report.Fail($"패널티가 예약되지 않음: count={s.PendingPenalties.Count}");
            s.EndTurn();
            s.BeginTurn();
            if (!s.Player.HasStatus(StatusEffectType.Stun))
                report.Fail("다음 턴에 패널티가 적용되지 않음");
            return report;
        }

        // S6: 플레이어 사망 → Defeat
        public static ScenarioReport S6_PlayerDeath_Defeat()
        {
            var report = new ScenarioReport { Name = "S6" };
            var s = NewBattle(5, 6, Monster("Grunt", 999, 5), Monster("Grunt", 999, 5));
            int turn = 0;
            bool toggle = false;
            while (!s.IsOver && turn < TurnCap)
            {
                turn++;
                s.BeginCommandPhase();
                RunEnemyTurn(s, ScenarioDefensePolicy.AlwaysFail, ref toggle);
                if (!s.IsOver) { s.EndTurn(); s.BeginTurn(); }
            }
            report.Turns = turn;
            if (turn >= TurnCap && !s.IsOver) report.Fail("턴 상한(50) 초과");
            if (s.Phase != BattlePhase.Defeat) report.Fail($"Defeat 아님: {s.Phase}");
            if (!s.Squad.IsDefeated) report.Fail("Squad 가 전멸하지 않음");
            return report;
        }

        // S7: 에너지 봉인(T5) — 방어 실패로 봉인되면 다음 턴 수집 0·비축 유지, 해제된 다음 턴엔 정상 수집
        public static ScenarioReport S7_EnergySeal_SkipsCollectKeepsReserve()
        {
            var report = new ScenarioReport { Name = "S7" };
            var s = NewBattle(30, 7, Monster("Sealer", 999, 1, specialInterval: 1, penalty: StatusEffectType.NoEnergyReserve));
            s.CollectEnergy(3);
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            s.ResolveEnemyIntent(0, false); // 방어 실패 → 봉인 예약
            s.EndTurn();
            s.BeginTurn();
            if (!s.IsEnergySealed) report.Fail("방어 실패 다음 턴에 봉인되지 않음");
            int gained = s.CollectEnergy(4);
            if (gained != 0) report.Fail($"봉인 턴 에너지 획득 {gained} != 0");
            if (s.Energy.Current != 3) report.Fail($"봉인 턴 비축 {s.Energy.Current} != 3");
            s.BeginCommandPhase();
            s.EndPlayerPhase();
            s.ResolveEnemyIntent(0, true); // 방어 성공 → 새 봉인 없음
            s.EndTurn();
            s.BeginTurn();
            if (s.IsEnergySealed) report.Fail("봉인이 풀리지 않음");
            gained = s.CollectEnergy(4);
            if (gained != 4 || s.Energy.Current != 7) report.Fail($"해제 턴 수집 {gained}, 에너지 {s.Energy.Current} != 7");
            report.Turns = s.Turn;
            return report;
        }

        // T10: 1스테이지 보스 '불편대왕' 기본값(코드로만 구성, asmdef 경계 때문에 AssetDatabase 는 쓰지 않는다)
        static MonsterDefinition BuildDefaultStage1Boss()
        {
            var m = ScriptableObject.CreateInstance<MonsterDefinition>();
            m.displayName = "불편대왕";
            m.maxHp = 80;
            m.attack = 4;
            m.defense = 1;
            m.randomness = 1f;
            m.actions = new List<MonsterActionDefinition>
            {
                new MonsterActionDefinition { id = "basic", kind = MonsterActionKind.Attack, weight = 1f, damageMultiplier = 1f },
                new MonsterActionDefinition { id = "heavy", kind = MonsterActionKind.Attack, weight = 0.6f, damageMultiplier = 1.75f, cooldownTurns = 1 },
                new MonsterActionDefinition { id = "rest", kind = MonsterActionKind.Charge, weight = 0.3f },
                new MonsterActionDefinition
                {
                    id = "special_burn", kind = MonsterActionKind.Special, weight = 0.8f, damageMultiplier = 1.5f, cooldownTurns = 2,
                    statuses = new List<StatusEffectSpec> { new StatusEffectSpec { type = StatusEffectType.Burn, turns = 2, magnitude = 2f } }
                },
                new MonsterActionDefinition
                {
                    id = "special_costup", kind = MonsterActionKind.Special, weight = 0.8f, damageMultiplier = 1f, cooldownTurns = 2,
                    statuses = new List<StatusEffectSpec> { new StatusEffectSpec { type = StatusEffectType.CostUp, turns = 2, magnitude = 1f } }
                },
            };
            m.specialMiniGameIds = new List<string> { "stage1_boss_special", "dummy_tap" };
            return m;
        }

        // S8: 1스테이지 보스. 플레이어 HP 999(먼저 죽어 평균 턴이 왜곡되는 것을 막는다), 예고 피해 상한은 기준 HP 30 으로 따로 잰다(D-12)
        public static ScenarioReport S8_Stage1Boss(MonsterDefinition boss = null)
        {
            var report = new ScenarioReport { Name = "S8" };
            if (boss == null) boss = BuildDefaultStage1Boss();
            var attack = Skill("Attack", 1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 1);
            const float forecastCap = 30f * 0.25f;
            int turnSum = 0;
            int maxForecast = 0;
            int specialForecastCount = 0;
            bool anyStatusApplied = false;
            for (int seed = 1; seed <= 10; seed++)
            {
                var s = NewBattle(999, seed, boss);
                bool toggle = false;
                int turn = 0;
                while (!s.IsOver && turn < TurnCap)
                {
                    turn++;
                    s.AddEnergy(3);
                    s.BeginCommandPhase();
                    while (s.CanCommand(attack)) s.TryCommand(attack, null);
                    if (!s.IsOver)
                    {
                        var (_, _, forecast) = RunEnemyTurn(s, ScenarioDefensePolicy.Alternate, ref toggle);
                        if (forecast > maxForecast) maxForecast = forecast;
                        if (forecast > forecastCap)
                            report.Fail($"시드{seed} 턴{turn}: 예고 피해 {forecast} > 기준HP25%({forecastCap})");
                        foreach (var it in s.Intents)
                            if (it.Kind == EnemyIntentKind.Special) specialForecastCount++;
                        if (s.Player.HasStatus(StatusEffectType.Burn) || s.Player.HasStatus(StatusEffectType.CostUp))
                            anyStatusApplied = true;
                    }
                    if (!s.IsOver) { s.EndTurn(); s.BeginTurn(); }
                }
                if (turn >= TurnCap && !s.IsOver) report.Fail($"시드{seed}: 턴 상한(50) 초과");
                turnSum += turn;
            }
            float avgTurns = turnSum / 10f;
            report.Turns = (int)Math.Round(avgTurns);
            report.Reasons.Add($"평균 턴 {avgTurns:F1}, 예고 피해 최댓값 {maxForecast}");
            if (avgTurns < 6f || avgTurns > 14f)
                report.Fail($"평균 턴 {avgTurns:F1} 이 6~14 밖");
            if (specialForecastCount == 0)
                report.Fail("모든 시드를 통틀어 Special 예고가 0회");
            if (!anyStatusApplied)
                report.Fail("상태이상이 한 번도 적용되지 않음");
            return report;
        }

        // T8(캠페인): 웨이브 밸런스 검증. 실제 에셋(MonsterDefinition)과 스테이지 배율로 전투를 seeds 번 돌린다.
        // 플레이어 모델: 기본 캐릭터(공격 3, 방어 0), 매 턴 에너지 3 → 강타(코스트2, +6) 가능하면 먼저, 남으면 공격(코스트1, +0).
        // 플레이어 HP 는 999(먼저 죽어 턴 수가 왜곡되지 않게), D-12 예고 피해 상한은 기준 HP(refPlayerHp=30)의 25% 로 잰다.
        // 판정: 모든 턴 실행 행동 ≤ MaxAttackersPerTurn, 예고 총 피해 ≤ 상한, 모든 시드가 maxTurnsToClear 턴 안에 Victory
        // (방어형이 끝없이 버티지 않음). requireKinds 에 적힌 예고 종류는 시드 전체에서 한 번 이상 나와야 한다.
        public static ScenarioReport RunWaveBalance(string name, IList<MonsterDefinition> wave, float hpMul, float atkMul,
            int maxTurnsToClear, int seeds = 10, float refPlayerHp = 30f, params EnemyIntentKind[] requireKinds)
        {
            var report = new ScenarioReport { Name = name };
            if (wave == null || wave.Count == 0) { report.Fail("웨이브가 비어 있음"); return report; }
            foreach (var m in wave)
                if (m == null) { report.Fail("웨이브에 null 몬스터"); return report; }

            var heavy = Skill("HeavyBlow", 2, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 6);
            var attack = Skill("Attack", 1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 0);
            float cap = refPlayerHp * 0.25f;
            int turnSum = 0, maxTurns = 0, maxForecast = 0, maxExecuted = 0;
            var seen = new HashSet<EnemyIntentKind>();
            var waveArr = new List<MonsterDefinition>(wave).ToArray();

            for (int seed = 1; seed <= seeds; seed++)
            {
                var squad = new Squad();
                squad.TryAdd(null, new Combatant("Player", 999, 3, 0));
                var s = new BattleState(squad, waveArr, seed, hpMul, atkMul);
                s.StartBattle();
                bool toggle = false;
                int turn = 0;
                int limit = Math.Max(maxTurnsToClear, TurnCap);
                while (!s.IsOver && turn < limit)
                {
                    turn++;
                    s.AddEnergy(3);
                    s.BeginCommandPhase();
                    while (s.CanCommand(heavy)) s.TryCommand(heavy, null);
                    while (s.CanCommand(attack)) s.TryCommand(attack, null);
                    if (!s.IsOver)
                    {
                        foreach (var it in s.Intents) seen.Add(it.Kind);
                        var (executed, _, forecast) = RunEnemyTurn(s, ScenarioDefensePolicy.Alternate, ref toggle);
                        if (forecast > maxForecast) maxForecast = forecast;
                        if (executed > maxExecuted) maxExecuted = executed;
                        // 로그가 길어지지 않게 턴 단위 실패는 처음 3건만 사유로 남긴다(판정은 그대로 FAIL)
                        if (executed > BattleState.MaxAttackersPerTurn)
                        {
                            if (report.Reasons.Count < 3) report.Fail($"시드{seed} 턴{turn}: 실행된 적 행동 {executed} > {BattleState.MaxAttackersPerTurn}");
                            else report.Pass = false;
                        }
                        if (forecast > cap)
                        {
                            if (report.Reasons.Count < 3) report.Fail($"시드{seed} 턴{turn}: 예고 피해 {forecast} > 기준HP25%({cap})");
                            else report.Pass = false;
                        }
                    }
                    if (!s.IsOver) { s.EndTurn(); s.BeginTurn(); }
                }
                if (s.Phase != BattlePhase.Victory)
                    report.Fail($"시드{seed}: {turn}턴 안에 처치 못함({s.Phase})");
                else if (turn > maxTurnsToClear)
                    report.Fail($"시드{seed}: 처치 {turn}턴 > 상한 {maxTurnsToClear}");
                turnSum += turn;
                if (turn > maxTurns) maxTurns = turn;
            }
            foreach (var k in requireKinds)
                if (!seen.Contains(k)) report.Fail($"{k} 예고가 한 번도 나오지 않음");
            float avg = turnSum / (float)seeds;
            report.Turns = (int)Math.Round(avg);
            report.Reasons.Add($"평균 {avg:F1}턴, 최대 {maxTurns}턴, 예고 피해 최댓값 {maxForecast}/{cap}, 행동 수 최댓값 {maxExecuted}");
            return report;
        }

        // T10: SquadEnergyRule(BattleFlow, 별도 asmdef 없음=Assembly-CSharp)은 이 클래스가 속한 BoolPyeon.Battle asmdef 에서
        // 참조할 수 없다(asmdef 경계). 그래서 같은 상수·같은 계산을 여기 로컬로 복제한다. 상수를 바꾸면 SquadEnergyRule.cs 도 같이 바꿀 것.
        const int LocalHighScorePerSuccess = 2;
        const int LocalVolumePerSuccess = 1;
        const int LocalVolumeComboBonusEvery = 4;
        const int LocalVolumeComboBonus = 1;

        static int GainForSuccessesLocal(CharacterTrait trait, int successes)
        {
            if (successes <= 0) return 0;
            if (trait == CharacterTrait.HighScore)
                return successes * LocalHighScorePerSuccess;
            int gain = successes * LocalVolumePerSuccess;
            gain += (successes / LocalVolumeComboBonusEvery) * LocalVolumeComboBonus;
            return gain;
        }

        // T10: 스쿼드(최대 3인) 밸런스 시뮬레이션에 쓰는 멤버 스펙. 클래스 하나 = 파일 하나 규칙 때문에 중첩 클래스로 둔다.
        public class SquadMemberSpec
        {
            public string Name;
            public int Hp;
            public int Atk;
            public int Def;
            public CharacterTrait Trait;
            public SkillDefinition[] Skills;
        }

        // T10: RunSquadBalance 결과 요약(로그·검증용). Reasons 에도 같은 내용을 문자열로 남긴다.
        public class SquadBalanceStats
        {
            public float AvgTurns;
            public int MaxTurns;
            public float AvgGainPerCollect;
            public float AvgGainPerTurn;
            public readonly Dictionary<string, int> SkillUses = new Dictionary<string, int>();
            public readonly Dictionary<string, float> AvgGainByMember = new Dictionary<string, float>();
        }

        // T10: 3인 스쿼드 × 웨이브 시뮬레이션. BattleController 의 턴 루프(멤버마다 수집→커맨드 반복 후 AdvanceMember, 다 돌면 적 턴)를 그대로 따른다.
        // 성공 횟수는 시드 기반 난수로 모델링한다: HighScore 0~2회, Volume 1~4회.
        public static ScenarioReport RunSquadBalance(string name, IList<SquadMemberSpec> members, IList<MonsterDefinition> wave,
            float hpMul, float atkMul, int minTurns, int maxTurns, int seeds = 10, SquadBalanceStats stats = null)
        {
            var report = new ScenarioReport { Name = name };
            if (members == null || members.Count == 0) { report.Fail("스쿼드가 비어 있음"); return report; }
            if (wave == null || wave.Count == 0) { report.Fail("웨이브가 비어 있음"); return report; }
            foreach (var m in wave)
                if (m == null) { report.Fail("웨이브에 null 몬스터"); return report; }

            var waveArr = new List<MonsterDefinition>(wave).ToArray();
            int turnSum = 0, maxTurnsSeen = 0;
            long totalGain = 0;
            int totalCollects = 0, totalSquadTurns = 0;
            float maxSkillCost = 0f;
            var skillUses = new Dictionary<string, int>();
            var gainByMember = new Dictionary<string, long>();
            var collectsByMember = new Dictionary<string, int>();
            foreach (var m in members)
            {
                gainByMember[m.Name] = 0;
                collectsByMember[m.Name] = 0;
                if (m.Skills != null)
                    foreach (var sk in m.Skills)
                        if (sk != null && sk.energyCost > maxSkillCost) maxSkillCost = sk.energyCost;
            }

            for (int seed = 1; seed <= seeds; seed++)
            {
                var squad = new Squad();
                foreach (var m in members)
                    squad.TryAdd(null, new Combatant(m.Name, m.Hp, m.Atk, m.Def));
                var s = new BattleState(squad, waveArr, seed, hpMul, atkMul);
                s.StartBattle();
                var rnd = new System.Random(seed * 1000 + 7);
                bool toggle = false;
                int turn = 0;
                int limit = Math.Max(maxTurns, TurnCap);
                while (!s.IsOver && turn < limit)
                {
                    turn++;
                    do
                    {
                        var spec = members[s.ActiveMemberIndex];
                        if (!s.IsEnergySealed)
                        {
                            int minSucc = spec.Trait == CharacterTrait.HighScore ? 0 : 1;
                            int maxSucc = spec.Trait == CharacterTrait.HighScore ? 2 : 4;
                            int successes = rnd.Next(minSucc, maxSucc + 1);
                            int desired = GainForSuccessesLocal(spec.Trait, successes);
                            int gained = s.CollectEnergy(desired);
                            totalGain += gained;
                            totalCollects++;
                            gainByMember[spec.Name] += gained;
                            collectsByMember[spec.Name]++;
                        }
                        s.BeginCommandPhase();
                        if (spec.Skills != null)
                        {
                            var sorted = new List<SkillDefinition>(spec.Skills);
                            sorted.Sort((a, b) => b.energyCost.CompareTo(a.energyCost));
                            foreach (var sk in sorted)
                            {
                                while (s.CanCommand(sk))
                                {
                                    s.TryCommand(sk, null);
                                    skillUses.TryGetValue(sk.displayName, out int c);
                                    skillUses[sk.displayName] = c + 1;
                                }
                            }
                        }
                    } while (!s.IsOver && s.AdvanceMember());
                    totalSquadTurns++;
                    if (s.IsOver) break;
                    s.EndPlayerPhase();
                    RunEnemyTurn(s, ScenarioDefensePolicy.Alternate, ref toggle);
                    if (!s.IsOver) { s.EndTurn(); s.BeginTurn(); }
                }
                if (s.Phase != BattlePhase.Victory)
                    report.Fail($"시드{seed}: {turn}턴 안에 처치 못함({s.Phase})");
                turnSum += turn;
                if (turn > maxTurnsSeen) maxTurnsSeen = turn;
            }

            float avgTurns = turnSum / (float)seeds;
            report.Turns = (int)Math.Round(avgTurns);
            if (avgTurns < minTurns || avgTurns > maxTurns)
                report.Fail($"평균 턴 {avgTurns:F1} 이 {minTurns}~{maxTurns} 밖");

            float avgGainPerCollect = totalCollects > 0 ? totalGain / (float)totalCollects : 0f;
            float avgGainPerTurn = totalSquadTurns > 0 ? totalGain / (float)totalSquadTurns : 0f;
            float r = maxSkillCost > 0f ? avgGainPerCollect / maxSkillCost : 0f;
            if (r < 0.8f || r > 1.5f)
                report.Fail($"수집당 획득/주력스킬코스트 비율 R={r:F2} 이 0.8~1.5 밖");

            var memberParts = new List<string>();
            foreach (var m in members)
            {
                float avg = collectsByMember[m.Name] > 0 ? gainByMember[m.Name] / (float)collectsByMember[m.Name] : 0f;
                memberParts.Add($"{m.Name}:{avg:F2}");
            }
            var skillParts = new List<string>();
            foreach (var kv in skillUses) skillParts.Add($"{kv.Key}:{kv.Value}");

            report.Reasons.Add(
                $"평균 {avgTurns:F1}턴(최대 {maxTurnsSeen}) | 수집당 획득 {avgGainPerCollect:F2}({string.Join(",", memberParts)}) | " +
                $"턴당 획득 {avgGainPerTurn:F2} | 스킬 사용 {{{string.Join(",", skillParts)}}} | R={r:F2}");

            if (stats != null)
            {
                stats.AvgTurns = avgTurns;
                stats.MaxTurns = maxTurnsSeen;
                stats.AvgGainPerCollect = avgGainPerCollect;
                stats.AvgGainPerTurn = avgGainPerTurn;
                foreach (var kv in skillUses) stats.SkillUses[kv.Key] = kv.Value;
                foreach (var m in members)
                    stats.AvgGainByMember[m.Name] = collectsByMember[m.Name] > 0 ? gainByMember[m.Name] / (float)collectsByMember[m.Name] : 0f;
            }
            return report;
        }

        // T10: 기본 3인 스펙(hero/cook/fashion). 스탯·trait 은 Hero.asset/Char_Cook.asset/Char_Fashion.asset 값과 같다.
        // 스킬은 Attack(코스트1)·HeavyBlow(코스트2, 데미지 6)로, AttackSkill.asset/HeavyBlowSkill.asset 코스트와 같다.
        public static List<SquadMemberSpec> DefaultSquadSpecs()
        {
            var attack = Skill("Attack", 1, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 0);
            var heavy = Skill("HeavyBlow", 2, SkillTargetType.SingleEnemy, SkillEffectKind.Damage, 6);
            return new List<SquadMemberSpec>
            {
                new SquadMemberSpec { Name = "hero", Hp = 30, Atk = 3, Def = 0, Trait = CharacterTrait.HighScore, Skills = new[] { attack, heavy } },
                new SquadMemberSpec { Name = "cook", Hp = 30, Atk = 3, Def = 0, Trait = CharacterTrait.Volume, Skills = new[] { attack, heavy } },
                new SquadMemberSpec { Name = "fashion", Hp = 30, Atk = 3, Def = 0, Trait = CharacterTrait.HighScore, Skills = new[] { attack, heavy } },
            };
        }

        // S12: 3인 스쿼드 vs 1스테이지 첫 잡몹 웨이브(쓰레기봉투류 2마리, S1_TrashBag 근사치). 목표 3~5턴.
        public static ScenarioReport S12_Squad_Stage1Trash()
        {
            var wave = new List<MonsterDefinition> { Monster("쓰레기봉투", 24, 2), Monster("쓰레기봉투", 24, 2) };
            // T10 조정: 3인 스쿼드는 1인 기준(S8)보다 화력이 3배 가까워 hpMul 1→2.2 로 올려야 목표 턴(3~5)에 맞는다.
            return RunSquadBalance("S12", DefaultSquadSpecs(), wave, 2.2f, 1f, 3, 5);
        }

        // S13: 3인 스쿼드 vs 엘리트(고무장갑 방패류 + 쓰레기봉투류, S1_RubberGloveShield 근사치). 목표 4~7턴.
        public static ScenarioReport S13_Squad_Stage1Elite()
        {
            var wave = new List<MonsterDefinition> { Monster("고무장갑방패", 34, 2), Monster("쓰레기봉투", 24, 2) };
            return RunSquadBalance("S13", DefaultSquadSpecs(), wave, 2.4f, 1f, 4, 7);
        }

        // S14: 3인 스쿼드 vs 1스테이지 보스(불편대왕). 목표 8~12턴.
        public static ScenarioReport S14_Squad_Stage1Boss()
        {
            var wave = new List<MonsterDefinition> { BuildDefaultStage1Boss() };
            // T10 조정: 3인 스쿼드 화력 때문에 hpMul 1→3.2 로 올려야 목표 턴(8~12)에 맞는다.
            return RunSquadBalance("S14", DefaultSquadSpecs(), wave, 3.2f, 1f, 8, 12);
        }

        public static ScenarioReport[] RunAllReports()
        {
            return new[]
            {
                S7_EnergySeal_SkipsCollectKeepsReserve(),
                S1_OneMonster_NormalAttackHits(),
                S2_ThreeMonsters_AlwaysFailEvade(),
                S3_GuardCommand_HalvesDamage(),
                S4_EnergyCostsAndCapAndCarryOver(),
                S5_SpecialInterval_PenaltyReserved(),
                S6_PlayerDeath_Defeat(),
                S8_Stage1Boss(),
                S12_Squad_Stage1Trash(),
                S13_Squad_Stage1Elite(),
                S14_Squad_Stage1Boss(),
            };
        }

        // RunCommand 로 부르기 쉽게: 전부 돌려 로그를 남기고 요약을 돌려준다.
        public static string RunAll()
        {
            var reports = RunAllReports();
            int pass = 0;
            foreach (var r in reports)
            {
                Debug.Log(r.ToString());
                if (r.Pass) pass++;
            }
            string summary = $"[Scenario] {pass}/{reports.Length} PASS";
            Debug.Log(summary);
            return summary;
        }
    }
}
