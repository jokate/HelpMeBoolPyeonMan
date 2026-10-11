using System.Collections.Generic;
using BoolPyeon.Input;
using BoolPyeon.MiniGames;
using UnityEngine;

/// <summary>
/// 가이드 검증용 미니게임 "잡아라!": 움직이는 초록 과녁을 모두 탭하면 성공,
/// 빨간 가짜 과녁을 탭하면 실패. 시간초과는 Runner 가 처리한다.
/// 난이도: 진짜 = Difficulty+1, 가짜 = Difficulty-1. 속도 = baseSpeed × ctx.Speed.
/// </summary>
public class TapTargetMiniGame : MiniGameBase
{
    public Transform minPos;
    public Transform maxPos;
    public GameObject targetPrefab;
    public GameObject decoyPrefab;
    public Animator resultAnimator;

    [Tooltip("과녁 이동 기본 속도(월드 단위/초)")]
    public float baseSpeed = 1.5f;

    [Tooltip("과녁 반지름(월드 단위). 범위 안쪽으로 이만큼 여유를 둔다.")]
    public float targetRadius = 0.6f;

    private readonly List<TapTargetMover> movers = new List<TapTargetMover>();
    private int remaining;

    protected override Animator ResultAnimator => resultAnimator;

    protected override void OnBegin(MiniGameContext ctx)
    {
        int difficulty = Mathf.Clamp(ctx.Difficulty, 1, 3);
        float speed = baseSpeed * Mathf.Max(0.1f, ctx.Speed);

        Vector2 lo = Vector2.Min(minPos.position, maxPos.position) + Vector2.one * targetRadius;
        Vector2 hi = Vector2.Max(minPos.position, maxPos.position) - Vector2.one * targetRadius;

        int targetCount = difficulty + 1;
        int decoyCount = difficulty - 1;
        for (int i = 0; i < targetCount; i++)
        {
            Spawn(targetPrefab, lo, hi, speed, false);
        }
        for (int i = 0; i < decoyCount; i++)
        {
            Spawn(decoyPrefab, lo, hi, speed, true);
        }
        remaining = targetCount;
    }

    private void Spawn(GameObject prefab, Vector2 lo, Vector2 hi, float speed, bool decoy)
    {
        Vector3 pos = new Vector3(Random.Range(lo.x, hi.x), Random.Range(lo.y, hi.y), 0f);
        // 매니저 아래에 두어 미니게임 씬에 생성되게 한다 (Runner 가 Additive 로 올려도 함께 언로드).
        GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform);
        TapTargetMover mover = go.GetComponent<TapTargetMover>();
        if (mover == null)
        {
            mover = go.AddComponent<TapTargetMover>();
        }
        Vector2 dir = Random.insideUnitCircle;
        if (dir.sqrMagnitude < 0.01f)
        {
            dir = Vector2.right;
        }
        mover.Launch(lo, hi, dir, speed, decoy);
        movers.Add(mover);
    }

    private void Update()
    {
        if (!IsRunning || !PointerInput.Down)
        {
            return;
        }
        RaycastHit2D hit = PointerInput.RaycastWorld2D();
        if (hit.collider != null)
        {
            HandleHit(hit.collider);
        }
    }

    /// <summary>현재 떠 있는 과녁 목록 (디버그·자동 검증용).</summary>
    public IReadOnlyList<TapTargetMover> Movers => movers;

    /// <summary>탭이 맞은 콜라이더 판정. Update 에서 호출하며, 자동 검증에서도 직접 부를 수 있다.</summary>
    public void HandleHit(Collider2D col)
    {
        if (!IsRunning || col == null)
        {
            return;
        }
        TapTargetMover mover = col.GetComponentInParent<TapTargetMover>();
        if (mover == null || !movers.Contains(mover))
        {
            return;
        }

        if (mover.isDecoy)
        {
            Resolve(MiniGameResult.Failed);
            return;
        }

        movers.Remove(mover);
        Destroy(mover.gameObject);
        remaining--;
        if (remaining <= 0)
        {
            Resolve(MiniGameResult.Success);
        }
    }

    protected override void OnResolved(MiniGameResult result)
    {
        foreach (TapTargetMover mover in movers)
        {
            if (mover != null)
            {
                mover.Stop();
            }
        }
        base.OnResolved(result);
    }
}
