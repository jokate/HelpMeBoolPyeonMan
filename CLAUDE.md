# 도와줘요 불편맨 (HelpMeBoolPyeonMan)

몇 초짜리 2D 미니게임을 이어 붙이는 게임. 미니게임 하나 = 씬 하나.

- Unity 6000.3.24f1, 2D (`com.unity.feature.2d`), 1920x1080 가로
- 입력: **구 Input Manager** (`activeInputHandler: 0`) → `Input.GetMouseButtonDown` 등. 새 Input System 을 쓰지 않는다.
- Unity MCP: `.mcp.json` 의 `unity-mcp` (Unity AI Assistant `com.unity.ai.assistant` 의 relay). **Unity 에디터가 이 프로젝트를 열고 있어야** 동작한다.

## 폴더와 이름

새 미니게임 `<Name>` 은 모든 폴더에서 `<Name>MiniGame` 한 이름을 쓴다.

| 무엇 | 위치 |
|---|---|
| 씬 | `Assets/Scenes/MiniGames/<Name>MiniGame.unity` |
| 스크립트 | `Assets/SampleScript/<Name>MiniGame/*.cs` |
| 스프라이트 | `Assets/SampleImage/<Name>MiniGame/` (Texture Type: Sprite (2D and UI)) |
| 애니메이션·컨트롤러 | `Assets/Animator/<Name>MiniGame/` |
| 프리팹 | `Assets/Prefabs/<Name>MiniGame/` |

기존 게임(Egg·Pizza·USB)은 폴더 이름이 조금씩 다르다(`EGGMiniGame`, `EggGame` 등). **이름을 맞추려고 옮기지 않는다** — 참조가 깨진다.

## 미니게임 구조 (기존 게임이 따르는 패턴)

- 게임마다 매니저 MonoBehaviour 하나가 규칙을 가진다 (예: `EggGameManager`). 공용 베이스 클래스는 아직 없다.
- 클릭 대상 찾기: `Camera.main.ScreenToWorldPoint(Input.mousePosition)` → `Physics2D.Raycast(pos, Vector2.zero)`. 대상에는 Collider2D 가 있어야 한다.
- 성공·실패 판정: `Start()` 에서 코루틴을 띄우고 `WaitUntil` 로 조건을 기다린다 (`SuccessCheck` / `FailedCheck`).
- 결과 연출: Animator bool 파라미터 **`isSuccess` / `isFailed`** 를 켠다. 컨트롤러는 `Idle` → `Success` / `Failed` 상태.
- 목표 지점·통과 판정: 태그 **`ClearObject`** + `OnTriggerEnter2D` (트리거 콜라이더).
- 난이도: 매니저의 `static int Level` 로 개수·속도를 조절한다 (`EggGameManager.Level` 참고).
- 씬 오브젝트 참조는 public 필드 + 인스펙터 연결 (`upPos`, `downPos`, `target` 등). 위치 범위는 빈 오브젝트 두 개로 잡는다.

새 게임도 이 패턴을 따른다. 공용 구조(예: 미니게임 베이스 클래스, 제한 시간 UI)가 필요하면 먼저 제안하고, 기존 게임을 한꺼번에 바꾸지 않는다.

## 작업 규칙

- **씬·프리팹·애니메이터·애니메이션 (`.unity` `.prefab` `.controller` `.anim`) 은 Unity MCP 로 만들고 고친다.** YAML 을 직접 편집하지 않는다 (fileID·GUID 참조가 깨진다).
- **`.meta` 는 만들거나 고치거나 따로 지우지 않는다.** 에셋을 옮기거나 지울 때도 MCP(에디터)를 통해서 한다.
- C# 스크립트는 파일로 직접 고쳐도 된다. 고친 뒤에는 MCP 로 에셋을 새로고침하고 **콘솔에 컴파일 에러가 없는지** 확인한다.
- 건드리지 않는 것: `Library/` `Temp/` `Logs/` `obj/` `.vs/` `UserSettings/`, 자동 생성되는 `*.csproj` `*.sln`, `Packages/` `ProjectSettings/` (필요하면 이유를 먼저 적는다).
- 작업 범위는 해당 미니게임 폴더로 한정한다. 관계없는 스크립트의 스타일·using 정리 같은 리팩터링은 하지 않는다.
- 스크립트 하나에 클래스 하나, 파일 이름 = 클래스 이름 (Unity 가 그래야 컴포넌트로 붙인다).
- 이미지·사운드를 새로 만들 수 없으면 기본 도형 스프라이트(Square/Circle)나 기존 이미지로 자리를 잡고, 필요한 에셋 목록을 남긴다.

## 검증

명령줄 빌드·테스트는 없다. 확인은 Unity MCP 로 한다.

1. 에셋 새로고침 → 콘솔 **컴파일 에러 0** (경고는 보고만).
2. 씬을 열어 계층 구조 확인: 매니저 오브젝트, public 필드가 모두 연결됐는지(None 없음), 콜라이더·태그·Animator 파라미터.
3. MCP 로 플레이 모드를 실행할 수 있으면 잠깐 실행해 콘솔 에러·예외가 없는지 본다.
4. 손으로만 확인할 수 있는 것(조작감, 성공·실패 연출, 난이도)은 사람이 할 확인 목록으로 남긴다. 예: "PizzaMiniGame 씬 Play → 커터를 끌어 피자를 자르면 Success 애니메이션".

Unity MCP 가 연결되지 않으면(에디터가 꺼져 있음 등) 씬·프리팹을 추측으로 만들지 말고, 스크립트까지만 하고 남은 에디터 작업을 목록으로 남긴다.
