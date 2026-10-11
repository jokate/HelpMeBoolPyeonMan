using UnityEngine;
using BoolPyeon.MiniGames;
using BoolPyeon.Input;

// 커터(뒤집개)를 잡고 천천히 들어 올려 조각(PizzaPiece)이 윗선(PizzaChecker)에 닿으면 성공.
// 실패 = 잡은 상태에서 빠르게 들어 올림(상승 속도 초과) / 제한 시간 초과. 중간에 놓는 것은 실패가 아니다.
public class PizzaServeManager : MiniGameBase
{
    public Transform cutter;
    public PizzaChecker checker;
    // 난이도 1 기준 최대 허용 상승 속도(월드 단위/초)·속도 측정 구간(초)·제한 시간(초).
    public float baseMaxLiftSpeed = 8f;
    public float sampleWindow = 0.1f;
    public float baseTimeLimit = 5f;
    public bool ignoreOverUI = false;

    private float maxLiftSpeed;
    private float timeLimit;
    private float elapsed;

    private bool grabbing;
    private float grabOffsetY;
    private float startX;
    private float minY;
    private float sampleStartY;
    private float sampleElapsed;

    protected override Animator ResultAnimator
    {
        get
        {
            Transform root = cutter != null ? cutter : transform;
            Transform piece = FindChildByName(root, "PizzaPiece");
            return piece != null ? piece.GetComponent<Animator>() : null;
        }
    }

    protected override void OnBegin(MiniGameContext ctx)
    {
        if (cutter == null)
        {
            cutter = transform;
        }
        if (checker == null)
        {
            checker = FindObjectOfType<PizzaChecker>();
        }
        if (checker != null)
        {
            Watch(checker);
        }

        startX = cutter.position.x;
        minY = cutter.position.y;

        maxLiftSpeed = baseMaxLiftSpeed * Mathf.Pow(0.85f, ctx.Difficulty - 1);
        timeLimit = Mathf.Max(2.5f, baseTimeLimit * Mathf.Pow(0.9f, ctx.Difficulty - 1));
        elapsed = 0f;
        grabbing = false;
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        elapsed += Time.deltaTime;
        if (elapsed >= timeLimit)
        {
            Resolve(MiniGameResult.Failed);
            return;
        }

        if (!grabbing && PointerInput.Down)
        {
            RaycastHit2D hit = PointerInput.RaycastWorld2D(null, ignoreOverUI);
            if (hit.collider != null && (hit.transform == cutter || hit.transform.IsChildOf(cutter)))
            {
                grabbing = true;
                grabOffsetY = cutter.position.y - PointerInput.WorldPosition().y;
                sampleStartY = cutter.position.y;
                sampleElapsed = 0f;
            }
        }

        if (grabbing)
        {
            if (PointerInput.Up || !PointerInput.Held)
            {
                grabbing = false; // 놓기만 한 것은 실패가 아니다.
                return;
            }

            float y = Mathf.Max(minY, PointerInput.WorldPosition().y + grabOffsetY);
            cutter.position = new Vector3(startX, y, cutter.position.z);

            sampleElapsed += Time.deltaTime;
            float window = Mathf.Max(0.0001f, sampleWindow);
            if ((y - sampleStartY) / window > maxLiftSpeed)
            {
                Resolve(MiniGameResult.Failed);
                return;
            }
            if (sampleElapsed >= window)
            {
                sampleStartY = y;
                sampleElapsed = 0f;
            }
        }
    }

    protected override void OnResolved(MiniGameResult result)
    {
        grabbing = false;
        base.OnResolved(result);
    }

    private static Transform FindChildByName(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name)
            {
                return t;
            }
        }
        return null;
    }
}
