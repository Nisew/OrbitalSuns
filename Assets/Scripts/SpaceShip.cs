using UnityEngine;
using System.Collections;

public class SpaceShip : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    Star orbitalParent;
    float time;

    [Header("SPACESHIP")]
    float life;
    float destructionTime = 2;
    bool orbiting;
    bool crashed;
    int civilization;
    [SerializeField] SpriteRenderer sprite;
    enum State { Inactive, Orbiting, Travelling}
    [SerializeField] State shipState;

    [Header("ORBIT")]
    Vector3 desiredPosition;
    float orbitRadius;
    float orbitSpeed = 75;
    float orbitWiggle;
    float wiggleSpeed = 0.5f;
    int orbitDirection;

    [Header("SPACE TRAVEL")]
    Star destiny;
    float speed = 2;

    void Start()
    {
        speed = Random.Range(1.8f, 2.2f);
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

    public void Tint(Color color)
    {
        sprite.color = color;
    }

    public void Create(Star parent, Vector3 pos, int civ, float _life, float _minOrbitRadius, float _maxOrbitRadius)
    {
        orbitalParent = parent;
        desiredPosition = pos;
        civilization = civ;

        SetOrbit(_minOrbitRadius, _maxOrbitRadius);
        SetLife(_life);
        SetPosition();
        this.gameObject.SetActive(true);
    }

    public void SetPosition()
    {
        gameObject.transform.position = desiredPosition;
    }

    public void SetOrbit(float _orbitRadius, float _maxOrbitRadius)
    {
        if (orbitalParent.typeOfStar() == "Blue")
        {
            orbitSpeed = Random.Range(60, 80);
            orbitRadius = Random.Range(_orbitRadius, _maxOrbitRadius);
        }

        if (orbitalParent.typeOfStar() == "Yellow")
        {
            orbitSpeed = Random.Range(40, 60);
            orbitRadius = Random.Range(_orbitRadius, _maxOrbitRadius);
        }

        if (orbitalParent.typeOfStar() == "Red")
        {
            orbitSpeed = Random.Range(15, 35);
            orbitRadius = Random.Range(_orbitRadius, _maxOrbitRadius);
        }
        orbitWiggle = orbitRadius;
    }

    public void SetLife(float _life)
    {
        life = _life;
    }

    void Inactive()
    {
        if(this.gameObject.activeInHierarchy)
        {
            SetOrbiting(orbitalParent);
        }
    }

    void Orbiting()
    {
        transform.RotateAround(orbitalParent.transform.position, new Vector3(0, 0, orbitDirection), orbitSpeed * Time.deltaTime);
        desiredPosition = (transform.position - orbitalParent.transform.position).normalized * orbitRadius + orbitalParent.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * wiggleSpeed);

        time += Time.deltaTime;

        if(time >= 5)
        {
            time = 0;
            orbitRadius = Random.Range(orbitWiggle - 0.25f, orbitWiggle + 0.25f);
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

            if (Vector2.Distance(transform.position, destiny.transform.position) < destiny.maxOrbitRadius)
            {
                if (destiny.civilization != civilization)
                {
                    destiny.AddInEnemyArmy(this);
                }
                else
                {
                    destiny.AddInArmy(this);
                }

                SetOrbiting(destiny);
                time = 0;
            }
        }
    }

    void SetInactive()
    {
        this.gameObject.SetActive(false);
    }

    void SetOrbiting(Star _orbitalParent)
    {
        orbitalParent = _orbitalParent;
        SetOrbit(orbitalParent.minOrbitRadius, orbitalParent.maxOrbitRadius);

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

    public void SetTravelling(GameObject target)
    {
        destiny = target.GetComponent<Star>();
        Vector2 difference = (Vector2)destiny.gameObject.transform.position - new Vector2(transform.position.x, transform.position.y);
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ - 90);
        time = Random.Range(0.2f, 0.7f);
        shipState = State.Travelling;
    }

/*
    void Update()
    {
        if(orbiting) //IN ORBIT
        {
            Orbit();

            if(crashed)
            {
                destructionTime -= Time.deltaTime;

                if(destructionTime <= 0)
                {
                    Destruction();
                }
            }
        }

        if(launching) //IN INTERSTELLAR SPACE
        {
            if(launchTime > 0)
            {
                launchTime -= Time.deltaTime;
            }
            else
            {
                if(!launched)
                {
                    launched = true;
                    orbitalParent.RemoveShip(this);
                }

                Transfer();
            }
        }
    }

    #region ORBIT METHODS

    public void SetOrbitDirection() //ORBITING CLOCKWISE OR NOT
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
    }

    public void SetOrbitRadius() //WIGGLE ORBITING
    {
        orbitRadius = Random.Range(orbitalParent.orbitDistance - 0.25f, orbitalParent.orbitDistance + 0.25f);
    }

    public void Orbit() //FREE ORBITING AROUND A STAR
    {
        transform.RotateAround(orbitalParent.transform.position, new Vector3(0, 0, orbitDirection), rotationSpeed * Time.deltaTime);
        desiredPosition = (transform.position - orbitalParent.transform.position).normalized * orbitRadius + orbitalParent.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);

        orbitWiggle -= Time.deltaTime;

        if(orbitWiggle <= 0)
        {
            SetOrbitRadius();
            orbitWiggle = 5;
        }
    }

    #endregion

    #region INTERSTELLAR TRAVEL METHODS

    public void Launch(Star destination) //SET DESTINY AND ROTATION TO ANOTHER STAR
    {
        destiny = destination;
        Vector2 difference = (Vector2)destiny.gameObject.transform.position - new Vector2(transform.position.x, transform.position.y);
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ - 90);
        orbiting = false;
        launching = true;
    }

    public void Transfer() //MOVEMENT FROM ONE STAR TO ANOTHER
    {
        transform.position = Vector2.MoveTowards(transform.position, destiny.gameObject.transform.position, Time.deltaTime * speed);

        if(Vector2.Distance(transform.position, destiny.transform.position) < destiny.orbitDistance)
        {

            if (destiny.player != player)
            {
                destiny.AddEnemyShip(this);
            }
            else if(destiny.player == player)
            {
                destiny.AddShip(this);
            }
            
            orbitalParent = destiny;
            SetOrbitDirection();
            ResetRandom();
            Creation();
        }
    }

    #endregion

    #region SHIP METHODS

    public void Creation() //SET SHIP FOR PLACEMENT
    {
        orbiting = true;
        launching = false;
        launched = false;
        crashed = false;
    }
    
    public void CrushAnotherShip(Ship ship) //SHIPS MAKING WAR
    {
        float a = ship.life;
        ship.life -= life;
        life -= a;
    }

    public void Destruction() //DESACTIVATE THE SHIP
    {
        this.gameObject.SetActive(false);
    }

    public void ResetRandom() //RESET SHIP RANDOM VALUES
    {
        SetOrbitDirection();
        rotationSpeed = Random.Range(30, 70);
        launchTime = Random.Range(0.05f, 0.25f);
        speed = Random.Range(2, 2.5f);
        destructionTime = Random.Range(1, 2);
        orbitRadius = Random.Range(orbitalParent.orbitDistance - 0.25f, orbitalParent.orbitDistance + 0.25f);
    }

    #endregion
    */

}
