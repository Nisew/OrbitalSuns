using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    Universe universe;
    UIMenu UImenu;

    [Header("CLICK CONTROL")]
    [SerializeField] Star selectedStar;
    [SerializeField]
    Star targetStar;
    float time;
    bool firstClick;

	void Start ()
    {
        universe = GameObject.FindGameObjectWithTag("Universe").GetComponent<Universe>();
        UImenu = GameObject.FindGameObjectWithTag("UI").GetComponent<UIMenu>();
    }
	
	void Update ()
    {
        CheckClick();
    }

    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0) && !UImenu.CheckMenuOpen())
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, new Vector2(0, 0), 0.01f);

            if (hits.Length == 0)
            {
                selectedStar = null;
                targetStar = null;
                UImenu.UpdateHostStar(selectedStar);
            }
            else if (hits.Length >= 1)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if(i == 0)
                    {
                        for (int o = 0; o < hits.Length; o++)
                        {
                            if (hits[o].collider.tag == "UIButton")
                            {
                                UImenu.ButtonLogic();
                                return;
                            }
                        }
                    }

                    if (hits[i].collider.tag == "Star")
                    {
                        if (hits[i].collider.GetComponent<Star>().gameObject != selectedStar && selectedStar != null)
                        {
                            if (firstClick && hits[i].collider.gameObject.GetComponent<Star>() == targetStar)
                            {
                                time = 1;
                                firstClick = false;
                                universe.SendShips(selectedStar.gameObject, targetStar.gameObject, true);
                                selectedStar = null;
                                UImenu.UpdateHostStar(selectedStar);
                                targetStar = null;
                                break;
                            }
                            if (!firstClick)
                            {
                                time = 1;
                                firstClick = true;
                                targetStar = hits[i].collider.GetComponent<Star>();
                                universe.SendShips(selectedStar.gameObject, targetStar.gameObject, false);
                                break;

                            }
                            else if (hits[i].collider.gameObject != targetStar && firstClick)
                            {
                                time = 1;
                                targetStar = hits[i].collider.GetComponent<Star>();
                                universe.SendShips(selectedStar.gameObject, targetStar.gameObject, false);
                                break;

                            }
                        }
                        else
                        {
                            selectedStar = hits[i].collider.GetComponent<Star>();
                            UImenu.UpdateHostStar(selectedStar);
                        }
                    }

                }
            }

        }

        if (firstClick)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                time = 1;
                firstClick = false;
                targetStar = null;
            }
        }
    }
}
