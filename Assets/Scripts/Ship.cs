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

    [Header("Orbit Elements")]
    Vector3 desiredPosition;
    public float orbitRadius;
    float radiusSpeed = 0.1f;
    float rotationSpeed;
    float orbitWiggle = 2;

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

    public void SetOrbit() //WIGGLE ORBITING
    {
        orbitRadius = Random.Range(orbitalParent.orbitDistance - 0.25f, orbitalParent.orbitDistance + 0.25f);
    }

    public void Orbit() //FREE ORBITING AROUND A STAR
    {
        transform.RotateAround(orbitalParent.transform.position, new Vector3(0, 0, -1), rotationSpeed * Time.deltaTime);
        desiredPosition = (transform.position - orbitalParent.transform.position).normalized * orbitRadius + orbitalParent.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);

        orbitWiggle -= Time.deltaTime;

        if(orbitWiggle <= 0)
        {
            SetOrbit();
            orbitWiggle = 1;
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
            Vector2 difference = (Vector2)destiny.gameObject.transform.position - new Vector2(transform.position.x, transform.position.y);
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

            if(destiny.player != orbitalParent.player)
            {
                destiny.AddEnemyShip(this);
            }
            else if(destiny.player == orbitalParent.player)
            {
                destiny.AddShip(this);
            }

            orbiting = true;
            launching = false;
            orbitalParent = destiny;

        }
    }

    #endregion

    #region SHIP METHODS

    public void Creation() //SET POSITION AND ROTATION TO BE CREATED
    {
        orbiting = true;
        launching = false;
        launched = false;

        rotationSpeed = Random.Range(30, 70);
        launchTime = Random.Range(0.05f, 0.25f);
        speed = Random.Range(1, 1.5f);
        orbitRadius = Random.Range(orbitalParent.orbitDistance - 0.25f, orbitalParent.orbitDistance + 0.25f);
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

    #endregion

}
