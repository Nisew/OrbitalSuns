using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    #region MENU METHODS

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    #endregion
}
