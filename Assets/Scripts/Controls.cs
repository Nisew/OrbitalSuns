using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    Universe universe;
    Vector3 touchStart;

	void Start ()
    {
        universe = GameObject.FindGameObjectWithTag("Universe").GetComponent<Universe>();
    }	
	void Update ()
    {
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) //CLIC
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D rayClic = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);

            if (rayClic && rayClic.collider != null)
            {
                universe.StarTouched(rayClic.collider.GetComponent<Star>());
            }
            else
            {
                universe.VoidTouched();
            }
        }
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) //PANNING
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            direction.z = 0;
            Camera.main.transform.position += direction;
        }
        else if (Input.touchCount == 2) //ZOOM
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * 0.1f);

            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.GetTouch(1).position);
            }
            if (Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
        }
        
        if(Input.GetMouseButtonDown(0)) //CLIC MOUSE
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D rayClic = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (rayClic && rayClic.collider != null)
            {
                universe.StarTouched(rayClic.collider.GetComponent<Star>());
            }
            else
            {
                universe.VoidTouched();
            }
        }
        if(Input.GetMouseButton(0)) //PANNING MOUSE
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction.z = 0;
            Camera.main.transform.position += direction;
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel") * 20); //ZOOM MOUSE

    }
    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, 10, 80);
    }
    
}
