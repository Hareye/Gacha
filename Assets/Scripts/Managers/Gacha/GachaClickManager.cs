using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaClickManager : MonoBehaviour
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
                if (hit.collider.tag.Equals("Card"))
                {
                    Debug.Log("Hit card...");
                    //Debug.Log(hit.transform.gameObject);

                    Card card = hit.transform.GetComponent<Card>();

                    card.showCardImage();
                    card.removeMaterial();
                } else if (hit.collider.tag.Equals("Skip Portal")) {
                    Debug.Log("Skip portal...");

                    GachaAnimationManager.instance.stopAll();
                    GachaManager.instance.skipPortalAnimation();
                } else if (hit.collider.tag.Equals("Portal"))
                {
                    Debug.Log("Hit portal...");

                    GachaAnimationManager.instance.stopAll();
                    GachaAnimationManager.instance.startAnimation(GachaManager.instance.showZoomAnimation());
                } else if (hit.collider.tag.Equals("Skip Zoom"))
                {
                    Debug.Log("Skip zoom...");

                    GachaAnimationManager.instance.stopAll();
                    GachaManager.instance.skipZoomAnimation();
                } else if (hit.collider.tag.Equals("Skip Result"))
                {
                    Debug.Log("Skip result...");

                    GachaAnimationManager.instance.stopAll();
                    GachaManager.instance.skipResultsAnimation();
                }
                else if (hit.collider.tag.Equals("Skip Card"))
                {
                    Debug.Log("Skip card...");

                    GachaAnimationManager.instance.stopAll();
                    GachaManager.instance.skipCardsAnimation();
                }
            }
        }
    }
}
