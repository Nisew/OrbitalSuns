using UnityEngine;
using System.Collections;

public class SpaceShip : MonoBehaviour
{
    [Header("SHIP PARAMETERS")]
    int player;
    float time;
    enum State { Inactive, Orbiting, Travelling}
    State shipState;
    bool isFighting;
    float destroyCounter;

    [Header("ORBIT PARAMETERS")]
    Star orbitalParent;
    float orbitRadius;
    float orbitSpeed;
    float orbitLateralSpeed;
    int orbitDirection;
    Vector3 desiredPosition;

    [Header("TRAVEL PARAMETERS")]
    Star destiny;
    float launchTime;
    float interstellarSpeed = 5;

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
        transform.position = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * orbitLateralSpeed);

        if (time >= 2.5f)
        {
            time = 0;
            orbitRadius = GetOrbitRadius();
        }
        else time += Time.deltaTime;

        if(isFighting)
        {
            if (destroyCounter <= 0)
            {
                orbitalParent.ShipDead(this);
                isFighting = false;
            }
            else destroyCounter -= Time.deltaTime;
        }
    }
    void Travelling()
    {
        transform.position = Vector2.MoveTowards(transform.position, destiny.transform.position, interstellarSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, destiny.transform.position) <= destiny.GetEnemyShipOrbitRadius()) //ARRIVED DESTINY
        {
            SetRandomSpeed();
            SetOrbiting(destiny);
        }
    }

    #endregion

    #region SET METHODS
    
    public void SetInactive()
    {
        this.gameObject.SetActive(false);
        shipState = State.Inactive;
    }
    void SetOrbiting(Star _orbitalParent)
    {
        orbitalParent = _orbitalParent;
        orbitRadius = GetOrbitRadius();

        if(orbitalParent.GetPlayer() == player)
        {
            orbitalParent.AddFriendlyShipToList(this);
        }
        else
        {
            orbitalParent.AddEnemyShipToOrbit(this);
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
    public void SetFighting(float time)
    {
        destroyCounter = time;
        isFighting = true;
    }

    #endregion

    #region GET METHODS

    float GetOrbitRadius()
    {
        if (orbitalParent.GetPlayer() == player)
        {
            return orbitalParent.GetShipOrbitRadius();
        }
        else return orbitalParent.GetEnemyShipOrbitRadius();
    }
    public int GetPlayer()
    {
        return player;
    }
    public bool GetFighting()
    {
        return isFighting;
    }

    #endregion
    
    public void BornInStar(Star parent)
    {
        this.gameObject.SetActive(true);
        orbitalParent = parent;
        player = orbitalParent.GetPlayer();
        gameObject.transform.position = orbitalParent.GetShipBirthPoint().position;
        GetComponentInChildren<SpriteRenderer>().color = parent.GetColor();

        SetRandomSpeed();
        SetOrbiting(parent);
    }
    void SetRandomSpeed()
    {
        orbitSpeed = Random.Range(40, 60);
        orbitLateralSpeed = Random.Range(1.5f, 2);
        interstellarSpeed = Random.Range(8, 9);
    }
    
}
