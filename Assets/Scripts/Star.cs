using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("Space Elements")]
    SpaceManager spaceManager;
    float time;

    [Header("Star Elements")]
    public Vector2[] spawnPoints = new Vector2[8];
    public int orbitSaturation;
    public float orbitDistance;
    public int player = 0;

    [Header("Ship Elements")]
    public List<Ship> orbitingShips = new List<Ship>();
    int provisionalOrbitingShips;
    public Vector2 objective;

	void Start ()
    {
        SetSpawnPoints();
    }
	
	void Update ()
    {
		if(orbitingShips.Count < orbitSaturation) //SPAWN SHIPS OVER TIME
        {
            time += Time.deltaTime;

            if(time >= 1f)
            {
                CreateShip();
                time = 0;
            }
        }
	}

    #region STAR METHODS

    public void LaunchShips(bool sendAll, Vector2 obj) //SEND ALL OR HALF OF ORBITING SHIPS
    {
        Debug.Log("AH");
        objective = obj;
        provisionalOrbitingShips = orbitingShips.Count;

        if (sendAll)
        {
            for (int i = 0; i < provisionalOrbitingShips; i++)
            {
                orbitingShips[i].Launch(objective);
                RemoveShip(orbitingShips[i]);
            }
        }
        else
        {
            for (int i = 0; i < provisionalOrbitingShips/2; i++)
            {
                orbitingShips[i].Launch(objective);
                RemoveShip(orbitingShips[i]);
            }
        }
    }

    public void CreateShip() //CREATE A NEW SHIP THAT STARTS ORBITING
    {
        GameObject ship = spaceManager.ActiveShip();
        ship.SetActive(true);
        ship.GetComponentInChildren<SpriteRenderer>().color = spaceManager.TintShips(player);

        int i = Random.Range(0, 7);

        ship.transform.position = spawnPoints[i];
        if (i == 0) ship.transform.localRotation = Quaternion.Euler(0, 0, -90);
        if (i == 1) ship.transform.localRotation = Quaternion.Euler(0, 0, -135);
        if (i == 2) ship.transform.localRotation = Quaternion.Euler(0, 0, -180);
        if (i == 3) ship.transform.localRotation = Quaternion.Euler(0, 0, -225);
        if (i == 4) ship.transform.localRotation = Quaternion.Euler(0, 0, -270);
        if (i == 5) ship.transform.localRotation = Quaternion.Euler(0, 0, -315);
        if (i == 6) ship.transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (i == 7) ship.transform.localRotation = Quaternion.Euler(0, 0, 45);

        ship.GetComponent<Ship>().orbitalParent = this;
        ship.GetComponent<Ship>().Creation();
        AddShip(ship.GetComponent<Ship>());
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

    public void RemoveShip(Ship ship) //REMOVE A SHIP FROM THE PLANET ORBIT
    {
        if (orbitingShips.Contains(ship))
        {
            orbitingShips.Remove(ship);
        }
    }

    #endregion

    #region SPACEMANAGER METHODS

    public void MeetUniverse(SpaceManager universe)
    {
        spaceManager = universe;
    }

    void SetSpawnPoints()
    {
        spawnPoints[0] = new Vector2(transform.position.x, transform.position.y + 0.3f);
        spawnPoints[1] = new Vector2(transform.position.x + 0.2f, transform.position.y + 0.2f);
        spawnPoints[2] = new Vector2(transform.position.x + 0.3f, transform.position.y);
        spawnPoints[3] = new Vector2(transform.position.x + 0.2f, transform.position.y - 0.2f);
        spawnPoints[4] = new Vector2(transform.position.x, transform.position.y - 0.3f);
        spawnPoints[5] = new Vector2(transform.position.x - 0.2f, transform.position.y - 0.2f);
        spawnPoints[6] = new Vector2(transform.position.x - 0.3f, transform.position.y);
        spawnPoints[7] = new Vector2(transform.position.x - 0.2f, transform.position.y + 0.2f);
    }

    #endregion

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, orbitDistance);

        Gizmos.DrawIcon(spawnPoints[0], "");
        Gizmos.DrawIcon(spawnPoints[1], "");
        Gizmos.DrawIcon(spawnPoints[2], "");
        Gizmos.DrawIcon(spawnPoints[3], "");
        Gizmos.DrawIcon(spawnPoints[4], "");
        Gizmos.DrawIcon(spawnPoints[5], "");
        Gizmos.DrawIcon(spawnPoints[6], "");
        Gizmos.DrawIcon(spawnPoints[7], "");
    }
}
