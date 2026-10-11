using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BoolPyeon.Battle;
using BoolPyeon.Core;
using BoolPyeon.Input;
using BoolPyeon.MiniGames;
using BoolPyeon.MiniGames.Phase;
using BoolPyeon.UI;

namespace BoolPyeon.BattleFlow
{
    /// <summary>
    /// Battle.unity 진행: 에너지 수집(MiniGamePhase) → 커맨드 → 적 턴을 승패가 날 때까지 반복한다.
    /// 규칙은 BattleState 가 가지고, 여기서는 연출·입력·페이즈 전환만 한다.
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        // BattleDebug 가 씬 재시작 전에 설정 (0 이면 wave 그대로)
        public static int DebugWaveCount;
        // T11.1: 에디터 전용·1회성 '보스 방 바로 진입' 디버그 플래그 키. PlayFromScratchMenu 가 설정한다.
        public const string DebugBossRoomKey = "BattleDebugBossRoom";
        // T6.2: 에디터 전용·1회성 '엘리트 방(ActiveEliteRooms[0]) 바로 진입' 디버그 플래그 키.
        // 맵 시드가 무작위라 자연 진입을 보장할 수 없어, 신규 몬스터 인텐트 예고를 실제 전투 흐름에서 확인하려고 추가.
        public const string DebugEliteRoomKey = "BattleDebugEliteRoom";
        // T13: 스테이지 클리어 보너스(정화 결정). 기존 값 10 을 한 곳에서 보게 뽑아 둔다.
        public const int StageClearBonus = 10;

        public CharacterDefinition character;
        public List<MonsterDefinition> wave = new List<MonsterDefinition>();
        // F-17: 방(웨이브 연속)을 진행한다. null 이면 기존 wave 리스트 1웨이브로 동작한다.
        public RoomDefinition room;
        public MiniGamePhase phase;
        // F-30: 스쿼드 편성 후보(리더 포함). SquadService.GetSquad 가 SaveData.squadIds 순서로 골라
        // phase.SetSquad 에 넘길 풀·에너지 규칙을 만든다.
        public List<CharacterDefinition> squadCandidates = new List<CharacterDefinition>();
        public List<MiniGameDefinition> specialPool = new List<MiniGameDefinition>();
        public GameObject phaseHudRoot;
        public BattleHud hud;
        // F-19: 방 클리어 강화 선택
        public List<UpgradeDefinition> upgradePool = new List<UpgradeDefinition>();
        public UpgradeChoiceHud upgradeHud;
        // F-18: 맵·방 선택. mapHud 가 없으면(레거시 씬) 기존처럼 room/wave 필드를 그대로 한 번 진행한다.
        public MapHud mapHud;
        public List<RoomDefinition> combatRooms = new List<RoomDefinition>();
        public List<RoomDefinition> eliteRooms = new List<RoomDefinition>();
        // T4: 보스 방 전용 정의. null 이면 eliteRooms[0] → combatRooms[0] 순서로 대신 쓴다.
        public RoomDefinition bossRoom;
        // T1: RunState.StageNumber 에 맞는 항목을 골라 아래 combatRooms/eliteRooms/bossRoom 대신 쓴다.
        // 맞는 항목이 없으면 기존 필드로 대체 동작한다.
        public List<StageDefinition> stages = new List<StageDefinition>();
        private StageDefinition activeStage;
        // T2: stages 가운데 가장 큰 stageNumber(없으면 1). 새 런 시작 스테이지의 상한으로 쓴다.
        private int MaxStageNumber()
        {
            int max = 1;
            if (stages != null)
            {
                foreach (var s in stages)
                    if (s != null && s.stageNumber > max) max = s.stageNumber;
            }
            return max;
        }
        private List<RoomDefinition> ActiveCombatRooms => activeStage != null ? activeStage.combatRooms : combatRooms;
        private List<RoomDefinition> ActiveEliteRooms => activeStage != null ? activeStage.eliteRooms : eliteRooms;
        private RoomDefinition ActiveBossRoom => activeStage != null ? activeStage.bossRoom : bossRoom;
        private float ActiveHpMultiplier => activeStage != null ? activeStage.monsterHpMultiplier : 1f;
        private float ActiveAttackMultiplier => activeStage != null ? activeStage.monsterAttackMultiplier : 1f;
        public int mapSeed;
        public int restHealPercent = 30;
        public BattleUnitView playerView;
        public BattleUnitView[] enemyViews = new BattleUnitView[3];
        // 후방 지원 스쿼드(리더 제외) 표시 자리. 스킬엔 같이 공격하고 방어엔 같이 막는다(연출만, 전투 수치 없음).
        public BattleUnitView[] supportViews = new BattleUnitView[2];
        public GameObject battleWorld;
        public float titleCutTime = 0.8f;
        public float enemyActionDelay = 0.5f;
        // T2.2: 스테이지 클리어 결과 표시 후 HQ 자동 복귀까지 대기(초)
        public float stageClearReturnDelay = 3f;
        private bool returningToHQ;
        // F-21: 결과 패널의 '본부로' 로 이동할 씬.
        public string hqSceneName = "HQ";
        // F-23: 튜토리얼 런. tutorial 이 null 이거나 tutorialRoom 이 없으면 튜토리얼을 건너뛴다.
        public TutorialDirector tutorial;
        public RoomDefinition tutorialRoom;
        private bool isTutorial;
        // F-27: 허브에서 구매한 영구 강화. 새 런/복원 시 RunState 에 적용한다.
        public List<BoolPyeon.Meta.PermanentUpgradeDefinition> permanentUpgrades = new List<BoolPyeon.Meta.PermanentUpgradeDefinition>();
        // F-28: 장착 후보 모듈 전체. 실제 발동은 SaveService.equippedModules 에 들어있는 것만.
        public List<BoolPyeon.Meta.ModuleDefinition> modules = new List<BoolPyeon.Meta.ModuleDefinition>();
        private readonly List<BoolPyeon.Meta.IModuleEffect> activeModuleEffects = new List<BoolPyeon.Meta.IModuleEffect>();

        // T-Battle: 켜져 있으면 사람 입력 없이 전투를 끝까지 진행한다(디버그 전용).
        public BattleAutoPilot autoPilot;

        public BattleState State { get; private set; }

        private Combatant target;
        private bool commandDone;

        // T5: 지금 행동 중인 멤버의 CharacterDefinition. ActiveMember 가 없으면 리더(character)로 폴백한다.
        private CharacterDefinition ActingCharacter => State?.ActiveMember?.Definition ?? character;

        // T5: 행동 중인 멤버의 스킬 목록. 리더(== character)면 RunState.Current.Skills(런 보상 반영), 아니면 정의 그대로.
        private List<SkillDefinition> ActingSkills()
        {
            var acting = ActingCharacter;
            if (acting == character && RunState.Current != null) return RunState.Current.Skills;
            return acting != null ? acting.skills : null;
        }

        private void Start()
        {
            if (hud != null)
            {
                hud.AttackClicked += OnAttack;
                hud.GuardClicked += OnGuard;
                hud.SkillClicked += OnSkill;
                hud.EndTurnClicked += OnEndTurn;
                hud.RetryClicked += OnRetry;
            }
            StartCoroutine(BattleLoop());
        }

        // 커맨드 페이즈 한 번(기절이면 표시만). F-28 반복 턴이 이걸 한 번 더 부른다.
        private IEnumerator RunCommandPhase()
        {
            State.BeginCommandPhase();
            EnsureTarget();
            RefreshAll();
            if (State.IsPlayerStunned)
            {
                yield return TitleCut("기절!");
                yield break;
            }
            commandDone = false;
            if (autoPilot != null && autoPilot.IsActive())
            {
                while (!commandDone && !State.IsOver)
                {
                    SkillDefinition skill = autoPilot.ChooseCommand(State);
                    if (skill != null) UseCommand(skill);
                    else OnEndTurn();
                }
                yield break;
            }
            if (hud != null) hud.ShowCommand(true);
            RefreshAll();
            yield return new WaitUntil(() => commandDone || State.IsOver);
            if (hud != null) hud.ShowCommand(false);
        }

        private List<MonsterDefinition> BuildWave()
        {
            var result = new List<MonsterDefinition>();
            foreach (var m in wave)
                if (m != null) result.Add(m);
            if (DebugWaveCount > 0 && result.Count > 0)
            {
                var source = new List<MonsterDefinition>(result);
                result.Clear();
                for (int i = 0; i < DebugWaveCount; i++) result.Add(source[i % source.Count]);
            }
            int maxSlots = enemyViews != null ? enemyViews.Length : 0;
            if (result.Count > maxSlots) result.RemoveRange(maxSlots, result.Count - maxSlots);
            return result;
        }

        // T2: RunState 생성용 편성 전원(리더 포함, 순서대로). squadCandidates 가 비어 있으면 리더 한 명.
        private List<CharacterDefinition> GetRunSquadMembers()
        {
            if (squadCandidates != null && squadCandidates.Count > 0)
            {
                var members = new BoolPyeon.Meta.SquadService(BoolPyeon.Save.SaveService.Default).GetSquad(squadCandidates);
                if (members != null && members.Count > 0) return members;
            }
            return new List<CharacterDefinition> { character };
        }

        private RoomProgress room_progress;

        // T6: index 0 = playerView(리더), 그 밖은 supportViews[idx-1]. 범위 밖이면 null.
        private BattleUnitView MemberView(int idx)
        {
            if (idx == 0) return playerView;
            int si = idx - 1;
            return supportViews != null && si >= 0 && si < supportViews.Length ? supportViews[si] : null;
        }

        // view 가 세 스쿼드 유닛(리더·서포트) 중 하나인지 — 적 타겟 클릭 판정에서 제외한다.
        private bool IsMemberView(BattleUnitView view)
        {
            if (view == null) return false;
            if (view == playerView) return true;
            if (supportViews != null)
                foreach (var v in supportViews)
                    if (v == view) return true;
            return false;
        }

        // 웨이브(=State) 하나가 바뀔 때마다 뷰·HUD 를 다시 연결한다. state.Members 를 인덱스로 그대로 뷰에 묶는다.
        private void BindMemberViews(BattleState state)
        {
            var members = state != null ? state.Members : null;
            int count = members != null ? members.Count : 0;
            for (int i = 0; i <= (supportViews != null ? supportViews.Length : 0); i++)
            {
                BattleUnitView view = MemberView(i);
                if (view == null) continue;
                if (i < count && members[i] != null)
                {
                    view.SetMemberLook(members[i].Definition);
                    view.Bind(members[i].Combatant);
                }
                else
                {
                    view.Bind(null);
                }
            }
        }

        private void SetupWave(BattleState state)
        {
            DetachModules();
            BindMemberViews(state);
            for (int i = 0; i < enemyViews.Length; i++)
            {
                if (enemyViews[i] == null) continue;
                if (i < state.Enemies.Count)
                {
                    var def = i < state.MonsterDefinitions.Count ? state.MonsterDefinitions[i] : null;
                    if (def != null)
                    {
                        enemyViews[i].ApplyLook(def.tint, def.spriteOverride);
                        enemyViews[i].SetActionSprites(def.attackSprite, def.hitSprite);
                    }
                }
                enemyViews[i].Bind(i < state.Enemies.Count ? state.Enemies[i] : null);
            }
            if (hud != null) hud.ClearLog();
            SetSkills();
            AttachModules(state);
        }

        // F-28: 웨이브 State 마다 장착된 모듈 효과를 다시 붙인다(웨이브가 바뀌면 State.Events 도 바뀐다).
        private void AttachModules(BattleState state)
        {
            var save = BoolPyeon.Save.SaveService.Default;
            activeModuleEffects.AddRange(
                BoolPyeon.Meta.ModuleService.AttachEquipped(modules, save, state.Events, state, character));
        }

        private void DetachModules()
        {
            foreach (var effect in activeModuleEffects)
                effect?.Detach();
            activeModuleEffects.Clear();
        }

        private IEnumerator BattleLoop()
        {
            if (character == null)
            {
                UnityEngine.Debug.LogError("BattleController: character 가 비어 있습니다.");
                yield break;
            }
            var saveService = BoolPyeon.Save.SaveService.Default;
            isTutorial = !saveService.Data.tutorialDone && tutorialRoom != null;
            if (isTutorial)
            {
                Log("[Tutorial] start (new save)");
            }
            else
            {
                Log("[Tutorial] skip (done)");
            }

            if (RunState.Current == null)
            {
                var saved = saveService;
                if (isTutorial)
                {
                    RunState.Current = new RunState(GetRunSquadMembers());
                }
                else if (saved.Data.hasRun)
                {
                    var restoreSquad = GetRunSquadMembers();
                    if (restoreSquad == null || restoreSquad.Count == 0)
                        restoreSquad = new List<CharacterDefinition> { character };
                    RunState.Current = BoolPyeon.Save.RunSaveMapper.Restore(saved.Data.run, restoreSquad, upgradePool);
                    Log("[Save] run loaded");
                }
                else
                {
                    RunState.Current = new RunState(GetRunSquadMembers());
                    int startStage = RunState.ComputeStartStage(saved.Data.highestClearedStage, MaxStageNumber());
                    RunState.Current.StageNumber = startStage;
                    Log($"[Stage] new run stage={startStage}");
                }
                // F-27: 새 런/복원 둘 다 영구 강화를 다시 계산해 적용한다(스냅샷에는 넣지 않는다).
                BoolPyeon.Meta.PermanentUpgradeService.ApplyTo(RunState.Current, permanentUpgrades, saved);
            }

            int stageNumberForSelect = RunState.Current != null ? RunState.Current.StageNumber : 1;
            activeStage = stages != null ? stages.Find(s => s != null && s.stageNumber == stageNumberForSelect) : null;
            Log($"[Stage] {stageNumberForSelect} def={(activeStage != null ? activeStage.name : "fallback")}");

            if (isTutorial)
            {
                room = tutorialRoom;
                if (tutorial != null) yield return tutorial.PlayOnce(tutorial.opening, "opening");
            }

            bool useMap = !isTutorial && mapHud != null && (ActiveCombatRooms.Count > 0 || ActiveEliteRooms.Count > 0);
            if (useMap)
            {
                if (RunState.Current.Map == null)
                {
                    int seed = mapSeed != 0 ? mapSeed : UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                    RunState.Current.Seed = seed;
                    RunState.Current.Map = StageMaze.Generate(seed, ActiveCombatRooms.Count, ActiveEliteRooms.Count);
                    Log($"[Map] seed={seed}");
                    SaveRun();

                    // T3: 새 런의 첫 방에서만 스테이지 타이틀 컷을 한 번 보여준다.
                    string stageTitle = activeStage != null
                        ? $"스테이지 {stageNumberForSelect} · {activeStage.displayName}"
                        : $"스테이지 {stageNumberForSelect}";
                    Log($"[Stage] title stage={stageNumberForSelect}");
                    yield return TitleCut(stageTitle);
                }
                StageMaze maze = RunState.Current.Map;
                RoomDefinition selectedRoom = null;
#if UNITY_EDITOR
                if (UnityEngine.PlayerPrefs.GetInt(DebugBossRoomKey, 0) == 1 && ActiveBossRoom != null)
                {
                    UnityEngine.PlayerPrefs.SetInt(DebugBossRoomKey, 0);
                    UnityEngine.PlayerPrefs.Save();
                    selectedRoom = ActiveBossRoom;
                    Log("[Debug] boss room direct");
                }
                else if (UnityEngine.PlayerPrefs.GetInt(DebugEliteRoomKey, 0) == 1 && ActiveEliteRooms.Count > 0)
                {
                    UnityEngine.PlayerPrefs.SetInt(DebugEliteRoomKey, 0);
                    UnityEngine.PlayerPrefs.Save();
                    selectedRoom = ActiveEliteRooms[0];
                    Log("[Debug] elite room direct");
                }
#endif
                while (selectedRoom == null)
                {
                    SetWorld(false);
                    MazeRoom node = maze.Current;
                    if (node == null)
                    {
                        UnityEngine.Debug.LogError("BattleController: 미로 현재 방 없음");
                        yield break;
                    }

                    if (!node.cleared && node.type == MazeRoomType.Rest)
                    {
                        yield return TitleCut("휴식 +HP");
                        RunState.Current.HealPercent(restHealPercent);
                        Log($"[Map] rest heal +{restHealPercent}%");
                        maze.CompleteCurrent();
                        SaveRun();
                        continue;
                    }

                    if (!node.cleared && (node.type == MazeRoomType.Combat || node.type == MazeRoomType.Elite))
                    {
                        List<RoomDefinition> pool = node.type == MazeRoomType.Elite ? ActiveEliteRooms : ActiveCombatRooms;
                        selectedRoom = (pool != null && node.roomIndex >= 0 && node.roomIndex < pool.Count) ? pool[node.roomIndex] : null;
                        if (selectedRoom == null)
                        {
                            UnityEngine.Debug.LogWarning("BattleController: 선택한 방에 맞는 방이 없어 클리어 처리합니다.");
                            maze.CompleteCurrent();
                        }
                        Log($"[Maze] enter {node.type} room={(selectedRoom != null ? selectedRoom.name : "none")}");
                        continue;
                    }

                    if (!node.cleared && node.type == MazeRoomType.Boss)
                    {
                        selectedRoom = ActiveBossRoom;
                        if (selectedRoom == null)
                        {
                            selectedRoom = ActiveEliteRooms.Count > 0 ? ActiveEliteRooms[0] : null;
                            if (selectedRoom == null) selectedRoom = ActiveCombatRooms.Count > 0 ? ActiveCombatRooms[0] : null;
                            UnityEngine.Debug.LogWarning("BattleController: bossRoom 이 비어 있어 대신 다른 방을 씁니다.");
                        }
                        if (selectedRoom == null)
                        {
                            UnityEngine.Debug.LogWarning("BattleController: 보스 방에 맞는 방이 없어 클리어 처리합니다.");
                            maze.CompleteCurrent();
                        }
                        Log($"[Maze] enter Boss room={(selectedRoom != null ? selectedRoom.name : "none")}");
                        continue;
                    }

                    Vector2Int dir = Vector2Int.zero;
                    if (autoPilot != null && autoPilot.IsActive())
                    {
                        dir = autoPilot.ChooseMazeDirection(maze);
                    }
                    else if (mapHud != null)
                    {
                        Vector2Int chosen = Vector2Int.zero;
                        bool picked = false;
                        void OnDir(Vector2Int d) { chosen = d; picked = true; }
                        mapHud.DirectionSelected += OnDir;
                        mapHud.Show(maze);
                        yield return new WaitUntil(() => picked);
                        mapHud.DirectionSelected -= OnDir;
                        mapHud.Hide();
                        dir = chosen;
                    }
                    if (dir == Vector2Int.zero || !maze.TryMove(dir))
                    {
                        UnityEngine.Debug.LogError("BattleController: 미로 이동 실패");
                        yield break;
                    }
                    Log($"[Maze] move {dir} -> {maze.CurrentPos} {maze.Current.type}");
                    SaveRun();
                }
                room = selectedRoom;
            }

            var squad = RunState.Current.CreateSquad();

            room_progress = room != null ? new RoomProgress(room, squad, null, ActiveHpMultiplier, ActiveAttackMultiplier) : null;
            if (room_progress != null)
            {
                State = room_progress.StartNextWave();
                if (State == null)
                {
                    UnityEngine.Debug.LogWarning("BattleController: room 에 웨이브가 없어 기존 wave 로 진행합니다.");
                    room_progress = null;
                }
            }
            if (room_progress == null)
            {
                State = new BattleState(squad, BuildWave(), null, ActiveHpMultiplier, ActiveAttackMultiplier);
            }
            SetupWave(State);
            SetPhaseHud(false);
            UpdateBossHpDisplay();

            if (room_progress != null)
            {
                bool isBossWave = room_progress.WaveIndex == room_progress.WaveCount - 1;
                if (isTutorial && isBossWave && tutorial != null) yield return tutorial.PlayOnce(tutorial.bossIntro, "boss");
                // T13: 튜토리얼이 아닌 1스테이지 보스 웨이브는 같은 대사를 key 'stage1_boss' 로 1회 재생한다(튜토리얼의 'boss' 키와 겹치지 않음).
                if (!isTutorial && State.BossIndex >= 0 && tutorial != null) yield return tutorial.PlayOnce(tutorial.bossIntro, "stage1_boss");
                if (!isTutorial && State.BossIndex >= 0) Log("[Stage] boss dialogue");
                yield return TitleCut(WaveTitleCutText(isBossWave));
                Log($"[Room] wave {room_progress.WaveIndex + 1}/{room_progress.WaveCount} start");
            }
            State.StartBattle();

            while (true)
            {
            while (!State.IsOver)
            {
                // 1) 에너지 수집
                SetWorld(true);
                RefreshAll();
                LogIntents();
                LogStatuses();
                yield return TitleCut($"턴 {State.Turn}");

                // T5: 살아 있는 멤버마다 '수집 → 커맨드 → 반복 턴' 을 돌고, 다음 멤버로 넘긴다. 끝나면 적 턴.
                do
                {
                    var member = State.ActiveMember;
                    if (member == null || member.Combatant.IsDead) continue;
                    Log($"[Squad] turn member={member.Definition?.id}");
                    string memberName = member.Definition != null
                        ? (string.IsNullOrEmpty(member.Definition.displayName) ? member.Definition.id : member.Definition.displayName)
                        : "?";
                    yield return TitleCut($"{memberName} 차례");

                    // 1) 에너지 수집
                    int gained = 0;
                    bool energyFull = State.Energy.Current >= State.Energy.Cap;
                    if (State.IsEnergySealed)
                    {
                        // T5: 에너지 봉인 — 수집 미니게임 없이 커맨드로 넘어간다(비축분 유지)
                        yield return TitleCut("에너지 봉인!");
                        Log($"에너지 봉인: 이번 턴 수집을 건너뜁니다. (비축 {State.Energy.Current})");
                    }
                    else if (energyFull)
                    {
                        Log("에너지가 가득 차 있어 수집을 건너뜁니다.");
                    }
                    else
                    {
                        if (isTutorial && tutorial != null) yield return tutorial.PlayOnce(tutorial.energyGuide, "energy");
                        yield return TitleCut("에너지 수집!");
                        float rushScale = State.EnergyCollectTimeScale;
                        UnityEngine.Debug.Log($"[Rush] energy collect timeScale={rushScale:F1}");
                        if (rushScale < 1f)
                        {
                            yield return TitleCut("재촉! 시간 단축");
                            Log("재촉: 수집 시간 ×0.7");
                        }
                        if (autoPilot != null && autoPilot.IsActive())
                        {
                            gained = autoPilot.EnergyGain(State, State.ActiveMemberIndex);
                        }
                        else if (phase != null && phase.runner != null && phase.pool != null && phase.pool.Count > 0)
                        {
                            // T5: 멤버마다 수집 — 지금 행동 중인 멤버(member) 한 명만 짧게 수집시킨다.
                            SetWorld(false);
                            SetPhaseHud(true);
                            phase.SetTimeScale(rushScale);
                            bool done = false;
                            if (squadCandidates != null && squadCandidates.Count > 0 && member.Definition != null)
                            {
                                var built = SquadPoolBuilder.BuildForMember(member.Definition, phase.pool);
                                Log($"[Squad] member={member.Definition.id} pool={built.pool.Count}");
                                phase.RunForMember(member.Definition.id, built.pool, built.rule, r => { gained = r.Energy; done = true; });
                            }
                            else
                            {
                                if (squadCandidates != null && squadCandidates.Count > 0)
                                {
                                    var squadService = new BoolPyeon.Meta.SquadService(saveService);
                                    var squadMembers = squadService.GetSquad(squadCandidates);
                                    var built = SquadPoolBuilder.Build(squadMembers, phase.pool);
                                    phase.SetSquad(built.pool, built.rule);
                                    Log($"[Squad] members=[{string.Join(",", squadMembers.ConvertAll(m => m != null ? m.id : "?"))}]");
                                }
                                phase.Run(r => { gained = r.Energy; done = true; });
                            }
                            yield return new WaitUntil(() => done);
                            SetPhaseHud(false);
                            SetWorld(true);
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning("BattleController: MiniGamePhase 구성이 비어 에너지 수집을 건너뜁니다.");
                        }
                        State.CollectEnergy(RunState.Current.ModifyEnergyGain(gained));
                        Log($"에너지 +{gained}");
                    }

                    // 2) 커맨드
                    if (isTutorial && tutorial != null) yield return tutorial.PlayOnce(tutorial.commandGuide, "command");
                    yield return RunCommandPhase();
                    if (State.IsOver) break;

                    // F-28: 반복 턴 모듈 — 커맨드를 가장 먼저 마친 멤버가 에너지 재수집 없이 커맨드 페이즈를 한 번 더 받는다.
                    if (State.ConsumeExtraCommandPhase())
                    {
                        yield return TitleCut("반복 턴!");
                        yield return RunCommandPhase();
                        if (State.IsOver) break;
                    }

                    RefreshAll();
                } while (!State.IsOver && State.AdvanceMember());
                if (State.IsOver) break;

                // 3) 적 턴
                State.EndPlayerPhase();
                if (State.ConsumePhase2Cut())
                {
                    yield return TitleCut("보스 격노!");
                    if (hud != null) hud.SetBossPhase2(true);
                    Log("[Boss] phase2");
                }
                RefreshAll();
                yield return TitleCut(State.Phase == BattlePhase.EnemySpecial ? "몬스터 특수 페이즈!" : "적 턴");
                for (int i = 0; i < State.Intents.Count && !State.IsOver; i++)
                {
                    EnemyIntent intent = State.Intents[i];
                    if (intent.Kind == EnemyIntentKind.Wait || State.Enemies[intent.MonsterIndex].IsDead) continue;
                    bool isSpecial = BattleState.IsEvadable(intent.Kind);
                    bool isMagic = intent.Kind == EnemyIntentKind.Magic;
                    bool isSupport = intent.Kind == EnemyIntentKind.Guard || intent.Kind == EnemyIntentKind.Buff || intent.Kind == EnemyIntentKind.Heal;
                    // T7: 공격 행동은 intent 의 대상 멤버에게. 대상이 죽었으면 BattleState 가 살아 있는 멤버로 재지정한다(전멸이면 -1 → 건너뜀).
                    int targetIdx = isSupport ? -1 : State.ResolveTargetMemberIndex(i);
                    if (!isSupport && targetIdx < 0) continue;
                    BattleUnitView targetView = isSupport ? null : MemberView(targetIdx);
                    string targetName = isSupport ? "" : State.Members[targetIdx].Combatant.Name;
                    if (!isSupport) UnityEngine.Debug.Log($"[Battle] enemy target monster={intent.MonsterIndex} kind={intent.Kind} planned={intent.TargetMemberIndex} target={targetIdx}({targetName})");
                    BattleUnitView view = ViewFor(intent.MonsterIndex);
                    Color flashColor = isSpecial ? Color.magenta : Color.yellow;
                    if (intent.Kind == EnemyIntentKind.Guard) flashColor = new Color(0.3f, 0.7f, 1f);
                    else if (intent.Kind == EnemyIntentKind.Buff) flashColor = new Color(1f, 0.6f, 0.1f);
                    else if (intent.Kind == EnemyIntentKind.Heal) flashColor = new Color(0.3f, 0.9f, 0.3f);
                    if (view != null)
                    {
                        view.Flash(flashColor);
                        if (!isSupport) view.PlayAttack();
                    }

                    bool defended = false;
                    MonsterDefinition monsterDef = State.MonsterDefinitions[intent.MonsterIndex];
                    List<int> hpBeforeSupport = null;
                    if (isSupport)
                    {
                        hpBeforeSupport = new List<int>();
                        foreach (var e in State.Enemies) hpBeforeSupport.Add(e.Hp);
                    }
                    if (!isSpecial)
                    {
                        // T6: 일반 공격·지원 행동은 미니게임 없이 바로 처리한다. 방어 커맨드(Guard)면 BattleState 가 50%로 줄인다.
                        UnityEngine.Debug.Log($"[Battle] normal action resolve (no minigame) monster={intent.MonsterIndex} kind={intent.Kind}");
                    }
                    else if (isSpecial)
                    {
                        MiniGameDefinition defenseGame = PickSpecialGame(monsterDef, intent);
                        if (defenseGame != null)
                        {
                            yield return RunDefense(defenseGame, ok => defended = ok, -1f, $"{targetName} 방어!");
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning("BattleController: 특수 페이즈 방어 미니게임을 찾지 못해 방어 없이 진행합니다.");
                        }
                    }

                    int dealt = State.ResolveEnemyIntent(i, defended);

                    if (isSupport)
                    {
                        int healedAmount = 0;
                        for (int e = 0; e < State.Enemies.Count; e++)
                        {
                            int healed = State.Enemies[e].Hp - hpBeforeSupport[e];
                            if (healed > 0)
                            {
                                healedAmount = healed;
                                BattleUnitView healedView = ViewFor(e);
                                if (healedView != null) healedView.ShowHeal(healed);
                            }
                        }
                        string supportName = State.Enemies[intent.MonsterIndex].Name;
                        if (intent.Kind == EnemyIntentKind.Guard) Log($"{supportName} 방어 태세");
                        else if (intent.Kind == EnemyIntentKind.Buff) Log($"{supportName} 강화");
                        else Log($"{supportName} 회복 +{healedAmount}");
                    }
                    else
                    {
                        if (dealt > 0 && targetView != null)
                        {
                            targetView.Flash(Color.red);
                            targetView.PlayHit();
                            targetView.ShowDamage(dealt, isMagic ? new Color(0.6f, 0.3f, 1f) : Color.red);
                            string hitMemberId = State.Members[targetIdx].Definition != null ? State.Members[targetIdx].Definition.id : "?";
                            UnityEngine.Debug.Log($"[Anim] member={hitMemberId} motion=hit");
                        }
                        string kind = isMagic ? "마법 공격" : isSpecial ? "특수 공격" : "공격";
                        if (defended)
                        {
                            if (targetView != null)
                            {
                                targetView.Flash(Color.cyan);
                                targetView.PlayDefend();
                            }
                            Log(isMagic ? $"{targetName} 마법 회피 성공!" : isSpecial ? $"{targetName} 특수 공격 방어 성공!" : $"{targetName}, {State.Enemies[intent.MonsterIndex].Name} 공격 회피!");
                        }
                        else
                        {
                            Log($"{State.Enemies[intent.MonsterIndex].Name} {kind} → {targetName} {dealt} 피해" + (isSpecial ? " (패널티 예약)" : ""));
                            if (State.Members[targetIdx].Combatant.IsDead) Log($"{targetName} 쓰러짐!");
                        }
                    }
                    RefreshAll();
                    yield return new WaitForSeconds(enemyActionDelay);
                }
                if (State.IsOver) break;

                State.EndTurn();
                State.BeginTurn();
            }

            if (room_progress == null) break;

            bool completedIsBossWave = room_progress.WaveIndex == room_progress.WaveCount - 1;
            room_progress.CompleteCurrentWave();
            if (room_progress.IsFailed)
            {
                Log($"[Room] wave {room_progress.WaveIndex + 1}/{room_progress.WaveCount} failed");
                break;
            }
            int waveKills = AwardWaveRewards(completedIsBossWave);
            if (room_progress.IsCleared)
            {
                Log($"[Room] wave {room_progress.WaveIndex + 1}/{room_progress.WaveCount} clear (kills +{waveKills})");
                yield return HandleRoomCleared(room_progress.Squad);
                yield break;
            }
            Log($"[Room] wave {room_progress.WaveIndex + 1}/{room_progress.WaveCount} clear (kills +{waveKills})");
            State = room_progress.StartNextWave();
            SetupWave(State);
            UpdateBossHpDisplay();
            bool nextIsBossWave = room_progress.WaveIndex == room_progress.WaveCount - 1;
            yield return TitleCut(WaveTitleCutText(nextIsBossWave));
            Log($"[Room] wave {room_progress.WaveIndex + 1}/{room_progress.WaveCount} start");
            State.StartBattle();
            }

            if (room_progress != null)
                Log(room_progress.IsCleared ? "[Room] cleared" : "[Room] failed");

            if (hud != null) hud.ShowBossHp(false, null);
            SetWorld(true);
            RefreshAll();
            bool victory = State.Phase == BattlePhase.Victory;
            yield return TitleCut(victory ? "승리!" : "패배...");
            int finalKills = RunState.Current != null ? RunState.Current.Kills : 0;
            int finalCrystals = RunState.Current != null ? RunState.Current.CrystalsEarned : 0;
            int endedStageNumber = RunState.Current != null ? RunState.Current.StageNumber : 1;
            Log($"[Run] end kills={finalKills} crystals={finalCrystals}");
            if (autoPilot != null && autoPilot.IsActive())
            {
                string hpSummary = string.Join(",", System.Linq.Enumerable.Select(State.Members,
                    m => $"{m.Definition?.id}:{m.Combatant.Hp}"));
                UnityEngine.Debug.Log($"[AutoPilot] END victory={victory} turns={State.Turn} hp={hpSummary}");
            }
            if (!victory)
            {
                RunState.Current = null;
                BoolPyeon.Save.SaveService.Default.ClearRun();
            }
            if (hud != null)
            {
                if (isTutorial)
                {
                    hud.ShowResult(victory, finalKills, finalCrystals);
                }
                else
                {
                    int nextStage = RunState.ComputeStartStage(BoolPyeon.Save.SaveService.Default.Data.highestClearedStage, MaxStageNumber());
                    // T3: '(마지막 스테이지)'는 클리어로 다음 스테이지가 상한에 막혔을 때만 붙인다.
                    bool lastStage = victory && nextStage <= endedStageNumber;
                    Log($"[Stage] result cleared={endedStageNumber} next={nextStage}");
                    hud.ShowResult(victory, finalKills, finalCrystals, endedStageNumber, nextStage, lastStage);
                }
            }
        }

        // F-21: 클리어한 웨이브의 처치 수만큼 정화 결정을 즉시 적립한다(보스 웨이브 +5).
        private int AwardWaveRewards(bool isBossWave)
        {
            if (RunState.Current == null || State == null) return 0;
            int count = 0;
            foreach (var e in State.Enemies)
                if (e != null && e.IsDead) count++;
            int crystals = count + (isBossWave ? 5 : 0);
            RunState.Current.AddKills(count);
            RunState.Current.AddCrystalsEarned(crystals);
            BoolPyeon.Save.SaveService.Default.AddCrystals(crystals);
            return count;
        }

        // F-19: 방 클리어 → 강화 3택 → 다음 방(씬 재로드). 이 코루틴이 끝나면 BattleLoop 은 종료한다.
        private IEnumerator HandleRoomCleared(Squad squad)
        {
            RunState.Current.CaptureHp(squad);

            if (isTutorial)
            {
                yield return TitleCut("튜토리얼 클리어!");
                if (tutorial != null) yield return tutorial.PlayOnce(tutorial.outro, "outro");
                BoolPyeon.Save.SaveService.Default.CompleteTutorial();
                GoToHQ();
                yield break;
            }

            yield return TitleCut("방 클리어!");

            var choices = RunState.Current.RollChoices(upgradePool, 3, null);
            if (choices.Count > 0 && autoPilot != null && autoPilot.IsActive())
            {
                int idx = autoPilot.ChooseUpgrade();
                if (idx >= 0 && idx < choices.Count)
                {
                    UpgradeDefinition autoPicked = choices[idx];
                    RunState.Current.Apply(autoPicked);
                    Log($"[Upgrade] {autoPicked.displayName}");
                }
            }
            else if (choices.Count > 0 && upgradeHud != null)
            {
                int chosenIndex = -1;
                void OnChosen(int idx) => chosenIndex = idx;
                upgradeHud.Chosen += OnChosen;
                upgradeHud.Show(choices);
                yield return new WaitUntil(() => chosenIndex >= 0);
                upgradeHud.Chosen -= OnChosen;
                upgradeHud.Hide();

                UpgradeDefinition picked = choices[chosenIndex];
                RunState.Current.Apply(picked);
                Log($"[Upgrade] {picked.displayName}");
            }
            else
            {
                Log("[Upgrade] 적용 가능한 강화 없음");
            }

            if (RunState.Current.Map != null)
            {
                RunState.Current.Map.CompleteCurrent();
                if (RunState.Current.Map.IsFinished)
                {
                    yield return StageClearSequence();
                    yield break;
                }
            }

            SaveRun();
            yield return TitleCut("다음 방");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // T13: 보스 처치로 미로가 끝났을 때의 스테이지 클리어 흐름.
        // 튜토리얼 런은 HandleRoomCleared 앞부분에서 이미 GoToHQ 로 빠지므로 여기 도달하지 않지만,
        // 안전하게 첫 줄에 가드를 둔다.
        private IEnumerator StageClearSequence()
        {
            if (isTutorial) yield break;

            int stageNumber = RunState.Current != null ? RunState.Current.StageNumber : 1;
            Log($"[Stage] clear stage={stageNumber}");

            SetWorld(true);
            if (hud != null) hud.ShowBossHp(false, null);

            yield return TitleCut($"스테이지 {stageNumber} 클리어!");

            RunState.Current.AddCrystalsEarned(StageClearBonus);
            BoolPyeon.Save.SaveService.Default.AddCrystals(StageClearBonus);
            Log($"[Stage] reward bonus={StageClearBonus}");

            // T2: 승리로만 진행을 갱신한다(사망 경로는 이 코루틴에 도달하지 않는다).
            BoolPyeon.Save.SaveService.Default.MarkStageCleared(stageNumber);

            int kills = RunState.Current.Kills;
            int crystals = RunState.Current.CrystalsEarned;
            Log($"[Run] end kills={kills} crystals={crystals}");

            if (autoPilot != null && autoPilot.IsActive())
                UnityEngine.Debug.Log($"[AutoPilot] END victory=True stageClear={stageNumber}");

            int nextStage = RunState.ComputeStartStage(BoolPyeon.Save.SaveService.Default.Data.highestClearedStage, MaxStageNumber());
            bool lastStage = nextStage <= stageNumber;
            Log($"[Stage] result cleared={stageNumber} next={nextStage}");
            if (hud != null) hud.ShowResult(true, kills, crystals, stageNumber, nextStage, lastStage);
            RunState.Current = null;
            BoolPyeon.Save.SaveService.Default.ClearRun();

            Log($"[Stage] auto return to HQ in {stageClearReturnDelay}s");
            if (stageClearReturnDelay > 0f) yield return new WaitForSecondsRealtime(stageClearReturnDelay);
            GoToHQ();
        }

        // F-20: 방 사이 시점(맵 생성·휴식·강화 선택 뒤)에 진행 중 런을 저장한다.
        private void SaveRun()
        {
            if (RunState.Current == null) return;
            BoolPyeon.Save.SaveService.Default.SaveRun(RunState.Current, ActiveCombatRooms.Count, ActiveEliteRooms.Count);
            Log($"[Save] run saved pos={(RunState.Current.Map != null ? RunState.Current.Map.CurrentPos.ToString() : "-")}");
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause && RunState.Current != null) SaveRun();
        }

        private void OnApplicationQuit()
        {
            if (RunState.Current != null) SaveRun();
        }

        private void OnDestroy()
        {
            DetachModules();
        }

        // T11: 웨이브에 isBoss 몬스터가 있으면 "보스!", 마지막 웨이브(레거시 중간보스)면 "중간보스!", 그 외 웨이브 번호
        private string WaveTitleCutText(bool isBossWave)
        {
            if (State != null && State.BossIndex >= 0) return "보스!";
            return isBossWave ? "중간보스!" : $"웨이브 {room_progress.WaveIndex + 1}/{room_progress.WaveCount}";
        }

        // T11: 웨이브가 바뀔 때마다 보스 HP 바 표시를 갱신한다(SetupWave 직후 호출)
        private void UpdateBossHpDisplay()
        {
            if (hud == null || State == null) return;
            int bossIdx = State.BossIndex;
            if (bossIdx >= 0)
            {
                hud.SetBossPhase2(false);
                hud.ShowBossHp(true, State.MonsterDefinitions[bossIdx].displayName);
                hud.SetBossHp(State.Enemies[bossIdx].Hp, State.Enemies[bossIdx].MaxHp);
                if (!_bossHpShownLogged)
                {
                    _bossHpShownLogged = true;
                    Log($"[Boss] hp bar shown name={State.MonsterDefinitions[bossIdx].displayName}");
                }
            }
            else
            {
                hud.ShowBossHp(false, null);
            }
        }

        // T11.1: UpdateBossHpDisplay 첫 표시 로그(캡처 실패 대비 확인용)
        private bool _bossHpShownLogged;

        private IEnumerator TitleCut(string text)
        {
            if (hud != null) hud.ShowTitle(text);
            float wait = titleCutTime;
            if (autoPilot != null && autoPilot.IsActive()) wait *= autoPilot.titleCutScale;
            yield return new WaitForSeconds(wait);
            if (hud != null) hud.ShowTitle(null);
        }

        private void Update()
        {
            if (State == null || !IsCommandLikePhase(State.Phase) || !PointerInput.Down)
            {
                return;
            }
            RaycastHit2D hit = PointerInput.RaycastWorld2D();
            if (hit.collider == null) return;
            BattleUnitView view = hit.collider.GetComponentInParent<BattleUnitView>();
            if (view == null || IsMemberView(view) || view.Unit == null || view.Unit.IsDead) return;
            target = view.Unit;
            RefreshAll();
        }

        private void EnsureTarget()
        {
            if (target != null && !target.IsDead) return;
            target = null;
            foreach (var e in State.Enemies)
                if (!e.IsDead) { target = e; break; }
        }

        // 에너지가 되는 한 여러 번 행동한다. 더 쓸 수 있는 커맨드가 없으면(또는 승리) 페이즈를 끝낸다
        private void UseCommand(SkillDefinition skill)
        {
            if (State == null || skill == null) return;
            EnsureTarget();
            var hpBefore = new List<int>();
            foreach (var e in State.Enemies) hpBefore.Add(e.Hp);
            if (!State.TryCommand(skill, target))
            {
                Log($"{skill.displayName} 사용 불가");
                return;
            }
            string line = skill.displayName;
            BattleUnitView actingView = MemberView(State.ActiveMemberIndex);
            // T6: 커맨드 종류에 따라 방어/기본공격/스킬 모션을 나눈다. 기본공격(attackSkill)이 아니고 방어(guardSkill)도 아니면 스킬 커맨드다.
            string actingMemberId = State.ActiveMember != null && State.ActiveMember.Definition != null
                ? State.ActiveMember.Definition.id : "?";
            if (actingView != null)
            {
                if (ActingCharacter != null && skill == ActingCharacter.guardSkill)
                {
                    actingView.PlayDefend();
                }
                else if (ActingCharacter != null && skill == ActingCharacter.attackSkill)
                {
                    actingView.PlayAttack();
                    UnityEngine.Debug.Log($"[Anim] member={actingMemberId} motion=attack");
                }
                else
                {
                    actingView.PlaySkill();
                    UnityEngine.Debug.Log($"[Anim] member={actingMemberId} motion=skill");
                }
            }
            for (int i = 0; i < State.Enemies.Count; i++)
            {
                int d = hpBefore[i] - State.Enemies[i].Hp;
                if (d > 0)
                {
                    bool guardedTarget = State.Enemies[i].HasStatus(StatusEffectType.Guarded);
                    line += $" → {State.Enemies[i].Name} {d} 피해" + (guardedTarget ? " (방어막)" : "");
                    BattleUnitView v = ViewFor(i);
                    if (v != null)
                    {
                        v.Flash(Color.red);
                        v.PlayHit();
                        v.ShowDamage(d, guardedTarget ? new Color(0.3f, 0.7f, 1f) : Color.white);
                    }
                }
            }
            Log(line);
            EnsureTarget();
            RefreshAll();
            if (State.IsOver)
            {
                commandDone = true;
            }
            else if (!State.CanAnyCommand(AllCommands()))
            {
                Log("더 쓸 수 있는 행동이 없어 턴 종료");
                commandDone = true;
            }
        }

        private IEnumerable<SkillDefinition> AllCommands()
        {
            if (ActingCharacter == null) yield break;
            yield return ActingCharacter.attackSkill;
            yield return ActingCharacter.guardSkill;
            var skills = ActingSkills();
            if (skills == null) yield break;
            foreach (var s in skills) yield return s;
        }

        private static bool IsCommandLikePhase(BattlePhase phase)
        {
            return phase == BattlePhase.Command;
        }

        private void OnAttack() => UseCommand(ActingCharacter != null ? ActingCharacter.attackSkill : null);
        private void OnGuard() => UseCommand(ActingCharacter != null ? ActingCharacter.guardSkill : null);

        private void OnSkill(int index)
        {
            var skills = ActingSkills();
            if (ActingCharacter == null || skills == null || index < 0 || index >= skills.Count) return;
            UseCommand(skills[index]);
        }

        private void OnEndTurn()
        {
            if (State != null && IsCommandLikePhase(State.Phase)) commandDone = true;
        }

        private void OnRetry()
        {
            if (isTutorial)
            {
                // 튜토리얼 패배: 플래그를 세우지 않고 씬을 다시 불러 재도전한다.
                RunState.Current = null;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
            GoToHQ();
        }

        // F-21: 결과 패널 '본부로' → HQ 씬으로 간다. 런이 아직 안 지워졌으면 여기서 마저 지운다.
        private void GoToHQ()
        {
            if (returningToHQ) return;
            returningToHQ = true;
            RunState.Current = null;
            BoolPyeon.Save.SaveService.Default.ClearRun();
            if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.HQ);
            if (SceneLoader.Instance != null) SceneLoader.Instance.LoadSingle(hqSceneName);
            else SceneManager.LoadScene(hqSceneName);
        }

        private void SetSkills()
        {
            if (hud == null || ActingCharacter == null) return;
            var skills = ActingSkills();
            var labels = new List<string>();
            var usable = new List<bool>();
            var icons = new List<Sprite>();
            foreach (var s in skills)
            {
                if (s == null) continue;
                int cost = State != null ? SkillResolver.EffectiveCost(s, State.Player) : s.energyCost;
                labels.Add($"{s.displayName} ({cost})");
                usable.Add(State != null && State.CanCommand(s));
                icons.Add(s.icon);
            }
            hud.SetSkills(labels, usable, icons);
        }

        public void RefreshAll()
        {
            if (State == null) return;
            EnsureTarget();
            for (int i = 0; i < State.Members.Count; i++)
            {
                BattleUnitView mv = MemberView(i);
                if (mv == null) continue;
                mv.Refresh(null);
                bool dead = State.Members[i] != null && State.Members[i].Combatant != null && State.Members[i].Combatant.IsDead;
                mv.SetSelected(i == State.ActiveMemberIndex && !dead);
            }
            for (int i = 0; i < State.Enemies.Count; i++)
            {
                BattleUnitView v = ViewFor(i);
                if (v == null) continue;
                v.Refresh(i < State.Intents.Count ? State.Intents[i] : null, IntentTargetName(i));
                v.SetSelected(IsCommandLikePhase(State.Phase) && State.Enemies[i] == target);
            }
            if (hud == null) return;
            hud.SetHp(State.Player.Hp, State.Player.MaxHp);
            int bossIdx = State.BossIndex;
            if (bossIdx >= 0 && bossIdx < State.Enemies.Count)
                hud.SetBossHp(State.Enemies[bossIdx].Hp, State.Enemies[bossIdx].MaxHp);
            if (State.Squad.Members.Count <= 1)
            {
                hud.SetEnergy(State.Energy.Current, State.Energy.Cap);
            }
            else
            {
                // T6: 행동 중인 멤버 기준으로만 '이름 에너지 cur/cap' 표시(전원 나열은 지운다)
                string activeName = ActingCharacter != null ? ActingCharacter.displayName : "?";
                hud.SetEnergyText($"{activeName} 에너지 {State.Energy.Current}/{State.Energy.Cap}");
            }
            hud.SetPlayerStatuses(BattleUnitView.StatusesText(State.Player));
            bool anySkill = false;
            var skills = ActingSkills();
            if (skills != null)
                foreach (var s in skills)
                    if (State.CanCommand(s)) { anySkill = true; break; }
            hud.SetCommandInteractable(ActingCharacter != null && State.CanCommand(ActingCharacter.attackSkill),
                ActingCharacter != null && State.CanCommand(ActingCharacter.guardSkill), anySkill);
            SetSkills();
        }

        private BattleUnitView ViewFor(int index)
        {
            return enemyViews != null && index >= 0 && index < enemyViews.Length ? enemyViews[index] : null;
        }

        private void SetWorld(bool active)
        {
            if (battleWorld != null) battleWorld.SetActive(active);
            if (hud != null) hud.ShowTopBar(active);
        }

        private void SetPhaseHud(bool active)
        {
            if (phaseHudRoot != null) phaseHudRoot.SetActive(active);
        }

        // 행동의 miniGameIds → 몬스터 정의의 specialMiniGameIds → specialPool → 에너지 수집용 phase.pool 순서로 고른다
        private MiniGameDefinition PickSpecialGame(MonsterDefinition def, EnemyIntent intent)
        {
            MiniGameDefinition byAction = FindFirstById(intent != null && intent.Action != null ? intent.Action.miniGameIds : null);
            if (byAction != null) return byAction;
            MiniGameDefinition byMonster = FindFirstById(def != null ? def.specialMiniGameIds : null);
            if (byMonster != null) return byMonster;
            if (specialPool != null && specialPool.Count > 0)
                return specialPool[UnityEngine.Random.Range(0, specialPool.Count)];
            if (phase != null && phase.pool != null && phase.pool.Count > 0)
                return phase.pool[UnityEngine.Random.Range(0, phase.pool.Count)];
            return null;
        }

        private MiniGameDefinition FindFirstById(List<string> ids)
        {
            if (ids == null) return null;
            foreach (string id in ids)
            {
                MiniGameDefinition found = FindById(specialPool, id) ?? (phase != null ? FindById(phase.pool, id) : null);
                if (found != null) return found;
            }
            return null;
        }

        private static MiniGameDefinition FindById(List<MiniGameDefinition> list, string id)
        {
            if (list == null || string.IsNullOrEmpty(id)) return null;
            foreach (MiniGameDefinition d in list)
                if (d != null && d.id == id) return d;
            return null;
        }

        // 특수 공격 방어·일반 공격 회피용 미니게임 한 판. 성공하면 onDone(true).
        // timeLimit<=0 이면 def.baseTimeLimit, title 이 비면 타이틀 컷 생략
        private IEnumerator RunDefense(MiniGameDefinition def, System.Action<bool> onDone, float timeLimit = -1f, string title = "방어!")
        {
            if (def != null && def.Has(MiniGameUsage.Special))
            {
                Log($"special defense minigame id={def.id} scene={def.sceneName}");
            }
            if (autoPilot != null && autoPilot.IsActive())
            {
                onDone?.Invoke(autoPilot.ResolveDefense());
                yield break;
            }
            if (!string.IsNullOrEmpty(title)) yield return TitleCut(title);
            SetWorld(false);
            SetPhaseHud(false);
            if (phase == null || phase.runner == null)
            {
                UnityEngine.Debug.LogWarning("BattleController: MiniGameRunner 가 없어 방어를 건너뜁니다.");
                onDone?.Invoke(false);
                yield break;
            }
            MiniGameContext ctx = MiniGameContext.Default;
            ctx.TimeLimit = timeLimit > 0f ? timeLimit : def.baseTimeLimit;
            ctx.Difficulty = Mathf.Clamp(State.Turn, def.minDifficulty, def.maxDifficulty);
            ctx.Speed = 1f;

            bool done = false;
            MiniGameResult result = MiniGameResult.Failed;
            phase.runner.Run(def, ctx, r => { result = r; done = true; });
            yield return new WaitUntil(() => done);

            SetWorld(true);
            onDone?.Invoke(result == MiniGameResult.Success);
        }

        private void Log(string line)
        {
            UnityEngine.Debug.Log("[Battle] " + line);
            if (hud != null) hud.Log(line);
        }

        // 턴마다 적이 고른 행동을 콘솔에 남긴다 (AI 확률 확인용)
        private void LogIntents()
        {
            if (State == null) return;
            for (int i = 0; i < State.Intents.Count; i++)
            {
                EnemyIntent intent = State.Intents[i];
                if (intent.Kind == EnemyIntentKind.Wait) continue;
                string id = intent.Action != null ? intent.Action.id : "legacy";
                string tn = IntentTargetName(i);
                UnityEngine.Debug.Log($"[AI] {State.Enemies[intent.MonsterIndex].Name} → {id} ({intent.Kind}, {intent.Damage})" + (string.IsNullOrEmpty(tn) ? "" : $" target={intent.TargetMemberIndex}({tn})"));
            }
        }

        // T7: 적 행동 i 의 대상 멤버 이름(예고 표시용, rng 미사용). 지원 행동·대상 없음이면 null.
        private string IntentTargetName(int i)
        {
            if (State == null || i < 0 || i >= State.Intents.Count) return null;
            EnemyIntentKind k = State.Intents[i].Kind;
            if (k != EnemyIntentKind.Attack && k != EnemyIntentKind.Special && k != EnemyIntentKind.Magic) return null;
            int t = State.PeekTargetMemberIndex(i);
            return t >= 0 && t < State.Members.Count ? State.Members[t].Combatant.Name : null;
        }

        // T8: 턴 시작 시 주인공·몬스터 상태이상을 한 줄 로그로 남긴다.
        private void LogStatuses()
        {
            if (State == null) return;
            string player = BattleUnitView.StatusesText(State.Player);
            if (string.IsNullOrEmpty(player) && State.Enemies.Count == 0) return;
            var parts = new System.Collections.Generic.List<string> { $"player={(string.IsNullOrEmpty(player) ? "-" : player)}" };
            for (int i = 0; i < State.Enemies.Count; i++)
            {
                string m = BattleUnitView.StatusesText(State.Enemies[i]);
                if (!string.IsNullOrEmpty(m)) parts.Add($"monster{i}={m}");
            }
            UnityEngine.Debug.Log($"[Battle] status {string.Join(" | ", parts)}");
        }
    }
}
