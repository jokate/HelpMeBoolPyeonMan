using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBookMover : MonoBehaviour
{
    public bool isCollide = false;
    public float moveSpeed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(!isCollide)
            this.transform.Translate(new Vector3(moveSpeed, 0, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("ClearObject"))
        {
            isCollide = true;
        }
    }
}
