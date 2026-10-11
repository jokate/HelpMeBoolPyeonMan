using System;
using System.Collections.Generic;

namespace BoolPyeon.Battle
{
    // F-14 전투 한 판의 순수 턴 규칙. 씬 진행은 BattleController 가 맡는다.
    // 한 턴: BeginTurn → (EnergyCollect) AddEnergy → BeginCommandPhase → TryCommand* → EndPlayerPhase
    //        → ResolveEnemyIntent* → EndTurn → 다음 BeginTurn
    // 커맨드 페이즈에서는 공격/스킬/방어를 에너지가 남는 한 모두 쓸 수 있다.
    // 특수 페이즈는 몬스터 쪽(EnemySpecial): 살아있는 몬스터의 인텐트에 Special 이 하나라도 있으면 적 턴이 EnemySpecial 로 진입한다.
    public class BattleState
    {
        public const int MaxAttackersPerTurn = 2; // D-12

        public Squad Squad { get; }
        // T3: 행동 중인 멤버를 인덱스로 추적한다. Player 는 ActiveMember.Combatant 의 별칭으로 남긴다
        // (BattleController·FirstStrikeEffect·HUD 가 참조 중이라 컴파일 유지, 이름 정리는 T5/T6).
        int activeMemberIndex;
        public int ActiveMemberIndex => activeMemberIndex;
        public SquadMember ActiveMember => Squad.Members[activeMemberIndex];
        public Combatant Player => ActiveMember.Combatant;
        public EnergyPool Energy => ActiveMember?.Energy;
        public IReadOnlyList<SquadMember> Members => Squad.Members;
        public BattleEvents Events { get; } = new BattleEvents();

        // T3: 멤버 순회가 한 바퀴 끝났으면 true. BeginTurn 이 다시 첫 멤버로 리셋한다.
        public bool AllMembersActed { get; private set; }

        readonly List<Combatant> enemies = new List<Combatant>();
        readonly List<MonsterDefinition> monsterDefs = new List<MonsterDefinition>();
        readonly List<EnemyIntent> intents = new List<EnemyIntent>();
        readonly List<StatusEffect> pendingPenalties = new List<StatusEffect>();
        readonly List<int> pendingPenaltyTargets = new List<int>();
        readonly List<Dictionary<MonsterActionDefinition, int>> lastUsedTurn = new List<Dictionary<MonsterActionDefinition, int>>();
        readonly Random rng;
        int[] patternIndex;

        // T11: 보스 2페이즈. 몬스터 인덱스마다 한 번만 전환한다.
        readonly bool[] phase2;
        bool phase2CutPending;
        public int Phase2TransitionCount { get; private set; }
        public event Action<int> Phase2Entered;

        public IReadOnlyList<Combatant> Enemies => enemies;
        public IReadOnlyList<MonsterDefinition> MonsterDefinitions => monsterDefs;
        public IReadOnlyList<EnemyIntent> Intents => intents;
        public IReadOnlyList<StatusEffect> PendingPenalties => pendingPenalties;
        public int Turn { get; private set; }
        public BattlePhase Phase { get; private set; } = BattlePhase.EnergyCollect;

        // seed 를 주면 적 AI 선택이 재현된다 (테스트용)
        // T1: hpMul/atkMul 은 StageDefinition 의 몬스터 배율. 기본 1f 라 기존 호출은 그대로 동작한다.
        public BattleState(Squad squad, IReadOnlyList<MonsterDefinition> wave, int? seed = null, float hpMul = 1f, float atkMul = 1f)
        {
            rng = seed.HasValue ? new Random(seed.Value) : new Random();
            Squad = squad ?? throw new ArgumentNullException(nameof(squad));
            if (squad.Members.Count == 0) throw new ArgumentException("squad 에 멤버가 없습니다.", nameof(squad));
            if (wave != null)
            {
                foreach (var def in wave)
                {
                    if (def == null) continue;
                    monsterDefs.Add(def);
                    enemies.Add(Combatant.FromMonster(def, hpMul, atkMul));
                    lastUsedTurn.Add(new Dictionary<MonsterActionDefinition, int>());
                }
            }
            patternIndex = new int[enemies.Count];
            phase2 = new bool[enemies.Count];
        }

        // T11: 살아있는 첫 isBoss 몬스터 인덱스. 없으면 -1
        public int BossIndex
        {
            get
            {
                for (int i = 0; i < monsterDefs.Count; i++)
                    if (monsterDefs[i] != null && monsterDefs[i].isBoss) return i;
                return -1;
            }
        }

        public bool IsPhase2(int i) => i >= 0 && i < phase2.Length && phase2[i];

        // 격노 컷이 예약돼 있으면 한 번만 true 를 돌려주고 소비한다
        public bool ConsumePhase2Cut()
        {
            if (!phase2CutPending) return false;
            phase2CutPending = false;
            return true;
        }

        // 살아있는 isBoss 몬스터의 HP 가 임계치 이하이고 아직 전환하지 않았으면 2페이즈로 바꾼다(몬스터마다 1회)
        public void UpdateBossPhases()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                var def = monsterDefs[i];
                if (def == null || !def.isBoss || phase2[i] || enemies[i].IsDead) continue;
                if (def.phase2Threshold <= 0f) continue;
                if (enemies[i].Hp <= enemies[i].MaxHp * def.phase2Threshold)
                {
                    phase2[i] = true;
                    Phase2TransitionCount++;
                    phase2CutPending = true;
                    Phase2Entered?.Invoke(i);
                }
            }
        }

        public bool IsOver => Phase == BattlePhase.Victory || Phase == BattlePhase.Defeat;

        // 진행 중이면 null
        public BattlePhase? Outcome => IsOver ? Phase : (BattlePhase?)null;

        public bool AllEnemiesDead
        {
            get
            {
                foreach (var e in enemies)
                    if (!e.IsDead) return false;
                return true;
            }
        }

        public void StartBattle()
        {
            Turn = 0;
            Events.RaiseBattleStart();
            BeginTurn();
        }

        public void BeginTurn()
        {
            if (IsOver) return;
            Turn++;
            for (int i = 0; i < pendingPenalties.Count; i++)
            {
                var p = pendingPenalties[i];
                int targetIdx = i < pendingPenaltyTargets.Count ? pendingPenaltyTargets[i] : activeMemberIndex;
                if (targetIdx >= 0 && targetIdx < Squad.Members.Count && !Squad.Members[targetIdx].Combatant.IsDead)
                    Squad.Members[targetIdx].Combatant.AddStatus(p.Type, p.RemainingTurns, p.Magnitude);
            }
            pendingPenalties.Clear();
            pendingPenaltyTargets.Clear();

            // T7: Burn — 턴 시작 시 지속 피해. 대기 패널티 적용 직후, 인텐트 계획 전에 준다. 살아 있는 전 멤버에게 적용한다.
            foreach (var m in Squad.Members)
                if (!m.Combatant.IsDead) m.Combatant.TickBurn();
            foreach (var e in enemies)
                if (!e.IsDead) e.TickBurn();
            if (Squad.IsDefeated) { Phase = BattlePhase.Defeat; return; }
            if (AllEnemiesDead) { Phase = BattlePhase.Victory; return; }

            ResetMemberOrder();
            UpdateBossPhases();
            PlanIntents();
            Phase = BattlePhase.EnergyCollect;
            Events.RaiseTurnStart(Turn);
        }

        // T3: 살아 있는 첫 멤버로 순회를 리셋한다
        void ResetMemberOrder()
        {
            AllMembersActed = false;
            for (int i = 0; i < Squad.Members.Count; i++)
            {
                if (!Squad.Members[i].Combatant.IsDead)
                {
                    activeMemberIndex = i;
                    return;
                }
            }
            activeMemberIndex = 0;
        }

        // T3: 다음 살아 있는 멤버로 넘긴다. 더 없으면 AllMembersActed=true 로 두고 false 를 돌려준다.
        public bool AdvanceMember()
        {
            for (int i = activeMemberIndex + 1; i < Squad.Members.Count; i++)
            {
                if (!Squad.Members[i].Combatant.IsDead)
                {
                    activeMemberIndex = i;
                    return true;
                }
            }
            AllMembersActed = true;
            return false;
        }

        // T7: 재지정된 대상을 intent 별로 기억해 연출(컨트롤러)과 실제 피해(ResolveEnemyIntent)가 같은 멤버를 쓰게 한다.
        readonly Dictionary<EnemyIntent, int> retargets = new Dictionary<EnemyIntent, int>();

        // T7: 적 행동 index 의 실제 대상 멤버 인덱스. 계획 대상이 죽었으면 살아 있는 멤버로 다시 고른다(결과는 캐시). 전멸이면 -1.
        public int ResolveTargetMemberIndex(int intentIndex)
        {
            if (intentIndex < 0 || intentIndex >= intents.Count) return -1;
            var intent = intents[intentIndex];
            if (retargets.TryGetValue(intent, out int cached) && cached >= 0 && cached < Squad.Members.Count && !Squad.Members[cached].Combatant.IsDead)
                return cached;
            int idx = PickAliveMemberIndex(intent.TargetMemberIndex);
            if (idx != intent.TargetMemberIndex) retargets[intent] = idx;
            return idx;
        }

        // T7: 예고 표시용 — rng 를 쓰지 않는다. 계획 대상이 살아 있으면 그 인덱스, 아니면 -1.
        public int PeekTargetMemberIndex(int intentIndex)
        {
            if (intentIndex < 0 || intentIndex >= intents.Count) return -1;
            var intent = intents[intentIndex];
            if (retargets.TryGetValue(intent, out int cached) && cached >= 0 && cached < Squad.Members.Count && !Squad.Members[cached].Combatant.IsDead)
                return cached;
            int t = intent.TargetMemberIndex;
            return t >= 0 && t < Squad.Members.Count && !Squad.Members[t].Combatant.IsDead ? t : -1;
        }

        // T3: preferred 가 살아 있으면 그대로, 아니면 살아 있는 후보 중에서 고른다(후보 1명이면 rng 미소비). 없으면 -1.
        int PickAliveMemberIndex(int preferred = -1)
        {
            if (preferred >= 0 && preferred < Squad.Members.Count && !Squad.Members[preferred].Combatant.IsDead)
                return preferred;
            var candidates = new List<int>();
            for (int i = 0; i < Squad.Members.Count; i++)
                if (!Squad.Members[i].Combatant.IsDead) candidates.Add(i);
            if (candidates.Count == 0) return -1;
            if (candidates.Count == 1) return candidates[0];
            return candidates[rng.Next(candidates.Count)];
        }

        void PlanIntents()
        {
            intents.Clear();
            int attackers = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                var e = enemies[i];
                var def = monsterDefs[i];
                var kind = EnemyIntentKind.Wait;
                int damage = 0;
                MonsterActionDefinition action = null;
                int targetIdx = -1;
                if (!e.IsDead && attackers < MaxAttackersPerTurn)
                {
                    targetIdx = PickAliveMemberIndex();
                    var target = targetIdx >= 0 ? Squad.Members[targetIdx].Combatant : Player;
                    var targetEnergy = targetIdx >= 0 ? Squad.Members[targetIdx].Energy : Energy;
                    if (def.actions != null && def.actions.Count > 0)
                    {
                        // Utility AI: 점수 비율대로 행동을 고른다
                        var used = lastUsedTurn[i];
                        var ctx = new EnemyAIContext(e, target, targetEnergy, Turn,
                            a => used.TryGetValue(a, out int t) ? t : -1);
                        IList<MonsterActionDefinition> pool = def.actions;
                        // T11: 2페이즈면 phase2Actions 를 후보에 더한다(주기 단축·행동 추가를 동시에 만족)
                        if (phase2[i] && def.phase2Actions != null && def.phase2Actions.Count > 0)
                        {
                            var combined = new List<MonsterActionDefinition>(def.actions);
                            combined.AddRange(def.phase2Actions);
                            pool = combined;
                        }
                        action = EnemyAI.Pick(pool, ctx, rng, def.randomness);
                        if (action != null)
                        {
                            used[action] = Turn;
                            switch (action.kind)
                            {
                                case MonsterActionKind.Attack:
                                    kind = EnemyIntentKind.Attack;
                                    damage = Math.Max(0, (int)Math.Ceiling(e.Attack * (double)action.damageMultiplier));
                                    break;
                                case MonsterActionKind.Special:
                                    kind = EnemyIntentKind.Special;
                                    damage = Math.Max(0, (int)Math.Ceiling(e.Attack * (double)action.damageMultiplier));
                                    break;
                                case MonsterActionKind.Magic:
                                    kind = EnemyIntentKind.Magic;
                                    damage = Math.Max(0, (int)Math.Ceiling(e.Attack * (double)action.damageMultiplier));
                                    break;
                                case MonsterActionKind.Guard:
                                    kind = EnemyIntentKind.Guard;
                                    break;
                                case MonsterActionKind.Buff:
                                    kind = EnemyIntentKind.Buff;
                                    break;
                                case MonsterActionKind.Heal:
                                    kind = EnemyIntentKind.Heal;
                                    break;
                                // Charge 등 그 외는 Wait 유지
                            }
                        }
                    }
                    else if (def.specialInterval > 0 && Turn % def.specialInterval == 0)
                    {
                        kind = EnemyIntentKind.Special;
                        damage = (int)Math.Ceiling(e.Attack * def.specialDamageMultiplier);
                    }
                    else if (CurrentAction(i) == MonsterActionKind.Attack)
                    {
                        kind = EnemyIntentKind.Attack;
                        damage = e.Attack;
                    }
                    if (kind != EnemyIntentKind.Wait) attackers++;
                }
                bool isAttackKind = kind == EnemyIntentKind.Attack || kind == EnemyIntentKind.Special || kind == EnemyIntentKind.Magic;
                intents.Add(new EnemyIntent(i, kind, damage, action, isAttackKind ? targetIdx : -1));
            }
        }

        MonsterActionKind CurrentAction(int i)
        {
            var pattern = monsterDefs[i].actionPattern;
            if (pattern == null || pattern.Count == 0) return MonsterActionKind.Attack;
            return pattern[patternIndex[i] % pattern.Count];
        }

        public void AddEnergy(int amount) => Energy.Add(amount);

        // T8: 멤버를 지정해 에너지를 더한다. 범위를 벗어나면 무시한다.
        public void AddEnergyTo(int memberIndex, int amount)
        {
            if (memberIndex < 0 || memberIndex >= Squad.Members.Count) return;
            Squad.Members[memberIndex].Energy.Add(amount);
        }

        // T5: 에너지 봉인(NoEnergyReserve) — 이번 턴 에너지 수집 페이즈를 건너뛴다. 비축분은 그대로 둔다.
        public bool IsEnergySealed => Player.HasStatus(StatusEffectType.NoEnergyReserve);

        // 수집 페이즈의 에너지 획득. 봉인이면 0 을 돌려주고 아무것도 더하지 않는다. 실제로 늘어난 양을 돌려준다.
        public int CollectEnergy(int amount)
        {
            if (IsEnergySealed || amount <= 0) return 0;
            int before = Energy.Current;
            Energy.Add(amount);
            return Energy.Current - before;
        }

        // F-28: 반복 턴 모듈 — 이번 턴에 추가 커맨드 페이즈 1회를 예약한다(에너지 재수집 없이).
        bool extraCommandPhasePending;
        public void GrantExtraCommandPhase() => extraCommandPhasePending = true;

        public bool ConsumeExtraCommandPhase()
        {
            if (!extraCommandPhasePending) return false;
            extraCommandPhasePending = false;
            return true;
        }

        public bool IsPlayerStunned => Player.HasStatus(StatusEffectType.Stun);

        // T7: Rush — 다음 에너지 수집 제한시간 배율. 실제 미니게임 연결은 T8.
        public const float RushTimeScale = 0.7f;
        public float EnergyCollectTimeScale => Player.HasStatus(StatusEffectType.Rush) ? RushTimeScale : 1f;

        // T7: UI 용 — 플레이어의 커맨드 코스트 증가량
        public int PlayerCostIncrease => Player.CostIncrease;

        // 살아있는 몬스터의 이번 턴 인텐트에 Special 이 하나라도 있으면 적 턴은 EnemySpecial
        public bool HasSpecialIntent
        {
            get
            {
                foreach (var it in intents)
                {
                    if (it.Kind != EnemyIntentKind.Special) continue;
                    if (it.MonsterIndex < 0 || it.MonsterIndex >= enemies.Count) continue;
                    if (!enemies[it.MonsterIndex].IsDead) return true;
                }
                return false;
            }
        }

        public void BeginCommandPhase()
        {
            if (IsOver) return;
            Phase = BattlePhase.Command;
        }

        static bool PhaseAllows(BattlePhase phase, SkillDefinition skill)
        {
            if (skill == null) return false;
            return phase == BattlePhase.Command;
        }

        public bool CanCommand(SkillDefinition skill)
        {
            return PhaseAllows(Phase, skill) && SkillResolver.CanUse(skill, Player, Energy);
        }

        // 에너지가 되는 한 여러 번 행동한다: 성공해도 페이즈를 유지하고, 적이 모두 죽으면 Victory
        public bool TryCommand(SkillDefinition skill, Combatant target)
        {
            if (!PhaseAllows(Phase, skill)) return false;
            bool ok = SkillResolver.TryUse(skill, Player, Energy, enemies, target, Events);
            if (!ok) return false;
            if (AllEnemiesDead) Phase = BattlePhase.Victory;
            return true;
        }

        // 지금 페이즈에서 쓸 수 있는 커맨드가 하나라도 있으면 true (null 항목은 CanCommand 가 false)
        public bool CanAnyCommand(IEnumerable<SkillDefinition> skills)
        {
            if (skills == null) return false;
            foreach (var s in skills)
                if (CanCommand(s)) return true;
            return false;
        }

        public void EndPlayerPhase()
        {
            if (IsOver) return;
            UpdateBossPhases();
            Phase = HasSpecialIntent ? BattlePhase.EnemySpecial : BattlePhase.Enemy;
            Events.RaisePlayerPhaseEnd();
        }

        // 회피/방어 미니게임 대상이 되는 인텐트인지 (Special: 방어, Magic: 회피)
        public static bool IsEvadable(EnemyIntentKind k) => k == EnemyIntentKind.Special || k == EnemyIntentKind.Magic;

        // 몬스터 한 마리의 예고 행동을 실행하고 주인공이 받은 피해를 돌려준다
        public int ResolveEnemyIntent(int index) => ResolveEnemyIntent(index, false);

        // defended=true 면 특수·마법 방어(회피) 미니게임 성공: 피해 0, 패널티 미예약. 일반 공격은 항상 defended=false 로 들어온다(T6)
        public int ResolveEnemyIntent(int index, bool defended)
        {
            if ((Phase != BattlePhase.Enemy && Phase != BattlePhase.EnemySpecial) || index < 0 || index >= intents.Count) return 0;
            var intent = intents[index];
            var enemy = enemies[intent.MonsterIndex];
            if (enemy.IsDead || intent.Kind == EnemyIntentKind.Wait) return 0;

            // T4: 적 진영 지원 행동(Guard/Buff/Heal)은 회피와 무관하게 먼저 처리하고 0을 반환한다.
            if (intent.Kind == EnemyIntentKind.Guard || intent.Kind == EnemyIntentKind.Buff || intent.Kind == EnemyIntentKind.Heal)
            {
                ResolveSupportIntent(intent, enemy);
                return 0;
            }

            // T3: 계획 당시 대상이 그새 죽었으면(예: 다른 공격으로 먼저 처치) 살아 있는 다른 멤버로 다시 고른다.
            int targetIdx = ResolveTargetMemberIndex(index);
            retargets.Remove(intent);
            if (targetIdx < 0) targetIdx = 0;
            var target = Squad.Members[targetIdx].Combatant;

            if (defended)
            {
                Events.RaiseDamageDealt(target, 0);
                return 0;
            }

            bool isMagic = intent.Kind == EnemyIntentKind.Magic;
            int rawDamage = enemy.ModifyOutgoingDamage(intent.Damage);
            int dealt = target.TakeDamage(rawDamage, isMagic);
            Events.RaiseDamageDealt(target, dealt);
            if (intent.Kind == EnemyIntentKind.Special || intent.Kind == EnemyIntentKind.Magic)
            {
                var def = monsterDefs[intent.MonsterIndex];
                if (intent.Action != null)
                {
                    // 데이터 기반: 행동에 적힌 상태이상만 부여 (비어 있으면 없음)
                    if (intent.Action.statuses != null)
                        foreach (var spec in intent.Action.statuses)
                            if (spec != null && spec.turns > 0)
                            {
                                pendingPenalties.Add(new StatusEffect(spec.type, spec.turns, spec.magnitude));
                                pendingPenaltyTargets.Add(targetIdx);
                            }
                }
                else if (intent.Kind == EnemyIntentKind.Special && def.penaltyTurns > 0)
                {
                    pendingPenalties.Add(new StatusEffect(def.penalty, def.penaltyTurns, def.penaltyMagnitude));
                    pendingPenaltyTargets.Add(targetIdx);
                }
            }
            if (Squad.IsDefeated) Phase = BattlePhase.Defeat;
            return dealt;
        }

        // T4: Guard/Buff/Heal — 대상은 targetsAllies=false 면 자신, true 면 Guard/Buff 는 살아있는 아군 전체, Heal 은 HP 비율이 가장 낮은 아군.
        void ResolveSupportIntent(EnemyIntent intent, Combatant enemy)
        {
            var action = intent.Action;
            bool targetsAllies = action != null && action.targetsAllies;
            int effectTurns = action != null && action.effectTurns > 0 ? action.effectTurns : 1;
            float magnitude = action != null ? action.effectMagnitude : 0f;
            // EndTurn 이 적 턴과 같은 턴에 상태이상을 틱하므로 +1 로 걸어 다음 턴에도 유지되게 한다.
            int turns = effectTurns + 1;

            switch (intent.Kind)
            {
                case EnemyIntentKind.Guard:
                {
                    float mag = magnitude > 0f ? magnitude : Combatant.DefaultGuardedMagnitude;
                    if (targetsAllies)
                    {
                        foreach (var e in enemies)
                            if (!e.IsDead) e.AddStatus(StatusEffectType.Guarded, turns, mag);
                    }
                    else
                    {
                        enemy.AddStatus(StatusEffectType.Guarded, turns, mag);
                    }
                    break;
                }
                case EnemyIntentKind.Buff:
                {
                    float mag = magnitude > 0f ? magnitude : Combatant.DefaultAttackUpMagnitude;
                    if (targetsAllies)
                    {
                        foreach (var e in enemies)
                            if (!e.IsDead) e.AddStatus(StatusEffectType.AttackUp, turns, mag);
                    }
                    else
                    {
                        enemy.AddStatus(StatusEffectType.AttackUp, turns, mag);
                    }
                    break;
                }
                case EnemyIntentKind.Heal:
                {
                    Combatant target = enemy;
                    if (targetsAllies)
                    {
                        Combatant lowest = null;
                        float lowestRatio = float.MaxValue;
                        foreach (var e in enemies)
                        {
                            if (e.IsDead) continue;
                            float ratio = e.MaxHp > 0 ? (float)e.Hp / e.MaxHp : 0f;
                            if (ratio < lowestRatio) { lowestRatio = ratio; lowest = e; }
                        }
                        if (lowest != null) target = lowest;
                    }
                    float healMul = action != null && action.damageMultiplier > 0f ? action.damageMultiplier : 1f;
                    int amount = Math.Max(1, (int)Math.Ceiling(enemy.Attack * healMul));
                    target.Heal(amount);
                    break;
                }
            }
        }

        public void EndTurn()
        {
            if (IsOver) return;
            foreach (var m in Squad.Members)
                if (!m.Combatant.IsDead) m.Combatant.EndTurn();
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].EndTurn();
                patternIndex[i]++;
            }
            Events.RaiseEnemyPhaseEnd();
        }

        // 디버그용: 적 전원 처치
        public void DebugDefeatAllEnemies()
        {
            foreach (var e in enemies)
                while (!e.IsDead) e.TakeDamage(9999);
            if (!IsOver) Phase = BattlePhase.Victory;
        }

        // 디버그용: 편성 전원 즉시 사망(전멸)
        // T8: 지정한 멤버 한 명만 즉시 사망시킨다. 전멸이면 패배로 전환한다.
        public void DebugKillMember(int memberIndex)
        {
            if (memberIndex < 0 || memberIndex >= Squad.Members.Count) return;
            var m = Squad.Members[memberIndex];
            while (!m.Combatant.IsDead) m.Combatant.TakeDamage(9999);
            if (Squad.IsDefeated && !IsOver) Phase = BattlePhase.Defeat;
        }

        public void DebugKillPlayer()
        {
            foreach (var m in Squad.Members)
                while (!m.Combatant.IsDead) m.Combatant.TakeDamage(9999);
            if (!IsOver) Phase = BattlePhase.Defeat;
        }
    }
}
