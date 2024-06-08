using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAnimationManager : MonoBehaviour
{
    public static MainAnimationManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void startSupportSlideIn(GameObject combatSlots, GameObject supportSlots, float seconds)
    {
        StartCoroutine(supportSlideIn(combatSlots, supportSlots, seconds));
    }

    public void startCombatSlideIn(GameObject combatSlots, GameObject supportSlots, float seconds)
    {
        StartCoroutine(combatSlideIn(combatSlots, supportSlots, seconds));
    }

    private IEnumerator supportSlideIn(GameObject combatSlots, GameObject supportSlots, float seconds)
    {
        RectTransform cTransform = combatSlots.GetComponent<RectTransform>();
        RectTransform sTransform = supportSlots.GetComponent<RectTransform>();

        float sCurrentPos = sTransform.localPosition.x;
        float cNewPos = -605;

        for (float i = 0; i <= seconds; i += Time.deltaTime)
        {
            cTransform.localPosition = new Vector2(i * (cNewPos / seconds), 0);
            sTransform.localPosition = new Vector2(sCurrentPos - ((sCurrentPos / seconds) * i), 0);
            yield return null;
        }

        cTransform.localPosition = new Vector2(cNewPos, 0);
        sTransform.localPosition = new Vector2(0, 0);
    }

    private IEnumerator combatSlideIn(GameObject combatSlots, GameObject supportSlots, float seconds)
    {
        RectTransform cTransform = combatSlots.GetComponent<RectTransform>();
        RectTransform sTransform = supportSlots.GetComponent<RectTransform>();

        float cCurrentPos = cTransform.localPosition.x;
        float sNewPos = 605;

        for (float i = 0; i <= seconds; i += Time.deltaTime)
        {
            sTransform.localPosition = new Vector2(i * (sNewPos / seconds), 0);
            cTransform.localPosition = new Vector2(cCurrentPos - ((cCurrentPos / seconds) * i), 0);
            yield return null;
        }

        sTransform.localPosition = new Vector2(sNewPos, 0);
        cTransform.localPosition = new Vector2(0, 0);
    }
}
