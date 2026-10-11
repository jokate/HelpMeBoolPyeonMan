using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoolPyeon.MiniGames;
using BoolPyeon.Input.Interaction;


public class UsbMover : MiniGameBase
{
    public GameObject target, startpos, usbSetter, noteBookMover;
    // 초당 이동 거리 (예전 FixedUpdate 0.075/스텝 × 50Hz)
    public float riseSpeed = 3.75f;
    public HoldInteraction hold = new HoldInteraction();

    private NoteBookMover noteBook;
    private UsbSetter setter;

    protected override Animator ResultAnimator => GetComponent<Animator>();

    protected override void OnBegin(MiniGameContext ctx)
    {
        noteBook = noteBookMover.GetComponent<NoteBookMover>();
        setter = usbSetter.GetComponent<UsbSetter>();
        if (setter != null)
        {
            setter.ApplyDifficulty(ctx.Difficulty);
        }
        noteBook.Launch(ctx.Speed);
        Watch(new PredicateCondition(MiniGameResult.Failed, () => noteBook.isCollide && !setter.isClear));
        Watch(new PredicateCondition(MiniGameResult.Success, () => noteBook.isCollide && setter.isClear));
    }

    void Update()
    {
        if (!IsRunning || noteBook == null || noteBook.isCollide)
        {
            return;
        }
        hold.Tick();
        float targetY = hold.IsHolding ? target.transform.position.y : startpos.transform.position.y;
        float step = riseSpeed * Context.Speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, targetY), step);
    }

    protected override void OnResolved(MiniGameResult result)
    {
        base.OnResolved(result);
        if (result == MiniGameResult.Success)
        {
            StartCoroutine(SlideOut());
        }
        else
        {
            Debug.Log("Oh no..");
        }
    }

    IEnumerator SlideOut()
    {
        float time = 1f;
        while (time > 0.0f)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x - 0.1f, transform.position.y), 0.001f);
            time -= Time.deltaTime;
            yield return null;
        }
    }
}
