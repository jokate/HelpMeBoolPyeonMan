using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsbSetter : MonoBehaviour
{
    public GameObject upPosition;
    public GameObject downPosition;
    public bool isClear = false;
    
    // Start is called before the first frame update
    void Start()
    {
        float randomPos = Random.Range(downPosition.transform.position.y, upPosition.transform.position.y);
        this.gameObject.transform.position = new Vector2(gameObject.transform.position.x, randomPos);
    }
    /// <summary>난이도가 오를수록 포트 판정 높이를 줄인다 (최소 0.5배).</summary>
    public void ApplyDifficulty(int difficulty)
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null)
        {
            return;
        }
        float scale = Mathf.Max(0.5f, 1f - 0.15f * (difficulty - 1));
        Vector2 size = box.size;
        size.y *= scale;
        box.size = size;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("ClearObject"))
        {
            Debug.Log("Clear");
            isClear = true;   
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("ClearObject"))
        {
            isClear = false;
        }
    }
}
