using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBookMover : MonoBehaviour
{
    public bool isCollide = false;
    // 예전 FixedUpdate 스텝당 이동량. 더 이상 쓰지 않음 (speedPerSecond 사용)
    public float moveSpeed = 0.1f;
    // 초당 이동 거리 (0.1/스텝 × 50Hz)
    public float speedPerSecond = 5f;

    private bool launched = false;
    private float speedMultiplier = 1f;

    /// <summary>미니게임이 시작될 때 매니저가 부른다. 그 전에는 멈춰 있다.</summary>
    public void Launch(float multiplier)
    {
        speedMultiplier = multiplier > 0f ? multiplier : 1f;
        launched = true;
    }

    void Update()
    {
        if (launched && !isCollide)
            this.transform.Translate(new Vector3(speedPerSecond * speedMultiplier * Time.deltaTime, 0, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("ClearObject"))
        {
            isCollide = true;
        }
    }
}
