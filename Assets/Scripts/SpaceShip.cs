using UnityEngine;
using System.Collections;

public class SpaceShip : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    public float time;

    [Header("SPACESHIP")]
    float life = 10;
    float attack = 10;
    float attackSpeed;
    bool shield;
    int civilization;
    [SerializeField] SpriteRenderer sprite;
    enum State { Inactive, Orbiting, Travelling}
    [SerializeField] State shipState;

    [Header("ORBIT")]
    Star orbitalParent;
    Star previousOrbitalParent;
    Vector3 desiredPosition;
    float orbitRadius;
    float orbitSpeed;
    float wiggleSpeed;
    bool enemyParent;
    int orbitDirection;

    [Header("SPACE TRAVEL")]
    Star destiny;
    float launchTime;
    float distance;
    float speed;
    float hyperSpeed;
    float hyperTime = 5;
    public float relativeSpeed;
    public bool hyper = false;
    public bool goingHyper = false;
    public bool stopingHyper = false;

    void Start()
    {
        speed = Random.Range(3, 3);
        hyperSpeed = Random.Range(8, 8);
        relativeSpeed = speed;
        wiggleSpeed = Random.Range(0.5f, 0.7f);
        attackSpeed = Random.Range(0.5f, 1.5f);
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

                if(goingHyper)
                {
                    GoHyper();
                }

                if(stopingHyper)
                {
                    StopHyper();
                }

            break;
        }
    }

    #region SET STATE

    void SetInactive()
    {
        this.gameObject.SetActive(false);
        shipState = State.Inactive;
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

        shipState = State.Orbiting;
    }

    public void SetTravelling(GameObject target)
    {
        distance = Vector2.Distance(orbitalParent.gameObject.transform.position, target.transform.position);
        destiny = target.GetComponent<Star>();
        previousOrbitalParent = orbitalParent;
        NewParent(destiny);

        Vector2 difference = (Vector2)destiny.gameObject.transform.position - new Vector2(transform.position.x, transform.position.y);
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ - 90);

        launchTime = Random.Range(-0.2f, -0.7f);
        time = 0;
        shipState = State.Travelling;
    }

    #endregion

    #region UPDATE METHODS

    void Inactive()
    {
        
    }

    void Orbiting()
    {
        transform.RotateAround(orbitalParent.transform.position, new Vector3(0, 0, orbitDirection), orbitSpeed * Time.deltaTime);
        desiredPosition = (transform.position - orbitalParent.transform.position).normalized * orbitRadius + orbitalParent.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * wiggleSpeed);

        if(time < 5)
        {
            time += Time.deltaTime;

            if(time >= 5)
            {
                time = 0;
                orbitRadius = orbitalParent.GetShipOrbitRadius();
            }
        }

        if(enemyParent)
        {
            if(orbitalParent.GetCiv() == civilization)
            {
                enemyParent = false;
                return;
            }
            attackSpeed -= Time.deltaTime;

            if(attackSpeed <= 0)
            {
                Shoot();
                attackSpeed = Random.Range(0.5f, 2.5f);
            }
        }
    }

    void Travelling()
    {
        if (launchTime < 0) //IGNITION COUNTDOWN
        {
            launchTime += Time.deltaTime;
        }
        else //LAUNCHED
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny.gameObject.transform.position, relativeSpeed * Time.deltaTime);

            if(!CheckDistanceWithOrigin())
            {
                if(!CheckDistanceWithDestiny() && relativeSpeed < hyperSpeed && !goingHyper)
                {
                    goingHyper = true;
                }
                else if(CheckDistanceWithDestiny() && relativeSpeed > speed && !stopingHyper)
                {
                    if (goingHyper)
                    {
                        goingHyper = false;
                    }
                    time = 0;
                    stopingHyper = true;
                }
            }

            if (Vector2.Distance(transform.position, destiny.transform.position) <= orbitRadius) //ARRIVED DESTINY
            {
                SetOrbiting();

                if (destiny.GetCiv() != civilization)
                {
                    enemyParent = true;
                    destiny.AddInEnemyArmy(this);
                }
                else
                {
                    destiny.AddInArmy(this);
                }
            }
        }
    }

    bool CheckDistanceWithOrigin() //RETURNS TRUE IF INSIDE ORIGIN SLOW ZONE
    {
        bool check = false;

        if (Vector2.Distance(transform.position, previousOrbitalParent.transform.position) <= previousOrbitalParent.GetHyperDistance())
        {
            check = true;
        }

        return check;
    }

    bool CheckDistanceWithDestiny() //RETURNS TRUE IF INSIDE DESTINY SLOW ZONE
    {
        bool check = false;

        if (Vector2.Distance(transform.position, orbitalParent.transform.position) <= orbitalParent.GetHyperDistance())
        {
            check = true;
        }

        return check;
    }

    void GoHyper()
    {
        if(time < hyperTime)
        {
            time += Time.deltaTime;
            //relativeSpeed = Easing.CubicEaseIn(time, speed, hyperSpeed, hyperTime);
            relativeSpeed = Mathf.Lerp(speed, hyperSpeed, hyperTime);

            if (time >= hyperTime)
            {
                time = 0;
                goingHyper = false;
                hyper = true;
                relativeSpeed = hyperSpeed;

            }
        }
    }

    void StopHyper()
    {
        if (time < hyperTime)
        {
            time += Time.deltaTime;
            relativeSpeed = Mathf.Lerp(hyperSpeed, speed, hyperTime);

            if (time >= hyperTime)
            {
                time = 0;
                stopingHyper = false;
                hyper = false;
                relativeSpeed = speed;
            }
        }
    }

    #endregion

    #region WAR

    public void Shoot()
    {
        orbitalParent.EnemyShot(this);
    }

    public float GetAttack()
    {
        return attack;
    }

    public void RecieveDamage(float dmg)
    {
        life -= dmg;
    }

    public bool IsDestroyed()
    {
        if (life <= 0) return true;
        else return false;
    }

    public int VictoryFlag()
    {
        return civilization;
    }

    #endregion

    #region CYCLE OF LIFE

    public void Create()
    {
        gameObject.transform.position = orbitalParent.GetSatelliteBirthPoint();
        civilization = orbitalParent.GetCiv();
        SetOrbiting();
    }

    public void Tint(Color color)
    {
        sprite.color = color;
    }

    public void NewParent(Star parent)
    {
        orbitalParent = parent;
        orbitSpeed = orbitalParent.GetShipOrbitSpeed();
        orbitRadius = orbitalParent.GetShipOrbitRadius();
    }

    public void Destroy(SpaceShip ship)
    {
        SetInactive();
        orbitalParent.RemoveFromArmy(ship);
        orbitalParent.ShipDestroyed(ship);
    }

    public void DestroyEnemy(SpaceShip enemyShip)
    {
        SetInactive();
        orbitalParent.RemoveFromEnemyArmy(enemyShip);
        orbitalParent.ShipDestroyed(enemyShip);
    }

    #endregion
}
