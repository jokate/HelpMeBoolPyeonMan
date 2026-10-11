도와줘요 불편맨 — 아트 프롬프트 목록 (Tripo 이미지 생성용)

작업 순서(캠페인): T1(이 문서 틀) → T2 플레이어 캐릭터 → T3 HQ 요원·초상화 → T4 몬스터·보스 →
T5 배경·맵 → T6 UI 아이콘 → T7 구현된 미니게임 → T8 예정 미니게임 → T9 검수.
이 문서는 T1 산출물이다. §2 표의 "프롬프트" 칸은 아직 비워 두고, 이후 작업에서 채운다.

## §0. 사용법

- 대상 도구: Tripo (텍스트→이미지). 이 문서의 프롬프트 칸을 그대로 입력에 붙여 쓴다.
- 기준 톤 이미지: `Docs/sketches/art_pizza.png` (손그림 선화 + 단색 채색, 투명 배경).
- 각 행의 "배치 경로"는 `CLAUDE.md` 폴더 규칙을 따른다. 새 파일을 받으면 Texture Type을
  Sprite (2D and UI)로 바꾸고 해당 경로에 넣은 뒤, 씬·프리팹 연결은 Unity MCP로 한다.
  이 문서 자체는 이미지·씬·코드를 만들지 않는다.
- "원본" 칸은 이 표의 근거(데이터 에셋 경로 또는 씬 오브젝트 경로)이며, 완료 기준 대조에 쓴다.

### 0-1. Tripo 사용 순서 (T9)

1. **2D/3D 선택**: 이 프로젝트는 2D(가로 1920x1080)이므로 **Tripo의 텍스트→이미지(2D)** 모드만 쓴다.
   텍스트→3D 모드는 쓰지 않는다(불필요한 폴리곤·리깅 생성).
2. **추천 설정**: 스타일은 §1-1 공통 스타일 블록을 프롬프트 앞에 그대로 붙이고, §1-3 네거티브
   프롬프트를 네거티브 입력란에 그대로 붙인다. 해상도는 각 표 행의 "권장 해상도" 칸을 그대로
   쓴다(예: 아이콘 256x256, 배경 1920x1080). 배경 투명(transparent background) 옵션이 있으면 켠다
   (배경·맵류 §2-j 제외 — 이 항목은 불투명 전체 배경이 목적).
3. **결과물 저장 이름 규칙**: 파일명은 표의 "에셋ID" 칸(예: `Grunt_Hit`, `Upgrade_Hp`)을 그대로
   쓰고 확장자는 `.png`. 상세표(§2-a-1 등)처럼 한 에셋에 상태가 여럿이면 에셋ID에 상태 접미사가
   이미 포함돼 있으므로(`Grunt_Stand`/`Grunt_Hit`/`Grunt_Attack`) 그대로 쓰면 된다. 대소문자·언더스코어를
   표와 똑같이 맞춘다(코드·씬에서 참조 시 이름이 어긋나면 연결이 깨진다).
4. **임포트**: 받은 png를 표의 "배치 경로" 칸 폴더에 넣고, Unity 에디터에서 해당 스프라이트의
   Texture Type을 **Sprite (2D and UI)** 로 바꾼다. 씬·프리팹·애니메이터 연결은 `CLAUDE.md` 규칙대로
   Unity MCP로 한다(YAML 직접 편집 금지, `.meta` 직접 생성·수정 금지).

### 0-2. 생성 진행 체크리스트

아래 순서로 §2 각 절을 소비한다. 절 하나를 끝낼 때마다 체크하고, 남은 에셋 수를 갱신한다.

- [x] §2-a 플레이어 캐릭터 (Hero, Char_Cook, Char_Fashion — 상세 §2-a-1~3)
- [x] §2-b 몬스터·보스 (Grunt, MidBoss, S1_Mosquito, S1_TrashBag, Boss_Stage1 — 상세 §2-b-1~5) + 추가분(S1_RubberGloveShield·AngryMixer·StaticFairy, S2_*, Boss_Stage2)
- [x] §2-c HQ 요원 전신·초상화 + §2-c-1 DLG 화자 + §2-c-2 대화창 프레임
- [x] §2-d 스킬 아이콘, §2-e 전투 강화 아이콘, §2-f 영구 강화 아이콘, §2-g 모듈 아이콘
- [x] §2-h 상태이상 아이콘, §2-i 전투 예고 아이콘
- [x] §2-j 배경·맵 (상세 §2-j-1~5)
- [x] §2-k 씬 UI 자리 스프라이트, §2-n 미니게임 용도·불편 유형 배지
- [x] §2-l 구현된 미니게임 (게임별로 나눠 진행 가능)
- [x] §2-m 예정 미니게임 (구현 착수 시점에 맞춰 순서대로)
- [x] §2-o 공용 UI 컴포넌트 (T3, 9종) — 프롬프트 완료, 이미지 생성은 차단(§0-2 남은 생성 수 참고)
- [x] §2-o-1 HUD 아이콘·맵 방 유형 아이콘 (T4, 7종: Icon_Crystal·Maze_DirArrow·MazeIcon_5종) — 프롬프트·코드 필드 완료, 이미지 생성 차단
- [x] §2-p 미니게임 페이즈 HUD 리소스 (T5, 7종) — 프롬프트·씬 연결 완료, 이미지 생성 차단
- [x] §2-q 전투 월드 표시 리소스 (T10, 8종) — 프롬프트 완료, 이미지 생성 차단, 코드 필드는 후속 구현
- [x] 각 항목 임포트 후 Texture Type 확인 (전부 Sprite (2D and UI))
- [~] 씬/프리팹 연결 — 일부 완료 (§0-3 "연결 현황" 참고)

**남은 생성 수 (T11 기준, 2026-09-30)**: 문서상 프롬프트는 모두 작성됐고, `Unity_AssetGeneration_GetModels`가 `Models: []`를 반환하는 구조적 차단이 T5.2부터 지속돼 이미지 생성이 전혀 실행되지 않았다. 아직 실제 이미지가 없는 항목 합계:
  - §2-m 예정 미니게임 13종 53행(BG/Target/FX_Success/FX_Fail 등, MG-04 오답 1행 포함)
  - §2-o 공용 UI 9종 + §2-o-1 HUD·맵 아이콘 7종 = 16종
  - §2-p 미니게임 페이즈 HUD 7종
  - §2-q 전투 월드 표시 8종
  - 기타: Obj_Egg_Chopstick 교체(T7.2 동의 대기), FlySwat_Swatter 실제 그림(T8.2 임시 UISprite)
  - **합계 약 88행(개별 스프라이트 시트 프레임 수는 제외한 표 행 기준)** — 전부 사람이 Unity AI Assistant 패널에서 생성 모델을 등록한 뒤에야 착수 가능하다.

### 0-3. 생성 결과 메모 (2026-09-30)

- 도구: Tripo 웹 → GPT Image 2.5. 그림체 기준은 Hero_Stand(Midjourney v8.2 결과)이고, 나머지는 이 이미지를 편집·참고 이미지로 넣어 그림체를 맞췄다. 캐릭터 외형 고정: Cook=짧은 갈색 머리 남성, Fashion=검은 단발 여성.
- 캐릭터는 단일 포즈 대신 3프레임 시트를 추가로 만들었다: `<이름>_Attack_Anim.png` / `<이름>_Defense_Anim.png`(Multiple, 768px 칸 3개, 발 기준 pivot). `<이름>_Attack.png`/`_Defense.png`는 타격·막기 프레임.
- 몬스터는 상태별 한 장씩(`_Stand/_Hit/_Attack`, 보스는 `_Phase2`), pivot은 바닥 가운데, PPU는 대기 자세 키 기준.
- 배경 제거는 스크립트로 했다(캐릭터 인식 모델 + 흰 배경 flood fill). 몸에서 떨어진 효과(흩날리는 종이, 악취 구름 등)는 일부 지워졌다.
- 대화창·버튼·패널·게이지·미로 타일은 같은 잉크선 스타일로 직접 그렸다. 9-slice 테두리 설정 완료(DialogueFrame_Panel 64px 등).
- 권장 해상도보다 작은 것: 보스·MidBoss(시트에서 잘라 768 캔버스), 배경(생성 1792x1024 → 1920x1080 확대).
- **사고**: `Assets/SampleImage/SamgakKimbapMiniGame/Kimbap_Wrapper/Laver/Tab/Arrow.png` 기존 이미지 4장을 같은 이름으로 덮어썼다(git 미추적, 복구 불가). 현재 파일은 새 생성본이다.
- 연결 현황
  - 완료(데이터): SkillDefinition.icon 4, MonsterDefinition.spriteOverride 12(+ 임시 tint 흰색으로), AgentDefinition.portrait 3, DLG_* 대사 31줄 portrait(speakerName 기준).
  - 완료(씬): Title 배경·시작 버튼·로고(Logo 오브젝트는 원래 비활성, 켜지 않음), HQ 배경·요원 버튼 3(전신)·패널·출격/스쿼드 버튼·대화창, Battle 배경·HP바 배경·커맨드 버튼 3·결과 패널·대화창·PlayerView(Hero_Stand)·MonsterView(런타임 spriteOverride). DialogueBox.prefab 프레임.
  - 완료(미니게임, 원래 placeholder 상자 안에 비율 유지로 표시, transform·콜라이더는 원래 값): USB, PeelSticker, ScreenBubble, ButtonShirt, Vacuum, FlySwat(+Fly.prefab), BossBlob, TapTarget(+Target/Decoy.prefab). 배치(상자 크기) 자체는 placeholder 그대로라 오브젝트가 작다 — 레이아웃 조정 필요. 권장 표시 크기는 §2-l 각 행 비고 참조(캠페인T8). transform 조정은 이미지 교체 때 스프라이트 PPU·크기로 맞추고 콜라이더 스케일은 보존(T8.1 방침).
  - 코드 필요(미연결): 전투 예고 아이콘 6종(IntentIcon이 색만 바꿈, §2-q Intent_Magic/Buff/Heal 3종 신규 설계 완료), 라벨 판·예고 받침·선택 마커·피격/정화 이펙트(§2-q, UI_DESIGN §20 신규 필드 목록), 강화/영구강화/모듈 아이콘(데이터에 icon 필드 없음), 상태이상 아이콘, 미로 타일·방향 버튼(MapHud가 런타임 생성, 스프라이트 필드 없음), 스킬 버튼 아이콘, 캐릭터 공격/방어 애니메이션 클립(BattleUnitView는 스프라이트 1장), Cook/Fashion(스프라이트 필드 없음), Fly 2프레임 날갯짓.
  - 기존 손그림 유지(미교체): Egg, Pizza, SamgakKimbap 씬.
- **2026-09-30 UI 리소스 31종**(§2-o·§2-o-1·§2-p·§2-q) 생성 완료 — 생성 모델 대신 기존 UI 프레임과 같은 잉크선 스타일로 스크립트로 그림. 연결 현황은 UI_DESIGN §22(월드 표시 §2-q 는 BattleUnitView 코드 대기).
- 파일 이름 차이: USB 실패 이미지는 `USB_FailedUSB`, Fly 2프레임은 `FlySwat_Fly_0/_1`, 스티커 뜯긴 상태는 `PeelSticker_TornHalf`, 예정 미니게임은 `MG##_<이름>_<부위>`.

## §1. 공통 스타일 블록 (영문)

### 1-1. 기본 프롬프트 접두(모든 행의 프롬프트 앞에 붙인다)
```
hand-drawn ink line art, flat single-tone color fill, simple cel shading,
sketchbook doodle style matching the tone of art_pizza.png reference,
transparent background, clean thin outline, no background scenery, isolated subject
```

### 1-2. 방향 규칙
- 전투 화면에 놓이는 캐릭터·몬스터(§2-a, §2-b): **측면(side view)**.
  플레이어 파티는 화면 오른쪽을 보고(faces right), 몬스터는 화면 왼쪽을 본다(faces left).
  프롬프트에 `side view, facing right` 또는 `side view, facing left` 를 명시한다.
- 초상화·아이콘(§2-c 초상화, §2-d~i, §2-l 일부 UI형 오브젝트): **정면(front-facing)**.
  프롬프트에 `front view, facing camera` 를 명시한다.
- 미니게임 오브젝트(§2-l, §2-m)는 게임마다 정한 시점을 따른다(대개 정면/평면).

### 1-3. 네거티브 프롬프트(모든 행에 공통 적용)
```
3D render, photorealistic, gradient shading, soft airbrush, text, watermark,
signature, background scenery, multiple objects, cropped, blurry, extra limbs
```

## §2. 전체 목록 표

열: 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고

### 2-a. 플레이어 캐릭터 (Assets/Data/Battle/*.asset)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| Hero | 캐릭터 | Assets/Data/Battle/Hero.asset | BattleUnitView.body(SpriteRenderer), 전투 화면 플레이어 표시 | 1024x1024 | Assets/SampleImage/Battle/Characters/ | §2-a-1 상세표 참고(기본/초상화3종/공격/방어 6칸) | 현재 Battle.unity PlayerView가 Square 도형 placeholder |
| Char_Cook | 캐릭터 | Assets/Data/Battle/Char_Cook.asset | 위와 동일(파티원) | 1024x1024 | Assets/SampleImage/Battle/Characters/ | §2-a-2 상세표 참고(기본/초상화3종/공격/방어 6칸) | Sprite 필드 없음(코드에 연결 필드 미구현, 범위 밖) |
| Char_Fashion | 캐릭터 | Assets/Data/Battle/Char_Fashion.asset | 위와 동일(파티원) | 1024x1024 | Assets/SampleImage/Battle/Characters/ | §2-a-3 상세표 참고(기본/초상화3종/공격/방어 6칸) | 위와 동일 비고 |

### 2-a-1. Hero 상세 프롬프트

캐릭터 세계관: 퇴근 후 회사원이 COMFORT(가상 조직)에 스카우트되어 "불편맨"이 된다. Hero는 균형형(밸런스) 주인공.
**캐릭터 고정 설명 문구(모든 프롬프트에 동일하게 삽입)**:
`office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | Tripo 3D용 프롬프트(영문) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|---|
| Hero_Stand | 기본 서 있는 스프라이트, BattleUnitView.body | 1024x1024 | Assets/SampleImage/Battle/Characters/ | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, side view, facing right, standing idle pose, arms relaxed at sides, full body` | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, front view, facing camera, neutral T-pose, full body, single character, symmetrical, clean toon low-poly style, reference pose for 3D model` | 전투 화면 기본 대기 자세, 오른쪽을 보고 섬 | Square placeholder 교체용 |
| Hero_Portrait_Default | 대화 초상화(기본) | 512x512 | Assets/SampleImage/Battle/Portraits/ | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, front view, facing camera, head-and-shoulders portrait, calm neutral expression, closed mouth` | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, front view, facing camera, bust only, neutral calm expression, symmetrical` | 대화창 기본 표정 초상화 | 대화 UI 초상화 슬롯용, 배치경로는 T3 범위와 별도 |
| Hero_Portrait_Joy | 대화 초상화(기쁨) | 512x512 | Assets/SampleImage/Battle/Portraits/ | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, front view, facing camera, head-and-shoulders portrait, big bright smile, eyes closed happily` | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, front view, facing camera, bust only, cheerful happy expression, symmetrical` | 성공·보상 시 기쁜 표정 | |
| Hero_Portrait_Worried | 대화 초상화(당황) | 512x512 | Assets/SampleImage/Battle/Portraits/ | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, front view, facing camera, head-and-shoulders portrait, flustered worried expression, sweat drop, wide eyes` | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, front view, facing camera, bust only, flustered nervous expression, symmetrical` | 실패·위기 시 당황한 표정 | |
| Hero_Attack | 공격 포즈 | 1024x1024 | Assets/SampleImage/Battle/Characters/ | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, side view, facing right, dynamic attack pose, lunging forward, fist swinging, action lines, full body` | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, front view, facing camera, dynamic attack action pose, lunging forward, full body, single character` | 공격 스킬 연출용 포즈 | attackSkill 연출 스프라이트 |
| Hero_Defense | 방어 포즈 | 1024x1024 | Assets/SampleImage/Battle/Characters/ | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, side view, facing right, defensive guard pose, arms crossed in front braced, crouched stance, full body` | `office worker hero, white dress shirt with loosened navy necktie, one sleeve rolled up, black slacks, ID lanyard around neck, short messy black hair, tired determined eyes, front view, facing camera, defensive guard pose, arms crossed braced, full body, single character` | 방어 스킬 연출용 포즈 | guardSkill 연출 스프라이트 |

### 2-a-2. Char_Cook 상세 프롬프트

캐릭터 세계관: 요리 담당 파티원(선택 캐릭터). 사무복 위에 앞치마를 걸친 모습.
**캐릭터 고정 설명 문구(모든 프롬프트에 동일하게 삽입)**:
`cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | Tripo 3D용 프롬프트(영문) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|---|
| Cook_Stand | 기본 서 있는 스프라이트 | 1024x1024 | Assets/SampleImage/Battle/Characters/ | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, side view, facing right, standing idle pose, arms relaxed at sides, full body` | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, front view, facing camera, neutral T-pose, full body, single character, symmetrical, clean toon low-poly style, reference pose for 3D model` | 파티원 기본 대기 자세 | 선택 캐릭터, 코드 스프라이트 필드 미구현(비고 T1 참고) |
| Cook_Portrait_Default | 대화 초상화(기본) | 512x512 | Assets/SampleImage/Battle/Portraits/ | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, front view, facing camera, head-and-shoulders portrait, calm neutral expression, closed mouth` | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, front view, facing camera, bust only, neutral calm expression, symmetrical` | 대화창 기본 표정 초상화 | |
| Cook_Portrait_Joy | 대화 초상화(기쁨) | 512x512 | Assets/SampleImage/Battle/Portraits/ | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, front view, facing camera, head-and-shoulders portrait, big bright smile, eyes closed happily` | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, front view, facing camera, bust only, cheerful happy expression, symmetrical` | 성공·보상 시 기쁜 표정 | |
| Cook_Portrait_Worried | 대화 초상화(당황) | 512x512 | Assets/SampleImage/Battle/Portraits/ | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, front view, facing camera, head-and-shoulders portrait, flustered worried expression, sweat drop, wide eyes` | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, front view, facing camera, bust only, flustered nervous expression, symmetrical` | 실패·위기 시 당황한 표정 | |
| Cook_Attack | 공격 포즈 | 1024x1024 | Assets/SampleImage/Battle/Characters/ | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, side view, facing right, dynamic attack pose, swinging ladle like a weapon, action lines, full body` | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, front view, facing camera, dynamic attack action pose, swinging ladle, full body, single character` | 공격 스킬 연출용 포즈 | |
| Cook_Defense | 방어 포즈 | 1024x1024 | Assets/SampleImage/Battle/Characters/ | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, side view, facing right, defensive guard pose, holding pot lid as a shield, crouched stance, full body` | `cook-duty teammate, white dress shirt with tan kitchen apron over it, sleeves rolled up, red-and-white chef bandana, ladle tucked in apron pocket, front view, facing camera, defensive guard pose, holding pot lid as shield, full body, single character` | 방어 스킬 연출용 포즈 | |

### 2-a-3. Char_Fashion 상세 프롬프트

캐릭터 세계관: 패션 담당 파티원(선택 캐릭터). 사무복에 화려한 액세서리를 더한 모습.
**캐릭터 고정 설명 문구(모든 프롬프트에 동일하게 삽입)**:
`fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | Tripo 3D용 프롬프트(영문) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|---|
| Fashion_Stand | 기본 서 있는 스프라이트 | 1024x1024 | Assets/SampleImage/Battle/Characters/ | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, side view, facing right, standing idle pose, arms relaxed at sides, full body` | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, front view, facing camera, neutral T-pose, full body, single character, symmetrical, clean toon low-poly style, reference pose for 3D model` | 파티원 기본 대기 자세 | 선택 캐릭터, 코드 스프라이트 필드 미구현(비고 T1 참고) |
| Fashion_Portrait_Default | 대화 초상화(기본) | 512x512 | Assets/SampleImage/Battle/Portraits/ | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, front view, facing camera, head-and-shoulders portrait, calm neutral expression, closed mouth` | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, front view, facing camera, bust only, neutral calm expression, symmetrical` | 대화창 기본 표정 초상화 | |
| Fashion_Portrait_Joy | 대화 초상화(기쁨) | 512x512 | Assets/SampleImage/Battle/Portraits/ | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, front view, facing camera, head-and-shoulders portrait, big bright smile, eyes closed happily` | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, front view, facing camera, bust only, cheerful happy expression, symmetrical` | 성공·보상 시 기쁜 표정 | |
| Fashion_Portrait_Worried | 대화 초상화(당황) | 512x512 | Assets/SampleImage/Battle/Portraits/ | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, front view, facing camera, head-and-shoulders portrait, flustered worried expression, sweat drop, wide eyes` | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, front view, facing camera, bust only, flustered nervous expression, symmetrical` | 실패·위기 시 당황한 표정 | |
| Fashion_Attack | 공격 포즈 | 1024x1024 | Assets/SampleImage/Battle/Characters/ | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, side view, facing right, dynamic attack pose, whipping the scarf like a weapon, action lines, full body` | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, front view, facing camera, dynamic attack action pose, whipping scarf, full body, single character` | 공격 스킬 연출용 포즈 | |
| Fashion_Defense | 방어 포즈 | 1024x1024 | Assets/SampleImage/Battle/Characters/ | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, side view, facing right, defensive guard pose, handbag held up as a shield, crouched stance, full body` | `fashion-duty teammate, dress shirt with colorful patterned scarf, oversized statement sunglasses on head, mismatched bright-colored accessories, stylish thin belt over office trousers, front view, facing camera, defensive guard pose, handbag held up as shield, full body, single character` | 방어 스킬 연출용 포즈 | |

### 2-a-4. T2 완료 확인 및 범위 밖 사항

- 완료 기준 확인: Hero/Char_Cook/Char_Fashion 각 6종(기본/초상화3종/공격/방어, 4종 이상 충족) 프롬프트 모두 채워짐. 캐릭터별 고정 설명 문구가 2D·3D 프롬프트 모두에 동일하게 삽입됨(grep 검증: 정의 1줄 + 행 6줄 = 7줄 일치, 3명 공통).
- 범위 밖(후속 작업 필요, 이 문서는 프롬프트 텍스트만 제공):
  - Char_Cook·Char_Fashion는 현재 코드(BattleUnitView 등)에 스프라이트를 연결할 필드가 없다. 실제 이미지를 붙이려면 별도 구현 작업(필드 추가 + 캐릭터 선택 로직)이 먼저 필요하다.
  - 이 문서의 Tripo/2D 프롬프트로 실제 이미지를 생성하고 `Assets/SampleImage/Battle/Characters/`, `.../Portraits/`에 배치하는 작업, Texture Type 설정, 씬·프리팹 연결(Unity MCP)은 사람 또는 후속 작업에서 수행한다.
- T2.2 재확인: 위 두 범위 밖 항목은 코드/씬 구현과 실제 이미지 생성이 필요한 별도 작업이라 T2 프롬프트 문서 범위에서는 더 진행할 것이 없다. T2(캐릭터 프롬프트) 완료 기준 충족으로 최종 마감.

### 2-b. 몬스터·보스 (Assets/Data/Battle/*.asset, Stage1/*.asset)

세계관: 각 몬스터는 담당 "불편(불쾌·성가심) 유형"을 몸으로 형상화한다(D-11: 캐릭터마다 담당 불편 유형이 다르다는 설정을
몬스터 쪽에도 적용). `displayName`(한글) 기준으로 테마를 정했다.
- **Grunt**(잡몹, 공용 최하급) / **MidBoss**(중간보스, 공용 중형): 스테이지 불문 공용 등장 — "쌓인 사무 업무" 그 자체를
  몬스터화(문서·마감 스탬프 모티프). MidBoss는 Grunt의 확대·강화판.
- **S1_Mosquito**(모기): 여름밤 사무실의 날벌레 불편 유형. **S1_TrashBag**(쓰레기봉투): 방치된 쓰레기·악취 불편 유형.
  스테이지1 잡몹 두 종.
- **Boss_Stage1**(불편대왕): 스테이지1 최종 보스. 데이터 `displayName`은 "불편대왕"이며, §2-c-1의 DLG 화자
  "불편의 악마"(Devil_Portrait_Default)와 이름이 정확히 일치하지 않는다(비고에 근거 기록). 전신 몬스터 프롬프트는
  §2-c-1 Devil_Portrait 고정 문구(양복·뿔·슬릭백 헤어)를 그대로 이어받아 같은 존재로 통일한다(빌드 단계 결정,
  근거: 세계관상 "불편"을 의인화한 존재가 스테이지1에 둘일 이유가 없고, 초상화·전신 모두 같은 실루엣을 쓰는 편이
  제작·인식 일관성이 높음).

각 몬스터는 상태 3~4종(기본/피격/공격(=일반 또는 특수공격 예고)) + 머리 위 행동 예고 아이콘을 쓴다.
예고 아이콘은 몬스터마다 새로 만들지 않고 §2-i(Intent_Attack/Intent_Guard/Intent_Special, BattleUnitView.intentIcon
공용 슬롯)를 그대로 재사용한다 — 코드 상 아이콘이 몬스터별이 아니라 슬롯 공용이라서다. 대응 관계:
일반공격(id: basic/heavy) → Intent_Attack, 대기·충전(id: rest) → Intent_Guard, 특수공격 예고(kind=2, id: special_*) → Intent_Special.

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| Grunt | 몬스터 | Assets/Data/Battle/Grunt.asset | BattleUnitView.body, MonsterView0~2 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | §2-b-1 상세표 참고(기본/피격/공격 3칸) | Battle.unity MonsterView는 Circle placeholder |
| MidBoss | 몬스터(중간보스) | Assets/Data/Battle/Rooms/MidBoss.asset | 위와 동일, 중형 보스 룸 | 1536x1536 | Assets/SampleImage/Battle/Monsters/ | §2-b-2 상세표 참고(기본/피격/공격 3칸) | Grunt 확대·강화판 |
| S1_Mosquito | 몬스터 | Assets/Data/Battle/Stage1/S1_Mosquito.asset | 위와 동일, 스테이지1 잡몹 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | §2-b-3 상세표 참고(기본/피격/공격 3칸) | |
| S1_TrashBag | 몬스터 | Assets/Data/Battle/Stage1/S1_TrashBag.asset | 위와 동일, 스테이지1 잡몹 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | §2-b-4 상세표 참고(기본/피격/공격 3칸) | |
| Boss_Stage1 | 몬스터(보스) | Assets/Data/Battle/Stage1/Boss_Stage1.asset | 위와 동일, 스테이지1 보스 + 보스 전용 미니게임(특수 방) 등장 | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | §2-b-5 상세표 참고(기본/피격/공격/페이즈2 예고 4칸) | displayName="불편대왕", DLG "불편의 악마"와 동일 존재로 통일(위 비고) |
| S1_RubberGloveShield | 몬스터(방어형, T6/T9) | Assets/Data/Battle/Stage1/S1_RubberGloveShield.asset | 위와 동일, 스테이지1 신규 잡몹(Guard 위주) | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `hand-drawn ink line art, flat single-tone color fill, simple cel shading, sketchbook doodle style matching art_pizza.png, transparent background, clean thin outline, no background scenery, isolated subject, giant yellow rubber kitchen glove monster with a round dish-sponge body fused to its palm, stubby fingers held up like a shield, side view, facing left, defensive braced pose, full body` | 방어 위주(Guard 행동 다수), 틴트 placeholder 사용 중(전용 스프라이트 미제작) |
| S1_AngryMixer | 몬스터(공격형, T6/T9) | Assets/Data/Battle/Stage1/S1_AngryMixer.asset | 위와 동일, 스테이지1 신규 잡몹(잦은 저배율 공격) | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `hand-drawn ink line art, flat single-tone color fill, simple cel shading, sketchbook doodle style matching art_pizza.png, transparent background, clean thin outline, no background scenery, isolated subject, angry hand-mixer monster with spinning twin beater arms, cord tail whipping behind, small angry face on the motor housing, side view, facing left, aggressive lunging pose, action lines, full body` | 연타 대신 저배율 잦은 공격(jab)으로 구현, 틴트 placeholder |
| S1_StaticFairy | 몬스터(마법형, T6/T9) | Assets/Data/Battle/Stage1/S1_StaticFairy.asset | 위와 동일, 스테이지1 신규 잡몹(Magic+상태이상) | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `hand-drawn ink line art, flat single-tone color fill, simple cel shading, sketchbook doodle style matching art_pizza.png, transparent background, clean thin outline, no background scenery, isolated subject, tiny static-electricity fairy monster made of crackling blue sparks, frizzy spiked hair from static, sock-shaped wings, side view, facing left, casting a spell pose with sparks gathering in hand, magic glow, full body` | Magic 행동으로 상태이상(D-7 4종 중 하나) 부여, 틴트 placeholder |
| S2_TangledCable | 몬스터(2스테이지 잡몹, T7/T9) | Assets/Data/Battle/Stage2/S2_TangledCable.asset | 위와 동일, 스테이지2 신규 잡몹 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `hand-drawn ink line art, flat single-tone color fill, simple cel shading, sketchbook doodle style matching art_pizza.png, transparent background, clean thin outline, no background scenery, isolated subject, sentient tangled cable-knot monster, snake-like body made of knotted charging cables, plug-shaped head, side view, facing left, idle coiled pose, full body` | Stage_2 배율(HP×1.3, 공격×1.2) 적용 |
| S2_JammedPrinter | 몬스터(2스테이지 잡몹, T7/T9) | Assets/Data/Battle/Stage2/S2_JammedPrinter.asset | 위와 동일, 스테이지2 신규 잡몹(방어형) | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `hand-drawn ink line art, flat single-tone color fill, simple cel shading, sketchbook doodle style matching art_pizza.png, transparent background, clean thin outline, no background scenery, isolated subject, jammed office printer monster, paper sheets jammed half-out of its mouth like teeth, blinking error-light eye, boxy body on stubby legs, side view, facing left, defensive braced pose, full body` | Guard 위주 행동 |
| S2_PopupAd | 몬스터(2스테이지 잡몹, T7/T9) | Assets/Data/Battle/Stage2/S2_PopupAd.asset | 위와 동일, 스테이지2 신규 잡몹(마법형) | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `hand-drawn ink line art, flat single-tone color fill, simple cel shading, sketchbook doodle style matching art_pizza.png, transparent background, clean thin outline, no background scenery, isolated subject, obnoxious pop-up ad window monster, rectangular browser-window body with a tiny red X close-button eye, flashing banner-ad limbs, side view, facing left, casting a spell pose with glowing banner beams, full body` | Magic + 상태이상 부여 |
| Boss_Stage2 | 몬스터(2스테이지 보스, T7/T9) | Assets/Data/Battle/Stage2/Boss_Stage2.asset | 위와 동일, 스테이지2 보스(2페이즈) | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | §2-b-6 상세표 참고(기본/피격/공격/페이즈2 예고 4칸) | 2페이즈 전환 시 Guard/Magic 행동 비중 증가(D-16), 전용 특수 미니게임 필요 |

### 2-b-6. Boss_Stage2 상세 프롬프트

**고정 설명 문구**: `giant malfunctioning office router-modem boss monster, tangled ethernet cable tentacles, flickering signal-bar antenna crown, cracked plastic shell glowing with error-red light, imposing hovering form`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| Boss_Stage2_Stand | 기본 대기(1페이즈) | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | `giant malfunctioning office router-modem boss monster, tangled ethernet cable tentacles, flickering signal-bar antenna crown, cracked plastic shell glowing with error-red light, imposing hovering form, side view, facing left, idle pose, full body` | 스테이지2 보스 기본 대기 | |
| Boss_Stage2_Hit | 피격 반응 | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | `giant malfunctioning office router-modem boss monster, tangled ethernet cable tentacles, flickering signal-bar antenna crown, cracked plastic shell glowing with error-red light, imposing hovering form, side view, facing left, flinching hit reaction, sparks flying from cracked shell, full body` | 피격 시 스파크가 튀며 움찔 | |
| Boss_Stage2_Attack | 공격/특수공격 예고·모션 | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | `giant malfunctioning office router-modem boss monster, tangled ethernet cable tentacles, flickering signal-bar antenna crown, cracked plastic shell glowing with error-red light, imposing hovering form, side view, facing left, cable tentacles lashing forward attack pose, action lines, full body` | 일반·특수 공격 연출 | |
| Boss_Stage2_Phase2 | 페이즈2 전환 예고 | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | `giant malfunctioning office router-modem boss monster, tangled ethernet cable tentacles, flickering signal-bar antenna crown, cracked plastic shell glowing with error-red light, imposing hovering form, side view, facing left, overloaded enraged transformation pose, shell cracking further, intense red glow surge, full body` | 2페이즈 전환 시(Guard/Magic 비중 증가) 연출 | |

### 2-b-1. Grunt 상세 프롬프트

**고정 설명 문구**: `paperwork clutter grunt monster, crumpled memo-paper stack body, stubby paper arms, red "URGENT" stamp mark on forehead, small round hovering form`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| Grunt_Stand | 기본 대기, BattleUnitView.body | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `paperwork clutter grunt monster, crumpled memo-paper stack body, stubby paper arms, red "URGENT" stamp mark on forehead, small round hovering form, side view, facing left, idle pose, full body` | 잡몹 기본 대기 자세 | Circle placeholder 교체용 |
| Grunt_Hit | 피격 반응 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `paperwork clutter grunt monster, crumpled memo-paper stack body, stubby paper arms, red "URGENT" stamp mark on forehead, small round hovering form, side view, facing left, flinching hit reaction, papers scattering, body tilted back, full body` | 피격 시 종이가 흩날리며 움찔 | |
| Grunt_Attack | 공격(일반/heavy) 예고·모션 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `paperwork clutter grunt monster, crumpled memo-paper stack body, stubby paper arms, red "URGENT" stamp mark on forehead, small round hovering form, side view, facing left, lunging attack pose, paper arm swinging forward, action lines, full body` | 일반공격(basic/heavy) 연출, 머리 위 Intent_Attack 아이콘과 함께 표시 | |

### 2-b-2. MidBoss 상세 프롬프트

**고정 설명 문구**: `towering overtime-document tower monster, stacked binder folders forming a tall body, binder-clip claws, red deadline stamp glowing on chest, imposing hovering form`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| MidBoss_Stand | 기본 대기 | 1536x1536 | Assets/SampleImage/Battle/Monsters/ | `towering overtime-document tower monster, stacked binder folders forming a tall body, binder-clip claws, red deadline stamp glowing on chest, imposing hovering form, side view, facing left, idle pose, full body` | 중간보스 기본 대기, Grunt보다 크고 위압적 | |
| MidBoss_Hit | 피격 반응 | 1536x1536 | Assets/SampleImage/Battle/Monsters/ | `towering overtime-document tower monster, stacked binder folders forming a tall body, binder-clip claws, red deadline stamp glowing on chest, imposing hovering form, side view, facing left, flinching hit reaction, folders shaking loose, full body` | 피격 시 바인더가 흔들리며 움찔 | |
| MidBoss_Attack | 특수공격(special_weaken/drain) 예고·모션 | 1536x1536 | Assets/SampleImage/Battle/Monsters/ | `towering overtime-document tower monster, stacked binder folders forming a tall body, binder-clip claws, red deadline stamp glowing on chest, imposing hovering form, side view, facing left, raising binder-clip claws high, ominous glow, charging special attack, action lines, full body` | 특수공격 예고 연출, 머리 위 Intent_Special 아이콘과 함께 표시 | |

### 2-b-3. S1_Mosquito 상세 프롬프트

**고정 설명 문구**: `giant office mosquito monster, oversized proboscis, thin buzzing wings, tiny loosened necktie around its neck, spindly legs`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| S1_Mosquito_Stand | 기본 대기 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `giant office mosquito monster, oversized proboscis, thin buzzing wings, tiny loosened necktie around its neck, spindly legs, side view, facing left, hovering idle pose, full body` | 여름밤 모기 불편 유형, 스테이지1 잡몹 | |
| S1_Mosquito_Hit | 피격 반응 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `giant office mosquito monster, oversized proboscis, thin buzzing wings, tiny loosened necktie around its neck, spindly legs, side view, facing left, flinching hit reaction, wings crumpled, full body` | 피격 시 날개가 구겨지며 움찔 | |
| S1_Mosquito_Attack | 특수공격(special_rush) 예고·모션 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `giant office mosquito monster, oversized proboscis, thin buzzing wings, tiny loosened necktie around its neck, spindly legs, side view, facing left, rushing dive attack pose, proboscis thrust forward, speed lines, full body` | 급습(special_rush) 예고 연출, 머리 위 Intent_Special 아이콘과 함께 표시 | |

### 2-b-4. S1_TrashBag 상세 프롬프트

**고정 설명 문구**: `sentient black trash-bag monster, knotted tie at top like messy hair, grime drips, food scrap debris poking out of torn seams, stubby limbs`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| S1_TrashBag_Stand | 기본 대기 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `sentient black trash-bag monster, knotted tie at top like messy hair, grime drips, food scrap debris poking out of torn seams, stubby limbs, side view, facing left, idle pose, full body` | 방치된 쓰레기·악취 불편 유형, 스테이지1 잡몹 | |
| S1_TrashBag_Hit | 피격 반응 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `sentient black trash-bag monster, knotted tie at top like messy hair, grime drips, food scrap debris poking out of torn seams, stubby limbs, side view, facing left, flinching hit reaction, seam tearing further, full body` | 피격 시 이음새가 더 찢어지며 움찔 | |
| S1_TrashBag_Attack | 특수공격(special_costup) 예고·모션 | 1024x1024 | Assets/SampleImage/Battle/Monsters/ | `sentient black trash-bag monster, knotted tie at top like messy hair, grime drips, food scrap debris poking out of torn seams, stubby limbs, side view, facing left, hurling debris attack pose, stinky green fume cloud, action lines, full body` | 코스트 증가(special_costup) 예고 연출, 머리 위 Intent_Special 아이콘과 함께 표시 | |

### 2-b-5. Boss_Stage1 상세 프롬프트

**고정 설명 문구**: `demonic office devil king, sharp business suit, small curved horns, slicked-back hair, sly menacing smirk, tall imposing full body`
(§2-c-1 Devil_Portrait_Default와 동일 캐릭터 — 상반신 버스트 프롬프트에서 이어받아 전신·측면으로 확장)

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| Boss_Stage1_Stand | 기본 대기(페이즈1) | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | `demonic office devil king, sharp business suit, small curved horns, slicked-back hair, sly menacing smirk, tall imposing full body, side view, facing left, idle pose, arms crossed, full body` | 스테이지1 최종 보스, 불편을 의인화한 존재 | isBoss=1 |
| Boss_Stage1_Hit | 피격 반응 | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | `demonic office devil king, sharp business suit, small curved horns, slicked-back hair, sly menacing smirk, tall imposing full body, side view, facing left, flinching hit reaction, suit jacket ruffled, full body` | 피격 시 정장이 흐트러지며 움찔 | |
| Boss_Stage1_Attack | 특수공격(special_burn/costup) 예고·모션 | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | `demonic office devil king, sharp business suit, small curved horns, slicked-back hair, sly menacing smirk, tall imposing full body, side view, facing left, raising clawed hand with dark flame aura, dramatic charging pose, action lines, full body` | 페이즈1 특수공격 예고, 머리 위 Intent_Special 아이콘과 함께 표시 | |
| Boss_Stage1_Phase2 | 페이즈2 전환(phase2Threshold=0.5) 특수공격 예고·모션 | 2048x2048 | Assets/SampleImage/Battle/Monsters/ | `demonic office devil king, sharp business suit, small curved horns, slicked-back hair, sly menacing smirk, tall imposing full body, side view, facing left, torn ragged suit, glowing red eyes, intensified dark flame aura, enraged expression, cooldown shortened aggressive stance, full body` | HP 50% 이하 페이즈2, 더 위협적인 외형으로 변화(찢긴 정장·붉게 빛나는 눈) | special_burn_phase2 예고, 머리 위 Intent_Special 아이콘과 함께 표시 |

### 2-c. HQ 요원(NPC) 전신·초상화 (Assets/Data/Hub/Agents/*.asset + DLG_* 화자)

NPC 세계관: 세 요원은 모두 COMFORT 본부(HQ) 소속 직원으로, 플레이어(불편맨)를 담당 업무별로 지원한다.
`displayName`(한글)과 `role`(0=Upgrade,1=Module,2=Story) 기준으로 성격을 정했다.
- **강화 요원**(Agent_Upgrade, role=0): 영구 강화(스탯 성장) 담당. 체육 코치/트레이너 톤 — 활기차고 밀어붙이는 성격.
- **모듈 요원**(Agent_Module, role=1): 특수 모듈(장비) 담당. 엔지니어 톤 — 차분하고 꼼꼼한 성격.
- **스토리 요원**(Agent_Story, role=2): 스토리 진행(대화) 담당. 기록관/사서 톤 — 조용하고 다정한 성격.

**공통 고정 문구(세 요원 모두, 직급장 표시로 통일감)**: `COMFORT HQ staff, ID badge with COMFORT logo on chest`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| Agent_Upgrade_Full | NPC 전신, HQ 씬 AgentButton_Upgrade(현재 sprite=null) | 1024x1024 | Assets/SampleImage/Hub/Agents/ | `COMFORT HQ staff, ID badge with COMFORT logo on chest, athletic drill-coach woman, tracksuit vest over dress shirt, whistle on neck strap, clipboard tucked under arm, confident grin, front view, facing camera, standing idle pose, full body` | 강화 요원 전신, 밝고 파이팅 넘치는 코치 인상 | role=Upgrade |
| Agent_Upgrade_Portrait_Default | 대화 초상화(기본) | 512x512 | Assets/SampleImage/Hub/Agents/ | `COMFORT HQ staff, ID badge with COMFORT logo on chest, athletic drill-coach woman, tracksuit vest over dress shirt, whistle on neck strap, front view, facing camera, head-and-shoulders portrait, calm confident expression, closed mouth` | 대화창 기본 표정 | AgentDefinition.portrait |
| Agent_Upgrade_Portrait_Encourage | 대화 초상화(격려) | 512x512 | Assets/SampleImage/Hub/Agents/ | `COMFORT HQ staff, ID badge with COMFORT logo on chest, athletic drill-coach woman, tracksuit vest over dress shirt, whistle on neck strap, front view, facing camera, head-and-shoulders portrait, big energetic grin, thumbs up gesture implied by shoulder angle` | 강화 성공·독려 대사용 | |
| Agent_Module_Full | NPC 전신, HQ 씬 AgentButton_Module | 1024x1024 | Assets/SampleImage/Hub/Agents/ | `COMFORT HQ staff, ID badge with COMFORT logo on chest, meticulous engineer man, dress shirt under open tool-pocket vest, safety goggles pushed up on forehead, wrench in hand, front view, facing camera, standing idle pose, full body` | 모듈 요원 전신, 차분한 엔지니어 인상 | role=Module |
| Agent_Module_Portrait_Default | 대화 초상화(기본) | 512x512 | Assets/SampleImage/Hub/Agents/ | `COMFORT HQ staff, ID badge with COMFORT logo on chest, meticulous engineer man, dress shirt under open tool-pocket vest, safety goggles pushed up on forehead, front view, facing camera, head-and-shoulders portrait, calm neutral expression, closed mouth` | 대화창 기본 표정 | AgentDefinition.portrait |
| Agent_Module_Portrait_Thinking | 대화 초상화(고민) | 512x512 | Assets/SampleImage/Hub/Agents/ | `COMFORT HQ staff, ID badge with COMFORT logo on chest, meticulous engineer man, dress shirt under open tool-pocket vest, safety goggles pushed up on forehead, front view, facing camera, head-and-shoulders portrait, thoughtful squint, hand near chin` | 모듈 설명·안내 대사용 | |
| Agent_Story_Full | NPC 전신, HQ 씬 AgentButton_Story | 1024x1024 | Assets/SampleImage/Hub/Agents/ | `COMFORT HQ staff, ID badge with COMFORT logo on chest, gentle archivist woman, cardigan over dress shirt, round glasses, holding a small tablet against chest, front view, facing camera, standing idle pose, full body` | 스토리 요원 전신, 조용한 사서 인상 | role=Story |
| Agent_Story_Portrait_Default | 대화 초상화(기본) | 512x512 | Assets/SampleImage/Hub/Agents/ | `COMFORT HQ staff, ID badge with COMFORT logo on chest, gentle archivist woman, cardigan over dress shirt, round glasses, front view, facing camera, head-and-shoulders portrait, soft warm smile, closed mouth` | 대화창 기본 표정 | AgentDefinition.portrait |
| Agent_Story_Portrait_Concerned | 대화 초상화(걱정) | 512x512 | Assets/SampleImage/Hub/Agents/ | `COMFORT HQ staff, ID badge with COMFORT logo on chest, gentle archivist woman, cardigan over dress shirt, round glasses, front view, facing camera, head-and-shoulders portrait, gentle worried frown, eyebrows raised` | 스토리 위기 장면 대사용 | |

### 2-c-1. 그 외 DLG_* 화자 초상화 (Agent_* 데이터에 없는 speakerName)

`Assets/Data/Dialogue/DLG_*.asset`의 `speakerName`을 grep으로 모두 뽑아 대조한 결과, 위 세 요원 외에 아래 화자가 더 있다.
(`불편맨`은 §2-a-1 Hero_Portrait_*로 이미 커버됨 — 별도 항목 없음.)

| 항목 | 화자(speakerName) | 등장 에셋 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|---|---|
| Coworker_Portrait_Default | 회사원 | DLG_Tutorial_Boss, DLG_Tutorial_Opening, DLG_Tutorial_Outro | 튜토리얼 배경 동료 캐릭터 대사 | 512x512 | Assets/SampleImage/Dialogue/Speakers/ | `plain office coworker, generic gray dress shirt, no necktie, plain black-rimmed glasses, tired slouched posture, front view, facing camera, head-and-shoulders portrait, awkward nervous smile` | 이름 없는 동료, 불편맨과 대비되는 평범한 직장인 | Agent_* 데이터 없음, DLG_*에만 등장 |
| Devil_Portrait_Default | 불편의 악마 | DLG_Tutorial_Opening, DLG_Tutorial_Outro | 튜토리얼 악역 내레이션 대사 | 512x512 | Assets/SampleImage/Dialogue/Speakers/ | `demonic office devil, sharp business suit, small curved horns, slicked-back hair, sly menacing smirk, front view, facing camera, head-and-shoulders portrait, dramatic sharp shadow under eyes` | Boss_Stage1과 다른 대화 전용 상반신 버스트(전신 몬스터 스프라이트는 §2-b/T4에서 별도 제작) | Assets/Data/Battle/Stage1/Boss_Stage1.asset과 세계관상 동일 존재 추정, 근거는 DLG_*뿐 |
| ComfortCommander_Portrait_Default | COMFORT 사령관 | DLG_Tutorial_Outro | 엔딩 직후 상부 지시 대사 | 512x512 | Assets/SampleImage/Dialogue/Speakers/ | `COMFORT commander, formal dark uniform with gold COMFORT insignia on shoulder, stern authoritative posture, short neat hair, front view, facing camera, head-and-shoulders portrait, cold serious expression` | COMFORT 조직의 상부 인물, Agent_*보다 상급자 | Agent_* 데이터 없음, DLG_*에만 등장 |
| Guide_Portrait_Default | 안내 | DLG_Tutorial_Command, DLG_Tutorial_Energy, DLG_Tutorial_Evasion | 조작 튜토리얼 안내 음성(비인간 화자로 설계) | 512x512 | Assets/SampleImage/Dialogue/Speakers/ | `COMFORT HQ AI guide hologram, glowing simple orb with a thin ring, soft blue light, minimal facial features (single glowing eye line), front view, facing camera, floating, symmetrical` | 사람이 아닌 HQ 시스템 안내 마스코트로 디자인(회사원·요원과 겹치지 않게) | 디자인 결정: 얼굴 없는 홀로그램으로 구체화(비고에 근거 명시) |

### 2-c-2. 대화창 프레임 (DialogueFrame 교체용)

`Assets/Scripts/UI/DialogueBox.cs`는 `root`(배경 패널) + `portrait`(Image) + `nameText` + `bodyText`로 구성되며
프레임 전용 필드 이름은 없다 — `root`에 붙는 배경 이미지(9-slice 패널)가 교체 대상이다.

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| DialogueFrame_Panel | 대화창 배경 패널(9-slice), DialogueBox.root 배경 Image | 1920x360 (9-slice, 코너 64px) | Assets/SampleImage/UI/Frames/ | `flat UI dialogue box frame, thick rounded rectangle border, cream paper texture fill, simple corner rivets, front view, orthographic UI panel, no character, empty interior` | 대화 박스 9-slice 프레임, 안쪽은 비워둠(텍스트·초상화가 위에 얹힘) | UI 배경형이므로 side/front 규칙 예외, 정면 평면 |
| DialogueFrame_PortraitSlot | 초상화 액자 테두리, DialogueBox.portrait 뒤/둘레 | 512x512 | Assets/SampleImage/UI/Frames/ | `flat UI portrait frame, simple rounded square border matching DialogueFrame_Panel style, cream paper texture edge, front view, orthographic UI element, empty transparent center` | 초상화 Image 뒤에 깔리는 얇은 액자 | 선택 사항, 생략 가능(간단히 portrait Image만 써도 됨) |

### 2-d. 스킬 아이콘 (Assets/Data/Battle/*.asset)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| AttackSkill | 스킬 아이콘 | Assets/Data/Battle/AttackSkill.asset | SkillDefinition.icon, SkillButton0~2 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, plain fist striking forward inside a circular badge frame, single bold impact line, front view` | Battle.unity SkillButton은 UISprite placeholder |
| GuardSkill | 스킬 아이콘 | Assets/Data/Battle/GuardSkill.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, simple round shield inside a circular badge frame, front view` | |
| HeavyBlowSkill | 스킬 아이콘 | Assets/Data/Battle/HeavyBlowSkill.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, clenched fist with radiating motion lines inside a circular badge frame, heavier and bigger than AttackSkill icon, front view` | Upgrade_LearnSkill_HeavyBlow로 습득, AttackSkill과 계열 공유하되 임팩트 크게 |
| PurifyStrikeSkill | 스킬 아이콘 | Assets/Data/Battle/PurifyStrikeSkill.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, fist wrapped in a small sparkling crystal shard inside a circular badge frame, front view` | 정화 결정(에너지) 모티프를 공격 아이콘에 결합 |

### 2-e. 전투 강화(Upgrade_*) 아이콘 (Assets/Data/Battle/Upgrades/*.asset)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| Upgrade_Hp | 강화 아이콘 | Assets/Data/Battle/Upgrades/Upgrade_Hp.asset | Battle.unity UpgradePanel ChoiceButton | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small red heart with an upward arrow inside a square badge frame with rounded corners, front view` | |
| Upgrade_Attack | 강화 아이콘 | Assets/Data/Battle/Upgrades/Upgrade_Attack.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small fist with an upward arrow inside a square badge frame with rounded corners, front view` | |
| Upgrade_Defense | 강화 아이콘 | Assets/Data/Battle/Upgrades/Upgrade_Defense.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small round shield with an upward arrow inside a square badge frame with rounded corners, front view` | |
| Upgrade_EnergyAbsorb | 강화 아이콘 | Assets/Data/Battle/Upgrades/Upgrade_EnergyAbsorb.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small crystal shard being drawn into a magnet-like swirl inside a square badge frame with rounded corners, front view` | PU_EnergyAbsorb와 같은 모티프(크리스탈 흡수) 공유, 배지 테두리로 강화(사각)/영구강화(원형) 구분 |
| Upgrade_EnergyCap | 강화 아이콘 | Assets/Data/Battle/Upgrades/Upgrade_EnergyCap.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small crystal-shaped gauge container with an upward arrow inside a square badge frame with rounded corners, front view` | |
| Upgrade_LearnSkill_HeavyBlow | 강화 아이콘 | Assets/Data/Battle/Upgrades/Upgrade_LearnSkill_HeavyBlow.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, clenched fist with radiating motion lines plus a small plus sign, inside a square badge frame with rounded corners, front view` | HeavyBlowSkill 아이콘과 같은 모티프에 습득 표시(플러스 기호) 추가 |
| Upgrade_SkillPower | 강화 아이콘 | Assets/Data/Battle/Upgrades/Upgrade_SkillPower.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small sparkling star burst with an upward arrow inside a square badge frame with rounded corners, front view` | |

### 2-f. 영구 강화(PU_*) 아이콘 (Assets/Data/PermanentUpgrades/*.asset)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| PU_MaxHp | 영구강화 아이콘 | Assets/Data/PermanentUpgrades/PU_MaxHp.asset | HQ AgentMenuPanel UpgradeSection PU_MaxHp_Row | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small red heart with an upward arrow inside a circular badge frame with a thin gold ring, front view` | Upgrade_Hp와 같은 모티프, 원형+금테로 "영구" 구분 |
| PU_Attack | 영구강화 아이콘 | Assets/Data/PermanentUpgrades/PU_Attack.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small fist with an upward arrow inside a circular badge frame with a thin gold ring, front view` | Upgrade_Attack과 같은 모티프, 원형+금테 |
| PU_Defense | 영구강화 아이콘 | Assets/Data/PermanentUpgrades/PU_Defense.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small round shield with an upward arrow inside a circular badge frame with a thin gold ring, front view` | Upgrade_Defense와 같은 모티프, 원형+금테 |
| PU_StartEnergy | 영구강화 아이콘 | Assets/Data/PermanentUpgrades/PU_StartEnergy.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small crystal shard with a clock/start flag hint inside a circular badge frame with a thin gold ring, front view` | 전투 시작 시 초기 에너지 보정 |
| PU_EnergyAbsorb | 영구강화 아이콘 | Assets/Data/PermanentUpgrades/PU_EnergyAbsorb.asset | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small crystal shard being drawn into a magnet-like swirl inside a circular badge frame with a thin gold ring, front view` | Upgrade_EnergyAbsorb와 같은 모티프, 원형+금테로 구분 |

### 2-g. 모듈(Module_*) 아이콘 (Assets/Data/Modules/*.asset)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| Module_FirstStrike | 모듈 아이콘 | Assets/Data/Modules/Module_FirstStrike.asset | HQ ModuleSection Module_FirstStrike_Row EquipButton | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small mechanical gear chip with a single forward-pointing lightning bolt, inside a hexagonal badge frame, front view` | 선제 공격 컨셉 |
| Module_RepeatTurn | 모듈 아이콘 | Assets/Data/Modules/Module_RepeatTurn.asset | HQ ModuleSection Module_RepeatTurn_Row EquipButton | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small mechanical gear chip with a single circular refresh arrow, inside a hexagonal badge frame, front view` | 턴 반복 컨셉 |
| Module_RepeatTurnPlus | 모듈 아이콘 | Assets/Data/Modules/Module_RepeatTurnPlus.asset | HQ ModuleSection ModuleRow_RepeatTurnPlus EquipButton | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small mechanical gear chip with a double circular refresh arrow and a small plus sign, inside a hexagonal badge frame, front view` | ModuleKind enum에 값이 없어 데이터 불일치(코드 이슈, 이번 범위 밖). Module_RepeatTurn과 같은 모티프에 plus 기호로 상위 버전 표시 |

### 2-h. 상태이상 아이콘 (Assets/Scripts/Battle/StatusEffectType.cs, D-7 기준 5종)

ROADMAP D-7: 기절(Stun)·받는 피해 증가(DamageTakenUp)는 "상태이상 목록에서 제외"로 확정됐고,
NoEnergyReserve는 "에너지 봉인"으로 재정의됐다(다음 턴 에너지 수집 페이즈 전체 스킵). 이 절은
D-7이 확정한 5종만 아이콘으로 만든다. StatusEffectType enum 자체(Stun·DamageTakenUp 포함 7값)는
코드 이슈이며 이번 범위 밖(아래 "T6 마무리" 검증 메모에 미해결 이슈로 남김).

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| Status_EnergySeal | 상태이상 아이콘 | StatusEffectType.NoEnergyReserve | 전투 유닛 상태이상 표시 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small crystal shard with a padlock over it inside a circular badge frame with a jagged red outline, front view` | 에너지 봉인(재정의: 다음 턴 에너지 수집 페이즈 스킵), D-7 |
| Status_Weaken | 상태이상 아이콘 | StatusEffectType.Weaken | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small drooping fist with downward arrow inside a circular badge frame with a jagged red outline, front view` | 쇠약: 2턴 공격력 -20%, D-7 |
| Status_Rush | 상태이상 아이콘 | StatusEffectType.Rush | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small cracked round shield with an X mark over it inside a circular badge frame with a jagged red outline, front view` | 초조: 2턴 방어 커맨드 봉쇄(회피만 가능), D-7 |
| Status_CostUp | 상태이상 아이콘 | StatusEffectType.CostUp | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small crystal shard with an upward heavy arrow pressing down on it inside a circular badge frame with a jagged red outline, front view` | 과부하: 2턴 모든 커맨드 코스트 +1, D-7 |
| Status_Burn | 상태이상 아이콘 | StatusEffectType.Burn | 위와 동일 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small simple flame inside a circular badge frame with a jagged red outline, front view` | 화상: 3턴 턴 종료 시 최대 HP 5% 고정 피해, D-7 |

### 2-i. 전투 예고 아이콘 (BattleUnitView.intentIcon)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| Intent_Attack | 예고 아이콘 | BattleUnitView.intentIcon (MonsterView0~2/IntentIcon) | 몬스터의 다음 행동 예고: 공격 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small red exclamation-style crossed claw marks inside a circular badge frame, front view` | Battle.unity IntentIcon은 Circle placeholder |
| Intent_Guard | 예고 아이콘 | 위와 동일 | 다음 행동 예고: 방어 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small blue round shield inside a circular badge frame, front view` | AttackSkill/GuardSkill과 실루엣 계열 공유하되 몬스터용은 색(빨강/파랑)으로 구분 |
| Intent_Special | 예고 아이콘 | 위와 동일 | 다음 행동 예고: 특수(미니게임 유발 등) | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small purple swirling burst with a tiny question mark inside a circular badge frame, front view` | |

### 2-j. 배경·맵 (씬: Title.unity, HQ.unity, Battle.unity)

**배경 전용 스타일 블록(§1-1 대신 §2-j 전 항목에 적용, transparent/no background scenery/isolated subject 제외)**:
```
hand-drawn ink line art, flat muted color fill, simple cel shading,
sketchbook doodle tone matching the tone of art_pizza.png reference,
full-screen 2D game background, 16:9 1920x1080 composition,
low detail near center to leave room for UI overlay, clean thin outline,
keep UI reserved zones as calm low-contrast flat areas
```
네거티브(§1-3 대신, background scenery 제외하고 UI 침범 요소 추가):
```
3D render, photorealistic, gradient shading, soft airbrush, text, watermark,
signature, characters in foreground, UI elements, cropped, blurry, extra limbs
```

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| BG_Title | 배경 | Title.unity Canvas/Background(IMG, sprite=null) | 타이틀 화면 배경 | 1920x1080 | Assets/SampleImage/Title/ | §2-j-1 참조: `COMFORT HQ exterior office building at dusk, small silhouette of a weary office-worker hero on lower-left, reserved top-center logo band (approx 5~50% height), reserved bottom 20% (≥220px) for start button, everyday-nuisance props (tangled earphones, flyers) only at edges` | |
| Logo_Title | 로고 | Title.unity Canvas/SafeArea/Logo(IMG, sprite=null) | 타이틀 로고 | 1024x512 | Assets/SampleImage/Title/ | §2-j-2 참조: `hand-drawn ink banner-shaped logo frame, ribbon stamp motif, no lettering, wide horizontal ribbon with central blank band about 70% width x 40% height for lettering, transparent background` | 레터링 안1(생성 후 합성)/안2(폰트 얹기, 권장) — §2-j-2 참조 |
| BG_Title_v2 | 배경 | (신규, 미배치 — T5 신규 키 비주얼안) | 타이틀 새 키 비주얼 배경 | 1920x1080 | Assets/SampleImage/Title/ | §2-j-1-v2 참조: `COMFORT HQ exterior office building at dusk, three heroes (불편맨 Hero / Cook / Fashion, Hero_Stand art style) standing together in a confident key-visual pose across the lower-middle area, reserved top-center logo band (approx 5~30% height) left empty, reserved bottom 20% (≥220px) left empty for touch-to-start caption, everyday-nuisance props (tangled earphones, flyers) only at edges` | 기존 BG_Title을 덮어쓰지 않는 신규 행. 기존 BG_Title은 유지, 교체 여부는 사람이 비교 후 결정 |
| Logo_Title_v2 | 로고 | (신규, 미배치 — T5 신규 레터링 로고안) | 타이틀 새 로고(레터링 포함) | 1024x512 | Assets/SampleImage/Title/ | §2-j-2-v2 참조: `hand-drawn ink banner-shaped logo frame with hand-lettered Korean game title "도와줘요 불편맨" centered in the ribbon band, ribbon stamp motif, wide horizontal ribbon, transparent background` | 기존 Logo_Title(글자 없는 틀)과 달리 레터링을 포함해 생성 시도. 기존 Logo_Title은 유지, 실패 시 기존 안2(폰트 얹기)로 폴백 |
| BG_HQ | 배경 | HQ.unity Canvas/Background(IMG, sprite=null) | HQ 화면 배경 | 1920x1080 | Assets/SampleImage/Hub/ | §2-j-3 참조: `Hades-style 2.5D office lobby interior viewed at an angle, three empty standing spots (each with 400x700 click area under a 700x700 agent) with prop hints only on the back wall (tool bench, module shelf, paper archive shelf), plain ceiling/wall top strip reserved ~120px for TopBar, everyday-nuisance props (tangled cables, paper stacks) only at edges, no people` | |
| BG_Battle | 배경 | Battle.unity Canvas/BackGroundImage(IMG, sprite=20230104_135653) | 전투 화면 배경 | 1920x1080 | Assets/SampleImage/Battle/ | §2-j-4 참조: `messy office corridor stage1 battlefield, scattered flying papers, a mosquito silhouette, trash-bag residue trail, left floor line for party, right floor line for monsters, reserved bottom 20% (~220px) flat floor for command bar and time bar, reserved lower-left log zone (~500x240 above (48,240)), reserved plain top strip (~180px) and top-right map panel area (~480x360), unit ground line between 30~45% screen height` | 현재 USBMiniGame 사진(20230104_135653)을 임시로 재사용 중 |
| Maze_Tile | 맵 타일 | Battle.unity Canvas/BattleHud/MapPanel (DirButton_Up/Down/Left/Right, sprite=null) | 미로 이동 UI 타일/방향 버튼 | 128x128 | Assets/SampleImage/Battle/Maze/ | §2-j-5 참조(6종 하위 스프라이트) | 미로 전용 씬 파일 없음. unity-mcp로 Battle.unity 계층 확인 완료: `MapPanel [Image,MapHud]` 아래 `Title`(Text), `MazeGrid`(빈 RectTransform, 런타임 생성 컨테이너), `DirButton_Up/Down/Left/Right`(Image+Button+Label)만 존재하고 방/통로 타일 GameObject는 씬에 미리 배치돼 있지 않음 — `MapHud.cs`가 런타임에 생성한다는 기존 가정과 일치, §2-j-5 6종 하위 스프라이트 설계 유효 |

### 2-j-1. BG_Title 상세 프롬프트

**고정 설명 문구**: `COMFORT HQ office headquarters exterior at dusk, tall office building silhouette, city night skyline behind`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §2-j 스타일 블록+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| BG_Title | 타이틀 배경 | 1920x1080 | Assets/SampleImage/Title/ | `COMFORT HQ office headquarters exterior at dusk, tall office building silhouette, city night skyline behind, small silhouette of the weary office-worker hero (불편맨) standing on lower-left corner (does not overlap button area), empty rectangular banner space at top-center reserved for logo (approx 1024x512 area, from ~5% to ~50% of height), empty clear area at bottom reserved for start button (bottom 20% of frame, ≥220px height), everyday-nuisance doodle props (tangled earphone cable, scattered flyers) kept only near the screen edges to match the sketchbook tone` | 배경 자체에 로고 글자 없음, 로고는 Logo_Title로 별도 합성. UI 빈 영역: 상단 중앙 로고 밴드(약 5~50%), 하단 220px(약 20%) 버튼 자리 | |

### 2-j-2. Logo_Title 상세 프롬프트

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1-1 접두+네거티브 공통 적용 — 투명 배경 유지) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| Logo_Title | 타이틀 로고 틀 | 1024x512 | Assets/SampleImage/Title/ | `hand-drawn ink banner-and-stamp shaped logo frame, ribbon banner with stamp seal accents, no text, no lettering, wide horizontal ribbon with a central blank band about 70% width x 40% height reserved for hand-lettered game title, transparent background, isolated subject` | 게임명 "도와줘요 불편맨" 글자는 이미지 생성기가 한글을 부정확하게 그리므로, 글자 없는 틀만 생성 | 레터링 안1: 이미지 생성기로 글자까지 그린 뒤 사람이 그래픽 툴로 손본다. 안2(권장, 현재 적용): 빈 리본 위에 Title.unity TitleText를 46pt 손그림 계열 한글 폰트로 얹는다(§18.2 L568). |

### 2-j-1-v2. BG_Title_v2 상세 프롬프트 (T5 신규 키 비주얼)

**고정 설명 문구**: `COMFORT HQ office headquarters exterior at dusk, tall office building silhouette, city night skyline behind, three heroes standing together as a key visual`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §2-j 스타일 블록+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| BG_Title_v2 | 타이틀 새 키 비주얼 배경 | 1920x1080 | Assets/SampleImage/Title/ | `COMFORT HQ office headquarters exterior at dusk, tall office building silhouette, city night skyline behind, three heroes standing together in a confident key-visual pose across the lower-middle area of the frame: 불편맨 Hero (center, Hero_Stand art style reference), Cook (short brown hair male, chef-ish office worker look) on one side, Fashion (black bob-cut female, stylish office worker look) on the other side, empty rectangular banner space at top-center reserved for logo (approx 5~30% of height, kept lower than BG_Title's 5~50% to leave more room for the character trio), empty clear area at bottom reserved for touch-to-start caption (bottom 20% of frame, ≥220px height), everyday-nuisance doodle props (tangled earphone cable, scattered flyers) kept only near the screen edges to match the sketchbook tone` | 신규 키 비주얼: 캐릭터 3인(Hero·Cook·Fashion) 포함. 상단 로고 영역과 하단 터치 문구 영역은 비워 둠(레터링·문구는 Logo_Title_v2/UI 텍스트로 별도 합성) | 기존 BG_Title(캐릭터 1인 실루엣, 로고 밴드 5~50%)을 덮어쓰지 않는 별도 신규 행. 파일명 BG_Title_v2로 생성·저장, Title.unity 교체는 사람 검토 후 별도 작업 |

### 2-j-2-v2. Logo_Title_v2 상세 프롬프트 (T5 신규 레터링 로고)

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1-1 접두+네거티브 공통 적용 — 투명 배경 유지) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| Logo_Title_v2 | 타이틀 새 로고(레터링 포함) | 1024x512 | Assets/SampleImage/Title/ | `hand-drawn ink banner-and-stamp shaped logo frame, ribbon banner with stamp seal accents, wide horizontal ribbon, hand-lettered Korean text "도와줘요 불편맨" centered in the ribbon band (bold, slightly wobbly ink brush lettering matching the sketchbook tone), transparent background, isolated subject` | Logo_Title(글자 없는 틀)과 달리 레터링까지 생성 시도하는 신규안. 한글 생성 정확도가 낮을 수 있으므로 결과를 사람이 검수 | 실패(글자 깨짐) 시 기존 Logo_Title + 안2(폰트 얹기, §18.2 L568)로 폴백. 기존 Logo_Title 파일은 변경하지 않음 |

### 2-j-3. BG_HQ 상세 프롬프트

**고정 설명 문구**: `Hades-style 2.5D office lobby hub interior, viewed at a slight angle, warm indoor lighting`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §2-j 스타일 블록+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| BG_HQ | HQ 배경 | 1920x1080 | Assets/SampleImage/Hub/ | `Hades-style 2.5D office lobby hub interior, viewed at a slight angle, warm indoor lighting, three empty standing floor spots left/center/right for agents (each spot leaves a 400x700 clear floor/wall area for a 700x700 full-body agent sprite, with extra floor margin at the very bottom for a nameplate), left spot has a tool workbench prop hint only on the back wall (강화 요원 자리), center spot has a module parts shelf prop hint only on the back wall (모듈 요원 자리), right spot has a paper archive bookshelf prop hint only on the back wall (스토리 요원 자리), plain monotone ceiling/wall strip across the top exactly 120px tall reserved for the TopBar and stage text, everyday-nuisance clutter (tangled cables, stacked paperwork) kept only near the edges, no people, no characters` | AgentButton_Upgrade(강화)/Module(모듈)/Story(스토리) 3개 버튼 위치에 대응하는 빈 자리. 근거 §18.2 L563-564, TopBar 높이는 UI_DESIGN.md §18 L189 실측 정확값(120) 재확인(T6.1) | 실측 완료(T6.1): HQ TopBar sizeDelta.y=120(정확값, 기존 '약120px' 표기를 정확값으로 정정) |

### 2-j-4. BG_Battle 상세 프롬프트

**고정 설명 문구**: `messy stage1 office corridor battlefield, scattered flying papers, faint mosquito and trash-bag traces`

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §2-j 스타일 블록+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| BG_Battle | 전투 배경(스테이지1) | 1920x1080 | Assets/SampleImage/Battle/ | `messy stage1 office corridor battlefield, scattered flying papers in the air, a faint mosquito silhouette buzzing, a torn trash-bag residue trail on the floor, clear left-side floor line for the player party, clear right-side floor line for monsters, unit ground line placed between 30~45% of screen height, plain flat-color reserved strip across the bottom ~20% (~220px) for the command bar and time bar, reserved lower-left area (~500x240, above screen position (48,240)) for the battle log kept clear of detail, plain top strip exactly 140px tall (full width) reserved for the HP HUD bar, plain clear band across the middle of the screen from y=140px to y=940px spanning from the left edge (x=48px, maze grid area up to 1000x800) across to the right edge (x=1344~1856px, direction-button column) — this is a wide horizontal band, NOT a small top-right corner, so keep background detail (props, floor clutter) only within the top 140px and bottom 220px strips and the narrow vertical margins at far left/right edges` | 현재 USBMiniGame 사진(20230104_135653)을 임시 교체하는 용도. 배경은 BackgroundCanvas(sortingOrder -100)에 그려진다. 근거 §18.2 L556-561 + 이번 세션 Unity_RunCommand 실측(아래 비고) | 실측 완료(T6.1): TopBar sizeDelta=(0,140) → HP HUD 상단 140px(정확값, 기존 '약180px' 추정 정정). MapPanel은 top-right 480x360 코너가 아니라 anchorMin/Max(0,0)-(1,1) 풀스트레치 컨테이너였고, 실제 배치는 MazeGrid(anchor left-center, anchoredPos x=48, sizeDelta 1000x800, 5x5 grid 실제 렌더 폭≈556px)가 좌측-중앙, DirButton_Up/Down/Left/Right(anchor right-center, x=-576~-224, y=±176, 160x160)가 우측 끝에 위치. '우상단 코너' 가정은 틀렸으므로 프롬프트를 중앙 가로 밴드 지시로 교체함 |

### T5 작업 기록

- §2-j 표에 BG_Title_v2(신규 키 비주얼, Hero·Cook·Fashion 3인 포함, 상단 로고 영역·하단 터치 문구 영역 비움)와
  Logo_Title_v2(레터링 포함 로고안) 2개 신규 행을 추가함. §2-j-1-v2/§2-j-2-v2 상세 절도 신설.
- 기존 BG_Title/Logo_Title 행과 파일 경로는 전혀 고치지 않음 — 배치 경로는 "(신규, 미배치)"로 표기해
  기존 Title.unity의 Logo/Background 참조(sprite=null)와 구분. 실제 생성·교체는 사람이 비교 후 별도 결정.
- 검증: Grep 'BG_Title_v2|Logo_Title_v2' 6건(표 2행 + 상세 절 제목/본문), 기존 BG_Title/Logo_Title 행·경로 불변.

### T5.1 작업 기록(타이틀 보조 메뉴 아이콘)

- T5가 범위 밖으로 남긴 이슈 해결: §27.3 L662가 "임시 사용 중"이라고 기록한 BottomMenu 3버튼
  (SettingsButton/NoticeButton/CreditsButton, 현재 Icon_Skill_Comic/Icon_Talk_Comic/Icon_Heart_Comic
  임시 재사용)을 위해 §2-o-1(HUD 아이콘) 표에 Icon_Settings(톱니바퀴)/Icon_Notice(스피커)/
  Icon_Credits(책·별) 3개 신규 행을 추가함.
- 각 행의 비고에 교체 방법(해당 버튼 Icon 자식의 Image.sprite만 변경, 코드·버튼 크기 불변)을 명시.
- 실제 이미지 생성(Tripo 등)과 Unity 에디터 Image.sprite 교체는 이번 단계에서 하지 않음(문서 프롬프트만
  작성) — T6 또는 별도 작업에서 진행.
- BG_Title_v2/Logo_Title_v2 실제 이미지 생성·Unity 에셋 임포트·씬 교체 역시 T5에서 이어진 미해결 이슈로,
  이번 단계는 아이콘 프롬프트 공백만 해결하는 것으로 범위를 한정함(decisions_added 참고).
- 검증: Grep 'Icon_Settings|Icon_Notice|Icon_Credits' → §2-o-1 표 3행에서 매치, 기존 Icon_Crystal 등
  행·경로 불변.

### T5.2 작업 기록(타이틀 아트 프롬프트 갱신 마무리)

- T5.1까지 작성된 BG_Title_v2/Logo_Title_v2(§2-j)와 Icon_Settings/Icon_Notice/Icon_Credits(§2-o-1)
  5개 행 전부 프롬프트·배치 경로·비고가 빠짐없이 채워져 있음을 재확인. 문서 공백 없음(완료 기준 충족).
- 실제 이미지 생성(Tripo 등)·Unity 에셋 임포트·Title.unity 스프라이트 교체는 이번에도 진행하지 않음:
  `Unity_AssetGeneration_GenerateAsset`은 대화당 1회 사용자 동의가 선행돼야 호출 가능한 도구라 자동
  진행 단계에서 실행할 수 없음(§2-j-5 MazeIcon_Crystal 때와 같은 사유). T6 또는 별도 작업에서 사람이
  동의 후 생성·교체를 진행해야 한다.
- 코드·씬 변경 없음 → unity-mcp 컴파일 에러 확인 생략(범위 외 판단 유지).
- 검증: Grep 'BG_Title_v2|Logo_Title_v2|Icon_Settings|Icon_Notice|Icon_Credits' → 기존 11건 그대로,
  신규 공백·표 깨짐 없음.

### 2-j-5. Maze_Tile 하위 스프라이트 상세 프롬프트

- 색과 글자(S/전/엘/휴/B, CombatColor 흰/EliteColor 빨강/RestColor 초록/BossColor 보라, FogColor)는 코드
  (`MapHud.cs` 257-293행)가 `Image.color`·`Text`로 입히므로 이미지에 굽지 않는다. 아래 스프라이트는 모두
  무채색(흰색/회색 바탕, 틴트 가능하도록) 또는 색 중립 장식만 넣는다.

| 항목 | 용도 | 권장 해상도 | 배치 경로 | 2D 이미지용 프롬프트(영문, §1-1 접두+네거티브 공통 적용) | 한글 설명 | 비고 |
|---|---|---|---|---|---|---|
| Maze_Room | 미로 방 셀(Combat/Elite/Rest/Boss 공통, Image.color로 틴트) | 128x128 | Assets/SampleImage/Battle/Maze/ | `simple rounded-square paper card icon, plain white blank surface, hand-drawn ink outline, no text, no symbols, front view` | 흰 바탕 카드, 코드가 색을 입힘, 글자 없음 | |
| Maze_PassH | 가로 통로 바(passH) | 128x32 | Assets/SampleImage/Battle/Maze/ | `simple horizontal dashed tape strip connector, hand-drawn ink line, plain light gray, no text, front view` | 방과 방 사이 가로 통로 표시 | |
| Maze_PassV | 세로 통로 바(passV) | 32x128 | Assets/SampleImage/Battle/Maze/ | `simple vertical dashed tape strip connector, hand-drawn ink line, plain light gray, no text, front view` | 방과 방 사이 세로 통로 표시 | |
| Maze_Current | 현재 위치 강조 테두리(Yellow outline) | 128x128 | Assets/SampleImage/Battle/Maze/ | `simple yellow highlighter-marker ring outline, hand-drawn ink style, transparent hollow center, no fill, no text, front view` | 코드가 노란 테두리+1.1x 스케일로 표시, 중앙은 투명하게 비워 방 셀 위에 겹쳐 쓸 수 있게 함 | |
| Maze_Fog | 미확인(안개) 셀 | 128x128 | Assets/SampleImage/Battle/Maze/ | `simple crumpled gray paper texture card icon, hand-drawn ink outline, plain muted gray, no text, no question mark, front view` | 물음표 등 글자는 코드 Text이므로 그리지 않음 | |
| Maze_DirButton | 방향 이동 버튼 배경(코드가 색만 틴트) | 128x128 | Assets/SampleImage/Battle/Maze/ | `simple hand-drawn ink up-arrow sticker icon, flat single-tone color fill, transparent background, isolated subject, front view` | DirButton_Up/Down/Left/Right 공용. 코드 회전 없음(`MapHud.cs` 77-101행은 `targetGraphic.color`만 바꾼다), 배경은 §18.2에서 UI_SquadButton으로 이미 대체됨. 화살표 그림은 §2-o-1 Maze_DirArrow(신규 자식) 참조 | |

### 2-k. 씬 UI 자리 스프라이트(placeholder, 실사용 여부는 T6에서 확정)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| UI_StartButton | UI 버튼 | Title.unity Canvas/SafeArea/StartButton(IMG, sprite=null) | 시작 버튼 | 512x256 | Assets/SampleImage/UI/ | `flat UI button frame, wide rounded rectangle border, cream paper texture fill matching DialogueFrame_Panel style, front view, orthographic UI panel, empty interior for start label text` | |
| UI_AgentButton | UI 버튼 | HQ.unity AgentButtons/AgentButton_Upgrade·Module·Story(IMG, sprite=null) | 요원 선택 버튼(전신 아트로 대체 가능) | 512x512 | Assets/SampleImage/UI/ | `flat UI button frame, square rounded border, cream paper texture fill matching DialogueFrame_Panel style, front view, orthographic UI panel, empty interior for agent portrait or label` | §2-c Agent_*_Full(전신 아트)과 역할이 다름 — 이 프롬프트는 버튼 테두리·배경 프레임만, 안쪽 내용은 Agent_*_Full을 별도로 얹거나 텍스트로 채움 |
| UI_SortieButton | UI 버튼 | HQ.unity Canvas/SortieButton(IMG, sprite=null) | 출격 버튼 | 512x256 | Assets/SampleImage/UI/ | `flat UI button frame, wide rounded rectangle border, cream paper texture fill matching DialogueFrame_Panel style, front view, orthographic UI panel, empty interior for sortie label text` | UI_StartButton과 같은 프레임 모티프, 라벨 텍스트로 구분 |
| UI_SquadButton | UI 버튼 | HQ.unity Canvas/SquadButton, SquadRow_0~2(IMG, sprite=null) | 편성 버튼·행 선택 표시 | 256x256 | Assets/SampleImage/UI/ | `flat UI button frame, square rounded border, cream paper texture fill matching DialogueFrame_Panel style, front view, orthographic UI panel, empty interior for squad slot label, thin selection highlight rim` | 선택 상태는 코드가 rim 강조를 틴트/토글하는 것을 전제로 테두리를 얇게 표현 |
| UI_HpBar | UI 게이지 | Battle.unity TopBar/HpBarBg·HpBar, BossHpRoot/BossHpBar(IMG, sprite=UISprite) | 체력 바 | 512x64 | Assets/SampleImage/UI/ | `flat UI gauge bar frame, thin horizontal rounded rectangle outline, empty fill area, front view, orthographic UI element` | EnergyGauge_Bar와 같은 프레임 모티프, 색은 Image.color로 입혀 무채색 배경으로 생성 |
| Command_Attack | UI 버튼(커맨드) | Battle.unity CommandPanel/AttackButton(IMG, sprite=UISprite) | 공격 커맨드 버튼 | 256x256 | Assets/SampleImage/UI/ | `flat icon, plain fist striking forward on a square rounded button background, front view` | AttackSkill 아이콘과 같은 모티프, 버튼용으로 배경 사각 |
| Command_Skill | UI 버튼(커맨드) | Battle.unity CommandPanel/SkillButton(IMG, sprite=UISprite) | 스킬 커맨드 버튼 | 256x256 | Assets/SampleImage/UI/ | `flat icon, small sparkling crystal shard on a square rounded button background, front view` | 정화 결정(에너지) 모티프로 스킬 메뉴 진입 표시 |
| Command_Guard | UI 버튼(커맨드) | Battle.unity CommandPanel/GuardButton(IMG, sprite=UISprite) | 방어 커맨드 버튼 | 256x256 | Assets/SampleImage/UI/ | `flat icon, simple round shield on a square rounded button background, front view` | GuardSkill 아이콘과 같은 모티프 |
| UI_ResultPanel | UI 패널 | Battle.unity BattleHud/ResultPanel, PhaseHudRoot/ResultPanel(IMG, sprite=null/UISprite) | 승리·패배 결과 패널 배경 | 1024x1024 | Assets/SampleImage/UI/ | `flat UI panel frame, thick rounded rectangle border, cream paper texture fill matching DialogueFrame_Panel style, front view, orthographic UI panel, empty interior for result text` | |
| EnergyGauge_Bar | UI 게이지 | Battle.unity PhaseHudRoot/EnergyGauge(IMG, sprite=null) | 에너지 게이지 바 배경 | 512x64 | Assets/SampleImage/UI/ | `flat UI gauge bar frame, thin horizontal rounded rectangle outline with segmented tick marks inside, empty fill area, front view, orthographic UI element` | 정화 결정 에너지 총량 표시, EnergyGauge_Cell이 안에 채워짐 |
| EnergyGauge_Cell | UI 게이지 칸 | Battle.unity PhaseHudRoot/EnergyGauge 내부 채움 단위 | 에너지 게이지 1칸(정화 결정) | 64x64 | Assets/SampleImage/UI/ | `flat icon, single small glowing crystal shard, front view, orthographic UI element` | 게이지 바 안에 반복 배치되는 낱개 결정 |

### 2-n. 미니게임 용도·불편 유형 배지 (Assets/Scripts/MiniGames/Definitions/MiniGameDefinition.cs 열거형)

MiniGameDefinition.usage(MiniGameUsage, [Flags] 3값)와 category(MiniGameCategory, 6값)를 아이콘 배지로 만든다.
데이터 에셋 자체(§2-l Assets/Data/MiniGames/*.asset)에는 이 값들이 필드로 들어 있어 별도 데이터 에셋 수는
없다 — 열거형 값 수(3, 6)가 완료 기준이다.

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| Usage_Energy | 미니게임 용도 배지 | MiniGameDefinition.usage = MiniGameUsage.Energy | 미니게임 선택 UI, 에너지 획득용 표시 | 128x128 | Assets/SampleImage/UI/Icons/ | `flat icon, small glowing crystal shard inside a small round tag badge, front view` | EnergyGauge_Cell과 같은 모티프 |
| Usage_Evasion | 미니게임 용도 배지 | MiniGameDefinition.usage = MiniGameUsage.Evasion | 위와 동일, 회피용 표시 | 128x128 | Assets/SampleImage/UI/Icons/ | `flat icon, small running footprint with a motion swoosh inside a small round tag badge, front view` | |
| Usage_Special | 미니게임 용도 배지 | MiniGameDefinition.usage = MiniGameUsage.Special | 위와 동일, 보스·특수 유발용 표시 | 128x128 | Assets/SampleImage/UI/Icons/ | `flat icon, small purple swirling burst with a tiny star inside a small round tag badge, front view` | Intent_Special과 같은 모티프 |
| Category_Food | 불편 유형 배지 | MiniGameCategory.Food | 미니게임 선택 UI, 불편 유형(음식) 표시 | 128x128 | Assets/SampleImage/UI/Icons/ | `flat icon, small fried egg on a spatula inside a small round tag badge, front view` | Egg/Pizza/SamgakKimbap 계열 |
| Category_Wear | 불편 유형 배지 | MiniGameCategory.Wear | 위와 동일(의류) | 128x128 | Assets/SampleImage/UI/Icons/ | `flat icon, small shirt button and buttonhole inside a small round tag badge, front view` | ButtonShirt 계열 |
| Category_Device | 불편 유형 배지 | MiniGameCategory.Device | 위와 동일(전자기기) | 128x128 | Assets/SampleImage/UI/Icons/ | `flat icon, small smartphone outline with a crack line inside a small round tag badge, front view` | USB/ScreenBubble 계열 |
| Category_Home | 불편 유형 배지 | MiniGameCategory.Home | 위와 동일(집안일) | 128x128 | Assets/SampleImage/UI/Icons/ | `flat icon, small broom leaning with a dust speck inside a small round tag badge, front view` | Vacuum/FlySwat 계열 |
| Category_Outside | 불편 유형 배지 | MiniGameCategory.Outside | 위와 동일(외부·야외) | 128x128 | Assets/SampleImage/UI/Icons/ | `flat icon, small footpath with a small cloud overhead inside a small round tag badge, front view` | 구현된 미니게임 없음(예정), MINIGAME_IDEAS 기준 |
| Category_Peel | 불편 유형 배지 | MiniGameCategory.Peel | 위와 동일(포장·스티커 떼기) | 128x128 | Assets/SampleImage/UI/Icons/ | `flat icon, small peeling sticker corner curling up inside a small round tag badge, front view` | PeelSticker 계열 |

### 2-l. 구현된 미니게임 (Assets/Scenes/MiniGames/*.unity, Assets/Data/MiniGames/*.asset)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| Egg | 미니게임 정의 | Assets/Data/MiniGames/Egg.asset | MiniGameDefinition(category=Food) | — | — | — | 씬 EggMiniGame.unity, 오브젝트는 아래 행. 정의 아이콘은 §2-n Category_Food 배지로 대체, 별도 이미지 불필요 |
| Egg_Fan | 미니게임 오브젝트 | EggMiniGame.unity EGGFrying/Fan(sprite=후라이팬_0) | 후라이팬 | 512x512 | Assets/SampleImage/EGGMiniGames/ | `hand-drawn ink line art frying pan, top-down flat view, round pan body with a handle extending to the right, plain empty cooking surface, no food, isolated subject`(§1-1 접두+네거티브 공통 적용) | 기존 커스텀 이미지(후라이팬.png)를 손그림 스타일로 교체 권장, Docs/sketches/01_egg_shell.png 톤 참고 |
| Egg_Egg | 미니게임 오브젝트 | EggMiniGame.unity EGGFrying/Egg(sprite=egg1_0) | 계란 | 256x256 | Assets/SampleImage/EGGMiniGames/ | `hand-drawn ink line art single fried egg, top-down view, round egg white with a raised yolk center, clean first-crack stage, isolated subject`(§1-1 접두+네거티브 공통 적용) | 기존 egg1~4·계란껍질1~3이 이미 깨짐 단계 진행을 표현하므로 유지, 위 프롬프트는 1단계 재생성/보강용 |
| BG_Egg | 미니게임 배경 | 씬에 없음(신규) | 주방 조리대 배경 | 1920x1080 | Assets/SampleImage/EGGMiniGames/ | `hand-drawn ink line art simple kitchen counter background, front view, plain countertop surface and faint wall line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | §4-4-1: EggMiniGame은 Object 레이어라 배경 배치 안전. Object 레이어 order -1 권장, 씬 미수정 |
| FX_Egg_Success | 미니게임 결과 연출 | EggMiniGame.unity EGGFrying/Egg(Animator, isSuccess) | 성공 연출(껍질 모두 벗겨짐) | 256x256 | Assets/SampleImage/EGGMiniGames/ | `hand-drawn ink line art fully peeled smooth boiled egg, top-down view, clean unbroken oval surface, no shell fragments, isolated subject`(§1-1 접두+네거티브 공통 적용) | EggGameManager.ResultAnimator=originalEgg의 Animator, isSuccess 상태에서 스프라이트 교체로 표현, 0.6초 안에 읽혀야 함 |
| FX_Egg_Fail | 미니게임 결과 연출 | EggMiniGame.unity EGGFrying/Egg(Animator, isFailed) | 실패 연출(노른자 터짐) | 256x256 | Assets/SampleImage/EGGMiniGames/ | `hand-drawn ink line art broken egg with yolk burst and splattering outward, top-down view, exaggerated splash lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | §4-2 기준 과장 표현, 0.6초 안, 안전영역(17.78×10) 내 |
| Obj_Egg_Chopstick | 미니게임 오브젝트 | T7.2: EggMiniGame.unity에 런타임 생성 placeholder(단색 베이지 8x64 사각 스프라이트, Object 레이어 order 5)로 배치 완료, egg 앵커 오브젝트 기준 (+1.2,-0.3) | 젓가락(껍질 집기 연출) | 512x512 | Assets/SampleImage/EGGMiniGames/ | `hand-drawn ink line art single pair of wooden chopsticks, angled view, simple tapered sticks crossing near the tip, isolated subject`(§1-1 접두+네거티브 공통 적용) | T7.1/T7.2: Unity_AssetGeneration_GenerateAsset 는 '생성 전 1회 사용자 확인' 이 도구 자체에서 대화턴 응답을 요구해 단일 응답형 자동 릴레이 구조상 호출 불가(consent 대기 상태 지속). CLAUDE.md 폴백 규칙(기본 도형 스프라이트로 자리 잡기)에 따라 placeholder 배치, 실제 이미지 생성 후 교체 필요 |
| Pizza | 미니게임 정의 | Assets/Data/MiniGames/Pizza.asset | MiniGameDefinition(category=Food) | — | — | — | 씬 PizzaMiniGame.unity. 정의 아이콘은 §2-n Category_Food 배지로 대체 |
| Pizza_TotalPizza | 미니게임 오브젝트 | PizzaMiniGame.unity PIzzaGame/TotalPizza(sprite=pizzaAssert_2) | 전체 피자 | 1024x1024 | Assets/SampleImage/PizzaMiniGame/ | `hand-drawn ink line art whole round pizza, top-down view, cheese and topping dots evenly scattered, clean circular edge, isolated subject`(§1-1 접두+네거티브 공통 적용) | 기존 pizzaAssert.png 있음, art_pizza.png가 이 톤의 완성 기준 샘플이므로 참고·재사용 |
| Pizza_Cutter | 미니게임 오브젝트 | PizzaMiniGame.unity PIzzaGame/Cutter(sprite=pizzaDga_0) | 피자 커터 | 512x512 | Assets/SampleImage/PizzaMiniGame/ | `hand-drawn ink line art pizza cutter tool, top-down view, round rolling blade attached to a simple handle, isolated subject`(§1-1 접두+네거티브 공통 적용) | 기존 pizzaDga.png 있음, 교체 시 art_pizza.png 톤 맞춤 |
| Pizza_Piece | 미니게임 오브젝트 | PizzaMiniGame.unity Cutter/PizzaPiece(sprite=piz1_0) | 조각난 피자 | 256x256 | Assets/SampleImage/PizzaMiniGame/PIZZApiece/ | `hand-drawn ink line art single triangular pizza slice, top-down view, cheese and topping visible on the cut edge, isolated subject`(§1-1 접두+네거티브 공통 적용) | 기존 piz1~9.png가 조각 진행을 표현하므로 유지, Docs/sketches/08_pizza_lift.png 참고 |
| Pizza_Plate | 미니게임 오브젝트 | PizzaMiniGame.unity PizzaPlate(sprite=plate_0), Triangle/Plate(sprite=foodplate_0) | 접시 | 512x512 | Assets/SampleImage/PizzaMiniGame/ | `hand-drawn ink line art empty round plate, top-down view, simple rim outline, no food, isolated subject`(§1-1 접두+네거티브 공통 적용) | 기존 plate.png·foodplate.png 있음, 교체는 선택 |
| BG_Pizza | 미니게임 배경 | 씬에 없음(신규) | 식탁 배경 | 1920x1080 | Assets/SampleImage/PizzaMiniGame/ | `hand-drawn ink line art simple dining table background, front view, plain tabletop surface and faint wall line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | §4-4-1: T7.1에서 PizzaMiniGame의 Default 레이어 SpriteRenderer/SortingGroup 1개를 Object로 이전 완료(sortingOrder 유지, 씬 저장). BG 이미지 배치 시 order -1 권장 |
| FX_Pizza_Success | 미니게임 결과 연출 | PizzaMiniGame.unity Cutter/PizzaPiece(Animator, isSuccess) | 성공 연출(접시에 온전히 안착) | 256x256 | Assets/SampleImage/PizzaMiniGame/ | `hand-drawn ink line art whole triangular pizza slice neatly placed on a plate, top-down view, clean edges, isolated subject`(§1-1 접두+네거티브 공통 적용) | PizzaServeManager.ResultAnimator=Cutter 아래 PizzaPiece의 Animator, 0.6초 안에 읽혀야 함 |
| FX_Pizza_Fail | 미니게임 결과 연출 | PizzaMiniGame.unity Cutter/PizzaPiece(Animator, isFailed) | 실패 연출(토핑 떨어짐) | 256x256 | Assets/SampleImage/PizzaMiniGame/ | `hand-drawn ink line art pizza slice with toppings sliding off and cheese stretching down, top-down view, exaggerated drip and drop lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | §4-2 기준 과장 표현(토핑 흘러내림·치즈 늘어짐), 0.6초 안, 안전영역 내, Docs/sketches/08_pizza_lift.png 참고 |
| USB | 미니게임 정의 | 씬 USBMiniGame.unity (정의 에셋 없음) | 미니게임(category=Device 추정) | — | — | — | Assets/Data/MiniGames에 USB.asset 없음(범위 밖, 데이터 작업 단계 대상). 정의 아이콘은 §2-n Category_Device 배지로 대체 |
| USB_NoteBook | 미니게임 오브젝트 | USBMiniGame.unity NoteBook(sprite=20230104_133908), UsbPort(sprite=usb) | 노트북·USB 포트 | 1024x1024 | Assets/SampleImage/USBMiniGame/ | `hand-drawn ink line art open laptop with a visible usb port slot on the side, front-top angled view, simple keyboard hint, isolated subject`(§1-1 접두+네거티브 공통 적용) | 현재 사진 이미지 사용 중, 손그림 스타일 교체 권장. Docs/sketches/05_usb.png 참고 |
| USB_Hand | 미니게임 오브젝트 | USBMiniGame.unity HandObject/usbHand(sprite=20230104_133401), FailedUSB(sprite=20230104_165756) | 손·USB 케이블 | 512x512 | Assets/SampleImage/USBMiniGame/ | 정상: `hand-drawn ink line art hand holding a usb cable plug, front view, simple cable line, isolated subject` / 실패(FailedUSB): `hand-drawn ink line art bent and kinked usb cable with a slightly damaged plug tip, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | FailedUSB가 이미 실패 전용 이미지 슬롯, 두 번째 프롬프트로 채움. Fail 슬롯은 FX_USB_Fail 행 참조 |
| BG_USB | 미니게임 배경 | 씬에 없음(신규) | 책상 배경 | 1920x1080 | Assets/SampleImage/USBMiniGame/ | `hand-drawn ink line art simple desk background, front view, plain desk surface and faint wall line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | §4-4-1: USBMiniGame은 부분적으로 Object 레이어라 비교적 안전, Object 레이어 order -1 권장, 씬 미수정. T7.1: USB 루트 Z=274.13→0 수정 완료(x,y 유지, USBMiniGame.unity 저장). UsbMover.cs L42·UsbSetter.cs L15가 위치에 Vector2를 대입해 Z에 의존하지 않음을 확인, Main Camera는 orthographic(pos Z=-10)이라 이동 영향 없음 |
| FX_USB_Success | 미니게임 결과 연출 | USBMiniGame.unity UsbMover(Animator, self, isSuccess) | 성공 연출(포트에 딱 꽂힘) | 256x256 | Assets/SampleImage/USBMiniGame/ | `hand-drawn ink line art usb plug fully inserted into a port with a small sparkle highlight, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | UsbMover.ResultAnimator=GetComponent<Animator>()(self), noteBook.isCollide+setter.isClear 조합 조건, 0.6초 안 |
| FX_USB_Fail | 미니게임 결과 연출 | USBMiniGame.unity FailedUSB(sprite=20230104_165756) 재사용 | 실패 연출(어긋나 꽂혀 휨) | 512x512 | Assets/SampleImage/USBMiniGame/ | `hand-drawn ink line art bent and kinked usb cable with a slightly damaged plug tip, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | USB_Hand 행의 기존 실패 프롬프트를 이 행으로 참조 이관, 오브젝트 이름(FailedUSB) 변경 없음, §4-2 기준 0.6초·안전영역 내 |
| SamgakKimbap | 미니게임 정의 | Assets/Data/MiniGames/SamgakKimbap.asset | MiniGameDefinition(category=Food) | — | — | — | 씬 SamgakKimbapMiniGame.unity. 정의 아이콘은 §2-n Category_Food 배지로 대체 |
| Kimbap_Wrapper | 미니게임 오브젝트 | SamgakKimbapMiniGame.unity Manager/Wrapper(sprite=Kimbap_Wrapper) | 삼각김밥 포장 | 512x512 | Assets/SampleImage/SamgakKimbapMiniGame/ | `hand-drawn ink line art triangular kimbap plastic wrapper, front view, simple clear film outline over a triangle shape, isolated subject`(§1-1 접두+네거티브 공통 적용) | 기존 커스텀 이미지 있음, Docs/sketches/02_samgak_kimbap.png 참고, 보강 목적 |
| Kimbap_Laver | 미니게임 오브젝트 | SamgakKimbapMiniGame.unity Manager/Laver(sprite=Kimbap_Laver) | 김 | 512x512 | Assets/SampleImage/SamgakKimbapMiniGame/ | `hand-drawn ink line art triangular seaweed laver sheet, front view, dark flat fill with faint fold crease lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | Docs/sketches/02_samgak_kimbap.png 참고, 보강 |
| Kimbap_Tab | 미니게임 오브젝트 | SamgakKimbapMiniGame.unity Manager/Tab_Top·Left·Right(sprite=Kimbap_Tab) | 포장 뜯는 탭 | 128x128 | Assets/SampleImage/SamgakKimbapMiniGame/ | `hand-drawn ink line art small wrapper pull-tab, front view, thin rectangular tab with a fold crease at the base, isolated subject`(§1-1 접두+네거티브 공통 적용) | 보강 |
| Kimbap_Arrow | 미니게임 오브젝트 | SamgakKimbapMiniGame.unity Manager/Arrow_1~3(sprite=Kimbap_Arrow), PeelStickerMiniGame Book/Sticker/Arrow | 조작 안내 화살표(공유) | 128x128 | Assets/SampleImage/SamgakKimbapMiniGame/ | `hand-drawn ink line art curved single-stroke guide arrow, front view, indicating a pull/peel direction, isolated subject`(§1-1 접두+네거티브 공통 적용) | PeelSticker·ScreenBubble·Vacuum 안내 화살표와 같은 모티프로 공유 재사용 가능 |
| BG_SamgakKimbap | 미니게임 배경 | 씬에 없음(신규) | 편의점 테이블 배경 | 1920x1080 | Assets/SampleImage/SamgakKimbapMiniGame/ | `hand-drawn ink line art simple convenience store table background, front view, plain tabletop surface and faint wall line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | §4-4-1: T7.1에서 SamgakKimbapMiniGame의 Default 레이어 SpriteRenderer/SortingGroup 9개를 Object로 이전 완료(sortingOrder 유지, 씬 저장). BG 이미지 배치 시 order -1 권장 |
| FX_Kimbap_Success | 미니게임 결과 연출 | SamgakKimbapMiniGame.unity Manager(ResultAnimator 필드, isSuccess) | 성공 연출(김이 깔끔히 감싸짐) | 256x256 | Assets/SampleImage/SamgakKimbapMiniGame/ | `hand-drawn ink line art triangular kimbap fully wrapped in clean unbroken seaweed, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | SamgakKimbapMiniGame.cs L41의 ResultAnimator 필드 참조, 0.6초 안 |
| FX_Kimbap_Fail | 미니게임 결과 연출 | SamgakKimbapMiniGame.unity Manager/Laver(SpriteRenderer, tornColor) | 실패 연출(김 찢어짐) | 256x256 | Assets/SampleImage/SamgakKimbapMiniGame/ | `hand-drawn ink line art torn seaweed sheet with a jagged rip and scattered small fragments, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | 코드의 laver SpriteRenderer·tornColor와 연결, §4-2 기준 과장(찢어짐) 0.6초 안, Docs/sketches/02_samgak_kimbap.png 참고 |
| PeelSticker | 미니게임 정의 | Assets/Data/MiniGames/PeelSticker.asset | MiniGameDefinition(category=Peel) | — | — | — | 씬 PeelStickerMiniGame.unity. 정의 아이콘은 §2-n Category_Peel 배지로 대체 |
| PeelSticker_Book | 미니게임 오브젝트 | PeelStickerMiniGame.unity Book/Page_BG·Spine·Line_0~4(sprite=UISprite) | 책·줄 | 512x512 | Assets/SampleImage/PeelStickerMiniGame/ | `hand-drawn ink line art open book page background, front view, faint ruled notebook lines, simple book spine along the left edge, isolated subject`(§1-1 접두+네거티브 공통 적용) | 전부 UISprite placeholder, 신규 제작 필요. Docs/sketches/03_peel_sticker.png 참고 |
| PeelSticker_Sticker | 미니게임 오브젝트 | PeelStickerMiniGame.unity Book/Sticker/StickerBody·PeeledFlap·TornHalf(sprite=UISprite), CornerTab(sprite=Knob) | 스티커·뜯긴 조각 | 256x256 | Assets/SampleImage/PeelStickerMiniGame/ | 온전한 스티커: `hand-drawn ink line art round sticker, front view, flat simple icon design, isolated subject` / 뜯긴 상태(TornHalf): `hand-drawn ink line art round sticker with one half peeled up in a curling flap and a small torn corner tab, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | 신규 제작 필요, Docs/sketches/03_peel_sticker.png 참고. Fail 슬롯은 FX_PeelSticker_Fail 행 참조 |
| BG_PeelSticker | 미니게임 배경 | 씬에 없음(신규) | 책상 배경 | 1920x1080 | Assets/SampleImage/PeelStickerMiniGame/ | `hand-drawn ink line art simple desk background, front view, plain desk surface and faint wall line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | §4-4-1: T7.1에서 PeelStickerMiniGame의 Default 레이어 SpriteRenderer/SortingGroup 12개를 Object로 이전 완료(sortingOrder 유지, 씬 저장). BG 이미지 배치 시 order -1 권장 |
| FX_PeelSticker_Success | 미니게임 결과 연출 | PeelStickerMiniGame.unity Manager(ResultAnimator 필드, isSuccess) | 성공 연출(통째로 깨끗이 떨어짐) | 256x256 | Assets/SampleImage/PeelStickerMiniGame/ | `hand-drawn ink line art round sticker fully peeled off in one clean piece, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | PeelStickerMiniGame.cs L41의 ResultAnimator 필드 참조, 0.6초 안 |
| FX_PeelSticker_Fail | 미니게임 결과 연출 | PeelStickerMiniGame.unity Book/Sticker/TornHalf(GameObject, tornColor) 재사용 | 실패 연출(반쪽만 찢어짐) | 256x256 | Assets/SampleImage/PeelStickerMiniGame/ | `hand-drawn ink line art round sticker with one half peeled up in a curling flap and a small torn corner tab, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | PeelSticker_Sticker 행의 기존 TornHalf 프롬프트를 이 행으로 참조 이관, 오브젝트 이름(TornHalf) 변경 없음, §4-2 기준 0.6초·안전영역 내 |
| ScreenBubble | 미니게임 정의 | Assets/Data/MiniGames/ScreenBubble.asset | MiniGameDefinition(category=Device) | — | — | — | 씬 ScreenBubbleMiniGame.unity. 정의 아이콘은 §2-n Category_Device 배지로 대체 |
| ScreenBubble_Phone | 미니게임 오브젝트 | ScreenBubbleMiniGame.unity PhoneFrame/Frame·Screen(sprite=UISprite) | 휴대폰 프레임·화면 | 1024x1024 | Assets/SampleImage/ScreenBubbleMiniGame/ | `hand-drawn ink line art smartphone outline with a screen protector film layer, front view, simple rounded rectangle body and flat screen area, isolated subject`(§1-1 접두+네거티브 공통 적용) | 전부 UISprite placeholder, 신규 제작 필요. Docs/sketches/06_screen_bubble.png 참고 |
| ScreenBubble_Bubble | 미니게임 오브젝트 | ScreenBubbleMiniGame.unity Bubbles/Bubble0~6(sprite=Knob) | 보호필름 기포 | 128x128 | Assets/SampleImage/ScreenBubbleMiniGame/ | `hand-drawn ink line art small round trapped air bubble under screen film, front view, soft raised bump outline with a highlight, isolated subject`(§1-1 접두+네거티브 공통 적용) | Knob placeholder, 신규 제작 필요 |
| ScreenBubble_Card | 미니게임 오브젝트 | ScreenBubbleMiniGame.unity Card, Arrow(sprite=UISprite) | 안내 카드·화살표 | 256x256 | Assets/SampleImage/ScreenBubbleMiniGame/ | `hand-drawn ink line art small instruction card with a guide arrow beside it, front view, simple rectangle card and thin single-stroke arrow, isolated subject`(§1-1 접두+네거티브 공통 적용) | 신규 제작 필요, 권장 표시 크기 약 90px(≈0.8u) |
| BG_ScreenBubble | 미니게임 배경 | 씬에 없음(신규) | 책상 위 배경 | 1920x1080 | Assets/SampleImage/ScreenBubbleMiniGame/ | `hand-drawn ink line art simple desk-top background seen from above, front view, plain desk surface and faint wall line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | §4-4-1: Object 이전 완료(T8.1), BG order -1 권장. 권장 표시 크기: Phone 높이 약 900px(≈8.3u) |
| FX_ScreenBubble_Success | 미니게임 결과 연출 | ScreenBubbleMiniGame.unity Screen(ResultAnimator, isSuccess), failedColor 없음(성공은 매끈한 스프라이트 교체) | 성공 연출(기포 없이 매끈) | 1024x1024 | Assets/SampleImage/ScreenBubbleMiniGame/ | `hand-drawn ink line art smartphone screen protector film perfectly smooth with no bubbles, front view, subtle sparkle highlight, isolated subject`(§1-1 접두+네거티브 공통 적용) | ScreenBubbleMiniGame.cs(L35 근처) ResultAnimator·isSuccess 참조, 0.6초 안 |
| FX_ScreenBubble_Fail | 미니게임 결과 연출 | ScreenBubbleMiniGame.unity Bubble(SpriteRenderer, failedColor) | 실패 연출(기포 커짐+필름 들뜸) | 128x128 | Assets/SampleImage/ScreenBubbleMiniGame/ | `hand-drawn ink line art large trapped air bubble under screen film with the film corner peeling up, front view, exaggerated bulge outline, isolated subject`(§1-1 접두+네거티브 공통 적용) | ScreenBubble_Bubble의 failedColor 색 변경과 연결, §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| ButtonShirt | 미니게임 정의 | Assets/Data/MiniGames/ButtonShirt.asset | MiniGameDefinition(category=Wear) | — | — | — | 씬 ButtonShirtMiniGame.unity. 정의 아이콘은 §2-n Category_Wear 배지로 대체 |
| ButtonShirt_Shirt | 미니게임 오브젝트 | ButtonShirtMiniGame.unity Shirt/Body·PlacketLeft·PlacketRight(sprite=UISprite) | 셔츠 몸체·플래킷 | 1024x1024 | Assets/SampleImage/ButtonShirtMiniGame/ | `hand-drawn ink line art shirt front torso with a button placket, front view, two vertical fabric strips with a center seam, isolated subject`(§1-1 접두+네거티브 공통 적용) | 전부 UISprite placeholder, 신규 제작 필요 |
| ButtonShirt_Hole | 미니게임 오브젝트 | ButtonShirtMiniGame.unity Shirt/Holes/Hole0~4(sprite=UISprite) | 단춧구멍 | 64x64 | Assets/SampleImage/ButtonShirtMiniGame/ | `hand-drawn ink line art small buttonhole slit, front view, thin oval stitched outline, isolated subject`(§1-1 접두+네거티브 공통 적용) | 신규 제작 필요 |
| ButtonShirt_Button | 미니게임 오브젝트 | ButtonShirtMiniGame.unity Shirt/Buttons/Button0~4(sprite=Knob), HintArrow(sprite=UISprite) | 단추·안내 화살표 | 64x64 | Assets/SampleImage/ButtonShirtMiniGame/ | `hand-drawn ink line art round shirt button with two holes, front view, isolated subject`, 안내 화살표는 Kimbap_Arrow 모티프 재사용(§1-1 접두+네거티브 공통 적용) | 신규 제작 필요, 권장 표시 크기 약 90px(≈0.8u) |
| BG_ButtonShirt | 미니게임 배경 | 씬에 없음(신규) | 옷장 거울 앞 배경 | 1920x1080 | Assets/SampleImage/ButtonShirtMiniGame/ | `hand-drawn ink line art simple wardrobe mirror background, front view, plain mirror frame outline and faint closet wall, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | §4-4-1: Object 이전 완료(T8.1), BG order -1 권장. 권장 표시 크기: Shirt 약 800x950px |
| FX_ButtonShirt_Success | 미니게임 결과 연출 | ButtonShirtMiniGame.unity Shirt(ResultAnimator, isSuccess) | 성공 연출(단추 줄 맞춰 채워짐) | 1024x1024 | Assets/SampleImage/ButtonShirtMiniGame/ | `hand-drawn ink line art shirt front torso with all buttons neatly fastened in a straight aligned row, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | ButtonShirtMiniGame.cs(L27 근처) ResultAnimator·isSuccess 참조, 0.6초 안 |
| FX_ButtonShirt_Fail | 미니게임 결과 연출 | ButtonShirtMiniGame.unity Button(SpriteRenderer, failedColor) | 실패 연출(단추 어긋나 밑단 삐뚤어짐) | 1024x1024 | Assets/SampleImage/ButtonShirtMiniGame/ | `hand-drawn ink line art shirt front torso with buttons fastened one hole off, hem crooked and uneven at the bottom, front view, exaggerated tilt lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | ButtonShirt_Button의 failedColor 색 변경과 연결, §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| Vacuum | 미니게임 정의 | Assets/Data/MiniGames/Vacuum.asset | MiniGameDefinition(category=Home) | — | — | — | 씬 VacuumMiniGame.unity. 정의 아이콘은 §2-n Category_Home 배지로 대체 |
| Vacuum_Floor | 미니게임 오브젝트 | VacuumMiniGame.unity Floor, Sofa/Body(sprite=UISprite) | 바닥·소파 | 1024x1024 | Assets/SampleImage/VacuumMiniGame/ | `hand-drawn ink line art living room floor with a sofa, front view, simple flat floor plane and sofa silhouette, isolated subject`(§1-1 접두+네거티브 공통 적용) | 전부 UISprite placeholder, 신규 제작 필요 |
| Vacuum_Head | 미니게임 오브젝트 | VacuumMiniGame.unity Vacuum/Head·Stick(sprite=UISprite), Handle(sprite=Knob) | 청소기 헤드·손잡이 | 512x512 | Assets/SampleImage/VacuumMiniGame/ | `hand-drawn ink line art vacuum cleaner head attached to a long stick with a handle, side view, simple nozzle shape, isolated subject`(§1-1 접두+네거티브 공통 적용) | 신규 제작 필요 |
| Vacuum_Dust | 미니게임 오브젝트 | VacuumMiniGame.unity Dust/Speck0~2(sprite=Knob), Arrow(sprite=UISprite) | 먼지·안내 화살표 | 64x64 | Assets/SampleImage/VacuumMiniGame/ | `hand-drawn ink line art small scattered dust clump specks, front view, simple fuzzy dot cluster shape, isolated subject`, 안내 화살표는 Kimbap_Arrow 모티프 재사용(§1-1 접두+네거티브 공통 적용) | 신규 제작 필요, 권장 표시 크기 약 80px |
| BG_Vacuum | 미니게임 배경 | 씬에 없음(신규) | 거실 벽면 배경 | 1920x1080 | Assets/SampleImage/VacuumMiniGame/ | `hand-drawn ink line art simple living room wall background, front view, plain wall surface with a faint baseboard line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | Vacuum_Floor가 바닥·소파를 이미 맡아 이 행은 벽면 전용. §4-4-1: Object 이전 완료(T8.1), BG order -1 권장. 권장 표시 크기: Floor는 화면 전체, Head 약 350px |
| FX_Vacuum_Success | 미니게임 결과 연출 | VacuumMiniGame.unity Head(ResultAnimator, isSuccess) | 성공 연출(먼지가 흡입구로 빨려 들어감) | 512x512 | Assets/SampleImage/VacuumMiniGame/ | `hand-drawn ink line art dust specks being sucked into a vacuum nozzle opening, side view, motion lines curving toward the nozzle, isolated subject`(§1-1 접두+네거티브 공통 적용) | VacuumMiniGame.cs(L44 근처) ResultAnimator·isSuccess 참조, 0.6초 안 |
| FX_Vacuum_Fail | 미니게임 결과 연출 | VacuumMiniGame.unity Dust(SpriteRenderer, failedColor) | 실패 연출(먼지가 튀어 흩날림) | 64x64 | Assets/SampleImage/VacuumMiniGame/ | `hand-drawn ink line art dust clump specks scattering outward in different directions, front view, exaggerated motion trail lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | Vacuum_Dust의 failedColor 색 변경과 연결, §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| FlySwat | 미니게임 정의 | Assets/Data/MiniGames/FlySwat.asset | MiniGameDefinition(category=Home) | — | — | — | 씬 FlySwatMiniGame.unity. 정의 아이콘은 §2-n Category_Home 배지로 대체 |
| FlySwat_Background | 미니게임 오브젝트 | FlySwatMiniGame.unity Background(sprite=Square), ResultRoot(sprite=Circle) | 방 배경(BG_FlySwat 슬롯 겸함) | 1920x1080 | Assets/SampleImage/FlySwatMiniGame/ | `hand-drawn ink line art simple indoor living room background with a window and a hanging light fixture, front view, plain wall and floor line, open empty flying space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경이라 no-background-scenery 항목은 예외) | BG_FlySwat 슬롯 겸함(별도 BG 행 생성 안 함). 창문·전등으로 파리가 날 빈 공간 확보. §4-4-1: Object 이전 완료(T8.1, FlySwat BG order=-10 유지 확인), BG order -1 권장 |
| FlySwat_Fly | 미니게임 오브젝트 | Assets/Prefabs/FlySwatMiniGame/Fly.prefab(SpriteRenderer, 씬에는 없고 FlySwatMiniGame.flyPrefab 필드가 런타임에 Instantiate) | 파리(런타임 생성) | 128x128 | Assets/SampleImage/FlySwatMiniGame/ | 1프레임(날개 위): `hand-drawn ink line art small housefly, top-down view, wings-up frame, isolated subject` / 2프레임(날개 아래): `hand-drawn ink line art small housefly, top-down view, wings-down frame, isolated subject`(§1-1 접두+네거티브 공통 적용, 두 프레임을 번갈아 스왑해 날갯짓 표현) | Fly.prefab 현재 Unity 빌트인 placeholder 스프라이트, 2프레임 모두 신규 제작 필요. 권장 표시 크기 약 150px |
| FlySwat_Swatter | [제안] 미니게임 오브젝트 | 씬·프리팹에 없음(코드 미구현, T1.1 확인) | 파리채 커서/휘두름 | 512x512 | Assets/SampleImage/FlySwatMiniGame/ | `hand-drawn ink line art fly swatter tool viewed from above, mesh paddle head with a simple handle, plus a second faint swing-motion afterimage frame offset to one side, top-down view, isolated subject`(§1-1 접두+네거티브 공통 적용) | 코드에 오브젝트 없음(T1.1 확인) → 커서 연출 구현 시 사용. 클릭 즉시 판정만 하는 현재 구조에 커서 스프라이트·휘두름 연출 추가는 코드·씬 연결 필요(이월). 권장 표시 크기 약 300px. T8.1 채택 확정(파리채 커서·휘두름 방향 확정, 코드 연결은 이월 유지). T8.2 코드·씬 연결(임시 UISprite, FlySwatSwatter.cs) |
| FX_FlySwat_Success | 미니게임 결과 연출 | FlySwatMiniGame.unity ResultRoot(ResultAnimator, isSuccess) | 성공 연출(명중: 납작해진 파리+별 임팩트) | 128x128 | Assets/SampleImage/FlySwatMiniGame/ | `hand-drawn ink line art flattened squashed housefly with a star-shaped impact burst behind it, top-down view, exaggerated impact lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | MINIGAME_IDEAS §6 메모(명중 효과) 반영, FlySwatMiniGame ResultAnimator·isSuccess 참조, 0.6초 안 |
| FX_FlySwat_Fail | 미니게임 결과 연출 | FlySwatMiniGame.unity ResultRoot(ResultAnimator, isFailed) | 실패 연출(파리 놓침) | 128x128 | Assets/SampleImage/FlySwatMiniGame/ | `hand-drawn ink line art empty swatter mark mesh imprint with a small fleeing housefly trailing a dotted escape path, top-down view, isolated subject`(§1-1 접두+네거티브 공통 적용) | §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BossBlob | 미니게임 정의 | Assets/Data/MiniGames/BossBlob.asset | MiniGameDefinition(usage=Special, 보스 전용) | — | — | — | 씬 BossBlobMiniGame.unity. 정의 아이콘은 §2-n Usage_Special 배지로 대체 |
| BossBlob_Boss | 미니게임 오브젝트 | BossBlobMiniGame.unity Boss(sprite=UISprite), Ground(sprite=UISprite) | 보스 형상·바닥 | 1024x1024 | Assets/SampleImage/BossBlobMiniGame/ | `hand-drawn ink line art blob-shaped jelly boss creature sitting on a simple ground puddle base, front view, round wobbly body silhouette with a simple face, small curved horns and a torn necktie fragment draped over the top, sly smirking expression, isolated subject`(§1-1 접두+네거티브 공통 적용) | UISprite placeholder, 신규 제작 필요. §2-b-5 톤 참조(불편대왕 뿔·넥타이 모티프), 콜라이더 형상 불변(blob 실루엣 유지). 권장 표시 크기 약 700px(≈6.5u). T8.1 채택 확정(뿔·넥타이 모티프 확정) |
| BossBlob_Player | 미니게임 오브젝트 | BossBlobMiniGame.unity PlayerDisplay(sprite=Knob), ResultRoot(sprite=Knob) | 플레이어 표시 | 256x256 | Assets/SampleImage/BossBlobMiniGame/ | `hand-drawn ink line art small round player marker token, front view, simple circular token with a tiny office-worker silhouette icon inside, isolated subject`(§1-1 접두+네거티브 공통 적용) | 신규 제작 필요, 권장 표시 크기 약 200px |
| BG_BossBlob | 미니게임 배경 | 씬에 없음(신규) | 특수 보스방 배경 | 1920x1080 | Assets/SampleImage/BossBlobMiniGame/ | `hand-drawn ink line art dark special boss room background, front view, plain dim office floor and faint wall line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | §2-b 몬스터 톤(어두운 사무실) 유지. §4-4-1: Object 이전 완료(T8.1), BG order -1 권장 |
| FX_BossBlob_Success | 미니게임 결과 연출 | BossBlobMiniGame.unity ResultRoot(ResultAnimator, isSuccess) | 성공 연출(blob이 터져 흩어짐) | 1024x1024 | Assets/SampleImage/BossBlobMiniGame/ | `hand-drawn ink line art jelly boss blob bursting apart into scattered droplet fragments, front view, exaggerated splash lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | §2-b 몬스터 톤 유지, ResultAnimator·isSuccess 참조, 0.6초 안 |
| FX_BossBlob_Fail | 미니게임 결과 연출 | BossBlobMiniGame.unity ResultRoot(ResultAnimator, isFailed) | 실패 연출(blob이 부풀어 덮침) | 1024x1024 | Assets/SampleImage/BossBlobMiniGame/ | `hand-drawn ink line art jelly boss blob swelling up and looming over to engulf the player, front view, exaggerated stretch lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | §2-b 몬스터 톤 유지, §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| TapTarget | 미니게임 정의 | Assets/Data/MiniGames/TapTarget.asset | MiniGameDefinition | — | — | — | 씬 TapTargetMiniGame.unity |
| TapTarget_Result | 미니게임 오브젝트 | TapTargetMiniGame.unity ResultRoot(sprite=Circle) | 결과 표시 | 256x256 | Assets/SampleImage/TapTargetMiniGame/ | `hand-drawn ink line art round result stamp shape, front view, simple thick circle outline for a success/fail stamp overlay, isolated subject`(§1-1 접두+네거티브 공통 적용) | 신규 제작 필요 |
| TapTarget_Target | 미니게임 오브젝트 | Assets/Prefabs/TapTargetMiniGame/Target.prefab(SpriteRenderer, 씬에는 없고 TapTargetMiniGame.targetPrefab 필드가 런타임에 Instantiate) | 탭 목표(런타임 생성) | 128x128 | Assets/SampleImage/TapTargetMiniGame/ | `hand-drawn ink line art star-shaped tap target, front view, simple five-point star outline, bright single-tone fill, isolated subject`(§1-1 접두+네거티브 공통 적용) | 현재 Unity 빌트인 UISprite placeholder, 신규 제작 필요 |
| TapTarget_Decoy | 미니게임 오브젝트 | Assets/Prefabs/TapTargetMiniGame/Decoy.prefab(SpriteRenderer, 씬에는 없고 TapTargetMiniGame.decoyPrefab 필드가 런타임에 Instantiate) | 오답 유도 오브젝트(런타임 생성) | 128x128 | Assets/SampleImage/TapTargetMiniGame/ | `hand-drawn ink line art cross-shaped decoy mark, front view, simple thick X outline, different single-tone color fill from the star target, isolated subject`(§1-1 접두+네거티브 공통 적용) | 현재 Unity 빌트인 UISprite placeholder, Target과 구분되는 모양·색으로 신규 제작 필요, 권장 표시 크기 약 200px |
| BG_TapTarget | 미니게임 배경 | 씬에 없음(신규) | 게시판·벽 배경 | 1920x1080 | Assets/SampleImage/TapTargetMiniGame/ | `hand-drawn ink line art simple bulletin board on a plain wall background, front view, faint board frame outline, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | §4-4-1: Object 이전 완료(T8.1), BG order -1 권장 |
| FX_TapTarget_Success | 미니게임 결과 연출 | TapTargetMiniGame.unity ResultRoot(ResultAnimator, isSuccess) | 성공 연출(별이 반짝 터짐) | 256x256 | Assets/SampleImage/TapTargetMiniGame/ | `hand-drawn ink line art five-point star bursting with a bright sparkle flash, front view, exaggerated radiating light lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | ResultAnimator·isSuccess 참조, 0.6초 안. 권장 표시 크기 약 400px |
| FX_TapTarget_Fail | 미니게임 결과 연출 | TapTargetMiniGame.unity ResultRoot(ResultAnimator, isFailed) | 실패 연출(오답 눌러 금 감+흔들림) | 256x256 | Assets/SampleImage/TapTargetMiniGame/ | `hand-drawn ink line art cross-shaped decoy mark with a jagged crack running through it and shaky wobble outline lines, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | §4-2 과장·0.6초·안전영역 17.78×10u 내. 권장 표시 크기 약 400px |
| VacuumWiden | 미니게임 정의 | Assets/Data/MiniGames/VacuumWiden.asset | MiniGameDefinition(category=Home) | — | — | — | 씬 VacuumWidenMiniGame.unity. 정의 아이콘은 §2-n Category_Home 배지로 대체 |
| VacuumWiden_Furniture | 미니게임 오브젝트 | VacuumWidenMiniGame.unity LeftFurniture·RightFurniture(sprite=Square) | 가구 틈 좌우 | 512x512 | Assets/SampleImage/VacuumWidenMiniGame/ | `hand-drawn ink line art piece of furniture leg or cabinet edge cut off at a vertical gap, front view, simple flat silhouette that reads naturally when mirrored for the opposite side, isolated subject`(§1-1 접두+네거티브 공통 적용) | 빌트인 Square placeholder, 좌우 동일 그림을 좌우 반전해 공유. 신규 제작 필요 |
| VacuumWiden_Stick | 미니게임 오브젝트 | VacuumWidenMiniGame.unity Head/Stick(sprite=Square) | 청소기 손잡이 대 | 256x256 | Assets/SampleImage/VacuumWidenMiniGame/ | `hand-drawn ink line art simple straight vacuum cleaner stick pole, side view, thin elongated shape, isolated subject`(§1-1 접두+네거티브 공통 적용) | 빌트인 Square placeholder, 신규 제작 필요 |
| VacuumWiden_WidthHandle | 미니게임 오브젝트 | VacuumWidenMiniGame.unity Head/WidthHandle(sprite=Circle) | 폭 조절 손잡이 | 128x128 | Assets/SampleImage/VacuumWidenMiniGame/ | `hand-drawn ink line art small round grip knob for adjusting width, front view, simple circular handle shape, isolated subject`(§1-1 접두+네거티브 공통 적용) | 빌트인 Circle placeholder, 신규 제작 필요 |
| VacuumWiden_Arrow | 미니게임 오브젝트 | VacuumWidenMiniGame.unity Arrow(sprite=Square) | 안내 화살표(Lv1·2만 표시) | 128x128 | Assets/SampleImage/VacuumWidenMiniGame/ | `hand-drawn ink line art simple horizontal double-headed arrow icon indicating width stretch, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | 빌트인 Square placeholder, Kimbap_Arrow와 달리 좌우 폭 조절을 나타내는 양방향 화살표로 신규 제작 필요 |
| VacuumWiden_Head(헤드 스트레치, [제안]) | 미니게임 오브젝트 | VacuumWidenMiniGame.unity Head/HeadSprite(sprite=Vacuum_Head 공유) | 흡입구 헤드(폭이 늘어남) | 512x512(중앙 반복 가능한 9-slice 또는 Left/Center/Right 3분할) | Assets/SampleImage/VacuumWidenMiniGame/ | `hand-drawn ink line art vacuum cleaner nozzle head designed as a horizontally tileable accordion-style opening, side view, repeating simple ribbed pattern in the middle section so stretching does not distort the silhouette, isolated subject`(§1-1 접두+네거티브 공통 적용) | **9-slice 또는 좌·중·우 3분할 권장**: `VacuumWidenMiniGame.cs ApplyWidth`가 단일 스프라이트를 가로로 최대 약 3.3배(`minWidth 1.2`~`maxWidth 4`) 균일 스케일하므로, 아코디언형 반복 패턴으로 그리거나(현재 스크립트 그대로 사용 가능) 좌·중·우 3분할로 바꾸려면 스크립트 수정이 추가로 필요(이번 범위 밖, 참고만). 당장은 Vacuum_Head 공유로 완료 상태 |
| BG_VacuumWiden | 미니게임 배경 | 씬에 없음(신규) | 가구 틈 배경 | 1920x1080 | Assets/SampleImage/VacuumWidenMiniGame/ | `hand-drawn ink line art simple living room interior background behind a furniture gap, front view, plain floor and wall lines, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | LeftFurniture/RightFurniture가 가구 자체를 맡으므로 이 행은 방 배경 전용. BG order -1 권장 |
| FX_VacuumWiden_Success | 미니게임 결과 연출 | VacuumWidenMiniGame.unity ResultRoot(Animator VacuumWidenResult, isSuccess) | 성공 연출(먼지 흡입) | 256x256 | Assets/SampleImage/VacuumWidenMiniGame/ | `hand-drawn ink line art dust specks being sucked into a vacuum nozzle opening, side view, motion lines curving toward the nozzle, isolated subject`(§1-1 접두+네거티브 공통 적용) | ResultRoot Animator·isSuccess 참조, 0.6초 안. Vacuum의 FX_Vacuum_Success와 모티프 공유 가능 |
| FX_VacuumWiden_Fail | 미니게임 결과 연출 | VacuumWidenMiniGame.unity ResultRoot(Animator VacuumWidenResult, isFailed) | 실패 연출(Wide=쾅 걸림 / Narrow=먼지 남음 2종) | 256x256 | Assets/SampleImage/VacuumWidenMiniGame/ | Wide: `hand-drawn ink line art vacuum head jammed and stuck against furniture edges with small impact burst lines, side view, isolated subject` / Narrow: `hand-drawn ink line art narrow vacuum nozzle opening with leftover dust specks not fully collected, side view, isolated subject`(둘 다 §1-1 접두+네거티브 공통 적용) | FailKind(Wide/Narrow) 두 경로 구분(코드 L291/300/307) — Wide는 ShakeFurniture 연출과, Narrow는 Dust localScale 0.5 축소와 함께 사용. §4-2 과장·0.6초·안전영역 17.78×10u 내 |

### 2-m. 예정 미니게임 (Docs/MINIGAME_IDEAS.md §3, §4 — 미구현)

MINIGAME_IDEAS.md §5(1차 제작 순서, L132-147)를 따라 정렬. 각 게임은 §4-1 슬롯 템플릿(BG 1·Target 1~3·Success·Fail)을 채운다. 회피용(3초 이하: MG-52·MG-51·MG-61·MG-23·MG-05 변형)은 Target 프롬프트에 큰 실루엣 지시를 넣는다(비고 참고).

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| BG_Zipper | 예정 미니게임 배경 | MINIGAME_IDEAS §5 순번6 MG-10(Wear, 드래그) | 옷감 지퍼 배경 | 1920x1080 | Assets/SampleImage/ZipperMiniGame/ | `hand-drawn ink line art simple jacket front close-up background around the zipper track, front view, plain fabric texture, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG10_Zipper | 예정 미니게임 오브젝트 | MINIGAME_IDEAS §4 MG-10(Wear, 드래그) | 지퍼·슬라이더 | 512x512 | Assets/SampleImage/ZipperMiniGame/ | 지퍼 트랙: `hand-drawn ink line art vertical zipper track on fabric, front view, evenly spaced zipper teeth down a narrow strip, isolated subject` / 슬라이더: `hand-drawn ink line art zipper pull slider tab, front view, small tab with a pull loop, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 1차 추천. 물린 지점은 톱니 사이에 작은 천 조각(구겨진 fabric snag) 아이콘 추가 생성 권장 |
| FX_Zipper_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번6 | 성공 연출(끝까지 올라감) | 256x256 | Assets/SampleImage/ZipperMiniGame/ | `hand-drawn ink line art zipper fully closed at the top with the pull tab resting flat, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_Zipper_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번6 | 실패 연출(천 씹힘) | 256x256 | Assets/SampleImage/ZipperMiniGame/ | `hand-drawn ink line art zipper jammed with a torn fabric snag caught in the teeth, exaggerated frayed thread lines, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_PizzaHalf | 예정 미니게임 배경 | MINIGAME_IDEAS §5 순번7 MG-07(Food, 긋기) | 주방 테이블 배경 | 1920x1080 | Assets/SampleImage/PizzaHalfMiniGame/ | `hand-drawn ink line art simple kitchen table background, top-down view, plain wooden table surface, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG07_PizzaHalf | 예정 미니게임 오브젝트 | MINIGAME_IDEAS §3 MG-07(Food, 긋기) | 피자 절반 자르기 점선·나이프 | 1024x1024 | Assets/SampleImage/PizzaHalfMiniGame/ | 피자: `hand-drawn ink line art whole round pizza with a faint center dashed cut-guide line, top-down view, isolated subject` / 나이프: `hand-drawn ink line art kitchen knife, top-down view, simple blade and handle silhouette, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 긋기 입력 구현 필요(ROADMAP F-32). 기존 Pizza_TotalPizza 톤 재사용 가능 |
| FX_PizzaHalf_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번7 | 성공 연출(균등 절반) | 256x256 | Assets/SampleImage/PizzaHalfMiniGame/ | `hand-drawn ink line art two pizza halves cut evenly along the center line, top-down view, clean straight cut edge, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_PizzaHalf_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번7 | 실패 연출(한쪽이 커서 토핑 이탈) | 256x256 | Assets/SampleImage/PizzaHalfMiniGame/ | `hand-drawn ink line art unevenly cut pizza with one oversized jagged half and cheese strings flying off, top-down view, exaggerated crumbs scattering, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_Puzzle | 예정 미니게임 배경 | MINIGAME_IDEAS §5 순번8 MG-04(Home, 드래그) | 퍼즐 테이블 배경 | 1920x1080 | Assets/SampleImage/PuzzleMiniGame/ | `hand-drawn ink line art simple table background with a faint empty 3x3 grid outline, top-down view, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | 확정(MINIGAME_IDEAS §5 순번8). 마지막 빈칸 후보 3개(정답1+오답2) 보강안 반영, 모델 등록 후 생성 대기 |
| MG04_Puzzle | 예정 미니게임 오브젝트 | MINIGAME_IDEAS §3 MG-04(Home, 드래그) | 퍼즐 조각(정답용 1종 원본) | 256x256 | Assets/SampleImage/PuzzleMiniGame/ | `hand-drawn ink line art single square jigsaw puzzle piece with one face-fragment illustration fragment on it (eye or mouth hint), flat front view, simple interlocking tab-and-blank edges, isolated subject`(§1-1 접두+네거티브 공통 적용) | 확정. 3x3 조각 세트는 이 프롬프트로 생성, 마지막 빈칸용 오답 2개는 `MG04_Puzzle_Decoy` 행 참고 |
| MG04_Puzzle_Decoy | 예정 미니게임 오브젝트 | MINIGAME_IDEAS §5 순번8 "마지막 한 조각" 보강안 | 마지막 빈칸 오답 조각 2종 | 256x256 | Assets/SampleImage/PuzzleMiniGame/ | `hand-drawn ink line art single square jigsaw puzzle piece, identical silhouette and tab-and-blank edges to the reference piece, but with the eye angle/blush position subtly shifted, flat front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | 신규 확정 행. 정답 조각과 테두리·형태 동일, 얼굴 부위 세부만 미세하게 달라 헷갈리게 함(2종 생성) |
| FX_Puzzle_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번8 | 성공 연출(완성) | 256x256 | Assets/SampleImage/PuzzleMiniGame/ | `hand-drawn ink line art completed 3x3 face puzzle with all pieces fitted seamlessly, top-down view, isolated subject`(§1-1 접두+네거티브 공통 적용) | 확정, §4-2 기준 0.6초 안. 정답 조각 스냅 고정 시 재생 |
| FX_Puzzle_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번8 | 실패 연출(오답 조각 튕겨나감) | 256x256 | Assets/SampleImage/PuzzleMiniGame/ | `hand-drawn ink line art puzzle piece popping back out of its slot with a small crack shake mark, top-down view, isolated subject`(§1-1 접두+네거티브 공통 적용) | 확정, §4-2 과장·0.6초·안전영역 17.78×10u 내. 오답(MG04_Puzzle_Decoy) 드롭 시 재생 |
| BG_RamenLid | 예정 미니게임 배경 | MINIGAME_IDEAS §5 순번9 MG-20(Food, 드래그 정지) | 편의점 테이블 배경 | 1920x1080 | Assets/SampleImage/RamenLidMiniGame/ | `hand-drawn ink line art simple table background, top-down view, plain surface, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG20_RamenLid | 예정 미니게임 오브젝트 | MINIGAME_IDEAS §4 MG-20(Food, 드래그 정지) | 컵라면 뚜껑 반만 열기 | 512x512 | Assets/SampleImage/RamenLidMiniGame/ | 컵: `hand-drawn ink line art cup noodle container, top-down view, round cup body with a hinged foil lid partially open on one side, a faint stop-line marked on the lid, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 1차 추천. 정지선 위치는 스프라이트 위에 얇은 점선으로만 표시, 실제 판정선은 코드에서 랜덤 배치 |
| FX_RamenLid_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 성공 연출(정지선에서 멈춤) | 256x256 | Assets/SampleImage/RamenLidMiniGame/ | `hand-drawn ink line art cup noodle lid held open exactly at the stop-line with steam lines rising, top-down view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_RamenLid_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 실패 연출(뚜껑 다 뜯김) | 256x256 | Assets/SampleImage/RamenLidMiniGame/ | `hand-drawn ink line art cup noodle lid torn completely off flying away with hot water splashing out, top-down view, exaggerated splash lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_Ketchup | 예정 미니게임 배경 | MINIGAME_IDEAS §5 순번9 MG-22(Food, 문지르기+홀드) | 테이블·감자튀김 배경 | 1920x1080 | Assets/SampleImage/KetchupMiniGame/ | `hand-drawn ink line art simple table background with a plate of fries nearby, top-down view, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG22_Ketchup | 예정 미니게임 오브젝트 | MINIGAME_IDEAS §4 MG-22(Food, 문지르기+홀드) | 케첩 봉투 | 512x512 | Assets/SampleImage/KetchupMiniGame/ | `hand-drawn ink line art nearly empty ketchup sachet packet, front view, flat pouch shape with a small torn corner opening and a faint fill-level line near the bottom, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 문지르기 입력 구현 필요(ROADMAP F-32). 짜지는 양은 게이지 UI(§2-e류 막대)로 별도 표현 |
| FX_Ketchup_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 성공 연출(적당량 짜짐) | 256x256 | Assets/SampleImage/KetchupMiniGame/ | `hand-drawn ink line art neat ketchup squeeze landing in a small dollop on a plate, top-down view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_Ketchup_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 실패 연출(사방에 튐) | 256x256 | Assets/SampleImage/KetchupMiniGame/ | `hand-drawn ink line art ketchup packet bursting with sauce spraying wildly in all directions, top-down view, exaggerated splash lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_ChargerCable | 예정 미니게임 배경 | MINIGAME_IDEAS §5 순번9 MG-30(Device, 드래그) | 가방 속 배경 | 1920x1080 | Assets/SampleImage/ChargerMiniGame/ | `hand-drawn ink line art simple bag interior background, top-down view, plain fabric lining texture, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG30_ChargerCable | 예정 미니게임 오브젝트 | MINIGAME_IDEAS §4 MG-30(Device, 드래그) | 꼬인 충전기 줄 | 512x512 | Assets/SampleImage/ChargerMiniGame/ | `hand-drawn ink line art tangled charger cable with multiple knots, top-down view, single continuous cable line looping into several loose knots, plug ends visible at both tips, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 1차 추천. 매듭 수↑ 난이도는 같은 프롬프트로 매듭 개수만 늘려 재생성 |
| FX_ChargerCable_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 성공 연출(일자로 풀림) | 256x256 | Assets/SampleImage/ChargerMiniGame/ | `hand-drawn ink line art fully untangled straight charger cable lying flat, top-down view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_ChargerCable_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 실패 연출(더 꼬임) | 256x256 | Assets/SampleImage/ChargerMiniGame/ | `hand-drawn ink line art charger cable pulled into a tighter double knot with frayed strain lines, top-down view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_TissueEnd | 예정 미니게임 배경 | MINIGAME_IDEAS §5 순번9 MG-41(Home, 문지르기→탭) | 욕실 선반 배경 | 1920x1080 | Assets/SampleImage/TissueMiniGame/ | `hand-drawn ink line art simple bathroom shelf background, front view, plain wall and shelf line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG41_TissueEnd | 예정 미니게임 오브젝트 | MINIGAME_IDEAS §4 MG-41(Home, 문지르기→탭) | 휴지 끝 찾기 | 512x512 | Assets/SampleImage/TissueMiniGame/ | 두루마리: `hand-drawn ink line art round tissue paper roll, top-down view, spiral wound texture lines, no visible loose end, isolated subject` / 들뜬 끝(탭 대상): `hand-drawn ink line art tiny lifted tissue paper edge tab on the roll surface, top-down view, faint barely-visible corner, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 문지르기 입력 구현 필요. 끝 표시를 흐리게 하는 난이도는 두 번째 스프라이트의 불투명도를 코드에서 낮춰 처리 |
| FX_TissueEnd_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 성공 연출(끝을 잡음) | 256x256 | Assets/SampleImage/TissueMiniGame/ | `hand-drawn ink line art tissue roll with a clearly pulled-out loose end tab, top-down view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_TissueEnd_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 실패 연출(시간 초과·헛손질) | 256x256 | Assets/SampleImage/TissueMiniGame/ | `hand-drawn ink line art tissue roll shredded into scattered torn paper bits, top-down view, exaggerated scatter lines, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_TransitCard | 예정 미니게임 배경 | MINIGAME_IDEAS §5 순번9 MG-50(Outside, 드래그) | 지갑 내부 배경 | 1920x1080 | Assets/SampleImage/TransitCardMiniGame/ | `hand-drawn ink line art simple wallet interior background, front view, plain card-slot lining texture, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG50_TransitCard | 예정 미니게임 오브젝트 | MINIGAME_IDEAS §4 MG-50(Outside, 드래그) | 교통카드 두 장 | 512x512 | Assets/SampleImage/TransitCardMiniGame/ | 정답 카드: `hand-drawn ink line art transit card, front view, simple rectangle card with a rounded corner and a small chip icon, isolated subject` / 오답 카드(비슷한 색): `hand-drawn ink line art membership card, front view, same simple rectangle shape as the transit card but a plain corner without a chip icon, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 1차 추천. 난이도(비슷한 색)는 두 카드에 같은 색 팔레트 지정해 재생성 |
| FX_TransitCard_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 성공 연출(삑) | 256x256 | Assets/SampleImage/TransitCardMiniGame/ | `hand-drawn ink line art transit card tapped cleanly on a reader with a small check mark beep icon, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_TransitCard_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번9 | 실패 연출(삐빅 오류) | 256x256 | Assets/SampleImage/TransitCardMiniGame/ | `hand-drawn ink line art wrong card bouncing off a reader with a small error X buzz icon, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_Elevator | 예정 미니게임 배경(회피) | MINIGAME_IDEAS §5 순번10 MG-52(Outside, 회피 3초 이하) | 엘리베이터 내부 배경 | 1920x1080 | Assets/SampleImage/ElevatorMiniGame/ | `hand-drawn ink line art simple elevator interior background, front view, plain metallic wall panel, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG52_Elevator | 예정 미니게임 오브젝트(회피) | MINIGAME_IDEAS §4 MG-52(Outside, 회피 3초 이하) | 엘리베이터 문 | 512x512 | Assets/SampleImage/ElevatorMiniGame/ | `hand-drawn ink line art elevator sliding double doors, front view, metallic vertical seam down the center, a small close-door button panel beside it, filling most of the frame as one bold readable silhouette, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 회피 페이즈용(MG-52), 지시문 즉시 반응형 짧은 게임. 화면 대부분을 채우는 큰 실루엣으로 한눈에 읽히게(3초 이하 회피 기준) |
| FX_Elevator_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번10 | 성공 연출(닫힘) | 256x256 | Assets/SampleImage/ElevatorMiniGame/ | `hand-drawn ink line art elevator doors slammed shut with a small motion burst line, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_Elevator_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번10 | 실패 연출(시간 초과) | 256x256 | Assets/SampleImage/ElevatorMiniGame/ | `hand-drawn ink line art elevator doors stuck wide open with an exaggerated buzzer alarm icon, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_AutoDoor | 예정 미니게임 배경(회피) | MINIGAME_IDEAS §5 순번10 MG-51(Outside, 회피) | 건물 입구 배경 | 1920x1080 | Assets/SampleImage/AutoDoorMiniGame/ | `hand-drawn ink line art simple building entrance background, front view, plain wall and floor line, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG51_AutoDoor | 예정 미니게임 오브젝트(회피) | MINIGAME_IDEAS §4 MG-51(Outside, 회피) | 자동문 | 512x512 | Assets/SampleImage/AutoDoorMiniGame/ | `hand-drawn ink line art automatic sliding glass door, front view, two glass panels meeting at a center seam, a small sensor dot above the frame, filling most of the frame as one bold readable silhouette, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 회피 페이즈용(MG-51). 화면 대부분을 채우는 큰 실루엣으로 한눈에 읽히게(3초 이하 회피 기준) |
| FX_AutoDoor_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번10 | 성공 연출(인식됨) | 256x256 | Assets/SampleImage/AutoDoorMiniGame/ | `hand-drawn ink line art automatic door fully open with a small green check sensor light, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_AutoDoor_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번10 | 실패 연출(부딪힘) | 256x256 | Assets/SampleImage/AutoDoorMiniGame/ | `hand-drawn ink line art automatic door stuck shut with a character silhouette bumping into the glass, exaggerated impact lines, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_PlasticWrap | 예정 미니게임 배경(회피) | MINIGAME_IDEAS §5 순번10 MG-61(Peel, 회피) | 마트 계산대 배경 | 1920x1080 | Assets/SampleImage/PlasticWrapMiniGame/ | `hand-drawn ink line art simple grocery counter background, front view, plain counter surface, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG61_PlasticWrap | 예정 미니게임 오브젝트(회피) | MINIGAME_IDEAS §4 MG-61(Peel, 회피) | 비닐 포장 벌리기 | 512x512 | Assets/SampleImage/PlasticWrapMiniGame/ | `hand-drawn ink line art plastic shopping bag opening, front view, thin crinkled film outline pinched shut at the mouth, two small handle loops at the top, filling most of the frame as one bold readable silhouette, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 회피 페이즈용(MG-61). 화면 대부분을 채우는 큰 실루엣으로 한눈에 읽히게(3초 이하 회피 기준) |
| FX_PlasticWrap_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번10 | 성공 연출(벌어짐) | 256x256 | Assets/SampleImage/PlasticWrapMiniGame/ | `hand-drawn ink line art plastic bag mouth stretched wide open, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_PlasticWrap_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §5 순번10 | 실패 연출(시간 초과) | 256x256 | Assets/SampleImage/PlasticWrapMiniGame/ | `hand-drawn ink line art plastic bag mouth stuck pinched shut with fingers slipping off, exaggerated motion lines, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_Straw | 예정 미니게임 배경(회피) | MINIGAME_IDEAS §4 MG-23(Food, 회피) | 테이블 배경 | 1920x1080 | Assets/SampleImage/StrawMiniGame/ | `hand-drawn ink line art simple table background, front view, plain surface, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현 |
| MG23_Straw | 예정 미니게임 오브젝트(회피) | MINIGAME_IDEAS §4 MG-23(Food, 회피) | 빨대 꽂기 | 256x256 | Assets/SampleImage/StrawMiniGame/ | `hand-drawn ink line art drink carton with a small foil straw-hole target dot, front view, simple box carton shape with a straw resting beside it, filling most of the frame as one bold readable silhouette, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 회피 페이즈용(MG-23), 몬스터 특수 공격 연계(§4 127행 참고). 화면 대부분을 채우는 큰 실루엣으로 한눈에 읽히게(3초 이하 회피 기준) |
| FX_Straw_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §4 MG-23 | 성공 연출(쏙 들어감) | 256x256 | Assets/SampleImage/StrawMiniGame/ | `hand-drawn ink line art straw inserted neatly through the foil hole, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_Straw_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §4 MG-23 | 실패 연출(팩 넘침) | 256x256 | Assets/SampleImage/StrawMiniGame/ | `hand-drawn ink line art drink carton crumpled with juice overflowing from a torn straw hole, exaggerated splash lines, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |
| BG_USB_Flip | 예정 미니게임 배경(회피 변형) | MINIGAME_IDEAS §3 MG-05 변형(회피, 속도↑) | 책상 배경 | 1920x1080 | Assets/SampleImage/USBMiniGame/ | `hand-drawn ink line art simple desk background, front view, plain desk surface, open empty space for gameplay, isolated subject`(§1-1 접두+네거티브 공통 적용, 배경 예외) | [예정] 미구현. 기존 USBMiniGame BG와 공유 가능(재사용 검토) |
| MG05_USB_Flip | 예정 미니게임 오브젝트(변형·회피) | MINIGAME_IDEAS §3 MG-05 변형(탭, 세 번 뒤집기) | USB 뒤집기 애니메이션 프레임 | 512x512 | Assets/SampleImage/USBMiniGame/ | 정면: `hand-drawn ink line art usb type-a connector plug, front-facing orientation showing the flat metal contact side, large bold readable silhouette, isolated subject` / 뒷면: `hand-drawn ink line art usb type-a connector plug, back-facing orientation showing the plain shell side, large bold readable silhouette, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. 기존 USB 미니게임의 변형판(속도↑ 버전, §4 127행 회피 목록), 두 프레임을 번갈아 뒤집는 연출. 회피용이라 화면 대부분을 채우는 큰 실루엣으로 한눈에 읽히게 |
| FX_USB_Flip_Success | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §3 MG-05 변형 | 성공 연출(포트에 꽂힘) | 256x256 | Assets/SampleImage/USBMiniGame/ | `hand-drawn ink line art usb connector plugged in flush with a small green check spark, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 기준 0.6초 안 |
| FX_USB_Flip_Fail | 예정 미니게임 결과 연출 | MINIGAME_IDEAS §3 MG-05 변형 | 실패 연출(어긋남) | 256x256 | Assets/SampleImage/USBMiniGame/ | `hand-drawn ink line art usb connector jammed sideways against the port with sparks flying, exaggerated impact lines, front view, isolated subject`(§1-1 접두+네거티브 공통 적용) | [예정] 미구현. §4-2 과장·0.6초·안전영역 17.78×10u 내 |

### 2-o. 공용 UI 컴포넌트 (T3)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| UI_TopBar | 공용 UI 프레임 | HQ.unity Canvas/SafeArea/TopBar(h120), Battle.unity Canvas/BattleHud/TopBar(h140) | 상단 정보 바 배경 | 512x128 | Assets/SampleImage/UI/ | `flat UI bar frame, wide horizontal cream paper strip with a thin ink line along the bottom edge, front view, orthographic UI element, uniform border thickness, plain stretchable center, empty interior for stage/currency text`(§1-1 접두+네거티브 공통 적용) | 9-slice 48,48,24,24, 표시 가변(h120~140), HQ·Battle TopBar 공용 |
| UI_CloseButton | 공용 UI 버튼 | HQ.unity Canvas/SafeArea/AgentMenuPanel/CloseButton, Canvas/SafeArea/SquadPanel/SquadCloseButton | 패널 닫기 버튼 | 256x256 | Assets/SampleImage/UI/ | `flat UI button icon, round cream paper button with a simple hand-drawn ink X mark centered, front view, orthographic UI element, uniform border thickness, plain stretchable center`(§1-1 접두+네거티브 공통 적용) | Simple(9-slice 0), 표시 100x100, 누름 상태는 Button 틴트로 처리 |
| UI_TabButton_On | 공용 UI 버튼 | HQ.unity Canvas/SafeArea/AgentMenuPanel (신규 자식, 미생성) 탭 버튼 3개 중 선택 상태 | AgentMenu 탭 선택 표시 | 512x192 | Assets/SampleImage/UI/ | `flat UI tab button frame, wide rounded rectangle with a thick bold ink border, bottom edge open (no border line at bottom to blend into panel), cream paper texture fill, front view, orthographic UI element, uniform border thickness, plain stretchable center`(§1-1 접두+네거티브 공통 적용) | 9-slice 48,48,48,24, 표시 240x96, UI_TabButton_Off와 짝 |
| UI_TabButton_Off | 공용 UI 버튼 | 위와 동일(신규 자식, 미생성) 탭 버튼 3개 중 비선택 상태 | AgentMenu 탭 비선택 표시 | 512x192 | Assets/SampleImage/UI/ | `flat UI tab button frame, wide rounded rectangle with a thin ink border, slightly darker muted cream paper texture fill, front view, orthographic UI element, uniform border thickness, plain stretchable center`(§1-1 접두+네거티브 공통 적용) | 9-slice 48,48,48,24, 표시 240x96, UI_TabButton_On보다 테두리 얇고 톤 어두움 |
| UI_CardFrame | 공용 UI 패널 | UpgradeChoiceHud/Card_0~2(신규 자식, 미생성 — 빌드 씬 전체 조회 결과 UpgradeChoiceHud/Card 오브젝트 미발견, 런타임 동적 생성 추정) | 강화 선택 카드 전용 세로형 프레임 | 512x683 | Assets/SampleImage/UI/ | `flat UI card frame, tall vertical rounded rectangle border, cream paper texture fill matching DialogueFrame_Panel style, a thin horizontal ink divider line separating an upper icon slot area from a lower name/description text area, front view, orthographic UI panel, uniform border thickness, plain stretchable center, empty interior`(§1-1 접두+네거티브 공통 적용) | 9-slice 64,64,64,64, 표시 480x640, UI_ResultPanel과 별도 전용 프레임 |
| UI_PanelDark | 공용 UI 패널 | HQ.unity Canvas/SafeArea/AgentMenuPanel, Canvas/SafeArea/SquadPanel 배경 | 어두운 패널 변형 | 512x512 | Assets/SampleImage/UI/ | `flat UI panel frame, thick rounded rectangle border, dark navy paper texture fill with a cream ink border outline, front view, orthographic UI panel, uniform border thickness, plain stretchable center, empty interior`(§1-1 접두+네거티브 공통 적용) | 9-slice 64,64,64,64, panel.base 틴트 대신 별도 스프라이트로 제작(잉크 선 뭉개짐 방지) |
| UI_NameTag | 공용 UI 요소 | HQ.unity/Battle.unity Canvas/DialogueBox (신규 자식, 미생성 — 실측 결과 DialogueBox 하위에 BoxBg/Portrait/TapButton만 있고 nameText 없음) | 대화창 화자 이름표 배경 | 384x96 | Assets/SampleImage/UI/ | `flat UI ribbon tag frame, small horizontal name-plate ribbon shape with folded ribbon ends, cream paper texture fill matching DialogueFrame_Panel style, front view, orthographic UI element, uniform border thickness, plain stretchable center, empty interior for name text`(§1-1 접두+네거티브 공통 적용) | 9-slice 40,40,24,24, 표시 360x64 |
| UI_TapNext | 공용 UI 요소 | HQ.unity/Battle.unity Canvas/DialogueBox/TapButton(기존 오브젝트, 실측 확인됨) | 대화창 '탭하여 계속' 표시 | 128x128 | Assets/SampleImage/UI/ | `flat icon, small single-stroke ink triangle pointing downward, plain single-tone fill, transparent background, isolated subject, front view, orthographic UI element, uniform border thickness, plain stretchable center`(§1-1 접두+네거티브 공통 적용) | Simple(9-slice 0), 표시 48x48, 깜빡임 연출은 코드 처리 |
| UI_ButtonDisabled | 공용 UI 버튼 | Title.unity Canvas/SafeArea/StartButton, HQ.unity Canvas/SortieButton, ConfirmButton 등 SpriteState.disabledSprite | 버튼 비활성 상태 | 512x256 | Assets/SampleImage/UI/ | `flat UI button frame, wide rounded rectangle border, dull gray paper texture fill (#5A5A66), faint muted ink border, front view, orthographic UI panel, uniform border thickness, plain stretchable center, empty interior for label text`(§1-1 접두+네거티브 공통 적용) | 9-slice 56,56,56,56(UI_StartButton 실측값과 동일), 표시 512x256 비율 유지, Button.SpriteState.disabledSprite로 적용 |

### 2-o-1. HUD 아이콘 (T4)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| Icon_Crystal | HUD 아이콘 | HQ.unity Canvas/SafeArea/TopBar/CrystalIcon(기존 오브젝트, sprite=Usage_Energy 임시 사용, 실측 확인됨) | crystalText(HubScreen.crystalText, Text) 옆 재화 아이콘 | 원본 128x128, 표시 64x64 | Assets/SampleImage/UI/Icons/ | `flat icon, small glowing crystal shard, no badge ring, front view`(§1-1 접두+네거티브 공통 적용) | Usage_Energy는 미니게임 용도 배지(§2-n)로 역할이 달라 전용 아이콘으로 분리. 교체 시 CrystalIcon.sprite를 Icon_Crystal로 바꾸면 된다(코드 변경 불필요). T4.1: 실제 스프라이트 생성은 에셋 생성 도구 동의가 필요해 이번 단계에서 미실행(다음 단계 이월) |
| Maze_DirArrow | HUD 아이콘 | Battle.unity Canvas/BattleHud/MapPanel/DirButton_Up~Right/Arrow(T4.1에서 신규 자식 생성 완료, Unity_RunCommand로 4개 모두 존재 확인) | 방향 버튼 위 화살표 그림, MapHud.buttons[0..3] 배경 위에 얹음 | 원본 128x128, 표시 64x64 | Assets/SampleImage/Battle/Maze/ | `simple hand-drawn ink up-arrow icon, flat single-tone color fill, transparent background, isolated subject, front view`(§1-1 접두+네거티브 공통 적용) | T4.1: 각 DirButton_Up/Down/Left/Right 아래 Arrow 자식을 만들고 RectTransform.localEulerAngles.z = 0/180/90/-90으로 4방향 적용, Battle.unity 저장 완료. 현재 sprite는 자리표시(UI/Skin/UISprite.psd, 흰 사각) — 실제 화살표 그림(위 프롬프트) 생성 후 4개 Arrow의 Image.sprite를 이 에셋으로 교체하면 된다(회전은 이미 씬에 고정돼 있어 추가 코드 불필요) |
| MazeIcon_Start | HUD 아이콘(맵 방 유형) | Battle.unity MapPanel/MazeGrid/Cell_x_y/Icon(런타임 생성, `MapHud.cs` BuildGrid에서 매 칸마다 생성. cellIcons[x,y]) | MazeRoomType.Start 셀 표시, LetterFor의 "S" 대체 | 원본 128x128, 표시는 셀(cellSize=100)의 약 60% | Assets/SampleImage/Battle/Maze/ | `flat icon, simple white silhouette of a starting flag, no outline color, front view`(§1-1 접두+네거티브 공통 적용) | T4.1: `MapHud.cs`에 public Sprite startIcon 필드 추가, RefreshGrid의 IconFor(type)로 연결 완료(필드가 비어 있으면 Icon 자식을 비활성 상태로 두고 기존 글자로 자동 대체). 인스펙터에서 MapHud.startIcon에 실제 스프라이트를 연결하는 작업은 스프라이트 생성 후 이월 |
| MazeIcon_Combat | HUD 아이콘(맵 방 유형) | 위와 동일, Cell_x_y/Icon(런타임 생성) | MazeRoomType.Combat 셀 표시, LetterFor의 "전" 대체 | 원본 128x128, 표시는 셀의 약 60% | Assets/SampleImage/Battle/Maze/ | `flat icon, simple white silhouette of a clenched fist, no outline color, front view`(§1-1 접두+네거티브 공통 적용) | T4.1: `MapHud.combatIcon` 필드 추가·연결 완료(코드). 스프라이트 연결은 이월 |
| MazeIcon_Elite | HUD 아이콘(맵 방 유형) | 위와 동일, Cell_x_y/Icon(런타임 생성) | MazeRoomType.Elite 셀 표시, LetterFor의 "엘" 대체 | 원본 128x128, 표시는 셀의 약 60% | Assets/SampleImage/Battle/Maze/ | `flat icon, simple white silhouette of a skull with a small star, no outline color, front view`(§1-1 접두+네거티브 공통 적용) | T4.1: `MapHud.eliteIcon` 필드 추가·연결 완료(코드). 스프라이트 연결은 이월 |
| MazeIcon_Rest | HUD 아이콘(맵 방 유형) | 위와 동일, Cell_x_y/Icon(런타임 생성) | MazeRoomType.Rest 셀 표시, LetterFor의 "휴" 대체 | 원본 128x128, 표시는 셀의 약 60% | Assets/SampleImage/Battle/Maze/ | `flat icon, simple white silhouette of a coffee cup, no outline color, front view`(§1-1 접두+네거티브 공통 적용) | T4.1: `MapHud.restIcon` 필드 추가·연결 완료(코드). 스프라이트 연결은 이월 |
| MazeIcon_Boss | HUD 아이콘(맵 방 유형) | 위와 동일, Cell_x_y/Icon(런타임 생성) | MazeRoomType.Boss 셀 표시, LetterFor의 "B" 대체 | 원본 128x128, 표시는 셀의 약 60% | Assets/SampleImage/Battle/Maze/ | `flat icon, simple white silhouette of a small crown, no outline color, front view`(§1-1 접두+네거티브 공통 적용) | T4.1: `MapHud.bossIcon` 필드 추가·연결 완료(코드). 스프라이트 연결은 이월 |
| Icon_Settings | HUD 아이콘(타이틀 보조 메뉴) | Title.unity Canvas/SafeArea/BottomMenu/SettingsButton/Icon(기존 오브젝트, sprite=Icon_Skill_Comic 임시 사용) | TitleMenu.settingsButton 아이콘, 설정 팝업 진입 | 원본 128x128, 표시 120x120 | Assets/SampleImage/UI/Icons/ | `flat icon, simple hand-drawn ink gear/cog cross-section, no badge ring, front view`(§1-1 접두+네거티브 공통 적용) | §27.3 L662 "임시 사용 중" 이슈 해결용 신규 프롬프트(T5.1). 교체 시 SettingsButton/Icon의 Image.sprite만 바꾸면 됨(코드·버튼 크기 변경 불필요). 실제 이미지 생성·교체는 이월 |
| Icon_Notice | HUD 아이콘(타이틀 보조 메뉴) | Title.unity Canvas/SafeArea/BottomMenu/NoticeButton/Icon(기존 오브젝트, sprite=Icon_Talk_Comic 임시 사용) | TitleMenu.noticeButton 아이콘, 공지 팝업 진입 | 원본 128x128, 표시 120x120 | Assets/SampleImage/UI/Icons/ | `flat icon, simple hand-drawn ink megaphone/speaker shape, no badge ring, front view`(§1-1 접두+네거티브 공통 적용) | §27.3 L662 "임시 사용 중" 이슈 해결용 신규 프롬프트(T5.1). 교체 시 NoticeButton/Icon의 Image.sprite만 바꾸면 됨. 실제 이미지 생성·교체는 이월 |
| Icon_Credits | HUD 아이콘(타이틀 보조 메뉴) | Title.unity Canvas/SafeArea/BottomMenu/CreditsButton/Icon(기존 오브젝트, sprite=Icon_Heart_Comic 임시 사용) | TitleMenu.creditsButton 아이콘, 크레딧 팝업 진입(가이드 원안 SupportButton 대신 채택안) | 원본 128x128, 표시 120x120 | Assets/SampleImage/UI/Icons/ | `flat icon, simple hand-drawn ink open-book/scroll shape with a small star accent, no badge ring, front view`(§1-1 접두+네거티브 공통 적용) | §27.3 L648/L662 "임시 사용 중" 이슈 해결용 신규 프롬프트(T5.1). 교체 시 CreditsButton/Icon의 Image.sprite만 바꾸면 됨. 실제 이미지 생성·교체는 이월 |

**제외 후보**
- 아이콘-스테이지(stageText 옆): `HubScreen.cs`에 stageText(Text)만 있고 아이콘 필드·오브젝트가 없다. UI_DESIGN §13 L392도 "문구만으로 충분"이라고 판정해 이번 T4에서 제외한다.
- MazeRoomType.Empty 아이콘: `MapHud.cs` LetterFor(257-290행)에 Empty용 대응 글자가 없어(닿지 않는 방이거나 미사용 값) 아이콘을 만들 대상이 없다.
- Maze_DirButton 재정의: 버튼 배경은 §18.2에서 이미 UI_SquadButton으로 교체됐다. 화살표만 §2-o-1 Maze_DirArrow로 새로 정의한다(위 §2-j-5 Maze_DirButton 비고도 함께 고쳤다).

**T4.1 완료 요약**: MazeIcon_5종은 `MapHud.cs`에 Sprite 필드(startIcon/combatIcon/eliteIcon/restIcon/bossIcon)와 Cell_x_y/Icon 렌더링 코드를 추가해 코드 후속을 마쳤다. Maze_DirArrow는 Battle.unity의 DirButton_Up/Down/Left/Right 아래 Arrow 자식을 만들고 Z회전(0/180/90/-90)까지 씬에 반영했다(자리표시 스프라이트). Icon_Crystal 실제 스프라이트 생성·CrystalIcon.sprite 교체만 에셋 생성 도구의 대화별 동의 절차 때문에 이번 단계에서 못 하고 다음 단계로 넘긴다.

### 2-p. 미니게임 페이즈 HUD 리소스 (T5)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| HUD_HintBanner | 미니게임 페이즈 HUD | `MiniGamePhaseHud.hintBanner`(startHintText 자식, T5.1에서 씬 생성·연결 완료, 스프라이트는 자리표시 UI/Skin/UISprite.psd, 실제 그림 생성은 이월) | 와리오식 짧은 지시문 뒤 배너 배경 | 원본 1024x192, 표시 960x180 | Assets/SampleImage/UI/MiniGameHud/ | `flat UI ribbon banner frame, wide torn paper ribbon shape with rough ink edges, cream paper texture fill, front view, orthographic UI element, uniform border thickness, plain stretchable center, empty interior for hint text, no text, no letters`(§1-1 접두+네거티브 공통 적용) | 9-slice 64,64,32,32. hintBanner는 표시/숨김을 startHintText.SetActive에 위임(코드 변경 없음) |
| HUD_FuseSpark | 미니게임 페이즈 HUD | `MiniGamePhaseHud.timeBarFuseIcon`(T5.1에서 씬 생성·연결 완료, 스프라이트는 자리표시, 실제 그림 생성은 이월) | 도화선 타임바 끝의 불꽃(fillAmount 비율 위치 추종) | 원본 128x128, 표시 64x64 | Assets/SampleImage/UI/MiniGameHud/ | `hand-drawn ink line art cartoon spark flame, small sizzling fuse-tip fire burst, flat single-tone color fill, transparent background, isolated subject, front view, no text, no letters`(§1-1 접두+네거티브 공통 적용) | Simple(9-slice 0). 남은 시간이 lowTimeThreshold 이하면 코드에서 1~1.2배 확대(Update L67-95 근처) |
| HUD_FuseBomb | 미니게임 페이즈 HUD | phaseTimeBar 형제(T5.1에서 씬 생성 완료, 필드 없음, 스프라이트는 자리표시, 실제 그림 생성은 이월) | 도화선 오른쪽 끝 고정 폭탄 | 원본 128x128, 표시 72x72 | Assets/SampleImage/UI/MiniGameHud/ | `hand-drawn ink line art round cartoon bomb, simple fuse stub on top, flat single-tone color fill, transparent background, isolated subject, front view, no text, no letters`(§1-1 접두+네거티브 공통 적용) | Simple(9-slice 0). 위치 고정(움직이지 않음)이라 코드 불필요 |
| HUD_Stamp_Success | 미니게임 페이즈 HUD | `MiniGamePhaseHud.successStamp`(feedbackText 형제, T5.1에서 씬 생성·연결 완료, 스프라이트는 자리표시, 실제 그림 생성은 이월) | 성공 도장(글자 없는 형태) | 원본 512x512, 표시 360x360 | Assets/SampleImage/UI/MiniGameHud/ | `hand-drawn ink line art circular rubber stamp shape, thick round ink border with a bold checkmark symbol inside, slightly uneven hand-stamped edge, flat single-tone color fill, transparent background, isolated subject, front view, no text, no letters`(§1-1 접두+네거티브 공통 적용) | Simple(9-slice 0). ShowFeedback 코루틴에서 feedbackText와 같은 scale/position 적용(Animator 미사용) |
| HUD_Stamp_Fail | 미니게임 페이즈 HUD | `MiniGamePhaseHud.failStamp`(feedbackText 형제, T5.1에서 씬 생성·연결 완료, 스프라이트는 자리표시, 실제 그림 생성은 이월) | 실패 도장(글자 없는 형태) | 원본 512x512, 표시 360x360 | Assets/SampleImage/UI/MiniGameHud/ | `hand-drawn ink line art angular jagged rubber stamp shape, thick uneven ink border with a bold X symbol inside, smudged ink bleed at the edges, flat single-tone color fill, transparent background, isolated subject, front view, no text, no letters`(§1-1 접두+네거티브 공통 적용) | Simple(9-slice 0). ShowFeedback 코루틴 공유(Animator 미사용) |
| HUD_EnergyPopupBg | 미니게임 페이즈 HUD | `MiniGamePhaseHud.energyPopupText` 뒤(선택, T5.1에서 energyPopupText 자체는 생성·연결 완료, 배경 이미지는 미적용) | 에너지 획득 '+N' 팝업 배경(선택) | 원본 128x64, 표시 128x64 | Assets/SampleImage/UI/MiniGameHud/ | `flat UI small rounded pill badge frame, thin ink border, cream paper texture fill, front view, orthographic UI element, uniform border thickness, plain stretchable center, empty interior, no text, no letters`(§1-1 접두+네거티브 공통 적용) | Simple(9-slice 0). 없어도 글자만으로 동작(선택 적용) |
| HUD_ResultPanel | 미니게임 페이즈 HUD | `MiniGamePhaseHud.resultPanel`.Image.sprite(기존 오브젝트, 필드 추가 없음) | 미니게임 단계 result 패널 배경 | 원본 1200x700, 표시 1200x700 | Assets/SampleImage/UI/MiniGameHud/ | `flat UI panel frame, wide rounded rectangle clipboard shape with a small clip detail at the top edge, cream paper texture fill, front view, orthographic UI panel, uniform border thickness, plain stretchable center, empty interior, no text, no letters`(§1-1 접두+네거티브 공통 적용) | 9-slice 64,64,64,64. UI_CardFrame·UI_PanelDark와 별도 전용 프레임 |

**제외·차이 메모**: 이 표의 도장(HUD_Stamp_Success/Fail)은 Animator 파라미터(isSuccess/isFailed)를 쓰지 않고 기존 ShowFeedback 코루틴의 크기·흔들림 애니메이션을 그대로 공유한다(CLAUDE.md 개별 미니게임 결과 연출과는 다른 방식, HUD는 원래 코루틴 기반). 미니게임 단계 result(§12)는 UI_DESIGN §12 표의 '보상 목록(VerticalLayoutGroup)'이 없고 resultText 한 개 문단으로 대체돼 있다(위 UI_DESIGN §12 T5 추가 참고).

### 2-q. 전투 월드 표시 리소스 (T10, UI_DESIGN §20)

| 에셋ID | 카테고리 | 원본 | 용도 | 권장 해상도 | 배치 경로 | 프롬프트 | 비고 |
|---|---|---|---|---|---|---|---|
| WorldLabelPlate | 월드 라벨 판 | `BattleUnitView.labelPlate`(신규) | 유닛 이름·HP·상태 라벨 배경(9-slice) | 512x208, 9-slice 여백 48 | Assets/SampleImage/UI/ | `flat UI nameplate panel, rounded rectangle wood or paper tag shape, thin ink border, cream fill, front view, orthographic UI element, uniform border thickness, plain stretchable center, empty interior, no text, no letters`(§1-1 접두+네거티브 공통 적용) | UI_DESIGN §20.1 라벨 판. 크기 2.2u×0.9u(head+0.35) |
| IntentBadgeBase | 예고 받침 | `BattleUnitView.intentBase`(신규) | 예고 아이콘 뒤 받침 배지 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat UI small round badge base plate, thin ink border, cream fill, front view, orthographic UI element, no text, no letters`(§1-1 접두+네거티브 공통 적용) | UI_DESIGN §20.1. 0.9u×0.9u(head+1.05) |
| WorldSelectionRing | 선택 마커 | `BattleUnitView.selectionSprite`(신규) | 발밑 선택 표시(타원 링) | 512x160 | Assets/SampleImage/UI/ | `flat icon, simple oval ring outline viewed from above, thin ink line, glowing accent-blue tone, transparent background, isolated subject, no text, no letters`(§1-1 접두+네거티브 공통 적용) | UI_DESIGN §20.1. 너비 body.size.x×1.3, 높이 0.35u, 발밑(-bodyHeight-0.1) |
| Intent_Magic | 예고 아이콘 | `BattleUnitView.intentSprites[Magic]`(신규) | 다음 행동 예고: 마법 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small purple sparkling star burst inside a circular badge frame, front view`(§1-1 접두+네거티브 공통 적용) | §2-i Intent_Attack/Guard/Special과 모티프 공유(원형 배지, 색으로 구분) |
| Intent_Buff | 예고 아이콘 | `BattleUnitView.intentSprites[Buff]`(신규) | 다음 행동 예고: 버프 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small green upward arrow with a tiny sparkle inside a circular badge frame, front view`(§1-1 접두+네거티브 공통 적용) | 위와 동일 |
| Intent_Heal | 예고 아이콘 | `BattleUnitView.intentSprites[Heal]`(신규) | 다음 행동 예고: 회복 | 256x256 | Assets/SampleImage/UI/Icons/ | `flat icon, small plus-shaped cross with a soft heart outline inside a circular badge frame, front view`(§1-1 접두+네거티브 공통 적용) | 위와 동일. 총 6종(§2-i 3종+본 3종)으로 EnemyIntentKind 전부 커버 |
| FX_Hit_Spark | 피격 이펙트 | `BattleUnitView.hitFxPrefab`(신규) | 피격 시 body 중심 스파크(4프레임 시트) | 1024x256(256x256×4프레임) | Assets/SampleImage/Battle/FX/ | `hand-drawn ink line art cartoon impact spark burst, 4-frame sprite sheet left to right showing spark expanding then fading, flat single-tone color fill, transparent background, isolated subject, front view, no text, no letters`(§1-1 접두+네거티브 공통 적용) | UI_DESIGN §20.1. 1.2u, body 중심, 0.25s. body 흰색 플래시 0.1s×2회는 코드(flashColor) |
| FX_Cleanse_Sparkle | 정화 이펙트 | `BattleUnitView.cleanseFxPrefab`(신규) | 상태이상 해제 시 위로 떠오르는 반짝임(4프레임) | 1024x256(256x256×4프레임) | Assets/SampleImage/Battle/FX/ | `hand-drawn ink line art cartoon sparkle particles, 4-frame sprite sheet left to right showing green sparkles rising and fading, flat single-tone color fill, transparent background, isolated subject, front view, no text, no letters`(§1-1 접두+네거티브 공통 적용) | UI_DESIGN §20.1. 1.4u, body 중심에서 위로 0.3u, 0.4s, success.green 계열 |

**검증 메모(캠페인T10)**: 8행(WorldLabelPlate/IntentBadgeBase/WorldSelectionRing/Intent_Magic/Intent_Buff/Intent_Heal/FX_Hit_Spark/FX_Cleanse_Sparkle) 신규 추가. §2-i 기존 3종(Intent_Attack/Guard/Special)과 합쳐 EnemyIntentKind 6종(Attack/Special/Guard/Magic/Buff/Heal) 전부 아이콘 ID가 있다. DamagePopup은 텍스트 스타일(폰트·외곽선·크기 3단계)만 UI_DESIGN §20.1/20.3에 정의하고 별도 스프라이트 에셋은 만들지 않는다(TextMesh 유지).

**검증 메모(캠페인T10.1 마무리)**: `Unity_AssetGeneration_GetModels` 재호출 → `Models:[]` 유지(T8.2~T9.2와 동일한 차단 지속, 사람이 Unity AI Assistant 패널에서 생성 모델을 등록해야 §2-q 8행 실제 이미지 생성 가능). 문서 설계(§2-q 8행, UI_DESIGN §20 요소표 10행·겹침 규칙·코드 필드 목록)는 T10에서 이미 완료 기준을 충족했으므로 이번 마무리 단계는 재확인만 하고 추가 변경은 하지 않는다. BattleUnitView.cs/DamagePopup.cs 필드 반영(§20.3 목록)은 여전히 후속 구현 작업으로 남긴다(이번에도 의도적으로 미착수).

**검증 메모(캠페인T10.2 마무리)**: `Unity_AssetGeneration_GetModels` 재호출 → `Models:[]` 유지(T8.2~T10.1과 동일, 변화 없음). §2-q 8행(WorldLabelPlate/IntentBadgeBase/WorldSelectionRing/Intent_Magic/Intent_Buff/Intent_Heal/FX_Hit_Spark/FX_Cleanse_Sparkle) 재검토 결과 ID 중복 없고 UI_DESIGN §20.1 요소표와 1:1 대응됨을 재확인. T10 설계와 T10.1 마무리에서 이미 완료 기준(크기·오프셋·프롬프트 ID·코드 필드 명시)을 채웠으므로 문서 추가 변경은 없다. 남은 두 항목(이미지 실제 생성, BattleUnitView.cs/DamagePopup.cs 필드 반영)은 사람의 모델 등록과 후속 구현 작업으로 계속 이월한다.

**검증 메모(캠페인T11 최종 일관성 검수)**: §0-2 체크리스트에 §2-o/§2-o-1/§2-p/§2-q 4개 절과 남은 생성 수(약 88행)를 새로 반영했다(이전까지 §2-m까지만 체크리스트에 있었음, 실제 프롬프트는 이미 작성돼 있었으나 체크리스트 누락이었음). 전수 확인 결과:
  - **에셋ID 중복**: §2-o(9종)·§2-o-1(7종)·§2-p(7종)·§2-q(8종) 총 31개 ID를 Grep `^\| <ID> \|` 패턴으로 검색 → 각 ID가 정확히 1개 행에서만 등장(중복 0건).
  - **오탈자**: 위 31개 ID를 UI_DESIGN.md §20 요소표·§13 표의 참조와 대조해 철자 불일치 없음(WorldLabelPlate/IntentBadgeBase/WorldSelectionRing/Intent_Magic·Buff·Heal/FX_Hit_Spark/FX_Cleanse_Sparkle이 §20.1과 1:1, UI_TopBar~UI_ButtonDisabled 9종이 §13과 1:1).
  - **기존 png와 이름 충돌**: `Assets/SampleImage/UI/**/*.png`(49개), `Assets/SampleImage/Battle/**/*.png`(65개) 전수 Glob 결과 위 31개 신규 ID와 동일한 파일명 0건(모두 아직 생성되지 않은 상태라 예상대로 충돌 없음).
  - §2-l/§2-m(구현·예정 미니게임)·§2-a~§2-k(캐릭터·몬스터·아이콘·배경) ID 중복은 T9.1/T9.2에서 이미 전수 확인 완료(재확인 결과 변동 없음, 이번 단계는 §2-o~§2-q 신규분에 집중).
  - Unity 콘솔: `Unity_GetConsoleLogs`(logTypes=Error) → errorCount 0(T11 재확인).

**검증 메모(캠페인T11.1 마무리)**: T11의 완료 기준(§0-2 체크리스트 갱신, 신규 31개 ID 전수 검사, 콘솔 Error 0)이 이미 충족돼 있어 이번 단계는 재확인만 수행하고 문서 본문 추가 변경은 없다. `Unity_AssetGeneration_GetModels` 재호출 → `Models:[]` 유지(변화 없음). `Unity_GetConsoleLogs`(Error) 재호출 → errorCount 0(변화 없음). 새로 발견된 ID 중복·오탈자·png 충돌은 없다. 남은 항목(사람의 생성 모델 등록, BattleUnitView.cs/DamagePopup.cs §20.3 필드 반영, 예정 미니게임 13종 씬·프리팹 부재)은 UI_DESIGN §21.4에 이월 기록.

**검증 메모(캠페인T11.2 마무리)**: T11.1까지 채운 완료 기준(§0-2 체크리스트 갱신, ID 중복·오탈자·png 충돌 0건, 콘솔 Error 0)이 그대로 유지됨을 재확인했다. `Unity_AssetGeneration_GetModels` 재호출 → `Models:[]` 유지(변화 없음). `Unity_GetConsoleLogs`(Error) 재호출 → errorCount 0(변화 없음). 새로 발견된 ID 중복·오탈자·png 충돌은 없다. 남은 항목(사람의 생성 모델 등록, BattleUnitView.cs/DamagePopup.cs §20.3 필드 반영, 예정 미니게임 13종 씬·프리팹 부재)은 UI_DESIGN §21.5에 이월 기록. 이번 캠페인(T1~T11.2)의 문서화 범위(스펙·프롬프트·검수)는 완료 기준을 모두 채웠다.

### 2-r. 코믹 UI 리뉴얼 (2026-09-30, 사용자 결정: 웹툰 코믹 + 캐릭터 그림체)

§2-o~§2-q 를 스크립트로 그린 1차본이 "PPT 같다"는 피드백을 받아 GPT Image 로 다시 뽑는다. 방향: **웹툰 코믹**(굵고 살짝 떨리는 먹선, 흰 종이+망점 음영, 말풍선, 집중선, 기울어진 캡션 박스) + **캐릭터 그림체(Hero_Stand)와 같은 얇고 깔끔한 선·평면 채색**. 오피스 소품(결재 도장·클립·포스트잇)은 포인트로만.

**진행 결과(2026-09-30)**: 컨셉 확정 → 키트 시트 4장(A 프레임·B HUD·C 아이콘·D 월드) 생성·절단·적용 완료, 총 100크레딧. 상세 UI_DESIGN §23.

진행 순서: ① 컨셉 목업 1장(현재 전투 캡처를 편집 원본으로, 레이아웃 유지) → 사용자 확인 → ② UI 키트 시트 2~3장(①을 참고 이미지로) → 잘라서 9-slice → 기존 §2-o~§2-q 파일 **교체가 아니라 `_Comic` 접미사로 새 파일**(덮어쓰기 금지) → 씬 스프라이트만 바꿔 끼움.

| 에셋ID | 방식 | 편집 원본 / 참고 | 프롬프트 |
|---|---|---|---|
| UIConcept_Battle | GPT Image 2.5 편집 | 원본: 현재 Battle Play 캡처(UI 포함), 참고(+): Hero_Stand | `Redesign only the game UI overlay of this screenshot into a Korean webtoon comic style game UI. Keep the characters, monsters and office background exactly as they are. Match the thin clean black ink line art and flat colors of the characters. UI: hand-inked comic panel frames with slightly uneven bold black borders and a small hard offset shadow, white paper fill with light halftone dot shading; command buttons as tilted comic caption boxes with bold hand-lettered Korean labels 공격, 스킬, 방어, 턴 종료; HP bar as an inked tube with flat red fill and tiny speed lines; energy as an inked battery with cyan fill; enemy intent shown in small jagged shout-burst bubbles; battle log in a small caption box; small office accents like a red approval stamp and a paper clip. Clean readable layout, 16:9 game screen, flat colors, no gradients, no glossy 3D, no photorealism.` |
| UIKit_Comic_A | GPT Image 2.5 편집 | 원본: UIConcept_Battle(확정본) | `Game UI asset sheet in exactly the same comic webtoon ink style as the reference image, separate isolated elements on a plain flat mid-gray background (#808080) with wide spacing, no text, no letters: 1 wide tilted caption-box button normal, 2 the same button pressed (shifted down, shadow gone), 3 the same button disabled (gray fill), 4 large rectangular panel frame with bold ink border and hard offset shadow, 5 dialogue speech-bubble box with a small tail at bottom-left, 6 small name caption tag, 7 round close button with a bold X, 8 empty HP tube frame, 9 jagged shout-burst badge, 10 tall card frame. Front view, flat colors, thick slightly uneven black ink outlines, halftone dot shading only inside, no gradients.` |
| UIKit_Comic_B | GPT Image 2.5 편집 | 원본: UIConcept_Battle(확정본) | `Game UI icon and effect sheet in exactly the same comic webtoon ink style as the reference, separate isolated elements on a plain flat mid-gray background (#808080), no text: 1 red approval stamp success mark (circle with check), 2 red stamp fail mark (jagged with X), 3 comic impact burst (POW shape, empty), 4 speed-line starburst ring, 5 small inked downward triangle, 6 inked arrow, 7 small round badge base, 8 horizontal ribbon banner for a hint message, 9 inked battery gauge frame vertical, 10 small pill tag. Flat colors, bold black ink outlines, no gradients.` |

### 2-s. 대화 표정·피격·스킬 모션 (2026-09-30, 생성 완료)

GPT Image 2.5(Tripo), 참고 이미지는 Tripo 기록의 기존 시트(화자 2x2, 스토리 요원, Hero/Cook/Fashion 공격·방어 시트). 원본: `Docs/Concepts/ExprMotion_Sheets.png`. 총 120크레딧.

| 파일 | 내용 |
|---|---|
| `Battle/Characters/{Hero,Cook,Fashion}_Hit.png` | 피격(뒤로 휘청). PPU·발 pivot = `_Attack_Anim` |
| `Battle/Characters/{Hero,Cook,Fashion}_Skill_Anim.png` | 스킬 3프레임(Multiple 768). Hero 크리스탈 펀치, Cook 불꽃 프라이팬, Fashion 스카프 채찍. 이펙트 보존 절단(rembg 마스크 ∪ 비흰색 픽셀) |
| `Dialogue/Speakers/Coworker_Portrait_{Panic,Determined,Relieved,Tired}.png` | 회사원 표정 4 |
| `Dialogue/Speakers/Devil_Portrait_{Angry,Laugh,Shocked,Annoyed}.png` | 불편의 악마 표정 4 |
| `Hub/Agents/Agent_Story_Portrait_{Joy,Surprised,Cheer,Embarrassed}.png` | 스토리 요원 표정 4 |

연결(표정 전환 구조, CharacterDefinition 피격·스킬 필드, BattleUnitView 스킬 재생)은 릴레이 캠페인(독립 스쿼드 캠페인 c20260930-231008-dd4e 다음, 같은 세션)으로 넘겼다.

### 2-t. 미니게임 배경·placeholder 교체·결과 연출 (2026-10-02, 생성·적용 완료)

GPT Image 2.5(Tripo), 화풍 참고는 기록의 표정 시트. 배경 14장(16:9, 그중 위에서 본 Egg·Pizza·USB 와 어두운 VacuumWiden 은 미사용) + 시트 5장(1:1, 2x2·3x3) = 약 400크레딧(1장 20). 시트는 흰 배경을 가장자리부터 지우고 칸별로 잘랐다(`mg_cut.py`). 파일 목록·씬 적용·크기 규칙은 `Docs/MINIGAME_RESOURCES.md` §14.

## §3. 제외 목록 (아트 프롬프트 대상 아님)

| 이름 | 종류 | 제외 이유 |
|---|---|---|
| Template.asset | MiniGameDefinition | 개발용 템플릿, 실제 미니게임 아님 |
| DummyTap.asset | MiniGameDefinition | 개발/테스트용 더미 |
| TemplateMiniGame.unity | 씬 | 위와 동일(템플릿 씬) |
| Boot.unity | 씬 | 부팅 로더 씬, 시각 에셋 없음 |
| SampleScene.unity | 씬 | Unity 기본 생성 샘플 씬, 게임과 무관 |
| Dev/DialogueSample.unity | 씬 | 대화 시스템 개발용 샘플 씬 |
| Debug/InputDebug.unity | 씬 | 입력 디버그용 씬 |
| DLG_Sample.asset 등 DLG_* 전체(Dialogue) | 대화 데이터 | 텍스트 데이터, 시각 에셋 아님(초상화는 §2-c에서 다룸) |
| Wave_Grunt1, Wave_Grunt2, Wave_MidBoss, Room_Grunt, Room_GruntGruntBoss (Battle/Rooms), Wave_S1_Mosquito2, Room_Stage1Boss, Room_S1_Trash, Room_S1_EliteSwarm, Wave_S1_TrashGrunt, Wave_S1_Swarm, Wave_S1_MosqGrunt, Wave_Stage1Boss, Wave_S1_Trash2, Room_S1_Mosquito (Battle/Stage1) | 전투 구성 데이터 | 몬스터 배치·순서 정의일 뿐 시각 에셋을 담지 않음(몬스터 자체는 §2-b) |
| Affinity_Story.asset | 호감도 데이터 | 수치 데이터, 시각 에셋 아님 |

## §4. 게임 디자인 기준 요구 슬롯

근거: Docs/MINIGAME_IDEAS.md §1(L8-22, 기준6 실패 과장)·§6(L149-154, 배경1+대상1~3+실패연출1+지시문),
Docs/UI_DESIGN.md §11(L339-357), Docs/MINIGAME_GUIDE.md L28-30,
Assets/Scripts/MiniGames/MiniGameRunner.cs L40·L50·L59-67·L84-112·L126-149, MiniGameBase.cs L29-30.

### 4-1. 미니게임당 슬롯 템플릿

| 슬롯 | 개수 | 담당(씬/호스트) | 렌더 방식 | 디자인 기준 | 파일명 규칙 |
|---|---|---|---|---|---|
| BG | 1 | 미니게임 씬 | SpriteRenderer | 불투명 풀프레임(투명 영역 없음) | `BG_<Name>` |
| Target | 1~3 | 미니게임 씬 | SpriteRenderer + Collider2D(필수, Raycast 대상) | 한 동사를 한눈에 읽히는 실루엣 | `Obj_<Name>_<부위>` |
| Goal(선택) | 0~1 | 미니게임 씬 | 트리거 Collider2D, 태그 `ClearObject` | 통과 판정이 필요한 게임만 | `Obj_<Name>_Goal` |
| Success | 공유 | 미니게임 씬 | Animator bool `isSuccess` | 0.6초 안에 읽힘(근거: MiniGameRunner resultHoldSeconds) | `FX_<Name>_Success` (게임별 실제 프롬프트는 T7·T8에서 작성) |
| Fail | 공유 | 미니게임 씬 | Animator bool `isFailed` | 과장·웃김 필수(기준6) | `FX_<Name>_Fail` (게임별 실제 프롬프트는 T7·T8에서 작성) |
| Instruction | 1 | 호스트(Battle Canvas) | 텍스트만, startHintText/instructionText | `MiniGameDefinition.instruction`의 '~해!' 형식, 0.7초 노출 | (텍스트 데이터, 이미지 아님) |

### 4-2. 실패 연출 기준

- 과장 방식은 찌그러짐·튀김·표정 붕괴 중 하나 이상을 쓴다.
- 0.6초 안에 끝나는 한 동작으로 만든다(길게 끌지 않는다).
- 핵심 실루엣은 안전영역(17.78×10 월드단위, MINIGAME_GUIDE.md L28-30) 안에 들어와야 한다.
- 게임별 실제 수준(강도·타이밍)은 T7·T8 재점검에서 하나씩 맞춘다.

### 4-3. UI 리소스 분류

(a) 호스트 공용 HUD (Battle Canvas 소속, MiniGamePhaseHud.cs 필드명 기준): `phaseTimeBar`(상단 24px), `energyGauge`(52x372), `startHintText`, `feedbackText`, `comboText`, `resultPanel`. 디자인·프롬프트 담당은 T3·T5.
(b) 미니게임 씬 전용: 월드 스프라이트(BG/Target/Fail FX)만 두고 Canvas는 두지 않는다(MINIGAME_GUIDE.md L28-30, 월드공간 확인).
(c) 배지·예고 아이콘: §2-n(배지)·§2-i(예고 아이콘) 참조, 별도 분류 없음.
(d) 전투 월드 표시(몬스터 위 경고 아이콘 등)는 T10에서 다룬다.

### 4-4. 배경 정책안

사실: MiniGameRunner.cs L126-149(DisableHostedComponents)에서 미니게임 씬의 Camera·AudioListener를 끄고 EventSystem을 파괴한다. 미니게임은 Additive로 로드되며(L40), 자체 카메라가 꺼지므로 Battle 카메라가 같은 월드 좌표에서 Battle 배경과 미니게임 오브젝트를 함께 그린다. Battle 배경을 숨기는 코드는 없다.

정책:
1. BG는 투명 영역 없이 1920x1080(16:9)으로 만들고, 화면비 여유분까지 가장자리를 늘려 그려 Battle 월드를 가린다.
2. sortingOrder는 Battle 월드보다 위여야 하며, 계층은 BG < Target < Fail FX 순서로 둔다. 실측값은 아래 4-4-1 참조 — Object 레이어 게임은 안전, Default 레이어 게임 다수는 **겹침 위험**(4-4-1 판정 참고).
3. 미니게임 Camera에 붙은 CameraFit(17.78×10)은 미니게임을 단독 실행할 때만 동작한다. 호스트(Battle) 모드에서는 Battle 카메라의 크기·투영이 그대로 적용된다. Battle 카메라 orthographicSize=5(세로 10 유닛), 안전영역 17.78×10과 세로는 일치, 가로는 카메라 화면비에 따라 자동 확장(별도 확인 불요).
4. 대안: Battle 월드 SpriteRenderer를 코드로 껐다 켜는 방식은 보류로 기록한다(T2 범위 밖, 코드 변경 필요).

### 4-4-1. 실측(T2.1)

Unity_RunCommand로 Battle.unity와 미니게임 11개 씬을 OpenSceneMode.Additive로 열어 SpriteRenderer.sortingLayerName/sortingOrder와 루트 위치를 조회했다(저장 없이 CloseScene). 사용 중인 Sorting Layer는 `Default(기본)`, `Object`(Object가 Default보다 나중 = 위에 그려짐) 둘뿐이다.

Battle 월드: SpriteRenderer 11개, 레이어 `Default`, Order 범위 [1, 5] (가장 큰 스프라이트 MonsterView0 order=2). Main Camera: ortho size=5, pos=(0,0,-10).

| 씬 | 루트 위치(특이점) | SpriteRenderer Order 범위 | 레이어 | BG로 보이는 스프라이트 Order | 판정 |
|---|---|---|---|---|---|
| EGGMiniGame | (0,0,0) | [0,1] | Object | Fan order=0 | 안전 — Object 레이어가 Default보다 항상 위 |
| PizzaMiniGame | (0,0,0) | [0,3] | Object+Default→T7.1에서 Default 1개 Object로 이전 | PizzaPlate order=0(이전 완료) | T7.1 조치 완료 — Default 잔존분 없음(실측 1개) |
| SamgakKimbapMiniGame | (0,0,0) | [0,5] | Default→T7.1에서 9개 Object로 이전 | Wrapper order=0(이전 완료) | T7.1 조치 완료 |
| USBMiniGame | **(-3.37, 2.04, 0) — T7.1에서 274.13→0 수정, x/y 유지** | [0,3] | Object | NoteBook order=1 | 안전(레이어). Z 이상치는 T7.1에서 0으로 수정·저장 완료(UsbMover/UsbSetter가 Vector2 대입이라 Z 미의존 확인) |
| PeelStickerMiniGame | (0,0,0) | [0,5] | Default→T7.1에서 12개 Object로 이전 | (뚜렷한 BG 없음, Arrow order=5 최대) | T7.1 조치 완료 |
| ScreenBubbleMiniGame | (0,0,0) | [0,4] | Default | Screen order=1 | 경계 위험 — Battle 최소 order(1)와 동률, 그리기 순서(씬 로드 순서)에 따라 뒤집힐 수 있음 |
| ButtonShirtMiniGame | (0,0,0) | [0,4] | Default | Body order=0 | 겹침 위험 |
| VacuumMiniGame | (0,0,0), 참조점 다수(Floor/Sofa 등) | [0,0] | Default | Body order=0 | 겹침 위험 — 전 스프라이트가 order 0, Battle order[1,5] 전부 위에 그려짐 |
| FlySwatMiniGame | (0,0,0) | [-10,5] | Default | Background order=**-10** | 겹침 위험 큼 — BG가 Battle 전체(order 1~5)보다 훨씬 아래 |
| BossBlobMiniGame | (0,0,0) | [0,0] | Default | (뚜렷한 BG 없음, 전부 order 0) | 겹침 위험 |
| TapTargetMiniGame | (0,0,0) | [5,5] | Default | (뚜렷한 BG 없음, 단일 스프라이트 order 5) | 경계 — Battle 최대(5)와 동률 |

판정 요약: Object 레이어를 쓰는 EGG·USB는 안전. **T7.1에서 Pizza·SamgakKimbap·PeelSticker 3개의 Default 레이어 SpriteRenderer/SortingGroup을 Object로 이전 완료(sortingOrder 유지, 각 씬 저장)** — 이 3게임은 조치 완료. **T8.1 이전 완료(나머지 6개: ScreenBubble 11개·ButtonShirt 14개·Vacuum 9개·FlySwat 2개·BossBlob 4개·TapTarget 1개 SpriteRenderer/SortingGroup, 프리팹 3개: Fly·Target·Decoy 각 1개, 총 42개를 Default→Object로 이전, sortingOrder 유지, 재확인 결과 Default 레이어 0개)** — 9씬·프리팹 모두 Battle 월드와의 겹침 위험 해소.

### 4-5. 적용 대상

- 구현된 11개 미니게임: §2-l 참조.
- 예정 미니게임: §2-m 참조.
- 게임별 재점검(슬롯 채움·실패 연출 수준 맞추기)은 T7·T8·T9에서 진행한다.

## 검증 메모

Grep으로 Assets/Data 아래 .asset 파일 이름(확장자 제외)과 Assets/Scenes 아래 .unity 씬 이름이
이 문서(§2 표 또는 §3 제외 목록)에 모두 나오는지 대조함. 결과는 build 단계 output 참고.

### T1.1 마무리(스크립트 확인)

- FlySwatMiniGame.cs, TapTargetMiniGame.cs를 직접 읽어 `flyPrefab`/`targetPrefab`/`decoyPrefab`
  public 필드가 가리키는 실제 프리팹(Assets/Prefabs/FlySwatMiniGame/Fly.prefab,
  Assets/Prefabs/TapTargetMiniGame/Target.prefab·Decoy.prefab)을 찾아 §2-l에 반영함.
  세 프리팹 모두 현재 Unity 빌트인 placeholder 스프라이트(UISprite 계열) 사용 중.
- FlySwat_Swatter는 실제로 대응하는 오브젝트·프리팹이 코드에 없음을 확인(클릭 판정만 존재,
  별도 시각 오브젝트 없음) → 표에서 프롬프트 대상 아님으로 정리.
- 미로 UI(MapPanel) 재확인은 T5 범위로 유지(이번 단계에서는 손대지 않음, 완전한 해결 아님).

### T1.2 마무리(전수 대조)

- Glob으로 Assets/Data/**/*.asset(70개)과 Assets/Scenes/**/*.unity(18개) 전체 목록을 다시 뽑아
  §2 표 또는 §3 제외 목록에 모두 등장하는지 대조함. 누락 없음(스킬 4종, Stage1 몬스터 4종,
  Agent_* 3종, Upgrade/PU/Module 전 항목, DLG_*·Wave_*·Room_* 등 데이터 전용 항목 포함).
- §1(스타일/방향/네거티브), §2(a~m 13개 카테고리), §3(제외 목록) 문서 틀이 모두 채워져 있음을 확인.
- 미로 UI(MapPanel) 추가 시각요소 재확인은 설계상 T5 전용 범위이므로 T1.2에서는 의도적으로
  다루지 않음(미해결 이슈 아님, T5로 정상 이관).
- T1(.1/.2) 완료 기준 충족: 조사 대상 전수 커버 + 문서 틀 완성. 이후 T2부터 카테고리별 프롬프트
  빈칸을 채운다.

### T3.1 마무리(화자 전수 재대조)

- `Assets/Data/Dialogue/DLG_*.asset` 전체(11개 파일)의 `speakerName`을 grep으로 다시 뽑아 §2-c·§2-c-1과
  대조함. 실제 등장 화자 8종: 스토리 요원·강화 요원·모듈 요원(§2-c, Agent_* 데이터 있음) /
  불편맨(§2-a-1 Hero_Portrait_*로 커버, §2-c-1에 재언급하지 않음) / 회사원·불편의 악마·COMFORT 사령관·안내
  (§2-c-1, Agent_* 데이터 없이 DLG_*에만 등장) — 8종 모두 프롬프트 항목이 이미 존재함, 누락 없음.
- **불편의 악마↔Boss_Stage1 연결**: Assets/Data/Battle/Stage1/Boss_Stage1.asset을 다시 열어 확인했으나
  이름·설명 필드에 "악마" 관련 텍스트 없음 — 코드·데이터로는 근거를 더 찾을 수 없음(추정 이상으로 확정 불가).
  §2-c-1 비고에 이미 "세계관상 동일 존재 추정, 근거는 DLG_*뿐"로 명시돼 있어 그대로 유지, T4에서 몬스터
  프롬프트 작성 시 이름 일치 여부를 재확인하는 것으로 이관함(이번 단계에서 더 해결할 수 있는 범위 아님).
- **안내(Guide) 비인간 홀로그램 결정**: 디자인 결정 사유(회사원·요원과 캐릭터 구분)가 §2-c-1 비고에 이미
  기록돼 있음을 확인. 기획 의도 재확인은 검수 단계(T9)의 역할이므로 이번 단계에서 임의로 뒤집지 않고 유지함.
- 이미지 생성·Assets 배치·`AgentDefinition.portrait` 필드 연결은 여전히 이 작업(문서 프롬프트 작성) 범위 밖
  — 코드에 `portrait` 필드 자체가 없다면 별도 구현 작업 필요(스크립트 변경은 T3 계열 범위 아님).
- T3.1 완료 기준 충족: DLG_* 화자 전수 재대조로 누락 없음을 재확인함. 3개 이슈 중 2개는 후속 태스크(T4/T9)로
  정상 이관되는 것이 맞고(이번 단계에서 억지로 확정하면 오히려 추측을 사실처럼 기록하게 됨), 1개는 범위 밖
  구현 사항이라 문서만으로는 닫을 수 없음 — 모두 의도된 이관이며 미해결 결함이 아님.

### T3.2 마무리(portrait 필드 존재 확인)

- 코드 재확인 결과 `Assets/Scripts/Hub/AgentDefinition.cs`에 `public Sprite portrait;` 필드가 **이미 존재**함
  (T3.1에서 "필드 자체가 없으면 별도 구현 필요"로 남겼던 조건부 이슈가 실제로는 해당 없음으로 확정됨).
  같은 구조로 `Assets/Scripts/UI/DialogueBox.cs`의 `DialogueLine.portrait`(Sprite)도 존재하고
  `DialogueBox.cs`가 `line.portrait`를 `Image.sprite`에 그대로 대입해 표시함 — 코드 배선은 끝나 있음.
- 다만 `Agent_Story/Upgrade/Module.asset`과 `DLG_*.asset`의 `portrait` 값이 전부 같은 placeholder
  (`Assets/SampleImage/Dialogue/Portrait_Sample.png`, guid `1eaa1bb4f922ad54c88c9c56fa4b9edc`)로 채워져 있어
  실제로는 화자별로 구분되지 않음. 즉 "portrait 필드 연결"은 코드상 이미 끝났고, 남은 것은
  ① §2-c/§2-c-1 프롬프트로 실제 초상화 이미지를 생성하고 ② 그 스프라이트를 각 Agent_*.asset·DLG_*.asset의
  `portrait` 슬롯에 Placeholder 대신 채워 넣는 **에셋 배치 작업**뿐이다(이미지 생성·MCP 에디터 작업, 이 문서
  프롬프트 작성 범위 밖 — T9 검수 또는 별도 에셋 배치 단계로 이관).
- 앞서 이관한 나머지 2개 이슈(불편의 악마↔Boss_Stage1 이름 일치는 T4, 안내 홀로그램 기획 의도 최종 확인은
  T9)는 이번 단계에서 다룰 근거가 추가로 나오지 않아 그대로 유지함.
- T3.2 완료 기준 충족: T3.1이 남긴 3개 미해결 이슈 중 "코드에 portrait 필드가 있는가"는 이번 단계에서
  확인·해소(존재함, 조건부 이슈 해제). 나머지 2개는 성격상 이 단계(T3 계열, 문서 프롬프트 작성)에서 닫을 수
  없는 후속 태스크 의존 사항이라 의도된 이관으로 유지. §2-c/§2-c-1 표 내용 자체는 변경 없음(이미 완비).

### T4 마무리(몬스터·보스 프롬프트, §2-b)

- `Assets/Data/Battle/*.asset`(Grunt, MidBoss)와 `Stage1/*.asset`(S1_Mosquito, S1_TrashBag, Boss_Stage1) 5개
  전부를 Read로 열어 `displayName`(한글 유니코드 이스케이프 디코딩) 확인: Grunt="잡몹", MidBoss="중간보스",
  S1_Mosquito="모기", S1_TrashBag="쓰레기봉투", Boss_Stage1="불편대왕". §2-b 표에 5개 모두 존재함(완료 기준
  "모든 적 Combatant 에셋이 표에 있음" 충족, Glob으로 Assets/Data/Battle 전수 재확인해도 그 외 몬스터 없음).
- 각 몬스터마다 기본(_Stand)/피격(_Hit)/공격 예고(_Attack) 3종 + Boss_Stage1은 phase2Threshold=0.5 기반
  페이즈2 예고(_Phase2) 1종을 추가로 채움(§2-b-1~5). 액션 데이터(`actions[].kind`, `id`)를 근거로 기본공격
  (basic/heavy)과 특수공격(special_*)을 구분해 프롬프트 지문에 반영함.
- 머리 위 행동 예고 아이콘: 몬스터별로 새로 만들지 않고 기존 §2-i(Intent_Attack/Guard/Special,
  BattleUnitView.intentIcon 공용 슬롯 — 코드가 몬스터별 아이콘 필드를 두지 않음)를 재사용하도록 §2-b 서문에
  대응표(일반공격→Attack, 대기·충전→Guard, 특수공격 예고→Special)로 명시함. 아이콘 자체 프롬프트 중복 생성 안 함.
- **불편의 악마↔Boss_Stage1 이름 불일치 해결**: `displayName`이 실제로 "불편대왕"임을 이번에 코드 확인으로
  확정(T3.1/T3.2에서는 추정이었음). "불편의 악마"(DLG_Tutorial_Opening/Outro)와 정확한 문자열 일치는 아니지만,
  같은 세계관에서 "불편을 의인화한 존재"가 스테이지1에 둘일 근거가 없어 빌드 단계 결정으로 두 이름을 동일
  캐릭터로 통일(§2-b-5 비고에 결정 근거 기록, decisions_added에도 남김). 초상화(§2-c-1 Devil_Portrait)와
  전신 몬스터(§2-b-5)가 같은 고정 문구(정장·뿔·슬릭백 헤어)를 공유하도록 프롬프트를 이어붙임.
- T4 완료 기준 충족: Assets/Data/Battle의 적 Combatant 5종 모두 §2-b 표에 있고, 각 행(요약행 + 상세 하위표)
  프롬프트가 채워짐. 코드·씬 변경 없음, 문서만 변경.

### T4.1 마무리(T4 인계 이슈 재확인)

- T4가 남긴 이슈를 재검토: (1) 안내 홀로그램 디자인 T9 최종 확인, (2) portrait·몬스터 스프라이트 실제 배치,
  (3) Battle.unity MonsterView 플레이스홀더 교체 — 세 가지 모두 T4.1(문서 작업) 범위가 아니라 T9(검수·배치)
  또는 별도 Unity MCP 에디터 작업 범위임을 재확인. 문서(§2-b, §2-b-1~5, §2-i 대응표)만으로 해결 가능한
  이슈는 없어 §2-b 내용을 다시 고치지 않음(불필요한 재작성 방지).
- Assets/Data/Battle 및 Stage1 폴더를 Glob으로 재확인: Grunt.asset, Rooms/MidBoss.asset,
  Stage1/S1_Mosquito.asset, Stage1/S1_TrashBag.asset, Stage1/Boss_Stage1.asset 5종 외 몬스터 Combatant
  없음 — T4 완료 기준("모든 적 Combatant 에셋이 표에 있음")이 T4.1 시점에도 여전히 충족됨을 재확인.
- §2-b 각 행의 상세표(§2-b-1~5)에 기본/피격/공격(+Boss_Stage1 페이즈2) 프롬프트가 빠짐없이 채워져 있음을
  재확인(빈 칸 없음). 결론: T4.1 완료 기준은 "T4 결과가 여전히 유효함을 확인"으로 충족되며, 실제 이슈 해결은
  각각의 후속 단계(T9, 별도 배치)로 이관 유지.
- 코드·씬·§2-b 표 내용 변경 없음(재확인 전용 세션이므로 §2 표는 그대로 유지).

### T5.1 마무리(배경·맵)

- §2-j 표의 (T5) 플레이스홀더 5칸(BG_Title/Logo_Title/BG_HQ/BG_Battle/Maze_Tile)을 모두 실제 프롬프트로
  채우고, §2-j-1~5 상세 절을 신설함. Maze_Tile은 코드(`MapHud.cs`) 색상·글자 규칙을 지키기 위해 6종 하위
  스프라이트(Maze_Room/Maze_PassH/Maze_PassV/Maze_Current/Maze_Fog/Maze_DirButton)로 분리.
- §1-1 공통 접두는 `transparent background`/`no background scenery`/`isolated subject`를 포함해 풀스크린
  배경과 모순되므로, §2-j 전용 스타일 블록(§1-1과 같은 손그림 톤은 유지하되 배경 요소를 허용)을 새로 도입해
  BG_Title/BG_HQ/BG_Battle에 적용함. Logo_Title과 Maze_Tile 하위 스프라이트는 원래 투명 배경이 맞는 대상이라
  §1-1 접두를 그대로 유지함.
- Battle.unity MapPanel 씬 계층은 이번 세션 도구가 파일 Read/Edit로 한정돼 unity-mcp 조회를 하지 못함 —
  §2-j-5는 `MapHud.cs`(격자 셀 100px, passH/passV, CombatColor 등)와 `StageMaze.cs`/`MazeRoom.cs`(5x5 DFS
  생성, 방 타입 6종)의 코드 근거만으로 작성함. 비고에 '씬 미확인, MapHud.cs 기준'으로 명시함(미해결 이슈로 이관).
- docs_read MCP 색인 문제는 이번에도 재현되지 않고(이번 단계 도구에 Read/Edit가 포함됨), 파일 직접
  편집으로 완료함. 근본 원인(색인 미등록) 조사는 범위 밖으로 남김.
- T5.1 완료 기준 충족: §2-j 표에 (T5) 남은 칸 없음, §2-j-1~5 상세 절 모두 채움, 한글 로고 합성 여부는
  사용자 확인 항목으로 명시. 코드·씬·.meta 변경 없음, `Docs/ART_PROMPTS.md`만 변경.

### T5.2 마무리(배경·맵)

- 이전 미해결 이슈였던 "Battle.unity MapPanel 씬 계층 미확인"을 unity-mcp(`Unity_RunCommand`, 씬을
  `OpenSceneMode.Additive`로 열어 하이어라키를 덤프한 뒤 저장 없이 `CloseScene`)로 해결함. 실제 경로는
  `Assets/Scenes/Battle.unity`(표기상 `Battle.unity`는 정확, `MiniGames/` 하위가 아님 — 표의 전체
  경로 표기는 원래도 파일명만 썼으므로 정정 불필요).
- 확인 결과: `MapPanel`은 `Image`+`MapHud` 컴포넌트를 가지며 자식은 `Title`(Text), `MazeGrid`(빈
  RectTransform), `DirButton_Up/Down/Left/Right`(Image+Button+Label 자식)뿐이다. 방/통로/현재 위치
  타일에 대응하는 GameObject가 씬에 미리 배치돼 있지 않으므로, `MapHud.cs`가 `MazeGrid` 아래에 런타임
  생성한다는 §2-j-5의 기존 전제가 맞음을 확인함 — 6종 하위 스프라이트(Maze_Room/PassH/PassV/Current/
  Fog/DirButton) 설계를 그대로 유지, 내용 변경 없음.
- §2-j 표의 Maze_Tile 행 비고를 "씬 미확인" → "unity-mcp로 확인 완료"로 갱신함(위 표 참조).
- docs_read MCP 색인 문제는 이번 단계에서 재조사하지 않음(범위 밖, T5 시리즈에서 해결 불가로 재확인만
  하고 넘어감). Read/Edit 직접 편집으로 계속 우회.
- 씬 파일은 Additive 오픈 후 저장하지 않고 닫아 원본 변경 없음(콘솔 컴파일 오류 없음, 조회 명령만 실행).
- T5.2 완료 기준 충족: T5.1이 남긴 두 미해결 이슈 중 MapPanel 씬 확인은 완료, docs_read 색인 문제는
  범위 밖으로 재확인만 하고 이관. `Docs/ART_PROMPTS.md`만 변경, 코드·씬·.meta 변경 없음.

### T6 마무리(UI 아이콘)

- 대상 데이터 에셋 Glob 대조: 스킬(Assets/Data/Battle/{Attack,Guard,HeavyBlow,PurifyStrike}Skill.asset) 4개
  = §2-d 4행, 전투 강화(Assets/Data/Battle/Upgrades/*.asset) 7개 = §2-e 7행, 영구 강화
  (Assets/Data/PermanentUpgrades/*.asset) 5개 = §2-f 5행, 모듈(Assets/Data/Modules/*.asset) 3개
  = §2-g 3행 — 모두 일치 확인(Glob 실행 결과 그대로).
- 상태이상은 §2-h를 코드 enum 그대로 둔 이전 초안(7종)에서 목표 문서가 요구한 "5종(에너지 봉인·쇠약·
  초조·과부하·화상)"으로 정정함. 근거: `Docs/ROADMAP.md` D-7 항목이 "기절·받는 피해 증가는 상태이상
  목록에서 제외(에너지 봉인·쇠약·초조·과부하·화상으로 대체)"라고 명시. `StatusEffectType.cs` enum 자체가
  Stun·DamageTakenUp을 여전히 가진 것은 코드 이슈이며 이번 범위 밖(build 단계에서 코드는 건드리지 않음).
- 커맨드(공격/스킬/방어)는 §2-k UI_CommandButton 1행을 Command_Attack/Command_Skill/Command_Guard
  3행으로 분리해 채움. 에너지 게이지·칸은 UI_EnergyGauge 1행을 EnergyGauge_Bar(바 프레임)와
  EnergyGauge_Cell(정화 결정 1칸)로 분리해 채움.
- 미니게임 용도 3종(MiniGameUsage: Energy/Evasion/Special)·불편 유형 6종(MiniGameCategory: Food/Wear/
  Device/Home/Outside/Peel) 배지는 새 §2-n에 신설. 둘 다 데이터 에셋이 아닌 열거형이라 Glob 대조 대신
  코드(`MiniGameDefinition.cs`)의 열거형 값 개수(3, 6)와 행 수 일치로 확인.
- §2-d~2-i, 2-k(일부), 2-n 프롬프트는 §1-1 공통 접두(hand-drawn ink line art 등)가 자동 적용되므로
  아이콘 고유 모티프·배지 테두리 모양만 프롬프트 칸에 적었다(기존 §2-j 방식과 동일).
- 미해결 이슈로 이관: (1) `StatusEffectType.cs` enum이 D-7에서 제외하기로 한 Stun·DamageTakenUp을
  여전히 갖고 있어 코드와 기획 문서가 어긋남 — 코드 수정은 이번 범위 밖, 다음 코드 작업에서 정리 권장.
  (2) `Module_RepeatTurnPlus.asset`은 기존에도 지적됐듯 `ModuleKind` enum에 대응 값이 없는 데이터
  불일치가 남아 있음(§2-g 비고, 이번 단계에서 해결 안 함). (3) 아이콘 실제 이미지 생성(Tripo)과 Unity
  에디터 배선(Sprite 교체, 인스펙터 연결)은 이번 build 단계에서 하지 않음 — 사람 확인 목록으로 이관.
- T6 완료 기준 충족: "해당 데이터 에셋 수와 아이콘 행 수가 같음"을 스킬·강화·영구강화·모듈 4개 그룹에서
  Glob 대조로 확인, 열거형 기반 그룹(상태이상·용도·불편유형)은 코드 열거형 값 개수로 대조. `Docs/ART_PROMPTS.md`
  만 변경, 코드·씬·.meta 변경 없음.

### T6.1 마무리(재확인 및 범위 재확정)

- Glob 재대조: `Assets/Data/Battle/{Attack,Guard,HeavyBlow,PurifyStrike}Skill.asset` 4개가 실제로
  존재함을 재확인(§2-d 4행과 일치, T6 이후 데이터 변경 없음). 전투 강화 7개·영구 강화 5개·모듈 3개도
  T6에서 이미 대조됐고 그 사이 해당 폴더에 변경이 없어 재확인 불필요(파일 목록 유지).
- HANDOFF가 넘긴 3개 미해결 이슈를 재검토한 결과, 셋 다 이번 build 단계(문서·스크립트 범위)에서
  해결할 수 없는 항목으로 재확인해 범위 밖으로 유지한다:
  1) `StatusEffectType.cs` enum의 Stun·DamageTakenUp 잔존 — 코드 수정 필요(이번 요청은 "코드 수정은
     범위 밖"이라고 명시). §2-h 서문에 이미 이 사실과 근거(ROADMAP D-7)가 기록돼 있어 추가 조치 없음.
  2) `Module_RepeatTurnPlus.asset`의 `ModuleKind` enum 불일치 — 기존 데이터/코드 이슈이며 §2-g 비고에
     이미 명시돼 있어 추가 조치 없음.
  3) §2-k의 UI_StartButton/UI_AgentButton/UI_SortieButton/UI_SquadButton/UI_HpBar 5행은 HANDOFF가
     "이번 작업 범위(커맨드/스킬/에너지/상태이상/모듈/강화/배지) 밖"이라고 명시적으로 범위 밖에 두었으므로
     `(T6)` 플레이스홀더를 그대로 유지한다(다음 T6 후속 단계에서 채울 것).
- 실제 이미지 생성(Tripo)과 Unity 에디터 내 Sprite 배선도 HANDOFF에 명시된 대로 이번 build 단계에서
  하지 않는다(user_checks로 이관).
- T6.1 완료 기준 충족: T6가 채운 §2-d~2-i, 2-n의 데이터 에셋 수 = 아이콘 행 수 일치가 그대로 유지됨을
  재확인했고, 나머지 3개 항목은 이번 요청 자체가 범위 밖으로 규정한 항목이라 추가 변경 없이 그 사실을
  문서에 재확정만 했다. `Docs/ART_PROMPTS.md`만 변경, 코드·씬·.meta 변경 없음.

### T6.2 마무리(§2-k UI 5행 채움)

- HANDOFF가 "다음 T6 후속 단계 대상"이라 명시한 §2-k `UI_StartButton`/`UI_AgentButton`/`UI_SortieButton`/
  `UI_SquadButton`/`UI_HpBar` 5행의 `(T6)` 플레이스홀더를 실제 프롬프트로 채움. T6→T6.1→T6.2가 계획상
  UI 아이콘 프롬프트의 마지막 단계이고 다음은 T7(구현된 미니게임 에셋)로 넘어가므로 이번에 확정했다.
- 스타일은 기존 §2-k `UI_ResultPanel`(패널 프레임)·`EnergyGauge_Bar`(게이지 프레임) 프롬프트와 동일한
  "flat UI 프레임 + cream paper texture / 무채색 배경, 안쪽은 비워 코드가 텍스트·색을 입힘" 방식을 그대로
  따름 — §1-1 공통 접두·네거티브가 자동 적용되므로 버튼·게이지 고유 형태만 적었다.
  - `UI_AgentButton`: 이전 비고("§2-c Agent_*_Full과 중복 가능")를 "역할이 다름 — 버튼 프레임과 전신
    아트는 별도" 로 정리해 중복 우려를 해소함(전신 아트를 버튼 안에 얹거나 라벨만 쓰는 두 방식 모두 가능).
  - `UI_HpBar`: `EnergyGauge_Bar`와 같은 프레임 모티프를 재사용하되 무채색으로 생성하고, 실제 색(초록/
    빨강 등 HP 상태)은 기존처럼 `Image.color`가 입힘 — 새 색 규칙을 만들지 않음.
- 이미지 생성(Tripo 등)과 Unity 에디터 Sprite 배선은 이번에도 하지 않음(HANDOFF·CLAUDE.md 상 build
  단계 범위 밖, unity-mcp로 직접 확인하지 않음) — user_checks로 이관.
- 기존 두 미해결 이슈는 이번 요청도 "코드 수정은 범위 밖"이라 재확인만 하고 유지한다:
  1) `StatusEffectType.cs` enum의 Stun·DamageTakenUp 잔존(§2-h 서문에 근거 기록됨, 코드 작업 단계 대상).
  2) `Module_RepeatTurnPlus.asset`의 `ModuleKind` enum 불일치(§2-g 비고에 근거 기록됨, 데이터 수정 단계 대상).
- T6.2 완료 기준 충족: §2-k 전체 행에 `(T6)` 플레이스홀더가 더 이상 없음(Grep 재확인, 매치 0건이 될 예정).
  이로써 T6 시리즈(T6/T6.1/T6.2)의 UI 아이콘 프롬프트 작성이 끝나고 다음은 T7. `Docs/ART_PROMPTS.md`만
  변경, 코드·씬·.meta 변경 없음.

### T7 마무리(구현된 미니게임 에셋 프롬프트)

- §2-l 36개 행(정의 11 + 오브젝트 25, FlySwat_Swatter 제외행 포함)의 `(T7)` 플레이스홀더를 모두 실제
  프롬프트 또는 `—`(정의 행, §2-n 배지로 대체)로 채움. Grep 재확인 대상: `\(T7\)` 매치 0건.
- 완료 기준(Scenes/MiniGames의 Template을 뺀 모든 씬에 해당 절이 있음): Glob으로 확인한 11개 씬
  (Egg/Pizza/USB/SamgakKimbap/PeelSticker/ScreenBubble/ButtonShirt/Vacuum/FlySwat/BossBlob/TapTarget)이
  모두 §2-l 표에 행으로 존재함을 재확인.
- Docs/sketches의 스케치 파일(01_egg_shell/02_samgak_kimbap/03_peel_sticker/05_usb/06_screen_bubble/
  08_pizza_lift/art_pizza)을 해당 미니게임 행에 참고로 명시함.
- 기존 이미지가 있는 항목(Egg·Pizza·USB·SamgakKimbap)은 비고에 "교체 권장/보강 목적/선택" 등으로
  교체 여부를 적음. 기존 조각·단계 진행 이미지(egg1~4, piz1~9)는 유지 권장으로 명시.
- 여러 프레임이 필요한 항목은 프레임별로 나눠 씀: FlySwat_Fly(날개 위/아래 2프레임),
  USB_Hand(정상/실패 2종), PeelSticker_Sticker(온전/뜯긴 2종).
- FlySwat_Swatter 행은 이미 이전 단계에서 "스크립트 확인 완료: 파리채 전용 오브젝트·프리팹 없음,
  표에서 제외 대상"으로 채워져 있어 이번에 변경하지 않음(클릭 즉시 판정 방식이라 별도 파리채 스프라이트
  불필요, MINIGAME_IDEAS §6의 "파리채 스프라이트" 메모는 현재 구현과 맞지 않아 실제 구현 기준으로 표를
  유지함).
- 이미지 생성(Tripo)과 Unity 에디터 Sprite 배선은 이번에도 하지 않음(build 단계 범위 밖) — user_checks로
  이관.
- 기존 두 미해결 이슈(StatusEffectType.cs enum, Module_RepeatTurnPlus.asset)는 이번 요청 범위 밖이라
  재확인 없이 유지.

### T7.1 마무리(구현된 미니게임 에셋 프롬프트 재검증)

- HANDOFF가 남긴 3개 이슈를 재확인:
  1) `StatusEffectType.cs` enum의 Stun·DamageTakenUp 잔존 — 코드 수정은 범위 밖, 기존 §2-h 서문 근거
     기록 그대로 유지.
  2) `Module_RepeatTurnPlus.asset`의 `ModuleKind` 불일치 — 데이터 수정은 범위 밖, 기존 §2-g 비고 근거
     기록 그대로 유지.
  3) 이미지 생성(Tripo)·Unity Sprite 배선 미수행 — 이번 build 단계에서도 수행하지 않음(다음 단계 대상,
     user_checks로 재이관).
- §2-l 재검증: Glob `Assets/Scenes/MiniGames/*.unity` 12개 중 TemplateMiniGame.unity를 뺀 11개
  (Egg/Pizza/USB/SamgakKimbap/PeelSticker/ScreenBubble/ButtonShirt/Vacuum/FlySwat/BossBlob/TapTarget)가
  모두 §2-l 표에 행으로 존재함을 재확인. Grep `\(T7\)` 매치 0건(표 셀·서술문 모두)도 재확인해 T7에서
  채운 내용이 그대로 유지되고 있음을 확인.
- 이번 T7.1 완료 기준(HANDOFF의 "원래 완료 기준" = T7 완료 기준과 동일: 씬마다 해당 절 존재)이 이미
  T7에서 충족돼 있어, 추가로 표 내용을 바꾸지 않고 재검증 결과만 이 절에 기록함. `Docs/ART_PROMPTS.md`만
  변경, 코드·씬·.meta 변경 없음.

### T7.2 마무리(구현된 미니게임 에셋 프롬프트 최종 재검증)

- HANDOFF가 다시 나열한 3개 이슈를 재확인(내용 동일, T7·T7.1에서 이미 범위 밖으로 규정됨):
  1) `StatusEffectType.cs` enum Stun·DamageTakenUp 잔존 — 코드 수정 단계 대상, 이번에도 범위 밖.
  2) `Module_RepeatTurnPlus.asset`의 `ModuleKind` 불일치 — 데이터 수정 단계 대상, 이번에도 범위 밖.
  3) 이미지 생성(Tripo)·Unity Sprite 배선 미수행 — T8 이후 별도 단계 대상, 이번에도 수행하지 않음
     (user_checks로 재이관).
- §2-l 최종 재검증: Glob `Assets/Scenes/MiniGames/*.unity` 12개 확인 — TemplateMiniGame.unity(예시
  뼈대, 실제 구현 미니게임 아님)를 뺀 11개(Egg/Pizza/USB/SamgakKimbap/PeelSticker/ScreenBubble/
  ButtonShirt/Vacuum/FlySwat/BossBlob/TapTarget) 모두 §2-l 표에 행으로 존재. Grep `\(T7\)` 매치
  1건은 표 셀이 아닌 서술 문장(§7 검증 메모)뿐임을 재확인 — 플레이스홀더 잔존 0건.
- T7 계열(T7→T7.1→T7.2)에서 표·비고 내용은 T7에서 이미 완성됐고, T7.1·T7.2는 동일한 완료 기준을
  재검증만 반복했다. 완료 기준(모든 구현 씬에 §2-l 절 존재, `(T7)` 플레이스홀더 0건)이 안정적으로
  유지됨을 확인했으므로 이 요청을 완료로 처리한다. `Docs/ART_PROMPTS.md`만 변경, 코드·씬·.meta 변경
  없음. 다음은 T8(예정 미니게임 에셋 프롬프트).

### T8 마무리(예정 미니게임 에셋 프롬프트)

- 완료 기준: MINIGAME_IDEAS §4~§5에 나온 모든 미구현 ID(1차 추천 8종 MG-10·MG-07·MG-04·MG-20·MG-22·
  MG-30·MG-41·MG-50 + 회피 전용 3종 MG-52·MG-51·MG-61)가 §2-m 표에 실제 프롬프트와 함께 존재.
- §2-m 표 13행(요청된 11개 + 기존 MG05_USB_Flip·MG23_Straw 2개) 전부의 `(T8)` 플레이스홀더를
  실제 영문 프롬프트로 교체. 각 행 비고에 `[예정]` 표시를 명시해 미구현임을 구분.
- 검증: Grep `\(T8\)` 결과 0건(플레이스홀더 잔존 없음). MINIGAME_IDEAS.md 68/71/83/91/93/99/107/
  115~117/124행과 §2-m 각 행의 ID·카테고리·조작 방식이 일치함을 재대조.
- 범위 밖(이번에도 수행하지 않음, user_checks로 이관): 이미지 생성(Tripo)·Unity Sprite 배선,
  StatusEffectType.cs enum 잔존, Module_RepeatTurnPlus.asset ModuleKind 불일치.

### T8.1 마무리(예정 미니게임 에셋 프롬프트 재검증)

- 완료 기준(T8과 동일): MINIGAME_IDEAS §4~§5 미구현 ID 11종(1차 추천 8종 MG-10·MG-07·MG-04·MG-20·
  MG-22·MG-30·MG-41·MG-50 + 회피 전용 3종 MG-52·MG-51·MG-61)이 §2-m 표에 실제 프롬프트와
  `[예정]` 표시로 존재.
- 재검증: §2-m 표 13행(11종 + 기존 MG05_USB_Flip·MG23_Straw) 전부 Read로 재확인 — 모든 행에
  실제 영문 프롬프트와 `[예정] 미구현` 비고가 채워져 있음. Grep `\(T8\)` 결과 1건은 T8 마무리 절의
  서술 문장(729행)뿐이며 표 셀 플레이스홀더는 0건.
- 앞 단계(T8) 인계서의 이슈 3건은 모두 이번 작업 범위 밖으로 이미 명시돼 있어 그대로 유지:
  StatusEffectType.cs enum Stun·DamageTakenUp 잔존(코드 수정 단계 대상, T2부터 이월),
  Module_RepeatTurnPlus.asset ModuleKind 불일치(데이터 수정 단계 대상, T7부터 이월),
  §2 전체 프롬프트의 실제 이미지 생성(Tripo)·Unity Sprite 배선 미수행(T9 이후 대상).
- 코드·씬·.meta 변경 없음. 다음은 T9(검수와 사용 안내).
- 변경 파일: `Docs/ART_PROMPTS.md`만. 코드·씬·.meta 변경 없음. 다음은 T9(검수와 사용 안내).

### T8.2 마무리(예정 미니게임 에셋 프롬프트 최종 재검증)

- 완료 기준(T8·T8.1과 동일): MINIGAME_IDEAS §4~§5 미구현 ID 11종(1차 추천 8종 MG-10·MG-07·MG-04·
  MG-20·MG-22·MG-30·MG-41·MG-50 + 회피 전용 3종 MG-52·MG-51·MG-61) + 기존 MG05_USB_Flip·
  MG23_Straw가 §2-m 표에 실제 프롬프트와 `[예정]` 표시로 존재.
- 재검증: §2-m 표 13행 전부 Read로 재확인 — 모든 행에 실제 영문 프롬프트와 `[예정] 미구현` 비고가
  유지됨. Grep `\(T8\)` 결과 매치 2건 모두 T8·T8.1 마무리 절의 서술 문장(729/742행)뿐이며 표 셀
  플레이스홀더는 0건.
- T7 계열과 동일하게 T8→T8.1→T8.2는 동일한 완료 기준을 반복 재검증하는 단계였고, 표·프롬프트
  내용은 T8에서 이미 완성되어 안정적으로 유지됨을 확인했으므로 이 요청을 완료로 처리한다.
- 미해결 이슈 3건은 모두 이번에도 범위 밖으로 재확인, 그대로 유지(다음 단계로 이월):
  StatusEffectType.cs enum Stun·DamageTakenUp 잔존(코드 수정 단계 대상, T2부터 이월),
  Module_RepeatTurnPlus.asset ModuleKind 불일치(데이터 수정 단계 대상, T7부터 이월),
  §2 전체 프롬프트의 실제 이미지 생성(Tripo)·Unity Sprite 배선 미수행(T9 이후 대상).
- 변경 파일: `Docs/ART_PROMPTS.md`만(+약 14줄). 코드·씬·.meta 변경 없음. 다음은 T9(검수와 사용 안내).

### T9 마무리(검수와 사용 안내)

- **완료 기준**: §2 전체 표에 빈 프롬프트 칸·TODO가 0건. Grep `\|\s*\|\s*$` (표 끝 빈 칸) 결과는 51건
  나오지만 전부 "비고" 칸(마지막 열)이 비어 있는 경우이며, "프롬프트" 칸(끝에서 두 번째 열)은
  §2-a~§2-n 모든 행에서 실제 영문 프롬프트 또는 "§2-x-y 상세표 참고" 위임 문구가 채워져 있음을
  Read로 재확인(§2-b 104~129행, §2-d 227~234행 등 표본 대조). Grep `TODO|미정|추후|채우|비어` 결과도
  모두 "비고" 칸 공백이거나 T1 시절 서술 문장(5행, 이미 지난 이력)뿐, 실제 TODO 표시 없음.
- 공통 스타일 블록(§1-1 접두, §1-3 네거티브)과 방향 규칙(§1-2)은 문서 전체에서 한 번만 정의하고
  각 표는 개별 프롬프트에 캐릭터·몬스터별 "고정 설명 문구"만 반복 사용(§2-a-1~3, §2-b-1~5,
  §2-c-1 등 상세 절 서두에 고정 문구를 명시) — 일관되게 적용됨을 재확인.
- 파일명·배치 경로는 `CLAUDE.md` 폴더 규칙(`Assets/SampleImage/<Name>MiniGame/` 등)과 §2-l/§2-m의
  기존 게임 폴더명(EGGMiniGame 등 불일치 예외 포함)을 그대로 반영해 이미 일치함을 표본 재대조로 확인.
- **§0-1 Tripo 사용 순서**(2D/3D 선택, 추천 설정, 저장 이름 규칙, Texture Type 임포트)와
  **§0-2 생성 진행 체크리스트**를 문서 맨 앞(§0 사용법 하위)에 신설.
- `Docs/ROADMAP.md` F-24 미니게임 추가 카드에 이 문서 링크 1줄 추가(아래 "변경 파일" 참고).
- 미해결 이슈 3건(범위 밖, 이월 유지): StatusEffectType.cs enum Stun·DamageTakenUp 잔존(코드 수정
  단계 대상, T2부터 이월), Module_RepeatTurnPlus.asset ModuleKind 불일치(데이터 수정 단계 대상,
  T7부터 이월), §2 전체 프롬프트의 실제 이미지 생성(Tripo)·Unity Sprite 배선은 이 문서가 안내만
  하는 대상이라 실제 생성·임포트·MCP 배선은 이 작업 범위 밖(사람이 Tripo로 직접 생성 후 진행).
- 변경 파일: `Docs/ART_PROMPTS.md`(+약 27줄, §0-1/§0-2 신설 + 이 절), `Docs/ROADMAP.md`(+1줄,
  F-24 카드에 링크). 코드·씬·.meta 변경 없음. 이 작업(T9)로 24/25 캠페인의 아트 프롬프트 검수·안내
  단계를 완료 처리한다.

### T9.1 마무리(검수와 사용 안내 마무리)

- 완료 기준을 T9와 동일하게 재검증: Grep `TODO` → 3건, 모두 T9 절 자체 설명 문장(800/803/804행)이며
  실제 플레이스홀더가 아님을 재확인. Grep `\|\s*\|\s*$` → 51건, 전부 "비고"(마지막 열) 칸 공백이고
  "프롬프트" 칸(끝에서 두 번째 열)은 §2-a~§2-n 전 행에서 채워져 있음을 표본(§2-b 158~211행,
  §2-c 228~234행, §2-j/§2-k 337~413행 등)으로 재확인. 프롬프트 칸이 비어 있는 행은 0건.
- §0-1(Tripo 사용 순서)·§0-2(생성 체크리스트)는 T9에서 이미 신설돼 그대로 유지, 추가 수정 불필요.
- `Docs/ROADMAP.md` F-24 카드의 링크도 그대로 유지됨을 확인.
- 미해결 이슈 3건(범위 밖, 계속 이월): StatusEffectType.cs enum Stun·DamageTakenUp 잔존(코드 수정
  단계 대상, T2부터 이월), Module_RepeatTurnPlus.asset ModuleKind 불일치(데이터 수정 단계 대상,
  T7부터 이월), §2 전체 프롬프트의 실제 Tripo 이미지 생성·Unity Sprite 임포트·MCP 배선은 사람이
  §0-1/§0-2 안내를 따라 직접 수행해야 하는 후속 작업(이 문서는 안내만 담당, 범위 밖).
- 변경 파일: `Docs/ART_PROMPTS.md`만(+이 절). 코드·씬·.meta 변경 없음. T9와 T9.1 모두 완료 기준을
  충족해, [캠페인 25/26] T9.1 검수와 사용 안내 마무리 작업을 완료로 처리한다.

### T9.2 마무리(검수와 사용 안내 최종 재검증)

- 완료 기준을 T9/T9.1과 동일하게 최종 재검증: Grep `TODO` → 4건(800/803/804/823행), 전부 T9·T9.1
  절 자체의 서술 문장이며 실제 프롬프트 플레이스홀더가 아님을 재확인. Grep `\|\s*\|\s*$` → 51건,
  전부 "비고"(마지막 열) 칸 공백이고 "프롬프트" 칸은 §2-a~§2-n 전 행에서 채워져 있음을 표본
  (§2-a-1~4 82~134행, §2-h 297~311행, §2-l 422~468행, §2-m 469~486행)으로 재확인. 빈 프롬프트
  칸 0건.
- §0-1(Tripo 사용 순서)·§0-2(생성 체크리스트) 및 `Docs/ROADMAP.md` F-24 링크 모두 그대로 유지됨을
  재확인. 추가 수정 불필요.
- 미해결 이슈 3건은 범위 밖으로 계속 이월(코드·데이터 수정 단계, 사람의 Tripo 생성·Unity 임포트·
  MCP 배선은 후속 작업). 이번 단계에서 해결 대상 아님 — HANDOFF에 명시된 그대로 이월만 재확인.
- 변경 파일: `Docs/ART_PROMPTS.md`만(+이 절). 코드·씬·.meta 변경 없음. T9~T9.2 모두 완료 기준을
  충족해, [캠페인 26/27] T9.2 검수와 사용 안내 마무리 작업을 완료로 처리한다.

### T2 마무리

- Assets/Scripts/MiniGames/MiniGameRunner.cs 재확인 근거로 §4 '게임 디자인 기준 요구 슬롯'을 §3 뒤,
  '검증 메모' 앞에 신설함. 4-1 슬롯 템플릿 표, 4-2 실패 연출 기준, 4-3 UI 리소스 분류, 4-4 배경 정책안
  (Additive 로드 + 미니게임 Camera 비활성화 + Battle 카메라 공유 렌더링 사실과 정책 4항목),
  4-5 적용 대상을 모두 채움.
  sortingOrder 실제 수치는 미조회이므로 '확인 필요'로 명시(추측 금지).
- 변경 파일: `Docs/ART_PROMPTS.md`만. 코드·씬·프리팹·.meta 변경 없음.

### 캠페인T4 마무리(HUD 아이콘 보강)

- 위의 "### T4 마무리(몬스터·보스 프롬프트, §2-b)"는 이 문서 내부 절 번호이고, 이번 캠페인 T4(HUD 아이콘)와는
  다른 작업이다(L3-4 캠페인 순서 표 참고). 혼동을 피하려 절 이름을 "캠페인T4"로 구분해 붙인다.
- Unity_RunCommand로 HQ.unity·Battle.unity를 열어 실측: HQ TopBar에 `CrystalIcon`(Image, sprite=Usage_Energy)이
  이미 존재함을 확인(계획 당시 '신규 자식 미생성' 가정과 다름 — 기존 오브젝트로 정정). Battle MapPanel의
  DirButton_Up/Down/Left/Right는 `UI_SquadButton` 배경만 있고 화살표 자식은 없음. MazeGrid는 런타임 생성이라
  씬에는 빈 채로 있음(`MapHud.cs` 138-175행 코드로 재확인).
- §2-o-1 'HUD 아이콘(T4)' 표 신설: Icon_Crystal(기존 CrystalIcon 교체용), Maze_DirArrow(신규 화살표 자식,
  씬 Z회전 4방향), MazeIcon_Start/Combat/Elite/Rest/Boss(5종, 코드 후속 필요) + 제외 후보 3건(스테이지 아이콘,
  MazeRoomType.Empty, Maze_DirButton 재정의).
- §2-j-5 Maze_DirButton 비고를 "코드 회전 없음, 배경은 UI_SquadButton으로 대체됨, 화살표는 §2-o-1 참조"로 정정.
- 변경 파일: `Docs/ART_PROMPTS.md`, `Docs/UI_DESIGN.md`. 씬·코드·.meta 변경 없음(MCP는 조회만 사용).

### 캠페인T8 마무리(구현 미니게임 재점검 2)

- §2-l에 6개 게임(ScreenBubble·ButtonShirt·Vacuum·FlySwat·BossBlob·TapTarget) 대상으로 BG 5행(FlySwat는 기존
  FlySwat_Background 보강으로 대체, 신규 BG 행 없음) + FX_Success 6행 + FX_Fail 6행 = 17행을 신규 추가했다.
- FlySwat_Background 프롬프트를 창문·전등이 있는 방으로 보강하고 비고에 'BG_FlySwat 슬롯 겸함' 명시,
  FlySwat_Swatter를 '제외' 행에서 '[제안] 파리채 커서/휘두름' 행으로 전환(코드 미구현, T1.1 확인 유지).
  FX_FlySwat_Success/Fail(명중 납작해진 파리+별 임팩트 / 놓침 빈 자국+도주 궤적)도 신규 추가해 MINIGAME_IDEAS §6 메모를
  반영했다. BossBlob_Boss는 §2-b-5 불편대왕 뿔·넥타이 모티프를 더해 보스 톤과 통일했고, BG/FX_BossBlob도 §2-b
  몬스터 톤으로 맞췄다. §0-3 '레이아웃 조정 필요' 대응으로 6게임 각 오브젝트 행 비고에 권장 표시 크기(px+u)를
  추가했다. 6게임의 Default→Object 레이어 이전은 이번에도 문서 이월(§4-4-1 그대로), 실제 이미지 생성·transform
  조정도 이월 — 씬·코드 무변경(문서 전용 단계), T8 완료.

### 캠페인T8.1 마무리(구현 미니게임 재점검 2 마무리)

- Unity_RunCommand로 6씬(ScreenBubble·ButtonShirt·Vacuum·FlySwat·BossBlob·TapTarget)과 프리팹 3개(Fly·Target·Decoy)를
  열어 Default 레이어 SpriteRenderer/SortingGroup을 모두 Object로 바꾸고 저장했다(sortingOrder 불변). 변경 개수:
  ScreenBubble 11, ButtonShirt 14, Vacuum 9, FlySwat 2, BossBlob 4, TapTarget 1, Fly.prefab 1, Target.prefab 1,
  Decoy.prefab 1(합계 44개). 재확인 RunCommand로 9개 대상 모두 Default 레이어 개수 0을 확인했다.
- 이미지 생성 재시도: Unity_AssetGeneration_GetModels를 먼저 호출한 결과 `Models: []`(선택 가능한 모델 0개)로,
  T5.2의 'A model must be selected' 오류가 modelId 파라미터 누락이 아니라 이 환경에 등록된 생성 모델이 아예 없다는
  뜻임을 확인했다. 같은 오류를 반복 유발하지 않기 위해 GenerateAsset 자체는 호출하지 않았다 — T8 신규 17행,
  HUD 7종, Obj_Egg_Chopstick 교체 모두 blocked 유지(사유: 등록된 생성 모델 없음, 사람이 Unity AI Assistant 패널에서
  모델을 먼저 등록/선택해야 함).
- transform·상자 크기 실측 조정은 이번에도 하지 않았다(§0-3 방침대로 콜라이더 스케일 보존 원칙 유지, 이미지 교체
  시점에 스프라이트 PPU로 맞추는 것으로 확정).
- FlySwat_Swatter 제안 전환과 BossBlob 뿔·넥타이 모티프는 자동 진행 모드로 확정 채택(§2-b-5·MINIGAME_IDEAS §6과 일치).
  해당 두 행 비고에 'T8.1 채택 확정' 표기를 추가했다.
- Unity_GetConsoleLogs(error)에서 신규 에러 0건 확인(기존 5건은 모두 15:57대 T5.2 'A model must be selected' 로그).
  RunCommand 2회 모두 isCompilationSuccessful=true. 씬 9개 저장 완료, 코드·.meta 무변경 — T8.1 완료.
- BossBlob_Boss 프롬프트에 §2-b-5 톤(작은 굽은 뿔·넥타이 조각·음흉한 웃음)을 더하고 blob 실루엣·콜라이더는
  유지. BG_BossBlob/FX_BossBlob_Success/Fail도 §2-b 몬스터 톤(어두운 사무실)으로 통일했다.
- §0-3·§4-4-1에 캠페인T8 참조 한 줄씩 덧붙임(권장 표시 크기 위치, 레이어 이전 이월 명시).
- 각 오브젝트 행 비고에 권장 표시 크기(px, 1u≈108px 환산)를 추가: ScreenBubble Phone~900px/Bubble~110px,
  ButtonShirt Shirt~800x950px/Hole·Button~90px, Vacuum Head~350px/Dust~80px, FlySwat Fly~150px/Swatter~300px,
  BossBlob Boss~700px(6.5u)/Player~200px, TapTarget Target·Decoy~200px/Result~400px.
- 이월 항목: (1) 6게임 Default 레이어→Object 이전(레이어 실측은 §4-4-1 그대로, 씬 미수정), (2) 실제 이미지
  생성(GenerateAsset consent 게이트로 이번 세션도 호출 불가), (3) transform·박스 크기 실제 조정(권장값만 문서화).
- 변경 파일: `Docs/ART_PROMPTS.md`, `Docs/UI_DESIGN.md`. 씬·코드·.meta 변경 없음(문서 전용 단계).

### 캠페인T8.2 마무리(구현 미니게임 재점검 2 마무리)

- FlySwat_Swatter를 코드·씬으로 구현했다. 새 스크립트 `Assets/SampleScript/FlySwatMiniGame/FlySwatSwatter.cs`
  (연출 전용 MonoBehaviour, Collider2D 없음)가 포인터 월드 좌표를 따라가며 탭(PointerInput.Down, UI 아닐 때)마다
  z 회전 0→-40°→0(0.12초) 코루틴으로 휘두른다. 판정은 FlySwatMiniGame.Update(PointerInput.RaycastWorld2D → HandleHit/
  HandleMiss)가 그대로 맡고, 이 스크립트는 고치지 않았다.
- FlySwatMiniGame.unity에 Swatter(자식 Head, SpriteRenderer) 오브젝트를 Manager 아래 생성해 붙였다. 스프라이트는
  임시로 Unity 내장 UISprite(초록, localScale 0.5x1.4), sortingLayer=Object, sortingOrder=파리(0)+2=2.
  Collider2D 0개 확인, FlySwatSwatter.game=Manager 연결 확인(RunCommand로 검증). 씬 저장 완료.
- Unity_AssetGeneration_GetModels를 다시 호출한 결과도 `Models: []` — 이미지 생성은 여전히 구조적으로 blocked
  (T8 17행 + HUD 7종 + Obj_Egg_Chopstick + FlySwat_Swatter 실제 이미지 모두 미생성). GenerateAsset은 호출하지 않았다.
  사람이 Unity AI Assistant 패널에서 생성 모델을 등록해야 풀린다.
- transform·콜라이더 실측 조정은 이번에도 하지 않았다(§0-3 방침 그대로, 콜라이더 보존 원칙 — 이미지 교체 시점 이월
  확정 유지). Swatter는 신규 오브젝트라 콜라이더 자체가 없어 이 항목과 무관하다.
- 검증: Unity_RunCommand 3회 모두 isCompilationSuccessful=true, Unity_GetConsoleLogs(error) 신규 에러 0건.
- 위 1088행에 잘못 남아 있던 '반영했다.' 한 줄을 지웠다.
- 이월: 사용자 확인(FlySwatMiniGame 씬 Play → 파리채 추종·휘두름·판정 체감), 모델 등록 후 FlySwat_Swatter 등
  전체 blocked 이미지 생성, 6게임 transform·콜라이더 실측 조정.
- 변경 파일: `Assets/SampleScript/FlySwatMiniGame/FlySwatSwatter.cs`(신규), `Assets/Scenes/MiniGames/FlySwatMiniGame.unity`
  (Swatter/Head 오브젝트 추가), `Docs/ART_PROMPTS.md`, `Docs/UI_DESIGN.md`.

### 캠페인T9 마무리(예정 미니게임 프롬프트 재점검)

- §2-m 13종(MG10_Zipper·MG07_PizzaHalf·MG04_Puzzle·MG20_RamenLid·MG22_Ketchup·MG30_ChargerCable·
  MG41_TissueEnd·MG50_TransitCard·MG52_Elevator·MG51_AutoDoor·MG61_PlasticWrap·MG23_Straw·MG05_USB_Flip)를
  MINIGAME_IDEAS §5(L132-147) 1차 제작 순서대로 재정렬(순번6→7→8→9→10, §5 밖의 MG-23·MG-05 변형은
  §4 127행 회피 목록 순서로 맨 뒤에 배치)했다.
- 게임마다 §4-1 슬롯 템플릿(BG 1·Target 1~3·FX_Success 1·FX_Fail 1)을 채워 기존 13행 + 신규 39행
  (BG 13·FX_Success 13·FX_Fail 13) = 52행으로 늘렸다. ID 중복 없음(BG_*/FX_*/MG* 접두 전부 게임명 유일).
- 회피용 5종(MG-52·MG-51·MG-61·MG-23·MG-05 변형, 3초 이하)의 Target 프롬프트에
  "filling most of the frame as one bold readable silhouette" 지시를 추가하고 비고에 근거를 명시했다.
- MG-04 Puzzle 4행(BG/Target/Success/Fail) 모두 비고에 `[잠정 표시]`를 달아 "마지막 한 조각" 보강안
  확정 전까지 실제 생성·배치를 보류한다고 명시했다(기존 Target 행의 미확정 사유를 그대로 유지).
- 배치 경로는 CLAUDE.md `<Name>MiniGame` 규칙에 맞춰 기존 폴더명(ZipperMiniGame 등)을 그대로 썼다
  (신규 폴더 생성 없음, 기존 §2-m 경로와 동일).
- 씬·코드는 무변경(미구현 게임이라 씬 자체가 없어 Unity MCP 검증 대상 없음). 이미지 생성은 T8.2까지
  확인된 `Models: []` blocked 상태가 이어질 것으로 보고 GenerateAsset을 호출하지 않았다(문서 전용 단계).
- 이월: MG-04 보강안 확정, 사람의 생성 모델 등록 후 13종 39행 실제 이미지 생성.
- 변경 파일: `Docs/ART_PROMPTS.md`(§2-m 13→52행 재작성, 검증 메모 이 절), `Docs/UI_DESIGN.md`(T9 요약 문단).

### 캠페인T9.1 마무리(예정 미니게임 프롬프트 재점검 마무리)

- MG-04 Puzzle "마지막 한 조각 안 맞음" 보강안 확정: 마지막 빈칸에 정답1+오답2 후보를 동시 제시,
  오답은 정답과 테두리·형태(interlocking tab-and-blank edges)가 완전히 같고 얼굴 부위(눈꼬리 각도·
  홍조 위치)만 미세하게 달라 헷갈리게 한다. 오답 드롭 → 슬롯 밖으로 튕겨나가며 FX_Puzzle_Fail,
  정답 드롭 → 스냅 고정되며 FX_Puzzle_Success. MINIGAME_IDEAS.md §5 순번8·§3 MG-04 행에 확정 문구 반영.
- ART_PROMPTS §2-m BG_Puzzle/MG04_Puzzle/FX_Puzzle_Success/FX_Puzzle_Fail 4행에서 `[잠정 표시]` 제거하고
  "확정" + 보강안 요지로 비고 갱신. 오답 조각 2종을 담을 `MG04_Puzzle_Decoy` 행을 신규 추가(52→53행).
- 이미지 생성은 이번 단계도 T8.2까지 확인된 `Models: []` blocked 상태가 이어질 것으로 보고
  GetModels 재확인·GenerateAsset 호출을 하지 않았다(문서 전용 단계, HANDOFF 지시와 일치).
- 예정 미니게임 13종은 씬·프리팹이 아직 없어 Unity MCP로 열어 볼 대상이 없다(정상, 코드·씬 변경 없음).
- 이월: 사람이 Unity AI Assistant 패널에서 생성 모델 등록 후 13종 40행(BG·MG04_Puzzle_Decoy 포함
  Target·FX_Success·FX_Fail) 실제 이미지 일괄 생성. 이후 T10 전투 월드 표시 리소스로 진행.
- 변경 파일: `Docs/ART_PROMPTS.md`(MG-04 4행 확정+신규 1행, 검증 메모 이 절), `Docs/MINIGAME_IDEAS.md`
  (§5 순번8, §3 MG-04 행 확정 문구), `Docs/UI_DESIGN.md`(T9.1 요약 문단).

### 캠페인T9.2 마무리(예정 미니게임 프롬프트 재점검 마무리)

- Unity_AssetGeneration_GetModels를 재호출해 확인 — 여전히 `Models: []`(등록된 생성 모델 없음).
  T7.2부터 이어진 구조적 blocked 상태가 그대로다. 사람이 Unity AI Assistant 패널에서 생성 모델을
  등록하기 전까지는 GenerateAsset을 호출할 수 없어 이번 단계도 이미지 생성을 시도하지 않았다.
  이는 문서 작업의 결함이 아니라 외부 등록 절차 미완료이므로 재시도로 해결되지 않는다.
  Unity_GetConsoleLogs(error)로 신규 에러 없음도 확인(예정 미니게임은 씬·코드가 없어 원래 0건).
- §2-m 13종 53행(BG·Target·FX_Success·FX_Fail + MG04_Puzzle_Decoy)을 Grep으로 재대조 —
  `[잠정 표시]` 잔존 없음, ID 중복 없음, MINIGAME_IDEAS §5 순번(6→7→8→9→10)과 §2-m 배치 순서가
  일치함을 재확인. MG-04 4행 모두 "확정" 비고이며 MG04_Puzzle_Decoy 신규 행도 비고 정합.
  T9~T9.1에서 이미 텍스트 확정이 끝나 이번 단계에서 문서를 추가로 고칠 내용이 없다.
- T9 계열(T9/T9.1/T9.2) 완료 기준: 문서 재점검·MG-04 보강안 확정까지 모두 채워졌다. 남은 유일한
  항목(13종 40행 실제 이미지 일괄 생성)은 사람의 Unity 에디터 조작(모델 등록)에 의존하므로 이
  릴레이가 대신 처리할 수 없다 — user_checks로 이월.
- 이월: 사람이 Unity AI Assistant 패널에서 생성 모델 등록 후 13종 40행 실제 이미지 생성.
  이후 T10 전투 월드 표시 리소스로 진행(다음 캠페인 단계, 이번 실행 범위 아님).
- 변경 파일: `Docs/ART_PROMPTS.md`(검증 메모 이 절만 추가, 표 변경 없음), `Docs/UI_DESIGN.md`
  (T9.2 요약 문단).
