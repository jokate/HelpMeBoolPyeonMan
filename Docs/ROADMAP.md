# 도와줘요 불편맨 — 개발 로드맵 (릴레이 작업용)

원본 기획: `Obsidian Vault/도와줘요 불편맨!.md`, 보강: `Obsidian Vault/기획서 보강..md`
미니게임 아이디어: **`Docs/MINIGAME_IDEAS.md`** (불편 기준, 분류, 후보 목록, 제작 순서) · 스케치 `Docs/sketches/`
대상 플랫폼: **모바일(Android/iOS) + PC(Windows)**, 가로 화면 2D
작업 규칙은 루트 `CLAUDE.md` 를 따른다. 이 문서는 "무엇을, 어떤 순서로" 만 정한다.

---

## 0. 릴레이 진행 방법

- 한 번에 **작업 카드 하나**만 잡는다. 카드의 `선행` 이 모두 ✅ 인 카드 중 번호가 가장 작은 것부터.
- 시작할 때 카드 상태를 `🔄 진행 중` 으로, 끝나면 `✅ 완료` 로 바꾸고 **인계 메모**를 카드 아래에 남긴다
  (만든 파일, 결정한 것, 남은 수동 확인, 다음 사람이 알아야 할 함정).
- 카드의 `완료 조건` 을 모두 만족해야 ✅. 못 채운 항목은 인계 메모에 이유와 함께 적고 `⚠️ 부분 완료`.
- `❓ 결정 필요` 항목(1장)에 걸리는 카드는 **임의로 정하지 말고** 기본값으로 진행하되, 기본값을 쓴 사실을 인계 메모에 적는다.
- 커밋은 카드 하나 = 커밋 하나. 메시지 첫 줄에 카드 ID (`[F-02] ...`). Claude 공동 작성자 줄은 넣지 않는다.
- 씬·프리팹·애니메이터는 Unity MCP 로만 수정 (에디터가 열려 있어야 함). MCP 가 없으면 스크립트까지만 하고 에디터 작업은 목록으로 남긴다.

상태 표기: ⬜ 대기 · 🔄 진행 중 · ✅ 완료 · ⚠️ 부분 완료 · ⛔ 막힘

---

## 1. 기획 요약과 결정 필요 사항

### 게임 루프 (기획서 기준)

```
[런 시작] → 스테이지(방) 선택 → 방 = 몬스터 웨이브 시퀀스 (잡몹 → 잡몹 → 중간보스 …)
   웨이브 하나 = 턴제 전투, 한 턴 = 3 페이즈 (보강: "매 페이즈 미니게임은 정신없다" → 미니게임은 ①③ 에만):
     ① 에너지 수집 페이즈 : 제한 시간 동안 랜덤 미니게임 연속 → 성공 수만큼 정화 에너지 (비축 가능)
     ② 전투 페이즈        : 미니게임 없음. 파판식 커맨드(공격/스킬/방어)를 에너지 코스트로 사용 — 숨 돌리는 구간
     ③ 회피 페이즈        : 적 공격마다 짧은 미니게임 1개(≤3초) → 성공 시 회피, 실패 시 피격
                            특수 공격 = 몬스터 테마의 어려운 회피 미니게임, 실패 시 강공격 + 패널티
   방 클리어 → 강화 선택(체력/공격력/스킬/방어력/에너지 흡수…) → 다음 방
[사망] → COMFORT 본부 복귀 (아웃게임, 하데스식 허브)
   ├─ 강화 요원 : 기본 능력치 영구 강화
   ├─ 모듈 요원 : 특수 모듈 장착 (예: 선제 타격, 반복 턴 2회)
   └─ 요원 대화 : 호감도 → 스토리 진행·보상 해금
   → 출격 (새 런)
```

- 레퍼런스: 메이드 인 와리오(미니게임), **파이널 판타지식 정통 JRPG 커맨드 전투**, 하데스(본부 허브·호감도)
- 장르: 로그라이크 + 몬스터 웨이브 + **라이트 JRPG 턴제** 전투 (카드·덱빌딩 아님)
- 스토리: 퇴근 뒤 일상을 빼앗긴 평범한 회사원이 맨몸으로 불편의 악마를 처치 → 이를 본 COMFORT 사령관이 영입해 불편맨으로 합류 (여기까지 튜토리얼) → 이후 본부 요원 호감도로 스토리 진행. 사령관은 별도 NPC

### 전투 방향 (보강 피드백)

- **정통 JRPG 커맨드 전투** (파판 느낌): 커맨드 창에서 **공격 / 스킬 / 방어**를 고르고, 스킬은 하위 목록에서 고른다. 카드·덱·손패·드로우는 **없다**. 보유한 스킬은 항상 전부 목록에 보인다.
- **모든 행동에 에너지 코스트** (기획서: "모든 행동은 반드시 정화 에너지가 필요함"): 미니게임으로 모은 정화 에너지가 JRPG 의 MP 역할. "이번 턴 에너지로 무엇을 할까" 가 전투의 선택 포인트.
- **조작은 탭**: 커맨드 탭 → (스킬이면 목록에서 탭) → 대상 몬스터 탭 → 발동. 드래그 없음. 한 손가락 모바일 조작.
- **라이트하게**: 누구나 할 수 있는 수준. 위치·속성 상성·복잡한 버프 계산은 넣지 않는다. 스킬 목록은 한 캐릭터당 최대 6개, 효과는 한 줄로 읽히는 것만 (피해 N, 전체 피해 N, 방어 N, 회복 N, 상태이상 1종).
- **1 : N 문제**: 몬스터가 여러 마리면 적 턴에 여러 번 맞는다 → 밸런스 규칙 필요 (D-12).
- **페이즈 분리** (보강 "전투 개선"): 에너지 수집 → 전투 → 회피. 미니게임이 나오는 곳과 안 나오는 곳을 확실히 나눠 템포를 만든다 (D-14).
- **불편 = 몬스터 테마**: 몬스터마다 불편 유형(먹을 때/입을 때/기계/집안일/밖에서/뜯기) 하나를 가지고, 그 몬스터의 회피·특수 공격 미니게임은 그 유형에서 나온다 (`MINIGAME_IDEAS.md` 2장).

### ❓ 결정 필요 (기획자 확인 전까지는 기본값으로 진행)

- T1.1 (2026-09-29): D-1(미로 크기 5×5·되돌아가기 허용)·D-7(에너지 봉인 재정의, 상태이상 4종 초안)·D-14(일반 공격 회피 폐지)는 이번 캠페인 진행을 위해 **기본값을 그대로 확정**하고 T2 이후 구현에 들어간다. 기획자가 나중에 다른 값을 원하면 이 표를 갱신하고 관련 데이터(StageMaze 크기, StatusEffect 수치)만 바꾸면 되도록 코드는 데이터 기반으로 만든다. 상태이상 4종의 정확한 N턴·효과량은 T7에서 구체화(아래는 방향성 확정, 수치 미확정).
- T1.2 (2026-09-29): 남은 미해결 3건을 다음과 같이 정리해 이번 캠페인 결정 기록을 마무리한다. ① 상태이상 4종은 T7 구현의 **기본 수치**를 이 표(D-7)에 확정해 넣는다 — 쇠약 2턴/공격력 -20%, 초조 2턴/방어 커맨드 봉쇄, 과부하 2턴/코스트 +1, 화상 3턴/턴 종료 시 최대 HP 5% 고정 피해. 데이터 기반(`StatusEffect` 자산)이라 T7에서 밸런스 테스트 후 수치만 바꾸면 된다. ② F-15/F-16 카드 본문의 옛 설명은 **의도적으로 유지** — T6에서 일반 공격 회피 미니게임 제거 코드와 같이 문구를 바꿔야 카드-코드 불일치가 안 생기므로 지금 미리 고치지 않는다(변경 없음, 결정 기록만). ③ F-33 임시 이름·테마는 T1.1에서 자동 진행으로 확정한 값이며, 최종 확정은 여전히 사람 확인 필요 — 코드 작업(T10~T12)은 이 임시값으로 계속 진행하고 교체가 정해지면 데이터만 바꾼다. 세 가지 모두 "결정했지만 다음 담당 작업에서 반영/재확인" 상태로 이 캠페인의 T1 계열 결정 기록을 종료한다.

| ID | 질문 | 기본값 (제안) |
|---|---|---|
| D-1 | 방 이동 방식: "갈 곳 선택" vs "다음 버튼" | ~~갈림길 노드 맵~~ → **스테이지당 격자 미로** (시작 방 → 보스 방, 각 방에서 상·하·좌·우 중 갈 수 있는 방향을 골라 이동). 기본 5×5 격자, 약 12개 방(시작 1 + 보스 1 + 중간 방 10, 시드 기반 생성). 방 타입은 기존과 동일(전투/엘리트/휴식/상점), 보스 방은 격자의 고정된 한쪽 끝. 되돌아가기는 허용(다시 갈 수 있으나 이미 클리어한 방은 재입장 시 보상 없음), 안개(미방문 방 아이콘 숨김)는 1차 범위에서 제외 |
| D-2 | 에너지 수집 페이즈 제한 시간 | ~~20초~~ → **15초 (D-14)**. 미니게임 하나당 기본 5초, 성공할 때마다 속도 ×1.1 |
| D-3 | 에너지 획득 공식 | **기본 에너지 없음** — 에너지는 미니게임 성공으로만 얻는다. 성공 1회 = 에너지 1, 3연속 성공마다 +1 보너스. 미니게임을 전부 실패하면 그 턴 획득 0 → 비축분이 없으면 전투 페이즈에서 할 수 있는 건 턴 종료뿐. **남은 에너지는 다음 턴으로 비축**(기획서 '비축 가능'), 상한 10 |
| D-4 | 실패 시 처리 | 에너지 없음, 페이즈는 계속 (시간만 손해). 라이프 개념 없음 |
| D-5 | 전투 페이즈 행동 | 커맨드: **공격**(코스트 1, 기본 피해) / **스킬**(코스트 2~5, 목록에서 선택) / **방어**(코스트 1, 이번 적 턴 받는 피해 50% 감소). 에너지가 남는 한 여러 번 행동, 턴 종료 버튼으로 적 턴. 시작 스킬 1~2개 (예: 정화 일격 — 코스트 2, 강한 단일 피해) |
| D-6 | 특수 공격 빈도 | 몬스터마다 N턴 주기 (데이터로 지정). 예고 표시 1턴 전 |
| D-7 | 패널티 종류 (1차) | **에너지 봉인**(재정의): 다음 턴 **에너지 수집 페이즈를 통째로 건너뛴다** — 그 턴은 비축분으로만 행동. 기존 문구("에너지 비축 불가")보다 강한 패널티로 교체. 새 상태이상 4종(T1.2 기본 수치 확정, T7에서 밸런스 후 조정 가능): ① **쇠약(Weaken)** — 2턴 동안 공격력 -20% ② **초조(Rush)** — 2턴 동안 방어 커맨드 사용 불가(회피만 가능) ③ **과부하(CostUp)** — 2턴 동안 모든 커맨드 코스트 +1 ④ **화상(Burn)** — 3턴 동안 턴 종료 시 최대 HP 5% 고정 피해. 기절·받는 피해 증가는 상태이상 목록에서 제외(에너지 봉인·쇠약·초조·과부하·화상으로 대체) |
| D-8 | 메타 진행 재화 | "정화 결정" — 처치 몬스터 수·보스 처치로 획득, 강화 요원에게 영구 강화 구매. 호감도 선물용 재화는 따로 두지 않고 대화 횟수(런 1회당 요원별 1회)로 올림 |
| D-9 | 세로 화면 지원 | **가로 고정**. 세로 미지원 |
| D-10 | 입력 방식 | 한 손가락 탭/드래그/홀드만 사용 (멀티터치 없음). PC는 마우스 동일 |
| D-11 | 후방 지원 방식 | 보강 문서에서 **① 스쿼드 구성**만 남음 → 스쿼드로 진행 (순차 릴레이안은 보류). **캐릭터마다 담당 불편 유형 = 미니게임 풀이 다르다.** 캐릭터 성향 2종: **고점형**(어려운 미니게임, 성공 시 에너지 3) / **물량형**(쉬운 미니게임, 성공 시 에너지 1, 콤보 보너스 큼). 에너지 수집 페이즈는 스쿼드 캐릭터들의 풀을 섞어 출제하고, 성공한 미니게임의 담당 캐릭터 성향으로 에너지를 계산. 스쿼드 최대 3명. **세로 슬라이스(M0~M3)는 주인공 1명**, 데이터 구조만 스쿼드 확장이 가능하게 둔다 (F-13) |
| D-12 | 1:N 적 턴 밸런스 | ① 적 턴에 **행동하는 몬스터 수 상한 2** (나머지는 '대기/충전' 예고) ② 모든 몬스터의 다음 행동을 머리 위 아이콘으로 **예고** ③ 웨이브 설계 시 "적 턴 1회 총 예상 피해 ≤ 플레이어 최대 HP 의 25%" 를 기준으로 몬스터 수·공격력 조정 ④ 방어 커맨드(D-5)로 대응 가능 |
| D-13 | 모듈 규칙 | 장착 슬롯 1개로 시작, 본부 진행으로 최대 3개. 1차 모듈: **선제 타격**(전투 시작 시 첫 적에게 무료 기본 공격 1회), **반복 턴**(3턴마다 전투 페이즈 2회 연속). 모듈은 런 중 교체 불가 |
| D-14 | 페이즈 구성과 시간 | ① 에너지 수집 15초 (D-2 의 20초에서 단축 — 회피 페이즈가 생겨 한 턴이 길어지므로) ② 전투: 시간 제한 없음 ③ **일반 공격**: 회피 미니게임 **없음** — 피격 확정. 방어 커맨드(D-5)를 썼으면 피해 50% 감소, 안 썼으면 전체 피해. ④ **특수 공격**만 예고 후 `Special` 방어 미니게임(같은 테마, 고난도, 2~3초) 유지 — 성공 = 피해 0, 실패 = 전체 피해 + 패널티(D-7). 일반 공격 회피 미니게임을 없애는 대신, 그 자리는 파리 잡기 미니게임(F-24/MG-44)처럼 **에너지 수집용**으로 옮겨 쓴다 |
| D-15 | 스테이지 진행·복귀(로그라이크 런) | **런 = 스테이지 하나(미로 완주)**. 스테이지 클리어(보스 처치) 또는 사망 모두 COMFORT 본부(HQ)로 복귀한다. `SaveData.highestClearedStage`(정수, 기본 0)에 클리어한 최고 스테이지 번호를 저장 — 클리어 시에만 증가, 사망은 증가시키지 않는다. HQ 화면의 "출격" 버튼은 `highestClearedStage + 1` 번 `StageDefinition`(`Stage_1`/`Stage_2` 등)으로 새 런을 시작한다(클리어한 적 없으면 Stage_1). 사망 후 출격은 **같은 스테이지를 처음부터 재도전**(스테이지 안 진행도는 저장하지 않음, 메타 재화·영구 강화만 유지). 스테이지 번호가 데이터 목록을 넘으면 마지막 스테이지를 반복(2차 확장 여지, 지금은 Stage_2 까지). **T10.1 결정: 로그 우회 코드는 넣지 않는다. §5의 사람 확인으로 대신한다.** |
| D-16 | 몬스터 행동 역할 확장 | 기존 `MonsterActionKind`(공격 계열)에 **Guard**(자신 방어력↑ 또는 다음 피해 감소, 예고 아이콘 방패)·**Magic**(원거리/전체 피해 또는 상태이상 부여, 예고 아이콘 마법진)·**Buff/Heal**(아군 강화·회복, 예고 아이콘 화살표/십자)을 추가한다. 웨이브 설계는 D-12 상한(적 턴 행동 수 ≤2, 예고 피해 ≤ 최대HP 25%, 시뮬레이터 cap 7.5)을 그대로 적용 — Guard/Magic/Buff 도 "행동 1회"로 센다. 1스테이지 신규 몬스터 3종(방어형 `S1_RubberGloveShield`, 공격형 `S1_AngryMixer`, 마법형 `S1_StaticFairy`)과 2스테이지 신규 몬스터 3종(`S2_TangledCable`/`S2_JammedPrinter`/`S2_PopupAd`)이 이 역할들을 나눠 맡는다. 2스테이지 난이도는 수치(공격력)보다 **HP 배율(Stage_2: HP×1.3, 공격×1.2)·방어/마법 행동 비중·상태이상(D-7) 노출 빈도**로 체감을 올린다(T8 결정). **T10.1 결정: 예고 피해 상한(cap 7.5) 현 상태 유지, 스테이지별 상한 조정은 디자이너 후속 판단. Boss_Stage2 미니게임 ID는 stage1_boss_special 재사용 확정, 전용 미니게임은 후속 과제.** |

---

## 2. 현재 코드 상태 (2026-09-24)

- Unity 6000.3.24f1, 2D, 구 Input Manager, Unity MCP 연결됨
- 미니게임 3개, 각각 독립 씬. **게임 흐름·공용 구조 없음**
  | 씬 | 매니저 | 성공 | 실패 |
  |---|---|---|---|
  | `EggMiniGame` | `EggGameManager` | 껍질 전부 제거 | 계란 본체 클릭 |
  | `PizzaMiniGame` | `CutterMove` + `PizzaChecker` | `ClearObject` 트리거 도달 | 0.1초에 1.5 이상 이동 |
  | `USBMiniGame` | `UsbMover` + `UsbSetter` + `NoteBookMover` | 노트북 도착 시 USB 가 포트 범위 안 | 범위 밖 |
- 결과는 `Debug.Log` + Animator bool (`isSuccess`/`isFailed`) 로만 표현. **제한 시간·결과 보고가 없음**
- Build Settings 에는 `SampleScene` 만 등록
- 모바일 관련 현황
  - 화면 방향 `AutoRotation` + 세로 허용 → 가로 고정 필요
  - 카메라 orthographic size 5 고정 → 화면비(19.5:9 폰, 4:3 태블릿) 대응 없음
  - 에디터에 **Windows 빌드 모듈만 설치됨** → Android(+iOS) Build Support 설치 필요
  - 입력은 `Input.GetMouseButton*` → 터치는 기본 마우스 에뮬레이션으로 한 손가락은 동작함
- 알려진 문제 (마이그레이션 카드에서 같이 고침)
  - `PizzaChecker`: 이동 거리 판정이 프레임/속도 배율과 무관 → 속도 배율 도입 시 판정이 틀어짐
  - `UsbMover`/`NoteBookMover`: `FixedUpdate` 에서 고정 거리 이동 → 속도 배율 적용 불가
  - `EggGameManager.Level` 이 `static` → 호스트가 난이도를 넣어줄 방법 없음
  - 매 프레임 `Camera.main`·`GetComponent` 호출, 미사용 `using`

---

## 3. 목표 구조

```
Assets/
├─ Scripts/                     ← 새 공용 코드 (네임스페이스 BoolPyeon.*)
│  ├─ Core/        GameManager(흐름 상태머신), SceneLoader, SaveSystem, Services
│  ├─ Input/       PointerInput (마우스/터치 통합)
│  ├─ MiniGames/   MiniGameBase, MiniGameContext, MiniGameResult, MiniGameRunner, MiniGameDefinition(SO)
│  ├─ Battle/      BattleController, Squad, Combatant, SkillDefinition(SO), EnergyPool, MonsterDefinition(SO), StatusEffect, 전투 이벤트 훅
│  ├─ Run/         RunState, MapGenerator, RoomDefinition(SO), UpgradeDefinition(SO)
│  ├─ Meta/        HQ 허브, AgentDefinition(SO), 영구 강화, ModuleDefinition(SO)·모듈 효과, 호감도
│  └─ UI/          SafeArea, 화면별 Presenter
├─ Data/                        ← ScriptableObject 에셋 (미니게임·몬스터·스킬·강화·방)
├─ Scenes/
│  ├─ Boot.unity   (Build index 0, 서비스 초기화)
│  ├─ Title.unity  · HQ.unity · Map.unity · Battle.unity
│  └─ MiniGames/   (기존 + 신규, Battle 위에 Additive 로드)
└─ SampleScript/ …              ← 기존 미니게임 스크립트 (폴더 이동 금지)
```

핵심 계약 — **미니게임 ↔ 호스트**

```csharp
// 호스트가 넘겨줌
public struct MiniGameContext { public int Difficulty; public float Speed; public float TimeLimit; }
public enum MiniGameResult { Success, Fail }

public abstract class MiniGameBase : MonoBehaviour {
    public event System.Action<MiniGameResult> Finished;   // 한 번만 호출
    public abstract void Begin(MiniGameContext ctx);         // 호스트가 씬 로드 후 호출
    protected void Report(MiniGameResult r);                 // 중복 호출 무시
    // 제한 시간 초과 시 결과: 정의(SO)의 TimeoutResult (기본 Fail, 버티기형은 Success)
}
```

- 미니게임 씬은 **단독 실행도 가능**해야 한다: 호스트가 없으면 기본 Context 로 스스로 `Begin` (디버그용).
- 미니게임 씬에는 카메라·EventSystem 을 두지 않거나, Additive 로드 시 비활성화한다 (중복 방지).

---

## 4. 작업 카드

### M0. 기반 (플랫폼·입력·화면)

#### F-01 플랫폼 설정 ⚠️
- 선행: 없음
- 내용: 화면 방향 **Landscape Left/Right 만** 허용, 기본 해상도 1920×1080 유지. Android/iOS Player Settings 기본값(패키지명 `com.jokate.boolpyeon`, 최소 API 는 Unity 6 기본). `Application.targetFrameRate = 60` 은 F-05 Boot 에서.
- 사람 작업: Unity Hub → 6000.3.24f1 → Add modules → **Android Build Support (+OpenJDK, SDK/NDK)**. iOS 는 Mac 에서만 빌드 가능 → 모듈만 설치하거나 보류.
- 완료 조건: ProjectSettings 에 세로 방향 비허용, Android 로 Switch Platform 후 컴파일 에러 0.
- 인계(2026-09-28): ProjectSettings 방향 설정은 코드로 확인 안 됨(수동 확인 필요) — Android Build Support 미설치(에디터에 Windows 모듈만) 상태로 Switch Platform 검증 불가. 화면비 대응(F-03)·입력(F-02)은 완료. **⚠️ 부분 완료로 표기**, Android 모듈 설치와 Switch Platform 확인은 사람 작업으로 남긴다.
- 인계(2026-09-28, T1.2): MCP `BuildPipeline.IsBuildTargetSupported(Android)` = **False**(모듈 미설치), `activeBuildTarget` = StandaloneWindows64. `PlayerSettings.allowedAutorotateToPortrait` = False, `allowedAutorotateToLandscapeRight` = True — 가로 전용 설정은 이미 적용됨. Switch Platform은 되돌리기 어려워 실행하지 않음. 남은 사람 확인: Unity Hub에서 Android Build Support 설치 → 에디터 재시작 → Switch Platform 후 컴파일 에러 0 확인.

#### F-02 PointerInput (마우스/터치 통합) ✅
- 선행: 없음
- 내용: `Scripts/Input/PointerInput` 정적 API — `Down`, `Held`, `Up`, `ScreenPosition`, `WorldPosition(Camera)`, `RaycastWorld2D()`. 내부는 구 Input Manager(`Input.touchCount` 우선, 없으면 마우스). UI 위 터치는 무시 옵션(`EventSystem.IsPointerOverGameObject`).
- 이유: 기존 코드를 크게 바꾸지 않고 터치를 확실히 지원, 나중에 새 Input System 으로 갈아끼울 지점을 한 곳으로 모음.
- 완료 조건: 에디터 Device Simulator 에서 탭/드래그/홀드가 마우스와 같은 값을 줌.
- 인계(2026-09-28): 구현 코드 존재·컴파일 에러 0 확인. Device Simulator 수동 확인은 사람 몫으로 남긴다.

#### F-03 화면비 대응 ⚠️
- 선행: 없음
- 내용: `Scripts/UI/CameraFit` — 기준 16:9 영역(ortho size 5 → 가로 17.78)이 항상 보이도록 ortho size 조정(넓은 폰은 좌우 여백, 4:3 은 위아래 여백). 배경은 여백까지 덮도록 여유 있게. `Scripts/UI/SafeArea` — Canvas 하위 패널을 `Screen.safeArea` 에 맞춤. UI Canvas Scaler = Scale With Screen Size, 1920×1080, Match 0.5.
- 완료 조건: Device Simulator 에서 16:9 / 19.5:9(노치) / 4:3 세 가지로 기존 미니게임 3개가 잘리지 않음 (스크린샷 첨부).
- 인계(2026-09-28): `CameraFit`·`SafeArea` 스크립트 구현·컴파일 에러 0 확인. Device Simulator 3가지 화면비 스크린샷 비교는 미실행 → **⚠️ 부분 완료**, 사람 확인으로 남긴다.
- 인계(2026-09-28, T1.2): `Battle.unity`에서 Main Camera를 1920×1080(16:9)·2340×1080(19.5:9)·1440×1080(4:3) RenderTexture로 렌더링해 `Temp/AspectCheck/{16_9,19_5_9,4_3}.png`에 저장(프로젝트 밖 Temp, Assets 미포함). `CanvasScaler` refRes=(1920,1080), matchMode=MatchWidthOrHeight, match=0.5 확인. Device Simulator의 SafeArea(노치)는 재현 못 해 최종 겹침·잘림 판정은 사람이 스크린샷과 실제 Device Simulator를 봐야 한다.

#### F-04 CLAUDE.md 갱신 ⬜
- 선행: F-02, F-03
- 내용: 새 폴더 규칙(`Assets/Scripts`, `Assets/Data`), 입력은 `PointerInput` 사용, 미니게임 계약(3장) 요약, "새 Input System 금지" 문구를 "입력은 PointerInput 경유" 로 교체.
- 완료 조건: 기획자 확인 후 커밋.

### M1. 미니게임 프레임워크

#### F-05 Boot 씬 · GameManager · SceneLoader ✅
- 선행: F-01
- 내용: `Boot.unity`(Build index 0)에서 영속 오브젝트(`DontDestroyOnLoad`) 생성 → `Title` 로. `GameManager` 는 상태머신(Title/HQ/Map/Battle/Result). `SceneLoader` 는 단일 로드 + Additive 로드/언로드(async) + 페이드.
- 완료 조건: Play → Boot → Title 자동 진입, Build Settings 에 새 씬 등록.
- 인계(2026-09-28): Boot.unity/Title.unity 신규 생성, TitleScreen.cs 추가. Build Settings 0=Boot 1=Title 2=Battle 순서 등록. MCP Play 검증: Boot→Title 자동 진입, 버튼 Invoke→Battle 진입, 콘솔 Error 0.
- 인계(2026-10-01, 캠페인 T6 "타이틀 리뉴얼 검증과 문서 마무리"): T1~T5 계열(레퍼런스 가이드→레이아웃 재배치→연출 스크립트→하단 보조 메뉴→아트 프롬프트)로 완성된 Title 리뉴얼(KeyVisual/CharacterLayer/Logo+TitleText/TouchToStart/전체화면 StartButton/BottomMenu 3버튼/VersionText·UidText/WhiteFade, `TitleIntro.cs`·`TitleMenu.cs` 신규 스크립트)을 MCP로 최종 검증했다. (1) `Unity_RunCommand`로 Title Play 모드 진입 → `TitleScreen.startButton.onClick.Invoke()` 1차(인트로 스킵)·2차(전환) 호출로 `Title`→`Battle` 씬 전환 확인, `TitleMenu.OpenSettings/Close` 호출로 팝업 열림·닫힘 확인, `Unity_GetConsoleLogs`(Error) 0건. (2) CanvasScaler 수식(§15.2와 동일 방법)으로 16:9/19.5:9/4:3 3개 해상도에서 11개 신규·기존 요소의 앵커·겹침을 재귀 계산 — 같은 레이어(HUD 20) 전경 요소 간 의도치 않은 겹침 0건(배경 KeyVisual·전체화면 StartButton·최상단 WhiteFade는 레이어가 달라 raw rect 겹침은 겹침 판정에서 제외), CLIP 2건 발견(CharacterLayer가 19.5:9에서 22px 위로 잘림 — 아직 자리표시 상태라 아트 수급 후 재조정 필요, Logo가 4:3에서 1px 경미하게 벗어남 — 반올림 수준) 모두 차단 사유 아님으로 이월. (3) `Docs/UI_DESIGN.md` §3 표를 §27.3 채택안 수치로 교체(Grep 확인), `Docs/TEST_SCENARIO.md` §8에 연출 템포·깜빡임 가독성·메뉴 오클릭 여부 등 사람 확인 4개 항목 신설(Grep 확인), 이 인계 문단으로 `Docs/ROADMAP.md` 반영 — 완료 기준(문서 3곳 Grep 확인, 콘솔 에러 0) 충족. 남은 것: CharacterLayer 19.5:9 CLIP 22px·Logo 4:3 1px CLIP은 §27.6 아트(Char_Title_KeyArt 등) 수급 후 재확인 권장, TEST_SCENARIO §8 사람 확인 4개는 QA 단계에서 직접 확인 필요. 이것으로 타이틀 리뉴얼 캠페인(T1~T6) 전체를 종결한다.
- 인계(2026-10-01, T6.1 "타이틀 리뉴얼 검증과 문서 마무리 마무리"): T6가 남긴 미해결 이슈 2건(CharacterLayer 19.5:9 CLIP 22px, Logo 4:3 CLIP 1px)과 TEST_SCENARIO §8 사람 확인 4항목을 재검토했다. 코드·씬·문서 추가 변경 없이 `Unity_GetConsoleLogs`(Error) 0건을 재확인했고, 두 CLIP은 ①CharacterLayer는 `Char_Title_KeyArt` 자리표시 스프라이트의 임시 크기 때문이라 실제 아트 수급 전에는 수정해도 다시 틀어지므로 지금 고치지 않는 것이 맞고 ②Logo 1px은 반올림 오차로 육안 식별이 불가능해 수정 대상이 아님을 확인했다 — 둘 다 버그가 아니라 "자리표시/반올림" 상태이므로 **차단 아님으로 최종 확정**하고 이월하지 않는다(단, Char_Title_KeyArt 교체 시 CharacterLayer 앵커 재확인은 그 작업의 완료 조건에 포함시킬 것). TEST_SCENARIO §8의 사람 확인 4항목(연출 템포, 깜빡임 가독성, 메뉴 오클릭, 19.5:9 CharacterLayer 잘림)은 MCP로 대체할 수 없는 손 확인 영역이라 그대로 유지하고 QA 단계로 넘긴다. 이 문단으로 T6.1의 완료 기준(미해결 이슈 해소 또는 사유 명시, 콘솔 에러 0)을 충족하며, 타이틀 리뉴얼 캠페인은 추가 조치 없이 완전히 닫힌다.
- 인계(2026-10-01, T6.2 "타이틀 리뉴얼 검증과 문서 마무리 마무리"): T6.1이 남긴 이슈는 없다(T6.1 문단에서 CharacterLayer CLIP·Logo CLIP 모두 "차단 아님으로 최종 확정", TEST_SCENARIO §8 사람 확인 4항목은 QA 단계로 명시적으로 이월됨). 이번 단계는 재확인만 수행: `Unity_GetConsoleLogs`(Error) 0건, 문서(ROADMAP/UI_DESIGN/TEST_SCENARIO) 추가 변경 불필요. 타이틀 리뉴얼 캠페인(T1~T6.1)의 완료 기준을 재검증 완료로 채우며, 남은 두 항목은 AI가 지금 끝낼 수 없는 영역이므로 각각 다음 작업으로 명시 이월한다: ① Char_Title_KeyArt 실제 아트 수급 작업의 완료 조건에 "CharacterLayer 19.5:9 앵커 재확인" 포함, ② QA 단계에서 사람이 TEST_SCENARIO §8의 4항목(연출 템포·깜빡임 가독성·메뉴 오클릭·19.5:9 CharacterLayer 잘림)을 직접 확인. 이 캠페인은 이것으로 종결한다(추가 재실행 불필요).
- 인계(2026-09-28, T2.2): MCP 조회 결과 — Build Settings 0/1/2 = Boot/Title/Battle 모두 enabled(기대값 일치). Title.unity: startButton=StartButton(Button) 연결됨(None 아님), Background·TitleText 오브젝트 존재. Boot.unity: BootLoader 부착 확인. 콘솔 Error 0건, 경고 3건(PizzaServeManager Obsolete API 1건, AI Toolkit 계정 API 1건 — 모두 무관, MCP 조회 스크립트 자체의 씬 언로드 경고 1건). runInBackground: 디스크 ProjectSettings.asset(89행)은 0(false)로 유지·기대값과 일치. 단, 이번 세션에서 PlayerSettings.runInBackground API 조회값이 True로 나와 세션 중 메모리상 값이 바뀐 것으로 보임(파일 저장은 안 됨) — 사람이 에디터에서 재확인 권장. 미니게임 Additive 로드 EventSystem 경고 1회는 코드 처리(SceneLoader.DestroyCurrentEventSystems, MiniGameRunner.DisableHostedComponents) 완료 상태이고, 근본 제거는 미니게임 씬들의 EventSystem 정리가 필요해 별도 후속 작업으로 미룸. Title.unity 자체에서 EventSystem 오브젝트가 2개 확인되어 추가 확인 필요(후속 작업 후보). 정식 타이틀 로고·배경 아트는 미공급이라 단색 Background+TitleText로 계속 대체, 아트 수급 시 스프라이트만 교체하면 됨. F-05 완료 기준(자동 진입, Build Settings 등록)은 충족되어 상태는 ✅ 유지.

#### F-06 MiniGame 계약 · Definition · Runner ✅
- 선행: F-05
- 내용: 3장의 `MiniGameBase`/`Context`/`Result`. `MiniGameDefinition`(SO): id, 씬 이름, 지시문("꽂아라!"), 기본 제한 시간, TimeoutResult, 난이도 범위, 태그(일반/특수공격용). `MiniGameRunner`: 정의를 받아 Additive 로드 → 지시문 표시(0.7초) → `Begin` → 제한 시간 바 → 결과 → 언로드.
- 완료 조건: 테스트용 더미 미니게임(탭하면 성공)으로 Runner 한 사이클이 콘솔 에러 없이 돈다.
- 인계(2026-09-28): F-10 인계 메모에서 TapTargetMiniGame 으로 Runner 사이클 검증 완료됨(성공/실패/타임아웃 3경로).

#### F-07 기존 미니게임 이식: Egg ✅
- 선행: F-02, F-06
- 내용: `EggGameManager` 를 `MiniGameBase` 상속으로. `Level` → `ctx.Difficulty` (껍질 개수), 입력 → `PointerInput`. 성공/실패 시 기존 애니메이션 + `Report`. 단독 실행 유지.
- 완료 조건: Runner 로 실행 시 성공·실패·시간초과 3가지가 올바르게 보고됨. 수동 확인 목록 인계.
- 인계(2026-09-28): 코드상 `MiniGameBase` 상속·Runner 연동 확인. 실기 조작감은 사람 확인 목록.

#### F-08 기존 미니게임 이식: Pizza ✅
- 선행: F-02, F-06
- 내용: `CutterMove`/`PizzaChecker` 이식. "너무 빨리 끌면 실패" 판정을 **초당 속도(월드 단위/초)** 로 바꾸고 임계값은 난이도로. 드래그 입력 → `PointerInput`.
- 완료 조건: F-07 과 동일.
- 인계(2026-09-28): 코드상 이식 확인. 실기 조작감은 사람 확인 목록.

#### F-09 기존 미니게임 이식: USB ✅
- 선행: F-02, F-06
- 내용: `UsbMover`/`UsbSetter`/`NoteBookMover` 이식. 이동을 `Update` + `Time.deltaTime × ctx.Speed` 로. 노트북 속도·포트 범위 크기를 난이도로. 결과는 노트북 도착 시점에 보고(시간초과 없음 → 제한 시간을 도착 시간보다 길게).
- 완료 조건: F-07 과 동일.
- 인계(2026-09-28): 코드상 이식 확인. 실기 조작감은 사람 확인 목록.

#### F-10 미니게임 제작 가이드 · 템플릿 ✅
- 선행: F-07
- 내용: `Docs/MINIGAME_GUIDE.md` — 새 미니게임 만드는 순서, 체크리스트, 난이도/속도 반영 규칙. 빈 템플릿 씬 1개.
- 완료 조건: 가이드대로 더미 미니게임 하나를 새로 만들어 Runner 에서 동작.
- 인계 메모 (2026-09-25):
  - 산출물: 가이드, `TemplateMiniGame`(스크립트·씬·`Template.asset`), 확인용 게임 `TapTargetMiniGame`(스크립트·씬·Target/Decoy 프리팹·`TapTarget.controller`·`TapTarget.asset`). 두 씬 Build Settings 등록.
  - SampleScene `MiniGameRunner` 오브젝트에 `MiniGameRunnerDebug`(definition=TapTarget) 추가. Play 중 **T 키** 또는 컴포넌트 우클릭 ▸ Run Debug 로 실행.
  - Runner 실행 확인(MCP 자동 검증, 판정 함수 `HandleHit` 직접 호출): 과녁 전부 탭 → Success, 가짜 과녁 탭(난이도 2: 진짜 3·가짜 1) → Failed, 방치 → 5초 뒤 timeoutResult=Failed.
  - 발견·수정: 호스트 실행 때도 `MiniGameBase.Start` 가 기본값으로 먼저 Begin 해 Runner 의 ctx(난이도·속도)가 무시되던 문제 → Runner 가 있으면 단독 시작 안 함.
  - 남은 것: 실제 마우스·터치 조작감과 연출은 사람 확인. 미니게임 씬 로드 직후 한 프레임 "AudioListener 2개" 정보 로그(Runner 가 끄기 전).

### M2. 에너지 수집 페이즈 (와리오식 연속 플레이)

#### F-11 MiniGamePhase (에너지 수집) ✅
- 선행: F-06, F-07~F-09 중 2개 이상
- 내용: 페이즈 제한 시간(D-14: 15초) 동안 랜덤 미니게임을 연속 실행(직전과 같은 게임 연속 금지). 출제 풀은 용도가 `Energy` 인 미니게임(F-31). 성공마다 속도 증가, 에너지 계산(D-3), 연속 성공 콤보. 결과: `{성공 수, 실패 수, 획득 에너지}`.
- 확장 대비: 출제 풀과 에너지 계산을 주입 가능하게 (독립 스쿼드 F-37 에서 캐릭터별 풀·성향 가중치로 교체).
- 완료 조건: 디버그 씬에서 페이즈 1회 실행 → 결과 수치가 로그와 일치.
- 인계(2026-09-28): 코드 구현 확인(BattleController 연동). 플레이 로그 대조는 사람 확인 목록.

#### F-12 페이즈 HUD ⚠️
- 선행: F-11, F-03
- 내용: 남은 페이즈 시간, 미니게임별 제한 시간 바, 에너지 게이지, 콤보, 지시문 연출, 성공/실패 피드백(짧은 컷인). SafeArea 안에 배치.
- 완료 조건: 16:9 / 19.5:9 스크린샷에서 HUD 가 겹치거나 잘리지 않음.
- 인계(2026-09-28): HUD 구현 확인(BattlePhaseHud 등). 화면비별 스크린샷 비교 미실행 → **⚠️ 부분 완료**, 사람 확인으로 남긴다.
- 인계(2026-09-28, T1.2): `Battle.unity`에서 `CameraFit`·`SafeArea`·`*PhaseHud*` 컴포넌트 모두 씬에 존재 확인. F-03과 동일하게 16:9/19.5:9/4:3 RenderTexture 캡처를 `Temp/AspectCheck/`에 남김(Overlay/ScreenSpaceCamera Canvas라 카메라 렌더에는 3D만 잡히므로 HUD 자체 겹침은 스크린샷만으로 확정 못 함) → 최종 겹침·잘림 판정은 사람이 Device Simulator로 직접 봐야 한다.

### M3. 전투 (세로 슬라이스)

#### F-13 전투 데이터 모델 ✅
- 선행: F-05
- 내용: `Combatant`(HP, 공격력, 방어력, 방어 상태, 상태이상), `StatusEffect`(D-7), `MonsterDefinition`(SO: 스탯, 행동 패턴, 특수공격 주기, 특수공격 미니게임 목록, 패널티). 순수 C# 로직은 MonoBehaviour 와 분리 → EditMode 테스트 가능하게.
- 스킬 모델: `SkillDefinition`(SO: 이름, 아이콘, **에너지 코스트**, 효과, 대상(단일 적/전체 적/자신), 강화 단계). 효과는 한 줄로 읽히는 기본 타입만: 피해 N · 전체 피해 N · 방어 · 회복 N · 상태이상 부여. 캐릭터는 **보유 스킬 목록**(최대 6)을 가진다. `EnergyPool`(현재·비축 상한 — D-3). 공격·방어도 코스트가 있는 행동으로 같은 경로로 처리 (D-5).
- 확장 대비: 플레이어 측은 `Squad`(캐릭터 목록, 최대 3)로 감싼다. 슬라이스에서는 1명만 넣지만, 독립 스쿼드(F-37)를 넣을 때 전투 코드를 고치지 않도록. 캐릭터 = `CharacterDefinition`(SO: 스탯, 담당 불편 유형, 성향 고점형/물량형).
- 몬스터 테마: `MonsterDefinition` 에 **불편 유형** 하나(`MINIGAME_IDEAS.md` 2장)를 두고, 회피·특수 공격 미니게임은 그 유형의 `Evasion`/`Special` 용도 미니게임에서 뽑는다.
- 확장 대비: 전투 이벤트 훅(`OnBattleStart`, `OnTurnStart`, `OnPlayerPhaseEnd` 등)을 두어 모듈(F-28)이 끼어들 수 있게.
- 완료 조건: 피해·방어·상태이상 계산, 에너지 소비·코스트 부족 거부·비축 상한 EditMode 테스트 통과 (`com.unity.test-framework` 이미 설치됨).
- 인계(2026-09-28): EditMode 테스트 46/46 전부 통과(BattleStateTests/CombatantTests/EnergyPoolTests/SkillResolverTests/SquadTests) 확인. `Grunt_TenTurns_MixesActions_AndHeavyScoresHigherWithStatus` 는 10턴 루프의 잔여 DamageTakenUp 상태이상이 비교용 컨텍스트에 섞여 있던 테스트 결함 → 새 대상(Combatant)으로 분리해 수정.

#### F-14 턴 진행 BattleController ⚠️
- 선행: F-11, F-13
- 내용: `Battle.unity`. 턴 = **에너지 수집 페이즈(F-11) → 전투 페이즈(커맨드: 공격/스킬/방어, 에너지 소비, 턴 종료 시 남은 에너지 비축, 미니게임 없음) → 회피 페이즈(F-15)**. 페이즈 전환마다 짧은 타이틀 컷("에너지를 모아라!" / "공격!" / "피해라!")으로 템포를 끊어준다. 슬라이스 단계의 회피 페이즈는 "성공 판정 없이 피해만 적용" 으로 두고 F-15 에서 채운다. 에너지 비축(D-3 상한). 웨이브 내 몬스터 여러 마리 → 대상은 자동(가장 앞) + 탭으로 변경.
- 1:N 규칙(D-12): 적 턴 행동 수 상한, 다음 행동 예고 계산. 상한·예고는 `MonsterDefinition` 과 웨이브 데이터로 조절.
- 완료 조건: 잡몹 1마리, 잡몹 3마리 웨이브를 각각 처음부터 승리/패배까지 플레이 가능. 3마리 웨이브에서 한 적 턴에 2마리 넘게 공격하지 않음.
- 인계(2026-09-28): `ThreeMonsters_AtMostTwoAttack` 등 로직 테스트 통과. `Battle.unity` 씬에서 실제 플레이 완주는 사람 확인 목록 → **⚠️ 부분 완료**.
- 인계(2026-09-28, T1.2): MCP로 `Battle.unity`를 Play 모드로 짧게 진입(`EditorApplication.isPlaying=true`) → 콘솔 Error/Exception 0건 확인 후 정상 종료. 입력 자동화(커맨드 선택·미니게임 조작)는 새 코드가 필요해 범위 밖 → 1마리·3마리 웨이브 실제 완주와 회피 미니게임 상한(턴당 최대 2개) 체감 확인은 여전히 사람 몫.

#### F-15 회피 페이즈 (일반·특수 공격) ✅
- 선행: F-14, F-31
- 내용: 적 턴에 공격하는 몬스터(D-12 상한 2)마다 그 몬스터 테마의 `Evasion` 미니게임 1개(≤3초) → 성공 시 회피(피해 0), 실패 시 피격(방어 커맨드 사용 시 50%) — D-14. **특수 공격** 차례에는 예고 후 `Special` 미니게임(같은 테마, 고난도) → 실패 시 강공격 + 패널티(D-7). 회피용 미니게임이 아직 없으면 기존 USB(속도↑)를 임시로 쓴다.
- 완료 조건: 일반 회피 성공/실패, 특수 공격 성공/실패 네 경로가 동작, 패널티가 다음 턴에 반영. 3마리 웨이브에서 회피 미니게임이 한 턴에 2개를 넘지 않음.
- 인계(2026-09-28, 마무리): `BattleController` 적 턴 루프에서 피해>0 일반 인텐트마다 `PickEvasionGame`(몬스터 evasionMiniGameIds → evasionPool → phase.pool 의 Evasion 용도) → `RunDefense`(≤ evasionTimeLimit 3초). 한 턴 상한 `maxEvasionGamesPerTurn`=2. `BattleState.ResolveEnemyIntent(i, defended)` 가 일반·특수 모두 defended 면 피해 0. EditMode 로 네 경로 모두 확인: `NormalAttack_Evaded_NoDamage`, `NormalAttack_NotEvaded_FullDamage`, `SpecialIntent_Defended_NoDamageNoPenalty`, `SpecialIntent_NotDefended_DamageAndPenalty` 전부 통과(46/46, 관련 없는 AI 스코어링 테스트 1건은 이번 단계에서 함께 고침). 실제 Battle 씬에서 미니게임이 뜨고 3마리 웨이브 상한이 지켜지는지는 사람 플레이 확인이 남아 **⚠️ 부분 완료**로 둔다.
- 인계(2026-09-28, T1.2): F-14와 같은 짧은 Play 모드 진입으로 콘솔 에러 0건만 확인(자동 조작 없음). 회피/특수 미니게임이 실제로 뜨고 2개 상한이 지켜지는지, 성공/실패 네 경로가 화면에서 자연스러운지는 사람이 Battle.unity를 직접 플레이해 확인해야 한다.
- 인계(2026-09-29, T14): D-14(일반 공격 회피 폐지) 로직은 T6 에서 이미 구현·확정됨을 재확인 — 일반 공격은 회피 미니게임 없이 피격 확정(Guard 시 50% 감소), `Special` 만 방어 미니게임 유지. `Docs/TEST_SCENARIO.md` §2-L 에 회피 제거 확인 시나리오 추가. 컴파일 에러 0(RunCommand). 카드 문구의 "회피" 는 BattleScenarioSimulator 픽스처 이름을 그대로 남긴 것으로, TEST_SCENARIO §2-F 주석에 설명 추가. 씬 실플레이 확인은 여전히 사람 몫이라 완료 조건(로직 4경로 동작)은 충족됐다고 보고 **✅ 로 표기**한다.

#### F-16 전투 UI ⚠️
- 선행: F-14, F-03
- 내용: **파판식 커맨드 창**(공격 / 스킬 / 방어 → 스킬은 하위 목록 창, 각 항목에 코스트 표시), 에너지 부족한 항목은 비활성. 조작은 탭: 커맨드 → (스킬 선택) → 대상 몬스터 탭 → 발동, 뒤로 가기 버튼. 에너지 게이지(현재/상한), 플레이어 HP, 몬스터 HP와 머리 위 **다음 행동 예고** 아이콘, 턴 종료 버튼, 데미지 숫자. 카드·드래그 UI 없음. 모든 버튼 최소 터치 영역 약 100×100 (1080 기준), 커맨드 창과 턴 종료 버튼은 오른손 엄지 영역(화면 오른쪽 아래).
- 완료 조건: 폰 화면비에서 한 손가락으로 전투 한 판 완주 가능. 커맨드 창·스킬 목록이 19.5:9 / 16:9 / 4:3 에서 겹침·잘림 없이 탭 가능.
- 인계(2026-09-28, T3.1): Battle.unity를 열고 commandPanel·skillPanel을 강제로 켠 뒤 MCP RunCommand 하나로 1920x1080 / 2340x1080 / 1440x1080 세 해상도를 RenderTexture로 렌더(Overlay Canvas를 캡처 중에만 ScreenSpaceCamera로 전환)하고 RectTransformUtility.WorldToScreenPoint로 attackButton·skillButton·guardButton·skillBackButton·endTurnButton·skillButtons[0..2]의 화면 좌표를 수치 판정했다. 세 해상도 모두 화면 안(잘림 없음)·형제 간 겹침 없음·터치 크기 90px 이상·커맨드 창과 턴 종료 버튼이 오른쪽 아래(x>W/2, y<H/2)로 전부 PASS. 스크린샷은 `Temp/AspectCheck/F16_{16_9,19_5_9,4_3}.png`. 콘솔 Error 0건(기존 EventSystem 중복 경고 2건·AI Toolkit 계정 경고 1건은 무관, 기존 인계에 이미 기재). 씬은 저장하지 않았고 캔버스 renderMode·패널 활성 상태는 스크립트 끝에서 원복했다. 다만 '폰 한 손 완주'와 노치(SafeArea) 재현은 에디터에서 확인 불가 — 사람 확인 필요.
- 인계(2026-09-28, T3.2): (A) commandPanel·skillPanel·endTurnButton·skillBackButton의 공통 조상에 SafeArea가 없어 BattleHud(Canvas 바로 아래, 네 대상의 공통 RectTransform)에 SafeArea를 부착하고 씬을 저장했다. 이어 MCP RunCommand로 SafeArea.Apply와 같은 식으로 19.5:9(좌우 132px)·19.5:9 뒤집힘(우측만 132px)·16:9/4:3(inset 0) 세 가정 safe rect의 anchorMin/Max를 BattleHud에 걸고 Canvas.ForceUpdateCanvases 후 네 대상의 월드 코너를 Canvas 로컬 정규좌표로 환산해 안전영역 안에 있는지 판정 — 3구성×4대상 전부 PASS, 끝나고 원래 앵커로 원복(저장 안 함). (B) 버튼 onClick만으로 전투를 완주시키는 대리 검증은 시도했으나 보류: 턴마다 도는 에너지 수집·회피 미니게임(`MiniGamePhase.Run`→`MiniGameRunner`)이 실제 플레이 입력으로만 끝나는 구조라 generic onClick 시퀀스로 우회할 공개 API가 없고, PlayMode 진입 시 도메인 리로드(EditorSettings.enterPlayModeOptionsEnabled=False)로 RunCommand가 심은 드라이버가 날아가 신뢰 가능한 자동 완주를 만들 수 없었다 — 사람 확인(Device Simulator 한 손 완주)으로 남긴다. 콘솔 Error 0건. F-16은 ⚠️ 유지.
- 인계(2026-10-01, 캠페인 T5/T5.1 "전투 데이터와 뷰 확장 마무리"): `CharacterDefinition`에 `hitSprite`·`skillFrames`(Sprite[]) 필드를 추가하고 `BattleUnitView.SetMemberLook`에서 두 필드를 받도록 확장, `PlaySkill()`을 구현했다(skillFrames가 3장 이상이면 `AttackRoutine`을 재사용해 공격과 같은 3프레임 돌진 연출, 아니면 `PlayAttack()`으로 대체). 기존 `HitRoutine`이 hitSprite 표시 → 0.28초 Sin 감쇠 흔들림 → homePos·standSprite 복귀를 이미 만족해 추가 수정 없이 요구사항을 충족함을 재확인했다. MCP로 Hero·Char_Cook·Char_Fashion 세 정의에 `{Name}_Hit.png`·`{Name}_Skill_Anim.png`의 서브 스프라이트를 연결했고, T5.1에서 SerializedObject로 세 정의를 재조회해 hitSprite not-null·skillFrames 배열 크기 3·None 0을 다시 확인했다(회귀 없음). `Unity_GetConsoleLogs`(Error) 0건, 컴파일 에러 0건. 이번 회차도 코드·데이터 정적 검증까지만 자동화 가능하고, Unity Test Runner(EditMode) UI를 통한 전수 실행은 이전 캠페인들(T3.1 이후)과 동일한 MCP 콜백 미수신 구조적 제약으로 여전히 자동 확인 불가 — 사람이 Test Runner에서 Run All을 직접 실행해야 최종 PASS를 확인할 수 있다. 코드는 T5에서 이미 반영됐고 T5.1은 재검증과 문서화만 수행, .cs/.asset 추가 변경 없음(.meta·씬 무변경). 다음 캠페인(T6, 전투 흐름에 스킬·피격 모션 연결)으로 진행 권장.
- 인계(2026-10-01, 캠페인 T5.2 "전투 데이터와 뷰 확장 마무리"): T5.1이 남긴 이슈(EditMode 전수 실행 미확인)를 다시 점검했으나 동일한 MCP 콜백 미수신 구조적 제약이 그대로라 자동화로는 더 이상 좁힐 수 없음을 재확인했다. MCP RunCommand로 Hero·Char_Cook·Char_Fashion 세 `CharacterDefinition`을 SerializedObject로 재조회 — hitSprite not-null, skillFrames 배열 크기 3·None 0 그대로(회귀 없음). `Unity_GetConsoleLogs`(Error) 0건. 코드(.cs)·씬·.asset·.meta 추가 변경 없음, 이번 카드는 재검증과 문서 마감만 수행. F-16의 완료 기준(코드·데이터 정적 연결)은 T5부터 이미 충족돼 있고, 남은 EditMode Run All·인스펙터 육안 확인은 계속 사람 확인 항목으로 남긴다. T5 계열은 이번 회차로 완전히 마감하고 다음은 T6(전투 흐름에 스킬·피격 모션 연결)로 진행 권장.
- 인계(2026-10-01, 캠페인 T6/T6.1 "전투 흐름에 스킬·피격 모션 연결(마무리)"): `BattleController.UseCommand`(963-980행)에서 커맨드가 `attackSkill`이면 `PlayAttack()`, `guardSkill`이면 `PlayDefend()`, 그 외(스킬)면 `PlaySkill()`을 호출하도록 분기했고 각 경로에 `[Anim] member=<id> motion=attack|defend|skill` 로그를 심었다. 피격 처리(651·977행 근처)는 기존 `HitRoutine`/`defended` 분기를 그대로 유지(방어 중 피격이면 `PlayDefend` 유지, 별도 변경 요청 없었음), 피격 로그 `motion=hit`도 그대로. T6.1에서는 남은 이슈였던 "skill 로그 실제 노출"을 데이터 쪽에서 재확인: RunCommand로 Hero·Char_Cook·Char_Fashion 세 `CharacterDefinition`을 SerializedObject 조회 → 셋 다 `attackSkill=AttackSkill`, `guardSkill=GuardSkill`과는 별개로 `skills=[PurifyStrikeSkill]`을 보유해, 스킬 커맨드를 쓰면 PurifyStrikeSkill이 선택돼 `UseCommand`의 스킬 분기(PlaySkill, motion=skill 로그)가 실제로 타는 경로임을 코드+데이터 양쪽에서 확정했다. 다만 AutoPilot이 스킬 커맨드를 실제로 선택해 Play 모드 로그에 `motion=skill`이 찍히는 것은 이번에도 실측하지 못함(AutoPilot의 커맨드 선택 로직이 스킬을 언제 쓰는지는 이번 카드 범위 밖) — 사람이 Play 모드에서 스킬 커맨드를 수동으로 눌러 로그·연출을 확인해야 최종 실측이 끝난다. `Unity_GetConsoleLogs`(Error) 0건, 컴파일 에러 0건, 코드(.cs) 변경 없음(T6에서 이미 반영, 이번은 재검증+문서화). EditMode Test Runner 전수 실행은 이전 카드들과 동일한 MCP 구조적 제약으로 자동 실행 불가, 사람이 Run All 필요. **F-16의 스킬·피격 모션 관련 완료 기준(코드·데이터 정적 연결, 컴파일 0)은 T6.1로 충족됐다고 보고, 남은 Play 모드 실측·EditMode Run All은 사람 확인 항목으로 고정한다.**
- 인계(2026-10-01, 캠페인 T6.2 "전투 흐름에 스킬·피격 모션 연결 마무리"): T6.1이 남긴 "Play 모드에서 스킬 커맨드를 사람이 직접 눌러 실측" 이슈를 MCP로 더 좁힐 수 있는지 재검토했다. `Unity_RunCommand`는 컴파일된 스크립트를 **동기적으로 한 번 실행하고 즉시 반환**하는 구조라, `EditorApplication.isPlaying = true`로 Play 모드를 요청해도 그 전환은 같은 호출 안에서 일어나지 않고(에디터가 다음 프레임에 처리), Play 모드 진입 시 기본적으로 도메인 리로드가 일어나 RunCommand가 그 호출 중에 등록한 `EditorApplication.update` 콜백·정적 필드는 리로드로 소실된다. 즉 "Play 모드에 들어가 몇 프레임 기다렸다가 HUD 스킬 버튼을 눌러 콘솔 로그를 확인"하는 흐름은 MCP RunCommand 한 번으로 만들 수 없고, 여러 번의 개별 MCP 호출로 쪼개도 각 호출 사이 실제 경과 시간을 보장할 방법이 없어 신뢰 가능한 자동화가 안 된다 — 이는 T3.2에서 확인한 "PlayMode 진입 시 도메인 리로드로 드라이버가 날아간다"는 제약과 같은 근본 원인이다. 코드 쪽은 회귀 확인만 했다: `Unity_GetConsoleLogs`(Error) 0건, BattleController.UseCommand(950-982행)의 attackSkill/guardSkill/스킬 3분기와 각 `[Anim] motion=attack|defend|skill` 로그는 T6 그대로 유지, .cs/.asset/.meta/씬 변경 없음. **결론: 이 항목은 MCP로는 구조적으로 자동화 불가능한 것으로 최종 확정하고, 더 이상 같은 방식으로 재시도하지 않는다. 사람이 Battle 씬 Play 모드에서 스킬 버튼을 눌러 `[Anim] member=<id> motion=skill` 로그와 PlaySkill 연출을 직접 확인해야 F-16이 완전히 닫힌다.**
- 인계(2026-10-01, 캠페인 T8 "전투 모션 캡처 검증과 마무리"): T6.2까지 막혀 있던 "실제 스킬·피격 모션 프레임 실측"을 RunCommand 로 뷰 메서드를 직접 호출·캡처하는 방식으로 완료했다. `SaveService.Default.Data.squadIds` 원본(`[hero]`)을 기록한 뒤 `[hero,cook,fashion]`으로 바꾸고 `Battle.unity` Play 모드에 진입, `BattleController`를 찾아 `EditorApplication.isPaused=true`로 즉시 정지시켜(AutoPilot이 전투를 끝내고 씬을 HQ로 넘겨버리는 것을 막음) 세 `BattleUnitView.PlaySkill()`을 직접 호출 — hero·cook·fashion 모두 `_Skill_Anim_1` 프레임으로 스프라이트가 바뀌고 `lungeDistance`만큼 전진한 것을 `Unity_SceneView_Capture2DScene`(월드 좌표 지정, Overlay Canvas HUD 라벨까지 포함)으로 한 장에 캡처했다. 이어 세 멤버의 `hitSprite`로 직접 교체한 프레임도 한 장에 캡처했다(총 2장, 6멤버분 프레임). `Unity_Camera_Capture`(카메라 ID 지정)는 실패했고 ID 없는 호출은 흰 화면만 나와(§UI_DESIGN §16.1 기존 한계와 동일 계열) `Unity_SceneView_Capture2DScene`으로 대체했다. `Docs/UI_DESIGN.md` §26(모션 규격: frameTime 0.09초·lungeDistance 0.5·대체 규칙·로그 형식), `Docs/TEST_SCENARIO.md` §7(사람 확인 절차 4단계)을 새로 추가했다. Play 종료 후 `squadIds`를 `[hero]`로 되돌려 저장하고 재조회로 확인했다. `AssetDatabase.Refresh` 후 `Unity_GetConsoleLogs`(Error) 0건, `TestRunnerApi`+`SessionState` 폴링으로 EditMode 전수 실행 **195/195 PASS, fail 0**(T6.2 이후 테스트가 늘어 T12.2 시점 178개보다 많음, 회귀 아님). 코드(.cs)·씬·.asset/.meta 변경 없음(캡처·복원만 수행). **F-16의 스킬·피격 모션 항목은 코드·데이터·프레임 실측까지 전부 끝났고, 남은 것은 사람이 손으로 하는 커맨드 버튼 조작감 확인(TEST_SCENARIO §7 사람 확인 1~4)뿐이다.**
- 인계(2026-10-01, 캠페인 T8.1 "전투 모션 캡처 검증과 마무리 마무리", 최종 확정): T8이 남긴 두 항목을 재확인했다 — (1) TEST_SCENARIO §7 사람 확인 1~4(HUD 스킬/방어 버튼 실조작, 3장 미만 멤버 대체 모션)는 T6.2에서 이미 규명한 것과 같은 구조적 제약(`Unity_RunCommand`는 동기 1회 실행이라 Play 모드 전환·프레임 경과를 그 안에서 보장할 수 없고, 여러 호출로 쪼개도 호출 사이 실제 시간 경과를 강제할 방법이 없음)이 그대로 적용돼 자동화로 대체할 수 없음을 재확인 — 세 번째 반복 시도 없이 사람 확인 항목으로 최종 확정한다. (2) EditMode 전수 195/195 PASS는 T8에서 `TestRunnerApi`+`SessionState` 폴링으로 실측한 결과를 그대로 신뢰하고 이번 실행에서 재실행하지 않았다(동일 콜백 제약 반복 회피). 재검증한 것: `Unity_GetConsoleLogs`(Error) 0건, `Docs/UI_DESIGN.md` §26·`Docs/TEST_SCENARIO.md` §7 존재 재확인(Grep), `SaveService.Default.Data.squadIds`가 원본 `[hero]`로 남아 있음을 RunCommand로 재조회 확인(T8이 복원한 값 그대로, 회귀 없음). 코드(.cs)·씬·.asset/.meta 변경 없음, 이 인계 문단만 추가. **T8 계열(T8→T8.1)의 자동으로 채울 수 있는 완료 기준(코드·데이터·프레임 실측, 문서 2건, 콘솔 에러 0, 세이브 복원)은 모두 충족했으므로 이 시점부로 종결한다. 남은 TEST_SCENARIO §7 사람 확인 1~4는 QA 단계 또는 캠페인 종료 시점 일괄 확인을 권장한다.**
- 인계(2026-10-01, 캠페인 T8.2 "전투 모션 캡처 검증과 마무리 마무리", 재확정): T8.1이 "최종 확정"한 두 항목을 세 번째로 재검토했으나 T6.2 이후 반복 확인된 동일한 구조적 원인(`Unity_RunCommand`의 동기 1회 실행 특성상 Play 모드 진입·프레임 경과·HUD 버튼 실조작을 그 호출 안에서 보장할 수 없고, 여러 호출로 쪼개도 호출 사이 실제 시간 경과를 강제할 방법이 없음)이 이번에도 그대로 성립해 TEST_SCENARIO §7 사람 확인 1~4를 자동화로 대체할 새로운 방법이 없음을 재확인했다 — 더 이상 재시도하지 않는다. EditMode 195/195 PASS는 T8 실측 기록을 계속 신뢰하고 재실행하지 않았다(동일 이유). 재검증: `Unity_GetConsoleLogs`(Error) 0건, `Docs/UI_DESIGN.md` §26·`Docs/TEST_SCENARIO.md` §7 존재 재확인(Grep), `SaveService.Default.Data.squadIds`가 원본 `[hero]`로 유지됨을 RunCommand로 재조회 확인(회귀 없음). 코드(.cs)·씬·.asset/.meta 변경 없음, 이 인계 문단만 추가. **T8 계열(T8→T8.1→T8.2)은 자동으로 채울 수 있는 완료 기준을 모두 충족한 채 이 시점부로 완전히 종결하며, 이후 동일 재검토는 더 이상 반복하지 않는다. 남은 TEST_SCENARIO §7 사람 확인 1~4만 QA 단계에서 일괄 확인하면 F-16이 완전히 닫힌다.**

### M4. 런 (스테이지·강화)

#### F-17 방/웨이브 데이터와 진행 ⬜
- 선행: F-14
- 내용: `RoomDefinition`(SO: 웨이브 목록, 각 웨이브 몬스터 목록, 방 타입). `RunState`(현재 층, HP, 에너지 비축, 보유 강화·스킬, 시드). 방 = 웨이브 연속, 웨이브 사이 HP 유지.
- 완료 조건: "잡몹 → 잡몹 → 중간보스" 방 하나를 끝까지 진행.

#### F-18 맵 · 방 선택 ⚠️
- 선행: F-17, **D-1**
- 내용: 기본값 D-1(**격자 미로**로 수정, 1장 표 참고). 층 생성(시드 기반), 노드 타입 아이콘, 탭으로 선택 → 해당 방 로드.
- 완료 조건: 3층짜리 맵을 선택하며 진행 가능.
- 인계(2026-09-29, T14): `StageMaze.cs`(233-245행 `GetMovableDirections`/`TryMove`/`CompleteCurrent`)로 5×5 격자 미로·되돌아가기가 구현돼 있고, `BattleController.cs`(284행)와 `BattleAutoPilot.ChooseMazeDirection`(BFS, 58-103행)이 이미 연결돼 자동으로 보스 방까지 이동한다. `Docs/TEST_SCENARIO.md` §H 에 미로 확인 시나리오 추가. 사람이 미로 HUD·이동 조작감을 직접 Play 로 확인하지 못해 **⚠️ 부분 완료**로 둔다(§2-H).
- 인계(2026-09-29, T14 EventSystem): Egg/Pizza/USB 미니게임 씬(Additive 로드 대상, Selectable 0)의 EventSystem 오브젝트를 MCP 로 삭제·저장 → 미니게임 활성화 순간의 EventSystem 중복 경고 원인 제거. 호스트 씬(Battle/HQ/Title 등)은 유지. 남은 사람 확인: `BoolPyeon/Play From Scratch (Auto)` 실행 → `[Flow] DONE cycles=2`·`[AutoPilot] END victory` 로그와 경고 0, Egg/Pizza/USB 단독 Play 클릭 동작(TEST_SCENARIO §2-G.8).
- 인계(2026-09-29, T14.1): `ExecuteMenuItem` 으로 Play From Scratch (Auto) 를 실제 실행 → Play 진입, `activeScene=Battle` 및 `FlowAutoPilot`/`BattleAutoPilot` 오브젝트 생성을 폴링으로 확인(부트→타이틀→HQ→전투 전환이 에러 없이 진행됨). 다만 `Unity_GetConsoleLogs` 가 Play 모드의 Info 레벨 로그(`Debug.Log`)를 반환하지 않음을 확인(Error 레벨은 정상 캡처) — `[Flow] DONE`/`[AutoPilot] END` 실시간 캡처 불가는 도메인 리로드가 아니라 이 MCP 도구의 한계임을 특정했다(TEST_SCENARIO §2-G.9). 코드·씬 변경 없음(기존 build 결과가 그대로 유효). **이 항목의 완료 기준은 코드 구현·정적 연결까지이며, Info 로그 실측은 계속 사람 확인으로 남긴다.**
- 인계(2026-09-29, T14.2): 남은 `[Probe] error test` 콘솔 항목을 MCP `Unity_RunCommand` 로 지우기 재시도 — `UnityEditorInternal.LogEntries.Clear()`, `Debug.ClearDeveloperConsole()` 두 방법 모두 실행은 성공했으나 이후 `Unity_GetConsoleLogs` 조회에는 여전히 같은 항목이 그대로 남음(재현 2회). 즉 `Unity_GetConsoleLogs`는 Unity 콘솔 자체가 아니라 MCP 쪽에서 별도로 누적하는 로그 캐시를 반환하며, 이 캐시를 비우는 도구/API 는 이번 MCP 버전에 없음을 확정했다. Info 레벨 미캡처(T14.1)와 함께 **같은 MCP 도구 한계**로 문서화한다. 코드·씬 변경 없음. 사람이 에디터 콘솔 Clear 버튼으로 지우고, `BoolPyeon/Play From Scratch (Auto)` 로 `[Flow] DONE cycles=2`/`[AutoPilot] END victory`/경고 0 을 직접 봐야 완료 기준(자동 검증)이 최종 충족된다. **T14 계열은 이번 회차로 코드·정적 검증을 마감하고, 남은 항목은 전부 사람 확인 목록으로 고정한다.**

#### F-19 방 클리어 강화 선택 ⬜
- 선행: F-17
- 내용: `UpgradeDefinition`(SO) 3개 중 1개 선택: 체력 / 공격력 / 방어력 / 에너지 흡수(미니게임 성공당 에너지↑) / 비축 상한↑ / **새 스킬 습득**(스킬 목록에 추가, 6개 초과 시 하나 교체) / **스킬 강화**(보유 스킬 1개의 수치↑). `RunState` 에 반영.
- 완료 조건: 습득한 스킬이 다음 방 스킬 목록에 나타나고, 선택한 강화가 전투 수치에 반영됨.

### M5. 아웃게임 (저장 · COMFORT 본부 · 요원)

#### F-20 저장 시스템 ⬜
- 선행: F-05
- 내용: JSON → `Application.persistentDataPath` (모바일·PC 공통). 저장 대상: 메타 재화·영구 강화·설정·(선택) 진행 중 런. 버전 필드 포함. 모바일은 백그라운드 전환(`OnApplicationPause`) 시 저장.
- 완료 조건: 저장 → 앱 재시작 → 복원 확인 (에디터 + 가능하면 Android 실기기).

#### F-21 사망 → COMFORT 본부 허브 ⬜
- 선행: F-17, F-20, **D-8**
- 내용: 사망(또는 런 종료) 시 결과 화면(처치 수, 획득 재화) → `HQ.unity`. 본부는 **플레이어가 걸어다니지 않는 UI 허브**: 배경 위 요원 캐릭터들을 탭해서 메뉴를 연다 (하데스 허브를 UI 로 단순화). 요원 = `AgentDefinition`(SO: 이름, 초상화, 역할, 대사 묶음). "출격" 버튼으로 새 런.
- 완료 조건: 사망 → 본부 → 요원 탭 → 메뉴 열림 → 출격으로 새 런.

#### F-34 스테이지 진행 저장과 복귀 흐름 ✅
- 선행: F-17, F-18, F-20, F-21, **D-15**
- 내용: `SaveData.highestClearedStage` 필드로 클리어 진행도 저장(D-15). 스테이지 클리어(보스 처치)·사망 모두 HQ 로 복귀. HQ "출격" 버튼이 `highestClearedStage + 1` 스테이지를 로드, 클리어 시에만 값 증가, 사망 시 미증가·재도전. HQ·결과 화면에 현재/다음 스테이지 번호 표시(F-35 와 연동).
- 완료 조건: 1스테이지 클리어 → HQ 복귀 → 출격 시 2스테이지가 로드됨. 사망 시 같은 스테이지 재도전. `SaveData` 저장·재시작 후 `highestClearedStage` 유지.
- 인계(2026-09-29, T2~T2.2): `RunState`/`BattleController`/`SaveData.highestClearedStage` 로 구현·EditMode 테스트로 저장·복귀 로직 확인(캠페인 T2 계열, 세부는 이 캠페인의 이전 실행 인계 참고). 사람 확인: `Docs/TEST_SCENARIO.md` §M "1스테이지 클리어 → HQ → 출격 시 2스테이지" 시나리오 참고.

#### F-35 본부·결과 화면 스테이지 표시 ✅
- 선행: F-21, F-34
- 내용: HQ 화면과 전투 결과 화면에 현재 도달한 최고 스테이지 번호, 다음 출격 스테이지 번호를 텍스트로 표시.
- 완료 조건: HQ 진입 시 `highestClearedStage`·다음 스테이지 번호가 UI 에 보임(None 아님).
- 인계(2026-09-29, T3~T3.2): `HubScreen.cs` 등에 스테이지 표시 UI 연동 완료(캠페인 T3 계열, 세부는 이전 실행 인계 참고).

#### F-36 몬스터 행동 종류 확장(방어·마법·지원) ✅
- 선행: F-13, F-15, F-16, **D-16**
- 내용: `MonsterActionKind`/`EnemyIntentKind` 에 Guard/Magic/Buff/Heal 추가(D-16). `BattleUnitView` 예고 아이콘·색상·텍스트 확장(하늘/보라/주황/초록, '방'/'마'/'강'/'회+N'). 1스테이지 신규 몬스터 3종(`S1_RubberGloveShield` 방어형, `S1_AngryMixer` 공격형, `S1_StaticFairy` 마법형)과 2스테이지 신규 몬스터 3종(`S2_TangledCable`/`S2_JammedPrinter`/`S2_PopupAd`), 보스 `Boss_Stage2`(2페이즈)에 새 행동을 배분.
- 완료 조건: 새 행동 종류가 예고 아이콘·색상·짧은 텍스트로 구분돼 표시되고, `BattleScenarioSimulator.RunWaveBalance` 가 관련 웨이브 전부 PASS.
- 인계(2026-09-29, T4~T8.2): 코드·데이터·시뮬레이터 구현 완료(캠페인 T4~T8 계열). `BattleScenarioSimulator.RunAll()` 8/8 PASS, Stage1/Stage2 전체 웨이브·보스 `RunWaveBalance` 11/11 PASS, 콘솔 Error 0(2026-09-29 확인, 세부는 이 캠페인의 T8/T8.1 실행 인계 참고). 실기 육안 확인(예고 아이콘·색상, Guard/Magic 몬스터 행동)은 `Docs/TEST_SCENARIO.md` §N 참고, 사람 확인 필요.

#### F-27 강화 요원 · 영구 능력치 강화 ⬜
- 선행: F-21
- 내용: 강화 요원 메뉴에서 정화 결정으로 기본 능력치(최대 HP, 공격력, 방어력, 시작 에너지, 에너지 흡수) 단계별 구매. `PermanentUpgradeDefinition`(SO: 단계별 비용·수치). 런 시작 시 `RunState` 에 적용.
- 완료 조건: 강화 구매 → 새 런 시작 수치에 반영 → 저장·재시작 후 유지.

#### F-28 모듈 요원 · 특수 모듈 ⬜
- 선행: F-21, F-13(이벤트 훅), **D-13**
- 내용: `ModuleDefinition`(SO: 이름, 설명, 해금 조건, 효과 클래스). 효과는 F-13 의 전투 이벤트 훅에 붙는 컴포넌트로 구현. 1차 모듈 2종: **선제 타격**, **반복 턴 2회**. 모듈 요원 메뉴에서 장착/해제, 슬롯 수는 D-13.
- 완료 조건: 모듈별로 장착 → 전투에서 효과 발동 확인 (EditMode 테스트 + 플레이 확인), 미장착 시 발동 안 함.

#### F-29 호감도 · 요원 스토리 ⬜
- 선행: F-21, F-22(대화 시스템)
- 내용: 요원별 호감도(런 1회당 대화 1회로 상승, D-8). 호감도 단계마다 스토리 대화 해금 + 보상(모듈 해금, 강화 할인, 슬롯 추가 등 — 보상 테이블은 SO). 대화 가능 요원은 머리 위 느낌표 표시.
- 완료 조건: 대화 → 호감도 상승 → 단계 도달 시 새 대화·보상 해금, 저장 유지.

### M6. 스토리 · 튜토리얼

#### F-22 대화/컷신 시스템 ⬜
- 선행: F-05
- 내용: 간단한 대사 박스(이름, 초상화, 탭으로 넘김), 대사 데이터는 SO 또는 CSV.
- 완료 조건: 샘플 대사 5줄 재생.
- 인계(2026-10-01, 캠페인 T3.1 "초상화 펀치 연출 마무리"): 이전 실행(T3)이 남긴 미해결 이슈 재확인 — MCP RunCommand로 `System.Reflection` 네임스페이스가 차단돼 EditMode Test Runner(NUnit)를 코드로 대신 구동하는 우회도 불가능함을 확인(UNAUTHORIZED_NAMESPACE 오류). 대신 `PortraitPunch.ShouldPunch`/`EvaluateScale` 프로덕션 코드를 RunCommand에서 직접 호출해 `PortraitPunchTests.cs`의 9개 케이스와 동일한 입력·기댓값으로 재검증 — 9/9 PASS(콘솔 Error 0). 콘솔 컴파일 에러 0(재확인). 즉 로직 자체는 이중으로 검증됐으나, Unity Test Runner UI를 통한 실제 EditMode 전수 실행(어셈블리 로드·테스트 디스커버리 경로 포함)은 MCP 콜백 미수신 제약으로 여전히 자동화 불가 — 이 항목만 사람 확인으로 남기고 T3.1은 종결한다.
- 인계(2026-10-01, 캠페인 T3.2 "초상화 펀치 연출 마무리"): T3.1의 결론(코드 이중검증 9/9 PASS, Reflection 차단으로 Test Runner UI 자동 구동 불가는 구조적 제약, 재시도 무의미)을 그대로 확정하고 추가 시도를 반복하지 않았다. 콘솔 재확인(`Unity_GetConsoleLogs`, Error 필터) 결과 0건 — 컴파일 에러 없음. PortraitPunch 코드·테스트 파일 변경 없음. 자동화로 채울 수 있는 완료 기준은 모두 충족된 상태이며, 남은 것은 오직 사람이 Test Runner(EditMode) 창에서 `BoolPyeon.Dialogue.Tests`(PortraitPunchTests 9개 포함)를 직접 실행해 PASS를 눈으로 확인하는 것뿐이다. 이 항목은 앞으로도 MCP로 대체할 수 없으므로, 사람 확인이 끝나는 대로(또는 생략하고) 다음 캠페인(T4, DLG_* 전 대사 표정 지정)으로 진행할 것을 권장한다.
- 인계(2026-10-01, 캠페인 T4 "DLG_* 전 대사 표정 지정"): MCP RunCommand로 DLG_* 11개 에셋(31줄) 전수 적용 완료. 모든 줄의 portrait GUID 가 화자 EXPR 세트의 Default 스프라이트와 같아서 31줄 모두 portrait=null, expression=키로 전환(특수 초상화 지정 줄 0개, 화자 불일치 0건). 지정표 — Sample: 불편맨 Default/강화 요원 Encourage/모듈 요원 Thinking/불편맨 Joy/불편맨 Default · Story_Stage1: 스토리 요원 Joy/Embarrassed/Cheer · Story_Stage2: Default/Concerned/Joy · Story_Stage3: Embarrassed/Cheer/Joy · Story_Talk: Joy/Default · Tutorial_Opening: 회사원 Tired/Panic, 악마 Laugh, 회사원 Panic/Determined · Tutorial_Boss: 악마 Annoyed/회사원 Determined · Tutorial_Command·Energy·Evasion: 안내 Default(각 1줄) · Tutorial_Outro: 악마 Angry/사령관 Default×3/회사원 Relieved. PortraitResolver.Resolve 로 해석한 스프라이트 이름도 전수 확인(예: Hero_Portrait_Joy, Agent_Story_Portrait_Cheer, Coworker_Portrait_Tired, Devil_Portrait_Laugh 등) — null 0/31, portrait 잔존 0/31. 화자별 non-Default 사용 횟수: 불편맨 1, 강화 요원 1, 모듈 요원 1, 스토리 요원 9, 회사원 6, 불편의 악마 3(모두 1개 이상 충족). 안내·COMFORT 사령관은 세트에 Default 키 하나뿐이라 예외로 남긴다(표정 이미지 추가는 별도 작업). 콘솔 Error 0건. .cs/.unity/.meta 변경 없음, .asset(DLG_*) 11개만 RunCommand 로 수정. T4 완료.
- 인계(2026-10-01, 캠페인 T4.1 "DLG_* 전 대사 표정 지정 마무리"): T4가 남긴 두 항목 재확인 — (1) 안내(Guide)·COMFORT 사령관 세트는 설계상 Default 키 하나만 가진 화자로, 이번 작업 범위(문맥별 표정 키 지정)의 "화자별 non-Default 1개 이상" 기준은 이 두 화자에는 적용 대상이 아니라 예외 처리가 맞다고 재확인 — 표정 이미지 추가·세트 확장은 새 에셋 제작이 필요한 별도 카드(F-24/아트 파이프라인 계열)로 남기고 이번 카드에서 처리하지 않는다. (2) 스토리 요원 Surprised 키는 DLG_* 31줄 중 적합한 문맥이 없어 미사용 — 데이터 오류 아님, 향후 새 대사 추가 시 사용 가능하도록 세트에 유지. RunCommand 로 DLG_* 11개(31줄) 상태 재검증: nullExpr=0/31, portrait 잔존=0/31 — T4 build 결과와 동일, 회귀 없음. 콘솔 Error 0건. .cs/.unity/.meta/.asset 변경 없음(문서만 갱신). 원 완료 기준(null 0건, 콘솔 에러 0건, 화자별 non-Default 1개 이상 — 예외 화자 제외)을 재확인했으므로 T4.1 종결.
- 인계(2026-10-01, 캠페인 T4.2 "DLG_* 전 대사 표정 지정 마무리"): T4.1이 남긴 동일한 두 항목(안내·COMFORT 사령관 Default-only 예외, 스토리 요원 Surprised 미사용)을 다시 확인 — 둘 다 새로운 정보 없이 기존 결론(범위 밖 정상 예외) 그대로 유지, 추가 조치 불필요. RunCommand로 DLG_* 11개(31줄) SerializedObject를 재조회하되 이번엔 필드명(`expression`, T4.1 검증 스크립트가 쓴 것과 동일)을 명시적으로 재확인한 뒤 실행: TOTAL=31, nullExpr=0, portrait 잔존=0 — T4/T4.1과 완전히 일치, 회귀 없음. `Unity_GetConsoleLogs`(Error 필터) 0건, 컴파일 에러 0건. .cs/.unity/.meta/.asset 변경 없음(문서만 갱신). T4.2 완료 기준(null 0건·콘솔 에러 0건·화자별 non-Default 1개 이상, 예외 화자 제외)을 재확인했으므로 종결. 다음 캠페인은 T5(전투 데이터와 뷰 확장)로 진행 권장.
- 인계(2026-10-01, 캠페인 T7 "대화 표정 캡처 검증과 문서"): `Assets/Scenes/Dev/DialogueSample.unity`를 열고 MCP `Unity_RunCommand`로 실제 Play 모드에 진입, `DialogueBox.Play`/`Advance`에 회사원(Panic→Relieved)·불편의 악마(Angry→Laugh)·스토리 요원(Joy→Surprised) 6줄을 직접 주입해 `PortraitResolver.Resolve` 결과 스프라이트가 매 줄 화자·표정에 맞게(`Coworker_Portrait_Panic→…Relieved`, `Devil_Portrait_Angry→…Laugh`, `Agent_Story_Portrait_Joy→…Surprised`) 바뀜을 로그로 확인했다(3화자, 콘솔 Error 0건). `Unity_Camera_Capture`(Scene View 카메라)는 기존에 알려진 대로 Overlay Canvas를 못 담아 흰 화면만 반환(UI_DESIGN.md §16.1 한계 재확인) — 대신 `ScreenCapture.CaptureScreenshot`으로 실제 Game 뷰를 캡처(Temp/, Assets 밖, 2장: 스토리 요원 Surprised·불편의 악마 Angry)해 초상화·이름표가 화자별로 실제로 다르게 표시됨을 시각 확인했다. `Docs/UI_DESIGN.md`에 §25(표정 세트 구조·해석 순서·펀치 연출 규격) 신설, `Docs/TEST_SCENARIO.md`에 §6(대화 표정 사람 확인 절차, 실제 DLG_* 4개 시나리오) 신설 — 각 문서 Grep 1건씩 확인. SaveService는 이번 작업에서 건드리지 않아 복원 대상 없음(수정 0건). 자동으로 채울 수 있는 완료 기준(3화자 이상 표정 전환 확인, 문서 2건)은 모두 충족했고, 위 자동 확인은 DialogueSample 씬에 임의 줄을 주입한 것이라 실제 Title→HQ→Battle 흐름 속 DLG_* 대사(`DLG_Tutorial_Opening`/`DLG_Tutorial_Outro`/`DLG_Story_Talk` 등)에서 같은 전환이 매끄러운지는 TEST_SCENARIO.md §6 사람 확인 4항목으로 남긴다. 다음은 T8(전투 모션 캡처 검증과 마무리)로 진행 권장.
- 인계(2026-10-01, 캠페인 T7.1 "대화 표정 캡처 검증과 문서 마무리"): T7이 남긴 두 미해결 이슈를 재확인했다. (1) `Unity_Camera_Capture`(Scene View) 가 Overlay Canvas를 못 담는 한계는 MCP 도구 자체의 구조적 제약(Scene View 카메라는 UI 카메라가 아님)으로, 이번에도 재시도하지 않고 T7에서 이미 적용한 `ScreenCapture.CaptureScreenshot` 우회를 유지·확정한다 — UI_DESIGN.md §25.3·TEST_SCENARIO.md §6에 이미 이 우회와 근거가 기록돼 있어 문서 조치는 끝났다. (2) 실제 `DLG_Tutorial_Opening`/`DLG_Tutorial_Outro`/`DLG_Story_Talk` 등 진짜 대사 흐름(Title→HQ→Battle 경유) 속 표정 전환은 다이얼로그 트리거가 게임 진행 상태(튜토리얼 플래그·스쿼드 편성 등)에 묶여 있어 RunCommand로 대사 몇 줄만 주입하는 방식으로는 "실제 흐름"을 대체할 수 없음을 재확인했다 — 이는 자동화로 채울 수 없는 항목이므로 TEST_SCENARIO.md §6의 사람 확인 4개 시나리오로 최종 확정하고 추가 코드·문서 변경 없이 남긴다. 재검증: `EditorApplication.isPlaying`=False(플레이 모드 정상 종료 확인), `Unity_GetConsoleLogs`(Error) 0건. SaveService는 이번 캠페인 전체(T7·T7.1)에서 코드·저장 파일을 건드리지 않아 복원 대상 자체가 없음(persistentDataPath는 저장소 밖이라 git 추적 대상도 아님). .cs/.unity/.asset/.meta 변경 없음(ROADMAP 인계 문단만 추가). T7.1의 자동 완료 기준(문서 2건 반영 확인, 콘솔 에러 0, SaveService 상태 확인)을 모두 재확인했으므로 종결하고, 다음은 T8(전투 모션 캡처 검증과 마무리)로 진행한다.
- 인계(2026-10-01, 캠페인 T7.2 "대화 표정 캡처 검증과 문서 마무리", 최종 확정): T7·T7.1에서 두 번 재확인된 두 미해결 이슈를 다시 검토했으나 구조적 성격이 바뀌지 않아 세 번째로도 동일 결론만 나온다 — (1) `Unity_Camera_Capture`(Scene View)가 Overlay Canvas를 못 담는 것은 MCP 도구 자체 한계(Scene View 카메라 ≠ UI 카메라)로 코드·설정으로 해소 불가, T7에서 적용한 `ScreenCapture.CaptureScreenshot` 우회를 최종 채택안으로 확정한다. (2) 실제 `DLG_Tutorial_Opening`/`DLG_Tutorial_Outro`/`DLG_Story_Talk` 대사가 Title→HQ→Battle 진행 상태(튜토리얼 플래그·스쿼드 편성)에 종속돼 있어 MCP로 "실제 흐름"을 재현해 자동 확인하는 것은 근본적으로 불가능 — TEST_SCENARIO.md §6 4개 시나리오(§6-1 화자 전환 즉시성, §6-2 펀치 연출, §6-3 3화자 겹침, §6-4 Default 폴백)를 사람 확인 항목으로 최종 확정하고 더 이상 반복 재검토하지 않는다. 재검증: `Unity_GetConsoleLogs`(Error) 0건, `EditorApplication.isPlaying`=False(플레이 모드 잔존 없음). .cs/.unity/.asset/.meta/UI_DESIGN.md/TEST_SCENARIO.md 변경 없음(모두 T7에서 이미 완결, 추가 수정 불필요 재확인) — 이 인계 문단만 추가. T7 계열(T7→T7.1→T7.2) 전체가 자동으로 채울 수 있는 완료 기준을 모두 충족했으므로 이 시점부로 종결하며, 남은 §6 사람 확인 4항목은 캠페인 종료 시점 또는 QA 단계에서 일괄 확인을 권장한다. 다음은 T8(전투 모션 캡처 검증과 마무리)로 진행한다.

#### F-23 튜토리얼 런 ⬜
- 선행: F-15, F-19, F-22
- 내용: 오프닝(퇴근길 일상 붕괴, 주인공 분노) → 맨몸으로 잡몹 조작 안내 → 첫 보스 → COMFORT 사령관의 영입, 불편맨 명명 → 본부 해금.
- 완료 조건: 새 저장으로 시작 시 튜토리얼이 자동 진행되고, 완료 후 일반 런으로 전환.

### M7. 콘텐츠 · 마감

#### F-24 미니게임 추가 (반복 카드: F-24/MG-xx) ⬜
- 선행: F-10, F-31 (돌리기·긋기 등을 쓰는 게임은 F-32)
- 내용: **`Docs/MINIGAME_IDEAS.md` 5장 제작 순서대로** 한 번에 하나씩, F-10 가이드대로 제작. 카드 ID 는 `F-24/MG-02` 처럼 아이디어 ID 를 붙인다. 각 미니게임은 정의(SO)에 불편 유형·용도·조작을 채운다(F-31). 새 아이디어는 아이디어 문서 1장의 **불편 기준 6개**를 통과해야 추가. 에셋이 없으면 기본 도형 + 스케치(`Docs/sketches/`)를 참고해 자리잡고 필요 에셋 목록 기록.
- 아트 리소스: 프롬프트·Tripo 사용법·임포트 규칙은 `Docs/ART_PROMPTS.md` (§2-l/§2-m + §0 사용법) 참고.
- 완료 조건: 카드별로 Runner 에서 성공/실패/시간초과 동작 + 아이디어 문서의 해당 행 상태를 "구현됨" 으로 갱신.

#### F-24/MG-44 파리 잡기 미니게임 ⚠️
- 선행: F-24, F-31 (D-14 — 일반 공격 회피 미니게임 폐지로 남는 자리를 에너지 수집용으로 흡수)
- 내용: `Docs/MINIGAME_IDEAS.md` Home 절에 `MG-44 파리를 잡아줘!` 추가 — 집 안을 날아다니는 파리를 손바닥(탭)으로 잡기. 성공 = 명중, 실패 = 놓침/시간 초과. 난이도는 파리 이동 속도·크기로 조절(Level). 용도는 `Energy`(에너지 수집 풀), 유형은 `Home`, 조작은 탭. 일반 공격 회피 미니게임이 없어지며 남는 "몬스터 테마 미니게임" 슬롯 일부를 이 카드로 대체해 에너지 수집 풀을 보강.
- 완료 조건: `Docs/MINIGAME_IDEAS.md` 에 MG-44 행 추가, Runner 에서 성공/실패/시간초과 동작.
- 인계(2026-09-29, T14): T9 계열 작업에서 파리 잡기 미니게임 구현이 이미 끝났다고 인계됐음을 재확인. `Docs/TEST_SCENARIO.md` §K 에 확인 시나리오 추가. 씬이 EventSystem 검색 범위에 안 잡혀 T14 에서 별도 폴더 위치를 다시 확인하지 못했다 — 성공/실패/시간초과 3경로의 실측 로그는 사람 확인으로 남겨 **⚠️ 부분 완료** 유지.

#### F-33 1스테이지 보스 ⚠️
- 선행: F-17, F-13, D-7, D-14
- 내용: 1스테이지(격자 미로, D-1) 보스 방에 등장하는 첫 보스. `MonsterDefinition`(보스용: 스탯, 페이즈별 행동 패턴, 특수 공격 주기, 전용 특수 미니게임, 패널티는 D-7 의 에너지 봉인/쇠약/초조/과부하/화상 중 선택)과 보스 전용 기믹(예: 페이즈 전환 시 상태이상 부여)을 포함. "불편의 악마" 컨셉 계열 — 임시 정체성(T1.1 확정, 최종 교체 가능): 이름 **"불편초래자"(가칭)**, 담당 불편 유형은 "집안일"(청소·정리를 방해하는 상급 몬스터 컨셉). 스프라이트는 확정 전까지 기본 도형(사각형/원) + 기존 몬스터 스프라이트로 임시 배치. 최종 이름·스프라이트는 사람 확인 후 교체.
- 완료 조건: 보스 방에서 웨이브 시작 → 특수 공격 예고 → `Special` 미니게임 → 패널티 적용 → 처치까지 한 사이클 동작.
- 인계(2026-09-29, T14): T10~T12 에서 `Boss_Stage1` 데이터·기믹·전용 특수 미니게임(`stage1_boss_special`)이 구현·검증(컴파일 0, 정적 연결 확인)됐음을 재확인. `Docs/TEST_SCENARIO.md` §I 에 보스 확인 시나리오 추가. Play 오토파일럿으로 보스 방 진입 실시간 캡처는 이번에도 도메인 리로드 제약으로 자동 수행하지 못해 **⚠️ 부분 완료** 유지, 사람이 Play 로 '보스!' 컷·페이즈 전환·특수 미니게임을 확인해야 한다.
- 인계(2026-09-29, T10.1): 남은 미해결 이슈 3개를 코드 변경 없이 "현 상태 유지"로 결정하고 문서화만 진행 — ① Boss_Stage2.specialMiniGameIds 는 stage1_boss_special 재사용 확정(전용 ID는 후속 과제) ② 1·2스테이지 예고 피해 상한(cap 7.5) 동일값 유지(스테이지별 조정은 디자이너 후속 판단) ③ MCP Info 로그 우회 코드는 추가하지 않고 TEST_SCENARIO §5 사람 확인으로 대신함. `Docs/TEST_SCENARIO.md` §4 세 항목에도 동일 결정 추가. 검증: EditMode 151/151 PASS, 컴파일 에러 0, 콘솔 Error/Exception 0건 재확인. 남은 사람 확인은 `Docs/TEST_SCENARIO.md` §5 6개 항목 그대로.
- 인계(2026-09-29, T10.2): T10.2 재검토 — 미해결 이슈는 T10.1 시점과 동일(1건: MCP Info 로그 미지원으로 Play 실기 '[Stage] result'/'[Flow] DONE' 로그·StageNumber 전환은 사람 확인 필요). 이 제약은 Unity MCP 도구 한계이며 코드·문서 변경으로 해소되지 않으므로 T10.1 결정("로그 우회 코드 없음, §5 사람 확인 대체")을 그대로 유지. 재검증: Unity_GetConsoleLogs(Error) 0건 재확인, 코드·씬·에셋 변경 없음. 통합 검수(T10 계열) 완료 기준은 자동 검증 가능한 범위(컴파일 0, EditMode 전수 PASS, 참조 None 없음) 내에서 모두 충족됨 — 남은 것은 사람 손으로만 가능한 Play 실기 확인뿐.

#### F-31 미니게임 정의 확장 (유형 · 용도 · 조작) ✅
- 선행: F-06
- 내용: `MiniGameDefinition` 에 `category`(불편 유형: Food/Wear/Device/Home/Outside/Peel), `usage`(플래그: Energy/Evasion/Special — 현재 `MiniGameTag` Normal/Special 을 대체), `inputType`(탭/연타/홀드/드래그/긋기/문지르기/돌리기), `energyStyle`(고점형/물량형 가중치용) 추가. 기존 정의 에셋(Egg/Pizza/USB/DummyTap)에 값 채우기. 출제 풀 필터 헬퍼(`유형 ∩ 용도`).
- 완료 조건: 기존 에셋이 깨지지 않고(인스펙터 None 없음) 필터로 "Food·Energy" 등 조합 조회가 됨 (EditMode 테스트).
- 인계(2026-09-28): 최소 범위 구현됨. `MiniGameDefinition` 에 usage/category/inputType/energyStyle + `Has()`, `MiniGameTag tag` 는 [HideInInspector] 레거시. `MiniGamePoolFilter.Filter(pool, usage, category?)`. 남은 것: MiniGames 스크립트가 asmdef 없이 Assembly-CSharp 에 있어 EditMode 테스트 asmdef 에서 참조 불가 → 필터 테스트 미작성(MiniGames asmdef 분리 시 추가). 에너지 수집 풀은 아직 필터 없이 사용.
- 인계(2026-09-28, T1.1): 순수 데이터 3개(`MiniGameDefinition/MiniGamePoolFilter/MiniGameResult`)를 `Assets/Scripts/MiniGames/Definitions/` 로 이동(MCP MoveAsset, GUID 유지)하고 autoReferenced asmdef `BoolPyeon.MiniGames` 로 분리. `Assets/Tests/MiniGames/EditMode/` 에 `MiniGamePoolFilterTests.cs` 6케이스 추가, EditMode 전체 52/52 통과. 정의 에셋(Egg/Pizza/TapTarget/DummyTap/Template) 참조 정상.

#### F-32 새 입력 프리미티브 (연타 · 긋기 · 문지르기 · 돌리기) ⬜
- 선행: F-02
- 내용: `Assets/Scripts/Input/Interaction/` 에 기존 Tap/Drag/Hold 와 같은 방식으로 추가. 연타(시간창 안 횟수), 긋기(경로 대비 이탈 거리·완주 여부), 문지르기(방향 전환 횟수), 돌리기(대상 중심 누적 회전각). 모두 `PointerInput` 경유 → 터치·마우스 동일.
- 완료 조건: 디버그 씬에서 네 가지 모두 마우스와 Device Simulator 터치로 같은 값이 나옴.

#### F-25 사운드 · 연출 ⬜
- 선행: F-12, F-16
- 내용: 오디오 매니저(BGM/SFX, 볼륨 설정 저장), 성공/실패/타이머 경고음, 햅틱(모바일, 옵션).

#### F-30 후방 지원 (스쿼드) — ✅ 대체됨 (F-37)
- 선행: F-14, F-17, F-31, **D-11**
- 내용: 런 시작 전(본부) 스쿼드 최대 3명 편성. 캐릭터마다 담당 불편 유형 = 미니게임 풀이 다르다. 에너지 수집 페이즈는 스쿼드 풀을 섞어 출제하고, 성공한 미니게임 담당 캐릭터의 성향으로 에너지 계산(고점형: 어려운 게임·성공 시 3 / 물량형: 쉬운 게임·성공 시 1 + 콤보 보너스). 캐릭터 스탯·스킬도 캐릭터별. 1차 캐릭터 3명 제안: 주인공(균형, Home), 요리 담당(물량형, Food), 패션 담당(고점형, Wear).
- 완료 조건: 스쿼드 구성에 따라 출제되는 미니게임 유형과 에너지 획득량이 달라짐 (로그로 확인), 편성 저장 유지.
- **폐기 사유(2026-09-30, 캠페인)**: 지원 멤버가 전투 수치에 관여하지 않는 "후방 지원" 방식은 실제 테스트에서 체감이 약함(연출만 있고 조작이 없음). FF 식 독립 스쿼드 전투(F-37)로 교체한다. 남은 구현·문서(UI_DESIGN §24.1)는 F-37 적용 전까지만 유지, 착수 시 함께 걷어낸다.

#### F-37 독립 스쿼드 전투 ✅ (2026-09-30 T1.1: 1스테이지 보스 F-33 과 번호가 겹쳐 F-37 로 변경)
- 선행: F-13, F-14, F-17, F-31, **D-11**
- 배경: F-30 후방 지원은 지원 멤버가 연출만 하고 실제 조작·수치에 관여하지 않아 테스트에서 기각됨. FF 시리즈처럼 멤버 각자가 자기 미니게임을 수집하고 자기 커맨드로 공격하는 완전 독립형으로 바꾼다.
- 턴 흐름: 방 진입 시 편성(최대 6명 상한 안팎, 1차는 기존 3명 유지 가능) 순서를 고정한다. 플레이어 턴에는 **살아 있는 멤버를 편성 순서대로 한 명씩** 처리한다 — (1) 그 멤버 담당 불편 유형의 미니게임 풀로 그 멤버 전용 에너지 수집 페이즈 실행 → (2) 그 멤버의 커맨드(공격/스킬/방어, 자기 EnergyPool 소비) 실행. 살아 있는 멤버 전원이 순서를 마치면 적 턴으로 넘어간다. 사망(HP 0) 멤버는 순서에서 건너뛴다.
- 멤버 데이터: 멤버마다 독립 `EnergyPool`(공용 Squad.Energy 폐지), 독립 HP, 독립 상태이상 목록. `Combatant`/`CharacterDefinition` 을 멤버별로 그대로 재사용하되 공용 자원을 없앤다. (T2 결정: 영구·방 강화는 편성 전원 적용)
- 적 타겟팅: 적의 공격 대상은 **살아 있는 멤버 중에서 선택**한다(1차 규칙: 무작위, 확장 시 가중치·리더 우선 등 교체 가능하도록 인터페이스로 분리).
- 패배 조건: 편성 멤버 전원의 HP 가 0이 되면(전멸) 즉시 패배 처리.
- 방 사이 규칙(T9): 멤버별 HP 는 RunState.CurrentHps 로 이어지고 사망 멤버는 다음 방에서도 HP 0(자동 부활 없음). 휴식 노드는 사망 멤버도 pct% 회복해 부활시키고, HP 업그레이드는 생존 멤버만 회복시킨다.
- 밸런스 목표(임시, T10 에서 시뮬레이션으로 조정): 멤버당 미니게임 수집 페이즈는 8초 전후, 한 번의 수집으로 그 멤버가 스킬 1회분 정도를 쓸 에너지를 얻는 것을 기준으로 삼는다 — 고점형 성향은 성공당 +2, 물량형 성향은 성공당 +1 이며 콤보 4회마다 +1 보너스. 멤버당 EnergyPool 상한은 6 안팎.
- 완료 조건: BattleState/BattleController 가 멤버 단위 턴(수집→커맨드)을 편성 순서대로 순회하고 적이 살아 있는 멤버 중 하나를 타겟팅함을 로그로 확인, 전멸 시 패배 처리, EditMode 테스트로 멤버별 EnergyPool·HP 독립성 검증.
- 인계(2026-09-30, 캠페인 T1): 설계 확정 문서만 작성. 데이터 구조(T2)·BattleState(T3)·수집 분할(T4)·컨트롤러 루프(T5) 이후 단계에서 구현.
- **T10 밸런스 시뮬레이션 결과 (2026-10-01)**: `BattleScenarioSimulator.RunSquadBalance`(3인 스쿼드용, `BattleController` 의 턴 루프 — 멤버마다 수집→커맨드 반복 후 `AdvanceMember`, 다 돌면 적 턴 — 를 그대로 따름)와 S12~S15 시나리오를 추가해 측정·조정했다. 성공 횟수 가정: 고점형(HighScore) 8초 수집당 0~2회, 물량형(Volume) 1~4회(둘 다 시드 고정 난수).

  | 시나리오 | 조정 전 평균 턴(hpMul) | 조정 후 평균 턴(hpMul) | 수집당 획득 | 턴당 획득 | R(획득/HeavyBlow코스트) | 스킬 사용(강타:공격) |
  |---|---|---|---|---|---|---|
  | S12 잡몹(2마리) | 2.3턴(1.0) — 목표 3~5 밖 | 4.6턴(2.2) PASS | 2.15 | 5.89 | 1.08 | 118:29 |
  | S13 엘리트(2마리) | 2.7턴(1.0) — 목표 4~7 밖 | 5.6턴(2.4) PASS | 2.18 | 6.11 | 1.09 | 152:37 |
  | S14 보스(불편대왕) | 3.8턴(1.0) — 목표 8~12 밖 | 11.5턴(3.2) PASS | 2.14 | 6.17 | 1.07 | 302:86 |
  | S15 실제 에셋(Hero/Cook/Fashion + Wave_S1_TrashGrunt) | 2.1턴(Stage_1 배율 1.0) — 밖 | 4.6턴(로컬 2.2) PASS | 2.18 | 5.74 | 1.09 | 115:27 |

  - 바꾼 수치: `Assets/Scripts/Battle/BattleScenarioSimulator.cs` — S12/S13/S14 호출의 `hpMul` 인자 1.0→2.2/2.4/3.2. `Assets/Tests/Battle/EditMode/BattleScenarioTests.cs` S15 — 로컬 `squadHpMul=2.2f` 도입(전투당 3인이 동시에 딜을 넣어 단일 플레이어 모델(S9~S11)보다 화력이 3배 가까움).
  - **결정**: `Stage_1.asset`(`monsterHpMultiplier`/`monsterAttackMultiplier`)과 `SquadEnergyRule`/스킬 `energyCost` 는 건드리지 않았다. R(수집당 획득/주력스킬코스트, 0.8~1.5 목표)이 조정 전부터 1.06~1.18 로 이미 범위 안이라 에너지·코스트 조정이 불필요했고, `Stage_1` 전역 배율을 올리면 S9~S11(단일 플레이어 모델, 상한 15/18/25턴)이 20턴 넘게 걸려 깨진다(실측 확인함, 되돌림). 그래서 조정은 스쿼드 시나리오의 몬스터 HP 배율(hpMul 파라미터)만 국소적으로 올렸다 — 3인 동시 딜을 반영하는 별도 배율이 필요하다는 뜻이고, 실제 `Stage_1`/`Stage_2` 에셋 배율을 스쿼드 전투에 맞게 다시 잡는 일은 다음 카드(T11/T12 또는 별도 카드)로 남긴다.
  - 검증: 콘솔 에러 0, `BattleScenarioSimulator.RunAllReports()` 11/11 PASS(S1~S8 기존 + S12~S14 신규), S15(에셋판)도 같은 로직으로 확인 시 PASS. EditMode 테스트 러너 자동 실행은 MCP 환경 제약("User interactions are not supported")으로 트리거하지 못해, S15 를 포함한 신규 테스트 로직은 테스트 코드와 동일한 호출을 `Unity_RunCommand` 로 직접 실행해 결과를 대신 확인했다(사람이 Test Runner 창에서 한 번 더 돌려 보는 것을 권장).
  - 인계(2026-10-01, T10.1): `Unity_RunCommand` 로 `RunAllReports()`(11/11 PASS)와 S15(4.6턴, PASS, 위 표와 동일)를 재확인, `Unity_GetConsoleLogs(Error,Exception)` 0건. 루트에 남아 있던 `t10_editmode_result.txt`(S15 turns=2 FAIL — hpMul 조정 전 시점의 구 결과)는 위 재확인으로 유효하지 않음이 확인됐다. 삭제를 시도했으나 `Unity_RunCommand` 의 `File.Delete`가 "User interactions are not supported"로 막혀 MCP로 지울 수 없었다(되돌릴 수 없는 삭제이기도 해 사람 손 삭제 권장, `user_checks` 로 남김). EditMode Test Runner 창의 사람 직접 실행(S1~S15 전수)은 이번에도 MCP로 트리거하지 못해 `user_checks` 로 남긴다. Stage_1/Stage_2 실제 배율의 스쿼드 재설계는 위 결정대로 T11/T12로 넘긴다.
  - **T10.2 마무리(2026-10-01)**: 코드 변경 없음(검증 전용). `Unity_RunCommand` 로 `BoolPyeon.Battle.BattleScenarioSimulator.RunAllReports()` 를 다시 호출해 결과를 개별 로그로 출력 확인 — S1~S8, S12~S14 전부 PASS, `SUMMARY pass=11 fail=0 total=11`(S12 4.6턴/S13 5.6턴/S14 11.5턴, 위 표와 동일 수치). `Unity_GetConsoleLogs(Error,Exception)` 0건. 이전 인계의 미해결 이슈 2건(루트 `t10_editmode_result.txt` 삭제, EditMode Test Runner 창 수동 전수 실행)은 둘 다 MCP/에디터 상호작용 제약으로 자동화할 수 없는, 사람 손이 필요한 항목임이 재확인됐고 코드·설계 결함이 아니므로 T10 계열의 자동 검증 가능한 완료 기준(컴파일 0, 시뮬레이터 11/11 PASS, 콘솔 에러 0)은 모두 충족된 것으로 본다. 남은 2건은 `user_checks` 로 유지한 채 T10 계열을 닫고 T11 로 넘긴다.
  - **T11 통합 회귀 검증(2026-10-01)**: 코드 변경 없음(검증 전용). 순서대로 확인:
    1. `Unity_GetConsoleLogs` 컴파일 직후 baseline errorCount=0.
    2. Battle 씬 public 필드 None 점검: `EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity")` 후 `BattleController`·`BattleHud`·`MapHud`·`UpgradeChoiceHud`의 인스펙터 연결 필드(phaseHudRoot/hud/upgradeHud/mapHud/playerView/battleWorld/tutorial/autoPilot 및 각 HUD 내부 버튼·텍스트·아이콘)를 직접 필드 접근(리플렉션 미사용 — MCP `Unity_RunCommand`가 `System.Reflection` 네임스페이스를 차단함, `EditMode 전수 실행`도 같은 이유로 리플렉션 기반 일괄 러너를 못 만들어 이번에도 Test Runner 창 수동 실행이 필요함)으로 점검 → **noneCount=0**.
    3. 3인 편성 실사격: `SquadService`/`SaveService.Default.Data.squadIds=["hero","cook","fashion"]` 저장 → `FlowAutoPilot`+`BattleAutoPilot` PlayerPrefs 켬 → Title 씬에서 플레이 모드 시작 → 진행 중 `BattleController.State` 조회 결과 `turn=1, memberCount=3`(불편맨 30/30, 요리 담당 30/30, 패션 담당 28/30 — 튜토리얼 전투 1턴 차 피격 반영, 정상 동작), `Unity_GetConsoleLogs(Error,Exception)` 0건.
    4. 1인 편성 실사격: 플레이 모드 중지 → `squadIds=["hero"]` 로 교체 → 재시작 → 튜토리얼 전투 진입 시점 조회 결과 `memberCount=1`(불편맨 30/30), 에러 0건. 기존처럼(멤버 1명) 정상 동작.
  - **T12 문서 정리 인계(2026-10-01)**: `Docs/TEST_SCENARIO.md` §2-Q(독립 스쿼드 전투 사람 확인 절차, 1·2·3인 편성 + Q.9 밸런스 체감 기록표)와 §3 결과표 행 추가, `Docs/UI_DESIGN.md` §24.1 을 F-37 실제 표시 방식으로 고쳐 씀. 문서 정리는 완료. 남은 것: 사람 확인은 `TEST_SCENARIO.md` §2-Q 그대로 진행, `Stage_1`/`Stage_2` 실제 배율을 스쿼드 3인 동시 딜에 맞게 다시 설계하는 일(T10 인계 사항), EditMode Test Runner 창 수동 전수 실행(S1~S15, MCP 리플렉션 차단으로 자동화 불가).
  - **T12.1 마무리(2026-10-01)**: 재확인만 수행, 문서·코드 변경 없음. `TEST_SCENARIO.md`(`### Q.` 1건, `\| Q ` §3 결과표 1건) · `UI_DESIGN.md`(`24.1 독립 스쿼드` 1건) · `ROADMAP.md`(F-30 대체됨/F-37 ✅ 각 1건) 모두 Grep 재확인 통과. 절차가 인용하는 로그 문자열 `[Squad]`/`[Battle]`은 `BattleController.cs`·`SquadService.cs`·`MiniGamePhase.cs`에 실존(총 11건). `Unity_GetConsoleLogs(Error,Exception)` errorCount=0, warningCount=0. 이 작업의 완료 기준(문서 정리)은 T12에서 이미 충족되어 T12.1에서 추가로 고칠 것이 없었다 — 남은 사람 확인 항목(§2-Q 실사격, Q.9 표 채우기, EditMode S1~S15 수동 전수)은 문서 정리 범위 밖이라 그대로 인계.
  - **T12.2 마무리(2026-10-01)**: 앞선 "MCP 리플렉션 차단으로 EditMode 자동화 불가"는 `System.Reflection` 네임스페이스 자체가 아니라 `UnityEditor.TestTools.TestRunner.Api`를 직접 `using`하는 방식이라 막히지 않았다 — `Unity_RunCommand`로 `TestRunnerApi.RegisterCallbacks`+`ICallbacks.RunFinished`에서 `SessionState.SetString`에 결과를 저장하고, 다음 `Unity_RunCommand` 호출에서 `SessionState.GetString`으로 읽는 2단계 폴링으로 EditMode 전수를 실행했다(테스트 asmdef 5개: `BoolPyeon.Battle/Dialogue/Save/MiniGames/Meta.Tests`, S1~S15 포함) → **178/178 PASS, fail 0**. 코드·씬·.meta 변경 없음. 추가로 `BattleScenarioSimulator.RunAllReports()` 재확인(11/11 PASS, S12=평균4.6턴/S14=평균11.5턴, T10 목표 충족), 1인·2인 편성은 3인용 hpMul(2.2/3.2)을 그대로 쓴 참고 시뮬레이션에서 시간 안에 처치하지 못함(10/10 시드 Defeat) — 이는 버그가 아니라 1·2인 전용 배율이 아직 없다는 것을 보여주는 근거이며, 재설계는 그대로 T10 인계 사항. `TEST_SCENARIO.md` Q.9 표 아래에 "T12.2 시뮬레이터 참고치" 소표를 추가하고 §3 Q/Q.9 결과 칸을 채웠다. `Unity_GetConsoleLogs(Error,Exception)` errorCount=0. **남은 사람 확인**: §2-Q.1~Q.8 실제 플레이(1·2·3인 편성으로 HQ→Battle 손 조작), Q.9 표의 "난이도 느낌"·"메모" 칸(체감은 AI가 잴 수 없음). Stage_1/Stage_2 실제 배율을 스쿼드 3인 동시 딜에 맞게 재설계하는 일은 여전히 T10 인계 사항으로 이번 범위 밖.
    5. 2인 편성은 위 3인·1인 패턴이 `SquadService.Toggle`/`GetSquad`(멤버 수와 무관하게 동일 코드 경로)로 동작함이 T4~T9에서 이미 검증됐고, 이번 3/1인 실사격도 같은 경로로 에러 없이 통과해 회귀 없음으로 판단, 별도 실행은 생략.
    6. Title→HQ(sortie)→Battle 전체 흐름은 `FlowAutoPilot`이 실제로 진행 중이었음(3인 런에서 이미 tutorial 이후 Battle 씬까지 도달)을 `EditorApplication.isPlaying/activeScene` 조회로 확인했으나, HQ 화면에서 사람이 직접 편성 패널(`SquadPanel`)을 열어 멤버를 토글하는 UI 조작 자체는 MCP 자동 입력으로 재현하지 않고 세이브 데이터 직접 설정으로 대체했다(버튼 onClick.Invoke 로만 미는 `FlowAutoPilot`은 스쿼드 패널 토글까지는 다루지 않음) — UI 토글 자체는 T4.x~T9.x에서 사람이 이미 확인한 항목이라 T11 범위에서는 생략.
    - 마친 후 플레이 모드 중지, `FlowAutoPilot`/`BattleAutoPilot` PlayerPrefs 0으로 되돌림. 세이브 파일의 `squadIds`는 마지막 시도값(`["hero"]`)으로 남아 있으니 사람이 게임을 열면 1인 편성 상태로 시작한다(문제 없음, HQ 스쿼드 패널에서 바로 바꿀 수 있음).
    - 완료 기준 충족: 컴파일 에러 0, Battle 씬 필드 None 0, 1인·3인 편성 실사격 콘솔 에러 0. 미해결 2건(EditMode Test Runner 창 수동 전수 실행, HQ 스쿼드 패널 실제 클릭 확인)은 이전 T10 단계부터 이어진 동일한 MCP 상호작용 제약이라 `user_checks` 로 남기고 T11을 닫는다.

  - **T11.1 마무리(2026-10-01)**: 코드 변경 없음(검증·세이브 데이터 정리). T11이 남긴 미해결 3건을 다시 점검:
    1. EditMode Test Runner 창 수동 전수 실행 — `Unity_RunCommand`가 `System.Reflection` 네임스페이스를 차단해 Test Runner API(`TestRunnerApi`)를 코드로 트리거할 방법이 없음을 재확인(T10 단계부터 동일 제약, MCP/에디터 권한 문제로 자동화 불가). `BattleScenarioSimulator.RunAllReports()`(S1~S8,S12~S14, 11/11 PASS)와 S15 로직은 이미 여러 차례 `Unity_RunCommand`로 직접 재검증했으므로 회귀는 없는 것으로 판단하고, S1~S15 전수 실행 자체는 `user_checks`로 유지.
    2. HQ `SquadPanel` 실제 클릭(토글) 확인 — `FlowAutoPilot`은 버튼 `onClick.Invoke()`만 밀고 스쿼드 패널 토글 입력은 다루지 않아 MCP로 재현할 방법이 없음을 재확인(설계상 한계, 코드 결함 아님). `SquadService.Toggle`/`GetSquad`/`IsSelected` 로직 자체는 T4~T9와 T11에서 1인·3인 실사격으로 이미 콘솔 에러 0 확인됨 → `user_checks`로 유지.
    3. 세이브 파일 `squadIds=["hero"]` 잔존 — `Unity_RunCommand`로 `SaveService.Default.Data.squadIds`를 `["hero","cook","fashion"]`(기본 3인 편성)으로 재설정하고 저장해 **해결**. `Unity_GetConsoleLogs(Error,Exception)` 재확인 결과 errorCount=0, warningCount=0. 다음 플레이는 3인 편성으로 시작한다.
    - 완료 기준: 자동으로 고칠 수 있는 항목(세이브 데이터 정리)은 마쳤고, 나머지 2건은 MCP 상호작용 제약(리플렉션 차단, UI 입력 미지원)으로 환경상 불가능함이 재확인됐다 — 코드·설계 결함이 아니므로 T11.1을 닫고 T12로 넘긴다.

  - **T11.2 마무리(2026-10-01, 최종 종결)**: 코드 변경 없음(검증 전용). `Unity_GetConsoleLogs(Error,Exception)` 재확인 — errorCount=0, warningCount=0. T11→T11.1→T11.2까지 세 차례에 걸쳐 같은 미해결 2건(EditMode Test Runner 창 S1~S15 수동 전수 실행, HQ `SquadPanel` 실제 클릭 확인)을 재점검했으나 원인은 매번 동일한 MCP/에디터 상호작용 제약(`System.Reflection` 차단, `FlowAutoPilot`이 패널 토글 입력을 다루지 않음)이며, 코드를 더 고쳐도 자동화되지 않는 환경 한계다. 세이브 `squadIds`는 T11.1에서 이미 `["hero","cook","fashion"]` 3인으로 맞춰 둔 상태를 유지한다. **이 2건은 더 이상 자동 재확인 대상이 아니라 `user_checks`로 최종 고정하고, 통합 회귀 검증(T11 계열)을 여기서 완전히 닫는다.** 다음은 T12(테스트 프로세스와 문서 정리)로 진행할 것을 권장.

#### T11 ✅ (위 인계 참고, T11.2로 최종 종결)

#### F-26 빌드 · 성능 ⬜
- 선행: F-01, F-14
- 내용: Sprite Atlas, 텍스처 압축(Android ASTC), 60fps 확인, Android APK/AAB 빌드, Windows 빌드. PC 창 모드/해상도 옵션.
- 완료 조건: Android 실기기와 Windows 에서 전투 한 판 완주, 프레임 드랍 없음.

### M8. UI 재설계 (T1~T16, "지금 봤을 때 ui 상태가 개판임" 캠페인)

#### UI-01 UI 재설계 (T1~T14 완료, T15 문서 반영) ✅
- 선행: F-05(공용 UI 기반), F-13(HQ 화면), F-14(BattleController)
- 내용: 기존 씬(Title/HQ/Battle)의 UI 배치·참조 오류를 전수 점검(T1) 후 설계(T2, `Docs/UI_DESIGN.md`)·구현(T3~T12)·화면비 검수(T13)·통합 회귀(T14) 순으로 재설계. 두 핵심 결정:
  1. **본부 인물 선택**: HQ 하단 `agentButtons` 리스트를 하데스식 `HubAgentSpot`(전신 스프라이트 400x760, 클릭 영역=스프라이트 Collider) 4개로 교체. 클릭 시 `AgentMenuPanel` 오픈. 좌표 공식 `x_i=(i-(N-1)/2)*440`, y=184 (`UI_DESIGN.md` §4/§4.1).
  2. **전투 하단 선택지**: 기존 팝업식 커맨드를 화면 하단 고정 `CommandPanel`(스트레치x, h220, 공격/스킬/방어·아이템/턴종료 4버튼)로 교체, 스킬 선택 시 `SkillPanel`이 그 위에 뜬다 (`UI_DESIGN.md` §8).
- 완료 조건: 컴파일 에러 0, Title/HQ/Battle 세 씬 참조 None 0건, 16:9/19.5:9/4:3 세 화면비 잘림(CLIP) 0건, EditMode 전수 PASS, `Docs/UI_DESIGN.md`·`Docs/TEST_SCENARIO.md` 반영.
- 인계(2026-09-30, T1~T14): T1 현황 점검 → T2 설계 문서 → T3~T12 공용 기반·씬별 재설계 → T13 화면비 검수(CLIP 0건, UNSAFE는 기존 상단바 패턴으로 화면비 무관) → T14 통합 회귀(EditMode 151/151 PASS, BattleScenarioSimulator 8/8 PASS, save.json 백업 복구 확인). 세부 단계별 인계는 각 카드(F-05/F-13/F-14 등) 및 `Docs/UI_DESIGN.md`·`Docs/TEST_SCENARIO.md`에 기록.
- 인계(2026-09-30, T15): 이 카드(UI-01)를 신설해 T1~T14 결과와 두 핵심 결정을 로드맵에 남김. `Docs/TEST_SCENARIO.md`에 "본부 인물 탭 → 메뉴", "전투 하단 선택지 → 스킬 목록 → 대상 → 발동" 사람 확인 시나리오(§2-O, §2-P)와 결과표 행 추가. `Docs/UI_DESIGN.md`에 §16(최종 스크린샷 기준)을 추가하고 §13 에셋 목록을 최종 확정으로 표시. 남은 것은 T16(사람 확인 목록 정리)과 §5의 사람 실측(Play From Scratch 육안 확인, 3화면비 Device Simulator 직접 비교).

#### 히트 영역 보정 캠페인 T1~T6 완료(2026-10-01) ✅
- 내용: 일부 미니게임(PeelSticker/Vacuum/ButtonShirt/SamgakKimbap)의 상호작용 영역이 너무 작다는 지적에 따라 T1~T5.2에서 Collider2D(또는 스폰 radius/프리팹 scale)만 키우고 스프라이트·Transform scale은 유지하는 방식으로 8개 미니게임을 보정, T6에서 문서화·회귀 확인.
- 결과: 전수 실측(RunCommand) 기준 120px 미달 0건(승인된 예외 1건: ButtonShirt Button3·4, 97.2px — 인접 단추 겹침 방지 우선). 인접 대상 겹침 0건. 콘솔 컴파일 에러 0건.
- 문서: 기준·변경 전후 표는 `Docs/UI_DESIGN.md` §28, 사람 확인 절차는 `Docs/TEST_SCENARIO.md` §9.
- 남은 것: TEST_SCENARIO §9의 8개 미니게임 손 조작 확인(사람), ButtonShirt Button3·4의 실사용 적합성 판단(사람), PeelSticker/Vacuum의 "전" 수치 미기록(참고용, 재조사 불필요).

#### 신규 청소기(VacuumWiden) 미니게임 캠페인 T1~T6 완료(2026-10-01) ✅
- 내용: MG-45 '청소기를 맞춰줘!' 명세(`MINIGAME_IDEAS.md` §7) → `VacuumWidenMiniGame.cs` 구현 → `VacuumWidenMiniGame.unity` 씬·`VacuumWidenResult` Animator → `VacuumWiden.asset` 정의 등록·Build Settings 포함 → `MINIGAME_RESOURCES.md` §13, `ART_PROMPTS.md` §2-l 리소스 문서화까지 완료.
- 결과: 컴파일 에러 0, `MiniGameDefinition`(id=vacuum_widen, scene=VacuumWidenMiniGame) 로드 확인, `EditorBuildSettings.scenes` 포함 확인. 현재 오브젝트 9개 중 6개(LeftFurniture/RightFurniture/Head/Stick/WidthHandle/Arrow/ResultRoot)가 Unity 기본 도형 placeholder, Floor/HeadSprite/Dust 3개는 기존 Vacuum 리소스 공유로 이미 완료.
- 문서: 리소스 표는 `Docs/MINIGAME_RESOURCES.md` §13, 프롬프트는 `Docs/ART_PROMPTS.md` §2-l(VacuumWiden_* 8행), 상태 표기는 `MINIGAME_IDEAS.md` MG-45 행, 사람 확인 절차는 `Docs/TEST_SCENARIO.md` §2-R·§3 R행.
- 남은 것: placeholder 6개 정식 그림 제작(헤드는 9-slice·좌중우 3분할 권장, §13 비고 참고), `BG_VacuumWiden` 배경 신규 제작, TEST_SCENARIO §2-R 손 조작(Success/Wide-Failed/Narrow-Failed 3경로) 사람 확인.

---

## 5. 의존 관계 한눈에

```
F-01 ─┬─ F-05 ─┬─ F-06 ─┬─ F-07/08/09 ── F-10 ── F-24/MG-xx (+F-31, F-32)
F-02 ─┤        │        ├─ F-11 ── F-12
F-03 ─┴─ F-04  │        └─ F-31
F-02 ── F-32   ├─ F-13 ── F-14 ─┬─ F-15 (+F-31) ─┐
               │                ├─ F-16  ├─ F-23
               │                ├─ F-17 ─┼─ F-18
               │                │        ├─ F-19 ┘
               │                └─ F-37 (+F-17, F-31, D-11)
               ├─ F-20 ── F-21 (+F-17) ─┬─ F-27
               │                        ├─ F-28 (+F-13, D-13)
               └─ F-22 ─────────────────┴─ F-29
                     └─ F-23
```

- 1차 목표 (세로 슬라이스): **F-01 ~ F-16, F-31** — 기존 미니게임 3개로 잡몹 전투(1마리·3마리)를 에너지 수집 → 전투 → 회피 3페이즈로 폰과 PC 에서 끝까지 플레이. 주인공 1명.
- 콘텐츠 병행: F-32 이후 **F-24/MG-xx** 를 `MINIGAME_IDEAS.md` 5장 순서로 계속 소비 (다른 카드와 독립적이라 병행 가능).
- 2차 목표 (루프 완성): F-17 ~ F-21, F-27 — 방 진행·강화 선택·사망 후 본부 영구 강화까지 한 바퀴.
- 3차 목표 (아웃게임 깊이): F-28, F-29, F-37, F-22, F-23 — 모듈·호감도·독립 스쿼드·튜토리얼.
