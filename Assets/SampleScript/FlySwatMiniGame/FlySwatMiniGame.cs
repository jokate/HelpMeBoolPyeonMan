using System.Collections.Generic;
using BoolPyeon.Input;
using BoolPyeon.MiniGames;
using UnityEngine;

/// <summary>
/// "파리를 잡아줘!": 방 안을 이리저리 날아다니는 파리를 제한 시간 안에 모두 탭해서 잡으면 성공.
/// 빈 곳을 too many times 헛탭하면 실패한다. 시간초과는 Runner 가 처리한다.
/// 난이도(ctx.Difficulty 1~3): 파리 수 = D. 속도 = 2.5 × (1 + 0.3 × (D-1)) × ctx.Speed.
/// </summary>
public class FlySwatMiniGame : MiniGameBase
{
    public Transform minPos;
    public Transform maxPos;
    public GameObject flyPrefab;
    public Animator resultAnimator;

    [Tooltip("파리 이동 기본 속도(월드 단위/초, 난이도 1 기준)")]
    public float baseSpeed = 2.5f;

    private readonly List<FlyMover> flies = new List<FlyMover>();
    private int misses;
    private int allowedMisses;

    protected override Animator ResultAnimator => resultAnimator;

    protected override void OnBegin(MiniGameContext ctx)
    {
        int difficulty = Mathf.Clamp(ctx.Difficulty, 1, 3);
        float speed = baseSpeed * (1f + 0.3f * (difficulty - 1)) * Mathf.Max(0.1f, ctx.Speed);
        float radius = difficulty == 1 ? 0.72f : (difficulty == 2 ? 0.64f : 0.61f);
        allowedMisses = difficulty == 1 ? 3 : 2;

        Vector2 lo = Vector2.Min(minPos.position, maxPos.position) + Vector2.one * radius;
        Vector2 hi = Vector2.Max(minPos.position, maxPos.position) - Vector2.one * radius;

        int flyCount = difficulty;
        for (int i = 0; i < flyCount; i++)
        {
            Spawn(lo, hi, speed, radius);
        }
    }

    private void Spawn(Vector2 lo, Vector2 hi, float speed, float radius)
    {
        Vector3 pos = new Vector3(Random.Range(lo.x, hi.x), Random.Range(lo.y, hi.y), 0f);
        GameObject go = Instantiate(flyPrefab, pos, Quaternion.identity, transform);
        go.transform.localScale = Vector3.one * (radius * 2f);
        FlyMover mover = go.GetComponent<FlyMover>();
        if (mover == null)
        {
            mover = go.AddComponent<FlyMover>();
        }
        mover.Launch(lo, hi, speed);
        flies.Add(mover);
    }

    private void Update()
    {
        if (!IsRunning || !PointerInput.Down || PointerInput.IsOverUI())
        {
            return;
        }
        RaycastHit2D hit = PointerInput.RaycastWorld2D();
        if (hit.collider != null)
        {
            HandleHit(hit.collider);
        }
        else
        {
            HandleMiss();
        }
    }

    /// <summary>현재 떠 있는 파리 목록 (디버그·자동 검증용).</summary>
    public IReadOnlyList<FlyMover> Flies => flies;

    /// <summary>탭이 파리에 맞았을 때의 판정. Update 에서 호출하며, 자동 검증에서도 직접 부를 수 있다.</summary>
    public void HandleHit(Collider2D col)
    {
        if (!IsRunning || col == null)
        {
            return;
        }
        FlyMover mover = col.GetComponentInParent<FlyMover>();
        if (mover == null || !flies.Contains(mover))
        {
            HandleMiss();
            return;
        }

        flies.Remove(mover);
        Destroy(mover.gameObject);
        if (flies.Count <= 0)
        {
            Resolve(MiniGameResult.Success);
        }
    }

    /// <summary>빈 곳을 탭했을 때의 판정. 허용치를 넘으면 실패한다.</summary>
    public void HandleMiss()
    {
        if (!IsRunning)
        {
            return;
        }
        misses++;
        if (misses > allowedMisses)
        {
            Resolve(MiniGameResult.Failed);
        }
    }

    protected override void OnResolved(MiniGameResult result)
    {
        foreach (FlyMover mover in flies)
        {
            if (mover != null)
            {
                mover.Stop();
            }
        }
        base.OnResolved(result);
    }
}
