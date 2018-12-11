using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
    [Header("Space Elements")]
    public Star orbitalParent;
    public float counter;

    [Header("Ship Elements")]
    public bool orbiting;
    public int player;
    public float life;
    public float destructionTime = 2;
    public bool crashed;

    [Header("Orbit Elements")]
    Vector3 desiredPosition;
    public float orbitRadius;
    float radiusSpeed = 0.3f;
    float rotationSpeed;
    float orbitWiggle = 5;
    int orbitDirection;

    [Header("Transfer Elements")]
    public Star destiny;
    float speed;
    float launchTime;
    bool launching;
    bool launched;

    void Start()
    {

    }

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

}
