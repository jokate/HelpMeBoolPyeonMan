# UI 디자인 (도와줘요 불편맨)

## §0 현황 (T1, 2026-09-30)

### 0.1 점검 방법
- Unity MCP `Unity_RunCommand`(읽기 전용 C#)로 Title → HQ → Battle 씬을 순서대로 `EditorSceneManager.OpenScene`으로 열고, 각 씬의 루트부터 깊이 4까지 `Canvas/CanvasScaler/RectTransform(anchorMin·anchorMax·pivot·sizeDelta·anchoredPosition)/LayoutGroup/ContentSizeFitter`를 덤프했다.
- 씬은 저장하지 않았다(마지막에 Battle.unity를 다시 열어 dirty 상태를 버림). 이미지 캡처(Camera_Capture 등)는 실행하지 않고 실측 수치로 겹침·잘림을 판정했다(§0.10 참고, 미캡처).
- 해상도 기준: 1920x1080. CanvasScaler가 모두 `ScaleWithScreenSize` + refRes 1920x1080이라 실측 앵커 비율이 곧 1920x1080 픽셀 좌표로 환산된다.

### 0.2 씬별 Canvas 요약
| 씬 | Canvas | renderMode | Scaler mode | refRes | matchWidthOrHeight |
|---|---|---|---|---|---|
| Title | Canvas | ScreenSpaceOverlay | ScaleWithScreenSize | 1920x1080 | 0.5 |
| HQ | Canvas | ScreenSpaceOverlay | ScaleWithScreenSize | 1920x1080 | **0 (Width만)** |
| Battle | Canvas | **ScreenSpaceCamera** | ScaleWithScreenSize | 1920x1080 | 0.5 |

문제: 세 씬의 Canvas 설정이 서로 다르다. HQ만 `matchWidthOrHeight=0`(가로 기준)이라 세로로 긴 기기에서 Title/Battle과 다르게 늘어난다. Battle만 `ScreenSpaceCamera`라 카메라 설정(Plane Distance 등)에 따라 스케일이 달라질 수 있다. **T13에서 세 씬을 동일 기준으로 통일해야 한다.**

### 0.3 씬·패널별 문제 표
| 씬 | 패널/오브젝트 | 경로 | 앵커·크기 | 문제 유형 | 설명 | 심각도 |
|---|---|---|---|---|---|---|
| Title | Canvas | Canvas | anchorMin/Max (0,0)-(0,0) | 앵커 고정처럼 보임 | 루트 Canvas의 RectTransform 값은 Canvas 컴포넌트가 덮어써 무의미하다(문제 아님, 오해 소지로만 기록) | 하 |
| Title | Logo | Canvas/SafeArea/Logo | pivot(0.5,1) pos(0,-80) size900x360, **activeSelf=false** | 미사용 | 비활성 상태로 방치. 켜면 TitleText(앵커 y0.70-0.95)와 겹칠 가능성이 큼(상단 영역 중복) | 중 |
| Title | StartButton | Canvas/SafeArea/StartButton | anchor(0.5,0.15) size480x140 | 빈 공간 | 화면 하단 15% 지점에 버튼 하나만 있고 나머지 UI 없음. 레이아웃 정보량이 적어 T4에서 재설계 여지가 큼 | 하 |
| HQ | AgentMenuPanel/LineText vs UpgradeSection·ModuleSection·StorySection | Canvas/AgentMenuPanel/{LineText, UpgradeSection...} | LineText anchor(0,0.30)-(1,1.00) sizeDelta(-40,-305); 각 Section anchor(0,0)-(1,0.62) sizeDelta(-40,-25) | **겹침**(§0.6 참고) | 두 영역이 패널 내부 y비율 0.30~0.62 구간에서 겹친다. LineText가 상시 텍스트이고 Section이 같은 시점에 켜지면 텍스트와 강화/모듈/스토리 목록이 겹쳐 보인다 | 상 |
| HQ | AgentButtons 내부 Story 버튼 | Canvas/AgentButtons/AgentButton_Story | anchoredPos(0,-300), size250x250 | 배치 이상 | 같은 컨테이너(AgentButtons, size700x300)의 Upgrade/Module은 y=0에 나란히 있는데 Story만 y=-300으로 컨테이너 밖(세로로 한참 아래)에 떨어져 있어 "3버튼 정렬"처럼 안 보이고 따로 노는 것처럼 보인다 | 중 |
| HQ | SquadPanel | Canvas/SquadPanel | anchor(0.5,0.5) size700x500, VerticalLayoutGroup | 크기 확인 필요 | TitleText+SquadRow_0~2(각 60)+CloseButton을 VerticalLayoutGroup에 맡겼는데 sizeDelta가 고정 700x500이라 행이 늘어나면(스쿼드원 4명 이상) 잘릴 수 있음(ContentSizeFitter 없음) | 중 |
| HQ | DialogueBox | Canvas/DialogueBox | BoxBg anchor(0,0)-(1,0) sizeDelta(0,280) | 확인됨(문제 아님) | 하단 대사창 패턴, Battle의 DialogueBox와 구조가 동일(중복 구현, T3에서 공용화 후보) | 하 |
| Battle | TopBar 내부 HpBar/EnergyText | Canvas/BattleHud/TopBar/{HpBar,EnergyText} | HpBar pivot(0,1) pos(0,0) size620x44(0~-44); EnergyText pos(0,-60) size620x70(-25~-95) | **겹침(경미)** | 로컬 y -44~-25 구간(약 19px)에서 HpBar 하단과 EnergyText 상단이 겹친다 | 중 |
| Battle | PhaseHudRoot/InstructionText vs MiniGameTimeBar·PhaseTimeBar | Canvas/PhaseHudRoot/{InstructionText, MiniGameTimeBar, PhaseTimeBar} | InstructionText anchor(0.15,0.85)-(0.85,0.95); MiniGameTimeBar(0.20,0.91)-(0.80,0.95); PhaseTimeBar(0.20,0.95)-(0.80,1.00) | **겹침**(§0.6 참고) | InstructionText가 켜지면 y 0.91~0.95 구간에서 두 타임바와 겹친다(현재 InstructionText는 기본 비활성) | 중 |
| Battle | PhaseHudRoot/FeedbackText·ResultPanel·StartHintText | Canvas/PhaseHudRoot/{FeedbackText, ResultPanel, StartHintText} | FeedbackText(0.30,0.45)-(0.70,0.60); ResultPanel(0.30,0.35)-(0.70,0.65); StartHintText(0.25,0.40)-(0.75,0.50) | 겹침(순차 표시 전제, §0.6 참고) | 세 오브젝트가 화면 중앙에서 서로 넓게 겹친다. 코드가 동시에 켜지 않는다는 전제라면 문제 아니지만, 문서화되지 않아 향후 실수 위험 | 중 |
| Battle | ResultPanel 이름 중복 | Canvas/BattleHud/ResultPanel (전투 승패), Canvas/PhaseHudRoot/ResultPanel (미니게임 단계 결과) | 각각 다른 부모 | 명명 혼동 | 같은 씬에 "ResultPanel"이라는 이름이 두 곳에 있어(용도가 다름: 전투 종료 vs 미니게임 단계 종료) 사람이 헷갈리기 쉽다 | 하 |
| Battle | BackGroundImage | Canvas/BackGroundImage | sizeDelta(0,-0.93) | 미세 오차 | 거의 무의미한 음수 오프셋(-0.93px), 실질적 문제 아님 | 하 |
| Battle/HQ | 모든 버튼 자식 Text | 다수 | GetComponentInChildren 의존 | 구조 의존 | §0.4 (C) 참고 | - |
| 전체 | 모든 활성 패널 | - | SafeArea 컴포넌트 없음 | 잘림 위험 | Title의 "SafeArea"라는 이름의 오브젝트는 있지만 풀스트레치 빈 컨테이너일 뿐 실제 노치·세이프에어리어 스크립트가 붙어있지 않음(HQ·Battle에는 이 이름조차 없음) | 중 |

### 0.4 참조 의존 표 (이름·구조에 기대는 곳 — 바꾸면 깨짐)
(A) 문자열 조회(`GameObject.Find`/`transform.Find`): **0건** — Grep으로 확인, 없음.

(B) 타입 조회(`FindObjectOfType` 등) — 이름은 상관없지만 해당 컴포넌트가 씬에 있어야 함:
| 스크립트:줄 | 대상 타입 | 깨지는 조건 |
|---|---|---|
| `FlowAutoPilot.cs:110` | `TitleScreen` | Title 씬에 TitleScreen 컴포넌트가 없으면 못 찾음 |
| `FlowAutoPilot.cs:142` | `HubScreen` | HQ 씬에 HubScreen 컴포넌트가 없으면 못 찾음 |
| `FlowAutoPilot.cs:177` | `BattleController` | Battle 씬에 BattleController가 없으면 못 찾음 |
| `BattleAutoPilot.cs` | `FindObjectOfType`/`GetComponent` 다수 | Battle 씬 내 관련 컴포넌트(BattleHud, MapHud 등) 타입 유지 필요 |

(C) 직렬화 필드(인스펙터 연결) — 오브젝트를 지우거나 계층을 옮기면 None이 됨:
| 스크립트:줄 | 필드 | 연결된 씬 경로 |
|---|---|---|
| `BattleHud.cs:14-38` | topBar, commandPanel, skillPanel, resultPanel, bossHpRoot, retryButton 등 | `Canvas/BattleHud/TopBar`, `.../CommandPanel`, `.../SkillPanel`, `.../ResultPanel`, `.../BossHpRoot` |
| `HubScreen.cs:15-24` | agentButtons(List), sortieButton, squadButton, squadPanel | `Canvas/AgentButtons/*`, `Canvas/SortieButton`, `Canvas/SquadButton`, `Canvas/SquadPanel` |
| `TitleScreen` | startButton | `Canvas/SafeArea/StartButton` |
| `MapHud.cs` | gridRoot | `Canvas/BattleHud/MapPanel/MazeGrid` |

구조 의존(자식 순서·구조가 바뀌면 깨짐):
| 스크립트:줄 | 의존 방식 | 깨지는 조건 |
|---|---|---|
| `BattleHud.cs:158` | `GetComponentInChildren<Text>()`로 버튼 자식 텍스트를 찾음 | 버튼 자식을 TMP로 바꾸거나 Text 자식을 없애면 깨짐 |
| `MapHud.cs:120` | `gridRoot`의 자식을 `GetChild`로 전부 파괴 | gridRoot 밑에 그리드 셀 외 다른 UI를 두면 함께 파괴됨 |
| `MiniGameRunner.cs:159` | 미니게임 루트 아래 `MiniGameBase`를 `GetComponentInChildren`로 찾음 | 계층 깊이·구성이 바뀌면 못 찾을 수 있음 |
| `BattleHud.FindIcon` (§18) | 스킬 버튼 자식 중 targetGraphic 이 아닌 첫 Image 를 아이콘 칸으로 씀 | 스킬 버튼 아래 `Icon` 보다 앞에 다른 Image 를 두면 거기에 아이콘이 들어감 |

### 0.5 미캡처 (T1.1 갱신)
- T1.1에서 `Unity_Camera_Capture`를 1회 시도했다 — 호출은 성공(1920x1080 PNG, 38666 bytes)했지만 반환된 이미지가 완전한 흰 화면(Scene View 카메라가 현재 열린 씬의 내용을 담지 못함)이라 겹침 판정에 쓸 수 없었다. 재시도하지 않고 실측 수치 판정(§0.3)과 §0.6 사람 확인 목록으로 대체한다.
- **T1.2 재시도**: (A) `Unity_SceneView_Capture2DScene`(월드 좌표 0,0, 1920x1080) — 1회 시도, 결과는 여전히 흰 화면(Scene View 직교 캡처는 Canvas Overlay UI를 담지 못함). (B) `Unity_RunCommand`로 `Camera.main.Render()` 결과를 1920x1080 RenderTexture에 그려 `Docs/Captures/T1_2_HQ.png`로 저장 — **성공**(1,218,582 bytes, 중심 픽셀이 순백이 아닌 실제 렌더 내용 확인). 단, 저장 시점에 에디터에 열려 있던 씬이 HQ가 아니라 Battle이었음(`SceneManager.GetActiveScene().name == "Battle"`로 확인). 파일을 `T1_2_Battle.png`로 이름을 바꾸려 했으나 `File.Move` 호출이 MCP에서 "User interactions are not supported" 오류로 거부됨 — **파일명은 `T1_2_HQ.png`로 남아 있지만 실제 내용은 Battle 씬 캡처**이니 다음 단계에서 참고 시 주의. Title·실제 HQ 씬 캡처는 씬 전환 없이 시도하지 않았다(턴 예산). 다음 단계에서 같은 방식(B)으로 HQ·Title도 이어서 시도할 수 있다.
- **T1.2 Title 캡처**: Title.unity를 열어(dirty 아님 확인) §0.5(B) 방식으로 `Docs/Captures/T1_2_Title.png`를 새로 저장(29,690 bytes, 기존 파일 없어 첫 시도). 단, Title 씬의 Canvas가 `renderMode=ScreenSpaceOverlay`라 `Camera.main.Render()`로는 UI가 찍히지 않고 배경만 담김 — Overlay UI 캡처는 여전히 자동화 불가(§0.6 L83에 기록). `T1_2_HQ.png` 파일명은 손대지 않았다(위 항목 그대로 유지).

### 0.6 겹침 제어 확인과 사람 확인 목록 (T1.1)

(a) 코드 제어 여부

| 항목 | 코드 제어 여부(근거 파일:줄) | 분류 | 해결 단계 |
|---|---|---|---|
| HQ LineText 대 Section | `AgentMenuPanel.cs:65-74` Show()에서 lineText는 매번 text만 설정하고 SetActive(false) 호출이 없어 항상 표시됨. Section만 조건부 SetActive | 레이아웃 문제(항상 겹침) | T7 — `AgentMenuPanel.Show()`에서 lineText 표시 제어(조건부 SetActive) 추가 |
| Battle Feedback·ResultPanel·StartHintText | `MiniGamePhaseHud.cs:109-127` HandlePhaseStarted가 셋 다 SetActive(false)로 초기화, `129-198` ShowFeedback→ShowResult→HideResultAfterDelay가 코루틴으로 순차 표시. 동시에 켜지는 경로 없음 | 문제 아님(순차 표시 규칙, 코드에 주석 없음) | T12(주석 보강) |
| InstructionText 대 TimeBar | 코드상 InstructionText는 기본 비활성(§0.3 30행), 활성화하는 호출부를 찾지 못함(미사용 가능성) | 레이아웃 문제(활성화 시 겹침) | T12 |
| TopBar HpBar 대 EnergyText | `BattleHud.cs:14-38` 필드만 선언, 표시 로직에 상호 배제 없음(둘 다 상시 표시). **T1.2 캡처(`Docs/Captures/T1_2_HQ.png`, 실제 내용은 Battle 씬 — §0.5 참고)로 시각 겹침 실제 확인됨**: HP 텍스트가 노란 에너지 바에, 에너지 텍스트가 파란 HP 바에 겹쳐 보임 | 레이아웃 문제(항상 겹침, 약 19px) — 캡처로 확인 완료 | T10 — `BattleHud` hpBar/energyText 배치 분리(세로 간격 재조정) |

(b) 사람 확인 체크리스트
- [ ] Game View 1920x1080에서 HQ 요원 버튼을 눌러 LineText와 강화·모듈·스토리 섹션 글자가 겹치는지 확인
- [ ] Battle 미니게임 단계를 Play해 피드백→결과 패널→시작 힌트가 동시에 뜨지 않고 순서대로 넘어가는지 확인
- [x] Battle TopBar에서 HpBar 아래와 EnergyText 위의 약 19px 겹침이 보이는지 확인 — **T1.2에서 `Docs/Captures/T1_2_HQ.png`(내용은 Battle 씬)로 확인됨**: HP/에너지 텍스트가 각각 반대쪽 바와 겹침. 해결은 T10.
- [ ] Title SafeArea 밖으로 UI가 잘리는 기기 비율이 있는지 확인 — T1.2 시도: `Docs/Captures/T1_2_Title.png`(29,690 bytes) 저장 성공했으나 Title 씬의 Canvas가 Screen Space - Overlay(`renderMode=ScreenSpaceOverlay`)라 `Camera.main.Render()` 캡처에 UI가 담기지 않음(배경만 캡처됨). SafeArea 잘림은 이 방식으로 확인 불가 — 사람이 Play 모드에서 직접 확인 필요.

### 0.7 (§1 이후 참조)
§1 이후(디자인 원칙, 와이어프레임, 공용 컴포넌트 스펙 등)는 아래 §1~§13에 있다.

## §1 레퍼런스와 차용 포인트 (T2)

| 장르 | 대표작 | 차용 포인트 | 적용 화면 |
|---|---|---|---|
| 턴제 커맨드 RPG 하단 선택지 | 파이널 판타지 시리즈 | 전투 중 화면 하단에 고정된 커맨드 바(공격/스킬/방어/아이템), 스킬 선택 시 하위 목록이 바 위로 펼쳐짐 | Battle 하단 CommandPanel·SkillPanel |
| 덱빌딩 로그라이트 전투 HUD | 슬레이 더 스파이어 | 상단에 HP·에너지·보스 체력바를 나란히 배치, 카드/선택 3택을 중앙에 배치 | Battle 상단 HP·에너지·보스바, Map·UpgradeChoice 3택 |
| 하데스식 허브 | 하데스 | 허브 공간 안에 배치된 인물을 직접 클릭하면 해당 인물의 메뉴가 열림(별도 리스트 UI 없이 공간 자체가 메뉴 입구) | HQ 인물 스폿 클릭 → AgentMenu |
| 모바일 수집형 로비 인물 선택 | 블루 아카이브 등 수집형 로비 | 중앙에 인물 전신 일러스트를 크게, 상단에 재화·스테이지 정보를 얇은 바로 고정 | HQ 중앙 인물 전신, 상단 TopBar 재화·스테이지 |

메이드 인 와리오(초단타 미니게임 지시문 UX)는 §11 MiniGamePhase 비고에서 보조 레퍼런스로 참고한다.

## §2 공용 규칙 (T2)

### 2.1 그리드
- 기본 단위 8px. 거터(요소 간 최소 간격) 24px. 가로 12컬럼(컬럼 폭 = (1920 - 좌우 SafeArea 96) / 12 ≈ 152px).

### 2.2 SafeArea
- 모든 화면 가장자리 여백: 좌우 48px, 상하 32px.
- 모든 HUD·패널 루트는 SafeArea 루트(풀스트레치 컨테이너, 여백만큼 안쪽으로 sizeDelta 축소) 아래에 둔다. T3에서 공용 `SafeAreaFitter` 컴포넌트를 제공하고 각 씬 Canvas 바로 아래 자식으로 배치한다.

### 2.3 터치
- 모든 클릭 가능 요소 최소 크기 100x100px.
- 주 버튼(Sortie, Start, 커맨드 버튼 등) 320x120px.
- 인접 클릭 요소 간 간격 16px 이상.

### 2.4 폰트 크기 단계
| 단계 | 크기(pt) | 용도 |
|---|---|---|
| Display | 72 | 타이틀 로고 보조 문구, Result 제목 |
| H1 | 56 | 화면 제목(Squad·UpgradeChoice 타이틀) |
| H2 | 44 | 패널 제목, 인물 이름 |
| Body | 36 | 대사·설명 본문 |
| Caption | 28 | 보조 라벨(재화 수치, 스테이지 번호) |
| Min | 24 | 최소 허용 크기(버튼 하위 텍스트 등) |

### 2.5 색상 토큰
| 토큰 | Hex | 용도 |
|---|---|---|
| bg.dark | #14141C | 배경 기본 |
| panel.base | #23232E | 패널 배경 |
| panel.border | #3A3A4A | 패널 테두리 |
| text.primary | #F2F2F5 | 본문 텍스트 |
| text.secondary | #A0A0AE | 보조 텍스트 |
| accent.primary | #4E8CFF | 강조(선택·포커스) |
| hp.red | #E5484D | HP 바 |
| energy.blue | #3EC5E8 | 에너지 바 |
| boss.purple | #9B5DE5 | 보스 체력바 |
| success.green | #3DD68C | 성공 연출 |
| fail.red | #D64545 | 실패 연출 |
| disabled.gray | #5A5A66 | 비활성 버튼 |

### 2.6 레이어 순서
| 레이어 | Canvas sortingOrder |
|---|---|
| 배경 | 0 |
| 월드 | 10 |
| HUD | 20 |
| 패널 | 30 |
| 대화 | 40 |
| 페이드 | 50 |

Canvas 목표 설정(모든 씬 공통, T13에서 통일): `renderMode=ScreenSpaceOverlay`, `CanvasScaler.uiScaleMode=ScaleWithScreenSize`, `referenceResolution=(1920,1080)`, `matchWidthOrHeight=0.5`.

## §3 Title (TitleScreen, T4, T2/T6 리뉴얼 반영)

```text
+--------------------------- 1920x1080 --------------------------+
| KeyVisual(배경, 전체)              CharacterLayer(0.72,0) 900x1000 |
|                                                                  |
|                 [ Logo 1000x300, TitleText(내부) ]               |
|                                                                  |
|                    화면을 터치하세요(TouchToStart)                 |
|          StartButton(전체 화면, 투명 raycast)                     |
| [Settings][Notice][Credits] 120x120x3           UID: ----        |
|  BottomMenu(48,32)                                   v1.0.0      |
+------------------------------------------------------------------+ WhiteFade(전체, 최상단, alpha 0)
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| KeyVisual | (0,0)-(1,1) | (0.5,0.5) | (0,0) | 전체 | 배경 0 | Canvas 직속(SafeArea 밖), BG_Title |
| CharacterLayer | (0.72,0) | (0.5,0) | (0,0) | 900x1000 | 배경 0 위 | Canvas 직속, 아트 없으면 비활성 자리표시 |
| Logo | (0.3,0.68) | (0.5,0.5) | (0,0) | 1000x300 | HUD 20 | Canvas/SafeArea, Logo_Title 리본 |
| TitleText | Logo 자식 | (0.5,0.5) | (0,0) | Logo 내부 | HUD 20 | fontSize 46 bold, 리본 위 |
| TouchToStart | (0.5,0.2) | (0.5,0.5) | (0,0) | 900x72 | HUD 20 | Canvas/SafeArea, 깜빡임(1.2s 주기) |
| StartButton | (0,0)-(1,1) | (0.5,0.5) | (0,0) | 전체 | HUD 20 | 필드명 유지, 투명 Image(raycast), Canvas/SafeArea |
| BottomMenu | (0,0) | (0,0) | (48,32) | 버튼 3개×120x120, 간격24 | HUD 20 | Canvas/SafeArea, SettingsButton/NoticeButton/CreditsButton |
| VersionText | (1,0) | (1,0) | (-48,32) | 300x36 | HUD 20 | Canvas/SafeArea, 유지 |
| UidText | (1,0) | (1,0) | (-48,72) | 300x36 | HUD 20 | Canvas/SafeArea, "UID: ----" 고정 표기 |
| WhiteFade | (0,0)-(1,1) | (0.5,0.5) | (0,0) | 전체 | 페이드 50 | Canvas 직속 최상단, alpha 0 |

차용: 수집형 로비의 로고+버튼 배치에 키 비주얼·연출·하단 메뉴를 더한 §27 리뉴얼안(블루 아카이브·쿠키런 킹덤 등 레퍼런스 공통 요소 채택, §27.2).

T6 실측 검증(2026-10-01, `Unity_RunCommand`로 CanvasScaler 공식 재귀 계산, §15.2와 같은 방법): 16:9(1920x1080)·19.5:9(2340x1080)·4:3(1440x1080) 3개 해상도에서 위 표 11개 요소의 절대 사각형을 계산했다. 서로 다른 레이어(배경 0 KeyVisual/CharacterLayer, HUD 20 전경 요소, 페이드 50 WhiteFade, StartButton 투명 전체화면)는 raw rect가 겹쳐도 레이어 순서상 실제로 서로 가리지 않으므로 겹침 판정에서 제외했다. 같은 레이어(HUD 20) 전경 요소끼리는 Logo/TitleText(부모-자식)·BottomMenu/Settings·Notice·CreditsButton(부모-자식) 외 의도치 않은 겹침 **0건**(TouchToStart, BottomMenu, VersionText, UidText, Logo 그룹이 서로 분리됨). CLIP(캔버스 밖으로 나감)은 2건 발견: (1) 19.5:9에서 CharacterLayer 높이(1000)가 캔버스 높이(978)를 넘어 위쪽이 22px 잘림 — 아직 활성 자리표시 상태라 실제 아트(투명 패딩 포함) 적용 시 재조정 필요, 차단 아님. (2) 4:3에서 Logo 왼쪽 경계가 1px 벗어남(xMin=-1) — 반올림 수준의 경미한 침범, 육안으로는 구분 불가. 둘 다 기능을 막는 CLIP이 아니라 이월 메모로만 남긴다(§27.6 Char_Title_KeyArt 수급 시 CharacterLayer 재확인 권장).

→ 연출 시간표는 §27.4, 보류안은 §27.5 참조.

## §4 HQ (HubScreen, T5/T6)

```text
+--------------------------- 1920x1080 --------------------------+
| [Crystal 재화]        stageText(중앙)                           | TopBar h120
|                                                                  |
|     (Spot1)    (Spot2)    (Spot3)    (Spot4)                   | 인물 스폿
|      400x760    400x760    400x760    400x760                  |
|                                                                  |
| [SquadButton 320x120]                    [SortieButton 400x140] |
+------------------------------------------------------------------+
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| TopBar | (0,1)-(1,1) | (0.5,1) | (0,0) | 스트레치x, 120 | HUD 20 | 기존 오브젝트 |
| crystalText | (0,1) | (0,1) | (48,-16) | 300x56 | HUD 20 | TopBar 좌상단 |
| stageText | (0.5,1) | (0.5,1) | (0,-16) | 400x56 | HUD 20 | TopBar 중앙 |
| HubAgentSpot(x N) | (0.5,0) | (0.5,0) | x_i=(i-(N-1)/2)*440, y=184 | 400x760 | 월드 10 | agentButtons 필드가 참조하는 대상을 스폿으로 교체, 필드명 유지, 클릭 영역=전신 스프라이트 Collider/Button 범위. 세로 184~944: SortieButton 위쪽 끝 172, TopBar 아래 가장자리 960 사이 |
| SquadButton | (0,0) | (0,0) | (48,32) | 320x120 | HUD 20 | 기존 필드명 유지 |
| SortieButton | (1,0) | (1,0) | (-48,32) | 400x140 | HUD 20 | 기존 필드명 유지 |

### §4.1 HubAgentSpot 배치값 (T2.1)

| 스폿 | anchoredPosition.x | 화면 x 범위 |
|---|---|---|
| Spot1 | -660 | 100~500 |
| Spot2 | -220 | 540~940 |
| Spot3 | 220 | 980~1380 |
| Spot4 | 660 | 1420~1820 |

y는 모두 184.

계산 근거:
(a) 원래 값인 앵커 y 0.45에 높이 800이면 위쪽 끝이 1286px라 화면을 넘어서 바꿨다.
(b) 가로 간격 40이면 좌우 여백이 100이다.
(c) 하단 버튼 가로 범위(Squad 48~368, Sortie 1472~1872)는 스폿과 겹치지만 세로로 분리돼 있다(버튼 위쪽 끝 172 < 스폿 아래 끝 184). N이 5 이상이면 간격 440을 유지할 때 폭이 1920을 넘으므로 스케일 0.85를 쓰거나 스크롤한다(T5 메모).

차용: 하데스식 허브(공간 속 인물 클릭)로 agentButtons를 HubAgentSpot 클릭으로 대체, 수집형 로비 스타일 상단 재화 바.

## §5 AgentMenu (AgentMenuPanel, T7)

```text
+------------------- 1600x900 (중앙 패널) ----------------------+
| [portrait  ]  nameText   [Upgrade][Module][Story] 탭          |
| [560x800   ]  ------------------------------------------------|
| [          ]                본문 영역(선택 탭 표시)             |
| [lineText  ]                                                   |
| (평소 숨김) ]                                        [Close]   |
+------------------------------------------------------------------+
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| portrait | (0,1) | (0,1) | (40,-40) | 560x800 | 패널 30 | 기존 위치 기준 |
| nameText | (0,1) | (0,1) | (620,-40) | 500x56 | 패널 30 | H2 |
| UpgradeSection/ModuleSection/StorySection | (0,0)-(1,0.62) | (0.5,0) | sizeDelta(-40,-25) | 가변 | 패널 30 | 기존 필드명·앵커 유지 |
| lineText | (0,0.30)-(1,1.00) | - | sizeDelta(-40,-305) | 가변 | 패널 30 | **Show()에서 Section 표시 중에는 SetActive(false)로 숨김(T7 코드 수정, §0.6(a) 겹침 해소)** |
| CloseButton | (1,1) | (1,1) | (-40,-40) | 100x100 | 패널 30 | 최소 터치 크기 충족 |

차용: 수집형 로비의 인물 상세(초상+탭) 패턴.

## §6 Squad (SquadPanel, T7)

```text
+----------------- 1400x800 (중앙 패널) -------------------+
|                     TitleText (H1)                        |
|  [ SquadRow 0                    1320x200 ]               |
|  [ SquadRow 1                    1320x200 ]               |
|  [ SquadRow 2                    1320x200 ]               |
|                                                  [Close]   |
+--------------------------------------------------------------+
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| TitleText | (0.5,1) | (0.5,1) | (0,-32) | 600x56 | 패널 30 | H1 |
| SquadRow_0~2 | VerticalLayoutGroup | - | spacing 24 | 1320x200 | 패널 30 | 기존 이름 유지, sizeDelta 700x500→1400x800로 확장(§0.3 27행 잘림 해소) |
| CloseButton | (1,1) | (1,1) | (-32,-32) | 100x100 | 패널 30 | - |

차용: 덱빌딩류의 편성 리스트 UI(행 3개, 스크롤 없이 고정 표시).

## §7 Dialogue (DialogueBox, T8)

```text
+--------------------------- 1920x1080 --------------------------+
| [portrait 280x280, 패널 밖으로 튀어나옴]                          |
|   +-- [nameText 360x64] --+                                    |
|   | bodyText (Body 36)                                         |
|   |                                        (탭하여 계속)          |
+------------------------------------------------------------------+ h320
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| BoxBg | (0,0)-(1,0) | (0.5,0) | sizeDelta(0,320) | 스트레치x, 320 | 대화 40 | 기존 크기 280→320로 확장 |
| portrait | (0,0) | (0,0) | (48,0) | 280x280 | 대화 40 | 패널 상단 밖으로 걸침(z-order로 위) |
| nameText | (0,1) | (0,1) | (360,-16) | 360x64 | 대화 40 | 명찰 스타일 |
| bodyText | (0,0)-(1,1) | - | sizeDelta(-720,-96) | 가변 | 대화 40 | Body 36 |
| tapButton | (0,0)-(1,1) | - | (0,0) | 스트레치 | 대화 40 | 투명 풀스크린 클릭 영역 |

차용: 턴제 RPG의 하단 대사창(HQ·Battle 공용화 후보, §0.3 28행 참고).

## §8 Battle (BattleHud, T9/T10)

```text
+--------------------------- 1920x1080 --------------------------+
| [HpBar 600x48]              [BossHpRoot 900x56]                 |
| [energyText 아래]                                                |
|                                                                  |
|                    (월드 유닛 영역, UI 없음)                       |
|                                                                  |
| [SkillPanel 600x(항목120), 하단 바 위에 뜸]                        |
+------------------------------------------------------------------+
| [공격][스킬][방어/아이템][턴종료]  각 360x140         CommandPanel h220 |
+------------------------------------------------------------------+
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| TopBar | (0,1)-(1,1) | (0.5,1) | (0,0) | 스트레치x, 140 | HUD 20 | 기존 오브젝트 |
| HpBar | (0,1) | (0,1) | (48,-16) | 440x48 | HUD 20 | 기존 필드명 유지, 폭 축소(600→440)해 BossHpRoot(x 510~1410)와 겹침 해소(T10.1) |
| energyText | (0,1) | (0,1) | (48,-80) | 620x40 | HUD 20 | HpBar 아래로 세로 간격 재조정(-80, 기존 -60에서 이동)해 §0.6(a) 겹침 해소 |
| BossHpRoot | (0.5,1) | (0.5,1) | (0,-16) | 900x56 | HUD 20 | 기존 필드명 유지 |
| CommandPanel | (0,0)-(1,0) | (0.5,0) | sizeDelta(0,220) | 스트레치x, 220 | HUD 20 | 신규, 버튼 4개 가로 배치(각 360x140, 간격 24) |
| SkillPanel | (0.5,0) | (0.5,0) | (0,220) | 600x(120*항목수) | 패널 30 | CommandPanel 바로 위, 뒤로 버튼 포함, 기존 필드명 유지 |
| retryButton/ResultPanel | §12 참고 | - | - | - | 패널 30 | 전투 승패용, MiniGamePhaseHud.result와 이름 구분 유지 |

차용: 파판식 하단 커맨드 바(공격/스킬/방어/턴종료) + 슬레이 더 스파이어식 상단 HP·보스바.

## §9 Map (MapHud, T11)

```text
+--------------------------- 1920x1080 --------------------------+
|                         titleText (상단 중앙)                    |
|  +------------------+          [ Up 160x160 ]                  |
|  |  MazeGrid        |     [Left 160x160][Right 160x160]         |
|  |  1000x800        |          [Down 160x160]                  |
|  +------------------+                                            |
+------------------------------------------------------------------+
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| titleText | (0.5,1) | (0.5,1) | (0,-32) | 500x56 | HUD 20 | H1 |
| gridRoot(MazeGrid) | (0,0.5) | (0,0.5) | (48,0) | 1000x800 | HUD 20 | 기존 필드명 유지, GetChild 파괴 방식 유지(§0.4 구조 의존) |
| 방향 버튼(Up/Down/Left/Right) | (1,0.5) 기준 십자 배치 | (0.5,0.5) | Up(-400,176)/Down(-400,-176)/Left(-576,0)/Right(-224,0) | 각 160x160 | HUD 20 | 최소 터치 크기 100x100 충족, 간격 16px 이상 |

차용: 로그라이트 맵 네비게이션(방향 버튼으로 다음 방 선택).

## §10 UpgradeChoice (UpgradeChoiceHud, T11)

```text
+--------------------------- 1920x1080 --------------------------+
|            [Card0 480x640]  [Card1 480x640]  [Card2 480x640]    |
|             name/desc         name/desc         name/desc       |
+------------------------------------------------------------------+
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| Card_0~2 | (0.5,0.5) | (0.5,0.5) | 중앙 기준 -528/0/+528 (간격 48) | 480x640 | 패널 30 | 각 카드에 name(H2)/desc(Body) |

차용: 덱빌딩 로그라이트의 3택 강화 카드 UI.

## §11 MiniGamePhase (MiniGamePhaseHud, T12)

```text
+--------------------------- 1920x1080 --------------------------+
| [phaseTimeBar 스트레치 h24]                          [energyGauge]|
|                                                          (세로,우측)|
|                     startHintText(중앙)                          |
|                  feedbackText(중앙 상단)                          |
+------------------------------------------------------------------+
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| phaseTimeBar | (0,0)-(1,0) | (0.5,0) | (0,0) | 스트레치x, 24 | HUD 20 | 기존 필드명 유지. **§18에서 하단으로 이동**(와리오식 도화선, 상단 HP 바와 겹침 해소). MiniGameTimeBar 는 그 위 (0,28) h16 |
| energyGauge | (1,0.5) | (0.5,0.5) | (-88,0) | 52x372 | HUD 20 | 세로 게이지(Filled Vertical, energy.blue). 뒤에 EnergyGaugeFrame(EnergyGauge_Bar 90° 회전 400x80) |
| startHintText | (0.5,0.5) | (0.5,0.5) | (0,0) | 800x120 | HUD 20 | 와리오식 짧은 지시문(1~3단어), H1 |
| feedbackText | (0.5,0.65) | (0.5,0.5) | (0,0) | 800x100 | HUD 20 | 결과 직후 짧게 표시, 기존 순차 표시 규칙 유지(§0.6(a)) |

차용: 메이드 인 와리오식 초단타 미니게임의 짧은 지시문 UX(보조 레퍼런스).

### T5 와리오식 리소스

| 요소 | 연결 필드 | 앵커 | pivot | 위치 | 크기 | 레이어 | 에셋ID |
|---|---|---|---|---|---|---|---|
| HintBanner | hintBanner(Battle.unity Canvas/PhaseHudRoot/StartHintText/HintBanner, T5.1에서 생성·연결 완료, sprite=자리표시 UISprite.psd) | (0.5,0.5) | (0.5,0.5) | (0,0) | 960x180 | HUD 20, 텍스트 뒤 sibling 0 | HUD_HintBanner(이월: 실제 스프라이트 생성) |
| FuseSpark | timeBarFuseIcon(Battle.unity Canvas/PhaseHudRoot/FuseSpark, T5.1에서 생성·연결 완료, sprite=자리표시) | (ratio,0.5)-(ratio,0.5) | (0.5,0.5) | (0,0) | 64x64 | HUD 20, phaseTimeBar 위 | HUD_FuseSpark(이월: 실제 스프라이트 생성) |
| FuseBomb | (필드 없음, Battle.unity Canvas/PhaseHudRoot/FuseBomb, T5.1에서 생성 완료·고정 장식, sprite=자리표시) | (1,0.5) | (0.5,0.5) | (0,0) | 72x72 | HUD 20 | HUD_FuseBomb(이월: 실제 스프라이트 생성) |
| SuccessStamp | successStamp(Battle.unity Canvas/PhaseHudRoot/SuccessStamp, feedbackText 형제, T5.1에서 생성·연결 완료, sprite=자리표시, 기본 비활성) | (0.5,0.5) | (0.5,0.5) | (0,0) | 360x360 | HUD 20 | HUD_Stamp_Success(이월: 실제 스프라이트 생성) |
| FailStamp | failStamp(Battle.unity Canvas/PhaseHudRoot/FailStamp, feedbackText 형제, T5.1에서 생성·연결 완료, sprite=자리표시, 기본 비활성) | (0.5,0.5) | (0.5,0.5) | (0,0) | 360x360 | HUD 20 | HUD_Stamp_Fail(이월: 실제 스프라이트 생성) |
| EnergyPopup | energyPopupText(Battle.unity Canvas/PhaseHudRoot/EnergyPopup, comboText 형제, T5.1에서 생성·연결 완료, 기본 비활성) | (0.5,0.5) | (0.5,0.5) | (0,40) | 200x60 | HUD 20 | HUD_EnergyPopupBg(선택, 이월) |

순차 표시 규칙(§0.6(a)) 보강: 도장(SuccessStamp/FailStamp)은 feedbackText 와 동시에 표시·소멸한다(ShowFeedback 루프 공유). EnergyPopup 은 Energy 증가 시 독립적으로 표시되며 위 표의 다른 요소와 표시 시점이 겹쳐도 서로 숨기지 않는다.

## §12 Result (ResultPanel·MiniGamePhaseHud.result, T12)

```text
+------------------- 1200x700 (중앙 패널, 페이드 위) -------------+
|                      제목 (Display 72)                         |
|                      보상 목록                                  |
|                  [   확인 버튼 400x120   ]                      |
+------------------------------------------------------------------+
```

| 요소 | 앵커 | pivot | 위치(anchoredPosition) | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|
| Panel | (0.5,0.5) | (0.5,0.5) | (0,0) | 1200x700 | 대화 40(전투 승패 ResultPanel), HUD 20(미니게임 단계 result) | 두 ResultPanel 이름은 §0.3 32행처럼 유지, 부모 경로로 구분 |
| TitleText | (0.5,1) | (0.5,1) | (0,-48) | 800x100 | 동일 | Display 72 |
| 보상 목록 | (0.5,0.5) | (0.5,0.5) | (0,0) | 900x300 | 동일 | VerticalLayoutGroup |
| ConfirmButton | (0.5,0) | (0.5,0) | (0,48) | 400x120 | 동일 | - |

차용: 로그라이트류 결과 화면(제목+보상 목록+확인 버튼 단일 패널).

**T5 추가**: 미니게임 단계 result 배경 = resultPanel의 Image.sprite ← HUD_ResultPanel(1200x700, 9-slice, 필드 추가 없음, 기존 resultPanel 오브젝트 재사용). 실측 차이: 위 표의 '보상 목록(VerticalLayoutGroup)'은 미니게임 단계 result에는 실제로 없다. 현재는 resultText(Text) 한 개가 성공/실패 수·에너지·최대콤보를 한 문단으로 보여준다. 전투 승패 ResultPanel(대화 40 레이어)은 이번 T5 대상이 아니다.

## §13 UI 에셋 목록 (T2)

| 에셋 | 용도 | 크기 | 상태 |
|---|---|---|---|
| 패널 9-slice | 모든 패널 배경(AgentMenu, Squad, UpgradeChoice, Result) | 가변(9-slice) | 있음(§18) — UI_ResultPanel(크림, 어두운 패널은 UI_PanelDark 신설(T3)) |
| 버튼-주 | Sortie, StartButton, ConfirmButton 등 | 320~480 x 120~140 | 있음(§18) — UI_StartButton/UI_SortieButton(크림 9-slice) |
| 버튼-보조 | Close, 방향 버튼, 탭 버튼 | 100~160px | 있음(§18) — UI_SquadButton(크림 9-slice, 방향 버튼은 코드 틴트) |
| 버튼-비활성 | 사용 불가 상태(disabled.gray) | 위와 동일 | UI_ButtonDisabled/UI_PanelDark 신설(T3) — 기존 disabledColor 틴트는 대체안으로 유지 |
| HP 바 | Battle TopBar HpBar | 440x48 | 있음(§18, T1.1) — UI_HpBar 프레임(HpBarBg) + UI_White 채움(hp.red 틴트) |
| 보스 체력바 | BossHpRoot | 900x84 | 있음(§18, T1.1) — UI_HpBar 프레임(BossHpBg) + UI_White 채움 |
| 에너지 바/게이지 | energyText 옆, MiniGamePhase energyGauge | 620x40 / 64x400 | 있음(§18) — EnergyGauge_Bar 세로 회전 프레임 + UI_White 채움 |
| 인물 전신 스프라이트 | HQ HubAgentSpot | 400x700 클릭 영역, 몸 700x700 | 있음(Agent_*_Full) |
| 초상(포트레이트) | AgentMenu portrait, Dialogue portrait | 560x800 / 340x340 | 있음 |
| 아이콘-재화 | crystalText 옆 | 64x64 | 자리표시 유지(T4.1) — 대상 HQ.unity Canvas/SafeArea/TopBar/CrystalIcon(기존 오브젝트, sprite=Usage_Energy 임시 유지). 실제 Icon_Crystal 스프라이트 생성·교체는 미완료(에셋 생성 도구 동의 필요, T5 이후 이월) |
| 아이콘-스테이지 | stageText 옆 | 48x48 | 제외(T4, `HubScreen.cs`에 대응 필드·오브젝트 없음, 문구만으로 충분) |
| 아이콘-맵 방향 | Battle MapPanel/DirButton_Up~Right/Arrow | 64x64 | 완료(T4.1) — 각 DirButton 아래 Arrow 자식 생성, RectTransform.localEulerAngles.z = 0/180/90/-90(Up/Down/Left/Right)로 4방향 적용. sprite는 자리표시(UI/Skin/UISprite.psd), 실제 Maze_DirArrow 그림 교체는 이월 |
| 아이콘-맵 방 유형 | Battle MapPanel/MazeGrid/Cell_x_y/Icon(런타임 생성) | 셀의 약 60% | 완료(T4.1, 코드) — `MapHud.cs`에 startIcon/combatIcon/eliteIcon/restIcon/bossIcon Sprite 필드 추가, Cell_x_y 하위 Icon 자식과 IconFor()로 연결됨(필드가 비면 기존 LetterFor 글자로 자동 대체). 5종 실제 스프라이트 생성·필드 연결은 이월 |
| 아이콘-스킬 | SkillPanel 항목 | 84x84 | 있음(§18) — SkillDefinition.icon 을 BattleHud.SetSkills 로 표시 |
| 아이콘-강화/영구강화/모듈 | UpgradeChoice 카드, AgentMenu 행 | 180 / 72 | 있음(§18) — 각 Definition.icon 필드 신설 |
| 흰 1색 스프라이트 | Filled 게이지(타임바·에너지 게이지) | 8x8 | 있음(§18) — UI_White.png |
| 로고 | Title Logo | 1200x360 | 있음(빈 리본) + TitleText 를 리본 위에 얹음(§18) |
| UI_TopBar | TopBar 프레임(가로로 늘려 씀). 씬 경로: HQ.unity Canvas/SafeArea/TopBar(h120), Battle.unity Canvas/BattleHud/TopBar(h140) | 원본 512x128, 표시 가변(h120~140), 9-slice 48,48,24,24 | 없음(§2-o 프롬프트, T3) — 배치 Assets/SampleImage/UI/ |
| UI_CloseButton | 닫기(X) 버튼. 씬 경로: HQ.unity Canvas/SafeArea/AgentMenuPanel/CloseButton, Canvas/SafeArea/SquadPanel/SquadCloseButton | 원본 256x256, 표시 100x100, Simple(9-slice 0) | 없음(§2-o 프롬프트, T3) — 배치 Assets/SampleImage/UI/ |
| UI_TabButton_On | AgentMenu 탭 버튼(선택). 씬 경로: HQ.unity Canvas/SafeArea/AgentMenuPanel (신규 자식, 미생성 — 실측 결과 UpgradeSection/ModuleSection/StorySection이 탭 버튼 없이 직접 배치돼 있음, 탭 버튼 3개는 새로 만들어야 함) | 원본 512x192, 표시 240x96, 9-slice 48,48,48,24 | 없음(§2-o 프롬프트, T3) — 배치 Assets/SampleImage/UI/ |
| UI_TabButton_Off | AgentMenu 탭 버튼(비선택). 씬 경로: 위와 동일(신규 자식, 미생성) | 원본 512x192, 표시 240x96, 9-slice 48,48,48,24 | 없음(§2-o 프롬프트, T3) — 배치 Assets/SampleImage/UI/ |
| UI_CardFrame | 강화 카드 전용 프레임. 씬 경로: UpgradeChoiceHud/Card_0~2 (신규 자식, 미생성 — 빌드 씬 전체(EditorBuildSettings.scenes)를 조회했으나 UpgradeChoiceHud·Card_ 오브젝트를 찾지 못함, 런타임 동적 생성으로 추정) | 원본 512x683, 표시 480x640, 9-slice 64,64,64,64 | 없음(§2-o 프롬프트, T3) — 배치 Assets/SampleImage/UI/ |
| UI_PanelDark | 어두운 패널 변형(AgentMenu·Squad 배경). 씬 경로: HQ.unity Canvas/SafeArea/AgentMenuPanel, Canvas/SafeArea/SquadPanel | 원본 512x512, 표시 가변, 9-slice 64,64,64,64 | 없음(§2-o 프롬프트, T3) — 배치 Assets/SampleImage/UI/ |
| UI_NameTag | 이름표 태그(DialogueBox 화자명 배경). 씬 경로: HQ.unity/Battle.unity Canvas/DialogueBox (신규 자식, 미생성 — 실측 결과 DialogueBox 하위에 BoxBg/Portrait/TapButton만 있고 nameText가 없음, 이름표 배경은 새 자식으로 추가 필요) | 원본 384x96, 표시 360x64, 9-slice 40,40,24,24 | 없음(§2-o 프롬프트, T3) — 배치 Assets/SampleImage/UI/ |
| UI_TapNext | '탭하여 계속' 표시. 씬 경로: HQ.unity/Battle.unity Canvas/DialogueBox/TapButton(기존 오브젝트, 실측 확인됨) | 원본 128x128, 표시 48x48, Simple(9-slice 0) | 없음(§2-o 프롬프트, T3) — 배치 Assets/SampleImage/UI/ |
| UI_ButtonDisabled | 비활성 버튼 상태 스프라이트. 대상: Title.unity Canvas/SafeArea/StartButton, HQ.unity Canvas/SortieButton, ConfirmButton 등의 SpriteState.disabledSprite | 원본 512x256(UI_StartButton과 동일 비율), 9-slice 56,56,56,56(UI_StartButton 실측값과 동일) | 없음(§2-o 프롬프트, T3) — 배치 Assets/SampleImage/UI/ |
| 폰트(한글 지원) | 전체 텍스트 | - | 확인 필요(현재 폰트가 한글을 지원하는지 T3에서 점검) |

## §14 T2 완료 확인 (T2.2)

### 14.1 설계 문서 완료 기준 재확인
- §1(레퍼런스) 1개, §2(공용 규칙) 1개, §3~§12(씬·패널별 요소 표) 10개, §13(에셋 목록) 1개 — 모두 존재(T2.1에서 확인, 본 단계에서 재검증).
- §8 energyText는 오프셋 `(48,-80)`로 이미 확정값이 문서에 있고(§8 표), §4 HubAgentSpot도 좌표 공식·배치표(§4.1)까지 확정됐다. 즉 **설계는 완료**됐고, 남은 것은 씬/코드 반영(구현)뿐이다.

### 14.2 이월 항목 — 의도적 지연(블로커 아님)

| 항목 | 현재 상태(문서) | 반영 위치 | 담당 단계 | 사유 |
|---|---|---|---|---|
| §8 energyText 실제 좌표, `BattleHud.cs` 반영 | 설계값 확정(§8: (48,-80), 620x40) | `Assets/SampleScript/*/BattleHud.cs`, Battle 씬 | T10 | 전투 씬 레이아웃 재설계 단계에서 코드+씬을 한 번에 바꿔야 참조가 안 깨짐(계획대로) |
| HubAgentSpot 실제 씬 배치(오브젝트 생성·좌표 적용) | 설계값 확정(§4.1: N=4 예시 -660/-220/220/660, 공식 `x_i=(i-(N-1)/2)*440`) | HQ 씬(HubScreen), `agentButtons` 참조부 | T5(데이터·스크립트) → T6(씬 배치) | 데이터·스크립트 선행 후 MCP로 씬 배치(계획대로) |
| N≥5일 때 스케일 0.85/스크롤 적용 | 메모만 있음(§4.1 (c)) — 현재 기획 인원 N=4 기준으로는 해당 없음 | HQ 씬, 스폰 로직 | T5 | N=4로 확정되면 불필요, N≥5 요구가 실제로 생길 때 T5에서 재검토(선반영 시 과설계) |

### 14.3 T2 계열 종료
- T2 → T2.1 → T2.2로 §1~§13 설계와 위 3개 이월 항목의 처리 계획까지 문서에 반영 완료. 이월 항목은 설계 미비가 아니라 **의존 순서상 후속 단계(T5/T6/T10)에서 코드·씬과 함께 반영해야 하는 구현 작업**이므로 T2 문서 작성 범위의 완료를 막지 않는다.
- 다음 단계(T3 공용 UI 기반)로 진행 가능.

## §15 T13 화면비·SafeArea 검수

### 15.1 Canvas 설정 통일
§0.2·§2.6에서 지적된 대로 세 씬의 Canvas 설정이 서로 달랐다(HQ `matchWidthOrHeight=0`, Battle `renderMode=ScreenSpaceCamera`). Unity MCP `Unity_RunCommand`로 세 씬을 열어 확인 후 통일하고 저장했다.

| 씬 | 변경 전 | 변경 후 |
|---|---|---|
| Title | Overlay / ScaleWithScreenSize / 1920x1080 / match 0.5 | (이미 일치, 변경 없음) |
| HQ | Overlay / ScaleWithScreenSize / 1920x1080 / **match 0** | Overlay / ScaleWithScreenSize / 1920x1080 / **match 0.5** |
| Battle | **ScreenSpaceCamera** / ScaleWithScreenSize / 1920x1080 / match 0.5 | **ScreenSpaceOverlay** / ScaleWithScreenSize / 1920x1080 / match 0.5 |

이제 세 씬 모두 §2.6 목표 설정과 동일하다.

### 15.2 검수 방법(실측, 미캡처)
- `Unity_RunCommand`로 CanvasScaler 공식(`scaleFactor = 2^lerp(log2(W/refW), log2(H/refH), match)`)을 그대로 구현해 16:9(1920x1080)·19.5:9(2340x1080)·4:3(1440x1080) 3개 해상도에서 각 씬의 활성(`activeInHierarchy`) `Graphic` 오브젝트의 절대 anchor 사각형을 재귀 계산했다(부모 RectTransform을 따라 앵커·오프셋을 누적, Canvas 루트는 `canvasRect`로 치환).
- 판정: 캔버스 영역(0~canvasW, 0~canvasH)을 벗어나면 `CLIP`(잘림), 좌우 48px·상하 32px 세이프 마진(§2.2)을 넘으면 `UNSAFE`(여유 침범, 잘림은 아님)로 표시.
- `Camera_Capture`는 Edit 모드에서 흰 화면만 반환해(기존 MCP 한계, T10부터 누적) 실제 스크린샷 비교는 이번에도 하지 못했다. 수치 계산으로 대체했다.
- 주의: `HorizontalLayoutGroup`이 붙은 오브젝트(Battle `CommandPanel`)는 씬을 막 연 직후 레이아웃 리빌드 전 값(저장된 값)을 읽으면 자식 앵커가 `(0,0)`으로 나와 오탐(가짜 CLIP)이 뜬다. 리빌드 후 실제 값은 `anchorMin/Max=(0,1)`, 자식이 패널 내부 y 55~165(패널 범위 0~220)에 위치해 잘리지 않는다. 확인 시 `LayoutRebuilder.ForceRebuildLayoutImmediate` 또는 재조회로 리빌드된 값을 봐야 한다.

### 15.3 결과
- **Title**: 16:9·19.5:9·4:3 모두 `CLIP`/`UNSAFE` **0건**. `Logo`(상단 고정, anchor y=1)·`VersionText`(우하단 고정, anchor(1,0))가 모두 올바른 모서리 앵커라 화면비가 바뀌어도 캔버스 안에 남는다.
- **HQ**: 3개 화면비 모두 동일하게 `TopBar`·`CrystalText`·`SortieStageText`가 세이프 마진(상단 32px)을 넘는 `UNSAFE`만 나옴(잘림 `CLIP`은 0건). TopBar가 화면 최상단에 딱 붙는 상태바 형태로 의도된 배치라 세 화면비에서 **동일하게** 나타나며, 화면비 때문에 새로 생기는 문제가 아니다(§0.3에서 이미 인지된 패턴). 텍스트가 실제로 잘리진 않는다.
- **Battle**: 3개 화면비 모두 `HpBar`류·`EndTurnButton`·`PhaseTimeBar`/`MiniGameTimeBar`가 동일하게 `UNSAFE`(상단·우측 세이프 마진 근접)만 나옴, `CLIP` 0건(CommandPanel 버튼은 §15.2 주의사항대로 오탐 확인 후 제외). 세 화면비에서 **일관되게** 같은 패턴이라 화면비 전환으로 새로 생기는 잘림·겹침은 없다.
- **결론**: 16:9·19.5:9·4:3 세 화면비에서 버튼 **잘림(CLIP) 0건**. 가장자리 세이프 마진(48/32px) 침범은 세 화면비 모두 이미 §0.3에 기록된 기존 상단바류 배치 때문이며 화면비가 바뀌어도 악화되지 않는다(상수 패턴).

### 15.4 남은 참고 사항(블로커 아님)
- HQ·Battle 상단바류를 세이프 마진 안쪽으로 더 넣을지는 디자인 선택(상태바를 화면 끝까지 채우는 흔한 패턴)이라 이번 검수에서 강제로 고치지 않았다. 필요하면 별도 작업으로 상단바 안쪽 패딩을 늘린다.
- `Camera_Capture` Edit 모드 한계는 여전히 해소되지 않음(기존 이슈, T10부터 누적 기록).

## §16 최종 스크린샷 기준과 에셋 목록 확정 (T15)

### 16.1 최종 스크린샷 기준
`Camera_Capture` 가 Edit 모드에서 흰 화면만 반환하는 MCP 한계(§15.2, T10부터 누적)로 실제 스크린샷 비교는 여전히 자동화하지 못한다. 사람이 Device Simulator 또는 Game 뷰에서 직접 캡처해 아래 기준으로 판정한다.

| 항목 | 기준 |
|---|---|
| 해상도 | 16:9(1920x1080), 19.5:9(2340x1080, 노치 포함), 4:3(1440x1080) 3종 |
| 씬 | Title, HQ(HubScreen), Battle(BattleHud) |
| 판정 1 (잘림) | §15.2 정의의 `CLIP`(캔버스 밖으로 나감) — 0건이어야 통과. §15.3 실측 기준 이미 0건 |
| 판정 2 (겹침) | 인접한 두 `Graphic`의 절대 사각형이 실제로 겹쳐 텍스트·버튼을 가리는가(세이프 마진 침범만으로는 실패 아님, §15.3 UNSAFE는 상단바 상시 패턴으로 통과 처리) |
| 판정 3 (신규 요소) | §4 HubAgentSpot 4개, §8 CommandPanel 4버튼 + SkillPanel이 화면비 3종 모두에서 동일한 상대 위치로 보이는가 |
| 저장 위치 | `Temp/AspectCheck/{16_9,19_5_9,4_3}.png`(프로젝트 밖, Assets 미포함, 기존 T1.2 관례 유지) |
| 통과 기준 | 판정 1~3 모두 통과해야 "최종"으로 표시. 하나라도 실패하면 §15.4처럼 남은 참고 사항에 기록하고 디자인 선택 여부를 사람이 정한다 |

### 16.2 필요 에셋 목록 확정
§13 표가 **최종 목록**이다(T2에서 초안 작성, T3~T13 구현 과정에서 추가 항목 없음 재확인). 자리표시(Square/Circle Sprite)로 남아 있는 항목은 실제 제작 전까지 그대로 사용한다:
- 자리표시 유지: 패널 9-slice, 버튼-주/보조/비활성, 에너지 바/게이지, 인물 전신 스프라이트(HQ HubAgentSpot), 아이콘 3종(재화/스테이지/스킬).
- 기존 재사용(제작 불필요): HP 바, 보스 체력바, 초상(포트레이트), 로고.
- 확인 필요(제작 아님): 폰트 한글 지원 — T3에서 점검 완료로 인계됨(기존 폰트 유지).
새 에셋이 필요해지면 이 목록에 행을 추가하고 "상태" 열을 갱신한다(§13 표 형식 유지, 별도 목록 만들지 않음).

## §17 사람 확인 체크리스트와 임시 도형 목록 (T16)

### 17.1 사람 확인 체크리스트
`Camera_Capture` Edit 모드 한계(§15.2/§16.1, T10부터 누적)로 아래는 전부 사람이 Play 모드 또는 Device Simulator에서 직접 확인해야 한다. TEST_SCENARIO.md §2/§5/§O/§P에 흩어진 "사람 확인 필요" 항목을 씬·주제별로 한 목록에 묶었다(원본 항목은 그대로 두고 참조만 추가, 중복 삭제하지 않음).

| # | 항목 | 확인 방법 | 근거(원본) |
|---|---|---|---|
| 1 | 본부(HQ) 인물 탭 감각 | HubScreen Play → `HubAgentSpot` 4개를 각각 클릭해 `AgentMenuPanel`이 즉시 열리고 선택 인물이 맞는지, 탭 반응이 끊기지 않는지 확인 | TEST_SCENARIO.md §O, §3 O행 |
| 2 | 전투 하단 선택지 엄지 영역 | BattleHud Play(또는 Device Simulator 세로 그립) → 화면 하단 `CommandPanel` 4버튼과 `SkillPanel` 항목이 엄지로 누르기 편한 위치·크기인지, 오탭 없이 스킬→대상→발동까지 이어지는지 확인 | TEST_SCENARIO.md §P, §3 P행 |
| 3 | 대화창 가독성 | Dialogue 패널 Play → 포트레이트·이름표·본문 텍스트가 겹치지 않고, 긴 대사에서 줄바꿈이 잘리지 않는지, 한글 폰트가 깨지지 않는지 확인 | UI_DESIGN.md §8(대화창 재설계), §13 폰트 확인 필요 항목 |
| 4 | 화면비별 모습(16:9 / 19.5:9 / 4:3) | Title·HQ(HubScreen)·Battle(BattleHud) 3씬을 Device Simulator에서 §16.1 표 기준(해상도 3종, CLIP 0건, 겹침 없음, 신규 요소 동일 위치)으로 캡처·비교 | UI_DESIGN.md §16.1 |
| 5 | 임시 도형 자리의 교체 대상 에셋 | 아래 §17.2 목록의 씬을 Play/Scene 뷰로 열어 자리표시 도형(Square/Circle Sprite)이 실제로 남아 있는지, 목록과 어긋나지 않는지 육안 대조 | §13(에셋 목록), §16.2, 본 §17.2 |
| 6 | 미니게임 단계 HUD·결과 화면 | 임의의 MiniGame 씬 Play → 진행 게이지, 결과(Success/Failed) 화면 레이아웃이 다른 씬과 톤이 맞는지 확인 | UI_DESIGN.md §12, TEST_SCENARIO.md §5.4(예고 아이콘 육안 확인과 함께) |
| 7 | Play From Scratch 전체 흐름 | `BoolPyeon/Play From Scratch (Auto)` 실행 후 `[Flow] DONE cycles=2` 로그·수동 조작감 확인, 끝나면 `Restore Save Backup` 실행 | TEST_SCENARIO.md §2-G.6/7, §5.1/5.5 |

### 17.2 씬별 임시 도형(자리표시) 목록
§13/§16.2 에셋 목록을 씬 기준으로 재정렬했다. "교체 대상 에셋"은 §13의 "에셋" 열과 동일한 이름을 쓴다.

| 씬 | 임시 도형 오브젝트 | 자리표시 스프라이트 | 교체 대상 에셋 |
|---|---|---|---|
| Title | Logo(비활성 상태만 해제 필요, 도형 아님) | - | 로고(있음, 자리표시 아님 — 참고용으로만 기재) |
| HQ(HubScreen) | `AgentMenuPanel` 배경(비활성 포함 전체 20개 Image 재스캔, T1.1) | 교체됨(§18.2, UI_ResultPanel/UI_SquadButton/아이콘 전용 스프라이트만 확인, UISprite 0건) | - |
| HQ(HubScreen) | `UpgradeChoicePanel`/`ResultPanel` | 씬에 해당 이름의 오브젝트 없음(T1.1 재확인, AgentMenuPanel/UpgradeSection으로 흡수된 것으로 추정) | - |
| HQ(HubScreen) | `SquadPanel`/`SquadRow_0~2` 배경 | UISprite (§19.1 실측 확인) | 패널/행 배경(§13 확인 필요) |
| HQ(HubScreen) | `SquadRow_0~2/SelectedImage` | Knob (§19.1 실측 확인) | 선택 마커 아이콘 |
| HQ(HubScreen) | `TopBar` 배경 | UISprite (§19.1 실측 확인, §18.2 조치는 Title 텍스트 겹침만 해결) | UI_TopBar 패널 배경(§13 확인 필요) |
| HQ(HubScreen) | `AgentSpot_Upgrade/Module/Story` 클릭 영역 | 투명(alpha 0, 정상) — 자리표시 아님 | - |
| HQ(HubScreen) | 위 AgentSpot 3개의 `Highlight` 자식 | Knob (§19.1 실측 확인) | 하이라이트 스프라이트(발밑 타원) |
| HQ(HubScreen) | Sortie/StartButton/ConfirmButton, Close/방향/탭 버튼 | 교체됨(§18.2, §19.1 실측에서 UISprite 미검출) | - |
| HQ(HubScreen) | crystalText/stageText 옆 아이콘 | 정정(T4): crystalText 옆 CrystalIcon은 기존 오브젝트로 실재하나 sprite=Usage_Energy 임시 사용 중(§2-o-1 Icon_Crystal로 교체 예정). stageText 옆 아이콘은 대상 없음(제외) | - |
| Battle(BattleHud) | `CommandPanel`/`SkillPanel` 배경, SkillPanel 항목 아이콘 | 교체됨(§18.2·§18.3, §19.1 실측에서 미검출) | - |
| Battle(BattleHud) | `PhaseTimeBar`/`MiniGameTimeBar`/`EnergyGauge` | 교체됨(§18.2, §19.1 실측 확인: Filled+UI_White로 정상, 자리표시 아님) | - |
| Battle(BattleHud) | `TopBar/HpBar`, `BossHpRoot/BossHpBar` | 교체됨(T1.1, UI_White로 지정·저장 완료) | - |
| Battle(BattleWorld) | 유닛별 `SelectionMarker`(1개), 몬스터별 `IntentIcon`(3개) — 총 7개 | Circle (§19.1 실측 확인) | 선택 마커·예고 아이콘(ART_PROMPTS §0-3 확인 필요, §18.5 4번과 동일 이슈) → §20 설계 |
| 전체 공용 | 사용 불가 상태 버튼(disabled.gray) | 자리표시(색상만 다름, 스프라이트 실측 대상 아님) | 버튼-비활성 |
| SamgakKimbap/FlySwat | `ResultRoot` | Circle (§19.1 실측 확인) | 결과 연출 이펙트(ART_PROMPTS 확인 필요) |
| PeelSticker | `Book/Spine`, `Line_0~4`, `Sticker/PeeledFlap` | UISprite (§19.1 실측 확인, 7개) | PeelSticker 전용 아트 |
| PeelSticker | `Book/Sticker/CornerTab` | Knob (§19.1 실측 확인) | 스티커 모서리 탭 |
| ScreenBubble | `PhoneFrame/Screen` | UISprite (§19.1 실측 확인) | 화면 배경 아트 |
| ButtonShirt | `Shirt/PlacketLeft`, `PlacketRight` | UISprite (§19.1 실측 확인) | 셔츠 플라켓 아트 |
| Vacuum | `Sofa/Body`, `Vacuum/Head/Stick` | UISprite (§19.1 실측 확인) | Vacuum 배경·청소기 아트 |
| Vacuum | `Vacuum/Head/Stick/Handle` | Knob (§19.1 실측 확인) | 청소기 손잡이 |
| BossBlob | `Ground` | UISprite (§19.1 실측 확인) | BossBlob 배경 아트 |
| BossBlob | `ResultRoot` | Knob (§19.1 실측 확인) | 결과 연출 이펙트 |

- HP 바·보스 체력바·초상(포트레이트)·로고는 기존 에셋을 재사용 중이라 임시 도형이 아니다(§13 "있음" 상태, 목록에서 제외). 단 Battle `HpBar`/`BossHpBar`는 §19.1 실측에서 Fill 스프라이트가 UISprite(자리표시)로 확인되어 위 표에 새로 추가했다.
- 위 목록은 §13/§16.2 표와 §19.1 실측을 함께 반영한 것이다. 실제 에셋으로 교체되면 §13 표의 "상태" 열을 먼저 갱신하고 본 표에서 해당 행을 지운다.
- 실측 기준은 §19.1.

## §18 UI 반영 패스 (2026-09-30, 캡처 검수)

T1~T16 문서는 "완료"였지만 실제 씬을 캡처해 보니 어긋난 곳이 많았다. 이번 패스에서 **실측 캡처 → 수정 → 재캡처 → Play 확인** 순서로 다시 맞췄다.

### 18.1 캡처 방법 (Edit 모드 흰 화면 한계 우회)
- `Unity_RunCommand` 에서 씬을 열고 루트 Canvas 를 잠시 `ScreenSpaceCamera`(planeDistance = near+0.05, sortingOrder 1000)로 바꾼 뒤 `Camera.main.Render()` → RenderTexture → PNG. 저장하지 않고 씬을 다시 열어 버린다. 패널은 캡처 직전에만 SetActive.
- Play 모드: `Application.logMessageReceived` 훅을 RunCommand 로 걸어 `[Flow]`·에러를 파일로 남기고(§2-G 9번의 Info 로그 미캡처 한계 우회), `ScreenCapture.CaptureScreenshot` 으로 실제 Overlay 화면을 3~6초마다 저장.

### 18.2 고친 문제 (실측으로 확인된 것)
| 씬 | 문제 | 원인 | 조치 |
|---|---|---|---|
| Battle | **전투 중 주인공·몬스터(및 미니게임 월드 오브젝트)가 화면에 안 보임** | T13 에서 Canvas 를 Overlay 로 통일하면서 풀스크린 `BackGroundImage`(UI) 가 월드 스프라이트를 덮음 | `BackgroundCanvas`(ScreenSpaceCamera, Main Camera, planeDistance 100, sortingOrder -100) 신설 후 BackGroundImage 이동 → §2.6 배경 0 < 월드 10 < HUD 20 순서 회복. Play 캡처로 유닛 표시 확인 |
| Battle | PhaseTimeBar·MiniGameTimeBar·EnergyGauge 가 줄지 않음 | Filled Image 에 sprite 가 없으면 fillAmount 를 무시하고 사각형 전체를 그림 | `UI_White.png` 를 sprite 로 지정(SampleScene 3개 포함) |
| Battle | 페이즈 HUD 가 켜질 때 타임바가 HP 바를 덮음 | 두 HUD 모두 화면 최상단 사용 | 타임바를 화면 하단으로 이동(§11 갱신) |
| Battle | 시작 대기 중 에너지 게이지가 0/10 인데 가득 참 | 씬 기본 fillAmount=1, 코드는 phase 진행 중에만 갱신 | 씬 기본값 0 |
| Battle | 커맨드 버튼 아이콘과 글자가 겹침, 스킬·턴종료·뒤로 버튼이 기본 UISprite | 아이콘 스프라이트를 버튼 배경으로 씀 | 크림 프레임(UI_StartButton) + 왼쪽 `Icon` 자식 + 오른쪽 글자, 360x140 |
| Battle | HpText 가 HP 바와 어긋남, EnergyText 42pt 가 40px 상자에 넘침 | 좌표·크기 불일치 | HP 바 안 가운데 정렬, 에너지 34pt/48px, 외곽선 |
| Battle | 결과 패널 140pt 3줄이 패널 밖으로 넘침 | 코드가 3줄을 한 Text 에 씀 | BestFit 32~64pt |
| Battle | 로그가 주인공 스프라이트를 가림 | 화면 왼쪽 중앙 배치 | 왼쪽 아래(커맨드 바 위) (48,240) LowerLeft |
| Battle | 강화 카드·방향 버튼이 흰 사각형, 페이즈 결과 14pt | 자리표시 그대로 | 카드 UI_AgentButton + 아이콘 180, 방향 UI_SquadButton(코드 틴트 유지), 결과 패널 900x480 + 제목 |
| HQ | 상단바 "Title" 자리표시가 스테이지 문구와 겹침 | UI_TopBar 프리팹의 Center/Title | HQ 인스턴스에서 Title 비활성(오버라이드) |
| HQ | 요원 이름표가 발을 덮고 배경에 묻힘, 요원이 작게 보임 | 전신 스프라이트(정사각)가 400 폭에 맞춰 400px 로 축소 | 클릭 영역 400x700 은 투명 유지, 몸은 `Body` 자식 700x700(HubAgentSpot.body 재연결), 이름표는 크림 태그로 발 아래, 하이라이트는 발밑 타원 |
| HQ | 강화·모듈 행 글자 16~18pt 어두운 회색, 버튼 초록 사각 | 초기 자리표시 | 행 96/150px, 30/28pt 밝은 글자, 아이콘 72, 크림 버튼 |
| HQ | 스쿼드 제목이 안 보임, 행 600x60 | 56pt 글자를 56px 상자 + Truncate(줄이 통째로 잘림) | 제목 800x80, 행 180px 어두운 카드, 특성은 "고점형/물량형" 한글 설명 |
| HQ·Battle | 대화창 이름·본문이 화면 맨 위에 뜸 | DialogueBox 루트가 풀스크린인데 Name/Body 가 루트 기준 앵커 + 씬 인스턴스 RectTransform 오버라이드 | 프리팹에서 Name/Body/Next 를 BoxBg 자식으로, 초상 340 을 박스 위로 걸치게, 씬 인스턴스의 자식 오버라이드 되돌림 |
| Title | 로고가 빈 리본 | Logo_Title 에 글자 없음, TitleText 비활성 | TitleText 를 리본 위(46pt)로 켬 |

T6: ART_PROMPTS §2-j 배경 4종에 UI 빈 영역 지시를 넣고, 레터링은 안2(폰트 얹기)를 권장

T6.1: Unity_RunCommand로 Battle.unity(활성 씬) TopBar/MapPanel/DirButton_*, HQ.unity(Additive 열람) TopBar RectTransform 실측 완료. HQ TopBar=120px(기존 §18 L189 값과 일치, 정확값 재확인). Battle TopBar(HP HUD)=140px(기존 '약180px' 추정을 정정). Battle MapPanel은 top-right 코너(480x360)가 아니라 풀스트레치 컨테이너이며, 실제 배치는 MazeGrid(좌측-중앙, x=48, 1000x800 컨테이너·5x5 grid 실렌더≈556px)와 DirButton 4개(우측 끝, x=-576~-224, y=±176, 160x160)로 나뉨 — ART_PROMPTS.md BG_Battle/BG_HQ 프롬프트·비고를 이 실측값으로 교체함(§2-j-4, §2-j-3)

T6.2: T6.1이 남긴 미해결 이슈 없음을 재확인(마무리 단계). ART_PROMPTS.md §2-j-3/§2-j-4 비고와 위 T6.1 요약이 일치함을 재검토, Unity_GetConsoleLogs로 컴파일 에러 0건(기존 에러 3건은 15:57대 T5.2 에셋 생성 실패 로그로 이번 범위와 무관) 확인. 씬·코드 변경 없음(문서 검토·검증 전용 단계) — T6 전체(T6/T6.1/T6.2) 완료.

## §27 Title 리뉴얼 가이드 (캠페인 T1, 2026-10-01)

### 27.1 현황

MCP `Unity_RunCommand`는 이번 세션에서도 ToolSearch에 잡히지 않아(unity-mcp deferred 도구 미노출) 에디터 실측 대신 **`Assets/Scenes/Title.unity` YAML을 읽기 전용으로 Grep 판독**(2026-10-01)해 아래 값을 확정했다. 편집은 하지 않았다.

| 요소 | 경로 | active | 앵커/pivot | pos | 크기 | 비고 |
|---|---|---|---|---|---|---|
| Canvas | Canvas | - | ScreenSpaceOverlay, ScaleWithScreenSize 1920x1080 | - | - | §0.2 |
| Logo | Canvas/SafeArea/Logo | **true**(활성, §0.3의 false는 과거 상태이거나 오기) | anchor(0.5,1) pivot(0.5,1) | (0,-80) | **1200x360 확정**(§3 계획값과 일치, §0.3의 900x360은 틀림) | fileID 428480778 |
| TitleText | Canvas/SafeArea/TitleText (Logo 자식 아님, SafeArea 직속) | true | anchor(0.5,1) pivot(0.5,0.5) | (0,-206) | 380x72, Text 컴포넌트 fontSize 46 bold | fileID 1830774576, §18.2 "리본 위 46pt" 기술과 일치 |
| StartButton | Canvas/SafeArea/StartButton | true(확정, 추정 아님) | anchor(0.5,0.15) pivot(0.5,0.5) | (0,0) | 480x140 | fileID 1838262531 |
| VersionText | Canvas/SafeArea/VersionText | **이미 존재함**(§3의 "신규" 계획과 달리 생성 완료 상태) | anchor(1,0) pivot(1,0) | (-48,32) | 300x36, Text fontSize 28 | fileID 201611149 |

문제점: (1) 정보량이 Logo+TitleText+StartButton+VersionText뿐으로 적다. (2) Logo는 활성 상태이나 비주얼이 리본 한 장뿐이라 밋밋하다(§18.2에서 지적한 "빈 리본" 문제, TitleText는 이미 켜져 있어 해소됨). (3) 터치 유도·연출·하단 메뉴·BGM이 전혀 없다. (4) §27.3 채택안(TitleText를 Logo 자식으로, Logo 1000x300)과 실측(TitleText는 SafeArea 직속, Logo 1200x360)이 다르다 — T2 변경점 참고.

### 27.2 레퍼런스 공통 요소

| 요소 | 블루 아카이브 | 쿠키런 킹덤 | 메이플스토리M | 원신 | 명일방주 | 채택 |
|---|---|---|---|---|---|---|
| 키 비주얼·캐릭터 | O (배경+캐릭터 합성) | O | O | O (풀샷 캐릭터) | O (실루엣 톤) | 채택 |
| 큰 로고 | O (상단) | O (상단) | O (상단) | O (중상단) | O (상단) | 채택 |
| 화면을 터치하세요 깜빡임 | O | O | O | O | O | 채택 |
| 하단 보조 메뉴(설정·공지·고객센터) | O | O | O | O | O | 채택 |
| 버전·UID 표기 | O(우하단, 작게) | O | O | O | O | 채택 |
| 로고 등장 연출 | O (페이드+스케일) | O | 드묾 | O (화려함) | O (심플) | 채택(단순 팝인) |
| 배경 아이들 모션 | O (캐릭터 호흡·깜빡임) | O | 거의 없음 | O | O (패럴랙스) | 채택(최소 스케일 펄스) |
| BGM | O | O | O | O | O | 채택(훅만, 에셋 보류) |
| 터치 후 화이트 페이드 | O | O(페이드) | O | O | O | 채택 |
| 다운로드 진행바 | O(최초 1회) | O | O | O | O | 보류(§27.5) |
| 계정/UID 실제 발급 | O | O | O | O | O | 보류(§27.5) |

### 27.3 채택안 요소 표 (T2가 이 이름·수치로 배치)

| 요소 | 부모 경로 | 앵커 | pivot | pos | 크기(px) | 레이어 | 비고 |
|---|---|---|---|---|---|---|---|
| KeyVisual | Canvas(직속, 맨 뒤) | (0,0)-(1,1) 풀스트레치 | (0.5,0.5) | (0,0) | 전체 | 배경 0 | BG_Title, SafeArea 밖 |
| CharacterLayer | Canvas(직속, KeyVisual 위) | (0.72,0) | (0.5,0) | (0,0) | 900x1000 | 배경 0 위 | 아트 없으면 비활성 자리표시 |
| Logo | Canvas/SafeArea | (0.3,0.68) | (0.5,0.5) | (0,0) | 1000x300 | HUD 20 | Logo_Title 리본 |
| TitleText | Logo 자식 | (0.5,0.5) | (0.5,0.5) | (0,0) | Logo 내부 | HUD 20 | 리본 위 Display, 글자 포함 레터링으로 대체 가능 |
| TouchToStart | Canvas/SafeArea | (0.5,0.2) | (0.5,0.5) | (0,0) | 900x72 | HUD 20 | "화면을 터치하세요" |
| StartButton | Canvas/SafeArea(TouchToStart 형제, 바로 아래) | (0,0)-(1,1) 풀스트레치 | (0.5,0.5) | (0,0) | 전체 | HUD 20 | 필드명 유지, 투명 Image(raycast용) |
| BottomMenu | Canvas/SafeArea(StartButton보다 위 형제) | (0,0) | (0,0) | (48,32) | 버튼 3개×120x120, 간격 24 | HUD 20 | SettingsButton/NoticeButton/CreditsButton(T4 구현 확정, 가이드 원안의 SupportButton 대신 크레딧 채택), HorizontalLayoutGroup. T4.1 확인: VersionText/UidText 월드 영역(x1572~1872, y32~108)과 겹치지 않음(x48~456, y32~152) |
| VersionText | Canvas/SafeArea | (1,0) | (1,0) | (-48,32) | 300x36 | HUD 20 | 유지 |
| UidText | Canvas/SafeArea | (1,0) | (1,0) | (-48,72) | 300x36 | HUD 20 | Caption, "UID: ----" 자리표시 |
| WhiteFade | Canvas(직속, 최상단) | (0,0)-(1,1) 풀스트레치 | (0.5,0.5) | (0,0) | 전체 | 페이드 50 | 흰색 alpha 0, raycastTarget 기본 off |

T2 변경점(27.1 실측 → 27.3 채택안): Logo 크기 1200x360 → 1000x300, anchor(0.5,1)pos(0,-80) → anchor(0.3,0.68)pos(0,0). TitleText 부모 SafeArea 직속 → Logo 자식, pos(0,-206) → Logo 내부 중앙(0,0). StartButton 전신 투명 풀스트레치 Image로 교체(현재 480x140 중앙 하단 Button → 전체 화면 raycast). VersionText·anchor·pos는 변경 없음(유지).

SafeArea 처리: 기존 SafeArea 컨테이너는 유지하고 그 안에 Logo/TitleText/TouchToStart/StartButton/BottomMenu/VersionText/UidText를 둔다. KeyVisual·CharacterLayer·WhiteFade는 전체 화면이 목적이라 SafeArea 밖(Canvas 직속)에 둔다.

### 27.4 연출 시간 (T3, 코루틴·unscaledTime 기준)

| 구간 | 시간 | 내용 |
|---|---|---|
| 배경 페이드인 | 0~0.4s | KeyVisual alpha 0→1 |
| 로고 팝인 | 0.3~0.75s | scale 0.6→1.08→1.0, alpha 0→1 |
| TouchToStart 등장 | 0.9s | 등장 후 깜빡임 주기 1.2s(alpha 1↔0.25, 사인) |
| 입력 허용 시점 | 0.9s | 그 전에 탭하면 연출 스킵 |
| 배경 아이들 | 주기 8s | KeyVisual scale 1.0↔1.03 |
| 캐릭터 아이들 | 주기 3s | CharacterLayer 상하 ±8px |
| 터치 후 전환 | 버튼 비활성→TouchToStart 빠른 점멸 0.3s→WhiteFade 0→1 0.35s→씬 로드 | 이중 탭 방지 |
| BGM | 진입 페이드인 0.5s, 터치 시 페이드아웃 0.35s | 오디오 에셋 없으면 훅만 둔다 |

### 27.5 보류안 (이유 포함)

- 공지 웹뷰 실연결 — 서버·콘텐츠 없음, NoticeButton은 자리표시 패널만.
- 계정 연동·UID 실제 발급 — 백엔드 없음, UidText는 "UID: ----" 고정 표기로 대체.
- 다운로드 진행바 — 에셋 번들 다운로드 구조가 없는 올인원 빌드라 불필요.
- 캐릭터 Live2D/Spine — 스프라이트 애니메이션 파이프라인 밖, 정적 PNG + 아이들 트윈으로 대체.
- 종료 메뉴 — HANDOFF 원안에 있었으나 레퍼런스 공통 요소가 아니라서 뺀다. 크레딧은 BottomMenu 3번째 버튼(CreditsButton)으로 T4에서 구현됐다 — 본 문서의 "고객센터/SupportButton" 표기는 가이드 원안이고, 실제 채택안은 크레딧이다(§27.3 L620 참고).
- 시즌 테마 배경 — 시즌 이벤트 시스템이 아직 없음.

### 27.6 필요 에셋 (T5가 프롬프트 작성)

| 파일 | 규격 | 위치 | 상태 |
|---|---|---|---|
| BG_Title_v2 | 1920x1080 이상 | Assets/SampleImage/Title/ | 없음, T5 프롬프트 |
| Char_Title_KeyArt | 투명 PNG, 하단 피벗 | Assets/SampleImage/Title/ | 없음, T5 프롬프트 |
| Logo_Title_v2 | 글자 포함 레터링 | Assets/SampleImage/Title/ | 없음, T5 프롬프트 |
| Icon_Settings / Icon_Notice / Icon_Support | 128px | Assets/SampleImage/Title/ | 없음, T5 프롬프트 |
| BGM_Title | 루프 | Assets/SampleImage/Title/ | 없음, T5 프롬프트 |
| SFX_TitleTap | 단발 | Assets/SampleImage/Title/ | 없음, T5 프롬프트 |

**임시 사용 중 (T4.1)**: 전용 아이콘이 없어 BottomMenu 버튼 3개는 기존 스프라이트로 자리만 잡았다. SettingsButton → `Icon_Skill_Comic`, NoticeButton → `Icon_Talk_Comic`, CreditsButton → `Icon_Heart_Comic`(씬 오브젝트 이름은 "크레딧"이며, 위 표의 `Icon_Support`·L648의 "고객센터"와 다르다 — T4 구현이 크레딧으로 확정했다). 교체 방법: 각 버튼의 자식 `Icon` 오브젝트 Image.sprite만 바꾸고 버튼 120x120·RectTransform은 그대로 둔다. 담당: T5.

**로고 리본 품질 (사용자 피드백, 2026-10-01)**: 사용자가 플레이 중 "리본이 짜치다"(Logo_Title 비주얼이 빈약하다)고 지적했다. 이는 §18.2/§27.1에서 이미 "빈 리본" 문제로 기록했고, 위 표의 `Logo_Title_v2`(글자 포함 레터링)로 교체 예정이다. T5에서 이 항목을 최우선으로 다룬다. 이번 단계(T4.1)는 BottomMenu 범위라 로고 아트는 직접 바꾸지 않았다.

### 27.7 §3 교체 방향 (T2에서 실행)

T2에서 §3 표를 위 §27.3 채택안 표로 교체한다. §27.1 실측(2026-10-01 YAML 판독)으로 Logo 크기(1200x360, 활성 상태), TitleText(SafeArea 직속, 380x72, 46pt), VersionText(이미 생성됨, 300x36) 모두 확정됐으므로 T2는 재덤프 없이 §27.3의 목표값(Logo 1000x300, TitleText를 Logo 자식으로 이동 등, §27.3 아래 "T2 변경점" 참고)으로 바로 진행할 수 있다. 그 외 바뀌는 점: StartButton을 풀스트레치(화면 전체 터치 영역)로 전환, 신규 요소 5개(CharacterLayer/TouchToStart/BottomMenu/UidText/WhiteFade) 추가. §0.4의 참조 의존(`TitleScreen.startButton` 경로 `Canvas/SafeArea/StartButton`, `FlowAutoPilot.cs:110`이 `TitleScreen` 타입 조회)은 경로·타입 모두 유지되므로 깨지지 않는다 — StartButton 오브젝트 이름과 TitleScreen 컴포넌트 존재만 지키면 된다.

T7: ART_PROMPTS.md §2-l에서 Egg·Pizza·SamgakKimbap·USB·PeelSticker 5개 미니게임에 §4-1 슬롯 템플릿 기준 누락 행 16개를 추가(BG 5, FX_Success 5, FX_Fail 5, Obj_Egg_Chopstick 1). BG는 모두 신규(씬에 배경 오브젝트 없음), Object 레이어 order -1 권장이나 씬은 미수정. FX_Success/Fail은 기존 스크립트의 ResultAnimator/SpriteRenderer 참조를 그대로 연결(코드 변경 없음), USB_Hand·PeelSticker_Sticker의 기존 실패 프롬프트는 FX_USB_Fail·FX_PeelSticker_Fail 행이 참조하도록 비고만 추가(오브젝트 이름 FailedUSB·TornHalf는 변경 없음). Obj_Egg_Chopstick은 스케치에만 있는 연출이라 씬 배치는 후속 과제로 남김. ID 중복 검사(Glob `BG_*`·`FX_*`·`*Chopstick*`, Grep 기존 문서)에서 겹침 없음 확인. 씬·코드 무변경(문서 전용 단계) — T7 완료.

T7.1: T7이 남긴 미해결 이슈 중 씬·코드로 실제 처리 가능한 것을 마무리했다. (1) USBMiniGame.unity 루트 위치를 Unity_RunCommand로 조회해 (-3.37, 2.04, 274.13)을 확인, UsbMover.cs L42·UsbSetter.cs L15가 위치에 Vector2를 대입해 Z 값에 의존하지 않음을 확인한 뒤 Z만 0으로 고치고(x,y 유지) 씬 저장. (2) §4-4-1 권장에 따라 PizzaMiniGame(1개)·SamgakKimbapMiniGame(9개)·PeelStickerMiniGame(12개)의 Default 레이어 SpriteRenderer/SortingGroup을 Object 레이어로 이전(sortingOrder는 유지)하고 각 씬 저장 — 이 3게임은 Battle 월드와의 겹침 위험이 해소됐다. 나머지 6게임(ScreenBubble·ButtonShirt·Vacuum·FlySwat·BossBlob·TapTarget)은 T8 범위라 이번에 손대지 않음. (3) Obj_Egg_Chopstick과 T5.2 HUD 7종의 실제 이미지 생성은 Unity_AssetGeneration_GenerateAsset 툴 자체가 최초 호출 전 사용자에게 대화형으로 확인을 받고 다음 턴에 진행하도록 요구하는데, 이번은 사람 응답 턴이 없는 자동 릴레이 단일 단계라 그 확인을 받을 수 없어 재시도하지 않았다(모델 미지정으로 인한 실패가 아니라 consent 게이트에서 막힘). 두 항목 모두 이미지 없이는 씬에 배치하지 않았다. Unity_GetConsoleLogs(error)로 확인한 에러 3건은 모두 timestamp 15:57대(T5.2 당시 'A model must be selected')로 이번 세션 변경과 무관, 이번에 만든 새 에러는 0건. 컴파일은 각 RunCommand에서 isCompilationSuccessful=true로 확인 — T7.1 완료(에셋 생성 항목은 다음 세션에 사용자 확인을 받고 이월).

T7.2: T7.1이 남긴 나머지 이슈를 마무리했다. Unity_GetConsoleLogs(error)로 재확인해 신규 에러 0건(기존 3건은 여전히 15:57대 T5.2 로그)을 재확인했다. Obj_Egg_Chopstick은 CLAUDE.md의 "이미지를 새로 만들 수 없으면 기본 도형 스프라이트로 자리를 잡는다" 규칙에 따라, Unity_RunCommand로 EggMiniGame.unity에 런타임 생성 placeholder(단색 베이지 8x64 사각 스프라이트, Object 레이어 order 5, egg 앵커 오브젝트 기준 (+1.2,-0.3))를 배치하고 씬 저장, isCompilationSuccessful=true 확인. HUD 7종(HUD_HintBanner 등)은 Chopstick과 달리 UI 슬롯 형태·비율이 제각각이라 단일 placeholder로 대체 시 레이아웃 왜곡 위험이 커 이번에는 배치하지 않고 이월했다(§4-4-1 HUD 슬롯 실측 참고). GenerateAsset 자체는 최초 호출 전 대화형 확인(플레인텍스트 메시지 전송 후 다음 턴 응답 대기)을 요구하는데, 이 릴레이 단계는 최종 응답이 JSON 한 개로 고정돼 그 대화형 확인을 구조적으로 수행할 수 없어 T7.1과 동일하게 호출하지 않았다(사람 승인 부족이 아니라 도구의 하드 세이프가드) — T7/T7.1/T7.2 전체 완료. 남은 실제 이미지 생성(Obj_Egg_Chopstick 교체 포함 HUD 7종)은 사람이 대화형으로 Unity AI Assistant 패널에서 직접 확인·진행해야 한다.

T8: ART_PROMPTS.md §2-l에서 6개 미니게임(ScreenBubble·ButtonShirt·Vacuum·FlySwat·BossBlob·TapTarget)에 T7과 같은 슬롯 템플릿(BG/FX_Success/FX_Fail)을 적용해 17행을 신규 추가했다(FlySwat은 신규 BG 행 대신 기존 FlySwat_Background 프롬프트를 보강). FlySwat_Swatter를 '제외' 행에서 '[제안] 파리채 커서/휘두름' 행으로 바꾸고 FX_FlySwat_Success/Fail(명중·놓침 연출, MINIGAME_IDEAS §6)을 추가했다. BossBlob_Boss는 §2-b-5 불편대왕 뿔·넥타이 모티프를 더해 보스 톤과 통일했고, BG/FX_BossBlob도 §2-b 몬스터 톤으로 맞췄다. §0-3 '레이아웃 조정 필요' 대응으로 6게임 각 오브젝트 행 비고에 권장 표시 크기(px+u)를 추가했다. 6게임의 Default→Object 레이어 이전은 이번에도 문서 이월(§4-4-1 그대로), 실제 이미지 생성·transform 조정도 이월 — 씬·코드 무변경(문서 전용 단계), T8 완료.

T8.1: T8이 남긴 이슈 4건을 마무리했다. (1) Unity_RunCommand로 6씬(ScreenBubble 11·ButtonShirt 14·Vacuum 9·FlySwat 2·BossBlob 4·TapTarget 1)과 프리팹 3개(Fly·Target·Decoy 각 1)의 Default 레이어 SpriteRenderer/SortingGroup을 Object로 이전하고(sortingOrder 유지) 저장, 재확인 결과 9개 대상 모두 Default 레이어 0개 — Battle 월드와의 겹침 위험을 6게임까지 전부 해소했다. (2) 이미지 생성은 Unity_AssetGeneration_GetModels를 먼저 호출해 `Models: []`(선택 가능 모델 0개)를 확인, T5.2의 'A model must be selected' 오류가 이 환경에 생성 모델이 등록돼 있지 않다는 뜻임을 확정하고 같은 오류를 반복시키지 않기 위해 GenerateAsset 재호출은 하지 않았다 — T8 신규 17행·HUD 7종·Obj_Egg_Chopstick 교체 모두 blocked 유지(사유: 등록된 생성 모델 없음). (3) transform·상자 크기는 콜라이더 스케일 보존 원칙에 따라 실측 조정하지 않고 §0-3에 'transform 조정은 이미지 교체 때 스프라이트 PPU·크기로 맞춘다'는 방침 문구를 추가했다. (4) FlySwat_Swatter 제안 전환과 BossBlob 뿔·넥타이 모티프는 자동 진행 모드로 채택 확정하고 해당 행 비고에 표기했다(파리채 커서 코드 연결은 이월 유지). Unity_GetConsoleLogs(error) 신규 에러 0건(기존 5건은 15:57대 T5.2 로그), RunCommand 2회 모두 isCompilationSuccessful=true — T8.1 완료. 남은 이미지 생성(HUD 7종, Obj_Egg_Chopstick, T8 신규 17행)은 사람이 Unity AI Assistant 패널에서 생성 모델을 등록/선택한 뒤 진행해야 한다.

T8.2: T8.1이 남긴 FlySwat 파리채 커서·휘두름을 코드·씬으로 구현했다. 새 스크립트 `Assets/SampleScript/FlySwatMiniGame/FlySwatSwatter.cs`(Collider2D 없는 연출 전용 MonoBehaviour)가 LateUpdate에서 PointerInput 월드 좌표를 따라가고, 탭(PointerInput.Down && !IsOverUI)마다 z 회전 0→-40°→0(0.12초) 코루틴으로 휘두른다. FlySwatMiniGame.cs·FlyMover.cs는 고치지 않아 기존 HandleHit/HandleMiss 판정이 그대로 유지된다. FlySwatMiniGame.unity에 Manager 자식으로 Swatter(자식 Head, SpriteRenderer=임시 UISprite 초록, scale 0.5x1.4, sortingLayer=Object·order=파리+2=2)를 만들어 붙이고, RunCommand로 game 필드 연결·Collider2D 0개·레이어를 확인한 뒤 저장했다. Unity_AssetGeneration_GetModels를 다시 호출했지만 여전히 `Models: []`라 이미지 생성은 구조적으로 blocked 상태를 유지한다(GenerateAsset 미호출). transform·콜라이더 실측 조정은 이번에도 이미지 교체 시점으로 이월했다(Swatter는 신규 오브젝트라 콜라이더가 애초에 없음). Unity_RunCommand 3회 모두 isCompilationSuccessful=true, Unity_GetConsoleLogs(error) 신규 에러 0건 — T8.2 완료. 사람 확인: FlySwatMiniGame 씬 Play로 파리채 추종·휘두름·판정 체감, Unity AI Assistant 패널에서 모델 등록.

T9: ART_PROMPTS.md §2-m(예정 미니게임 13종)을 §4-1 슬롯 템플릿(BG/Target/FX_Success/FX_Fail)으로 보강했다. MINIGAME_IDEAS §5(1차 제작 순서 L132-147)를 따라 순번6 MG-10 Zipper → 순번7 MG-07 PizzaHalf → 순번8 MG-04 Puzzle → 순번9 MG-20·MG-22·MG-30·MG-41·MG-50 → 순번10 회피 3종(MG-52·MG-51·MG-61) + §4 회피 목록의 MG-23·MG-05 변형 순으로 표 전체를 재배치했다. 13종 각각에 BG 1행 + 기존 Target 행 + FX_Success 1행 + FX_Fail 1행을 채워 13×4=52행(기존 13 + 신규 39)으로 늘렸다. 회피용 5종(MG-52·MG-51·MG-61·MG-23·MG-05 변형)의 Target 프롬프트에는 "filling most of the frame as one bold readable silhouette"를 넣고 비고에 "화면 대부분을 채우는 큰 실루엣으로 한눈에 읽히게(3초 이하 회피 기준)"를 명시했다. MG-04 Puzzle은 "마지막 한 조각" 보강안이 §5에서 여전히 미확정이라 BG·Target·FX_Success·FX_Fail 4행 모두 비고에 `[잠정 표시]`를 달고 실제 생성·배치는 보류로 남겼다. 배치 경로는 모두 CLAUDE.md `Assets/SampleImage/<Name>MiniGame/` 규칙을 따르는 기존 폴더명(ZipperMiniGame·PizzaHalfMiniGame·PuzzleMiniGame 등)을 그대로 재사용했다(신규 폴더 생성 없음). 씬·코드는 손대지 않았다(문서 전용 단계, 미구현 게임이라 씬 자체가 없음) — T9 완료. 이미지 생성은 T8.2까지 확인된 `Models: []`(등록된 생성 모델 없음) blocked 상태가 이어질 것으로 예상해 이번에도 GenerateAsset을 호출하지 않았다.

T9.1: T9이 남긴 MG-04 Puzzle "마지막 한 조각 안 맞음" 보강안을 확정했다. 마지막 빈칸에 정답1+오답2 후보를 동시 제시하고, 오답은 정답과 테두리·인터로킹 형태가 완전히 같으며 얼굴 부위(눈꼬리 각도·홍조 위치)만 미세하게 달라 헷갈리게 한다. 오답 드롭 시 슬롯 밖으로 튕겨나가며 FX_Puzzle_Fail, 정답 드롭 시 스냅 고정되며 FX_Puzzle_Success가 재생되도록 규칙을 MINIGAME_IDEAS.md §5 순번8·§3 MG-04 행에 반영했다. ART_PROMPTS.md §2-m의 BG_Puzzle·MG04_Puzzle·FX_Puzzle_Success·FX_Puzzle_Fail 4행에서 `[잠정 표시]`를 제거하고 "확정" 비고로 갱신했으며, 오답 조각 2종을 위한 `MG04_Puzzle_Decoy` 행을 신규 추가했다(52→53행). 이미지 생성은 T8.2까지 확인된 `Models: []` blocked 상태가 이어질 것으로 예상해 이번 문서 전용 단계에서는 재확인·GenerateAsset 호출을 하지 않았다. 예정 미니게임 13종은 씬·프리팹이 아직 없어 Unity MCP 검증 대상이 없다(정상) — T9.1 완료. 이월: 사람이 Unity AI Assistant 패널에서 생성 모델 등록 후 13종 40행 실제 이미지 생성, 이후 T10 전투 월드 표시 리소스로 진행.

T9.2: T9.1이 남긴 마지막 이슈(13종 40행 이미지 일괄 생성)를 다시 점검했다. Unity_AssetGeneration_GetModels를 재호출했지만 여전히 `Models: []`(등록된 생성 모델 없음)라, T7.2부터 이어진 구조적 blocked 상태가 계속된다 — 사람이 Unity AI Assistant 패널에서 모델을 등록하기 전까지는 GenerateAsset을 호출할 수 없다. 이는 문서·코드로 해결할 수 없는 외부 절차이므로 이번 단계에서는 §2-m 13종 53행(BG·Target·FX_Success·FX_Fail·MG04_Puzzle_Decoy)을 Grep으로 재대조해 `[잠정 표시]` 잔존 0건·ID 중복 0건·MINIGAME_IDEAS §5 순번과 배치 순서 일치를 재확인하는 데 그쳤다. 추가로 고칠 문서 내용은 없었다 — T9(예정 미니게임 프롬프트 재점검) 계열이 문서 기준으로 완료됐다. 사람 확인: Unity AI Assistant 패널에서 생성 모델 등록 후 13종 40행 이미지 일괄 생성, 이후 T10으로 진행.

### 18.3 코드 변경 (UI 표시용, 로직 변경 없음)
- `UpgradeDefinition` / `PermanentUpgradeDefinition` / `ModuleDefinition` 에 `public Sprite icon` 추가, 데이터 15개에 `Assets/SampleImage/UI/Icons/<같은 이름>.png` 연결.
- `PermanentUpgradeRow.iconImage`, `ModuleRow.iconImage`, `UpgradeChoiceHud.icons` — 아이콘이 없으면 Image 를 끈다.
- `BattleHud.SetSkills(labels, usable, icons)` 오버로드(기존 2인자 호출은 그대로 동작). 아이콘 칸 = 버튼 자식 중 targetGraphic 이 아닌 첫 Image (**구조 의존 추가**, §0.4 표 참고: 스킬 버튼 아래에 다른 Image 를 먼저 두면 그 Image 에 아이콘이 들어간다). `BattleController.SetSkills` 가 `SkillDefinition.icon` 을 넘긴다. 동적 생성용 `UI_Button.prefab` 에도 `Icon` 자식 추가.
- `SquadRow` 특성 표시를 enum 이름 대신 `SquadEnergyRule` 규칙 요약으로.

### 18.4 검증
- 콘솔 컴파일 에러 0, EditMode 테스트 **151/151 PASS**.
- `Play From Scratch (Auto)` 2회: Error/Exception 0. 두 번째 실행에서 전투 유닛 표시, HP/보스/로그/결과 패널 표시 확인(캡처). 세이브는 매번 `Restore Save Backup` 으로 되돌렸다.
- 자동 진행 한계(UI 문제 아님, 별도 작업 필요): ① 튜토리얼 대사를 넘기는 자동 입력이 없어 FlowAutoPilot 이 첫 대사에서 `STUCK` — 이번에는 RunCommand 로 TapButton 을 눌러 우회. ② BattleAutoPilot 이 튜토리얼 전투에서 매번 패배해 `Battle retry/continue` 가 반복되고 HQ 로 넘어가지 않는다(에너지 0 상태로 공격만 반복).

### 18.5 사람 확인 (§17.1 에 추가)
- [ ] Battle 에서 월드 유닛이 배경 위·HUD 아래에 보이는지(Game 뷰 16:9/19.5:9/4:3). BackgroundCanvas 가 카메라 모드라 CameraFit 이 카메라 크기를 바꿔도 배경이 화면을 채우는지.
- [ ] 미니게임 페이즈에서 하단 타임바 2줄이 실제로 줄어드는지, 미니게임 오브젝트가 배틀 배경 위에 보이는지.
- [ ] HQ 요원 700px 전신 크기가 적당한지(크면 `Body` 크기만 줄이면 된다), 이름표 탭도 메뉴를 여는지.
- [ ] 월드 라벨(유닛 이름·HP 글자)과 예고 아이콘(빨간 원)이 머리 위에서 겹친다 — BattleUnitView 월드 표시라 이번 UI 패스 범위 밖, 예고 아이콘 스프라이트 연결(ART_PROMPTS §0-3)과 함께 정리 필요. (§20)

## §19 스프라이트 실측 전수조사 (T1 캠페인, 2026-09-30)

### 19.0 조사 방법과 판정 기준
- `Unity_RunCommand` 로 Title·HQ·Battle 3씬과 `Assets/Scenes/MiniGames/*MiniGame.unity`(Template 제외) 11씬을 `EditorSceneManager.OpenScene(Additive)` 로 열어 비활성 오브젝트를 포함한 모든 `Image`·`SpriteRenderer` 를 `GetComponentsInChildren(true)` 로 훑고, 저장 없이 `CloseScene` 으로 닫았다. `Assets/Prefabs/FlySwatMiniGame/Fly.prefab`, `TapTargetMiniGame/Target·Decoy.prefab` 은 `PrefabUtility.LoadPrefabContents`/`UnloadPrefabContents` 로 같은 방식으로 훑었다.
- 자리표시 판정: 스프라이트 이름이 `UISprite`·`Background`·`Square`·`Circle`·`Knob`·`InputFieldBackground`·`Checkmark`·`UIMask` 이거나 `null` 이면 자리표시. 예외 ① `Image.type == Filled` 이면서 스프라이트가 `UI_White` 면 정상 게이지로 제외. 예외 ② 스프라이트가 `null` 이고 `color.a <= 0.001` 이면 "투명 클릭영역"으로 따로 센다(HQ `AgentSpot_*`, `DialogueBox/TapButton` 등 — 의도된 투명 히트박스).
- 두 번의 RunCommand 로 나눠 실행했다(씬 7개씩). 마지막 씬(TapTargetMiniGame)을 닫을 때 "마지막 씬은 닫을 수 없다"는 Warning 이 떴지만 저장하지 않았으므로 파일 변경은 없다(Unity_GetConsoleLogs Error 0 으로 확인).

### 19.1 씬/프리팹별 요약

| 씬/프리팹 | 전체 Image+SR | UISprite | Circle | Knob | 기타 자리표시 | 투명 클릭영역 | 자리표시 합계 | 비고 |
|---|---|---|---|---|---|---|---|---|
| Title | 3 | 0 | 0 | 0 | 0 | 0 | 0 | - |
| HQ | 51 | 4 | 0 | 6 | 0 | 4 | 10 | TopBar·SquadRow 3개=UISprite 4, Highlight 3+SelectedImage 3=Knob 6 |
| Battle | 53 | 0 | 7 | 0 | 0 | 1 | 7 | HpBar·BossHpBar는 T1.1에서 UI_White로 교체 완료(제외), 타임바 3개도 UI_White로 정상(제외), SelectionMarker/IntentIcon 7개=Circle |
| SamgakKimbapMiniGame | 9 | 0 | 1 | 0 | 0 | 0 | 1 | ResultRoot |
| EggMiniGame | 3 | 0 | 0 | 0 | 0 | 0 | 0 | - |
| PizzaMiniGame | 7 | 0 | 0 | 0 | 0 | 0 | 0 | - |
| USBMiniGame | 5 | 0 | 0 | 0 | 0 | 0 | 0 | - |
| PeelStickerMiniGame | 12 | 7 | 0 | 1 | 0 | 0 | 8 | Book Spine/Line_0~4/PeeledFlap=UISprite, CornerTab=Knob |
| ScreenBubbleMiniGame | 11 | 1 | 0 | 0 | 0 | 0 | 1 | PhoneFrame/Screen |
| ButtonShirtMiniGame | 14 | 2 | 0 | 0 | 0 | 0 | 2 | Shirt PlacketLeft/Right |
| VacuumMiniGame | 9 | 2 | 0 | 1 | 0 | 0 | 3 | Sofa/Body, Vacuum Stick=UISprite, Handle=Knob |
| FlySwatMiniGame | 2 | 0 | 1 | 0 | 0 | 0 | 1 | ResultRoot |
| BossBlobMiniGame | 4 | 1 | 0 | 1 | 0 | 0 | 2 | Ground=UISprite, ResultRoot=Knob |
| TapTargetMiniGame | 1 | 0 | 0 | 0 | 0 | 0 | 0 | - |
| Fly.prefab | 1 | 0 | 0 | 0 | 0 | 0 | 0 | - |
| Target.prefab | 1 | 0 | 0 | 0 | 0 | 0 | 0 | - |
| Decoy.prefab | 1 | 0 | 0 | 0 | 0 | 0 | 0 | - |
| **총계** | **187** | **17** | **9** | **9** | **0** | **5** | **35** | 씬 14개 + 프리팹 3개 (T1.1에서 Battle HpBar·BossHpBar 2건 교체 완료로 37→35) |

- §18.2 L514 의 "타임바 3개 UI_White 지정" 조치는 이번 실측에서 그대로 확인됐다(PhaseTimeBar·MiniGameTimeBar·EnergyGauge 는 Filled+UI_White라 자리표시에서 제외). 이전 스카우트가 이 셋을 UISprite로 잘못 옮겨 적었던 것이며, §18.2는 틀리지 않았다.
- 다만 Battle `TopBar/HpBar`, `BossHpRoot/BossHpBar` 는 §18.2 조치 대상이 아니었고, 이번 실측에서 Filled+UISprite(자리표시)로 새로 확인됐다. §17.2에 신규 행으로 추가했다.

### T1.1 보강 (2026-09-30)
- Battle `TopBar/HpBar`, `BossHpRoot/BossHpBar`: `Unity_RunCommand`로 `FindObjectsByType<Image>(FindObjectsInactive.Include, ...)` 로 찾아 `type==Filled`이고 경로에 `HpBar`가 들어간 Image만 골라 sprite를 `UI_White`로 바꾸고 저장했다(color·fillAmount·RectTransform은 그대로 둠). 저장 후 재조회로 `HpBar sprite=UI_White`, `BossHpBar sprite=UI_White`, 부모 프레임 `HpBarBg sprite=UI_HpBar(Sliced)`를 확인했다. §19.1 Battle 행의 자리표시 합계를 9→7로, 총계를 37→35로 고쳤다.
- HQ `AgentMenuPanel`/`UpgradeChoicePanel`/`ResultPanel`: `FindObjectsInactive.Include`로 비활성 오브젝트까지 포함해 재스캔했다(SetActive는 바꾸지 않음). `AgentMenuPanel` 하위 Image 20개 전부가 `UI_ResultPanel`/`UI_SquadButton`/전용 아이콘·포트레이트 이름을 갖고 있어 UISprite 자리표시는 0건이었다. `UpgradeChoicePanel`·`ResultPanel`이라는 이름의 오브젝트는 HQ 씬에 존재하지 않았다(Transform 전체 검색으로 재확인, `UpgradeSection`이 그 역할을 흡수한 것으로 보인다). 따라서 이 두 이름은 §17.2에서 "씬에 없음"으로 정리했고, 교체 미확증 이슈는 해소됐다.
- Unity_GetConsoleLogs(Error/Warning) = 0/0.

### 19.2 씬별 자리표시 상세

| 경로 | 컴포넌트 | 스프라이트 | 용도 추정 | 교체 대상 에셋 |
|---|---|---|---|---|
| HQ Canvas/SafeArea/AgentSpots/AgentSpot_Upgrade·Module·Story/Highlight | Image | Knob | 발밑 하이라이트 | ART_PROMPTS 확인 필요 |
| HQ Canvas/SafeArea/TopBar | Image | UISprite | 상단바 패널 배경 | §13 확인 필요 |
| HQ Canvas/SafeArea/SquadPanel/SquadRow_0~2 | Image | UISprite | 스쿼드 행 배경 | §13 확인 필요 |
| HQ Canvas/SafeArea/SquadPanel/SquadRow_0~2/SelectedImage | Image | Knob | 선택 표시 | 선택 마커 아이콘 |
| Battle Canvas/BattleHud/TopBar/HpBar | Image(Filled) | UI_White(T1.1 교체 완료) | HP 게이지 | - |
| Battle Canvas/BattleHud/BossHpRoot/BossHpBar | Image(Filled) | UI_White(T1.1 교체 완료) | 보스 HP 게이지 | - |
| Battle BattleWorld/PlayerView, EnemySlot0~2/MonsterView*/SelectionMarker | SpriteRenderer | Circle | 선택 표시(4개) | ART_PROMPTS 확인 필요 |
| Battle BattleWorld/EnemySlot0~2/MonsterView*/IntentIcon | SpriteRenderer | Circle | 몬스터 예고 아이콘(3개) | ART_PROMPTS §0-3(§18.5 4번과 동일 이슈) |
| SamgakKimbapMiniGame ResultRoot | SpriteRenderer | Circle | 결과 연출 자리 | ART_PROMPTS 확인 필요 |
| FlySwatMiniGame ResultRoot | SpriteRenderer | Circle | 결과 연출 자리 | ART_PROMPTS 확인 필요 |
| PeelStickerMiniGame Book/Spine, Line_0~4, Sticker/PeeledFlap | SpriteRenderer | UISprite | 스티커북 배경 요소(7개) | §13/ART_PROMPTS 확인 필요 |
| PeelStickerMiniGame Book/Sticker/CornerTab | SpriteRenderer | Knob | 스티커 모서리 탭 | 위와 동일 |
| ScreenBubbleMiniGame PhoneFrame/Screen | SpriteRenderer | UISprite | 화면 배경 | ScreenBubble 배경 아트 |
| ButtonShirtMiniGame Shirt/PlacketLeft, PlacketRight | SpriteRenderer | UISprite | 셔츠 플라켓(2개) | ButtonShirt 셔츠 아트 |
| VacuumMiniGame Sofa/Body | SpriteRenderer | UISprite | 소파 | Vacuum 배경 아트 |
| VacuumMiniGame Vacuum/Head/Stick | SpriteRenderer | UISprite | 청소기 막대 | Vacuum 청소기 아트 |
| VacuumMiniGame Vacuum/Head/Stick/Handle | SpriteRenderer | Knob | 청소기 손잡이 | 위와 동일 |
| BossBlobMiniGame Ground | SpriteRenderer | UISprite | 바닥 | BossBlob 배경 아트 |
| BossBlobMiniGame ResultRoot | SpriteRenderer | Knob | 결과 연출 자리 | ART_PROMPTS 확인 필요 |

- Fly.prefab, Target.prefab, Decoy.prefab 은 모두 실제 스프라이트(FlySwat_Fly_0, TapTarget_Target, TapTarget_Decoy)를 쓰고 있어 자리표시가 없다(§17.2 목록에도 없음, §19.1 표에서 확인용으로만 남긴다).
- Title·Egg·Pizza·USB·TapTarget 은 자리표시가 0개다.

## §20 BattleUnitView 월드 표시 (캠페인 T10)

### 20.0 기준
- 단위는 월드 유닛(u), PPU 100(1u = 100px 스프라이트 기준). 기준점 `head` = `body.bounds.max.y`(유닛 스프라이트 위쪽 끝).
- 레이어는 §2.6 을 따라 `World`. `sortingOrder` 는 `body` 를 기준으로 상대값(`body+1`~`body+4`, `body-1`)으로 적는다.
- 폰트 크기 단계는 §2.4(Caption 28pt 등)를 그대로 쓰되, 월드 공간 `TextMesh` 는 `characterSize`×`fontSize` 조합으로 값이 달라 별도 표기한다. 색은 §2.5 토큰 이름으로 적는다.
- BattleHud(TopBar/HpBar/energyText)는 §8에서 이미 설계됐다. 이 절은 `BattleWorld`(PlayerView/EnemySlot0~2/MonsterView) 아래 유닛별 월드 표시만 다룬다.

### 20.1 요소 표

| 요소 | 코드 심볼 | 크기 | 위치(head 기준 오프셋) | sortingOrder | 색·폰트 | 프롬프트 ID | 필요한 코드 필드 |
|---|---|---|---|---|---|---|---|
| 라벨 판 | `BattleUnitView.labelPlate`(신규) | 2.2u×0.9u(최대 3줄) | (0, +0.35) | body+1 | panel.base 배경, panel.border 외곽선 | WorldLabelPlate | `labelPlate`(SpriteRenderer), `labelFont`(Font) |
| label 텍스트 | `BattleUnitView.label`(기존 TextMesh) | characterSize 0.05, fontSize 56(Caption 28pt 상당) | 판 중앙(라벨 판과 동일 오프셋) | body+2 | 1줄 이름=text.primary, 2줄 HP=hp.red 계열, 3줄 상태=1줄 요약 | - | `labelFont`, `labelMaxLines`=3 |
| 예고 받침 | `BattleUnitView.intentBase`(신규) | 0.9u×0.9u | (0, +1.05) | body+3 | panel.base | IntentBadgeBase | `intentBase`(SpriteRenderer) |
| intentIcon | `BattleUnitView.intentIcon`(기존) | 0.7u | 받침 중앙(0, +1.05) | body+4 | 틴트 없음(white) | Intent_Attack / Intent_Guard / Intent_Special / Intent_Magic / Intent_Buff / Intent_Heal | `intentSprites`(Kind→Sprite 배열, 6종), `intentFallbackColor` |
| intentText | `BattleUnitView.intentText`(기존) | fontSize 44 | (+0.4, +0.8) | body+4 | text.primary, 외곽선은 그림자 오프셋 방식 | - | `intentFont` |
| selectionMarker | `BattleUnitView.selectionMarker`(신규 스프라이트) | 너비 `body.bounds.size.x`×1.3, 높이 0.35u | (0, -bodyHeight-0.1)(발밑) | body-1 | accent.primary | WorldSelectionRing | `selectionSprite`(Sprite), `selectionPulse`(bool) |
| DamagePopup 일반 | `DamagePopup`(기존, fontSize 필드화 대상) | fontSize 64 | head+0.2, x ±0.3 랜덤 | World 최상위 | hp.red, 외곽선 검정 2단 그림자 | - | `popupFont`, `sizeNormal`, `riseDistance`(0.8u), `duration`(0.6s), `outlineColor` |
| DamagePopup 강타(≥MaxHp 20%) | 위와 동일 | fontSize 80 | 위와 동일 | 위와 동일 | hp.red | - | `sizeCrit` |
| DamagePopup 회복 | 위와 동일 | fontSize 56 | 위와 동일 | 위와 동일 | success.green | - | `sizeHeal` |
| 피격 이펙트 | `BattleUnitView`(신규 프리팹 참조) | 1.2u, body 중심, 0.25s | body 중심 | body+1(플래시는 body 자체) | body 흰색 플래시 0.1s×2회 | FX_Hit_Spark | `hitFxPrefab`, `flashColor`, `flashDuration` |
| 정화 이펙트 | `BattleUnitView`(신규 프리팹 참조) | 1.4u, body 중심에서 위로 0.3u 이동, 0.4s | body 중심 → +0.3u | body+1 | success.green 계열 | FX_Cleanse_Sparkle | `cleanseFxPrefab` |
| 방어막/사망 상태 | `BattleUnitView.Refresh`(기존 로직 상수화) | - | - | - | 방어막: 현재 Lerp 0.35 파랑, 사망: 회색 α0.5 | - | `guardTint`, `guardBlend`(0.35f), `deadColor` |

### 20.2 겹침 해소 규칙 (§18.5 4번 해답)
- 세로 스택 순서(아래→위): 발밑 셀렉션 링(body-1) → body → 라벨 판(body+1, +0.35) → 예고 받침·아이콘(body+3~4, +1.05).
- 몬스터 슬롯(`EnemySlot0~2`) 간 최소 x 간격은 2.4u(라벨 판 폭 2.2u + 여백 0.2u)로 한다. 간격이 부족하면 라벨 판 너비를 줄이지 말고 슬롯 위치를 조정한다.
- 보스(예: MidBoss 등 768 캔버스급)는 head 오프셋 값은 그대로 쓰되, 라벨 판·예고 받침을 1.2배 스케일로 키운다(오프셋도 1.2배 비례 적용).

### 20.3 코드 필드 목록 (미반영, 구현은 후속)
- `BattleUnitView` 신규/변경 필드: `labelPlate`(SpriteRenderer), `labelFont`(Font), `labelMaxLines`(int=3), `intentBase`(SpriteRenderer), `intentSprites`(Kind→Sprite 매핑, 6종: Attack/Guard/Special/Magic/Buff/Heal), `intentFallbackColor`(Color), `intentFont`(Font), `selectionSprite`(Sprite), `selectionPulse`(bool), `hitFxPrefab`(GameObject), `flashColor`(Color), `flashDuration`(float), `cleanseFxPrefab`(GameObject), `guardTint`(Color), `guardBlend`(float=0.35f), `deadColor`(Color).
- `DamagePopup` 신규/변경 필드: `popupFont`(Font), `sizeNormal`/`sizeCrit`/`sizeHeal`(int, 각 64/80/56), `riseDistance`(float=0.8f), `duration`(float=0.6f), `outlineColor`(Color). 기존 `fontSize 48` 하드코딩은 위 3단계 필드로 대체 대상이다.

### 20.4 사람 확인
- [ ] Battle Play에서 세로 스택(발밑 링→body→라벨 판→예고 받침)이 기획대로 겹치지 않는지, 몬스터 3마리 간 라벨 판·아이콘이 서로 겹치지 않는지 확인한다.
- [ ] 데미지 숫자(일반/강타/회복 3단계)가 배경 위에서 가독성 있게 보이는지 확인한다.

### 20.5 캠페인T10.1 마무리
§20 설계(20.0~20.4)는 T10에서 완료 기준(요소마다 크기·오프셋·프롬프트 ID·코드 필드 명시)을 이미 채웠다. 이번 마무리 단계에서 `Unity_AssetGeneration_GetModels`를 다시 호출해 `Models:[]` 차단이 그대로인지 확인했다(재확인만, 문서 추가 변경 없음). 남은 두 항목은 범위상 후속 작업이다: (1) 사람이 모델을 등록해야 ART_PROMPTS §2-q 8행 실제 이미지 생성 가능, (2) BattleUnitView.cs/DamagePopup.cs에 §20.3 필드 반영은 구현 단계(T10 이후 별도 작업)에서 씬과 함께 처리한다(코드만 먼저 바꾸면 인스펙터 참조가 비어 §CLAUDE.md 검증 기준의 "None 없음"을 못 채운다).

### 20.6 캠페인T10.2 마무리
`Unity_AssetGeneration_GetModels` 재호출 결과 `Models:[]` 그대로다(변화 없음). §20 요소표 10행과 ART_PROMPTS §2-q 8행을 다시 대조해 크기·오프셋·order·프롬프트 ID·코드 필드가 빠짐없이 명시됐음을 재확인했다(문서 추가 변경 없음). T10의 완료 기준(전투 월드 표시 요소별 설계 완료)은 이미 충족돼 있어 이번 마무리는 재확인 절차로 마친다. 이미지 실제 생성(사람의 모델 등록 필요)과 BattleUnitView.cs/DamagePopup.cs 필드 반영(씬 연결 동반 후속 구현)은 다음 작업(T11 또는 별도 구현 작업)으로 이월한다.

## §21 T11 최종 일관성 검수와 사용 안내 (캠페인 T11)

### 21.1 §13·§17.2 상태 열 최신화
- §13 표: T3(UI_TopBar~UI_ButtonDisabled 9종)·T4(Icon_Crystal/Maze_DirArrow/MazeIcon_5종)·T4.1(맵 방 유형 코드 필드) 행 모두 "없음(§2-o 프롬프트, T3)" / "자리표시 유지" 상태 그대로 유효함을 재확인(문서 갱신 후 상태 변화 없음 — 이미지 생성이 차단된 상태가 지속되기 때문).
- §17.2 목록: `BattleWorld` 유닛별 SelectionMarker/IntentIcon 7개 행이 §20 설계(ART_PROMPTS §2-q)로 이어졌음을 재확인. 새로 자리표시가 늘거나 줄지 않았다(§19.1 실측 이후 씬 변경 없음).
- ART_PROMPTS §0-2 체크리스트에 §2-o/§2-o-1/§2-p/§2-q 4개 절이 새로 반영됐으므로, 이 문서의 §13·§17.2도 해당 4개 절을 참조하도록 위 §21.2 우선순위 표에 정리했다(표 자체의 "상태" 열 값은 변경 없음, 참조 누락만 보완).

### 21.2 사람이 생성할 순서 (우선순위 표)
`Unity_AssetGeneration_GetModels` 차단이 풀리면(사람이 Unity AI Assistant 패널에서 생성 모델 등록) 아래 순서로 생성한다. 기준: (1) 코드·씬 연결이 이미 끝나 이미지만 넣으면 바로 보이는 것 우선, (2) 여러 씬에서 공유돼 파급 효과가 큰 것 우선, (3) 구현 자체가 안 끝난 예정 미니게임은 마지막.

| 순위 | 절 | 에셋 | 이유 |
|---|---|---|---|
| 1 | §2-o (T3) | UI_TopBar 등 공용 UI 9종 | Title/HQ/Battle 전 씬에서 공유, 코드 변경 없이 스프라이트만 넣으면 즉시 반영 |
| 2 | §2-o-1 (T4) | Icon_Crystal·Maze_DirArrow·MazeIcon 5종 | 필드·씬 연결 이미 완료(T4.1), 이미지만 채우면 끝 |
| 3 | §2-p (T5) | HUD_HintBanner 등 미니게임 페이즈 HUD 7종 | MiniGamePhaseHud 필드·씬 연결 완료(T5.1), 모든 미니게임 공통 HUD라 파급 큼 |
| 4 | §2-q (T10) | WorldLabelPlate 등 전투 월드 표시 8종 | BattleUnitView.cs/DamagePopup.cs 필드 반영(§20.3)이 선행돼야 해서 이미지가 있어도 바로 못 씀 — 필드 반영 작업과 묶어서 진행 권장 |
| 5 | §2-l 잔여 | Obj_Egg_Chopstick 교체, FlySwat_Swatter 실제 그림 | 이미 임시 배치돼 게임은 동작 중, 교체는 비주얼 완성도 개선 |
| 6 | §2-m (T9) | 예정 미니게임 13종 53행 | 씬·프리팹 자체가 아직 없어 이미지가 먼저 있어도 바로 못 씀 — 각 미니게임 구현 착수 시점에 맞춰 순서대로(MINIGAME_IDEAS §5 제작 순서 그대로 따름) |

### 21.3 전수 일관성 검증 결과
- ART_PROMPTS.md §2-o/§2-o-1/§2-p/§2-q 신규 31개 에셋ID를 Grep으로 전수 검사 → 중복 0건, 오탈자 0건(§20.1·§13 참조와 철자 일치).
- `Assets/SampleImage/UI/**/*.png`(49개)·`Assets/SampleImage/Battle/**/*.png`(65개) Glob 결과 위 31개 신규 ID와 파일명 충돌 0건.
- `Unity_GetConsoleLogs`(Error) → errorCount 0.
- **완료 기준 충족**: 충돌 0건 검증 결과 기록 완료(본 절 + ART_PROMPTS §2-q 끝 "캠페인T11" 메모), 콘솔 Error 0.

### 21.4 캠페인T11.1 마무리
T11에서 이미 완료 기준(§0-2 체크리스트 갱신, 신규 31개 ID 중복·오탈자·png 이름 충돌 0건 검증, §13·§17.2 참조 보완, 콘솔 Error 0)을 채웠으므로 이번 마무리 단계는 재확인 절차로 마친다.
- `Unity_AssetGeneration_GetModels` 재호출 → `Models:[]` 그대로(T5.2부터 이어진 구조적 차단 지속, 변화 없음).
- `Unity_GetConsoleLogs`(Error) 재호출 → errorCount 0(T11과 동일).
- 21.2 우선순위 표·21.3 검증 결과는 모니터링 결과 그대로 유효해 추가 변경이 필요 없었다.
- 이번 작업 범위(T11.1: 최종 일관성 검수와 사용 안내 갱신 "마무리")에서 새로 발견된 불일치는 없다. 아래 세 항목은 구조적으로 이 문서 갱신 작업 범위 밖이라 이월한다(§21.2 우선순위 표·MINIGAME_IDEAS §5 제작 순서를 그대로 따른다):
  1. **사람**: Unity AI Assistant 패널에서 생성 모델 등록 → `Models:[]` 차단 해제.
  2. 모델 등록 후 §21.2 순서(§2-o→§2-o-1→§2-p→§2-q→§2-l 잔여→§2-m)대로 이미지 생성 착수.
  3. §2-q 이미지 생성과 묶어 `BattleUnitView.cs`/`DamagePopup.cs`에 §20.3 필드 반영(씬 연결 동반, 코드만 먼저 바꾸면 인스펙터 참조가 비어 CLAUDE.md 검증 기준을 못 채움).
  4. 예정 미니게임 13종(MG-04 등)은 씬·프리팹이 아직 없어 §2-m 프롬프트가 있어도 바로 못 씀 — MINIGAME_IDEAS §5 제작 순서대로 구현 착수 시 사용.

### 21.5 캠페인T11.2 마무리
T11.1까지 채운 완료 기준(§0-2 체크리스트 갱신, ID 중복·오탈자·png 충돌 0건, 콘솔 Error 0)이 그대로 유지됨을 재확인하고 마무리한다.
- `Unity_AssetGeneration_GetModels` 재호출 → `Models:[]` 그대로(T5.2부터 이어진 구조적 차단 지속, 변화 없음).
- `Unity_GetConsoleLogs`(Error) 재호출 → errorCount 0(T11·T11.1과 동일).
- §21.2 우선순위 표·§21.3/21.4 검증 결과는 모니터링 결과 그대로 유효해 추가 변경이 필요 없었다.
- 이번 작업 범위(T11.2: 최종 일관성 검수와 사용 안내 갱신 "마무리")에서 새로 발견된 ID 중복·오탈자·png 충돌은 없다. §21.4의 이월 4항목(모델 등록·§21.2 순서대로 생성 착수·§20.3 필드 반영·예정 미니게임 13종 구현 착수)은 그대로 유효하며, 이 문서 갱신 작업 범위 밖이라 계속 이월한다. 이번 세션에서 T1~T11.2까지 "Ui 리소스 필요한부분 디자인과 미니게임 관련 애셋에 대한 이미지 요구 프롬프트 제작수행" 전체 요청의 문서화 범위(설계·스펙·프롬프트·검수)는 완료 기준을 모두 채웠고, 남은 항목은 전부 위 이월 4항목(사람의 모델 등록이 선행 조건)으로 수렴한다.

## §22 UI 리소스 생성·연결 (2026-09-30)

ART_PROMPTS §2-o·§2-o-1·§2-p·§2-q 의 31개를 **기존 UI 프레임과 같은 잉크선 스타일로 스크립트로 그려서** 만들었다. 생성 모델 등록(§21.4)을 기다리지 않았고 크레딧은 쓰지 않았다. 그린 스크립트는 세션 스크래치 `draw_ui2.py`(`draw_frames.py`/`draw_ui.py` 헬퍼 재사용)이다. 기존 파일과 이름 충돌 0건을 확인하고 복사했다.

| 에셋 | 경로 | 9-slice(L,B,R,T) | 연결 |
|---|---|---|---|
| UI_TopBar | UI/ | 48,24,48,24 | UI_TopBar.prefab(HQ 상단바, 크림) — 글자는 ink 로 변경 |
| UI_CloseButton | UI/ | - | HQ AgentMenu·Squad 닫기(기존 X 글자 Label 은 끔) |
| UI_TabButton_On/Off | UI/ | 48,24,48,48 | **미연결** — AgentMenu 탭 UI 가 없음(역할별 섹션 하나만 표시) |
| UI_CardFrame | UI/ | 64 | 강화 선택 카드 3장(이름은 구분선 아래 -298, 설명 -386~) |
| UI_PanelDark | UI/ | 64 | AgentMenuPanel·SquadPanel·SquadRow_0~2 |
| UI_NameTag | UI/ | 48,24,48,24 | DialogueBox.prefab `BoxBg/NameTagBg`(박스 윗선에 걸친 명찰), NameText 가운데 정렬 |
| UI_TapNext | UI/ | - | DialogueBox.prefab `NextIndicator/Arrow`(글자 ▼ 대신) |
| UI_ButtonDisabled | UI/ | 56 | **미연결** — 버튼이 ColorTint 전환이라 disabledSprite 를 쓰려면 SpriteSwap 으로 바꿔야 함(눌림 틴트가 사라지는 부작용, 보류) |
| Icon_Crystal | UI/Icons/ | - | HQ TopBar/CrystalIcon |
| Maze_DirArrow | Battle/Maze/ | - | MapPanel DirButton_*/Arrow(버튼 위쪽 64, 글자는 아래 26pt) |
| MazeIcon_Start/Combat/Elite/Rest/Boss | Battle/Maze/ | - | MapHud.startIcon~bossIcon. 흰 실루엣+잉크 외곽선(흰 전투 칸에서도 보이게). Combat 은 주먹 대신 **교차한 칼**(60px 에서 주먹이 안 읽힘) |
| HUD_HintBanner | UI/MiniGameHud/ | 96,32,96,32 | `StartHintText` 오브젝트 자체의 Image 로 바꾸고 글자는 자식 `Label`(52pt ink). 전에는 배너가 글자의 자식이라 글자를 덮었다 |
| HUD_FuseSpark / HUD_FuseBomb | UI/MiniGameHud/ | - | 하단 타임바(§11)에 맞춰 앵커 y=0. 불꽃은 코드가 x 만 움직임, 폭탄은 왼쪽 끝(불이 타들어 가는 쪽) |
| HUD_Stamp_Success / Fail | UI/MiniGameHud/ | - | SuccessStamp/FailStamp |
| HUD_ResultPanel | UI/MiniGameHud/ | 64 | 미니게임 단계 결과 패널(900x480, 위 클립) |
| HUD_EnergyPopupBg | UI/MiniGameHud/ | 28 | **미연결** — EnergyPopup 이 Text 라 자식 배경은 글자를 덮는다(§22 HintBanner 와 같은 구조 변경 필요, 선택 사항이라 보류) |
| WorldLabelPlate, IntentBadgeBase, WorldSelectionRing, Intent_Magic/Buff/Heal | UI/, UI/Icons/ | Plate 48 | **미연결(월드 표시)** — §20.3 BattleUnitView 필드 반영이 선행. WorldSelectionRing 은 HQ 요원 발밑 하이라이트에 먼저 사용 |
| FX_Hit_Spark, FX_Cleanse_Sparkle | Battle/FX/ | Multiple 4프레임(256) | **미연결** — §20.3 hitFxPrefab/cleanseFxPrefab 필요 |

- 캡처로 HQ(상단바·대화 명찰·요원 메뉴·스쿼드)·Battle(강화 카드·맵 버튼/방 아이콘·힌트 배너·도장·타임바 불꽃/폭탄·결과 패널) 확인. 콘솔 Error 0.
- 남은 연결: §20.3 월드 표시 코드(라벨 판·예고 받침·예고 아이콘 6종·선택 링·피격/정화 FX) — 이미지는 이제 모두 있다.

## §23 코믹 UI 키트 적용 (2026-09-30, 사용자 결정)

§22 스크립트 도형판이 "PPT 같다"는 피드백으로 **웹툰 코믹 + 캐릭터 그림체** 방향으로 다시 만들었다(ART_PROMPTS §2-r).

- 컨셉: `Docs/Concepts/UIConcept_Battle_v1.png` (GPT Image 2.5, 참고 = Tripo 기록의 BG_Battle + Hero_Stand). 사용자 확정.
- 키트 시트 4장(각 20크레딧, 참고 = 컨셉): `Docs/Concepts/SheetA_Frames.png`(버튼 3상태·정사각 버튼·클립 패널·말풍선·명찰·상단바·카드·HP 튜브·닫기), `SheetB_HUD.png`(도장 2·POW·집중선·외침 말풍선·리본 배너·배터리·폭탄·불꽃·알약 태그·▼·화살표), `SheetC_Icons.png`(커맨드 4·맵 방 5·결정·체크·하트·번개·말줄임), `SheetD_World.png`(이름판·외침 배지·선택 링·데미지 스플래시·피격 4프레임·정화 4프레임).
- 회색 배경을 가장자리 연결 영역으로 지우고 요소별로 잘랐다(스크래치 `cut_sheet.py`). 결과 41장은 `Assets/SampleImage/UI/Comic/*_Comic.png`, FX 2장은 `Assets/SampleImage/Battle/FX/*_Comic.png`(4프레임 Multiple). §22 파일은 그대로 두었다(덮어쓰기 없음).
- 9-slice 는 원본 해상도가 커서 `Image.pixelsPerUnitMultiplier = 원본 높이 / 표시 높이` 로 테두리 두께를 맞췄다. HP 튜브·배터리는 안쪽 여백(튜브 L34 B46 R48 T32, 배터리 L20 B19 R19 T34 원본 px)을 multiplier 로 나눠 채움 Image 를 안쪽에 맞췄다.

| 화면 | 적용 |
|---|---|
| Title | 시작 버튼 |
| HQ | 상단바(UI_TopBar.prefab), 결정 아이콘, 출격·스쿼드 버튼, 요원 이름표·발밑 링, 요원 메뉴/스쿼드 패널(흰 종이 → 글자 ink 로 변경), 행 버튼, 닫기(클립과 안 겹치게 -120) |
| 대화창(prefab) | 박스 = 클립 패널, 명찰, ▼ |
| Battle | HP·보스 HP 튜브, 커맨드 4(아이콘 포함, 턴 종료 아이콘 추가), 스킬·뒤로(UI_Button.prefab 포함), 결과 패널·본부로, 맵 방향 버튼·화살표·방 아이콘 5, 강화 카드, 힌트 배너, 도화선 불꽃·폭탄, 도장 2, 미니게임 결과 패널, 에너지 배터리 |

- **미연결**(쓸 자리가 아직 없음): WorldLabelPlate/IntentBadge/WorldSelectionRing/DamageSplash/FX 2종(§20.3 BattleUnitView 코드 대기), UI_Button_Pressed(버튼 SpriteSwap 전환 시), ShoutBubble·POW·SpeedRing·PillTag·Icon_Check/Heart/Bolt/Talk(연출용 예비).
- 확인: 캡처 8장(Title·HQ·대화·요원 메뉴·스쿼드·전투·강화·맵·미니게임 HUD), 콘솔 Error 0.

## §24 전투 월드 표시·액션 애니메이션 구현 (2026-09-30)

§20.3 코드 필드를 코믹 키트(§23)로 구현했다. 캡처: `Docs/Captures/Battle_Anim_Overlay.png`(공격·몬스터 공격·방어·회복·대기 5장).

- **배치 기준**: `BattleUnitView.MeasureBody()` 가 몸 스프라이트의 Tight 메시 꼭짓점으로 실제 그림 영역(머리·발·중심·폭)을 잰다(투명 여백이 큰 768/1024 캔버스라 bounds 는 못 씀). 몬스터가 바뀌어도 자동으로 맞는다.
- **§20.2 대비 바뀐 점**: 이름·HP 판을 머리 위가 아니라 **발 아래**로 옮겼다. 머리 위에는 예고 말풍선만 두어 겹침이 구조적으로 없어졌다. 선택 링은 발밑.

| 요소 | 필드 | 스프라이트 | 위치 | order |
|---|---|---|---|---|
| 이름·HP 판 | `labelPlate`(자식 LabelPlate, Sliced, 0.5배) | WorldLabelPlate_Comic | 발 -0.2 아래, 글자 줄 수·길이에 맞춰 크기 | 6 (글자 7, 잉크색, cs 0.07) |
| 예고 말풍선 | `intentBase`(자식 IntentBase) | IntentBadge_Comic | 머리 +0.7, 높이 1.15u | 8 |
| 예고 아이콘 | `intentIcon` + `intentSprites[7]` | Attack=Icon_Attack, Special=Icon_Bolt, Guard=Icon_Guard, Magic=Icon_Skill, Buff=UI_ArrowUp, Heal=Icon_Heart (_Comic) | 말풍선 중앙 0.58u | 9 |
| 예고 숫자 | `intentText`(아이콘 밖으로 꺼냄) | - | 말풍선 오른쪽 아래, 해로운 행동은 빨강, 흰 그림자 | 11 |
| 선택 링 | `selectionMarker` | WorldSelectionRing_Comic | 발밑, 폭 = 몸 폭×0.9 | 1 |
| 피격 | `hitFxFrames`(4) + `damageSplash` | FX_Hit_Spark_Comic, FX_DamageSplash_Comic | 몸 중앙 / 머리 위 숫자, 최대 HP 20% 이상이면 1.3배 | 11~13 |
| 회복 | `healFxFrames`(4) | FX_Cleanse_Sparkle_Comic | 몸 중앙 위 | 11 |

- **액션 애니메이션**(Animator 없이 코루틴): `PlayAttack`(주인공 = Hero_Attack_Anim 3프레임 + 0.7u 전진, 몬스터 = `MonsterDefinition.attackSprite` + 0.6u 전진), `PlayDefend`(Hero_Defense_Anim 3프레임), `PlayHit`(`hitSprite` + 흔들림). 끝나면 서 있는 스프라이트·원위치로 돌아온다. `BattleController` 호출 지점: 커맨드 사용(주인공 공격/방어·맞은 적 피격), 적 행동(공격하는 적·피격된 주인공, 방어 성공 시 주인공 방어).
- 데이터: `MonsterDefinition.attackSprite/hitSprite` 신설, 몬스터 12종에 `<이름>_Attack/_Hit` 연결.
- 새 파일: `Assets/Scripts/BattleFlow/SpriteFx.cs`(프레임 이펙트 1회 재생). `DamagePopup` 에 스플래시 배경·그림자·크기 인자 추가.
- 검증: 컴파일 에러 0, EditMode 151/151, Play 에서 공격·피격·방어·회복 트리거 캡처 확인, 세이브 복원.
- 남은 것: Cook/Fashion 은 `CharacterDefinition` 에 스프라이트 필드가 없어 전투에 나오지 않는다(주인공만 애니메이션). 몬스터 공격은 1장 전환이라 주인공보다 단순하다.

### 24.1 독립 스쿼드 전투 표시 (F-37)
- 멤버마다 `BattleUnitView` 하나씩(HP·에너지·상태이상이 서로 독립, 공용 게이지 없음).
- 현재 차례인 멤버를 강조 표시한다(선택 링 등 기존 PlayerView 강조 방식 재사용).
- 사망 시 회색(0.3,0.3,0.3,0.5) 틴트와 `쓰러짐` 라벨(`BattleUnitView.cs`).
- 적 피격 연출(SpriteFx·DamagePopup)은 그 행동의 실제 target 멤버에게 재생된다.
- 이력: F-30 후방 지원 표시(`SupportView0/1`)는 F-37 로 대체됨(2026-09-30, 지원 멤버가 연출만 하고 전투 수치에 관여하지 않아 테스트 기각).

## §25 대화 표정 세트·펀치 연출 규격 (캠페인 T7, 2026-10-01)

### 25.1 표정 세트 구조
- `SpeakerExpressionRegistry`(ScriptableObject, `EXPR_Registry`) — `List<SpeakerExpressionSet> sets`, `Find(speakerName)` 로 화자 1명의 세트를 이름 일치 조회.
- `SpeakerExpressionSet`(화자 1명) — `speakerName`(예: "회사원"·"불편의 악마"·"스토리 요원"), `List<ExpressionEntry> expressions`, `TryGet(key, out sprite)`. 기본 키 상수 `DefaultKey = "Default"`.
- `ExpressionEntry` — `key`(표정 이름, 예: Panic/Angry/Joy) + `sprite`(초상화) 한 쌍.
- `DialogueLine` — `speakerName`, `portrait`(직접 지정 스프라이트, 있으면 세트 조회 생략), `expression`(세트 조회 키), `text`.
- 화자별 세트 목록(2026-10-01 기준, `SpeakerExpressionRegistry.cs` 주석): 불편맨(Default/Joy/Worried), 스토리 요원(Default/Concerned/Joy/Surprised/Cheer/Embarrassed), 강화 요원(Default/Encourage), 모듈 요원(Default/Thinking), 불편의 악마(Default/Angry/Laugh/Shocked/Annoyed), 회사원(Default/Panic/Determined/Relieved/Tired), 안내·COMFORT 사령관(Default 만).

### 25.2 해석 순서 (`PortraitResolver.Resolve`)
1. `line.portrait` 가 지정돼 있으면 그대로 반환(세트 조회 없음, 기존 호환 경로).
2. `registry` 가 없으면 `null`.
3. `registry.Find(line.speakerName)` 로 세트를 찾는다 — 없으면 `null`.
4. `line.expression` 이 비어 있으면 `Default` 를 키로 쓴다.
5. 세트에서 키로 조회(`TryGet`) 성공 시 그 스프라이트.
6. 키가 `Default` 가 아니고 실패했으면 `Default` 로 한 번 더 조회(폴백).
7. 그래도 없으면 `null`(포트레이트 `Image.enabled = false`, `DialogueBox.Render` L86).

### 25.3 펀치 연출 규격 (`PortraitPunch`, `DialogueBox.PunchRoutine`)
- 발동 조건(`ShouldPunch`): 이전 줄 스프라이트와 이번 줄 스프라이트가 **둘 다 null 이 아니고 서로 다를 때만**. 대화 첫 줄(이전 없음)·포트레이트 없는 줄은 대상 아님.
- 곡선(`EvaluateScale`): `duration`(기본 0.12초)의 앞 절반 `1 → peak`(기본 1.1배), 뒤 절반 `peak → 1` 삼각형 보간. `elapsed<=0`/`elapsed>=duration`/`duration<=0` 이면 1(기본 크기).
- 적용 대상: `portrait.rectTransform.localScale`(스프라이트 자체는 즉시 교체, 스케일만 코루틴으로 보간).
- 재생 중 새 줄이 오면 `StopPunch()` 로 이전 코루틴을 멈추고 스케일을 1로 되돌린 뒤 새로 판정한다(겹쳐 재생되지 않음).
- 이번 캠페인(T7) 실측: `DialogueSample.unity` Play 모드에서 회사원(Panic→Relieved)·불편의 악마(Angry→Laugh)·스토리 요원(Joy→Surprised) 총 6줄을 `DialogueBox.Play`/`Advance` 로 재생 — 매 줄 `PortraitResolver.Resolve` 결과가 `Coworker_Portrait_Panic → …Relieved → Devil_Portrait_Angry → …Laugh → Agent_Story_Portrait_Joy → …Surprised` 순으로 정확히 바뀜(RunCommand 로그, 콘솔 에러 0건). 매 줄 이전 스프라이트와 다르므로 `ShouldPunch=true` 6/6.

## §26 전투 스킬·피격 모션 규격 (캠페인 T8, 2026-10-01)

- 공격·스킬(`BattleUnitView.AttackRoutine`, 프레임 3장 이상일 때): f0 을 `frameTime`(기본 0.09초) 동안 보이고, f1 에서 `lungeDistance`(기본 0.5) 만큼 전진해 `frameTime×2`(0.18초) 동안 보이고, f2 에서 홈과 전진 위치의 중간(`Vector3.Lerp(home, forward, 0.5)`)으로 반쯤 복귀해 `frameTime`(0.09초) 동안 보인 뒤 `StopAction`이 위치를 `homePos`로, 스프라이트를 `standSprite`로 되돌린다. 한 사이클 총 소요 약 0.36초.
- 대체 규칙(`PlaySkill`): `skillFrames`가 3장 미만(null 포함)이면 `PlayAttack()`으로 대체한다. `attackFrames`도 3장 미만이면 `AttackRoutine`의 `n<3` 분기(전진 0.12초 유지 후 0.18초 대기, 프레임 전환 없음)로 대체한다.
- 피격(`HitRoutine`): `hitSprite`를 표시하고 0.28초 동안 `Mathf.Sin` 기반 좌우 흔들림(진폭 0.08, 경과 비율에 따라 점점 줄어듦)을 재생한 뒤 `homePos`·`standSprite`로 복귀한다. `hitSprite`가 없으면 스프라이트 교체 없이 흔들기만 한다.
- 방어(`PlayDefend`): `defendFrames`를 프레임마다 `frameTime`씩 순서대로 보인 뒤 마지막 프레임을 0.25초 유지한다. `defendFrames`가 비어 있으면 아무 것도 하지 않는다(현재 위치·스프라이트 유지).
- 새 액션은 `StartAction`→`StopAction`으로 이전 코루틴을 끊고 위치·스프라이트를 원상 복귀한 뒤 새로 시작한다(겹쳐 재생되지 않음).
- 로그 형식: `[Anim] member=<id> motion=attack|skill|hit`(`BattleController.UseCommand`·피격 처리 경로에서 기록).
- 이번 캠페인(T8) 실측: `Battle.unity` Play 모드에서 3인 편성(hero·cook·fashion)으로 `BattleUnitView.PlaySkill()`을 직접 호출해 `hero→Hero_Skill_Anim_0`, `cook→Cook_Stand`(첫 호출, 스킬 f0가 대기 스프라이트와 동일한 프레임이라 이름만 같음), `fashion` 포함 세 멤버 모두 f1 프레임(`Hero/Cook/Fashion_Skill_Anim_1`)으로 스프라이트가 바뀌고 `lungeDistance`만큼 전진한 위치(hero 약 -4.80, cook 약 -6.90, fashion 약 -2.90)를 `Unity_SceneView_Capture2DScene`으로 캡처해 확인했다. 이어 세 멤버의 `hitSprite`(`Hero/Cook/Fashion_Hit`)로 직접 교체한 프레임도 캡처로 확인했다(총 2장, 6멤버 프레임 모두 포함). 콘솔 에러 0건.

## §28 미니게임 히트 영역 기준 (히트 영역 캠페인 T6, 2026-10-01)

### 28.1 기준
- 미니게임 클릭·드래그 대상의 Collider2D는 **1080p 화면 기준 지름(원형) 또는 짧은 변(사각형) 최소 120px**을 목표로 한다.
- 보정은 **Collider2D(radius/size)만** 키운다. `SpriteRenderer`의 스프라이트와 `Transform.localScale`(TapTarget 프리팹 제외)은 바꾸지 않는다 — 비주얼 크기와 판정 크기를 분리해, 그림은 그대로 두고 손 닿는 범위만 넓힌다.
- 같은 화면에 동시에 있는 대상끼리는 영역이 겹치면 안 된다(겹치면 오조작 유발). `Bounds.Intersects`로 판정.
- 예외: ButtonShirt Button3·4(97.2px)는 인접 단추와 겹치지 않는 선을 우선해 120px 기준에 못 미치는 채로 승인됐다(28.2 참고).
- TapTarget은 콜라이더가 아니라 프리팹 `Transform.localScale`(1.0→1.3)로 보정한 예외다 — Target/Decoy 둘 다 CircleCollider2D.radius=0.5(불변)이고 스케일로 지름을 키웠다.

### 28.2 변경 전후 표
px 환산식은 28.3 참고. 월드→px 계수는 모든 미니게임 씬에서 `Camera.main.orthographicSize=5` → 108px/unit로 실측 일치했다.

| 게임 | 오브젝트 | 필드 | 전→후(필드값) | px 전→후 | 겹침(인접 대상) |
|---|---|---|---|---|---|
| PeelSticker (T1) | CornerTab | BoxCollider2D.size | 미기록→(1.4, 1.4) | 미기록→151.2px | 단일 대상(해당 없음) |
| Vacuum (T2~T2.2) | Handle | CircleCollider2D.radius(local) | 미기록→0.653 | 미기록→141.0px | Head(64.0px)·Dust(86.4px)·SofaEdge(64.8px)와 역할이 달라 겹침 비교 대상 아님 |
| ButtonShirt (T3~T3.2) | Button0–2 | CircleCollider2D.radius(local) | 0.5→1.3 | 108.0→140.4px | Button0-1,1-2 겹침 False |
| ButtonShirt (T3~T3.2) | Button3–4 | CircleCollider2D.radius(local) | 0.5→0.9(겹침 방지로 축소 유지, **120px 미달 예외**) | 108.0→97.2px | Button2-3,3-4 겹침 False |
| SamgakKimbap (T4) | Tab_Top/Left/Right | BoxCollider2D.size | (1,1)→(1.8,1.8) | 86.4→155.5px | Top-Left,Top-Right,Left-Right 겹침 전부 False |
| FlySwat (T5) | Fly(런타임 스폰, difficulty 1/2/3) | `radius`(난이도별) | 0.5(flyRadius, 삭제)→0.61/0.64/0.72 | 80px대→최소 131.8px(난이도3, 최댓값 155.5px) | 동시 스폰 간격은 코드 스폰 로직이 별도 관리, 정적 겹침 비교 대상 아님 |
| BossBlob (T5) | Blob(런타임 스폰, difficulty 1/2/3) | `radius`(난이도별) | 100px대→0.6/0.62/0.68 | 100px대→최소 129.6px(난이도3, 최댓값 146.9px) | 위와 동일(런타임 스폰) |
| ScreenBubble (T5~T5.1) | 카드(씬 인스턴스) | `cardRadius` | 0.4→0.5 | 86.4→108.0px(카드 단독), 접촉 판정은 `cardRadius+bubbleRadii[i]`로 최소버블(0.18) 기준 약 146.9px | 버블과 카드는 드래그 접촉 판정이라 정적 겹침 비교 대상 아님 |
| TapTarget (T5~T5.2) | Target/Decoy 프리팹 | `Transform.localScale`(콜라이더 radius=0.5 불변) | 1.0→1.3 | 108.0→140.4px | Target·Decoy는 동시 등장 배치가 런타임 결정, 정적 겹침 비교 대상 아님 |
| USB | USB 콜라이더 | (유지) | 변경 없음 | 변경 없음 | 클릭 대상 특성상 유지 결정(T5 기록) |

PeelSticker·Vacuum의 "전" 수치는 ROADMAP·UI_DESIGN·git log 어디에도 남아 있지 않아 "미기록"으로 둔다(T1·T2 당시 변경 커밋이 콜라이더 값을 로그하지 않음).

### 28.3 측정 방법
- `Unity_RunCommand`로 각 미니게임 씬을 `OpenSceneMode.Additive`로 열고, 씬 안의 `Camera`에서 `orthographicSize`를 읽어 `pxPerUnit = 1080 / (2 * orthographicSize)`를 구한다(모든 씬에서 108px/unit으로 일치).
- 씬에 정적으로 존재하는 대상은 `Collider2D.bounds.size`의 짧은 변 × pxPerUnit로 px를 구하고, 같은 씬의 대상 쌍을 `Bounds.Intersects`로 겹침 판정한 뒤 저장 없이 씬을 닫는다.
- 런타임 스폰 대상(FlySwat Fly, BossBlob Blob)은 씬에 인스턴스가 없어, 매니저 스크립트의 난이도별 `radius` 로컬 변수 값(`Spawn()` 호출부)을 직접 읽어 지름을 계산했다.
- 프리팹만 있는 대상(TapTarget Target/Decoy)은 `AssetDatabase.LoadAssetAtPath<GameObject>`로 불러와 `CircleCollider2D.radius × Transform.localScale`로 계산했다.
- 코드형 판정(ScreenBubble `cardRadius`)은 씬에 로드된 매니저 컴포넌트 인스턴스의 필드 값을 직접 읽었다(스크립트 기본값 0.4와 다르게 씬 인스턴스에 0.5로 오버라이드돼 있음 확인).
- 전수 측정 결과: 120px 미달은 ButtonShirt Button3·4(97.2px, 승인된 예외) 뿐이었다. 그 외 전부 120px 이상, 인접 대상 겹침 0건.
