using UnityEngine;
using System.Collections;

public class SpaceShip : MonoBehaviour
{
    float time;
    int player;
    
    enum State { Inactive, Orbiting, Travelling}
    State shipState;

    [Header("ORBIT PARAMETERS")]
    Star orbitalParent;
    Vector3 desiredPosition;
    float orbitRadius;
    float orbitSpeed;
    float wiggleSpeed;
    int orbitDirection;

    [Header("TRAVEL PARAMETERS")]
    Star destiny;
    float launchTime;
    float speed = 5;

    void Start()
    {

    }

    public void Update()
    {
        switch(shipState)
        {
            case State.Inactive:
                Inactive();
            break;

            case State.Orbiting:
                Orbiting();
            break;

            case State.Travelling:
                Travelling();
            break;
        }
    }

    #region UPDATE METHODS

    void Inactive()
    {
        
    }

    void Orbiting()
    {
        transform.RotateAround(orbitalParent.transform.position, new Vector3(0, 0, orbitDirection), orbitSpeed * Time.deltaTime);
        desiredPosition = (transform.position - orbitalParent.transform.position).normalized * orbitRadius + orbitalParent.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * wiggleSpeed);

        if (time >= 5)
        {
            time = 0;
            orbitRadius = GetOrbitRadius();
        }
        else time += Time.deltaTime;
    }

    void Travelling()
    {
        transform.position = Vector2.MoveTowards(transform.position, destiny.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, destiny.transform.position) <= destiny.GetEnemyShipOrbitRadius()) //ARRIVED DESTINY
        {
            orbitRadius = GetOrbitRadius();
            SetOrbiting(destiny);
        }
    }

    #endregion

    float GetOrbitRadius()
    {
        if (orbitalParent.GetPlayer() == player)
        {
            return orbitalParent.GetShipOrbitRadius();
        }
        else return orbitalParent.GetEnemyShipOrbitRadius();
    }

    void SetInactive()
    {
        this.gameObject.SetActive(false);
        shipState = State.Inactive;
    }

    void SetOrbiting(Star _orbitalParent)
    {
        orbitalParent = _orbitalParent;

        if(orbitalParent.GetPlayer() == player)
        {
            orbitalParent.AddFriendlyShipToList(this);
        }
        else
        {
            orbitalParent.AddEnemyShipToList(this);
        }

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

        shipState = State.Orbiting;
    }

    public void SetTravelling(Star target)
    {
        transform.up = target.transform.position - transform.position;
        destiny = target;
        shipState = State.Travelling;
    }
    
    public void BornInStar(Star parent)
    {
        orbitalParent = parent;
        player = orbitalParent.GetPlayer();
        gameObject.transform.position = orbitalParent.GetShipBirthPoint().position;
        GetComponentInChildren<SpriteRenderer>().color = parent.GetColor();

        SetOrbitSpeed();
        SetOrbiting(parent);
    }

    void SetOrbitSpeed()
    {
        orbitRadius = orbitalParent.GetShipOrbitRadius();
        orbitSpeed = Random.Range(70, 100);
        wiggleSpeed = Random.Range(0.5f, 1);
    }
    
    public void Destroy(SpaceShip ship)
    {

    }
    
}
