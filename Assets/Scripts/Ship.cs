using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
    [Header("Space Elements")]
    public Star orbitalParent;
    public float counter;

    [Header("Ship Elements")]
    public bool orbiting;
    public float live;

    [Header("Orbit Elements")]
    Vector3 desiredPosition;
    public float orbitRadius;
    float radiusSpeed = 0.5f;
    float rotationSpeed;

    [Header("Transfer Elements")]
    public Vector2 destiny;
    float speed;
    float launchTime;
    float orbitTime = 3;

    void Start()
    {
        Creation();
    }

    void Update()
    {
        if(orbiting)
        {
            ToOrbit();
            Orbit();
        }
        else
        {
            Transfer();
        }
    }

    public void ToOrbit()
    {
        if(counter < orbitTime)
        {
            counter += Time.deltaTime;
        }

        if(counter >= orbitTime)
        {
            orbitalParent.AddShip(this);
        }
    }

    public void Orbit()
    {
        transform.RotateAround(orbitalParent.transform.position, new Vector3(0, 0, -1), rotationSpeed * Time.deltaTime);
        desiredPosition = (transform.position - orbitalParent.transform.position).normalized * orbitRadius + orbitalParent.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }

    public void Transfer()
    {
        transform.position = Vector2.MoveTowards(transform.position, destiny, Time.deltaTime * speed);
    }

    public void Launch(Vector2 destination)
    {
        Vector2 difference = destination - new Vector2(transform.position.x, transform.position.y);
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ - 90);

        destiny = destination;
        orbiting = false;
    }

    public void Creation()
    {
        rotationSpeed = Random.Range(50, 100);
        launchTime = Random.Range(0.1f, 0.5f);
        speed = Random.Range(0.8f, 1);
        orbitRadius = Random.Range(orbitalParent.orbitDistance - 0.25f, orbitalParent.orbitDistance + 0.25f);
    }
}
