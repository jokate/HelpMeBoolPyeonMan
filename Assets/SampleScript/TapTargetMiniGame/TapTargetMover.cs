using UnityEngine;

/// <summary>
/// 과녁 하나의 이동: 범위(min~max) 안에서 직선으로 움직이다 벽에 튕긴다.
/// 속도는 TapTargetMiniGame 이 ctx.Speed 를 반영해 넣어 준다.
/// </summary>
public class TapTargetMover : MonoBehaviour
{
    public bool isDecoy;

    private Vector2 min;
    private Vector2 max;
    private Vector2 velocity;
    private bool moving;

    public void Launch(Vector2 boundsMin, Vector2 boundsMax, Vector2 direction, float speed, bool decoy)
    {
        min = boundsMin;
        max = boundsMax;
        velocity = direction.normalized * speed;
        isDecoy = decoy;
        moving = true;
    }

    public void Stop()
    {
        moving = false;
    }

    private void Update()
    {
        if (!moving)
        {
            return;
        }
        Vector3 p = transform.position;
        p.x += velocity.x * Time.deltaTime;
        p.y += velocity.y * Time.deltaTime;

        if (p.x < min.x || p.x > max.x)
        {
            velocity.x = -velocity.x;
            p.x = Mathf.Clamp(p.x, min.x, max.x);
        }
        if (p.y < min.y || p.y > max.y)
        {
            velocity.y = -velocity.y;
            p.y = Mathf.Clamp(p.y, min.y, max.y);
        }
        transform.position = p;
    }
}
