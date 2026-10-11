using BoolPyeon.Input;
using BoolPyeon.MiniGames;
using UnityEngine;

/// <summary>
/// "기포를 없애줘!" 보호필름 아래 기포를 카드로 밀어 화면 밖으로 빼내면 성공.
/// 시간초과는 Runner 가 처리한다. 카드는 화면 아무 곳이나 누르면 그 자리로 잡혀 이동한다.
/// </summary>
public class ScreenBubbleMiniGame : MiniGameBase
{
    public Transform card;
    public SpriteRenderer[] bubbles;
    public Transform screenMin;
    public Transform screenMax;
    public Transform innerMin;
    public Transform innerMax;
    public GameObject arrow;
    public Animator resultAnimator;

    [Tooltip("난이도 1~3 별 기포 개수")]
    public int[] bubbleCounts = { 3, 5, 7 };

    [Tooltip("난이도 1~3 별 기포 크기 배율")]
    public float[] bubbleScales = { 1f, 0.8f, 0.6f };

    [Tooltip("기포 기본 지름(월드 단위)")]
    public float baseBubbleDiameter = 0.6f;

    [Tooltip("카드의 접촉 반경(월드 단위)")]
    public float cardRadius = 0.4f;

    [Tooltip("기포 배치 시 서로 떨어뜨릴 최소 간격")]
    public float minSpacing = 1.0f;

    public Color failedColor = new Color(0.85f, 0.2f, 0.2f, 0.85f);

    private Vector3 cardStartPos;
    private float[] bubbleRadii;
    private bool[] bubbleActive;
    private bool holding;
    private Vector3 lastPointerWorld;

    protected override Animator ResultAnimator => resultAnimator;

    public int RemainingBubbles
    {
        get
        {
            int count = 0;
            if (bubbleActive == null)
            {
                return 0;
            }
            for (int i = 0; i < bubbleActive.Length; i++)
            {
                if (bubbleActive[i])
                {
                    count++;
                }
            }
            return count;
        }
    }

    protected override void OnBegin(MiniGameContext ctx)
    {
        int difficulty = Mathf.Clamp(ctx.Difficulty, 1, 3);
        int count = bubbleCounts[difficulty - 1];
        float scale = bubbleScales[difficulty - 1];
        bool showArrow = difficulty < 3;

        holding = false;
        if (card != null)
        {
            cardStartPos = card.position;
        }
        if (arrow != null)
        {
            arrow.SetActive(showArrow);
        }

        int total = bubbles != null ? bubbles.Length : 0;
        bubbleActive = new bool[total];
        bubbleRadii = new float[total];

        for (int i = 0; i < total; i++)
        {
            SpriteRenderer sr = bubbles[i];
            if (sr == null)
            {
                continue;
            }
            bool active = i < count;
            bubbleActive[i] = active;
            sr.gameObject.SetActive(active);
            if (!active)
            {
                continue;
            }
            sr.color = new Color(0.75f, 0.85f, 0.95f, 0.6f);
            float diameter = baseBubbleDiameter * scale;
            sr.transform.localScale = new Vector3(diameter, diameter, 1f);
            bubbleRadii[i] = diameter * 0.5f;
            sr.transform.position = RandomInnerPosition(i);
        }
    }

    private Vector3 RandomInnerPosition(int index)
    {
        if (innerMin == null || innerMax == null)
        {
            return Vector3.zero;
        }
        Vector3 min = innerMin.position;
        Vector3 max = innerMax.position;
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector3 candidate = new Vector3(
                Random.Range(Mathf.Min(min.x, max.x), Mathf.Max(min.x, max.x)),
                Random.Range(Mathf.Min(min.y, max.y), Mathf.Max(min.y, max.y)),
                0f);
            bool ok = true;
            for (int i = 0; i < index; i++)
            {
                if (!bubbleActive[i] || bubbles[i] == null)
                {
                    continue;
                }
                if (Vector2.Distance(candidate, bubbles[i].transform.position) < minSpacing)
                {
                    ok = false;
                    break;
                }
            }
            if (ok)
            {
                return candidate;
            }
        }
        return new Vector3(
            Random.Range(Mathf.Min(min.x, max.x), Mathf.Max(min.x, max.x)),
            Random.Range(Mathf.Min(min.y, max.y), Mathf.Max(min.y, max.y)),
            0f);
    }

    private void Update()
    {
        if (!IsRunning || card == null)
        {
            return;
        }

        if (!holding)
        {
            if (!PointerInput.Down)
            {
                return;
            }
            if (PointerInput.IsOverUI())
            {
                return;
            }
            holding = true;
            lastPointerWorld = card.position;
        }

        if (PointerInput.Up || !PointerInput.Held)
        {
            holding = false;
            return;
        }

        Vector3 world = PointerInput.WorldPosition();
        world.z = card.position.z;
        Vector2 from = lastPointerWorld;
        Vector2 to = world;
        lastPointerWorld = world;
        ApplySweep(from, to);
    }

    /// <summary>
    /// 카드를 from → to 로 옮기고, 접촉 반경 안의 기포를 delta 만큼 함께 밀어낸다.
    /// 기포 중심이 화면 경계 밖으로 나가면 비활성화한다. RunCommand 로도 직접 호출할 수 있다.
    /// 남은 활성 기포 수를 반환한다.
    /// </summary>
    public int ApplySweep(Vector2 from, Vector2 to)
    {
        if (!IsRunning)
        {
            return RemainingBubbles;
        }

        Vector2 delta = to - from;
        if (card != null)
        {
            Vector3 pos = card.position;
            pos.x = to.x;
            pos.y = to.y;
            card.position = pos;
        }

        if (bubbles != null)
        {
            for (int i = 0; i < bubbles.Length; i++)
            {
                if (bubbleActive == null || i >= bubbleActive.Length || !bubbleActive[i])
                {
                    continue;
                }
                SpriteRenderer sr = bubbles[i];
                if (sr == null)
                {
                    continue;
                }
                float contactRadius = cardRadius + bubbleRadii[i];
                if (Vector2.Distance(from, sr.transform.position) <= contactRadius)
                {
                    sr.transform.position += (Vector3)delta;
                }

                if (IsOutsideScreen(sr.transform.position))
                {
                    DeactivateBubble(i);
                }
            }
        }

        if (RemainingBubbles <= 0)
        {
            Resolve(MiniGameResult.Success);
        }

        return RemainingBubbles;
    }

    private bool IsOutsideScreen(Vector3 pos)
    {
        if (screenMin == null || screenMax == null)
        {
            return false;
        }
        Vector3 min = screenMin.position;
        Vector3 max = screenMax.position;
        float lowX = Mathf.Min(min.x, max.x);
        float highX = Mathf.Max(min.x, max.x);
        float lowY = Mathf.Min(min.y, max.y);
        float highY = Mathf.Max(min.y, max.y);
        return pos.x < lowX || pos.x > highX || pos.y < lowY || pos.y > highY;
    }

    private void DeactivateBubble(int index)
    {
        bubbleActive[index] = false;
        if (bubbles[index] != null)
        {
            bubbles[index].transform.localScale = Vector3.zero;
            bubbles[index].gameObject.SetActive(false);
        }
    }

    [ContextMenu("Debug Clear All")]
    private void DebugClearAll()
    {
        if (bubbles == null || screenMax == null)
        {
            return;
        }
        Vector3 outside = screenMax.position + new Vector3(10f, 10f, 0f);
        for (int i = 0; i < bubbles.Length; i++)
        {
            if (bubbleActive == null || i >= bubbleActive.Length || !bubbleActive[i])
            {
                continue;
            }
            if (bubbles[i] != null)
            {
                bubbles[i].transform.position = outside;
            }
            DeactivateBubble(i);
        }
        if (RemainingBubbles <= 0)
        {
            Resolve(MiniGameResult.Success);
        }
    }

    protected override void OnResolved(MiniGameResult result)
    {
        if (result == MiniGameResult.Failed && bubbles != null)
        {
            for (int i = 0; i < bubbles.Length; i++)
            {
                if (bubbleActive != null && i < bubbleActive.Length && bubbleActive[i] && bubbles[i] != null)
                {
                    bubbles[i].color = failedColor;
                }
            }
        }
        base.OnResolved(result);
    }
}
