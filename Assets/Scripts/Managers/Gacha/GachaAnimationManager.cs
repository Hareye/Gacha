using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// All animations by script are run as a coroutine by this manager so that they can be handled by one central script
public class GachaAnimationManager : MonoBehaviour
{
    public static GachaAnimationManager instance;

    [SerializeField]
    public Material portalMat;

    private void Awake()
    {
        instance = this;

        portalMat.SetFloat("_Alpha", 0);
    }

    private void OnDestroy()
    {
        portalMat.SetFloat("_Alpha", 0);
    }

    public void startDarken(Image image, float color, float seconds)
    {
        StartCoroutine(darken(image, color, seconds));
    }

    public void startCardFadeIn(GameObject cards, int pulls)
    {
        StartCoroutine(cardFadeIn(cards, pulls));
    }

    public void startPortalFadeIn(float seconds)
    {
        StartCoroutine(portalFadeIn(seconds));
    }

    public void startPortalFadeOut(float seconds)
    {
        StartCoroutine(portalFadeOut(seconds));
    }

    public void startFadeIn(Image image, float seconds)
    {
        StartCoroutine(fadeIn(image, seconds));
    }

    public void startFadeIn(RawImage image, float seconds)
    {
        StartCoroutine(fadeIn(image, seconds));
    }

    public void startFadeIn(CanvasGroup canvas, float seconds)
    {
        StartCoroutine(fadeIn(canvas, seconds));
    }

    public void startFadeOut(Image image, float seconds)
    {
        StartCoroutine(fadeOut(image, seconds));
    }

    public void startFadeOut(CanvasGroup canvas, float seconds)
    {
        StartCoroutine(fadeOut(canvas, seconds));
    }

    public void startZoomIn(Transform transform, float scale, float seconds)
    {
        StartCoroutine(zoomIn(transform, scale, seconds));
    }

    public void stopAll()
    {
        StopAllCoroutines();
    }

    public void startAnimation(IEnumerator anim)
    {
        StartCoroutine(anim);
    }


    public IEnumerator cardFadeIn(GameObject cards, int pulls)
    {
        for (int i = 0; i < pulls; i++)
        {
            StartCoroutine(fadeIn(cards.transform.Find("Card" + i).GetComponent<Image>(), 0.5f));
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator darken(Image image, float color, float seconds)
    {
        float val;

        for (float i = 0; i <= seconds; i += Time.deltaTime)
        {
            val = (200 - (i * ((200 - color) / seconds))) / 255;
            image.color = new Color(val, val, val);
            yield return null;
        }

        val = color / 255;
        image.color = new Color(val, val, val);
    }

    private IEnumerator portalFadeIn(float seconds)
    {
        for (float i = 0; i <= seconds; i += Time.deltaTime)
        {
            portalMat.SetFloat("_Alpha", i / seconds);
            yield return null;
        }

        portalMat.SetFloat("_Alpha", 1);
    }

    private IEnumerator portalFadeOut(float seconds)
    {
        for (float i = seconds; i >= 0; i -= Time.deltaTime)
        {
            portalMat.SetFloat("_Alpha", i / seconds);
            yield return null;
        }

        portalMat.SetFloat("_Alpha", 0);
    }

    private IEnumerator fadeIn(Image image, float seconds)
    {
        for (float i = 0; i <= seconds; i += Time.deltaTime)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, i / seconds);
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    }

    private IEnumerator fadeIn(RawImage image, float seconds)
    {
        for (float i = 0; i <= seconds; i += Time.deltaTime)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, i / seconds);
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    }

    private IEnumerator fadeIn(CanvasGroup canvas, float seconds)
    {
        for (float i = 0; i <= seconds; i += Time.deltaTime)
        {
            canvas.alpha = i / seconds;
            yield return null;
        }

        canvas.alpha = 1;
    }

    private IEnumerator fadeOut(Image image, float seconds)
    {
        for (float i = seconds; i >= 0; i -= Time.deltaTime)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, i / seconds);
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }

    private IEnumerator fadeOut(CanvasGroup canvas, float seconds)
    {
        for (float i = seconds; i >= 0; i -= Time.deltaTime)
        {
            canvas.alpha = i / seconds;
            yield return null;
        }

        canvas.alpha = 0;
    }

    private IEnumerator zoomIn(Transform transform, float scale, float seconds)
    {
        for (float i = seconds / scale; i <= seconds; i += Time.deltaTime)
        {
            transform.localScale = new Vector2((scale / (seconds / i)), (scale / (seconds / i)));
            yield return null;
        }

        transform.localScale = new Vector2(scale, scale);
    }
}
