using UnityEngine;
using BoolPyeon.MiniGames;
using BoolPyeon.Input.Interaction;

public class CutterMove : MiniGameBase
{
    public DragInteraction drag = new DragInteraction();
    // 난이도 1 의 최대 허용 속도(월드 단위/초). 예전 판정 0.1초에 1.5 이동과 같음
    public float baseMaxSpeed = 15f;

    private PizzaChecker checker;

    protected override Animator ResultAnimator => checker != null ? checker.GetComponent<Animator>() : null;

    protected override void OnBegin(MiniGameContext ctx)
    {
        if (drag.target == null)
        {
            Transform cutter = FindChildByName(transform, "Cutter");
            drag.target = cutter != null ? cutter : transform;
        }
        checker = GetComponentInChildren<PizzaChecker>();
        if (checker != null)
        {
            Watch(checker);
        }
    }

    void Update()
    {
        if (!IsRunning)
        {
            return;
        }
        drag.Tick();
        float maxSpeed = baseMaxSpeed * Mathf.Pow(0.85f, Context.Difficulty - 1);
        if (drag.IsDragging && drag.Speed > maxSpeed)
        {
            Resolve(MiniGameResult.Failed);
        }
    }

    protected override void OnResolved(MiniGameResult result)
    {
        drag.Release();
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
