using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour
{
    [Header("Space Elements")]
    Universe universe;

    [Header("Canvas Elements")]
    public GameObject panelObject;
    public Slider energyWheelSlider;
    public Text energyText;
    public Text shipText;
    Image starMiniature;

    [Header("Star Elements")]
    Star hostStar;
    float energyWheel;

    [Header("UI Elements")]
    public Image buttonImage;
    public Color buttonSelected;
    bool isGlowing;
    bool menuOpen;

    void Start()
    {
        universe = GameObject.FindGameObjectWithTag("Universe").GetComponent<Universe>();
    }

    void Update()
    {
        if(hostStar != null && !isGlowing)
        {
            buttonImage.color = buttonSelected;
            isGlowing = true;
        }
        if(hostStar == null && isGlowing)
        {
            buttonImage.color = new Color(0, 0, 0, 0);
            isGlowing = false;
        }
    }

    #region SCENE METHODS

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    #endregion

    #region GAMEPLAY UI
    
    public void MoveWheel()
    {
        energyWheel = energyWheelSlider.value;
        hostStar.EnergyWheelChange(energyWheel);

        energyText.text = energyWheel + "%";
        shipText.text = (100 - energyWheel) + "%";
    }

    public void UpdateHostStar(Star star)
    {
        hostStar = star;
        //starMiniature = hostStar.GetMiniature();
    }

    public void ButtonLogic()
    {
        if(!menuOpen)
        {
            panelObject.SetActive(true);
            menuOpen = true;
        }
        else
        {
            panelObject.SetActive(false);
            menuOpen = false;
        }
    }

    public void CloseStarMenu()
    {
        panelObject.SetActive(false);
        menuOpen = false;
    }

    #endregion

    public bool CheckMenuOpen()
    {
        return menuOpen;
    }
}
