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
