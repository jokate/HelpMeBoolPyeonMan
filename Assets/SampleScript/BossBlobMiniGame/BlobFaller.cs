using UnityEngine;

/// <summary>
/// 불편 덩어리 하나의 낙하: Launch 이후 아래로 일정 속도로 떨어지다가,
/// 태그 ClearObject(바닥) 에 닿으면 owner.OnBlobLanded 를 호출한다.
/// </summary>
public class BlobFaller : MonoBehaviour
{
    private float speed;
    private BossBlobMiniGame owner;
    private bool falling;

    public void Launch(float fallSpeed, BossBlobMiniGame miniGame)
    {
        speed = fallSpeed;
        owner = miniGame;
        falling = true;
    }

    public void Stop()
    {
        falling = false;
    }

    private void Update()
    {
        if (!falling)
        {
            return;
        }
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!falling || other == null || owner == null)
        {
            return;
        }
        if (other.CompareTag("ClearObject"))
        {
            falling = false;
            owner.OnBlobLanded(this);
        }
    }
}
