using System.Collections;
using BoolPyeon.Input;
using BoolPyeon.Input.Interaction;
using BoolPyeon.MiniGames;
using UnityEngine;

/// <summary>
/// "청소기를 넣어줘!" 손잡이를 돌려 청소기 대를 눕힌 뒤 헤드를 소파 밑 틈으로 밀어
/// 먼지에 닿으면 성공. 대를 덜 눕힌 채(허용각보다 세운 채) 틈에 밀어 넣으면 '쾅' 실패.
/// </summary>
public class VacuumMiniGame : MiniGameBase
{
    private enum Mode
    {
        None,
        Rotate,
        Drag
    }

    public RotateInteraction rotate;

    public Transform head;
    public Transform stick;
    public Transform handle;
    public Collider2D sofaEdge;
    public Collider2D dust;
    public Transform sofaBody;
    public GameObject arrow;
    public Animator resultAnimator;
    public Transform startPos;
    public Transform dustPos;

    [Tooltip("난이도 1~3 별 허용 기울기(도). 이 이하로 눕혀야 틈을 통과한다")]
    public float[] allowedTilts = { 40f, 30f, 20f };

    [Tooltip("난이도 1~3 별 소파 틈 높이 표현(연출용)")]
    public float[] sofaGapYs = { 1.2f, 0.9f, 0.6f };

    public float startTilt = 75f;
    public float minTilt = 5f;
    public float maxTilt = 85f;

    public Color normalColor = new Color(0.35f, 0.35f, 0.35f, 1f);
    public Color failedColor = new Color(0.85f, 0.2f, 0.2f, 1f);

    protected override Animator ResultAnimator => resultAnimator;

    private Mode mode = Mode.None;
    private float tiltAtGrab;
    private Collider2D headCollider;
    private SpriteRenderer headRenderer;
    private Vector3 sofaBodyBaseScale = Vector3.one;
    private Vector3 sofaBodyBasePos;
    private float allowedTilt;

    /// <summary>테스트/디버그용: 현재 기울기(도). Runner 검증에서 직접 설정한다.</summary>
    public float Tilt
    {
        get => tilt;
        set
        {
            tilt = Mathf.Clamp(value, minTilt, maxTilt);
            ApplyStickRotation();
        }
    }
    private float tilt;

    /// <summary>테스트/디버그용: 이번 난이도의 허용 기울기(도).</summary>
    public float AllowedTilt => allowedTilt;

    /// <summary>테스트/디버그용: 헤드가 소파 틈(SofaEdge) 안에 있는지.</summary>
    public bool IsInsideGap => head != null && sofaEdge != null && sofaEdge.OverlapPoint(head.position);

    private void Awake()
    {
        if (head != null)
        {
            headCollider = head.GetComponent<Collider2D>();
            headRenderer = head.GetComponent<SpriteRenderer>();
        }
        if (sofaBody != null)
        {
            sofaBodyBaseScale = sofaBody.localScale;
            sofaBodyBasePos = sofaBody.localPosition;
        }
        if (rotate != null && rotate.center == null && head != null)
        {
            rotate.center = head;
        }
    }

    protected override void OnBegin(MiniGameContext ctx)
    {
        int difficulty = Mathf.Clamp(ctx.Difficulty, 1, 3);
        allowedTilt = allowedTilts[difficulty - 1];
        float gapY = sofaGapYs[difficulty - 1];

        mode = Mode.None;
        rotate?.Reset();

        tilt = startTilt;
        ApplyStickRotation();

        if (head != null && startPos != null)
        {
            Vector3 p = startPos.position;
            p.z = head.position.z;
            head.position = p;
        }

        if (headRenderer != null)
        {
            headRenderer.color = normalColor;
        }

        if (sofaBody != null)
        {
            sofaBody.localPosition = sofaBodyBasePos;
            float refGap = sofaGapYs[0] != 0f ? sofaGapYs[0] : 1f;
            sofaBody.localScale = new Vector3(
                sofaBodyBaseScale.x,
                sofaBodyBaseScale.y * (gapY / refGap),
                sofaBodyBaseScale.z);
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
            case Mode.Rotate:
                UpdateRotate();
                break;
            case Mode.Drag:
                UpdateDrag();
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
        if (handle != null && hit.collider.transform == handle)
        {
            mode = Mode.Rotate;
            tiltAtGrab = tilt;
            rotate?.Reset();
            rotate?.Tick();
        }
        else if (head != null && hit.collider.transform == head)
        {
            mode = Mode.Drag;
        }
    }

    private void UpdateRotate()
    {
        if (PointerInput.Up || !PointerInput.Held)
        {
            mode = Mode.None;
            rotate?.Reset();
            return;
        }
        rotate?.Tick();
        if (rotate != null)
        {
            Tilt = tiltAtGrab + rotate.AccumulatedAngle;
        }
    }

    private void UpdateDrag()
    {
        if (PointerInput.Up || !PointerInput.Held)
        {
            mode = Mode.None;
            return;
        }
        if (head == null || startPos == null || dustPos == null)
        {
            return;
        }
        float minX = Mathf.Min(startPos.position.x, dustPos.position.x);
        float maxX = Mathf.Max(startPos.position.x, dustPos.position.x);
        Vector3 world = PointerInput.WorldPosition();
        Vector3 p = head.position;
        p.x = Mathf.Clamp(world.x, minX, maxX);
        head.position = p;
    }

    private void ApplyStickRotation()
    {
        if (stick != null)
        {
            stick.localRotation = Quaternion.Euler(0f, 0f, tilt - 90f);
        }
    }

    private void CheckState()
    {
        if (!IsRunning || headCollider == null)
        {
            return;
        }

        if (IsInsideGap && tilt > allowedTilt)
        {
            Resolve(MiniGameResult.Failed);
            return;
        }

        if (dust != null && dust.gameObject.activeInHierarchy && dust.OverlapPoint(head.position))
        {
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
            if (sofaBody != null)
            {
                StartCoroutine(ShakeSofa());
            }
        }
    }

    private IEnumerator ShakeSofa()
    {
        Vector3 basePos = sofaBody.localPosition;
        float elapsed = 0f;
        const float duration = 0.3f;
        while (elapsed < duration)
        {
            float offset = Mathf.Sin(elapsed * 50f) * 0.05f;
            sofaBody.localPosition = basePos + new Vector3(offset, 0f, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        sofaBody.localPosition = basePos;
    }
}
