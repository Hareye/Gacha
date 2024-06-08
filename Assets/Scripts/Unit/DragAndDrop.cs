using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Transform currentParent;
    private Transform newParent;

    private float currentX;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        currentParent = transform.parent;
        newParent = canvas.transform.Find("Party Screen/Unit List/No Mask").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(newParent);
        currentX = rectTransform.localPosition.x;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(currentParent);
        rectTransform.localPosition = new Vector2(currentX, 0);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
