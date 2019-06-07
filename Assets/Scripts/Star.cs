using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    Universe universe;
    float time;

    [Header("STAR")]
    [SerializeField] Transform newSatellitePoint;
    [SerializeField] Transform newSatellitePointChild;
    public float minOrbitRadius;
    public float maxOrbitRadius;
    int maxShips;
    [SerializeField] int civilization;

    [Header("CIVILIZATION STATS")]
    float shipCost;
    float shipAttack;
    float shipLife;
    float shipShield;

    [Header("STAR INFO")]
    float temperature;
    enum starType { Yellow, Blue, Red };
    [SerializeField] starType type;

    [Header("ENERGY FLOW")]
    float energyOutput;
    float armyEnergy;
    float investEnergy;

    [Header("SATELLITE ARMY")]
    [SerializeField] List<SpaceShip> satellites = new List<SpaceShip>();
    int launchingSatellites;
    List<SpaceShip> enemySatellites = new List<SpaceShip>();
    Vector2 target;

    [Header("SATELLITE UPGRADES")]
    float spaceShipLife = 50;

    [Header("SPRITE")]
    [SerializeField] SpriteRenderer starSprite;
    [SerializeField] SpriteRenderer lightSprite;
    Transform lightSpriteTransform;
    float lightSpeed;
    
	void Start ()
    {
        
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
        shipScript.SetOrbitRadius();
        shipScript.Create();

        AddInArmy(shipScript);
    }

    public int GetCiv()
    {
        return civilization;
    }

    public float GetMinOrbitRadius()
    {
        return minOrbitRadius;
    }

    public float GetMaxOrbitRadius()
    {
        return maxOrbitRadius;
    }

    public Vector3 GetBirthPoint()
    {
        return newSatellitePointChild.transform.localPosition;
    }

    #region LIST METHODS

    public void AddInArmy(SpaceShip satellite)
    {
        if(!satellites.Contains(satellite))
        {
            satellites.Add(satellite);
        }
        else Debug.Log("Ship already in the star army list");
    }

    public void RemoveFromArmy(SpaceShip satellite)
    {
        if (satellites.Contains(satellite))
        {
            satellites.Remove(satellite);
        }
        else Debug.Log("Ship already out of star army list");
    }

    public void AddInEnemyArmy(SpaceShip enemySatellite)
    {
        if (!enemySatellites.Contains(enemySatellite))
        {
            satellites.Add(enemySatellite);
        }
        else Debug.Log("Ship already in the star enemies orbiting list");
    }

    public void RemoveFromEnemyArmy(SpaceShip enemySatellite)
    {
        if (enemySatellites.Contains(enemySatellite))
        {
            enemySatellites.Remove(enemySatellite);
        }
        else Debug.Log("Ship already out of star enemies orbiting list");
    }

    #endregion

    public void SendArmy(GameObject target, bool all)
    {
        if (all)
        {
            launchingSatellites = satellites.Count;
            for (int i = 0; i < launchingSatellites; i++)
            {
                satellites[0].SetTravelling(target);
                RemoveFromArmy(satellites[0]);
            }
        }
        else
        {
            launchingSatellites = satellites.Count;
            for (int i = 0; i < launchingSatellites / 2; i++)
            {
                satellites[0].SetTravelling(target);
                RemoveFromArmy(satellites[0]);
            }
        }
    }

    public void SetUniverse(Universe everything)
    {
        universe = everything;
    }

    public string typeOfStar()
    {
        return type.ToString();
    }

    public void SetProperties(float _temperature, float _volume, float _energy)
    {
        starSprite.transform.localScale = new Vector3(_volume, _volume, 1);
        minOrbitRadius = minOrbitRadius * _volume;
        maxOrbitRadius = maxOrbitRadius * _volume;
        temperature = _temperature;
        energyOutput = _energy;
        GetComponent<CircleCollider2D>().radius = _volume;
        newSatellitePointChild.transform.localPosition = new Vector3(-this.GetComponent<CircleCollider2D>().radius, 0, 0);
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
        Gizmos.DrawWireSphere(transform.position, minOrbitRadius);
        Gizmos.DrawWireSphere(transform.position, maxOrbitRadius);
        Gizmos.color = Color.blue;
    }

}
