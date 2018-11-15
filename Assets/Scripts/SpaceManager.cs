using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManager : MonoBehaviour
{
    [Header("Star Elements")]
    public List<Star> starList;

    [Header("Ship Elements")]
    public List<GameObject> totalShips;
    public GameObject shipPrefab;
    public int totalNumberShips = 0;

    [Header("Player Elements")]
    public Color player1;
    public Color player2;
    public GameObject selectedStar;

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
		if(selectedStar == null) //SELECT A STAR IF NONE IS SELECTED
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if(hit.collider != null)
                {
                    if(hit.collider.tag == "Star")
                    {
                        selectedStar = hit.collider.gameObject;
                    }
                }
            }
        }

        if(selectedStar != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider == null) //DESELECTS A STAR IF ONE IS SELECTED
                {
                    selectedStar = null;
                }
                else if(hit.collider.tag == "Star")
                {
                    if(hit.collider.gameObject != selectedStar) //TRANSFER SHIPS TO CLICKED STAR
                    {
                        selectedStar.GetComponent<Star>().LaunchShips(false, hit.collider.transform.position);
                    }
                }
            }
        }
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
            totalNumberShips += star.GetComponent<Star>().orbitSaturation;
        }

        return starScript;
    }

    void CreateAllShips()
    {
        for(int i = 0; i < totalNumberShips*1.5; i++)
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

    public Color TintShips(int player)
    {
        Color shipColor = new Color(0, 0, 0);

        if (player == 1) shipColor = player1;
        if (player == 2) shipColor = player2;

        return shipColor;
    }

    #endregion
}
