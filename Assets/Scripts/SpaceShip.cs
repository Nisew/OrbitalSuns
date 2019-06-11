using UnityEngine;
using System.Collections;

public class SpaceShip : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    float time;

    [Header("SPACESHIP")]
    float life;
    float destructionTime = 2;
    int civilization;
    [SerializeField] SpriteRenderer sprite;
    enum State { Inactive, Orbiting, Travelling}
    [SerializeField] State shipState;

    [Header("ORBIT")]
    Star orbitalParent;
    Vector3 desiredPosition;
    float orbitRadius;
    float orbitSpeed;
    float wiggleSpeed;
    int orbitDirection;

    [Header("SPACE TRAVEL")]
    Star destiny;
    float speed;

    void Start()
    {
        speed = Random.Range(1.9f, 2.1f);
        wiggleSpeed = Random.Range(0.5f, 0.7f);
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
        time += Time.deltaTime;

        if (time < 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, orbitalParent.gameObject.transform.position, Time.deltaTime * speed);
        }
        else
        {
            transform.RotateAround(orbitalParent.transform.position, new Vector3(0, 0, orbitDirection), orbitSpeed * Time.deltaTime);
            desiredPosition = (transform.position - orbitalParent.transform.position).normalized * orbitRadius + orbitalParent.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * wiggleSpeed);

            if(time >= 5)
            {
                time = 0;
                orbitRadius = orbitalParent.GetShipOrbitRadius();
            }
        }
    }

    void Travelling()
    {
        if(time > 0)
        {
            time -= Time.deltaTime;            
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny.gameObject.transform.position, Time.deltaTime * speed);

            if (Vector2.Distance(transform.position, destiny.transform.position) <= 0)
            {
                if (destiny.GetCiv() != civilization)
                {
                    destiny.AddInEnemyArmy(this);
                }
                else
                {
                    destiny.AddInArmy(this);
                }

                NewParent(destiny);
                SetOrbiting();
            }
        }
    }

    #endregion

    public void Tint(Color color)
    {
        sprite.color = color;
    }

    public void NewParent(Star parent)
    {
        civilization = parent.GetCiv();
        orbitalParent = parent;
        orbitSpeed = parent.GetShipOrbitSpeed();
        orbitRadius = orbitalParent.GetShipOrbitRadius();
    }

    public void Create()
    {
        gameObject.transform.localPosition = orbitalParent.GetSatelliteBirthPoint();
        civilization = orbitalParent.GetCiv();
        SetOrbiting();
        this.gameObject.SetActive(true);
    }

    #region SET STATE

    void SetInactive()
    {
        this.gameObject.SetActive(false);
    }

    void SetOrbiting()
    {
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

        time = Random.Range(-1, -0.1f);

        shipState = State.Orbiting;
    }

    public void SetTravelling(GameObject target)
    {
        destiny = target.GetComponent<Star>();
        Vector2 difference = (Vector2)destiny.gameObject.transform.position - new Vector2(transform.position.x, transform.position.y);
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ - 90);
        time = Random.Range(0.2f, 0.7f);
        shipState = State.Travelling;
    }

    #endregion  

}
