using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    Universe universe;
    float time;

    [Header("STAR INFO")]
    float temperature;
    enum starType { Yellow, Blue, Red };
    [SerializeField] starType type;

    [Header("STAR STATS")]
    [SerializeField] Transform newSatellitePoint;
    [SerializeField] Transform newSatellitePointChild;
    [SerializeField] int civilization;

    public float minShipOrbitRadius;
    public float maxShipOrbitRadius;
    float shipOrbitSpeed;
    int maxShips;

    public float minPanelOrbitRadius;
    public float maxPanelOrbitRadius;
    float panelOrbitSpeed;
    int maxPanels;

    [Header("SATELLITE STATS")]
    float shipCost;
    float shipAttack;
    float shipLife;
    float shipShield;

    [Header("ENERGY FLOW")]
    float energyOutput;
    float armyEnergy;
    float investEnergy;

    [Header("SATELLITE ARMY")]
    [SerializeField] List<SpaceShip> Ships = new List<SpaceShip>();
    //[SerializeField] List<SolarPanel> Panels = new List<SolarPanel>();
    List<SpaceShip> enemyShips = new List<SpaceShip>();
    int launchingShips;
    Vector2 target;

    [Header("SPRITE")]
    [SerializeField] SpriteRenderer starSprite;
    
	void Start ()
    {
        if(type == starType.Yellow)
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
        time += Time.deltaTime;

        if(time >= 1f)
        {
            time = 0;
            SpaceShip();
        }
    }

    public void CivShipStats(float _shipLife, float _shipCost, float _shipAttack, float _shipShield)
    {
        shipLife = _shipLife;
        shipCost = _shipCost;
        shipAttack = _shipAttack;
        shipShield = _shipShield;
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
        if(!Ships.Contains(satellite))
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

        minShipOrbitRadius = minShipOrbitRadius * _volume;
        maxShipOrbitRadius = maxShipOrbitRadius * _volume;
        maxPanelOrbitRadius = maxPanelOrbitRadius * _volume;
        maxPanelOrbitRadius = maxPanelOrbitRadius * _volume;
        GetComponent<CircleCollider2D>().radius = _volume;
        newSatellitePointChild.transform.localPosition = new Vector3(this.GetComponent<CircleCollider2D>().radius, 0, 0);

        temperature = _temperature;
        energyOutput = _energy;
    }

    public string typeOfStar()
    {
        return type.ToString();
    }

    #endregion

    public int GetCiv()
    {
        return civilization;
    }

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
            for (int i = 0; i < launchingShips / 2; i++)
            {
                Ships[0].SetTravelling(target);
                RemoveFromArmy(Ships[0]);
            }
        }
    }

    public void SetUniverse(Universe everything)
    {
        universe = everything;
    }

 /*   
    public void ArmyBattle() //FIGHT AGAINST INVASORS
    {
        if (orbitingShips.Count == 0) //LOSE
        {
            player = enemyOrbitingShips[0].player;
            starLight.color = spaceManager.GetPlayerColor(player);

            for (int i = 0; i < enemyOrbitingShips.Count; i++)
            {
                Ship enemyShip = enemyOrbitingShips[i];

                RemoveEnemyShip(enemyOrbitingShips[i]);
                AddShip(enemyShip);
            }
        }
        else
        {
            for (int i = 0; i < enemyOrbitingShips.Count; i++)
            {
                if (orbitingShips.Count == 0) break;

                enemyOrbitingShips[i].CrushAnotherShip(orbitingShips[i]);

                if (enemyOrbitingShips[i].life <= 0) //ENEMY SHIP DEAD
                {
                    enemyOrbitingShips[i].crashed = true;
                    RemoveEnemyShip(enemyOrbitingShips[i]);
                }

                if (orbitingShips.Count == 0) break;

                if (orbitingShips[i].life <= 0) //ALLY SHIP DEAD
                {
                    orbitingShips[i].crashed = true;
                    RemoveShip(orbitingShips[i]);
                }
            }
        }
    } 

    public void LaunchShips(bool sendAll, Star obj) //SEND ALL OR HALF OF ORBITING SHIPS
    {

    }
    
    public void Selected(bool selected)
    {
        if(selected)
        {
            starLight.gameObject.GetComponent<Animator>().SetTrigger("Selection");
        }
        else
        {
            starLight.gameObject.GetComponent<Animator>().SetTrigger("Deselection");
        }
    }
    */
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minShipOrbitRadius);
        Gizmos.DrawWireSphere(transform.position, maxShipOrbitRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minPanelOrbitRadius);
        Gizmos.DrawWireSphere(transform.position, maxPanelOrbitRadius);
    }

}
