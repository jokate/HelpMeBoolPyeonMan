using System.Collections;
using UnityEngine;
using BoolPyeon.Battle;

namespace BoolPyeon.BattleFlow
{
    /// <summary>
    /// 주인공·몬스터 하나의 월드 표시. 자식 TextMesh 에 이름·HP·예고를 쓴다.
    /// 몬스터는 Collider2D 로 탭 대상이 된다.
    /// </summary>
    public class BattleUnitView : MonoBehaviour
    {
        public SpriteRenderer body;
        public TextMesh label;
        public GameObject selectionMarker;
        public SpriteRenderer intentIcon;
        public TextMesh intentText;

        [Header("코믹 오버레이 (UI_DESIGN §24)")]
        [Tooltip("발밑 이름·HP 판(Sliced). label 뒤에 깔린다.")]
        public SpriteRenderer labelPlate;
        [Tooltip("머리 위 예고 아이콘 뒤의 외침 말풍선")]
        public SpriteRenderer intentBase;
        [Tooltip("EnemyIntentKind 순서(Attack, Special, Wait, Guard, Magic, Buff, Heal)대로 예고 아이콘. 비면 색 원으로 대체")]
        public Sprite[] intentSprites = new Sprite[7];
        public Sprite[] hitFxFrames;
        public Sprite[] healFxFrames;
        public Sprite damageSplash;

        [Header("액션 애니메이션")]
        [Tooltip("공격 프레임(주인공 3프레임, 몬스터는 SetActionSprites 로 1장)")]
        public Sprite[] attackFrames;
        public Sprite[] defendFrames;
        public Sprite hitSprite;
        public Sprite[] skillFrames;
        [Tooltip("공격할 때 앞으로 나가는 거리(+ 오른쪽, - 왼쪽)")]
        public float lungeDistance = 0.5f;
        public float frameTime = 0.09f;

        public Combatant Unit { get; private set; }

        private Color baseColor = Color.white;
        private Coroutine flashRoutine;
        private EnemyIntent lastIntent;
        private string lastTargetName;

        // T6: body 의 원래(씬에 배치된) 색·스프라이트. ApplyLook 이 매번 여기서부터 다시 계산해 틴트가 누적되지 않게 한다.
        private Color originalColor = Color.white;
        private Sprite originalSprite;
        private bool hasOriginalLook;

        // 스프라이트의 실제 그림 영역(투명 여백 제외)을 이 오브젝트 로컬 좌표로 잰 값. 오버레이 배치 기준.
        private float headY = 2f;
        private float footY;
        private float centerX;
        private float bodyWidth = 1.5f;
        private Sprite standSprite;
        private Vector3 homePos;
        private bool homeSaved;
        private Coroutine actionRoutine;
        private TextMesh intentShadow;

        static readonly Color Ink = new Color(0.17f, 0.15f, 0.13f);
        static readonly Color IntentRed = new Color(0.84f, 0.27f, 0.27f);

        private void Awake()
        {
            // 예고 숫자가 아이콘 크기 조정에 끌려가지 않도록 아이콘 밑에서 꺼내 둔다
            if (intentText != null && intentIcon != null && intentText.transform.IsChildOf(intentIcon.transform))
            {
                intentText.transform.SetParent(transform, false);
                intentText.transform.localScale = Vector3.one;
            }
        }

        public void Bind(Combatant unit)
        {
            Unit = unit;
            gameObject.SetActive(unit != null);
            if (body != null)
            {
                CaptureOriginalLook();
                baseColor = body.color;
                standSprite = body.sprite;
            }
            if (!homeSaved)
            {
                homePos = transform.localPosition;
                homeSaved = true;
            }
            StopAction();
            MeasureBody();
            LayoutOverlays();
            SetSelected(false);
            Refresh(null);
        }

        // T6: 스쿼드 멤버의 외형(스프라이트·공격/방어/피격/스킬 프레임)을 def 로 바꾼다. Bind(Combatant) 전에 부른다.
        public void SetMemberLook(CharacterDefinition def)
        {
            if (def == null) return;
            if (def.standSprite != null) ApplyLook(Color.white, def.standSprite);
            if (def.attackFrames != null && def.attackFrames.Length > 0) attackFrames = def.attackFrames;
            if (def.defendFrames != null && def.defendFrames.Length > 0) defendFrames = def.defendFrames;
            if (def.hitSprite != null) hitSprite = def.hitSprite;
            skillFrames = (def.skillFrames != null && def.skillFrames.Length > 0) ? def.skillFrames : null;
        }

        // 몬스터 정의의 공격·피격 스프라이트. Bind 전에 부른다(ApplyLook 과 같이).
        public void SetActionSprites(Sprite attack, Sprite hit)
        {
            attackFrames = attack != null ? new[] { attack } : null;
            hitSprite = hit;
        }

        private void MeasureBody()
        {
            headY = 2f; footY = 0f; centerX = 0f; bodyWidth = 1.5f;
            if (body == null || body.sprite == null) return;
            Vector2[] verts = body.sprite.vertices;
            if (verts == null || verts.Length == 0) return;
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            foreach (Vector2 v in verts)
            {
                Vector3 p = transform.InverseTransformPoint(body.transform.TransformPoint(v));
                minX = Mathf.Min(minX, p.x); maxX = Mathf.Max(maxX, p.x);
                minY = Mathf.Min(minY, p.y); maxY = Mathf.Max(maxY, p.y);
            }
            headY = maxY; footY = minY; centerX = (minX + maxX) * 0.5f; bodyWidth = maxX - minX;
        }

        // 세로 스택: 발밑 선택 링 → 몸 → (발 아래) 이름·HP 판, 머리 위에는 예고 말풍선만 둔다(§20.2 겹침 해소)
        private void LayoutOverlays()
        {
            if (selectionMarker != null)
            {
                selectionMarker.transform.localPosition = new Vector3(centerX, footY + 0.05f, 0f);
                var ring = selectionMarker.GetComponent<SpriteRenderer>();
                if (ring != null && ring.sprite != null) FitWidth(selectionMarker.transform, ring.sprite, Mathf.Clamp(bodyWidth * 0.9f, 1.2f, 3f));
            }
            if (label != null)
            {
                label.anchor = TextAnchor.UpperCenter;
                label.alignment = TextAlignment.Center;
                label.color = Ink;
                label.transform.localPosition = new Vector3(centerX, footY - 0.2f, -0.02f);
            }
            float intentY = headY + 0.7f;
            if (intentBase != null)
            {
                intentBase.transform.localPosition = new Vector3(centerX, intentY, 0f);
                if (intentBase.sprite != null) FitHeight(intentBase.transform, intentBase.sprite, 1.15f);
            }
            if (intentIcon != null) intentIcon.transform.localPosition = new Vector3(centerX, intentY + 0.03f, -0.01f);
            if (intentText != null)
            {
                intentText.anchor = TextAnchor.MiddleLeft;
                intentText.fontStyle = FontStyle.Bold;
                intentText.transform.localPosition = new Vector3(centerX + 0.45f, intentY - 0.38f, -0.03f);
            }
        }

        private static void FitHeight(Transform t, Sprite s, float height)
        {
            float h = s.bounds.size.y;
            if (h > 0f) t.localScale = Vector3.one * (height / h);
        }

        private static void FitWidth(Transform t, Sprite s, float width)
        {
            float w = s.bounds.size.x;
            if (w > 0f) t.localScale = Vector3.one * (width / w);
        }

        private void CaptureOriginalLook()
        {
            if (hasOriginalLook || body == null) return;
            originalColor = body.color;
            originalSprite = body.sprite;
            hasOriginalLook = true;
        }

        // T6: 몬스터 정의의 tint·spriteOverride 를 원래 모습 위에 적용한다. Bind 보다 먼저 불러야 한다.
        public void ApplyLook(Color tint, Sprite sprite)
        {
            if (body == null) return;
            CaptureOriginalLook();
            body.color = originalColor * tint;
            body.sprite = sprite != null ? sprite : originalSprite;
        }

        public void Refresh(EnemyIntent intent, string intentTargetName = null)
        {
            if (Unit == null)
            {
                return;
            }
            lastIntent = intent;
            lastTargetName = intentTargetName;
            bool useIconForIntent = intentIcon != null && intentText != null;
            if (label != null)
            {
                string line = $"{Unit.Name}\nHP {Unit.Hp}/{Unit.MaxHp}";
                if (Unit.IsGuarding) line += "\n[방어]";
                if (!Unit.IsDead && Unit.HasStatus(StatusEffectType.Guarded)) line += "\n[방어막]";
                if (!useIconForIntent && !Unit.IsDead && intent != null) line += "\n" + IntentText(intent) + TargetSuffix(intent, intentTargetName);
                if (!Unit.IsDead && Unit.Statuses.Count > 0) line += "\n" + StatusesText(Unit);
                if (Unit.IsDead) line = $"{Unit.Name}\n쓰러짐";
                label.text = line;
                ResizeLabelPlate(line);
            }
            RefreshIntentIcon(intent);
            if (body != null && flashRoutine == null)
            {
                if (Unit.IsDead)
                {
                    body.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                }
                else if (Unit.HasStatus(StatusEffectType.Guarded))
                {
                    body.color = Color.Lerp(baseColor, new Color(0.3f, 0.7f, 1f), 0.35f);
                }
                else
                {
                    body.color = baseColor;
                }
            }
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = !Unit.IsDead;
            }
            if (Unit.IsDead)
            {
                SetSelected(false);
            }
        }

        // 이름·HP 판 크기를 글자 줄 수·길이에 맞춘다(판은 0.5배로 줄여 잉크 테두리를 얇게 쓴다)
        private void ResizeLabelPlate(string text)
        {
            if (labelPlate == null || label == null) return;
            string[] lines = text.Split('\n');
            // 글자 칸 높이 ≈ characterSize*fontSize*0.1. 한글은 그 폭 그대로, 숫자·영문·기호는 절반 남짓으로 센다.
            float charH = label.characterSize * label.fontSize * 0.1f;
            float widest = 0f;
            foreach (var l in lines)
            {
                float w = 0f;
                foreach (char c in l) w += c >= 0x1100 ? 1f : 0.55f;
                widest = Mathf.Max(widest, w);
            }
            Vector2 size = new Vector2(Mathf.Max(1.3f, widest * charH + 0.45f), lines.Length * charH * 1.1f + 0.3f);
            const float k = 0.5f;
            labelPlate.drawMode = SpriteDrawMode.Sliced;
            labelPlate.transform.localScale = Vector3.one * k;
            labelPlate.size = size / k;
            labelPlate.transform.localPosition = new Vector3(centerX, footY - 0.2f - size.y * 0.5f + 0.12f, 0f);
        }

        // 머리 위 행동 예고 아이콘: 없거나(intentIcon/intentText 미연결) 죽은 유닛이면 끈다
        private void RefreshIntentIcon(EnemyIntent intent)
        {
            if (intentIcon == null || intentText == null) return;
            bool show = Unit != null && !Unit.IsDead && intent != null && intent.Kind != EnemyIntentKind.Wait;
            intentIcon.gameObject.SetActive(show);
            intentText.gameObject.SetActive(show);
            if (intentBase != null) intentBase.gameObject.SetActive(show);
            if (!show) return;
            int k = (int)intent.Kind;
            Sprite icon = intentSprites != null && k < intentSprites.Length ? intentSprites[k] : null;
            if (icon != null)
            {
                intentIcon.sprite = icon;
                intentIcon.color = Color.white;
                FitHeight(intentIcon.transform, icon, 0.58f);
                intentText.text = IntentShort(intent) + TargetSuffix(intent, lastTargetName);
                bool harmful = intent.Kind == EnemyIntentKind.Attack || intent.Kind == EnemyIntentKind.Special || intent.Kind == EnemyIntentKind.Magic;
                intentText.color = harmful ? IntentRed : Ink;
                UpdateIntentShadow();
                return;
            }
            intentIcon.color = intent.Kind switch
            {
                EnemyIntentKind.Attack => Color.red,
                EnemyIntentKind.Special => Color.magenta,
                EnemyIntentKind.Guard => new Color(0.3f, 0.7f, 1f),
                EnemyIntentKind.Magic => new Color(0.6f, 0.3f, 1f),
                EnemyIntentKind.Buff => new Color(1f, 0.6f, 0.1f),
                EnemyIntentKind.Heal => new Color(0.3f, 0.9f, 0.3f),
                _ => Color.gray
            };
            intentText.text = IntentShort(intent) + TargetSuffix(intent, lastTargetName);
            UpdateIntentShadow();
        }

        // T7: 공격·특수·마법 예고에 대상 멤버 이름을 붙인다(예: "5→요리사"). 이름이 없으면 빈 문자열.
        static string TargetSuffix(EnemyIntent intent, string targetName)
        {
            if (intent == null || string.IsNullOrEmpty(targetName)) return "";
            bool harmful = intent.Kind == EnemyIntentKind.Attack || intent.Kind == EnemyIntentKind.Special || intent.Kind == EnemyIntentKind.Magic;
            return harmful ? "→" + targetName : "";
        }

        // TextMesh 는 외곽선이 없어 흰 사본을 살짝 비켜 깔아 배경 위에서도 읽히게 한다
        private void UpdateIntentShadow()
        {
            if (intentText == null) return;
            if (intentShadow == null)
            {
                var go = new GameObject("IntentTextShadow");
                go.transform.SetParent(intentText.transform, false);
                go.transform.localPosition = new Vector3(0.03f, -0.03f, 0.01f);
                intentShadow = go.AddComponent<TextMesh>();
                intentShadow.font = intentText.font;
                var mr = go.GetComponent<MeshRenderer>();
                var src = intentText.GetComponent<MeshRenderer>();
                if (src != null)
                {
                    mr.sharedMaterial = src.sharedMaterial;
                    mr.sortingLayerID = src.sortingLayerID;
                    mr.sortingOrder = src.sortingOrder - 1;
                }
            }
            intentShadow.text = intentText.text;
            intentShadow.characterSize = intentText.characterSize;
            intentShadow.fontSize = intentText.fontSize;
            intentShadow.fontStyle = intentText.fontStyle;
            intentShadow.anchor = intentText.anchor;
            intentShadow.color = Color.white;
        }

        private Vector3 BodyCenterWorld()
        {
            return transform.TransformPoint(new Vector3(centerX, (headY + footY) * 0.5f, 0f));
        }

        public void ShowDamage(int amount, Color color)
        {
            if (amount <= 0) return;
            SpriteFx.Play(hitFxFrames, BodyCenterWorld() + new Vector3(0f, 0f, -0.1f), 1.6f, 11);
            Vector3 pos = transform.TransformPoint(new Vector3(centerX, headY + 0.1f, -0.2f));
            bool big = Unit != null && Unit.MaxHp > 0 && amount >= Unit.MaxHp * 0.2f;
            DamagePopup.Spawn(pos, amount.ToString(), color, damageSplash, big ? 1.3f : 1f);
        }

        public void ShowHeal(int amount)
        {
            if (amount <= 0) return;
            SpriteFx.Play(healFxFrames, BodyCenterWorld() + new Vector3(0f, 0.3f, -0.1f), 1.8f, 11, 10f);
            Vector3 pos = transform.TransformPoint(new Vector3(centerX, headY + 0.1f, -0.2f));
            DamagePopup.Spawn(pos, "+" + amount, new Color(0.3f, 0.9f, 0.3f));
        }

        // ---- 액션 애니메이션 --------------------------------------------------------

        public void PlayAttack()
        {
            StartAction(AttackRoutine(attackFrames));
        }

        public void PlayDefend()
        {
            if (defendFrames == null || defendFrames.Length == 0) return;
            StartAction(FramesRoutine(defendFrames, 0.25f));
        }

        public void PlayHit()
        {
            StartAction(HitRoutine());
        }

        // T5: 스킬 프레임이 3장 이상이면 공격과 같은 방식으로 재생하고, 없으면 PlayAttack 으로 대체한다.
        public void PlaySkill()
        {
            if (skillFrames != null && skillFrames.Length >= 3)
            {
                StartAction(AttackRoutine(skillFrames));
            }
            else
            {
                PlayAttack();
            }
        }

        private void StartAction(IEnumerator routine)
        {
            if (!isActiveAndEnabled || body == null) return;
            StopAction();
            actionRoutine = StartCoroutine(routine);
        }

        private void StopAction()
        {
            if (actionRoutine != null)
            {
                StopCoroutine(actionRoutine);
                actionRoutine = null;
            }
            if (homeSaved) transform.localPosition = homePos;
            if (body != null && standSprite != null) body.sprite = standSprite;
        }

        private IEnumerator AttackRoutine(Sprite[] frames)
        {
            int n = frames != null ? frames.Length : 0;
            Vector3 forward = homePos + new Vector3(lungeDistance, 0f, 0f);
            if (n >= 3)
            {
                body.sprite = frames[0];
                yield return new WaitForSeconds(frameTime);
                body.sprite = frames[1];
                transform.localPosition = forward;
                yield return new WaitForSeconds(frameTime * 2f);
                body.sprite = frames[2];
                transform.localPosition = Vector3.Lerp(homePos, forward, 0.5f);
                yield return new WaitForSeconds(frameTime);
            }
            else
            {
                if (n > 0 && frames[0] != null) body.sprite = frames[0];
                float t = 0f;
                while (t < 0.12f)
                {
                    t += Time.deltaTime;
                    transform.localPosition = Vector3.Lerp(homePos, forward, t / 0.12f);
                    yield return null;
                }
                yield return new WaitForSeconds(0.18f);
            }
            transform.localPosition = homePos;
            body.sprite = standSprite;
            actionRoutine = null;
        }

        private IEnumerator FramesRoutine(Sprite[] frames, float hold)
        {
            foreach (var f in frames)
            {
                if (f != null) body.sprite = f;
                yield return new WaitForSeconds(frameTime);
            }
            yield return new WaitForSeconds(hold);
            body.sprite = standSprite;
            actionRoutine = null;
        }

        private IEnumerator HitRoutine()
        {
            if (hitSprite != null) body.sprite = hitSprite;
            float t = 0f;
            while (t < 0.28f)
            {
                t += Time.deltaTime;
                float shake = Mathf.Sin(t * 70f) * 0.08f * (1f - t / 0.28f);
                transform.localPosition = homePos + new Vector3(shake, 0f, 0f);
                yield return null;
            }
            transform.localPosition = homePos;
            body.sprite = standSprite;
            actionRoutine = null;
        }

        // 머리 위 아이콘 옆 짧은 글자. 폰트에 확실히 있는 한글·숫자만 쓴다
        public static string IntentShort(EnemyIntent intent)
        {
            int magicAmount = MagicOrHealAmount(intent);
            switch (intent.Kind)
            {
                case EnemyIntentKind.Attack: return intent.Damage.ToString();
                case EnemyIntentKind.Special: return intent.Damage + "!";
                case EnemyIntentKind.Magic: return "마" + magicAmount;
                case EnemyIntentKind.Guard: return "방";
                case EnemyIntentKind.Buff: return "강";
                case EnemyIntentKind.Heal: return "회+" + magicAmount;
                default: return "";
            }
        }

        static int MagicOrHealAmount(EnemyIntent intent)
        {
            float mul = intent.Action != null && intent.Action.damageMultiplier > 0f ? intent.Action.damageMultiplier : 1f;
            return Mathf.CeilToInt(intent.Damage * mul);
        }

        public static string IntentText(EnemyIntent intent)
        {
            string id = intent.Action != null && !string.IsNullOrEmpty(intent.Action.id) ? $" [{intent.Action.id}]" : "";
            switch (intent.Kind)
            {
                case EnemyIntentKind.Attack: return $"공격 {intent.Damage}{id}";
                case EnemyIntentKind.Special: return $"특수 {intent.Damage}!{id}";
                case EnemyIntentKind.Guard: return $"방어 태세{id}";
                case EnemyIntentKind.Magic: return $"마법 {MagicOrHealAmount(intent)}{id}";
                case EnemyIntentKind.Buff: return $"강화{id}";
                case EnemyIntentKind.Heal: return $"회복{id}";
                default: return "대기";
            }
        }

        // 상태이상 목록: 타입 한글 이름 + 남은 턴
        public static string StatusesText(Combatant unit)
        {
            string result = "";
            foreach (var s in unit.Statuses)
            {
                if (result.Length > 0) result += " ";
                result += $"{StatusLabel(s.Type)} {s.RemainingTurns}";
            }
            return result;
        }

        static string StatusLabel(StatusEffectType type)
        {
            switch (type)
            {
                case StatusEffectType.Stun: return "기절";
                case StatusEffectType.NoEnergyReserve: return "수집봉인";
                case StatusEffectType.DamageTakenUp: return "받는피해↑";
                case StatusEffectType.Weaken: return "약화";
                case StatusEffectType.Rush: return "재촉";
                case StatusEffectType.CostUp: return "코스트↑";
                case StatusEffectType.Burn: return "화상";
                case StatusEffectType.Guarded: return "방어막";
                case StatusEffectType.AttackUp: return "공격↑";
                default: return type.ToString();
            }
        }

        public void SetSelected(bool selected)
        {
            if (selectionMarker != null)
            {
                selectionMarker.SetActive(selected);
            }
        }

        public void Flash(Color color)
        {
            if (body == null || !isActiveAndEnabled)
            {
                return;
            }
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(FlashRoutine(color));
        }

        private IEnumerator FlashRoutine(Color color)
        {
            body.color = color;
            yield return new WaitForSeconds(0.15f);
            flashRoutine = null;
            Refresh(lastIntent, lastTargetName);
        }
    }
}
