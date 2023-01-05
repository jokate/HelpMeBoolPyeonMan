using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CutterMove : MonoBehaviour
{
    private bool isClicked = false;
    private GameObject target;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isClicked = true;
            CastRay();
        } else if(Input.GetMouseButtonUp(0))
        {
            isClicked = false;
            target = null;
        }
        if (isClicked) {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            if(target != null)
                target.transform.position = new Vector2(mousePos.x, mousePos.y);
        }
    } 
    private void FixedUpdate()
    {
        
    }

    void CastRay() {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Ray2D ray = new Ray2D(mousePos, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if(hit)
            if (hit.collider.gameObject.name == "Cutter")
            {
                target = hit.transform.gameObject;
            }
    }
}
