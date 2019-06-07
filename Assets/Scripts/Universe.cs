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
    
    float yellowMinTemp = 2000;
    float yellowMaxTemp = 2500;
    float yellowMinVolume = 1.3f;
    float yellowMaxVolume = 2.7f;
    float yellowMinEnergyOutput = 100;
    float yellowMaxEnergyOutput = 150;

    float blueMinTemp = 0;
    float blueMaxTemp = 999;
    float blueMinVolume = 0.9f;
    float blueMaxVolume = 1.3f;
    float blueMinEnergyOutput = 700;
    float blueMaxEnergyOutput = 950;

    float redMinTemp = 999;
    float redMaxTemp = 9999;
    float redMinVolume = 4;
    float redMaxVolume = 6;
    float redMinEnergyOutput = 200;
    float redMaxEnergyOutput = 350;

    [Header("SATELLITES")]
    List<GameObject> spaceshipActiveList = new List<GameObject>();
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
            ListAddSpaceShip(provisionalShip);
        }
        else
        {
            provisionalShip = spaceShipUnactiveList[0];
            ListAddSpaceShip(provisionalShip);
        }
        provisionalShip.GetComponent<SpaceShip>().Tint(civilizationColor);
        return provisionalShip;
    }

    public void ListAddSpaceShip(GameObject ship)
    {
        if(!spaceshipActiveList.Contains(ship))
        {
            spaceshipActiveList.Add(ship);
        }
        //ACTIVE/RESTART THE SHIP COMPONENTS
    }

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
/*
    #region SHIPS METHODS

    public GameObject ActiveShip() //PASS AN INACTIVE SHIP OR CREATES A NEW ONE IF ALL ARE ACTIVE
    {
        GameObject ship = totalShips[0];

        for (int i = 0; i < totalShips.Count; i++)
        {
            if (!totalShips[i].activeInHierarchy)
            {
                ship = totalShips[i];
                break;
            }
            else if(i >= totalShips.Count - 1 && totalShips[i].activeInHierarchy)
            {
                ship = Instantiate(shipPrefab);
                totalShips.Add(ship);
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

    #region SELECTION METHODS

    void CheckSelection() //SELECTION OF STARS
    {
        if (selectedStar == null) //SELECT A STAR IF NONE IS SELECTED
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.tag == "Star" && hit.collider.GetComponent<Star>().player == 1)
                    {
                        selectedStar = hit.collider.gameObject;
                        selectedStar.GetComponent<Star>().Selected(true);
                    }
                }
            }
        }

        if (selectedStar != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider == null) //DESELECTS A STAR IF ONE IS SELECTED
                {
                    selectedStar.GetComponent<Star>().Selected(false);
                    selectedStar = null;
                }
                else if (hit.collider.tag == "Star")
                {
                    if (hit.collider.gameObject != selectedStar)
                    {
                        if (!firstClick)
                        {
                            selectedTargetStar = hit.collider.gameObject;
                            firstClick = true;
                            selectedStar.GetComponent<Star>().LaunchShips(false, selectedTargetStar.GetComponent<Star>()); //TRANSFER HALF SHIPS
                        }
                        else if (hit.collider.gameObject == selectedTargetStar && firstClick)
                        {
                            firstClick = false;
                            clickTime = 0;
                            selectedStar.GetComponent<Star>().LaunchShips(true, hit.collider.GetComponent<Star>()); //TRANSFER ALL SHIPS
                            selectedStar.GetComponent<Star>().Selected(false);
                            selectedStar = null;
                            selectedTargetStar = null;
                        }
                        else if (hit.collider.gameObject != selectedTargetStar && firstClick)
                        {
                            selectedTargetStar = hit.collider.gameObject;
                            clickTime = 0;
                            selectedStar.GetComponent<Star>().LaunchShips(false, selectedTargetStar.GetComponent<Star>()); //TRANSFER HALF SHIPS
                        }
                    }
                }
            }

            if (firstClick)
            {
                clickTime += Time.deltaTime;

                if (clickTime >= doubleClickTime)
                {
                    firstClick = false;
                    clickTime = 0;
                }
            }
        }
    }

    public Color GetPlayerColor(int player)
    {
        Color playerColor;

        if (player == 1) playerColor = player1;
        else playerColor = player2;

        return playerColor;
    }

    #endregion

    #region UI METHODS

    public void UpdateStarPower(float power)
    {
        starPower += Mathf.RoundToInt(power);
        UIScript.UpdateStarPower(starPower);
    }

    #endregion
    */

}
