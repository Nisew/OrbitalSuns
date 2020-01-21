using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("ORBIT PARAMETERS")]
    Star orbitalParent;
    float orbitRadius;
    float orbitSpeed;
    float rotationSpeed;
    int orbitDirection;
    Vector3 desiredPosition;

    [Header("ASTEROID PARAMETERS")]
    float destroyCounter;
    bool isFighting;

    void Start()
    {
        
    }
    
    void Update()
    {
        Orbiting();
    }

    public void SetFighting(float time)
    {
        destroyCounter = time;
        isFighting = true;
    }

    void SetOrbiting(Star _orbitalParent)
    {
        orbitalParent = _orbitalParent;
        orbitRadius = orbitalParent.GetEnemyShipOrbitRadius();

        Vector2 difference = (Vector2)orbitalParent.transform.position - new Vector2(transform.position.x, transform.position.y);
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        int random = Random.Range(0, 2);
        if (random == 0)
        {
            orbitDirection = 1;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ - 180);
        }
        else
        {
            orbitDirection = -1;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        }        
    }

    void Orbiting()
    {
        transform.RotateAround(orbitalParent.transform.position, new Vector3(0, 0, orbitDirection), orbitSpeed * Time.deltaTime);
        desiredPosition = (transform.position - orbitalParent.transform.position).normalized * orbitRadius + orbitalParent.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * 0.1f);
        transform.Rotate(new Vector3(0, 0, rotationSpeed));

        if (isFighting)
        {
            if (destroyCounter <= 0)
            {
                //orbitalParent.AsteroidDestroyed(this);
                isFighting = false;
            }
            else destroyCounter -= Time.deltaTime;
        }
    }

    public void BornInStar(Star parent)
    {
        this.gameObject.SetActive(true);
        orbitalParent = parent;
        gameObject.transform.position = orbitalParent.GetAsteroidBirthPoint().position;
        SetRandomSize();
        SetRandomSpeed();
        SetOrbiting(parent);
    }

    void SetRandomSize()
    {
        float randomScale = Random.Range(3, 5);
        gameObject.transform.localScale = new Vector3(randomScale, randomScale, 1);
    }

    void SetRandomSpeed()
    {
        orbitSpeed = Random.Range(4, 7);
        int random = Random.Range(0, 2);

        if(random == 0)
        {
            rotationSpeed = Random.Range(0.2f, 0.5f);
        }
        else
        {
            rotationSpeed = Random.Range(-0.2f, -0.5f);
        }
    }

}
