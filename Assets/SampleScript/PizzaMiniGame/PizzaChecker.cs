using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PizzaChecker : MonoBehaviour
{
    private Animator anim;
    private Vector2 originalPos;
    private float dist;
    private bool isFailed = false;
    // Start is called before the first frame update
    void Start()
    {

        anim = GetComponent<Animator>();
        StartCoroutine(TimeCheck());
        StartCoroutine(FailedCheck());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator TimeCheck()
    {
        while(true)
        {
            originalPos = transform.position;
            float time = 0.1f;
            while(time > 0.0f)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            dist = ((Vector2)transform.position - originalPos).magnitude;
            if (dist > 1.5f)
            {
                isFailed = true;
                gameObject.GetComponentInParent<CutterMove>().enabled = false;
                break;
            }
        }
    }
    IEnumerator FailedCheck()
    {
        yield return new WaitUntil(() => isFailed == true);
        anim.SetBool("isFailed", true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ClearObject"))
        {
            Debug.Log("Clear?");
            StopAllCoroutines();
            anim.SetBool("isSuccess", true);
            gameObject.GetComponentInParent<CutterMove>().enabled = false;
        }
    }
}
