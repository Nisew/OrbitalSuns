using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    Universe universe;
    float relativeTime = 0;
    
    [Header("STAR STATS")]
    [SerializeField] Transform newShipTransform;
    [SerializeField] float minShipOrbitRadius;
    [SerializeField] float maxShipOrbitRadius;
    [SerializeField] float minEnemyShipOrbitRadius;
    [SerializeField] float maxEnemyShipOrbitRadius;
        
    List<SpaceShip> friendlyShips = new List<SpaceShip>();
    List<SpaceShip> enemyShips = new List<SpaceShip>();

    [Header("STATE PARAMETERS")]
    [SerializeField] int player;
    Color playerColor;
    [SerializeField] GameObject lightTexture;
    [SerializeField] GameObject targetTexture;

    enum State { Neutral, ShipFactory, ShipWar }
    State starState;

    void Start()
    {
        ChangePlayer(player);
    }

    void Update()
    {
        switch (starState)
        {
            case State.Neutral:
                Neutral();
                break;
            case State.ShipFactory:
                ShipFactory();
                break;
            case State.ShipWar:
                ShipWar();
                break;
            default:
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

    public void SetShipWar()
    {
        starState = State.ShipWar;
    }

    #endregion

    #region UPDATE METHODS

    void Neutral()
    {

    }

    void ShipFactory()
    {
        if(relativeTime > 1)
        {
            CreateSpaceShip();
            relativeTime = 0;
        }
        else
        {
            relativeTime += Time.deltaTime;
        }
    }

    void ShipWar()
    {

    }

    #endregion

    public void Selected(bool isSelected)
    {
        targetTexture.gameObject.SetActive(isSelected);
    }

    public void ChangePlayer(int _player)
    {
        player = _player;
        playerColor = universe.GetPlayerColor(_player);
        lightTexture.GetComponent<SpriteRenderer>().color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.4f);

        if(player == 0)
        {
            SetNeutral();
        }
        else if(starState == State.Neutral)
        {
            SetShipFactory();
        }
    }

    void CreateSpaceShip()
    {
        SpaceShip provisionalSpaceShip = universe.CreateSpaceShip();

        provisionalSpaceShip.BornInStar(this);
        friendlyShips.Add(provisionalSpaceShip);
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

    public void SetUniverse(Universe everything)
    {
        universe = everything;
    }

    #region GETTERS

    public Transform GetShipBirthPoint()
    {
        newShipTransform.parent.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
        Transform shipBirth = newShipTransform;
        return shipBirth;
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

    #region DELETE

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