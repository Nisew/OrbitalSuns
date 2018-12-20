using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour
{
    [Header("Space Elements")]
    public SpaceManager spaceManager;

    [Header("UI Elements")]
    public Text playerPowerText;
    public Text totalPowerText;
    public Text shipPowerText;
    public Text starPowerText;
    public Slider productionSlider;

    [Header("Star Values")]
    public GameObject Star;
    public float totalPower;
    public float shipPower;
    public float starPower;

    #region MENU METHODS

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    #endregion

    #region UI METHODS

    public void UpdateStarPower(int power)
    {
        playerPowerText.text = "" + power;
    }

    public void Slider()
    {
        
    }

    public void OpenStarMenu(GameObject panel)
    {
        if(!panel.activeInHierarchy)
        {
            panel.SetActive(true);
        }
    }

    public void GetStar()
    {
        Star = spaceManager.selectedStar;

        float[] starValues = new float[3];
        starValues = Star.GetComponent<Star>().PassInfoToUI();
        totalPower = starValues[0];
        shipPower = starValues[1];
        starPower = starValues[2];

        totalPowerText.text = "" + totalPower;
        starPowerText.text = starPower + "%";
        shipPowerText.text = shipPower + "%";
    }

    #endregion

}
