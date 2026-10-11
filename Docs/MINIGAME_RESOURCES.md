# 미니게임 리소스 참고 문서

구현된 미니게임 11개(Egg·Pizza·USB·SamgakKimbap·PeelSticker·ScreenBubble·ButtonShirt·Vacuum·FlySwat·BossBlob·TapTarget)의
씬 오브젝트, 현재 스프라이트, 저장 경로, ART_PROMPTS 키를 정리한다. 값은 모두 Unity_RunCommand로 씬·프리팹을 직접 열어 읽은 실측값이다(§검증 메모).

## §0. 공통 규칙

- Texture Type: **Sprite (2D and UI)**.
- 피벗: 오브젝트류는 가운데(Center), 캐릭터·몬스터류는 바닥 가운데(Bottom Center) — UI_DESIGN §28 참조.
- PPU: 기존 커스텀 이미지는 100(일부 UISprite/Knob 빌트인 placeholder는 200/256) — 교체 시 100을 기준으로 맞춘다.
- **콜라이더·Transform scale은 그대로 두고 SpriteRenderer.sprite만 교체한다.** 히트 영역 기준은 UI_DESIGN §28, 교체 절차는 ART_PROMPTS §0 참조.
- 기존 파일을 덮어쓰지 않는다(새 파일명 사용, 필요하면 번호 접미사).
- 폴더 이름은 기존 그대로 쓴다(`EGGMiniGames`처럼 게임마다 철자가 다름 — CLAUDE.md "폴더와 이름" 참조).

## §1. 리소스 뽑는 순서 체크리스트

- [ ] 1) 이 문서에서 상태가 `placeholder` 또는 `신규 필요`인 행을 고른다.
- [ ] 2) ART_PROMPTS.md §2-l에서 해당 키의 프롬프트를 확인한다(§1-1 공통 접두사·네거티브 포함).
- [ ] 3) 권장 원본 해상도로 생성한다(Unity_AssetGeneration_GenerateAsset 또는 외부 생성 후 Import).
- [ ] 4) 저장 경로·파일명이 이미 있는지 확인한다(덮어쓰기 금지).
- [ ] 5) Import 설정(Texture Type=Sprite, pivot, PPU)을 맞춘다.
- [ ] 6) MCP로 해당 SpriteRenderer.sprite만 교체한다(콜라이더·scale 불변).
- [ ] 7) 콘솔 에러 0을 확인하고 Play 모드로 짧게 확인한다.
- [ ] 8) 이 문서의 상태 칸을 `완료`로 갱신한다.

---

## §2. Egg (`Assets/Scenes/MiniGames/EggMiniGame.unity`, `Assets/SampleImage/EGGMiniGames/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| EGGMiniGame/EGGFrying/Fan | 후라이팬_0 (커스텀) | 후라이팬 | 512x512 / 1334x528 | EGGMiniGames/후라이팬.png | 완료(교체 권장) | Egg_Fan |
| EGGMiniGame/EGGFrying/Egg | egg1_0 (커스텀) | 계란(진행 단계) | 256x256 / 1069x268 | EGGMiniGames/egg1.png 등 | 완료(유지) | Egg_Egg |
| Obj_Egg_Chopstick | Placeholder_Chopstick (단색 placeholder, asset 경로 없음) | 젓가락 연출 | 512x512 / 9x69 | EGGMiniGames/Obj_Egg_Chopstick.png | placeholder | Obj_Egg_Chopstick |
| (씬에 없음) | — | 주방 배경 | 1920x1080 | EGGMiniGames/BG_Egg.png | 신규 필요 | BG_Egg |
| EGGFrying/Egg (Animator isSuccess/isFailed) | egg1_0 재사용 | 성공/실패 연출 | 256x256 | EGGMiniGames/ | 신규 필요(연출용 별도 스프라이트 없음) | FX_Egg_Success / FX_Egg_Fail |

## §3. Pizza (`Assets/Scenes/MiniGames/PizzaMiniGame.unity`, `Assets/SampleImage/PizzaMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| PIzzaGame/TotalPizza | pizzaAssert_2 (커스텀) | 전체 피자 | 1024x1024 / 967x266 | PizzaMiniGame/pizzaAssert.png | 완료(교체 권장) | Pizza_TotalPizza |
| PIzzaGame/Cutter | pizzaDga_0 (커스텀) | 피자 커터 | 512x512 / 365x172 | PizzaMiniGame/pizzaDga.png | 완료(교체 권장) | Pizza_Cutter |
| Cutter/PizzaPiece | piz1_0 (커스텀) | 조각난 피자 | 256x256 / 507x156 | PizzaMiniGame/PIZZApiece/piz1.png 등 | 완료 | Pizza_Piece |
| Cutter/PizzaPiece/Triangle (비활성) | pizzaAssert_1 (커스텀) | 피자 단면 연출 | 256x256 / 386x240 | PizzaMiniGame/pizzaAssert.png | 완료 | Pizza_TotalPizza(공유) |
| Cutter/PizzaPiece/Plate (비활성) | foodplate_0 (커스텀) | 접시 보조 | 512x512 / 324x132 | PizzaMiniGame/foodplate.png | 완료 | Pizza_Plate |
| PIzzaGame/PizzaPlate | plate_0 (커스텀) | 접시 | 512x512 / 1273x382 | PizzaMiniGame/plate.png | 완료 | Pizza_Plate |
| (씬에 없음) | — | 식탁 배경 | 1920x1080 | PizzaMiniGame/BG_Pizza.png | 신규 필요 | BG_Pizza |
| Cutter/PizzaPiece (Animator isSuccess/isFailed) | piz1_0 재사용 | 성공/실패 연출 | 256x256 | PizzaMiniGame/ | 신규 필요 | FX_Pizza_Success / FX_Pizza_Fail |

## §4. USB (`Assets/Scenes/MiniGames/USBMiniGame.unity`, `Assets/SampleImage/USBMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| USBMiniGame/NoteBook | USB_NoteBook (커스텀) | 노트북 | 1024x1024 / 1207x1207 | USBMiniGame/USB_NoteBook.png | 완료(ART_PROMPTS 기록은 사진이라 했으나 실측은 이미 커스텀 이미지로 교체돼 있음) | USB_NoteBook |
| NoteBook/UsbPort | usb (커스텀) | USB 포트 | — / 123x123 | USBMiniGame/usb.png | 완료 | USB_NoteBook(공유) |
| HandObject/usbHand | USB_Hand (커스텀) | 손·케이블 | 512x512 / 484x484 | USBMiniGame/USB_Hand.png | 완료(ART_PROMPTS 기록 대비 이미 교체됨) | USB_Hand |
| HandObject/usbHand/FailedUSB (비활성) | USB_FailedUSB (커스텀) | 실패 연출 | 512x512 / 67x67(활성 시 확대) | USBMiniGame/USB_FailedUSB.png | 완료 | FX_USB_Fail |
| (씬에 없음) | — | 책상 배경 | 1920x1080 | USBMiniGame/BG_USB.png | 신규 필요 | BG_USB |
| UsbMover (Animator isSuccess) | — | 성공 연출 | 256x256 | USBMiniGame/ | 신규 필요 | FX_USB_Success |

## §5. SamgakKimbap (`Assets/Scenes/MiniGames/SamgakKimbapMiniGame.unity`, `Assets/SampleImage/SamgakKimbapMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| ResultRoot | Circle (Unity 기본 도형) | 결과 표시 루트 | 256x256 / 324x324 | SamgakKimbapMiniGame/ | placeholder | FX_Kimbap_Success / FX_Kimbap_Fail |
| Manager/Wrapper | Kimbap_Wrapper (커스텀) | 포장 | 512x512 / 3318x2765(큰 스케일) | SamgakKimbapMiniGame/Kimbap_Wrapper.png | 완료 | Kimbap_Wrapper |
| Manager/Laver | Kimbap_Laver (커스텀) | 김 | 512x512 / 2875x2378 | SamgakKimbapMiniGame/Kimbap_Laver.png | 완료 | Kimbap_Laver |
| Manager/Tab_Top·Left·Right | Kimbap_Tab (커스텀) | 뜯는 탭 | 128x128 / 111x111 | SamgakKimbapMiniGame/Kimbap_Tab.png | 완료 | Kimbap_Tab |
| Manager/Arrow_1~3 (비활성) | Kimbap_Arrow (커스텀) | 안내 화살표 | 128x128 / 42~83 | SamgakKimbapMiniGame/Kimbap_Arrow.png | 완료 | Kimbap_Arrow |
| (씬에 없음) | — | 편의점 테이블 배경 | 1920x1080 | SamgakKimbapMiniGame/BG_SamgakKimbap.png | 신규 필요 | BG_SamgakKimbap |

## §6. PeelSticker (`Assets/Scenes/MiniGames/PeelStickerMiniGame.unity`, `Assets/SampleImage/PeelStickerMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| Book/Page_BG | PeelSticker_Book (커스텀) | 책 배경 | 512x512 / 86x86 | PeelStickerMiniGame/PeelSticker_Book.png | 완료 | PeelSticker_Book |
| Book/Spine, Line_0~4 | UISprite (빌트인 placeholder) | 책등·줄 | 512x512 | PeelStickerMiniGame/PeelSticker_Book.png(통합 또는 분리) | placeholder | PeelSticker_Book |
| Book/Sticker/StickerBody | PeelSticker_Sticker (커스텀) | 스티커 | 256x256 / 28x28 | PeelStickerMiniGame/PeelSticker_Sticker.png | 완료 | PeelSticker_Sticker |
| Book/Sticker/PeeledFlap | UISprite (빌트인 placeholder) | 뜯긴 플랩 | 256x256 | PeelStickerMiniGame/PeelSticker_Sticker.png(관련) | placeholder | PeelSticker_Sticker |
| Book/Sticker/TornHalf (비활성) | PeelSticker_TornHalf (커스텀) | 실패 연출 | 256x256 / 14x14 | PeelStickerMiniGame/PeelSticker_TornHalf.png | 완료 | FX_PeelSticker_Fail |
| Book/Sticker/CornerTab | Knob (빌트인 placeholder) | 모서리 탭 | 256x256 | PeelStickerMiniGame/PeelSticker_Sticker.png(관련) | placeholder | PeelSticker_Sticker |
| Book/Sticker/Arrow | Kimbap_Arrow (커스텀, 공유) | 안내 화살표 | 128x128 / 88x88 | SamgakKimbapMiniGame/Kimbap_Arrow.png | 완료(공유 자산) | Kimbap_Arrow |
| (씬에 없음) | — | 책상 배경 | 1920x1080 | PeelStickerMiniGame/BG_PeelSticker.png | 신규 필요 | BG_PeelSticker |

## §7. ScreenBubble (`Assets/Scenes/MiniGames/ScreenBubbleMiniGame.unity`, `Assets/SampleImage/ScreenBubbleMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| PhoneFrame/Frame | ScreenBubble_Phone (커스텀) | 휴대폰 프레임 | 1024x1024 / 93x93 | ScreenBubbleMiniGame/ScreenBubble_Phone.png | 완료 | ScreenBubble_Phone |
| PhoneFrame/Screen | UISprite (빌트인 placeholder) | 화면 | 1024x1024 | ScreenBubbleMiniGame/ScreenBubble_Phone.png(관련) | placeholder | ScreenBubble_Phone |
| Bubbles/Bubble0~6 | ScreenBubble_Bubble (커스텀, ART_PROMPTS 기록 대비 이미 교체됨) | 기포 | 128x128 / 13x13 | ScreenBubbleMiniGame/ScreenBubble_Bubble.png | 완료 | ScreenBubble_Bubble |
| Card | ScreenBubble_Card (커스텀) | 안내 카드 | 256x256 / 9x9 | ScreenBubbleMiniGame/ScreenBubble_Card.png | 완료 | ScreenBubble_Card |
| Arrow | Kimbap_Arrow (커스텀, 공유) | 안내 화살표 | 128x128 / 18x18 | SamgakKimbapMiniGame/Kimbap_Arrow.png | 완료(공유 자산) | Kimbap_Arrow |
| (씬에 없음) | — | 책상 위 배경 | 1920x1080 | ScreenBubbleMiniGame/BG_ScreenBubble.png | 신규 필요 | BG_ScreenBubble |

## §8. ButtonShirt (`Assets/Scenes/MiniGames/ButtonShirtMiniGame.unity`, `Assets/SampleImage/ButtonShirtMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| Shirt/Body | ButtonShirt_Shirt (커스텀, ART_PROMPTS 기록 대비 이미 교체됨) | 셔츠 몸체 | 1024x1024 / 59x59 | ButtonShirtMiniGame/ButtonShirt_Shirt.png | 완료 | ButtonShirt_Shirt |
| Shirt/PlacketLeft·Right | UISprite (빌트인 placeholder) | 플래킷 | 1024x1024 | ButtonShirtMiniGame/ButtonShirt_Shirt.png(관련) | placeholder | ButtonShirt_Shirt |
| Shirt/Holes/Hole0~4 | ButtonShirt_Hole (커스텀) | 단춧구멍 | 64x64 / 4x4 | ButtonShirtMiniGame/ButtonShirt_Hole.png | 완료 | ButtonShirt_Hole |
| Shirt/Buttons/Button0~4 | ButtonShirt_Button (커스텀) | 단추 | 64x64 / 11x11 | ButtonShirtMiniGame/ButtonShirt_Button.png | 완료 | ButtonShirt_Button |
| Shirt/HintArrow | Kimbap_Arrow (커스텀, 공유) | 안내 화살표 | 128x128 / 5x5 | SamgakKimbapMiniGame/Kimbap_Arrow.png | 완료(공유 자산) | Kimbap_Arrow |
| (씬에 없음) | — | 옷장 거울 앞 배경 | 1920x1080 | ButtonShirtMiniGame/BG_ButtonShirt.png | 신규 필요 | BG_ButtonShirt |

## §9. Vacuum (`Assets/Scenes/MiniGames/VacuumMiniGame.unity`, `Assets/SampleImage/VacuumMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| Floor | Vacuum_Floor (커스텀, ART_PROMPTS 기록 대비 이미 교체됨) | 바닥 | 1024x1024 / 17x17 | VacuumMiniGame/Vacuum_Floor.png | 완료 | Vacuum_Floor |
| Sofa/Body | UISprite (빌트인 placeholder) | 소파 | 1024x1024 | VacuumMiniGame/Vacuum_Floor.png(관련) | placeholder | Vacuum_Floor |
| Vacuum/Head | Vacuum_Head (커스텀) | 청소기 헤드 | 512x512 / 9x9 | VacuumMiniGame/Vacuum_Head.png | 완료 | Vacuum_Head |
| Vacuum/Head/Stick | UISprite (빌트인 placeholder) | 손잡이 대 | 512x512 | VacuumMiniGame/Vacuum_Head.png(관련) | placeholder | Vacuum_Head |
| Vacuum/Head/Stick/Handle | Knob (빌트인 placeholder) | 손잡이 | 512x512 | VacuumMiniGame/Vacuum_Head.png(관련) | placeholder | Vacuum_Head |
| Dust/Speck0~2 | Vacuum_Dust (커스텀) | 먼지 | 64x64 / 4~5 | VacuumMiniGame/Vacuum_Dust.png | 완료 | Vacuum_Dust |
| Arrow | Kimbap_Arrow (커스텀, 공유) | 안내 화살표 | 128x128 / 7x7 | SamgakKimbapMiniGame/Kimbap_Arrow.png | 완료(공유 자산) | Kimbap_Arrow |
| (씬에 없음) | — | 거실 벽면 배경 | 1920x1080 | VacuumMiniGame/BG_Vacuum.png | 신규 필요 | BG_Vacuum |

## §10. FlySwat (`Assets/Scenes/MiniGames/FlySwatMiniGame.unity`, `Assets/SampleImage/FlySwatMiniGame/`, `Assets/Prefabs/FlySwatMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| ResultRoot | Circle (Unity 기본 도형) | 결과 표시 루트 | 128x128 / 324x324 | FlySwatMiniGame/ | placeholder | FX_FlySwat_Success / FX_FlySwat_Fail |
| Manager/Swatter/Head | UISprite (빌트인 placeholder) | 파리채 커서(T8.2 임시) | 512x512 / 9x24 | FlySwatMiniGame/FlySwat_Swatter.png | placeholder | FlySwat_Swatter |
| Background | FlySwat_Background (커스텀) | 방 배경(BG 겸용) | 1920x1080 / 1728x972 | FlySwatMiniGame/FlySwat_Background.png | 완료 | FlySwat_Background |
| 프리팹: Fly.prefab/Fly | FlySwat_Fly_0 (커스텀, 1프레임만) | 파리(런타임 생성) | 128x128 / 97x97 | FlySwatMiniGame/FlySwat_Fly_0.png (2번째 프레임 FlySwat_Fly_1 필요) | 완료(1프레임, 2프레임째 신규 필요) | FlySwat_Fly |

## §11. BossBlob (`Assets/Scenes/MiniGames/BossBlobMiniGame.unity`, `Assets/SampleImage/BossBlobMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| Boss | BossBlob_Boss (커스텀, ART_PROMPTS 기록 대비 이미 교체됨) | 보스 형상 | 1024x1024 / 28x28 | BossBlobMiniGame/BossBlob_Boss.png | 완료 | BossBlob_Boss |
| Ground | UISprite (빌트인 placeholder) | 바닥 | — / 277x14 | BossBlobMiniGame/ | placeholder | —(§2-l 미등재) |
| PlayerDisplay | BossBlob_Player (커스텀) | 플레이어 표시 | 256x256 / 26x26 | BossBlobMiniGame/BossBlob_Player.png | 완료 | BossBlob_Player |
| ResultRoot | Knob (빌트인 placeholder) | 결과 표시 루트 | 1024x1024 / 65x65 | BossBlobMiniGame/ | placeholder | FX_BossBlob_Success / FX_BossBlob_Fail |

## §12. TapTarget (`Assets/Scenes/MiniGames/TapTargetMiniGame.unity`, `Assets/SampleImage/TapTargetMiniGame/`, `Assets/Prefabs/TapTargetMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| ResultRoot | TapTarget_Result (커스텀, ART_PROMPTS 기록 대비 이미 교체됨) | 결과 표시 | 256x256 / 324x324 | TapTargetMiniGame/TapTarget_Result.png | 완료 | TapTarget_Result |
| 프리팹: Target.prefab/Target | TapTarget_Target (커스텀, ART_PROMPTS 기록 대비 이미 교체됨) | 탭 목표(런타임 생성) | 128x128 / 140x140 | TapTargetMiniGame/TapTarget_Target.png | 완료 | TapTarget_Target |
| 프리팹: Decoy.prefab/Decoy | TapTarget_Decoy (커스텀, ART_PROMPTS 기록 대비 이미 교체됨) | 오답 유도(런타임 생성) | 128x128 / 140x140 | TapTargetMiniGame/TapTarget_Decoy.png | 완료 | TapTarget_Decoy |
| (씬에 없음) | — | 게시판·벽 배경 | 1920x1080 | TapTargetMiniGame/BG_TapTarget.png | 신규 필요 | BG_TapTarget |

---

## §13. VacuumWiden (`Assets/Scenes/MiniGames/VacuumWidenMiniGame.unity`, `Assets/SampleImage/VacuumWidenMiniGame/`)

| 오브젝트 경로 | 현재 스프라이트 | 용도 | 권장 원본 / 화면 표시(px) | 저장 경로·파일명 | 상태 | ART_PROMPTS 키 |
|---|---|---|---|---|---|---|
| Floor | Vacuum_Floor (커스텀, 공유) | 바닥 | 1024x1024 | VacuumMiniGame/Vacuum_Floor.png(공유) | 완료(공유 자산) | Vacuum_Floor |
| LeftFurniture | Shape_Square (빌트인 placeholder) | 가구 틈 왼쪽 | 512x512 | VacuumWidenMiniGame/VacuumWiden_Furniture.png | placeholder | VacuumWiden_Furniture |
| RightFurniture | Shape_Square (빌트인 placeholder) | 가구 틈 오른쪽 | 512x512 | VacuumWidenMiniGame/VacuumWiden_Furniture.png(공유·좌우 반전) | placeholder | VacuumWiden_Furniture |
| Head/HeadSprite | Vacuum_Head (커스텀, 공유) | 흡입구 헤드(폭 늘어남) | 512x512 | VacuumMiniGame/Vacuum_Head.png(공유) | 완료(공유 자산, 아래 비고 참고) | Vacuum_Head |
| Head/Stick | Shape_Square (빌트인 placeholder) | 손잡이 대 | 256x256 | VacuumWidenMiniGame/VacuumWiden_Stick.png | placeholder | VacuumWiden_Stick |
| Head/WidthHandle | Shape_Circle (빌트인 placeholder) | 폭 조절 손잡이 | 128x128 | VacuumWidenMiniGame/VacuumWiden_WidthHandle.png | placeholder | VacuumWiden_WidthHandle |
| Dust | Vacuum_Dust (커스텀, 공유) | 먼지 | 64x64 | VacuumMiniGame/Vacuum_Dust.png(공유) | 완료(공유 자산) | Vacuum_Dust |
| Arrow | Shape_Square (빌트인 placeholder) | 안내 화살표(Lv1·2만, Lv3 숨김) | 128x128 | VacuumWidenMiniGame/VacuumWiden_Arrow.png | placeholder | VacuumWiden_Arrow |
| ResultRoot | Circle (Unity 기본 도형, Animator: VacuumWidenResult) | 성공/실패 결과 연출 | 256x256 | VacuumWidenMiniGame/ | placeholder | FX_VacuumWiden_Success / FX_VacuumWiden_Fail |
| (씬에 없음) | — | 가구 틈 배경 | 1920x1080 | VacuumWidenMiniGame/BG_VacuumWiden.png | 신규 필요 | BG_VacuumWiden |

비고:
- **헤드 스트레치 방식**: `VacuumWidenMiniGame.cs`(L181~187 `ApplyWidth`)는 `HeadSprite`의 `transform.localScale.x`를 `width / startWidth` 비율로 균일하게 늘린다(9-slice나 Image 컴포넌트가 아닌 SpriteRenderer 단일 스프라이트 scale). `minWidth 1.2 ~ maxWidth 4` 범위라 최대 약 3.3배까지 가로로 늘어나므로, 단일 실루엣 그림은 늘어날수록 비례가 깨진다. **9-slice(가장자리 고정 중앙만 늘어남) 또는 좌·중·우 3분할 스프라이트(Head_Left/Head_Center/Head_Right를 따로 배치)를 권장** — 다만 현재 스크립트는 단일 `HeadSprite` transform만 조작하므로, 분할 방식을 쓰려면 스크립트 쪽 `ApplyWidth` 로직도 같이 바꿔야 한다(이 문서 범위 밖, 코드 변경 필요 — 참고용으로만 남김). 당장은 가로로 늘어나도 자연스러운 디자인(예: 아코디언 형태의 흡입구, 가로줄 패턴)을 권장.
- **가구 틈 배경**: `LeftFurniture`/`RightFurniture`는 같은 `VacuumWiden_Furniture.png`를 좌우 반전해 공유하는 구조(소파·장식장 다리 등 틈을 사이에 둔 가구 두 쪽). 틈 폭은 `GapZone`(BoxCollider2D, trigger)이 난이도별로 `gapMins/gapMaxs` 범위에서 무작위로 정하므로, 가구 그림은 중앙 모서리 쪽이 수평으로 잘려도 어색하지 않은 디자인이 좋다.
- **Dust**: `Vacuum_Dust.png`를 그대로 공유. 성공 시 `localScale = Vector3.one`, 실패(Narrow) 시 `Vector3.one * 0.5f`로 줄어드는 연출(코드 L320/L338) — 별도 FX 이미지 없이 scale만으로 표현.
- **성공·실패 FX**: `ResultRoot`에 `VacuumWidenResult` Animator(현재 기본 Circle 도형)가 붙어 있다. `isSuccess`/`isFailed` bool로 상태 전환(공용 패턴). 넓어서 실패(Wide)는 "쾅"(가구 흔들림, `ShakeFurniture`), 좁아서 실패(Narrow)는 먼지가 줄어드는 연출로 구분되므로, FX 이미지도 Wide/Narrow 두 종류로 나눠 제작을 권장(§7.4 기존 인계 — Furniture_L/R, Dust, Arrow 네 파일이 아직 없다고 적혀 있었으나 실측 결과 Dust는 Vacuum_Dust 공유로 완료 상태, Furniture/Stick/WidthHandle/Arrow만 신규 필요).
- **현재 placeholder 요약**: `Shape_Square`/`Shape_Circle`(Unity 기본 도형)을 쓰는 LeftFurniture/RightFurniture/Head/Stick/Head/WidthHandle/Arrow, 그리고 기본 `Circle` 도형을 쓰는 ResultRoot — 총 6개 오브젝트가 아직 정식 그림이 아니다. Floor/HeadSprite/Dust 3개는 VacuumMiniGame 리소스를 공유해 이미 완료 상태다.

## §14. 2026-10-02 아트 적용 기록 (Tripo GPT Image 2.5)

§2~§13 표의 `placeholder`·`신규 필요` 행을 생성해 씬에 적용했다. 위 표의 "현재 스프라이트·상태" 칸은 적용 전 실측값이고, 아래가 현재 상태다. 전후 캡처: `Docs/Captures/MiniGames_Art_Before.png`, `MiniGames_Art_After.png`.

### 14-1. 넣은 파일 (Assets/SampleImage/ 아래)

| 게임 | 배경 | 오브젝트 | 성공·실패 연출 |
|---|---|---|---|
| Egg | `EGGMiniGames/BG_Egg_Front` (Canvas/Image 교체) | `Obj_Egg_Chopstick` | `FX_Egg_Success`, `FX_Egg_Fail` (미연결) |
| Pizza | `PizzaMiniGame/BG_Pizza_Front` (Canvas/Image 교체) | — | `FX_Pizza_Success`, `FX_Pizza_Fail` (미연결) |
| USB | `USBMiniGame/BG_USB_Front` (Canvas/BackGroundImage 교체) | — | `FX_USB_Success` (미연결) |
| SamgakKimbap | `BG_SamgakKimbap` | — | `FX_Kimbap_Success`, `FX_Kimbap_Fail` (연결) |
| PeelSticker | `BG_PeelSticker` | `PeelSticker_Spine`, `_PeeledFlap`, `_CornerTab` | — |
| ScreenBubble | `BG_ScreenBubble` | (Screen 은 기본 도형 유지) | — |
| ButtonShirt | `BG_ButtonShirt` | `ButtonShirt_PlacketPlain` | — |
| Vacuum | `BG_Vacuum` | `Vacuum_FloorStrip`, `_Sofa`, `_Nozzle`, `_Stick`, `_Handle` | — |
| FlySwat | (기존 유지) | `FlySwat_Swatter` | `FX_FlySwat_Success`, `FX_FlySwat_FailFly` (연결) |
| BossBlob | (없음, 어두운 단색 유지) | `BossBlob_Ground` | `FX_BossBlob_Success`, `FX_BossBlob_Fail` (연결) |
| TapTarget | `BG_TapTarget` | — | (기존 유지) |
| VacuumWiden | `BG_VacuumWiden_Floor` (기존 Floor 렌더러는 끔) | `VacuumWiden_Furniture`, `_Stick`, `_WidthHandle`, 헤드는 `Vacuum_Nozzle` | `FX_VacuumWiden_Success`, `_Fail_Wide` (연결), `_Fail_Narrow` (미연결) |
| 공용 | — | — | `UI/FX_Common_Success`, `_Fail`, `_Trophy` (미연결) |

- 배경은 씬 루트의 `Background_Art` (SpriteRenderer, Sorting Layer `Object`, Order -100, PPU 108 = 17.78x10 유닛).
- 연출 "연결" = ResultRoot 의 Success/Failed 상태에 게임별 클립(`Assets/Animator/<Game>MiniGame/<Game>_Result_Success|Failed.anim`, TapTarget 클립 복사본 + 스프라이트 키)을 넣었다. FX 텍스처는 PPU = 긴 변(1유닛 크기).
- 미연결 FX: Egg·Pizza·USB 는 결과 연출이 오브젝트 자체 애니메이션이라 넣을 자리가 없다. VacuumWiden 은 실패 상태가 하나라 Wide 만 연결.
- 쓰지 않는 파일(지워도 됨, MCP 삭제가 확인 창 때문에 막혀 남김): `EGGMiniGames/BG_Egg`, `PizzaMiniGame/BG_Pizza`, `USBMiniGame/BG_USB`(위에서 본 시점이라 기존 화면과 안 맞음), `VacuumWidenMiniGame/BG_VacuumWiden`, `ButtonShirtMiniGame/ButtonShirt_Placket`, `ScreenBubbleMiniGame/ScreenBubble_Screen`, `FlySwatMiniGame/FX_FlySwat_Fail`.
- 그대로 둔 placeholder: PeelSticker Line_0~4, ScreenBubble Screen(기본 도형이 맞는 모양), VacuumWiden Arrow(세로 막대 — 만든 `VacuumWiden_Arrow` 는 가로 양방향 화살표라 연결하지 않음), ScreenBubble Arrow.

### 14-2. 크기 바로잡기 (중요)

PeelSticker·ScreenBubble·ButtonShirt·Vacuum·BossBlob 다섯 씬은 Transform scale 이 "스프라이트 = 1유닛" 을 전제로 잡혀 있는데(예: 휴대폰 5.4x9.4, 소파 7x3) 실제 스프라이트가 0.16유닛(UISprite)이라 화면에 1/6 크기로 나오고 있었다. SamgakKimbap 의 Wrapper·Laver 는 반대로 5.12유닛 스프라이트라 30유닛으로 나왔다. **Transform·콜라이더는 그대로 두고** SpriteRenderer 를 `drawMode = Sliced` + `size` 로 맞춰 그림이 Transform scale 크기(월드)에 꼭 맞게 했다.

- 규칙: `size = (원하는 월드 크기 / lossyScale) / (텍스처에서 그림이 차지하는 비율)`.
- **주의**: SpriteRenderer 를 Simple → Sliced 로 바꾸면 Unity 가 겉모습을 유지하려고 Transform localScale 을 자동으로 바꾼다. 바꾼 직후 localScale 을 원래 값으로 되돌리고 `size` 를 다시 넣어야 한다(이번 작업에서 전부 원래 값으로 복원 확인).
- ButtonShirt Body 는 셔츠 그림 비율 때문에 6.5x8.6 으로, Vacuum·ButtonShirt 안내 화살표는 0.6 유닛으로 보이게 했다.
- BossBlob Ground 의 BoxCollider2D 는 0.16 로컬(월드 2.56x0.13)로 그림(16x0.8)보다 훨씬 작다 — 콜라이더는 건드리지 않았으니 플레이로 확인 필요.

### 14-3. 사람이 확인할 것

- 각 미니게임 씬 Play → 오브젝트 크기·터치 영역이 그림과 맞는지(특히 크기를 바로잡은 5개 씬).
- SamgakKimbap·FlySwat·BossBlob·VacuumWiden: 성공/실패 때 가운데 결과 그림이 뜨는지.
- Vacuum: 스크립트가 헤드 색(normalColor/failedColor)을 덧칠한다 — 노즐 그림 색이 어색하면 인스펙터 색을 흰색으로.
- Egg: 젓가락 그림 pivot 이 가운데로 바뀌었다(이전 placeholder 는 아래쪽) — 연출 위치 확인.

## 검증 메모

- 조회 일시: 2026-10-01, 도구: `Unity_RunCommand`(`EditorSceneManager.OpenScene(Additive)`로 11개 씬을 순서대로 열어 `GetComponentsInChildren<SpriteRenderer>(true)`를 수집하고 저장 없이 닫음, 프리팹은 `AssetDatabase.LoadAssetAtPath<GameObject>`로 직접 로드).
- 게임별 MCP 조회 SpriteRenderer 개수 / 문서 행 개수(배경·FX 포함) / 경로 일치 여부:

| 게임 | MCP 조회 개수 | 문서 행 개수(실측 행만) | 경로 일치 |
|---|---|---|---|
| Egg | 3 | 3 | 일치 |
| Pizza | 6 | 6 | 일치 |
| USB | 4 | 4 | 일치 |
| SamgakKimbap | 9 | 9 | 일치 |
| PeelSticker | 11 | 11 | 일치 |
| ScreenBubble | 11 | 11 | 일치 |
| ButtonShirt | 15 | 15 | 일치 |
| Vacuum | 9 | 9 | 일치 |
| FlySwat | 3(씬) + 1(프리팹 Fly) | 4 | 일치 |
| BossBlob | 4 | 4 | 일치 |
| TapTarget | 1(씬) + 2(프리팹 Target/Decoy) | 3 | 일치 |
| VacuumWiden | 9 | 10 | 일치(문서 10행 = 실측 9 + 배경 1 신규행) |

- 프리팹 조회: `Assets/Prefabs/FlySwatMiniGame/Fly.prefab`, `Assets/Prefabs/TapTargetMiniGame/Target.prefab`, `Assets/Prefabs/TapTargetMiniGame/Decoy.prefab` — 3개 모두 SpriteRenderer 1개씩, 전부 커스텀 스프라이트로 이미 교체되어 있음(ART_PROMPTS.md §2-l 기록은 placeholder로 남아 있어 실제와 어긋남 — ART_PROMPTS 갱신은 이 문서 범위 밖이라 반영하지 않음, 다음 문서 작업에서 참고).
- `—(§2-l 미등재)` 키: BossBlob/Ground 1건.
- 화면 표시 px는 `bounds.size × 1080/(orthographicSize×2)`(모든 씬 Main Camera ortho=5, 1unit=108px)로 계산했다. Wrapper·Laver(SamgakKimbap)처럼 bounds가 화면보다 큰 값은 Transform scale이 1보다 큰 placeholder 배치 그대로이며, 실제 생성 시 권장 원본(§2-l) 해상도를 기준으로 Import하고 화면에 보이는 비율은 기존 scale을 유지해 자동 결정된다.
- 이번 작업에서 파일 쓰기는 `Docs/MINIGAME_RESOURCES.md` 신규 생성 1건뿐이며, 씬 조회는 Unity_RunCommand로 읽기만 하고 저장하지 않아 dirty 변경이 없다(Bash 도구 미제공으로 `git status` 명령 자체는 실행하지 못함 — 사람이 최종 `git status`로 재확인 권장).
