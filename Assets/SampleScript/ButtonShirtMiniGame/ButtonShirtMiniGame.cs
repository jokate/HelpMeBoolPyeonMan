using BoolPyeon.Input;
using BoolPyeon.MiniGames;
using UnityEngine;

/// <summary>
/// "단추를 맞춰줘!" 한 칸씩 밀려 잠긴 셔츠 단추를 위부터 제 구멍으로 옮기면 성공.
/// 시작 배치: 단추 i 는 구멍 i+1 에 잠겨 있고(맨 아래 단추는 옷감 밖에 매달려 있음), 구멍 0 은 비어 있다.
/// </summary>
public class ButtonShirtMiniGame : MiniGameBase
{
    public Transform[] buttons;
    public Transform[] holes;
    public Transform shirtRoot;
    public GameObject arrow;
    public Animator resultAnimator;

    [Tooltip("난이도 1~3 별 단추 개수")]
    public int[] buttonCounts = { 3, 4, 5 };

    [Tooltip("난이도 1~3 별 스냅 반경(월드 단위)")]
    public float[] snapRadii = { 0.6f, 0.5f, 0.5f };

    [Tooltip("실패 시 셔츠가 비뚤어지는 각도(z, 도)")]
    public float failedTilt = -10f;

    public Color normalColor = new Color(0.65f, 0.78f, 0.95f, 1f);
    public Color failedColor = new Color(0.85f, 0.2f, 0.2f, 0.85f);

    [Tooltip("맨 아래 단추가 옷감 밖에 매달려 있는 오프셋")]
    public Vector3 danglingOffset = new Vector3(0f, -0.8f, 0f);

    protected override Animator ResultAnimator => resultAnimator;

    private int count;
    private float snapRadius;
    private int[] buttonHole;
    private int[] occupant;
    private int dragIndex = -1;
    private int dragOriginHole = -1;
    private Vector3 dragOriginPos;

    public int CorrectCount
    {
        get
        {
            int total = 0;
            if (buttonHole == null)
            {
                return 0;
            }
            for (int i = 0; i < count && i < buttonHole.Length; i++)
            {
                if (buttonHole[i] == i)
                {
                    total++;
                }
            }
            return total;
        }
    }

    protected override void OnBegin(MiniGameContext ctx)
    {
        int difficulty = Mathf.Clamp(ctx.Difficulty, 1, 3);
        count = buttonCounts[difficulty - 1];
        snapRadius = snapRadii[difficulty - 1];
        bool showArrow = difficulty < 3;

        dragIndex = -1;
        dragOriginHole = -1;

        if (arrow != null)
        {
            arrow.SetActive(showArrow);
        }
        if (shirtRoot != null)
        {
            shirtRoot.rotation = Quaternion.identity;
        }

        int totalHoles = holes != null ? holes.Length : 0;
        int totalButtons = buttons != null ? buttons.Length : 0;

        occupant = new int[totalHoles];
        for (int i = 0; i < totalHoles; i++)
        {
            occupant[i] = -1;
        }
        buttonHole = new int[totalButtons];

        for (int i = 0; i < totalHoles; i++)
        {
            if (holes[i] == null)
            {
                continue;
            }
            holes[i].gameObject.SetActive(i < count);
        }

        for (int i = 0; i < totalButtons; i++)
        {
            Transform button = buttons[i];
            if (button == null)
            {
                continue;
            }
            bool active = i < count;
            button.gameObject.SetActive(active);
            if (!active)
            {
                buttonHole[i] = -1;
                continue;
            }

            SpriteRenderer sr = button.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = normalColor;
            }

            if (i == count - 1)
            {
                buttonHole[i] = -1;
                button.position = holes[i].position + danglingOffset;
            }
            else
            {
                int hole = i + 1;
                buttonHole[i] = hole;
                occupant[hole] = i;
                button.position = holes[hole].position;
            }
        }
    }

    private void Update()
    {
        if (!IsRunning || buttons == null)
        {
            return;
        }

        if (dragIndex < 0)
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
            int idx = FindButtonIndex(hit.collider.transform);
            if (idx < 0)
            {
                return;
            }
            dragIndex = idx;
            dragOriginHole = buttonHole[idx];
            dragOriginPos = buttons[idx].position;
            if (dragOriginHole >= 0 && dragOriginHole < occupant.Length && occupant[dragOriginHole] == idx)
            {
                occupant[dragOriginHole] = -1;
            }
            buttonHole[idx] = -1;
            return;
        }

        if (PointerInput.Up || !PointerInput.Held)
        {
            int hole = FindNearestHole(buttons[dragIndex].position);
            bool placed = hole >= 0 && TryPlace(dragIndex, hole);
            if (!placed && IsRunning)
            {
                buttons[dragIndex].position = dragOriginPos;
                buttonHole[dragIndex] = dragOriginHole;
                if (dragOriginHole >= 0 && dragOriginHole < occupant.Length)
                {
                    occupant[dragOriginHole] = dragIndex;
                }
            }
            dragIndex = -1;
            return;
        }

        Vector3 world = PointerInput.WorldPosition();
        world.z = buttons[dragIndex].position.z;
        buttons[dragIndex].position = world;
    }

    private int FindButtonIndex(Transform t)
    {
        if (buttons == null)
        {
            return -1;
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == t)
            {
                return i;
            }
        }
        return -1;
    }

    private int FindNearestHole(Vector3 pos)
    {
        if (holes == null)
        {
            return -1;
        }
        int best = -1;
        float bestDist = snapRadius;
        for (int i = 0; i < count && i < holes.Length; i++)
        {
            if (holes[i] == null)
            {
                continue;
            }
            float d = Vector2.Distance(pos, holes[i].position);
            if (d <= bestDist)
            {
                bestDist = d;
                best = i;
            }
        }
        return best;
    }

    /// <summary>
    /// 단추를 구멍에 놓아본다. 제 구멍(hole == button)이고 비어 있으면 잠근다.
    /// 다른 빈 구멍이면 즉시 Failed. 이미 찬 구멍이면 false 를 반환하고(호출부가 원위치로 되돌린다).
    /// RunCommand 로 직접 호출할 수 있다.
    /// </summary>
    public bool TryPlace(int button, int hole)
    {
        if (!IsRunning)
        {
            return false;
        }
        if (buttons == null || button < 0 || button >= buttons.Length || buttons[button] == null)
        {
            return false;
        }
        if (holes == null || hole < 0 || hole >= holes.Length || holes[hole] == null)
        {
            return false;
        }

        if (occupant[hole] != -1 && occupant[hole] != button)
        {
            return false;
        }

        if (hole != button)
        {
            Resolve(MiniGameResult.Failed);
            return false;
        }

        int oldHole = buttonHole[button];
        if (oldHole >= 0 && oldHole < occupant.Length && occupant[oldHole] == button)
        {
            occupant[oldHole] = -1;
        }
        occupant[hole] = button;
        buttonHole[button] = hole;
        buttons[button].position = holes[hole].position;

        SpriteRenderer sr = buttons[button].GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = normalColor;
        }

        if (CorrectCount >= count)
        {
            Resolve(MiniGameResult.Success);
        }
        return true;
    }

    [ContextMenu("Debug Solve")]
    private void DebugSolve()
    {
        for (int i = 0; i < count; i++)
        {
            TryPlace(i, i);
        }
    }

    protected override void OnResolved(MiniGameResult result)
    {
        if (result == MiniGameResult.Failed)
        {
            if (shirtRoot != null)
            {
                shirtRoot.rotation = Quaternion.Euler(0f, 0f, failedTilt);
            }
            if (buttons != null)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i] != null && buttons[i].gameObject.activeSelf)
                    {
                        SpriteRenderer sr = buttons[i].GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            sr.color = failedColor;
                        }
                    }
                }
            }
        }
        base.OnResolved(result);
    }
}
