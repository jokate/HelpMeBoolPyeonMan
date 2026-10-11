using System.Collections;
using System.Collections.Generic;
using BoolPyeon.Input;
using BoolPyeon.MiniGames;
using UnityEngine;

/// <summary>
/// "불편 덩어리를 막아줘!": 보스가 위에서 떨어뜨리는 불편 덩어리를 바닥에 닿기 전에
/// 전부 탭해서 부수면 방어 성공. 덩어리 하나라도 바닥(태그 ClearObject)에 닿으면 즉시 실패.
/// 시간초과는 Runner 가 처리한다.
/// 난이도(ctx.Difficulty 1~3): 개수 N = 2+D (3/4/5). 낙하 속도 = 3.0×(1+0.25×(D-1))×ctx.Speed.
/// 생성 간격 = 0.8/0.65/0.5초. 반지름 0.68/0.62/0.6 (T5 경계값 보정, 최소 지름 120px 보장).
/// </summary>
public class BossBlobMiniGame : MiniGameBase
{
    public Transform spawnLeft;
    public Transform spawnRight;
    public GameObject blobPrefab;
    public Animator resultAnimator;
    public Transform boss;

    [Tooltip("덩어리 낙하 기본 속도(월드 단위/초, 난이도 1 기준)")]
    public float baseFallSpeed = 3.0f;

    private readonly List<BlobFaller> blobs = new List<BlobFaller>();
    private int totalToSpawn;
    private int spawnedCount;
    private bool spawnDone;
    private Coroutine spawnRoutine;

    protected override Animator ResultAnimator => resultAnimator;

    protected override void OnBegin(MiniGameContext ctx)
    {
        int difficulty = Mathf.Clamp(ctx.Difficulty, 1, 3);
        float fallSpeed = baseFallSpeed * (1f + 0.25f * (difficulty - 1)) * Mathf.Max(0.1f, ctx.Speed);
        float radius = difficulty == 1 ? 0.68f : (difficulty == 2 ? 0.62f : 0.6f);
        float interval = difficulty == 1 ? 0.8f : (difficulty == 2 ? 0.65f : 0.5f);

        totalToSpawn = 2 + difficulty;
        spawnedCount = 0;
        spawnDone = false;

        spawnRoutine = StartCoroutine(SpawnRoutine(totalToSpawn, fallSpeed, radius, interval));
    }

    private IEnumerator SpawnRoutine(int count, float fallSpeed, float radius, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            Spawn(fallSpeed, radius);
            spawnedCount++;
            yield return new WaitForSeconds(interval);
        }
        spawnDone = true;
        CheckAllCleared();
    }

    private void Spawn(float fallSpeed, float radius)
    {
        float lx = spawnLeft != null ? spawnLeft.position.x : -6f;
        float rx = spawnRight != null ? spawnRight.position.x : 6f;
        float y = spawnLeft != null ? spawnLeft.position.y : 3.5f;
        Vector3 pos = new Vector3(Random.Range(Mathf.Min(lx, rx), Mathf.Max(lx, rx)), y, 0f);

        GameObject go = Instantiate(blobPrefab, pos, Quaternion.identity, transform);
        go.transform.localScale = Vector3.one * (radius * 2f);
        BlobFaller faller = go.GetComponent<BlobFaller>();
        if (faller == null)
        {
            faller = go.AddComponent<BlobFaller>();
        }
        faller.Launch(fallSpeed, this);
        blobs.Add(faller);
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
            BlockBlob(hit.collider);
        }
    }

    /// <summary>현재 떨어지고 있는 덩어리 목록 (디버그·자동 검증용).</summary>
    public IReadOnlyList<BlobFaller> Blobs => blobs;

    /// <summary>탭으로 덩어리를 부순다. 남은 덩어리가 없고 생성도 끝났으면 성공.</summary>
    public void BlockBlob(Collider2D col)
    {
        if (!IsRunning || col == null)
        {
            return;
        }
        BlobFaller faller = col.GetComponentInParent<BlobFaller>();
        if (faller == null || !blobs.Contains(faller))
        {
            return;
        }

        blobs.Remove(faller);
        Destroy(faller.gameObject);
        CheckAllCleared();
    }

    private void CheckAllCleared()
    {
        if (spawnDone && blobs.Count <= 0)
        {
            Resolve(MiniGameResult.Success);
        }
    }

    /// <summary>덩어리가 바닥에 닿았을 때의 판정: 즉시 실패.</summary>
    public void OnBlobLanded(BlobFaller faller)
    {
        if (!IsRunning)
        {
            return;
        }
        Resolve(MiniGameResult.Failed);
    }

    protected override void OnResolved(MiniGameResult result)
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
        foreach (BlobFaller faller in blobs)
        {
            if (faller != null)
            {
                faller.Stop();
            }
        }
        base.OnResolved(result);
    }
}
