using BoolPyeon.Input;
using BoolPyeon.MiniGames;
using UnityEngine;

/// <summary>
/// "깔끔하게 떼줘!" 책에 붙은 스티커의 말린 모서리(cornerTab)를 잡고 오른쪽 위 대각선으로
/// 속도 한계 안에서 끝까지 당기면 성공. 너무 빨리 당기면 찢어져 반쪽이 남는다.
/// 시간초과는 Runner 가 처리한다. 손을 떼도 진행도는 남고 다시 잡을 수 있다.
/// </summary>
public class PeelStickerMiniGame : MiniGameBase
{
    public Transform cornerTab;
    public SpriteRenderer stickerBody;
    public SpriteRenderer peeledFlap;
    public GameObject tornHalf;
    public GameObject arrow;
    public Animator resultAnimator;

    [Tooltip("난이도 1~3 별 떼는 거리(월드 단위)")]
    public float[] peelDistances = { 3f, 3.5f, 4f };

    [Tooltip("난이도 1~3 별 속도 한계(월드 단위/초). 이 값을 넘겨 당기면 실패.")]
    public float[] speedLimits = { 10f, 8f, 6f };

    [Tooltip("속도 샘플 창(초). 삼각김밥과 같은 방식.")]
    public float sampleWindow = 0.1f;

    public Color tornColor = new Color(0.55f, 0.5f, 0.4f);

    private static readonly Vector2 Axis = new Vector2(1f, 1f).normalized;

    private Vector3 startPos;
    private float peelDistance;
    private float speedLimit;
    private float pulled;
    private bool holding;
    private Vector2 sampleStartPos;
    private float sampleElapsed;
    private float currentSpeed;
    private float grabOffset;

    protected override Animator ResultAnimator => resultAnimator;

    protected override void OnBegin(MiniGameContext ctx)
    {
        int difficulty = Mathf.Clamp(ctx.Difficulty, 1, 3);
        speedLimit = speedLimits[difficulty - 1];
        peelDistance = peelDistances[difficulty - 1];
        bool showArrow = difficulty < 3;

        pulled = 0f;
        holding = false;
        if (cornerTab != null)
        {
            startPos = cornerTab.position;
            cornerTab.gameObject.SetActive(true);
        }
        if (arrow != null)
        {
            arrow.SetActive(showArrow);
        }
        if (tornHalf != null)
        {
            tornHalf.SetActive(false);
        }
        UpdateVisual(0f);
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        if (!holding)
        {
            if (!PointerInput.Down)
            {
                return;
            }
            RaycastHit2D hit = PointerInput.RaycastWorld2D();
            if (hit.collider == null || cornerTab == null)
            {
                return;
            }
            if (hit.transform != cornerTab && !hit.transform.IsChildOf(cornerTab))
            {
                return;
            }
            holding = true;
            grabOffset = Vector2.Dot((Vector2)PointerInput.WorldPosition() - (Vector2)cornerTab.position, Axis);
            sampleStartPos = cornerTab.position;
            sampleElapsed = 0f;
            currentSpeed = 0f;
            return;
        }

        if (PointerInput.Up || !PointerInput.Held)
        {
            holding = false;
            return;
        }

        Vector3 world = PointerInput.WorldPosition();
        float proj = Mathf.Clamp(Vector2.Dot((Vector2)world - (Vector2)startPos, Axis) - grabOffset, 0f, peelDistance);
        cornerTab.position = startPos + (Vector3)(Axis * proj);

        sampleElapsed += Time.deltaTime;
        if (sampleElapsed >= sampleWindow)
        {
            Vector2 now = cornerTab.position;
            currentSpeed = (now - sampleStartPos).magnitude / sampleElapsed;
            sampleStartPos = now;
            sampleElapsed = 0f;
        }

        ApplyPeel(proj, currentSpeed);
    }

    /// <summary>
    /// 판정 핵심 로직. 속도가 한계를 넘으면 실패, peelDistance 에 닿으면 성공.
    /// RunCommand 로도 직접 호출할 수 있다.
    /// </summary>
    public bool ApplyPeel(float distance, float speed)
    {
        if (!IsRunning)
        {
            return false;
        }
        if (speed > speedLimit)
        {
            Resolve(MiniGameResult.Failed);
            return false;
        }

        pulled = Mathf.Clamp(distance, 0f, peelDistance);
        UpdateVisual(pulled / peelDistance);

        if (pulled >= peelDistance - 0.001f)
        {
            holding = false;
            Resolve(MiniGameResult.Success);
        }
        return true;
    }

    private void UpdateVisual(float progress)
    {
        progress = Mathf.Clamp01(progress);
        if (stickerBody != null)
        {
            Color c = stickerBody.color;
            c.a = 1f - progress;
            stickerBody.color = c;
        }
        if (peeledFlap != null)
        {
            peeledFlap.transform.localScale = new Vector3(1f + progress, 1f + progress, 1f);
        }
    }

    [ContextMenu("Debug Succeed")]
    private void DebugSucceed()
    {
        ApplyPeel(peelDistance, 0.1f);
    }

    [ContextMenu("Debug Tear (too fast)")]
    private void DebugTearFast()
    {
        ApplyPeel(peelDistance * 0.5f, speedLimit + 1f);
    }

    protected override void OnResolved(MiniGameResult result)
    {
        if (result == MiniGameResult.Failed)
        {
            if (tornHalf != null)
            {
                tornHalf.SetActive(true);
            }
            if (stickerBody != null)
            {
                stickerBody.color = tornColor;
            }
        }
        base.OnResolved(result);
    }
}
