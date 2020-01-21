using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{   
    [Header("STAR PARAMETERS")]
    [SerializeField] int player;
    [SerializeField] GameObject lightTexture;
    [SerializeField] GameObject targetTexture;
    Universe universe;
    Color playerColor;
    enum State { Neutral, ShipFactory}
    State starState;
    float shipTime = 0;
    bool inWar = false;

    [Header("SATELLITES PARAMETERS")]
    [SerializeField] Transform newShipGameobjectTransform;
    [SerializeField] Transform newAsteroidGameobjectTransform;
    [SerializeField] int asteroidCount;
    [SerializeField] float minShipOrbitRadius;
    [SerializeField] float maxShipOrbitRadius;
    [SerializeField] float minEnemyShipOrbitRadius;
    [SerializeField] float maxEnemyShipOrbitRadius;
    List<SpaceShip> friendlyShips = new List<SpaceShip>();
    List<SpaceShip> enemyShips = new List<SpaceShip>();
    List<Asteroid> asteroidList = new List<Asteroid>();
    
    void Start()
    {
        ChangePlayer(player);
    }

    void Update()
    {
        if(inWar) War();
        switch (starState)
        {
            case State.Neutral:
                Neutral();
                break;
            case State.ShipFactory:
                ShipFactory();
                break;
            default:
                Neutral();
                break;
        }
    }

    #region STATE METHODS

    public void SetNeutral()
    {
        starState = State.Neutral;
    }
    public void SetShipFactory()
    {
        starState = State.ShipFactory;
    }
    
    #endregion

    #region UPDATE METHODS

    void Neutral()
    {

    }
    void ShipFactory()
    {
        if(shipTime > 1)
        {
            CreateSpaceShip();
            shipTime = 0;
        }
        else
        {
            shipTime += Time.deltaTime;
        }
    }

    #endregion

    #region SHIPS METHODS

    void CreateSpaceShip()
    {
        SpaceShip provisionalSpaceShip = universe.CreateSpaceShip();

        provisionalSpaceShip.BornInStar(this);
    }
    public void SendHalfShips(Star target)
    {
        int shipsToSend = (friendlyShips.Count / 2);

        for (int i = 0; i <= shipsToSend; i++)
        {
            friendlyShips[0].SetTravelling(target);
            friendlyShips.Remove(friendlyShips[0]);
        }
    }
    public void SendAllShips(Star target)
    {
        int shipsToSend = friendlyShips.Count;

        for (int i = 0; i < shipsToSend; i++)
        {
            friendlyShips[0].SetTravelling(target);
            friendlyShips.Remove(friendlyShips[0]);
        }
    }
    public void AddFriendlyShipToList(SpaceShip friendlyShip)
    {
        friendlyShips.Add(friendlyShip);
    }
    public void AddEnemyShipToList(SpaceShip enemyShip)
    {
        enemyShips.Add(enemyShip);
    }
    public void AddEnemyShipToOrbit(SpaceShip enemyShip)
    {
        AddEnemyShipToList(enemyShip);

        if(friendlyShips.Count > 0)
        {
            if (!inWar) inWar = true;
        }
    }
    public void ShipDead(SpaceShip ship)
    {
        ship.SetInactive();
        universe.RemoveActiveShip(ship);
        universe.AddUnactiveShip(ship);

        if(ship.GetPlayer() == player)
        {
            friendlyShips.Remove(ship);

            if (friendlyShips.Count == 0)
            {
                LostWar();
            }
        }
        else
        {
            enemyShips.Remove(ship);
        }
    }

    #endregion

    #region STAR METHODS

    void War()
    {
        SpaceShip enemyShip = null;
        SpaceShip friendlyShip = null;

        for(int i = 0; i < enemyShips.Count; i++)
        {
            if(!enemyShips[i].GetFighting())
            {
                enemyShip = enemyShips[i];
                break;
            }
        }

        for (int i = 0; i < friendlyShips.Count; i++)
        {
            if (!friendlyShips[i].GetFighting())
            {
                friendlyShip = friendlyShips[i];
                break;
            }
        }

        if(enemyShip != null && friendlyShip != null)
        {
            float randomNum = Random.Range(0.5f, 1.5f);
            enemyShip.SetFighting(randomNum);
            friendlyShip.SetFighting(randomNum);
        }
    }
    void CreateAsteroids()
    {
        for (int i = 0; i < asteroidCount; i++)
        {
            Asteroid provisionalAsteroid = universe.CreateAsteroid();

            provisionalAsteroid.BornInStar(this);
            asteroidList.Add(provisionalAsteroid);
        }
    }
    public void LostWar()
    {
        inWar = false;
        ChangePlayer(enemyShips[0].GetPlayer());

        int totalEnemyShips = enemyShips.Count;

        for (int i = 0; i < totalEnemyShips; i++)
        {
            AddFriendlyShipToList(enemyShips[0]);
            enemyShips.RemoveAt(0);
        }
    }
    public void SetUniverse(Universe everything)
    {
        universe = everything;
    }
    public void ChangePlayer(int _player)
    {
        player = _player;
        playerColor = universe.GetPlayerColor(_player);
        lightTexture.GetComponent<SpriteRenderer>().color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.4f);

        if(player == 0)
        {
            SetNeutral();
            CreateAsteroids();
        }
        else if(starState == State.Neutral)
        {
            SetShipFactory();
        }
    }
    public void AsteroidDestroyed()
    {

    }
    public void Selected(bool isSelected)
    {
        targetTexture.gameObject.SetActive(isSelected);
    }

    #endregion

    #region GETTERS

    public Transform GetShipBirthPoint()
    {
        newShipGameobjectTransform.parent.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
        return newShipGameobjectTransform;
    }
    public Transform GetAsteroidBirthPoint()
    {
        newAsteroidGameobjectTransform.parent.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
        return newAsteroidGameobjectTransform;
    }
    public float GetShipOrbitRadius()
    {
        return Random.Range(minShipOrbitRadius, maxShipOrbitRadius);
    }
    public float GetEnemyShipOrbitRadius()
    {
        return Random.Range(minEnemyShipOrbitRadius, maxEnemyShipOrbitRadius);
    }
    public int GetPlayer()
    {
        return player;
    }
    public Color GetColor()
    {
        return playerColor;
    }

    #endregion

    #region UNITY GIZMOS

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minShipOrbitRadius);
        Gizmos.DrawWireSphere(transform.position, maxShipOrbitRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxEnemyShipOrbitRadius);
        Gizmos.DrawWireSphere(transform.position, minEnemyShipOrbitRadius);
    }

    #endregion
}