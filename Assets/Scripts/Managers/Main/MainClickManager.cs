using UnityEngine.UI;
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
                // Grab the positions of the corners of the viewport (array order: bottom left, top left, top right, bottom right)
                RectTransform viewportTransform = hit.transform.parent.parent.parent.parent.GetComponent<ScrollRect>().viewport;
                Vector3[] viewportCorners = new Vector3[4];
                viewportTransform.GetWorldCorners(viewportCorners);

                // If raycast was sent in between the y position of the bottom left and top right viewport corners, then register the click
                if (hit.point.y < viewportCorners[1].y && hit.point.y > viewportCorners[0].y)
                {
                    if (hit.collider.tag.Equals("Card"))
                    {
                        Debug.Log("Hit card...");

                        if (UnitManager.instance.getViewMode())
                        {
                            UnitManager.instance.setUpUnitInfo(hit.transform.gameObject.GetComponent<Unit>().unit);
                            MainManager.instance.showScreen("Unit Info");
                        }
                        else
                        {
                            UnitManager.instance.toggleSelect(hit.transform.gameObject);
                        }
                    }
                }
            }
        }
    }
}
