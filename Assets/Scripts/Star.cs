using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("Space Elements")]
    SpaceManager spaceManager;
    float time;

    [Header("Star Elements")]
    public int orbitSaturation;
    public float orbitDistance;
    public float bornDistance;
    public int player = 0;
    public float energyProduction;
    public Transform birthPoint;

    [Header("Ship Elements")]
    public List<Ship> orbitingShips = new List<Ship>();
    public List<Ship> enemyOrbitingShips = new List<Ship>();
    int provisionalOrbitingShips;
    public Vector2 objective;

	void Start ()
    {
        GetBirthPoint();
    }
	
	void Update ()
    {
		if(orbitingShips.Count < orbitSaturation) //SPAWN SHIPS OVER TIME
        {
            time += Time.deltaTime;

            if(time >= energyProduction)
            {
                CreateShip();
                time = 0;
            }
        }

        if(enemyOrbitingShips.Count > 0) //FIGHT!
        {
            War();
        }
	}

    #region STAR METHODS

    public void LaunchShips(bool sendAll, Star obj) //SEND ALL OR HALF OF ORBITING SHIPS
    {
        provisionalOrbitingShips = orbitingShips.Count;

        if (sendAll)
        {
            for (int i = 0; i < provisionalOrbitingShips; i++)
            {
                orbitingShips[i].Launch(obj);
            }
        }
        else
        {
            for (int i = 0; i < provisionalOrbitingShips/2; i++)
            {
                orbitingShips[i].Launch(obj);
            }
        }
    }

    public void CreateShip() //TAKE A SHIP FROM SPACE MANAGER AND PLACE IT IN GAME
    {
        GameObject ship = spaceManager.ActiveShip();
        ship.SetActive(true);
        ship.GetComponent<Ship>().player = player;
        ship.GetComponentInChildren<SpriteRenderer>().color = spaceManager.TintShips(player);
        birthPoint.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        ship.gameObject.transform.position = birthPoint.GetChild(0).position;        
        ship.GetComponent<Ship>().orbitalParent = this;
        ship.GetComponent<Ship>().ResetRandom();
        ship.GetComponent<Ship>().Creation();
        AddShip(ship.GetComponent<Ship>());
    }

    public void GetBirthPoint() //SET THE POSITION OF THE BIRTH OBJECT
    {
        if (birthPoint == null)
        {
            foreach (Transform child in this.gameObject.transform)
            {
                if (child.tag == "BirthPoint")
                {
                    birthPoint = child.GetChild(0);
                    birthPoint.transform.position = new Vector2(bornDistance, 0);
                    birthPoint = child.gameObject.transform;
                }
            }
        }
    }

    public void War() //FIGHT AGAINST INVASORS
    {
        if (orbitingShips.Count == 0) //LOSE
        {
            player = enemyOrbitingShips[0].player;

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

    #endregion

    #region LIST METHODS

    public void AddShip(Ship ship) //ADD A SHIP TO THE PLANET ORBIT
    {
        if (!orbitingShips.Contains(ship))
        {
            orbitingShips.Add(ship);
        }
    }

    public void AddEnemyShip(Ship ship) //ADD AN ENEMY SHIP TO THE PLANET ORBIT
    {
        if (!enemyOrbitingShips.Contains(ship))
        {
            enemyOrbitingShips.Add(ship);
        }
    }

    public void RemoveShip(Ship ship) //REMOVE A SHIP FROM THE PLANET ORBIT
    {
        if (orbitingShips.Contains(ship))
        {
            orbitingShips.Remove(ship);
        }
    }

    public void RemoveEnemyShip(Ship ship) //REMOVE AN ENEMY SHIP FROM THE PLANET ORBIT
    {
        if (enemyOrbitingShips.Contains(ship))
        {
            enemyOrbitingShips.Remove(ship);
        }
    }

    #endregion

    #region SPACEMANAGER METHODS

    public void MeetUniverse(SpaceManager universe)
    {
        spaceManager = universe;
    }

    #endregion

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, orbitDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, bornDistance);
    }

}
