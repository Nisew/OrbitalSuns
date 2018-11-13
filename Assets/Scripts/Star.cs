using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("Space Elements")]
    SpaceManager spaceManager;

    [Header("Star Elements")]
    public int orbitSaturation = 100;
    public List<GameObject> orbitingShips = new List<GameObject>();
    public Vector2[] spawnPoints = new Vector2[8];
    public float orbitDistance;

    float counter;

	void Start ()
    {

    }
	
	void Update ()
    {
		if(orbitingShips.Count <= orbitSaturation)
        {
            counter += Time.deltaTime;

            if(counter >= 0.5f)
            {
                CreateShip();
                counter = 0;
            }
        }
	}

    public void CreateShip() //CREATE A NEW SHIP THAT STARTS ORBITING
    {
        GameObject ship = spaceManager.ActiveShip();
        ship.SetActive(true);

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

        AddShip(ship);

    }

    #region LIST METHODS

    public void AddShip(GameObject ship) //ADD A SHIP TO THE PLANET ORBIT
    {
        if (!orbitingShips.Contains(ship))
        {
            orbitingShips.Add(ship);
        }
    }

    public void RemoveShip(GameObject ship) //REMOVE A SHIP FROM THE PLANET ORBIT
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
