using BoolPyeon.Input;
using BoolPyeon.MiniGames;
using UnityEngine;

/// <summary>
/// "삼각김밥을 뜯어줘!" 비닐 탭 3개(위→왼쪽→오른쪽)를 순서대로, 속도 한계 안에서
/// 끝까지 당기면 성공. 순서가 틀리거나 너무 빨리 당기면 실패. 시간초과는 Runner 가 처리한다.
/// tabs/arrows 순서: 0=위(아래로 당김), 1=왼쪽, 2=오른쪽.
/// </summary>
public class SamgakKimbapMiniGame : MiniGameBase
{
    public Transform[] tabs = new Transform[3];
    public GameObject[] arrows = new GameObject[3];
    public SpriteRenderer laver;
    public SpriteRenderer wrapper;
    public Animator resultAnimator;

    [Tooltip("당겨야 하는 거리(월드 단위)")]
    public float pullDistance = 2.5f;

    [Tooltip("난이도 1~3 별 속도 한계(월드 단위/초). 이 값을 넘겨 당기면 실패.")]
    public float[] speedLimits = { 14f, 11f, 8f };

    [Tooltip("속도 샘플 창(초). DragInteraction 과 같은 방식.")]
    public float sampleWindow = 0.1f;

    public Color tornColor = new Color(0.55f, 0.5f, 0.4f);

    private static readonly Vector2[] Axes = { new Vector2(0f, -1f), new Vector2(-1f, 0f), new Vector2(1f, 0f) };

    private Vector3[] startPos = new Vector3[3];
    private float[] pulled = new float[3];
    private int step;
    private int holding = -1;
    private float speedLimit;
    private bool showArrows;
    private Vector2 sampleStartPos;
    private float sampleElapsed;
    private float currentSpeed;

    protected override Animator ResultAnimator => resultAnimator;

    protected override void OnBegin(MiniGameContext ctx)
    {
        int difficulty = Mathf.Clamp(ctx.Difficulty, 1, 3);
        speedLimit = speedLimits[difficulty - 1];
        showArrows = difficulty < 3;

        step = 0;
        holding = -1;
        for (int i = 0; i < tabs.Length; i++)
        {
            pulled[i] = 0f;
            if (tabs[i] != null)
            {
                startPos[i] = tabs[i].position;
                tabs[i].gameObject.SetActive(true);
            }
            if (arrows[i] != null)
            {
                arrows[i].SetActive(showArrows && i == 0);
            }
        }
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        if (holding == -1)
        {
            if (!PointerInput.Down)
            {
                return;
            }
            RaycastHit2D hit = PointerInput.RaycastWorld2D();
            if (hit.collider == null)
            {
                return;
            }
            int idx = IndexOf(hit.transform);
            if (idx < 0)
            {
                return;
            }
            ApplyPull(idx, 0f, 0f);
            if (IsRunning)
            {
                holding = idx;
                sampleStartPos = tabs[idx].position;
                sampleElapsed = 0f;
                currentSpeed = 0f;
            }
            return;
        }

        if (PointerInput.Up || !PointerInput.Held)
        {
            holding = -1;
            return;
        }

        Transform tab = tabs[holding];
        Vector2 axis = Axes[holding];
        Vector3 world = PointerInput.WorldPosition();
        float proj = Mathf.Clamp(Vector2.Dot((Vector2)world - (Vector2)startPos[holding], axis), 0f, pullDistance);
        tab.position = startPos[holding] + (Vector3)(axis * proj);

        sampleElapsed += Time.deltaTime;
        if (sampleElapsed >= sampleWindow)
        {
            Vector2 now = tab.position;
            currentSpeed = (now - sampleStartPos).magnitude / sampleElapsed;
            sampleStartPos = now;
            sampleElapsed = 0f;
        }

        int heldTab = holding;
        ApplyPull(heldTab, proj, currentSpeed);
    }

    private int IndexOf(Transform t)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i] != null && (t == tabs[i] || t.IsChildOf(tabs[i])))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 판정 핵심 로직. 순서가 틀리거나(tab != step) 속도가 한계를 넘으면 실패,
    /// pullDistance 에 닿으면 해당 탭을 완료 처리한다. RunCommand 로도 직접 호출할 수 있다.
    /// </summary>
    public bool ApplyPull(int tab, float pulledDistance, float speed)
    {
        if (!IsRunning)
        {
            return false;
        }
        if (tab != step)
        {
            Resolve(MiniGameResult.Failed);
            return false;
        }
        if (speed > speedLimit)
        {
            Resolve(MiniGameResult.Failed);
            return false;
        }

        pulled[tab] = pulledDistance;
        if (pulledDistance >= pullDistance - 0.001f)
        {
            if (arrows[step] != null)
            {
                arrows[step].SetActive(false);
            }
            if (tabs[step] != null)
            {
                tabs[step].gameObject.SetActive(false);
            }
            holding = -1;
            step++;
            if (step >= tabs.Length)
            {
                Resolve(MiniGameResult.Success);
            }
            else if (showArrows && arrows[step] != null)
            {
                arrows[step].SetActive(true);
            }
        }
        return true;
    }

    [ContextMenu("Debug Succeed")]
    private void DebugSucceed()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            ApplyPull(i, pullDistance, 0.1f);
        }
    }

    [ContextMenu("Debug Tear (wrong order)")]
    private void DebugTear()
    {
        ApplyPull(1, 0f, 0f);
    }

    [ContextMenu("Debug Tear (too fast)")]
    private void DebugTearFast()
    {
        ApplyPull(step, pullDistance * 0.5f, speedLimit + 1f);
    }

    protected override void OnResolved(MiniGameResult result)
    {
        if (laver != null && result == MiniGameResult.Failed)
        {
            laver.color = tornColor;
        }
        base.OnResolved(result);
    }
}
