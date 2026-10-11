using System.Collections;
using BoolPyeon.Input;
using UnityEngine;

/// <summary>
/// FlySwatMiniGame 의 연출 전용 파리채: 포인터를 따라다니다가 탭할 때마다 휘두른다.
/// 판정에는 관여하지 않는다(Collider2D 없음). FlySwatMiniGame.Update 가 판정을 그대로 한다.
/// </summary>
public class FlySwatSwatter : MonoBehaviour
{
    public FlySwatMiniGame game;
    public Transform visual;
    public float swingAngle = -40f;
    public float swingTime = 0.12f;
    public Vector2 offset;

    private SpriteRenderer[] renderers;
    private Coroutine swingRoutine;

    private void Awake()
    {
        if (visual == null)
        {
            visual = transform;
        }
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void LateUpdate()
    {
        bool active = game != null;
        SetVisible(active);
        if (!active)
        {
            return;
        }

        Vector3 world = PointerInput.WorldPosition();
        world.z = 0f;
        transform.position = world + (Vector3)offset;

        if (PointerInput.Down && !PointerInput.IsOverUI())
        {
            if (swingRoutine != null)
            {
                StopCoroutine(swingRoutine);
            }
            swingRoutine = StartCoroutine(Swing());
        }
    }

    private IEnumerator Swing()
    {
        float half = swingTime * 0.5f;
        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            visual.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, swingAngle, t / half));
            yield return null;
        }
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            visual.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(swingAngle, 0f, t / half));
            yield return null;
        }
        visual.localRotation = Quaternion.identity;
        swingRoutine = null;
    }

    private void SetVisible(bool visible)
    {
        if (renderers == null)
        {
            return;
        }
        foreach (SpriteRenderer r in renderers)
        {
            if (r != null)
            {
                r.enabled = visible;
            }
        }
    }

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }
}
