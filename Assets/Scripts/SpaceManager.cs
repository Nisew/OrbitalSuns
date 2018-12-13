﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManager : MonoBehaviour
{
    [Header("UI Elements")]
    public UIMenu UIScript;

    [Header("Space Elements")]
    float doubleClickTime = 0.5f;
    bool firstClick;
    float clickTime;

    [Header("Star Elements")]
    public List<Star> starList;
    public int starPower;

    [Header("Ship Elements")]
    public List<GameObject> totalShips;
    public GameObject shipPrefab;

    [Header("Player Elements")]
    public Color player1;
    public Color player2;
    public GameObject selectedStar;
    public GameObject selectedTargetStar;

    void Awake()
    {
        starList = TakeAllStars();
    }

	void Start()
    {
        CreateFewShips(5);
	}
	
	void Update()
    {
        CheckSelection();
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
        }

        return starScript;
    }

    void CreateFewShips(int ships) //CREATES INACTIVE SHIPS
    {
        for(int i = 0; i < ships; i++)
        {
            GameObject ship = Instantiate(shipPrefab);
            totalShips.Add(ship);
            ship.SetActive(false);
        }
    }

    #endregion

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

}
