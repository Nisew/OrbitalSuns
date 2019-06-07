using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [Header("UNIVERSE LAWS")]
    Universe universe;

    [Header("CLICK CONTROL")]
    [SerializeField] GameObject selectedStar;
    [SerializeField] GameObject targetStar;
    float time;
    bool firstClick;

	void Start ()
    {
        universe = GameObject.FindGameObjectWithTag("Universe").GetComponent<Universe>();
    }
	
	void Update ()
    {
        CheckSelection();
    }

    void CheckSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, new Vector2(0, 0), 0.01f);

            if (hits.Length == 0)
            {
                Debug.Log("Deselected");
                selectedStar = null;
                targetStar = null;
            }
            else if(hits.Length == 1)
            {
                if(hits[0].collider.tag == "Star")
                {

                    if(hits[0].collider.gameObject != selectedStar && selectedStar != null)
                    {
                        if (!firstClick)
                        {
                            firstClick = true;
                            targetStar = hits[0].collider.gameObject;
                            universe.SendShips(selectedStar, targetStar, false);
                        }
                        else if (hits[0].collider.gameObject == targetStar && firstClick)
                        {
                            firstClick = false;
                            time = 1;
                            targetStar = hits[0].collider.gameObject;
                            universe.SendShips(selectedStar, targetStar, true);
                            selectedStar = null;
                            targetStar = null;
                        }
                        else if (hits[0].collider.gameObject != targetStar && firstClick)
                        {
                            targetStar = hits[0].collider.gameObject;
                            time = 1;
                            universe.SendShips(selectedStar, targetStar, false);
                        }
                        else
                        {
                            targetStar = hits[0].collider.gameObject;
                            Debug.Log("target: ", targetStar);
                        }

                    }
                    else
                    {
                        selectedStar = hits[0].collider.gameObject;
                        Debug.Log("Selected: ", selectedStar);
                    }
                }
                else if (hits[0].collider.tag == "UIButton")
                {
                    Debug.Log("UIButton");
                }
            }
            else if(hits.Length > 1)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if(hits[i].collider.tag == "UIButton")
                    {
                        Debug.Log("UIButton");
                    }
                }
            }
        }

        if(firstClick)
        {
            time -= Time.deltaTime;

            if(time <= 0)
            {
                time = 1;
                firstClick = false;
                targetStar = null;
            }
        }
    }

        /*if (selectedStar != null)
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
        }*/
}
