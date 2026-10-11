# 테스트 시나리오 (집에서 혼자 확인)

작성: build 단계, MCP 로 SampleScene 실측 후 작성. Unity 에디터가 켜져 있어야 MCP 확인이 가능했고, 이 문서의 표는 그 실측값이다.

## §0 사전 조건 (실측)

| 항목 | 값 |
|---|---|
| Unity 버전 | 6000.3.24f1 |
| Build Settings 씬(순서) | SampleScene, EggMiniGame, PizzaMiniGame, USBMiniGame, TapTargetMiniGame, TemplateMiniGame — 전부 enabled=True |
| SampleScene 루트 | Main Camera, Canvas, EventSystem, NoteBook, usbHand, MiniGameRunner, MiniGamePhase, MiniGamePhaseDebug |
| MiniGameRunner.instructionText / timeBarImage | InstructionText / MiniGameTimeBar — 둘 다 연결됨(None 아님) |
| MiniGameRunnerDebug.runner / definition / runKey | MiniGameRunner / **TapTarget** / T |
| MiniGamePhase.runner | MiniGameRunner (연결됨) |
| MiniGamePhase.pool (3개) | `dummy_tap`→씬 USBMiniGame, `egg_break`→씬 EggMiniGame, `pizza_serve`→씬 PizzaMiniGame |
| MiniGamePhase.config | phaseDuration=20, speedPerSuccess=1.1, energyPerSuccess=1, comboBonusEvery=3, energyCap=10, difficultyStepSuccesses=3 |
| MiniGamePhaseDebug.phase / hud / autoRunOnStart / startKey | MiniGamePhase / PhaseHudRoot / **False** / Space |
| MiniGamePhaseHud 필드(phaseTimeBar/energyGauge/energyText/comboText/feedbackText/resultPanel/resultText/startHintText) | 전부 연결됨(None 아님) |

**주의**: `autoRunOnStart = False` 다 — 인계서 초안은 자동 시작을 가정했지만 실측은 수동. 시나리오 A 는 **Space 또는 좌클릭으로 직접 시작**해야 한다.

**참고**: pool 의 `dummy_tap` 이 `USBMiniGame.unity` 씬을 쓴다. 즉 페이즈 루프로 USB(더미탭) 를 이미 확인할 수 있다 — §2 인계서의 "USB 정의 에셋 없음" 제한은 **TapTarget 계열 USB 가 아니라 별도 USB 정의 에셋**을 말하며, 씬 자체는 페이즈 풀에 이미 포함돼 있다. 시나리오 C 에서 이 점을 다시 확인한다.

## §1 자동 확인 (MCP, 완료)

- [x] 콘솔 Error: 0건(2026-09-29 T10, Unity_GetConsoleLogs(Error) 재확인).
- [x] Test Runner ▸ EditMode 전체 151/151 PASS, FailCount=0(2026-09-29 T10). **정정**: RunCommand 안에서 `TestRunnerApi`(`ICallbacks`+`SessionState` 로 비동기 결과를 전달)를 직접 써서 필터 없이 전체 EditMode 스위트를 실행할 수 있다. 과거 "MCP 로 Test Runner 실행 API 를 못 찾음" 기술은 낡은 내용이었다.

## §1-A 표준 자동 검증 절차 (재사용 프로세스)

T8~T10.2 에서 반복한 자동 검증을 매번 새로 찾지 않도록 순서를 고정한다. Unity 에디터가 켜져 있고 unity-mcp 가 연결돼 있을 때만 수행 가능(꺼져 있으면 이 절차를 건너뛰고 스크립트까지만 하고 남은 일을 목록으로 남긴다).

1. **컴파일 에러 확인**: `Unity_GetConsoleLogs(logTypes="Error")` → `errorCount == 0` 이어야 통과. (`warningCount` 는 보고만, 실패 기준 아님)
2. **EditMode 전수 테스트**: `Unity_RunCommand` 로 `TestRunnerApi`(`ICallbacks` + `SessionState`) 를 직접 실행해 필터 없이 전체 EditMode 스위트를 돌린다. `FailCount == 0` 이어야 통과. (참고: §1 정정 항목 — Test Runner 실행 API 는 MCP 로 접근 가능하다)
3. **전투 밸런스(있는 경우)**: `Unity_RunCommand` 로 `BattleScenarioSimulator.RunAll()` 및 스테이지별 `RunWaveBalance` 를 실행, 전량 PASS 확인.
4. **씬 참조 점검**: 대상 씬을 열어 매니저 오브젝트의 public 필드가 None 이 아닌지, 콜라이더·태그·Animator 파라미터가 맞는지 확인(§0 사전 조건 표처럼 기록).
5. **로그 기반 시나리오(제약)**: `[Stage] result`, `[Flow] DONE` 같은 커스텀 Info 로그는 `Unity_GetConsoleLogs` 가 Info 레벨을 지원하지 않아 자동 검증 불가 — 이 부분은 §5 처럼 사람이 에디터 콘솔에서 직접 읽어 확인한다. 코드로 로그 레벨을 Error/Warning 으로 올려 우회하지 않는다(로그 의미가 바뀌면 다른 확인에 영향).
6. 완료 후 Docs/ROADMAP.md 와 이 문서 §3 결과표에 실행 시각·결과를 기록한다.

## §2 사람이 확인할 시나리오

### A. 페이즈 루프 (SampleScene)
1. 진입: `Assets/Scenes/SampleScene.unity` 열고 Play.
2. 조작: Space 또는 좌클릭으로 페이즈 시작. 이후 각 미니게임을 순서대로 클리어/실패 시도.
3. 기대: 콘솔에 매 판마다 `MiniGamePhase: {id} -> {result} | Combo=.. Energy=.. Speed=..`. 직전 판과 같은 id 연속 없음. 성공마다 Energy +1(3연속 성공 시 +1 추가), Speed 1.10→1.21→1.33 배로 증가, Energy 상한 10. 20초 경과 시 `Phase result: S=.. F=.. E=.. MaxCombo=..` 로그 + 화면에 ResultPanel(성공/실패/에너지/최대콤보) 표시.
4. 합격: 위 로그·수치가 모두 관찰됨. 실패: 로그 미출력, 같은 게임 연속 출제, 20초 넘게 지속.
5. 메모: ______

### B. 단일 미니게임 (MiniGameRunnerDebug)
1. 진입: SampleScene Play. 인스펙터에서 `MiniGameRunner` 오브젝트의 `MiniGameRunnerDebug.definition` 을 `Egg`/`Pizza`/`TapTarget`/`DummyTap` 순서로 교체(Play 전 또는 Play 중 인스펙터에서 드래그).
2. 조작: T 키 또는 컴포넌트 우클릭 ▸ Run Debug.
3. 기대: 성공 조작 → `MiniGame result: Success` + Success 애니메이션. 실패 조작 → Failed. 아무 것도 안 하고 시간(기본 baseTimeLimit, definition별 설정값) 경과 → definition 의 timeoutResult(대개 Failed).
4. 합격: 4종 모두 성공/실패/시간초과 로그가 한 번씩 확인됨.
5. 메모: ______

### C. 미니게임 씬 단독 실행
1. 진입: `EggMiniGame.unity` / `PizzaMiniGame.unity` / `USBMiniGame.unity` / `TapTargetMiniGame.unity` 를 각각 열고 Play.
2. 조작: 게임별 정상 조작(클릭 대상, 드래그 등).
3. 기대: 컴파일 에러 0, 클릭이 인식됨, 결과에 따라 Animator `isSuccess`/`isFailed` 전환.
4. 합격: 4개 씬 모두 클릭 인식 + 결과 애니메이션 전환.
5. 메모: ______

### D. 입력 회귀 (2026-09-25 12:23 버그 재발 확인)
1. 진입: SampleScene Play, 페이즈 진행 중 Egg 게임이 뜰 때.
2. 조작: 노른자 위 껍질을 클릭.
3. 기대: 클릭이 무시되지 않고 껍질이 반응(제거/이동 등 EggGameManager 로직대로).
4. 합격: 클릭이 씹히지 않음. 실패: 클릭해도 아무 반응 없음(EventSystem.IsPointerOverGameObject 가 배경 Canvas 때문에 항상 true 인 회귀).
5. 메모: ______

### E. 전투 데이터 (EditMode)
1. 진입: Unity 메뉴 Window ▸ General ▸ Test Runner ▸ EditMode 탭.
2. 조작: `BoolPyeon.Battle.Tests` 전체 Run.
3. 기대: 22/22 초록. 그리고 Project 창에서 Assets 우클릭 ▸ Create ▸ BoolPyeon ▸ Battle 아래 메뉴 3종이 보임.
4. 합격: 22/22 통과 + 메뉴 3종 노출.
5. 메모: ______

### F. 전투 자동 시나리오 (BattleScenarioSimulator / BattleAutoPilot)
1. 로직 시나리오(사람 입력 없음, RunCommand 로 확인 완료): `BoolPyeon.Battle.BattleScenarioSimulator.RunAll()` 을 부르면 S1~S6 이 전부 PASS 한다(2026-09-28 확인, 6/6 PASS). **주의(T14, D-14 반영)**: S1~S3 의 "회피" 문구는 시뮬레이터 픽스처 이름이며, 실제 게임 규칙은 D-14 로 바뀌었다 — 일반 공격은 회피 미니게임이 없고 피격이 확정, `Guard` 커맨드를 썼으면 피해 50% 감소(안 썼으면 전체 피해), `Special`(특수 공격) 만 방어 미니게임이 있다. 시뮬레이터 코드(`BattleScenarioSimulator.cs` 330행)의 `dummy_tap` 픽스처는 테스트용으로 그대로 두고 있다(§4).
   - S1 1마리·Special 방어 항상 성공 → Victory, 받은 피해 0
   - S2 3마리·Special 방어 항상 실패 → 실행 적 행동 ≤2, 방어 미니게임 ≤2, 예고 피해 ≤ 최대HP 25%
   - S3 Guard 커맨드 후 Special 방어 실패 → 피해가 Guard 없을 때의 50%
   - S4 에너지 코스트·상한10·턴 종료 비축
   - S5 특수 공격 주기 예고·방어 실패 시 패널티 다음 턴 적용
   - S6 플레이어 사망 → Defeat
2. 진입: `Assets/Scenes/Battle.unity` 계층에 `BattleAutoPilot` 오브젝트가 있다(기본 enabled=false). `BattleController.autoPilot` 에 연결돼 있다. **T14**: `BattleAutoPilot.ChooseMazeDirection` 이 BFS 로 미로(StageMaze) 안에서 보스 방까지 경로를 골라 자동 이동한다(58-103행).
3. 조작(사람 확인): 인스펙터에서 `BattleAutoPilot.enabled` 를 켜거나, PlayerPrefs `BattleAutoPilot`=1 로 설정한 뒤 Play.
4. 기대: 콘솔에 `[AutoPilot] energy +.. / command=attack / defense=.. / map select .. / upgrade=0` 로그가 이어지고, 방 클리어·웨이브 전환·미로 이동(BFS)·보스 방 진입까지 사람 손 없이 진행된다. 마지막에 `[AutoPilot] END victory=.. turns=.. hp=..` 로그가 남는다(승리로 방을 다 돌면 HandleRoomCleared 쪽 별도 결과 로그로 끝날 수 있다).
5. 2026-09-28 RunCommand 로 1개 층(Room)을 오토파일럿으로 완주 확인: `[AutoPilot] energy/command/defense/map select/upgrade` 로그 전부 관찰, 콘솔 Error/Exception 0건. 장시간 실행(여러 웨이브·런 전체)과 END 로그, Defeat 경로는 사람이 마저 확인해야 한다(§4).
6. 주의: 오토파일럿 실행은 실제 세이브(정화 결정·튜토리얼 완료·run)를 바꾼다. 실행 전 세이브 백업/초기화 여부를 먼저 정한다.
7. 메모: ______

### H. 미로(StageMaze) — T2/T4
1. 진입: `Battle.unity` Play, HQ 출격 후 미로 HUD 확인.
2. 조작: 방마다 상/하/좌/우 중 갈 수 있는 방향 버튼(`GetMovableDirections`)으로 이동, 이미 클리어한 방으로 되돌아가기 가능(재입장 보상 없음).
3. 기대: 기본 5×5 격자, 보스 방은 격자 한쪽 끝 고정, `StageMaze.TryMove`/`CompleteCurrent` 가 정상 동작(233-245행).
4. 합격: HUD 로 현재 위치·이동 가능 방향이 보이고, 보스 방까지 도달 가능. 메모: ______

### I. 보스 — T10~T12
1. 진입: 미로 보스 방 도달(§H) 또는 BattleAutoPilot BFS 자동 이동.
2. 기대: '보스!' 컷 → 보스 HP 바 → phase2Threshold 도달 시 페이즈 전환(추가 행동) → 특수 공격 예고 → 전용 특수 미니게임 `stage1_boss_special`(Boss_Stage1.specialMiniGameIds) → 방어 성공/실패에 따른 처치 흐름.
3. 합격: 위 순서가 콘솔 로그·화면으로 확인됨, 처치 시 스테이지 클리어(§J)로 이어짐. 메모: ______

### J. 상태이상 4종 + 에너지 봉인(D-7) — T7/T8
1. 대상: 쇠약(2턴, 공격력 -20%) / 초조(2턴, 방어 커맨드 봉쇄) / 과부하(2턴, 코스트 +1) / 화상(3턴, 턴 종료 시 최대HP 5% 고정 피해).
2. 기대: 상태이상 부여 시 UI 아이콘 표시, 효과가 명시된 턴수만큼 지속 후 해제.
3. 에너지 봉인(D-7 재정의): 부여된 다음 턴은 에너지 수집 페이즈를 통째로 건너뛰고 비축분으로만 행동.
4. 합격: 4종 상태이상과 에너지 봉인이 각각 최소 1회 관찰됨. 메모: ______

### K. 파리 잡기 미니게임(MG-44, 에너지 수집용)
1. 진입: 에너지 수집 페이즈에서 `MG-44 파리를 잡아줘!` 가 출제될 때까지 진행(용도 Energy).
2. 조작: 화면을 날아다니는 파리를 탭.
3. 기대: 명중=성공, 놓침/시간 초과=실패, Level 로 파리 속도·크기 난이도 조절.
4. 합격: 성공·실패·시간초과 3경로 모두 확인. 메모: ______

### L. 일반 공격 회피 제거 확인(D-14, T6)
1. 진입: Battle.unity Play, 적 일반 공격 차례.
2. 기대: 회피용 미니게임이 뜨지 않고 즉시 피격 판정(Guard 사용 시 50% 감소, 미사용 시 전체 피해). `Special`(특수 공격) 차례에만 방어 미니게임이 뜬다.
3. 합격: 일반 공격에서 미니게임이 한 번도 뜨지 않음. 메모: ______

### M. 스테이지 진행(복귀 → 다음 스테이지) — T2/T9
1. 진입: 세이브를 `highestClearedStage=0`(또는 백업 초기화)로 두고 `BoolPyeon/Play From Scratch (Auto)` 또는 수동 플레이로 1스테이지(`Stage_1`) 미로를 보스까지 진행.
2. 조작: 보스(`Boss_Stage1`)를 처치해 스테이지 클리어 → HQ 복귀 확인.
3. 기대: `SaveData.highestClearedStage` 가 0→1 로 갱신, HQ 화면에 다음 출격 스테이지가 "2"로 표시(§N 카드 F-35 참고). "출격" 버튼을 누르면 `Stage_2` 미로가 로드됨(Boss_Stage2, S2_* 몬스터 등장).
4. 대조 확인(사망 경로): 1스테이지 도중 사망 시 HQ 복귀 후 `highestClearedStage` 는 그대로(미증가), 출격 시 다시 `Stage_1` 부터 재도전.
5. 합격: 클리어 시에만 스테이지 번호가 올라가고, 출격이 항상 `highestClearedStage+1` 스테이지로 감. 사망 시 같은 스테이지 재도전. 메모: ______

### N. 방어·마법 몬스터 행동 확인 — T4~T8
1. 진입: `Battle.unity` Play, 1스테이지 신규 몬스터(`S1_RubberGloveShield` 방어형 / `S1_AngryMixer` 공격형 / `S1_StaticFairy` 마법형)가 포함된 웨이브(`Wave_S1_GloveTrash`/`Wave_S1_MixerMosq`/`Wave_S1_FairyGlove`/`Wave_S1_Trio`) 또는 2스테이지 웨이브(`S2_TangledCable`/`S2_JammedPrinter`/`S2_PopupAd`, `Boss_Stage2`)로 전투.
2. 기대: 머리 위 예고 아이콘·색상·텍스트가 행동 종류별로 구분됨 — 공격(빨강/'강'), 방어(하늘/'방'), 마법(보라/'마'), 지원·회복(초록/'회+N'). `S1_RubberGloveShield` 는 Guard 위주, `S1_StaticFairy` 는 Magic + 상태이상(D-7 4종 중 하나) 부여, `S1_AngryMixer` 는 잦은 저배율 공격(연타 대체).
3. 대조 확인(2스테이지 난이도): `Boss_Stage2` 는 2페이즈 전환 시 행동 패턴이 바뀌고, Stage_2 배율(HP×1.3, 공격×1.2)로 체감 난이도가 1스테이지보다 높음(수치가 아니라 HP·행동 비중·상태이상 노출로 차이가 남 — D-16).
4. 합격: 네 종류(공격/방어/마법/지원)의 예고 표시가 모두 최소 1회 관찰되고, 적 턴 총 예상 피해가 과도하지 않음(D-12, `BattleScenarioSimulator.RunWaveBalance` 로 사전 확인됨). 메모: ______

### O. 본부 인물 탭 → 메뉴 (UI-01, T5~T7)
1. 진입: `HQ.unity` Play, `HubAgentSpot`(요원 전신 스프라이트, 400x760) 중 하나를 클릭(탭).
2. 기대: 클릭한 스폿의 요원 정보로 `AgentMenuPanel`(§5)이 열림 — 포트레이트(280x280), 이름, 강화/모듈/대화 진입 버튼이 보임. 스폿 4개(Spot1~4, x=-660/-220/220/660, y=184) 모두 각각 다른 요원 메뉴가 열려야 한다.
3. 대조 확인: 스폿이 아닌 배경을 클릭하면 메뉴가 열리지 않음. 메뉴의 닫기 버튼으로 HQ로 복귀.
4. 합격: 4개 스폿 모두 클릭 반응, 메뉴 내용이 스폿별로 다름, None 참조 없음. 메모: ______

### P. 전투 하단 선택지 → 스킬 목록 → 대상 → 발동 (UI-01, T9~T10)
1. 진입: `Battle.unity` Play, 전투 페이즈(②)에서 화면 하단 고정 `CommandPanel`(공격/스킬/방어·아이템/턴종료 4버튼, h220) 확인.
2. 조작: `스킬` 버튼 클릭 → `SkillPanel`(600x120*항목수)이 CommandPanel 바로 위에 열리고 보유 스킬 목록이 전부 보임(카드·드로우 없음, CLAUDE.md 규칙).
3. 조작: 스킬 하나 선택 → 대상 선택(적 유닛 클릭, 월드 영역) → 에너지 코스트 차감·스킬 발동 확인.
4. 대조 확인: 에너지가 부족한 스킬은 비활성(disabled.gray) 표시, 뒤로 버튼으로 SkillPanel을 닫고 CommandPanel로 복귀.
5. 합격: 하단 고정 바에서 공격/스킬/방어·아이템/턴종료 4버튼이 항상 보이고, 스킬→대상→발동까지 팝업 없이 하단 바 흐름으로 끝남. 메모: ______

### Q. 독립 스쿼드 전투 (F-37, 캠페인 T1~T11)
1. 사전 조건: 씬은 `Title.unity`→`HQ.unity`→`Battle.unity` 순서(Glob 확인됨, `Assets/Scenes/Title.unity` `Assets/Scenes/HQ.unity` `Assets/Scenes/Battle.unity`). HQ 에서 직접 편성하거나, `SaveService.Default.Data.squadIds` 를 `["hero"]`/`["hero","cook"]`/`["hero","cook","fashion"]` 로 바꾼 뒤 저장하고 Battle 진입.
2. Q.1 1인 편성(`[hero]`): Battle Play 진입. 콘솔에 `[Squad] turn member=hero` 만 반복해서 나오는지 확인한다.
3. Q.2 2인 편성(`[hero,cook]`): 편성 순서대로 `[Squad] turn member=hero` → `[Squad] turn member=cook` → (적 턴) 순으로 로그가 도는지 확인한다.
4. Q.3 3인 편성(`[hero,cook,fashion]`): 방 진입 시 `[Squad] members=[hero,cook,fashion]` 이 한 번 찍히고, 이후 매 턴 `[Squad] turn member=<id>` 가 hero→cook→fashion 순으로 세 번 돈 뒤 적 턴으로 넘어가는지 확인한다.
5. Q.4 담당 미니게임: 각 멤버 차례에 `[Squad] member=<id> pool=<N>` 로그를 확인하고, 그 차례에 실제로 출제된 미니게임이 그 멤버 담당 불편 유형에서만 나오는지(다른 멤버 전용 미니게임이 섞이지 않는지) 육안으로 본다.
6. Q.5 에너지 분리: 한 멤버가 수집에 성공했을 때 그 멤버의 `BattleUnitView` 에너지 게이지만 올라가고, 다른 멤버 게이지는 그대로인지 확인한다.
7. Q.6 적 대상 분산: `[Battle] enemy target monster=.. kind=.. planned=.. target=<idx>(<이름>)` 로그를 여러 턴 기록해, target 이 살아 있는 멤버들 사이에서 분산되는지(매번 같은 멤버만 맞지 않는지) 확인한다.
8. Q.7 멤버 사망: `BattleDebug` 의 멤버 처치 단축키(`F` = 차례인 멤버 한 명 사망, `Assets/Scripts/BattleFlow/BattleDebug.cs`)로 멤버 하나를 죽인다. `<이름> 쓰러짐!` 로그와 `BattleUnitView` 라벨 `쓰러짐` + 회색(0.3,0.3,0.3,0.5) 표시를 확인한다. 이후 턴 로그(`[Squad] turn member=`)에서 그 멤버가 빠지고, 적 target 로그에도 다시 나오지 않는지 확인한다.
9. Q.8 전멸 패배: `BattleDebug` 의 `D`(편성 전원 사망) 단축키로 전멸시킨다. `[Battle] enemy target..]`/`[Squad] ..` 계열에 전멸 전용 로그 문자열은 없으므로(Grep 확인, `패배|전멸|Defeat` 검색 결과 로그는 `BattleController.TitleCut`/`BattleHud.resultText` 의 화면 문구 `"패배..."` 뿐), 화면에 `패배...` 결과 문구가 뜨고 타이틀 컷·씬 전환이 일어나는지 육안으로 확인한다.
10. Q.9 밸런스 체감 기록표. 비교 기준(T10 목표): 잡몹 3~5턴, 보스 8~12턴, 턴당 에너지 획득 약 5.7~6.2.

   | 편성 | 스테이지 | 턴당 에너지(멤버1/2/3) | 전투 턴 수 | 스킬 사용 수 | 난이도 느낌(1~5) | 메모 |
   |---|---|---|---|---|---|---|
   | 1인 | Stage_1 잡몹 | | | | | |
   | 1인 | Stage_1 보스 | | | | | |
   | 2인 | Stage_1 잡몹 | | | | | |
   | 2인 | Stage_1 보스 | | | | | |
   | 3인 | Stage_1 잡몹 | | | | | |
   | 3인 | Stage_1 보스 | | | | | |

   **T12.2 시뮬레이터 참고치(2026-10-01, `BattleScenarioSimulator`, hpMul 은 3인 기준으로 튜닝된 값 그대로 사용 — 난이도 느낌·메모는 사람이 §2-Q 실제 플레이 후 채운다)**

   | 편성 | 스테이지 | 출처 | 턴당 에너지(멤버 평균) | 전투 턴 수(평균/최대) | 스킬 사용 수(HeavyBlow/Attack) | 비고 |
   |---|---|---|---|---|---|---|
   | 1인 | Stage_1 잡몹 근사(쓰레기봉투x2, hpMul=2.2) | sim(T12.2) | 1.84 | 10.0 / 12 | 91 / 0 | 10/10 시드 미처치(Defeat) — 3인용 hpMul 로는 1인이 시간 안에 못 잡음, 별도 재설계 필요(T10 인계) |
   | 1인 | Stage_1 보스 근사(hp120,atk6, hpMul=3.2) | sim(T12.2) | 2.00 | 5.0 / 5 | 50 / 0 | 10/10 시드 미처치(턴상한5로 세팅, 절대 미처치) — 위와 동일 사유 |
   | 2인 | Stage_1 잡몹 근사 | sim(T12.2) | 4.30(턴당) | 6.4 / 8 | 115 / 39 | 목표(3~5턴) 초과, 3인용 hpMul 로는 2인도 느림 |
   | 2인 | Stage_1 보스 근사 | sim(T12.2) | 3.94(턴당) | 10.0 / 10 | 166 / 62 | 10/10 시드 미처치 — 위와 동일 사유 |
   | 3인 | Stage_1 잡몹(S12, hpMul=2.2) | sim(T12.2) | 5.89(턴당) | 4.6 / 6 | 118 / 29 | PASS(목표 3~5턴) |
   | 3인 | Stage_1 보스(S14, hpMul=3.2) | sim(T12.2) | 6.17(턴당) | 11.5 / 13 | 302 / 86 | PASS(목표 8~12턴) |

   위 1·2인 행은 Stage_1 배율(hpMul)이 3인 스쿼드 화력 기준으로 튜닝돼 있어 나온 참고 수치다(시뮬레이터가 1·2인을 지원하지 못하는 것은 아니지만, 공식 밸런스 판정 기준은 아니다). 1·2인 편성용 배율 재설계는 T10 인계 사항으로 이번 범위 밖이며, 실제 체감은 사람이 §2-Q 절차로 확인한다.

## §2-G 처음부터 플레이(Play From Scratch)

1. 목적: 세이브를 백업·초기화한 뒤 Boot 씬부터 Title→튜토리얼 전투→HQ→출격→전투를 사람 손 없이(또는 손으로) 끝까지 지나가는지 확인한다.
2. 구성: `Assets/Scripts/Editor/PlayFromScratchMenu.cs`(메뉴 3개), `Assets/Scripts/BattleFlow/FlowAutoPilot.cs`(RuntimeInitializeOnLoadMethod 로 자동 생성, DontDestroyOnLoad). 씬·프리팹 변경 없음.
3. 조작(사람 확인):
   - 메뉴 `BoolPyeon/Play From Scratch`: 세이브를 `save.backup.json` 으로 백업하고 초기화한 뒤 Boot 씬을 열고 Play. 직접 마우스로 튜토리얼부터 조작.
   - 메뉴 `BoolPyeon/Play From Scratch (Auto)`: 위와 같고 PlayerPrefs `FlowAutoPilot`=1, `BattleAutoPilot`=1 을 켠다. 자동 진행 중 F10 을 누르면 FlowAutoPilot 이 멈추고 사람이 이어받는다.
   - 메뉴 `BoolPyeon/Restore Save Backup`: `save.backup.json` → `save.json` 복원.
4. 기대: 콘솔에 `[Flow] start` → `[Flow] Title start` → (튜토리얼 전투는 BattleAutoPilot 로그) → `[Flow] HQ arrive (post-tutorial, not counted)` → `[Flow] HQ sortie` → `[Flow] HQ arrive cycles=1` → … → `[Flow] DONE cycles=2`. Error/Exception 0.
5. 2026-09-28 RunCommand 로 `PlayFromScratchMenu.PlayFresh(true)` 실행: 컴파일 에러 0(수정 전 `Input.GetKeyDown` 네임스페이스 충돌 1건 수정), Play 진입 후 약 40초 뒤 확인 시 `EditorApplication.isPlaying=true`, `activeScene=Battle`, `FlowAutoPilotPref=0`(FlowAutoPilot 이 이미 Stop() 호출됨 — DONE/FAIL/STUCK 중 어느 것인지는 GetConsoleLogs 버퍼가 초기 로그를 밀어내 특정하지 못함), `GetConsoleLogs(Error,Exception)=0건`. Boot→Title→(튜토리얼)Battle→HQ→(적어도 1회)Battle 까지는 씬 전환이 실제로 일어난 것을 확인했다(활성 씬이 Battle로 두 번째 진입해 있었음). Play 는 이후 수동으로 정지시켰다.
6. 사람 확인 필요: `[Flow] DONE cycles=2` 로그가 실제로 찍히는지, 회당 사이클 카운트가 의도대로 도는지, 수동 모드의 조작감은 이번 build 단계에서 확인하지 못했다(§2-F 와 같은 콘솔 로그량 문제로 초기 `[Flow]` 로그가 유실됨). 테스트 뒤에는 `Restore Save Backup` 으로 원래 세이브를 되돌려야 한다.
7. **T14(2026-09-29)**: `BoolPyeon/Play From Scratch (Auto)` 메뉴는 RuntimeInitializeOnLoadMethod 로 `FlowAutoPilot` 을 자동 생성하고, `BattleAutoPilot.ChooseMazeDirection`(BFS) 이 미로 이동까지 자동으로 처리하도록 이미 연결돼 있다(플래너 단계에서 확인). 이번 build 단계에서도 Play 모드 도메인 리로드 때문에 `[Flow] DONE cycles=2` 실시간 캡처는 RunCommand 로 재현하지 못했다(§4, 기존 미해결 이슈와 동일). **사람 확인 필요**: 메뉴 실행 후 콘솔에 `[Flow] DONE cycles=2` 와 `[AutoPilot] END victory=..` 가 찍히고 EventSystem 중복 경고가 없는지 직접 확인한다.
8. **EventSystem 정리(T14)**: 씬별 EventSystem 조사 결과 호스트 씬(Battle/HQ/Title/SampleScene/DialogueSample)에 1개씩, Boot·새 미니게임 씬(FlySwat/BossBlob 등)에는 없음, Additive 로 불리는 Egg/Pizza/USB 미니게임 씬에만 1개씩 남아 있었다(UI Selectable 0). 이 셋에서 EventSystem 오브젝트를 MCP 로 삭제·저장했다 — Additive 활성화 순간(MiniGameRunner 가 파괴하기 전) 중복 경고가 나던 원인 제거. Single 로드는 `SceneLoader.DestroyCurrentEventSystems` 가 처리. 세 씬은 Physics2D 클릭이고 `PointerInput` 이 `EventSystem.current == null` 을 처리하므로 단독 Play 도 동작해야 한다(사람 확인).
9. **T14.1(2026-09-29) 원인 특정**: `BoolPyeon/Play From Scratch (Auto)` 를 `EditorApplication.ExecuteMenuItem` 로 실행 → Play 진입 확인, 이후 폴링에서 `activeScene=Battle`, `FlowAutoPilot`/`BattleAutoPilot` 오브젝트 존재를 재확인(부트→타이틀→HQ→전투 전환이 실제로 일어남, 에러 없음). 이 상태에서 `Unity_GetConsoleLogs` 로 `Debug.Log`(Info) 메시지를 조회했으나 매번 0건이었던 반면, 같은 세션에서 `Debug.LogError` 로 남긴 테스트 로그는 정확히 캡처됐다 — **`Unity_GetConsoleLogs` 도구가 Play 모드의 Info 레벨 로그를 사실상 반환하지 않는 것이 `[Flow] DONE`/`[AutoPilot] END` 캡처 실패의 근본 원인**(이전 인계의 "도메인 리로드" 추정보다 더 구체적인 진단). Error/Exception 레벨은 도구로 확인 가능하므로, 콘솔 에러 0 확인 자체는 계속 자동으로 할 수 있다. Info 로그(DONE/END 등) 확인은 사람이 에디터 콘솔 창을 직접 봐야 한다. (주의: 이 세션에서 진단용으로 남긴 `Debug.LogError("[Probe] error test")` 는 FlowAutoPilot 을 의도대로 FAIL 정지시켰다 — 실제 게임 결함이 아니라 진단 부산물이니 콘솔에 남아 있어도 무시한다.)
10. **T14.2(2026-09-29) 콘솔 지우기 재시도**: 위 `[Probe] error test` 를 없애려고 `Unity_RunCommand` 로 `UnityEditorInternal.LogEntries.Clear()`, `Debug.ClearDeveloperConsole()` 를 각각 실행(둘 다 실행 성공 로그 확인) → 직후 `Unity_GetConsoleLogs` 로 재조회하면 두 방법 모두 해당 항목이 그대로 남아 있었다(2회 재현). **결론: `Unity_GetConsoleLogs` 는 실제 Unity 콘솔이 아니라 MCP 가 별도로 유지하는 로그 캐시를 읽으며, 이 캐시를 코드로 비우는 방법이 이번 MCP 버전에는 없다.** Info 레벨 미캡처(9번)와 같은 계열의 MCP 도구 한계로 확정한다 — 콘솔 항목 삭제는 사람이 에디터 콘솔 창의 Clear 버튼으로 해야 한다.
11. **T14(2026-09-30) 통합 회귀 검수**: §1-A 절차 전체 실행. (1) `Unity_GetConsoleLogs(Error)`=0건. (2) `TestRunnerApi` EditMode 전체 스위트 RunCommand 로 실행: PassCount=151, FailCount=0. (3) `BattleScenarioSimulator.RunAll()`: 8/8 PASS. (4) Title/HQ/Battle 세 씬을 SerializedObject 로 전수 순회해 커스텀 스크립트 public 필드 None 여부 확인: None 으로 나온 항목은 전부 Unity 기본 `Image/Text.m_Material`(항상 None 이 기본값), 일부 버튼의 `m_Sprite`(기본 흰 사각형으로 정상), `EventSystem.m_FirstSelected`(선택사항) 뿐이었고, `BattleUnitView.intentIcon/intentText` 가 PlayerView 에서 None 인 것은 `BattleUnitView.cs` 108행에서 null 체크 후 끄도록 설계된 의도된 동작(적 유닛만 인텐트 아이콘을 씀) — 실질적인 미연결 버그 없음. (5) `PlayFromScratchMenu.PlayFresh(true)` 를 RunCommand 로 실행 후 폴링: Boot→Title(FlowAutoPilotPref=1)→Battle(튜토리얼, BattleAutoPilotPref=1)까지 씬 전환 실제 확인, 매 폴링 시점 `GetConsoleLogs(Error,Exception)`=0건. 이후 `EditorApplication.isPlaying=false` 로 수동 정지(원래 세이브는 `PlayFresh` 가 `save.backup.json` 으로 자동 백업했으므로 `Restore Save Backup` 메뉴로 복원 필요 — 사람 확인 항목에 추가). `[Flow] DONE cycles=2`/`[AutoPilot] END` Info 로그 확인은 9번 항목과 같은 MCP 한계로 이번에도 자동 확인 불가, 사람이 에디터 콘솔에서 직접 확인해야 한다. **T14 완료 기준(컴파일 0·EditMode 전수·None 전수 점검·Title→HQ→Battle 전환 확인) 충족.**

## §2-R VacuumWidenMiniGame 사람 확인 (캠페인 T6, 2026-10-01)

1. 진입: `Assets/Scenes/MiniGames/VacuumWidenMiniGame.unity` 열고 Play.
2. 조작: `WidthHandle`(흡입구 옆 손잡이)을 끌어 흡입구 폭을 가구 틈(`GapZone`) 폭에 맞춘 뒤, `Head`를 위로 밀어 넣는다.
3. 기대: VacuumWidenMiniGame Play → 핸들로 폭을 틈에 맞춰 밀면 Success, 넓으면 쾅 Failed(좁으면 먼지가 남아 Failed). `ResultRoot`의 Animator(`VacuumWidenResult`) `isSuccess`/`isFailed` bool로 전환.
4. 합격: 폭을 틈에 맞춰 밀어 넣으면 Success 연출, 너무 넓게 벌린 채 밀면 가구에 걸려 쾅(Wide 실패), 너무 좁으면 먼지가 남아 Failed(Narrow 실패) — 세 경로 모두 확인.
5. 메모: ______

## §3 결과 기록

| 시나리오 | 결과(P/F) | 콘솔 로그 / 메모 |
|---|---|---|
| A 페이즈 루프 | | |
| B 단일 미니게임 | | |
| C 씬 단독 실행 | | |
| D 입력 회귀 | | |
| E 전투 EditMode | P | EditMode 전체 151/151 PASS, FailCount=0(2026-09-30 T14 재확인) |
| F 전투 자동 시나리오(로직 S1~S6) | P | RunCommand BattleScenarioSimulator.RunAll() 8/8 PASS (2026-09-30 T14 재확인) |
| F 전투 자동 시나리오(씬 오토파일럿 완주) | | 1개 층 부분 확인(§2-F.5), 전체 런·Defeat 경로는 사람 확인 필요 |
| G 처음부터 플레이(Play From Scratch) | | 컴파일 0·Error 0, Boot→Title→Battle 전환 재확인(§2-G.11), DONE 로그·사이클 카운트·조작감은 사람 확인 필요 |
| H 미로 | | 코드 구현 확인(§H), 사람 플레이 확인 필요 |
| I 보스 | | 코드 구현 확인(§I), 사람 플레이 확인 필요 |
| J 상태이상·에너지 봉인 | | 코드 구현 확인(§J), 사람 플레이 확인 필요 |
| K 파리 잡기 | | 코드 구현 확인(§K), 사람 플레이 확인 필요 |
| L 일반 공격 회피 제거 | | 코드 구현 확인(§L), 사람 플레이 확인 필요 |
| M 스테이지 진행(복귀→다음 스테이지) | | EditMode 근거(D1) 확인: EditMode 151/151 PASS 안에 SaveServiceTests/RunStateTests 포함(2026-09-29 T10). 실기(D2)는 `PlayFresh(true)` 로 Play 진입·Boot→Title→(튜토리얼)→Battle 전환까지 확인됐으나, RunCommand 폴링(약 62초, 5회) 동안 `RunState.Current.StageNumber` 가 1에서 바뀌지 않고 `SaveService.Default.Data.highestClearedStage` 도 0 — 자동 진행이 1스테이지 클리어까지 도달하는 것을 이번 세션에서 시간 안에 확인하지 못함(§4). Play 는 정지 후 RestoreBackup 완료. 사람 플레이 확인 필요(§5). |
| N 방어·마법 몬스터 행동 | | 코드·시뮬레이터 확인(§N, D-16, RunWaveBalance 11/11 PASS), 실기 예고 아이콘 육안 확인 필요 |
| O 본부 인물 탭 → 메뉴 | | 씬 구성 확인(§O, `HubAgentSpot` 4개·`AgentMenuPanel` 연결, T5~T7), 실기 클릭 반응 육안 확인 필요 |
| P 전투 하단 선택지 → 스킬 목록 → 대상 → 발동 | | 씬 구성 확인(§P, `CommandPanel`/`SkillPanel` 연결, T9~T10), 실기 조작 흐름 육안 확인 필요 |
| Q 독립 스쿼드 전투(1·2·3인) | P | T11 에서 1인/3인 `memberCount` 확인, 콘솔 에러 0(§Q.1~Q.8). EditMode 전수 자동 실행(2026-10-01, T12.2, RunCommand+TestRunnerApi+SessionState): 178/178 PASS, 0 fail(BoolPyeon.Battle.Tests 포함 전체 asmdef). §2-Q 실제 플레이 체감(Q.1~Q.8 손 조작)은 여전히 사람 확인 필요. |
| Q.9 밸런스 체감 기록 | | 시뮬 참고치 기입 완료(2026-10-01, T12.2, §2-Q.9 표 아래 소표). BattleScenarioSimulator.RunAllReports() 11/11 PASS(S1~S8,S12~S14), S12=4.6턴/S14=11.5턴으로 T10 목표(잡몹3~5, 보스8~12) 충족. 체감 기록(난이도 느낌·메모)은 사람이 §2-Q 절차대로 실제 플레이해야 채울 수 있다. |
| R VacuumWiden 미니게임 | | 코드·씬 구성 확인(T3~T5, 컴파일 에러 0, MiniGameDefinition·EditorBuildSettings 등록 확인). 조작감·Success/Wide-Failed/Narrow-Failed 세 경로 손 조작 확인은 사람 확인 필요(§2-R). |

## §4 알려진 제한
- USB 는 별도 정의 에셋(`Assets/Data/MiniGames/USB.asset`)이 없다. 페이즈 풀은 `DummyTap` 정의가 `USBMiniGame.unity` 씬을 사용해 대신한다(§0 참고).
- `BattleController`(F-14) 는 씬으로 구현돼 있고, 순수 로직 시나리오(§2-F, S1~S6)는 RunCommand 로 자동 확인된다. `BattleAutoPilot` 으로 씬 Play 오토파일럿도 가능하나(§2-F), 여러 웨이브·런 전체 완주와 Defeat 경로, 실제 마우스 조작감은 아직 사람이 확인해야 한다.
- ~~Test Runner(EditMode)에서 BoolPyeon.Battle.Tests(BattleScenarioTests 포함)를 직접 돌리는 것은 MCP 가 지원하지 않아 사람이 확인해야 한다~~ **정정(2026-09-29 T10)**: RunCommand + `TestRunnerApi`(비동기 콜백 결과를 `SessionState` 에 저장해 다음 RunCommand 에서 읽는 방식)로 전체 EditMode(151개, 19개 파일)를 자동 실행·확인할 수 있었다(151/151 PASS). MCP 미지원 기술은 낡았다.
- **T10 통합 검수(2026-09-29)**: Battle.unity·HQ.unity 를 열어 모든 MonoBehaviour 의 SerializedObject 오브젝트 참조를 순회한 결과, 커스텀 게임 필드 중 null 은 3개뿐이었다 — `PlayerView(BattleUnitView).intentIcon/intentText`, `PhaseHudRoot(MiniGamePhaseHud).startHintText`, `SquadRow_0/1/2(SquadRow).def`. 셋 다 코드에 null 가드가 있어(`BattleUnitView.cs`, `MiniGamePhaseHud.cs`, 런타임에 채워지는 `SquadRow.def`) 의도된 null 로 판단했다(문제 아님). 그 외 null 은 전부 UGUI 내장 필드(`Image.m_Material`, 일부 `Image.m_Sprite`, `EventSystem.m_FirstSelected`)로 Unity 표준상 정상.
- **T10 오토파일럿 완주 미확인(2026-09-29)**: `PlayFromScratchMenu.PlayFresh(true)` 로 Play 진입은 성공하고 Boot→Title→Battle 전환까지 확인했으나, 이후 약 62초(5회 폴링) 동안 `RunState.Current.StageNumber` 가 1에서 바뀌지 않았다(1스테이지 클리어·HQ 복귀·재출격까지 도달 못함). 시간 제약으로 EditMode 근거(SaveServiceTests/RunStateTests, 151/151 통과 안에 포함)로 대체했다. 실기 StageNumber=2 확인은 사람 몫(§5). **T10.1 결정: 로그 우회 코드는 넣지 않는다. §5의 사람 확인으로 대신한다.**
- **T8.2 이슈 유지**: 1·2스테이지 예고 피해 최댓값이 둘 다 7(cap 7.5)로 수렴 — 조정 여부는 사람 결정 필요. **T10.1 결정: 현 상태 유지. D-12 cap 7.5 이내이고, 스테이지 차이는 배율·행동 종류로 둔다. 상한을 스테이지별로 바꾸는 일은 디자이너 후속 판단.**
- **Boss_Stage2 미니게임 ID(2026-09-29 T10)**: `Assets/Data/Battle/Stage2/Boss_Stage2.asset` 의 `specialMiniGameIds` = `["stage1_boss_special"]` — 전용 ID가 아직 없어 Boss_Stage1 ID 를 이미 재사용 중이다(설계 권고와 일치, 값 비어있지 않음). `evasionMiniGameIds` 는 빈 배열. **T10.1 결정: 재사용 확정(BossBlob.asset id=stage1_boss_special). 전용 미니게임은 후속 과제.**
- `MiniGamePhaseDebug.autoRunOnStart = False` — 자동 시작을 기대하면 안 된다. 자동 시작을 원하면 인스펙터에서 체크박스를 켠다(코드 변경 아님).
- **T14(2026-09-29) EventSystem 중복 경고**: 원인은 `MiniGameRunner.RunRoutine` 이 Additive 로 미니게임 씬을 `LoadSceneAsync` 로 불러올 때 씬이 활성화(Awake/OnEnable)된 뒤에야 `DisableHostedComponents` 로 EventSystem 을 파괴해, 그 사이 한 프레임 동안 호스트(Battle/Title/HQ)와 미니게임 씬 EventSystem 이 동시에 활성 상태였던 것이다. 조치: `MiniGameRunner.cs` 에서 `loadOp.allowSceneActivation = false` 로 로드하고, 씬이 활성화되기 **전**(progress≥0.9, Awake/OnEnable 전)에 `DisableHostedComponents` 로 EventSystem·Camera·AudioListener 를 먼저 정리한 뒤 `allowSceneActivation = true` 로 전환하도록 바꿨다. 씬(.unity) 은 건드리지 않았다(호스트 씬 EventSystem 은 그대로 유지). 컴파일 에러 0 확인(RunCommand). 실시간 재현으로 경고가 실제로 사라지는지는 사람이 Play 로 확인해야 한다(§2-G.7, dummy_tap 등 미니게임 페이즈 진입 시).
- BattleScenarioSimulator.cs 330행의 `dummy_tap` 두 번째 하드코딩은 테스트 픽스처라 그대로 둔다(정리는 선택 사항, T13.2 인계와 동일).

## §5 T10 통합 검수 사람 확인 목록

1. `BoolPyeon/Play From Scratch (Auto)` 실행 후 에디터 콘솔에서 `[Stage] result cleared=1 next=2` 와 `[Flow] DONE cycles=2` 로그를 직접 확인한다(MCP 는 Play 모드 Info 로그를 못 읽는다).
2. HQ 화면에 다음 출격 스테이지 `2` 표시를 확인하고, 출격 시 `Stage_2` 웨이브와 S2_* 신규 몬스터(JammedPrinter/PopupAd/TangledCable)가 등장하는지 확인한다(§M).
3. 전투 중 사망 시 `Stage_1` 로 재도전(스테이지 번호가 올라가지 않음)하는지 확인한다.
4. §N 신규 몬스터 예고 아이콘 4종(방어/마법/지원 포함)을 육안 확인하고, `Boss_Stage2` 의 2페이즈 전환(50% 체력, `firewall_all` 등 phase2Actions)을 확인한다.
5. 확인이 끝나면 `BoolPyeon/Restore Save Backup` 을 실행하고 에디터 콘솔을 Clear 한다.
6. §3 A~L(페이즈 루프, 단일 미니게임, 씬 단독 실행 등 기존 항목) 결과 칸을 실측으로 채운다 — 이번 T10 단계는 신규 기능(M/N)과 회귀 확인(A~L 중 D1/D2, E) 범위만 다뤘다.
7. (T15 추가) §O 본부 인물 탭·§P 전투 하단 선택지 시나리오를 실기로 확인하고 §3 O/P 결과 칸을 채운다.

## §6 대화 표정 사람 확인 절차 (캠페인 T7, 2026-10-01)

- 자동 확인(2026-10-01, RunCommand): `Assets/Scenes/Dev/DialogueSample.unity` Play 모드에서 `DialogueBox.Play`/`Advance` 로 회사원(Panic→Relieved)·불편의 악마(Angry→Laugh)·스토리 요원(Joy→Surprised) 6줄을 재생 — 매 줄 `PortraitResolver` 결과 스프라이트가 화자·표정에 맞게 바뀜을 로그로 확인, `ScreenCapture.CaptureScreenshot`(Temp/, Assets 밖) 으로 스토리 요원·불편의 악마 2장 실제 Game 뷰 캡처 확인(초상화·이름표 정상 표시). 콘솔 에러 0건. `Unity_Camera_Capture`(Scene View 카메라)는 기존 한계(§UI_DESIGN.md §16.1)대로 Overlay Canvas 를 담지 못해 흰 화면 — Play 중 `ScreenCapture.CaptureScreenshot` 로 대체(UI_DESIGN.md §25.3 근거).
- 사람 확인(실제 DLG_* 대사 흐름, 위 자동 확인은 임의 줄 주입이라 대체 불가):
  1. `DLG_Tutorial_Opening`(튜토리얼, 회사원↔불편의 악마) 또는 `DLG_Tutorial_Boss`(회사원+불편의 악마)를 Title→HQ→Battle 진행 중 실제로 띄워, 화자가 바뀔 때마다 이름표·초상화가 즉시 바뀌고 같은 화자라도 `expression` 이 다르면 표정이 바뀌는지 육안 확인한다.
  2. `DLG_Story_Talk`/`DLG_Story_Stage1~3`(스토리 요원) 진행 중 Joy/Default 등 표정 전환이 어색하지 않은지(펀치 연출로 잠깐 커졌다 돌아오는 튐이 튀지 않는지) 확인한다.
  3. `DLG_Tutorial_Outro`(불편의 악마 + COMFORT 사령관 + 회사원 3화자)에서 화자 전환이 잦은 구간의 가독성(이름표·초상화 겹침 없음)을 확인한다.
  4. 표정 이미지가 없는 화자(안내/COMFORT 사령관, Default 하나뿐)가 대사 중 `expression` 이 잘못 지정돼도 `Default` 로 폴백해 포트레이트가 사라지지 않는지 확인한다.

## §7 전투 스킬·피격 모션 사람 확인 절차 (캠페인 T8, 2026-10-01)

- 자동 확인(2026-10-01, RunCommand+Capture): `Battle.unity` Play 모드에서 `squadIds=[hero,cook,fashion]`(3인 편성)으로 세 `BattleUnitView.PlaySkill()`을 직접 호출 — 셋 다 대기 스프라이트에서 `_Skill_Anim_1` 프레임으로 바뀌고 `lungeDistance`만큼 전진한 것을 `Unity_SceneView_Capture2DScene`으로 확인(UI_DESIGN.md §26 근거). 이어 세 멤버의 `hitSprite`(`Hero/Cook/Fashion_Hit`)로 교체한 프레임도 캡처 확인. 콘솔 에러 0건. EditMode 전수 실행(`TestRunnerApi`+`SessionState` 폴링) 195/195 PASS, fail 0.
- 사람 확인(실제 손 조작, 위 자동 확인은 RunCommand 로 뷰 메서드를 직접 호출한 것이라 대체 불가):
  1. `Battle.unity` Play → HQ 에서 hero·cook·fashion 3인 편성 후 Battle 진입. 멤버마다 차례에 스킬 커맨드를 눌러 전진 3프레임(f0→lunge f1→절반 복귀 f2) 애니메이션과 `[Anim] member=<id> motion=skill` 로그를 확인한다.
  2. 적 턴에 멤버가 공격을 맞을 때 `hitSprite`로 바뀌며 0.28초 좌우로 흔들리다 원래 자세로 돌아오는지 확인한다.
  3. `skillFrames`가 3장 미만인 멤버(있다면)가 스킬 커맨드를 써도 공격 모션으로만 재생되는지(추가 프레임 전환 없이) 확인한다.
  4. 방어 커맨드를 눌러 `defendFrames`가 순서대로 재생된 뒤 마지막 프레임에서 잠깐 멈추는지 확인한다.

## §8 Title 리뉴얼 사람 확인 절차 (캠페인 T6, 2026-10-01)

- 자동 확인(2026-10-01, RunCommand+Play): `Title.unity` Play 모드에서 `TitleScreen.startButton.onClick.Invoke()`를 두 번 호출(1차=인트로 스킵, 2차=실제 전환)해 `Title`→`Battle` 씬 전환을 확인, `TitleMenu.OpenSettings()`/`Close()` 호출로 팝업 열림·닫힘(`IsOpen` true→false)을 확인했다. `Unity_GetConsoleLogs`(Error) 0건. 16:9/19.5:9/4:3 3개 해상도 CanvasScaler 수식 재계산으로 CLIP·겹침은 UI_DESIGN.md §3(T6 실측 검증 문단) 참고 — 같은 레이어 전경 요소 간 의도치 않은 겹침 0건, 발견된 CLIP 2건(CharacterLayer 19.5:9, Logo 4:3 1px)은 모두 경미하거나 자리표시 상태의 이월 사항.
- 사람 확인(실제 손 조작·눈으로 보는 체감, 위 자동 확인은 `onClick.Invoke()` 직접 호출이라 터치·연출 타이밍 체감을 대체하지 못함):
  1. **연출 템포**: `Title.unity` Play → 배경 페이드인(0~0.4s)·로고 팝인(0.3~0.75s, scale 0.6→1.08→1.0)·TouchToStart 등장(0.9s)이 끊기거나 너무 느리게/빠르게 느껴지지 않는지 확인한다(§27.4 시간표 기준).
  2. **깜빡임 가독성**: TouchToStart 문구가 1.2초 주기로 alpha 1↔0.25로 깜빡이는 동안 글자가 너무 흐려지거나 눈이 피로할 정도로 빠르지 않은지 확인한다.
  3. **메뉴 오클릭 여부**: BottomMenu(좌하단, 120x120 버튼 3개 간격 24px)와 StartButton(전체 화면 투명)이 겹치는 영역에서, 버튼을 누르려다 의도치 않게 화면 전환(StartButton)이 먼저 반응하지 않는지, 반대로 화면을 터치했는데 메뉴 버튼이 가로채지 않는지 손으로 직접 눌러 확인한다. 설정·공지·크레딧 팝업이 열려 있을 때 배경을 탭해도 전환되지 않는지(`TitleScreen.OnStart`의 `menu.IsOpen` 가드) 함께 확인한다.
  4. 19.5:9 비율 기기(또는 Device Simulator)에서 CharacterLayer(아트 적용 전 자리표시)가 위쪽으로 잘려 보이는지 육안으로 가늠한다(위 CLIP 2건 중 하나, 아트 수급 전까지는 참고용).

## §9 미니게임 히트 영역 사람 확인 (히트 영역 캠페인 T6)

자동 확인(2026-10-01, RunCommand): 8개 미니게임의 클릭·드래그 대상 Collider2D/스폰 radius/프리팹 scale을 전수 실측 — 120px 미달은 ButtonShirt Button3·4(97.2px, 승인된 예외) 뿐, 그 외 전부 120px 이상이고 인접 대상 겹침 0건(`Docs/UI_DESIGN.md` §28 참고). 다만 이 수치는 정적 실측이라 실제 터치 조작감은 사람이 직접 확인해야 한다.

1. **PeelSticker**: `PeelStickerMiniGame.unity` Play → CornerTab(151.2px) 모서리를 터치로 잡아 떼어낼 때 스티커 그림 바깥쪽 여유 영역에서도 잡히는지 확인한다.
2. **Vacuum**: `VacuumMiniGame.unity` Play → Handle(141.0px) 손잡이를 끌 때 옆의 Head·Dust·SofaEdge와 혼동 없이 손잡이만 반응하는지 확인한다.
3. **ButtonShirt**: `ButtonShirtMiniGame.unity` Play → Button0~2(140.4px) 가장자리를 잡아 끌면 잡히고 옆 단추는 딸려오지 않는지, Button3·4(97.2px, 좁음)도 정확히 중심을 누르지 않아도 잡히는지 확인한다.
4. **SamgakKimbap**: `SamgakKimbapMiniGame.unity` Play → Tab_Top/Left/Right(155.5px) 모서리를 끌어 `pullDistance`만큼 당기면 Success로 이어지는지, 세 탭이 서로 간섭 없이 각각 반응하는지 확인한다.
5. **FlySwat**: `FlySwatMiniGame.unity` Play → 난이도 3(가장 작은 파리, 약 131.8px)에서도 손가락/마우스로 때리기 쉬운지, 난이도 1(155.5px)과 체감 차이가 과하지 않은지 확인한다.
6. **BossBlob**: `BossBlobMiniGame.unity` Play → 난이도 3(가장 작은 Blob, 약 129.6px)이 떨어질 때 터치가 잘 반응하는지 확인한다.
7. **ScreenBubble**: `ScreenBubbleMiniGame.unity` Play → 카드를 버블 쪽으로 끌 때(`cardRadius` 0.5 기준) 작은 버블(`bubbleScales` 최소 0.6)도 쉽게 접촉 판정이 나는지 확인한다.
8. **TapTarget**: `TapTargetMiniGame.unity` Play → Target/Decoy(scale 1.3, 140.4px) 탭이 그림 크기 대비 과하게 크거나 작게 느껴지지 않는지, Decoy를 잘못 누르는 일이 늘지는 않았는지 확인한다.

확인 결과는 위 §3 결과 기록 표에 게임별 행을 추가해 P/F로 채운다(이번 build 단계는 자동 실측까지만 완료, 손 조작 확인은 사람 몫).
