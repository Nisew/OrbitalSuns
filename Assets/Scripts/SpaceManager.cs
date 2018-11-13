using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManager : MonoBehaviour
{
    public List<Star> starList;
    public List<GameObject> totalShips;
    public int totalNumberShips = 0;
    public GameObject shipPrefab;

    void Awake()
    {
        starList = TakeAllStars();
        CreateAllShips();
    }

	void Start()
    {
		
	}
	
	void Update()
    {
		
	}

    #region BIG BANG METHODS

    List<Star> TakeAllStars()
    {
        GameObject[] stars;
        List<Star> starScript = new List<Star>();

        stars = GameObject.FindGameObjectsWithTag("Star");

        foreach(GameObject star in stars)
        {
            starScript.Add(star.GetComponent<Star>());
            star.GetComponent<Star>().MeetUniverse(GetComponent<SpaceManager>());
            totalNumberShips += star.GetComponent<Star>().orbitSaturation*2;
        }

        return starScript;
    }

    void CreateAllShips()
    {
        for(int i = 0; i < totalNumberShips*2; i++)
        {
            GameObject ship;

            ship = Instantiate(shipPrefab);

            totalShips.Add(ship);
            ship.SetActive(false);

        }
    }

    #endregion

    #region SHIPS METHODS

    public GameObject ActiveShip()
    {
        GameObject ship = totalShips[0];

        for (int i = 0; i < totalShips.Count; i++)
        {
            if (!totalShips[i].activeInHierarchy)
            {
                ship = totalShips[i];
                break;
            }
        }

        return ship;
    }

    #endregion
}
