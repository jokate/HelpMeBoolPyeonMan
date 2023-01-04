using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class UsbMover : MonoBehaviour
{
    public GameObject target, startpos, usbSetter, noteBookMover;
    private bool isTouched = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ClearChecker());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isTouched = true;
        } else if(Input.GetMouseButtonUp(0))
        {
            isTouched = false;
        }
    }
    private void FixedUpdate()
    {
        if (!noteBookMover.GetComponent<NoteBookMover>().isCollide)
        {
            if (isTouched)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, target.transform.position.y), 0.075f);

            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, startpos.transform.position.y), 0.075f);
            }
        }
    }
    IEnumerator ClearChecker()
    {
        yield return new WaitUntil(() => noteBookMover.GetComponent<NoteBookMover>().isCollide == true);
        if(usbSetter.GetComponent<UsbSetter>().isClear)
        {
            float time = 1f;
            while(time > 0.0f)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x-0.1f, transform.position.y), 0.001f);
                time -= Time.deltaTime;
                yield return null;
            }
        } else
        {
            this.gameObject.GetComponent<Animator>().SetBool("isFailed", true);
            Debug.Log("Oh no..");
        }
    }
}
