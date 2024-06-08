using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainClickManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                //Debug.Log(hit.transform.gameObject);

                if (hit.collider.tag.Equals("Card"))
                {
                    Debug.Log("Hit card...");

                    UnitManager.instance.setUpUnitInfo(hit.transform.gameObject.GetComponent<Unit>().unit);
                    MainManager.instance.showScreen("Unit Info");
                }
            }
        }
    }
}
