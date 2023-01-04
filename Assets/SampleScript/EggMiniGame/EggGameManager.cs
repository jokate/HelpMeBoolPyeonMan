using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

public class EggGameManager : MonoBehaviour
{

    public static int Level = 2;
    public GameObject originalEgg, downPos, upPos;
    public List<GameObject> eggShells;
    public List<GameObject> speggShells;
    private GameObject target;
    private bool isFailed = false;

    private void Start()
    {
        for(int i = 0; i < Level; i++)
        {
            float x = Random.Range(downPos.transform.position.x, upPos.transform.position.x);
            float y = Random.Range(downPos.transform.position.y, upPos.transform.position.y);
            int number = Random.Range(0, eggShells.Count);
            Debug.Log(number);
            GameObject speggshell = Instantiate(eggShells[number], originalEgg.transform);
            speggshell.transform.position = new Vector2(x, y);
            speggShells.Add(speggshell);
        }
        StartCoroutine(SuccessCheck());
        StartCoroutine(FailedCheck());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
    }

    private void FixedUpdate()
    {

    }
    private void CastRay() {

        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Ray2D ray = new Ray2D(mousePos, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit)
        {
            target = hit.transform.gameObject;
            if (target != null && target.name == originalEgg.name)
            {
                isFailed = true;
            } else if (target != null && speggShells.Contains(target))
            {
                speggShells.Remove(target);
                Destroy(target);

            }
        }
    }
    IEnumerator FailedCheck()
    {
        yield return new WaitUntil(() => isFailed == true);
        Debug.Log("oh no...");
        
        originalEgg.GetComponent<Animator>().SetBool("isFailed", true);
    }
    IEnumerator SuccessCheck()
    {
        yield return new WaitUntil(() => speggShells.Count == 0);
        Debug.Log("Good");
        this.enabled = false;
    }
}
