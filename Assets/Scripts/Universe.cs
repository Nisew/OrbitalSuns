using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    [Header("LISTS")]
    List<Star> starList = new List<Star>();
    List<SpaceShip> unactiveShipList = new List<SpaceShip>();
    List<SpaceShip> activeShipList = new List<SpaceShip>();

    [Header("PREFABS")]
    [SerializeField] GameObject spaceShip;
    [SerializeField] GameObject asteroid;

    [Header("STAR NAVIGATION")]
    float time;
    bool firstClic;
    Star selectedStar;
    Star destinyStar;

    [SerializeField] Color playerColor0;
    [SerializeField] Color playerColor1;
    [SerializeField] Color playerColor2;

    void Awake()
    {
        BigBang();
    }

	void Start()
    {

	}
	
	void Update()
    {
        if (firstClic) CheckDoubleClic();
	}
    
    public void BigBang()
    {
        GameObject[] gasClouds;

        gasClouds = GameObject.FindGameObjectsWithTag("Star");

        foreach(GameObject star in gasClouds)
        {
            Star starScript = star.GetComponent<Star>();
            starScript.SetUniverse(this);
            starList.Add(starScript);
        }
    }

    public Color GetPlayerColor(int player)
    {
        Color provColor;

        switch (player)
        {
            case 0:
                provColor = playerColor0;
                break;

            case 1:
                provColor = playerColor1;
                break;

            case 2:
                provColor = playerColor2;
                break;
            default:
                provColor = playerColor0;
                break;
        }

        return provColor;
    }

    public SpaceShip CreateSpaceShip()
    {
        SpaceShip newSpaceShip;

        if(unactiveShipList.Count == 0)
        {
            newSpaceShip = Instantiate(spaceShip).GetComponent<SpaceShip>();
        }
        else
        {
            newSpaceShip = unactiveShipList[0];
            RemoveUnactiveShip(newSpaceShip);
        }

        AddActiveShip(newSpaceShip);
        return newSpaceShip;
    }

    public Asteroid CreateAsteroid()
    {
        Asteroid provisionalAsteroid = Instantiate(asteroid.GetComponent<Asteroid>());
        return provisionalAsteroid;
    }

    public void StarTouched(Star touchedStar)
    {
        if(selectedStar == null)
        {
            selectedStar = touchedStar;
            selectedStar.Selected(true);
        }
        else if(touchedStar != selectedStar && touchedStar != destinyStar)
        {
            destinyStar = touchedStar;
            selectedStar.SendHalfShips(destinyStar);
            firstClic = true;
        }
        else
        {
            selectedStar.SendAllShips(destinyStar);
            time = 0;
            firstClic = false;
            selectedStar.Selected(false);
            selectedStar = null;
            destinyStar = null;
        }
    }

    public void VoidTouched()
    {
        if(selectedStar != null)
        {
            selectedStar.Selected(false);
            selectedStar = null;
        }
    }
    
    void CheckDoubleClic()
    {
        if (time >= 0.5f)
        {
            firstClic = false;
            destinyStar = null;
            time = 0;
        }
        else time += Time.deltaTime;
    }

    #region SHIPS LIST MANAGEMENT

    public void AddUnactiveShip(SpaceShip ship)
    {
        if (!unactiveShipList.Contains(ship))
        {
            unactiveShipList.Add(ship);
        }
        else
        {
            Debug.Log("Ship already in unactive list");            
        }
    }

    void RemoveUnactiveShip(SpaceShip ship)
    {
        if(unactiveShipList.Contains(ship))
        {
            unactiveShipList.Remove(ship);
        }
        else
        {
            Debug.Log("Ship is not in unactive list");
        }
    }

    void AddActiveShip(SpaceShip ship)
    {
        if (!activeShipList.Contains(ship))
        {
            activeShipList.Add(ship);
        }
        else
        {
            Debug.Log("Ship already in the active list");
        }
    }

    public void RemoveActiveShip(SpaceShip ship)
    {
        if (activeShipList.Contains(ship))
        {
            activeShipList.Remove(ship);
        }
        else
        {
            Debug.Log("Ship is not in active list");
        }
    }

    #endregion

}
