using System.Collections.Generic;
using UnityEngine;
using BoolPyeon.MiniGames;
using BoolPyeon.Input;
using BoolPyeon.Input.Interaction;

public class EggGameManager : MiniGameBase
{
    // 난이도 1 에서의 껍질 수. 실제 개수 = Clamp(baseShellCount + Difficulty - 1, 1, maxShellCount)
    public int baseShellCount = 2;
    public int maxShellCount = 6;
    // ctx.TimeLimit 이 없을 때(씬 단독 Play) 쓰는 제한 시간
    public float baseTimeLimit = 5f;
    public GameObject originalEgg, downPos, upPos;
    public List<GameObject> eggShells;
    public List<GameObject> speggShells;
    public TapInteraction tap = new TapInteraction();
    private bool isFailed = false;
    private float timeLimit;
    private float elapsed;

    protected override Animator ResultAnimator => originalEgg.GetComponent<Animator>();

    protected override void OnBegin(MiniGameContext ctx)
    {
        speggShells.Clear();
        isFailed = false;
        int count = Mathf.Clamp(baseShellCount + ctx.Difficulty - 1, 1, Mathf.Max(1, maxShellCount));
        for(int i = 0; i < count; i++)
        {
            float x = Random.Range(downPos.transform.position.x, upPos.transform.position.x);
            float y = Random.Range(downPos.transform.position.y, upPos.transform.position.y);
            int number = Random.Range(0, eggShells.Count);
            GameObject speggshell = Instantiate(eggShells[number], originalEgg.transform);
            speggshell.transform.position = new Vector2(x, y);
            speggShells.Add(speggshell);
        }
        timeLimit = ctx.TimeLimit > 0f ? ctx.TimeLimit : baseTimeLimit;
        elapsed = 0f;
        Watch(new PredicateCondition(MiniGameResult.Failed, () => isFailed));
        Watch(new PredicateCondition(MiniGameResult.Success, () => speggShells.Count == 0 && !isFailed));
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
            isFailed = true;
            return;
        }

        if (!PointerInput.Down)
        {
            return;
        }

        GameObject target = PickTarget();
        if (target == null)
        {
            return;
        }
        if (target == originalEgg)
        {
            isFailed = true;
        }
        else
        {
            speggShells.Remove(target);
            Destroy(target);
        }
    }

    // 누른 지점의 콜라이더를 모두 모아 껍질을 달걀보다 먼저 고른다.
    // Raycast(zero)는 겹친 콜라이더 중 아무거나 돌려주므로 쓰지 않는다.
    private GameObject PickTarget()
    {
        if (tap.ignoreOverUI && PointerInput.IsOverUI())
        {
            return null;
        }

        Collider2D[] hits = Physics2D.OverlapPointAll(PointerInput.WorldPosition());
        int bestIndex = -1;
        bool eggHit = false;
        foreach (Collider2D col in hits)
        {
            if (col == null)
            {
                continue;
            }
            GameObject go = col.gameObject;
            int index = IndexOfShell(go.transform);
            if (index > bestIndex)
            {
                bestIndex = index;
            }
            else if (index < 0 && go == originalEgg)
            {
                eggHit = true;
            }
        }

        if (bestIndex >= 0)
        {
            return speggShells[bestIndex];
        }
        return eggHit ? originalEgg : null;
    }

    // 콜라이더가 껍질 프리팹의 자식에 붙어 있어도 껍질로 인정한다.
    private int IndexOfShell(Transform t)
    {
        while (t != null && t.gameObject != originalEgg)
        {
            int index = speggShells.IndexOf(t.gameObject);
            if (index >= 0)
            {
                return index;
            }
            t = t.parent;
        }
        return -1;
    }
}
