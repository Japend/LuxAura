using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour {

    public UnityEngine.UI.Text unitiesMouseText;

    EventEntity currentSelection;
    int unitsCarried;
    List<EventEntity> entitiesSelected;

	// Use this for initialization
	void Awake () {
        entitiesSelected = new List<EventEntity>();
	}
	
	// Update is called once per frame
	void Update () {

        checkForSelection();

        unitiesMouseText.transform.localPosition = new Vector3(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2, 0);

        if (unitsCarried > 0)
            unitiesMouseText.text = System.Convert.ToString(unitsCarried);
        else
            unitiesMouseText.text = "";
	}

    private void checkForSelection()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //it there is collision
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform.parent.GetComponent<EventEntity>() != null)
                {
                    if (hit.collider.transform.parent.GetComponent<EventEntity>().CurrentPlayerOwner == GlobalData.HUMAN_PLAYER)
                    {
                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            prepareAttack(hit.collider.transform.parent.gameObject);
                        }
                        else
                        {
                            unitsCarried += hit.collider.transform.parent.GetComponent<EventEntity>().SelectUnits();
                            //we add it to the list
                            if (!entitiesSelected.Contains(hit.collider.transform.parent.GetComponent<EventEntity>()))
                                entitiesSelected.Add(hit.collider.transform.parent.GetComponent<EventEntity>());
                        }
                    }
                    else
                    {
                        prepareAttack(hit.collider.transform.parent.gameObject);
                    }
                }
            }

            //if there is none
            else
            {
                deselect();
            }
        }

        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                unitsCarried += Game.Instance.Players[GlobalData.HUMAN_PLAYER].SelectAllUnits();

                foreach (EventEntity ent in Game.Instance.Players[GlobalData.HUMAN_PLAYER].Planets)
                {
                    if (!entitiesSelected.Contains(ent))
                        entitiesSelected.Add(ent);
                }
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform.parent.GetComponent<EventEntity>() != null)
                {
                    if (hit.collider.transform.parent.GetComponent<EventEntity>().CurrentPlayerOwner == GlobalData.HUMAN_PLAYER)
                    {
                        unitsCarried += hit.collider.transform.parent.GetComponent<EventEntity>().SelectUnits(true);
                        //we add it to the list
                        if (!entitiesSelected.Contains(hit.collider.transform.parent.GetComponent<EventEntity>()))
                            entitiesSelected.Add(hit.collider.transform.parent.GetComponent<EventEntity>());
                    }
                }
            }
        }
    }

    private void prepareAttack(GameObject objective)
    {
        foreach (EventEntity ent in entitiesSelected)
        {
            ent.UseUnits(objective);
        }
        entitiesSelected.Clear();
        unitsCarried = 0;
    }

    private void deselect()
    {
        foreach (EventEntity ent in entitiesSelected)
        {
            ent.DeselectUnits();
        }

        entitiesSelected.Clear();
        unitsCarried = 0;
    }
}
