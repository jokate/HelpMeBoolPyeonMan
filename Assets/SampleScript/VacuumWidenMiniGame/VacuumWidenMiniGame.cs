using System.Collections;
using BoolPyeon.Input;
using BoolPyeon.MiniGames;
using UnityEngine;

/// <summary>
/// "청소기를 맞춰줘!" 흡입구 폭을 틈 폭에 맞게 좌우로 늘린 뒤 위로 밀어
/// 먼지에 닿으면 성공. 틈 안에서 폭이 너무 넓으면 걸려서 '쾅' 실패,
/// 먼지에 닿았는데 폭이 너무 좁으면 먼지가 남아 실패한다.
/// </summary>
public class VacuumWidenMiniGame : MiniGameBase
{
    private enum Mode
    {
        None,
        Width,
        Push
    }

    private enum FailKind
    {
        None,
        Wide,
        Narrow
    }

    public Transform head;
    public Transform headSprite;
    public Transform widthHandle;
    public Transform leftFurniture;
    public Transform rightFurniture;
    public Transform startPos;
    public Transform gapPos;
    public BoxCollider2D headCollider;
    public BoxCollider2D gapZone;
    public Collider2D dust;
    public GameObject arrow;
    public Animator resultAnimator;

    [Tooltip("난이도 1~3 별 허용 오차(유닛)")]
    public float[] tolerances = { 0.5f, 0.35f, 0.2f };

    [Tooltip("난이도 1~3 별 틈 최소 폭")]
    public float[] gapMins = { 1.6f, 1.4f, 1.2f };

    [Tooltip("난이도 1~3 별 틈 최대 폭")]
    public float[] gapMaxs = { 2.4f, 2.0f, 1.6f };

    public float minWidth = 1.2f;
    public float maxWidth = 4f;
    public float startWidth = 1.2f;
    public float headHeight = 0.6f;
    public float handleGap = 0.75f;

    public Color normalColor = new Color(0.35f, 0.35f, 0.35f, 1f);
    public Color failedColor = new Color(0.85f, 0.2f, 0.2f, 1f);

    protected override Animator ResultAnimator => resultAnimator;

    private Mode mode = Mode.None;
    private FailKind failKind = FailKind.None;
    private float width;
    private float gapWidth;
    private float tolerance;
    private float widthAtGrab;
    private float grabX;
    private SpriteRenderer headRenderer;
    private Vector3 headSpriteBaseScale = Vector3.one;
    private float leftHalfWidth;
    private float rightHalfWidth;

    /// <summary>테스트/디버그용: 현재 흡입구 폭. Runner 검증에서 직접 설정한다.</summary>
    public float Width
    {
        get => width;
        set
        {
            width = Mathf.Clamp(value, minWidth, maxWidth);
            ApplyWidth();
        }
    }

    /// <summary>테스트/디버그용: 이번 난이도의 틈 폭.</summary>
    public float GapWidth => gapWidth;

    /// <summary>테스트/디버그용: 이번 난이도의 허용 오차.</summary>
    public float Tolerance => tolerance;

    /// <summary>헤드 위쪽 끝 지점(먼지·틈 판정 기준점).</summary>
    public Vector3 HeadTop => head != null ? head.position + Vector3.up * (headHeight / 2f) : Vector3.zero;

    /// <summary>테스트/디버그용: 헤드가 틈(GapZone) 안에 있는지.</summary>
    public bool IsInsideGap => gapZone != null && gapZone.OverlapPoint(HeadTop);

    private void Awake()
    {
        if (headSprite != null)
        {
            headRenderer = headSprite.GetComponent<SpriteRenderer>();
            headSpriteBaseScale = headSprite.localScale;
        }
        leftHalfWidth = GetHalfWidth(leftFurniture);
        rightHalfWidth = GetHalfWidth(rightFurniture);
    }

    private static float GetHalfWidth(Transform t)
    {
        if (t == null)
        {
            return 0.5f;
        }
        Renderer r = t.GetComponent<Renderer>();
        return r != null ? r.bounds.extents.x : 0.5f;
    }

    protected override void OnBegin(MiniGameContext ctx)
    {
        int difficulty = Mathf.Clamp(ctx.Difficulty, 1, 3);
        tolerance = tolerances[difficulty - 1];
        gapWidth = Random.Range(gapMins[difficulty - 1], gapMaxs[difficulty - 1]);

        mode = Mode.None;
        failKind = FailKind.None;

        LayoutGap();

        if (head != null && startPos != null)
        {
            Vector3 p = startPos.position;
            p.z = head.position.z;
            head.position = p;
        }

        Width = startWidth;

        if (headRenderer != null)
        {
            headRenderer.color = normalColor;
        }

        if (dust != null)
        {
            dust.transform.localScale = Vector3.one;
            dust.gameObject.SetActive(true);
        }

        if (arrow != null)
        {
            arrow.SetActive(difficulty < 3);
        }
    }

    private void LayoutGap()
    {
        if (gapPos == null || gapZone == null)
        {
            return;
        }
        float cx = gapPos.localPosition.x;

        gapZone.size = new Vector2(gapWidth, gapZone.size.y);
        Vector3 gapZonePos = gapZone.transform.localPosition;
        gapZonePos.x = cx;
        gapZone.transform.localPosition = gapZonePos;

        if (leftFurniture != null)
        {
            Vector3 p = leftFurniture.localPosition;
            p.x = cx - (gapWidth / 2f + leftHalfWidth);
            leftFurniture.localPosition = p;
        }

        if (rightFurniture != null)
        {
            Vector3 p = rightFurniture.localPosition;
            p.x = cx + (gapWidth / 2f + rightHalfWidth);
            rightFurniture.localPosition = p;
        }
    }

    private void ApplyWidth()
    {
        if (headSprite != null)
        {
            Vector3 s = headSpriteBaseScale;
            s.x = headSpriteBaseScale.x * (width / startWidth);
            headSprite.localScale = s;
        }

        if (headCollider != null)
        {
            headCollider.size = new Vector2(Mathf.Max(width, 1.2f), 1.2f);
        }

        if (widthHandle != null)
        {
            Vector3 hp = widthHandle.localPosition;
            hp.x = width / 2f + handleGap;
            widthHandle.localPosition = hp;
        }
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        switch (mode)
        {
            case Mode.None:
                TryGrab();
                break;
            case Mode.Width:
                UpdateWidth();
                break;
            case Mode.Push:
                UpdatePush();
                break;
        }

        CheckState();
    }

    private void TryGrab()
    {
        if (!PointerInput.Down || PointerInput.IsOverUI())
        {
            return;
        }
        RaycastHit2D hit = PointerInput.RaycastWorld2D();
        if (hit.collider == null)
        {
            return;
        }
        if (widthHandle != null && hit.collider.transform == widthHandle)
        {
            mode = Mode.Width;
            widthAtGrab = width;
            grabX = PointerInput.WorldPosition().x;
        }
        else if (head != null && hit.collider.transform == head)
        {
            mode = Mode.Push;
        }
    }

    private void UpdateWidth()
    {
        if (PointerInput.Up || !PointerInput.Held)
        {
            mode = Mode.None;
            return;
        }
        if (!IsInsideGap)
        {
            float worldX = PointerInput.WorldPosition().x;
            Width = widthAtGrab + 2f * (worldX - grabX);
        }
    }

    private void UpdatePush()
    {
        if (PointerInput.Up || !PointerInput.Held)
        {
            mode = Mode.None;
            return;
        }
        if (head == null || startPos == null || gapPos == null)
        {
            return;
        }
        float minY = Mathf.Min(startPos.position.y, gapPos.position.y);
        float maxY = Mathf.Max(startPos.position.y, gapPos.position.y);
        Vector3 world = PointerInput.WorldPosition();
        Vector3 p = head.position;
        p.y = Mathf.Clamp(world.y, minY, maxY);
        head.position = p;
    }

    private void CheckState()
    {
        if (!IsRunning || headCollider == null)
        {
            return;
        }

        if (IsInsideGap && width > gapWidth + tolerance)
        {
            failKind = FailKind.Wide;
            Resolve(MiniGameResult.Failed);
            return;
        }

        bool touchingDust = dust != null && dust.gameObject.activeInHierarchy && dust.OverlapPoint(HeadTop);

        if (touchingDust && width < gapWidth - tolerance)
        {
            failKind = FailKind.Narrow;
            Resolve(MiniGameResult.Failed);
            return;
        }

        if (touchingDust)
        {
            failKind = FailKind.None;
            Resolve(MiniGameResult.Success);
        }
    }

    protected override void OnResolved(MiniGameResult result)
    {
        base.OnResolved(result);

        if (result == MiniGameResult.Success)
        {
            if (dust != null)
            {
                dust.transform.localScale = Vector3.zero;
            }
        }
        else if (result == MiniGameResult.Failed)
        {
            if (headRenderer != null)
            {
                headRenderer.color = failedColor;
            }

            if (failKind == FailKind.Wide)
            {
                StartCoroutine(ShakeFurniture());
            }
            else if (failKind == FailKind.Narrow)
            {
                if (dust != null)
                {
                    dust.transform.localScale = Vector3.one * 0.5f;
                }
            }
        }
    }

    private IEnumerator ShakeFurniture()
    {
        Vector3 leftBase = leftFurniture != null ? leftFurniture.localPosition : Vector3.zero;
        Vector3 rightBase = rightFurniture != null ? rightFurniture.localPosition : Vector3.zero;
        float elapsed = 0f;
        const float duration = 0.3f;
        while (elapsed < duration)
        {
            float offset = Mathf.Sin(elapsed * 50f) * 0.05f;
            if (leftFurniture != null)
            {
                leftFurniture.localPosition = leftBase + new Vector3(offset, 0f, 0f);
            }
            if (rightFurniture != null)
            {
                rightFurniture.localPosition = rightBase + new Vector3(offset, 0f, 0f);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (leftFurniture != null)
        {
            leftFurniture.localPosition = leftBase;
        }
        if (rightFurniture != null)
        {
            rightFurniture.localPosition = rightBase;
        }
    }
}
