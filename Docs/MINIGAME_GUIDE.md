# 미니게임 제작 가이드 (F-10)

새 미니게임 하나 = 씬 하나 + 매니저 스크립트 하나 + 정의 에셋 하나. 이 문서대로 만들면 `MiniGameRunner` 가 지시문·제한 시간·결과 처리를 대신 해 준다.
검증용 참고 게임: **TapTargetMiniGame** ("잡아라!" — 움직이는 초록 과녁을 모두 탭, 빨간 과녁은 탭하면 실패).

## ① 만드는 순서

1. **이름 정하기**: `<Name>MiniGame` 한 이름을 모든 폴더에 쓴다 (CLAUDE.md 표).
2. **폴더 5개** (Unity 에디터/MCP 로 생성 — `.meta` 를 직접 만들지 않는다):
   - `Assets/Scenes/MiniGames/` (씬 파일 `<Name>MiniGame.unity`)
   - `Assets/SampleScript/<Name>MiniGame/`
   - `Assets/SampleImage/<Name>MiniGame/`
   - `Assets/Animator/<Name>MiniGame/`
   - `Assets/Prefabs/<Name>MiniGame/`
3. **스크립트**: `Assets/Scripts/MiniGames/Template/TemplateMiniGame.cs` 를 `Assets/SampleScript/<Name>MiniGame/<Name>MiniGame.cs` 로 복사 → 클래스 이름을 파일 이름과 같게 바꾸고 `namespace` 줄은 지우거나 바꾼다 (`using BoolPyeon.MiniGames;` 필요). 스크립트 하나에 클래스 하나.
4. **씬**: `Assets/Scenes/MiniGames/TemplateMiniGame.unity` 를 MCP(`AssetDatabase.CopyAsset`)로 복제 → Manager 오브젝트의 컴포넌트를 새 스크립트로 교체.
5. **정의 에셋**: `Assets/Data/MiniGames/<Name>.asset` (Create ▸ BoolPyeon ▸ MiniGame Definition). `id`(snake_case), `sceneName`(= 씬 파일 이름), `instruction`(`~해줘!` 한 마디), `baseTimeLimit`(보통 5), `timeoutResult`(보통 Failed), `min/maxDifficulty`.
6. **Build Settings 등록**: MCP 로 `EditorBuildSettings.scenes` 에 추가 (`ProjectSettings/EditorBuildSettings.asset` 직접 편집 금지). 등록 안 하면 Runner 의 `LoadSceneAsync` 가 실패한다.
7. **Runner 로 Play**: SampleScene 의 `MiniGameRunner` 오브젝트의 `MiniGameRunnerDebug.definition` 에 정의 에셋 연결 → Play → **T 키**(또는 컴포넌트 우클릭 ▸ Run Debug). 결과는 콘솔 `MiniGame result: ...`. 성공·실패·시간초과 세 경우를 확인한다.
8. **아이디어 문서 갱신**: `Docs/MINIGAME_IDEAS.md` 해당 행 상태를 "구현됨" 으로.

## ② 체크리스트 (Play 전에)

- [ ] 콘솔 컴파일 에러 0
- [ ] 매니저의 public 필드가 모두 연결됨 (None 없음)
- [ ] 클릭 대상에 Collider2D, 목표·통과 판정 대상은 태그 `ClearObject`
- [ ] 결과 Animator: bool `isSuccess` / `isFailed`, 상태 `Idle` → `Success` / `Failed`
- [ ] 미니게임 씬에 **Canvas·EventSystem 을 두지 않는다** (Runner 호스트 씬과 중복·클릭 가로채기 방지). Main Camera 는 하나, `CameraFit` 부착 (Runner 가 호스트 중복 컴포넌트를 끈다)
- [ ] 생성하는 오브젝트는 매니저 아래(`Instantiate(prefab, pos, rot, transform)`)에 둔다 — Additive 로드 시 활성 씬(호스트)에 생기지 않게
- [ ] 16:9 와 4:3 에서 중요한 요소가 기준 영역(17.78 × 10 월드) 안에 있음

## ③ 난이도·속도 규칙

| 무엇 | 어디서 | 규칙 |
|---|---|---|
| 개수·허용 오차 | `ctx.Difficulty` (정의의 min~max 범위) | `OnBegin` 에서 한 번 정한다. 예: 과녁 수 = Difficulty+1 |
| 움직임 | `ctx.Speed` | `Time.deltaTime × baseSpeed × ctx.Speed` |
| 판정 임계 | 월드 단위, 월드 단위/초 | 픽셀·프레임 기준 금지 (화면비·프레임률에 흔들림) |
| 제한 시간 | Runner (`ctx.TimeLimit` 또는 `baseTimeLimit`) | 게임은 타이머를 갖지 않는다. 시간초과 결과는 정의의 `timeoutResult` |

**MiniGameBase 계약**
- `Start` 를 선언하지 않는다. 단독 실행(씬 바로 Play, `MiniGameRunner` 없음)이면 베이스가 한 프레임 뒤 `Begin(MiniGameContext.Default)` (Difficulty 1, Speed 1) 를 부른다. **단독 실행에서는 시간초과가 없다.**
- 시작 처리는 `OnBegin(ctx)`, 입력은 `IsRunning` 일 때만.
- 결과는 `Resolve(MiniGameResult.Success/Failed)` — 조건 해제 → `OnResolved`(연출) → Runner 에 보고. 두 번째 호출은 무시된다.
- 조건 방식: `Watch(new PredicateCondition(result, () => 조건))`, 트리거는 `TriggerTagCondition` 컴포넌트를 `Watch`. 먼저 Watch 한 조건이 우선(실패를 먼저).
- 연출: `ResultAnimator` 오버라이드 → 베이스가 bool 을 켠다. 정지 등은 `OnResolved` 오버라이드 후 `base.OnResolved`.

## ④ 입력

- `BoolPyeon.Input.PointerInput` 만 쓴다 (마우스·터치 동일). `Input.GetMouse*` 직접 호출 금지.
  - `Down` / `Held` / `Up`, `WorldPosition()`, `RaycastWorld2D()` (UI 위 포인터면 무시).
- 조작 유형별 헬퍼: `Assets/Scripts/Input/Interaction/` 의 `TapInteraction` / `DragInteraction` / `HoldInteraction`. 연타·긋기·문지르기·돌리기는 F-32 이후.

## ⑤ 참고 구현

| 조작 | 파일 | 볼 곳 |
|---|---|---|
| 탭 (대상 고르기) | `Assets/SampleScript/EggMiniGame/EggGameManager.cs` | `PickTarget` |
| 탭 + 이동 + 난이도 | `Assets/SampleScript/TapTargetMiniGame/TapTargetMiniGame.cs` | `OnBegin`, `Update` |
| 드래그 속도 판정 | Pizza 의 `CutterMove` | 빠르게 올리면 실패 |
| 이동·통과 판정 | USB 의 `UsbMover` | `ClearObject` 트리거 |

## 부록: TapTarget 구성과 필요 에셋

- 씬 `TapTargetMiniGame`: Main Camera(ortho 5, CameraFit) · Manager(`TapTargetMiniGame`: minPos, maxPos, targetPrefab, decoyPrefab, resultAnimator) · MinPos(-7,-3.5) · MaxPos(7,3.5) · ResultRoot(Animator `TapTarget.controller`, 스프라이트).
- 프리팹 `Assets/Prefabs/TapTargetMiniGame/`: `Target`(초록 원, CircleCollider2D, 태그 ClearObject, TapTargetMover), `Decoy`(빨간 원, CircleCollider2D, Untagged, TapTargetMover).
- 정의 `Assets/Data/MiniGames/TapTarget.asset`: id `tap_target`, sceneName `TapTargetMiniGame`, instruction "잡아라!", 5초, Failed, 난이도 1~3.
- 필요 에셋(현재 기본 원 스프라이트로 대체): 과녁 그림(진짜/가짜 구분되는 두 종), 배경, 성공·실패 연출 이미지.
