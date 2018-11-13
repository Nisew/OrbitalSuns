using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
    [Header("Space Elements")]
    public Star orbitalParent;

    [Header("Orbit Elements")]
    Vector3 desiredPosition;
    public float orbitRadius;
    float radiusSpeed = 0.5f;
    float rotationSpeed;
    public bool orbiting;


    void Start()
    {
        
    }

    void Update()
    {
        if(orbiting)
        {
            transform.RotateAround(orbitalParent.transform.position, new Vector3(0, 0, -1), rotationSpeed * Time.deltaTime);
            desiredPosition = (transform.position - orbitalParent.transform.position).normalized * orbitRadius + orbitalParent.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
        }
    }

    public void Creation()
    {
        rotationSpeed = Random.Range(50, 100);
        orbitRadius = Random.Range(orbitalParent.orbitDistance - 0.5f, orbitalParent.orbitDistance + 0.5f);
    }
}
