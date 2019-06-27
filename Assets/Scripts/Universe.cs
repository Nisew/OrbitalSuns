using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    float time;
    float doubleClickTime = 0.5f;
    float clickTime;
    bool firstClick;

    [Header("STAR CLUSTERS")]
    List<Star> starCluster = new List<Star>();
    
    float yellowMinTemp = 5000;
    float yellowMaxTemp = 7700;
    float yellowMinVolume = 1.5f;
    float yellowMaxVolume = 2;
    float yellowMinEnergyOutput = 235;
    float yellowMaxEnergyOutput = 265;

    float blueMinTemp = 27000;
    float blueMaxTemp = 34000;
    float blueMinVolume = 0.5f;
    float blueMaxVolume = 1;
    float blueMinEnergyOutput = 255;
    float blueMaxEnergyOutput = 275;

    float redMinTemp = 4000;
    float redMaxTemp = 6500;
    float redMinVolume = 4;
    float redMaxVolume = 6;
    float redMinEnergyOutput = 220;
    float redMaxEnergyOutput = 240;

    [Header("SATELLITES")]
    List<GameObject> spaceShipActiveList = new List<GameObject>();
    List<GameObject> spaceShipUnactiveList = new List<GameObject>();
    List<GameObject> solarPanelActiveList = new List<GameObject>();
    List<GameObject> solarPanelUnactiveList = new List<GameObject>();
    List<GameObject> cometList = new List<GameObject>();
    [SerializeField] GameObject solarPanel;
    [SerializeField] GameObject spaceShip;
    [SerializeField] GameObject comet;

    [Header("CIVILIZATIONS")]
    [SerializeField] Color civilization1;
    [SerializeField] Color civilization2;
    GameObject selectedStar;
    GameObject destinyStar;

    [Header("UI")]
    UIMenu UIScript;

    void Awake()
    {
        BigBang();
    }

	void Start()
    {

	}
	
	void Update()
    {

	}

    #region CREATION OF MATTER

    public void BigBang()
    {
        GameObject[] gasClouds;

        gasClouds = GameObject.FindGameObjectsWithTag("Star");

        foreach(GameObject star in gasClouds)
        {
            if(star.GetComponent<Star>())
            {
                Star starScript = star.GetComponent<Star>();

                string starType = star.GetComponent<Star>().typeOfStar();
                float temp = 0;
                float volume = 0;
                float energy = 0;

                switch (starType)
                {
                    case ("Yellow"):
                        volume = Random.Range(yellowMinVolume, yellowMaxVolume);
                        temp = Random.Range(yellowMinTemp, yellowMaxTemp);
                        energy = Random.Range(yellowMinEnergyOutput, yellowMaxEnergyOutput);
                    break;

                    case ("Blue"):
                        volume = Random.Range(blueMinVolume, blueMaxVolume);
                        temp = Random.Range(blueMinTemp, blueMaxTemp);
                        energy = Random.Range(blueMinEnergyOutput, blueMaxEnergyOutput);
                        break;

                    case ("Red"):
                        volume = Random.Range(redMinVolume, redMaxVolume);
                        temp = Random.Range(redMinTemp, redMaxTemp);
                        energy = Random.Range(redMinEnergyOutput, redMaxEnergyOutput);
                        break;
                }

                starScript.SetProperties(temp, volume, energy);
                starScript.SetUniverse(this);
                starCluster.Add(starScript);
            }
        }        
    }

    public GameObject CreateSpaceShip(int civ)
    {
        GameObject provisionalShip;
        Color civilizationColor = new Color();

        if(civ == 1)
        {
            civilizationColor = civilization1;
        }
        if (civ == 2)
        {
            civilizationColor = civilization2;
        }

        if (spaceShipUnactiveList.Count <= 0)
        {
            provisionalShip = Instantiate(spaceShip);
            AddActiveSpaceShip(provisionalShip);
        }
        else
        {
            provisionalShip = spaceShipUnactiveList[0];
            RemoveUnactiveSpaceShip(provisionalShip);
            AddActiveSpaceShip(provisionalShip);
        }
        provisionalShip.GetComponent<SpaceShip>().Tint(civilizationColor);
        provisionalShip.SetActive(true);
        return provisionalShip;
    }

    #region LIST METHODS

    public void AddActiveSpaceShip(GameObject ship)
    {
        if(!spaceShipActiveList.Contains(ship) && !spaceShipUnactiveList.Contains(ship))
        {
            spaceShipActiveList.Add(ship);
        }
    }

    public void RemoveActiveSpaceShip(GameObject ship)
    {
        if (spaceShipActiveList.Contains(ship))
        {
            spaceShipActiveList.Remove(ship);
        }
    }

    public void AddUnactiveSpaceShip(GameObject ship)
    {
        if (!spaceShipUnactiveList.Contains(ship) && !spaceShipActiveList.Contains(ship))
        {
            spaceShipUnactiveList.Add(ship);
        }
    }

    public void RemoveUnactiveSpaceShip(GameObject ship)
    {
        if (spaceShipUnactiveList.Contains(ship))
        {
            spaceShipUnactiveList.Remove(ship);
        }
    }

    #endregion

    public void SendShips(GameObject startStar, GameObject endStar, bool all)
    {
        startStar.GetComponent<Star>().SendArmy(endStar, all);
    }

    /*void CreateFewShips(int ships) //CREATES INACTIVE SHIPS
    {
        for(int i = 0; i < ships; i++)
        {
            GameObject ship = Instantiate(shipPrefab);
            totalShips.Add(ship);
            ship.SetActive(false);
        }
    }*/

    #endregion

}
