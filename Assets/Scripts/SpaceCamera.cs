using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceCamera : MonoBehaviour
{
    [Header("Camera Elements")]
    Camera cam;

    [Header("Zoom Elements")]
    float zoomSpeed = 1;

    [Header("Drag Elements")]
    float dragSpeed = 15;
    bool panning;

	void Start ()
    {
        cam = Camera.main;
    }
	
	void LateUpdate ()
    {
        if(!panning)
        {
            if(Input.GetMouseButtonDown(1))
            {
                panning = true;
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if(cam.orthographicSize > 5)
                {
                    cam.orthographicSize -= zoomSpeed;
                    dragSpeed --;
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if(cam.orthographicSize < 40)
                {
                    cam.orthographicSize += zoomSpeed;
                    dragSpeed ++;
                }
            }
        }

        if(panning)
        {
            if(Input.GetMouseButtonUp(1))
            {
                panning = false;
            }

            float movementX = transform.position.x + Input.GetAxisRaw("Mouse X") * Time.deltaTime * -dragSpeed;
            float movementY = transform.position.y + Input.GetAxisRaw("Mouse Y") * Time.deltaTime * -dragSpeed;

            cam.transform.position = new Vector3(movementX, movementY, -10);
        }
    }
}
