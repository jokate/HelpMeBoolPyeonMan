using UnityEngine;

/// <summary>
/// 파리 한 마리의 이동: 범위(min~max) 안에서 0.25~0.6초마다 방향을 무작위로 틀고,
/// 범위 경계에서는 반사한다. 속도는 FlySwatMiniGame 이 ctx.Speed 를 반영해 넣어 준다.
/// </summary>
public class FlyMover : MonoBehaviour
{
    private Vector2 min;
    private Vector2 max;
    private Vector2 direction;
    private float speed;
    private bool moving;
    private float nextTurnTime;

    public void Launch(Vector2 boundsMin, Vector2 boundsMax, float moveSpeed)
    {
        min = boundsMin;
        max = boundsMax;
        speed = moveSpeed;
        direction = Random.insideUnitCircle.normalized;
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.right;
        }
        moving = true;
        ScheduleNextTurn();
        ApplyFacing();
    }

    public void Stop()
    {
        moving = false;
    }

    private void ScheduleNextTurn()
    {
        nextTurnTime = Time.time + Random.Range(0.25f, 0.6f);
    }

    private void Update()
    {
        if (!moving)
        {
            return;
        }

        if (Time.time >= nextTurnTime)
        {
            float angle = Random.Range(-90f, 90f);
            direction = Quaternion.Euler(0f, 0f, angle) * direction;
            direction.Normalize();
            ScheduleNextTurn();
            ApplyFacing();
        }

        Vector3 p = transform.position;
        p.x += direction.x * speed * Time.deltaTime;
        p.y += direction.y * speed * Time.deltaTime;

        if (p.x < min.x || p.x > max.x)
        {
            direction.x = -direction.x;
            p.x = Mathf.Clamp(p.x, min.x, max.x);
            ApplyFacing();
        }
        if (p.y < min.y || p.y > max.y)
        {
            direction.y = -direction.y;
            p.y = Mathf.Clamp(p.y, min.y, max.y);
            ApplyFacing();
        }

        transform.position = p;
    }

    private void ApplyFacing()
    {
        Vector3 scale = transform.localScale;
        float sign = direction.x < 0f ? -1f : 1f;
        scale.x = sign * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
