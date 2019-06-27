using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    Universe universe;
    float time = 2;

    [Header("STAR INFO")]
    float temperature;
    enum starType { Yellow, Blue, Red };
    [SerializeField]
    starType type;

    [Header("STAR STATS")]
    [SerializeField]
    Transform newSatellitePoint;
    [SerializeField]
    Transform newSatellitePointChild;
    [SerializeField]
    int civilization;

    public float minShipOrbitRadius;
    public float maxShipOrbitRadius;
    float shipOrbitSpeed;
    int maxShips;

    public float minPanelOrbitRadius;
    public float maxPanelOrbitRadius;
    float panelOrbitSpeed;
    int maxPanels;

    [Header("SATELLITE STATS")]
    float shipCost = 100;
    float shipAttack;
    float shipLife;
    bool shipShield;

    [Header("ENERGY FLOW")]
    float energyWheel;
    [SerializeField]
    float energyOutput;
    float armyEnergyRatio;
    [SerializeField]
    float armyEnergy;
    float investEnergyRatio;
    [SerializeField]
    float investEnergy;

    [Header("SATELLITE ARMY")]
    [SerializeField]
    List<SpaceShip> Ships = new List<SpaceShip>();
    //[SerializeField] List<SolarPanel> Panels = new List<SolarPanel>();
    List<SpaceShip> enemyShips = new List<SpaceShip>();
    int launchingShips;
    Vector2 target;

    [Header("SPRITE")]
    [SerializeField]
    SpriteRenderer starSprite;

    void Start()
    {
        if (type == starType.Yellow)
        {
            shipOrbitSpeed = 50;
        }
        else if (type == starType.Blue)
        {
            shipOrbitSpeed = 70;
        }
        else if (type == starType.Red)
        {
            shipOrbitSpeed = 20;
        }
    }

    void Update()
    {
        EnergyFlow();

        if (enemyShips.Count > 0)
        {
            //ALERT! ENEMIES

            if (Ships.Count <= 0)
            {
                Surrender();
            }
        }
    }

    public void CivShipStats(float _shipLife, float _shipCost, float _shipAttack, bool _shipShield)
    {
        shipLife = _shipLife;
        shipCost = _shipCost;
        shipAttack = _shipAttack;
        shipShield = _shipShield;
    }

    #region ORBIT METHODS

    public Vector3 GetSatelliteBirthPoint()
    {
        return newSatellitePointChild.transform.position;
    }

    #region SHIPS

    public float GetShipOrbitRadius()
    {
        return Random.Range(minShipOrbitRadius, maxShipOrbitRadius);
    }

    public float GetShipOrbitSpeed()
    {
        return Random.Range(shipOrbitSpeed - 7.5f, shipOrbitSpeed + 7.5f);
    }

    #endregion

    #endregion

    #region LIST METHODS

    public void AddInArmy(SpaceShip satellite)
    {
        if (!Ships.Contains(satellite))
        {
            Ships.Add(satellite);
        }
        else Debug.Log("Ship already in the star army list");
    }

    public void RemoveFromArmy(SpaceShip satellite)
    {
        if (Ships.Contains(satellite))
        {
            Ships.Remove(satellite);
        }
        else Debug.Log("Ship already out of star army list");
    }

    public void AddInEnemyArmy(SpaceShip enemySatellite)
    {
        if (!enemyShips.Contains(enemySatellite))
        {
            enemyShips.Add(enemySatellite);
        }
        else Debug.Log("Ship already in the star enemies orbiting list");
    }

    public void RemoveFromEnemyArmy(SpaceShip enemySatellite)
    {
        if (enemyShips.Contains(enemySatellite))
        {
            enemyShips.Remove(enemySatellite);
        }
        else Debug.Log("Ship already out of star enemies orbiting list");
    }

    #endregion

    #region STAR STATS

    public void SetProperties(float _temperature, float _volume, float _energy)
    {
        starSprite.transform.localScale = new Vector3(_volume, _volume, 1);

        minShipOrbitRadius *= _volume;
        maxShipOrbitRadius *= _volume;
        minPanelOrbitRadius *= _volume;
        maxPanelOrbitRadius *= _volume;
        GetComponent<CircleCollider2D>().radius *= _volume;
        newSatellitePointChild.transform.localPosition = new Vector2(newSatellitePointChild.transform.localPosition.x + GetComponent<CircleCollider2D>().radius, 0);

        temperature = _temperature;
        energyOutput = _energy;
    }

    public string typeOfStar()
    {
        return type.ToString();
    }

    public int GetCiv()
    {
        return civilization;
    }

    #endregion

    #region WAR

    public void SendArmy(GameObject target, bool all)
    {
        if (all)
        {
            launchingShips = Ships.Count;
            for (int i = 0; i < launchingShips; i++)
            {
                Ships[0].SetTravelling(target);
                RemoveFromArmy(Ships[0]);
            }
        }
        else
        {
            launchingShips = Ships.Count;

            if(launchingShips == 1)
            {
                Ships[0].SetTravelling(target);
                RemoveFromArmy(Ships[0]);
            }
            else
            {
                for (int i = 0; i < launchingShips / 2; i++)
                {
                    Ships[0].SetTravelling(target);
                    RemoveFromArmy(Ships[0]);
                }
            }
        }
    }

    public void EnemyShot(SpaceShip enemyShip)
    {
        enemyShip.RecieveDamage(Ships[0].GetAttack());
        Ships[0].RecieveDamage(enemyShip.GetAttack());

        if (enemyShip.IsDestroyed()) enemyShip.DestroyEnemy(enemyShip);
        if (Ships[0].IsDestroyed()) Ships[0].Destroy(Ships[0]);
    }

    public void ShipDestroyed(SpaceShip ship)
    {
        universe.AddUnactiveSpaceShip(ship.gameObject);
    }

    void Surrender()
    {
        SpaceShip enemyShip;

        civilization = enemyShips[0].VictoryFlag();

        for (int i = 0; i < enemyShips.Count; i++)
        {
            enemyShip = enemyShips[i];
            RemoveFromEnemyArmy(enemyShip);
            AddInArmy(enemyShip);
        }
    }

    #endregion

    #region STAR CREATIONS

    void EnergyFlow()
    {
        investEnergyRatio = (energyOutput / 100) * energyWheel;
        armyEnergyRatio = energyOutput - investEnergyRatio;

        armyEnergy += armyEnergyRatio * Time.deltaTime / time;
        investEnergy += investEnergyRatio * Time.deltaTime / time;

        if (armyEnergy >= shipCost)
        {
            armyEnergy = 0;
            SpaceShip();
        }
    }

    void SpaceShip()
    {
        GameObject ship = universe.CreateSpaceShip(civilization);
        SpaceShip shipScript = ship.GetComponent<SpaceShip>();

        newSatellitePoint.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        shipScript.NewParent(this);
        shipScript.Create();

        AddInArmy(shipScript);
    }

    #endregion

    public float GetHyperDistance()
    {
        return maxPanelOrbitRadius;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minShipOrbitRadius);
        Gizmos.DrawWireSphere(transform.position, maxShipOrbitRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minPanelOrbitRadius);
        Gizmos.DrawWireSphere(transform.position, maxPanelOrbitRadius);
    }

    public void SetUniverse(Universe everything)
    {
        universe = everything;
    }

}