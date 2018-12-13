using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Text starPowerText;
    public Slider productionSlider;

    #region MENU METHODS

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    #endregion

    #region UI METHODS

    public void UpdateStarPower(int power)
    {
        starPowerText.text = "Star Power: " + power;
    }

    public void Slider()
    {
        
    }

    #endregion

}
