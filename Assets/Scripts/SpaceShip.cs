using UnityEngine;
using System.Collections;

public class SpaceShip : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    float time;

    [Header("SPACESHIP")]
    float life = 10;
    float attack = 10;
    float attackSpeed;
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
    bool enemyParent;
    int orbitDirection;

    [Header("SPACE TRAVEL")]
    Star destiny;
    float speed;

    void Start()
    {
        speed = Random.Range(1.9f, 2.1f);
        wiggleSpeed = Random.Range(0.5f, 0.7f);
        attackSpeed = Random.Range(0.5f, 2.5f);
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
        destiny = target.GetComponent<Star>();
        NewParent(destiny);

        Vector2 difference = (Vector2)destiny.gameObject.transform.position - new Vector2(transform.position.x, transform.position.y);
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ - 90);

        time = Random.Range(0.2f, 0.7f);
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

        if(time >= 5)
        {
            time = 0;
            orbitRadius = orbitalParent.GetShipOrbitRadius();
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
        if(time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny.gameObject.transform.position, Time.deltaTime * speed);

            if (Vector2.Distance(transform.position, destiny.transform.position) <= orbitRadius)
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
