using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private Image currentImage;
    private Sprite cardImage;
    private Sprite cardBack;

    private void Awake()
    {
        currentImage = GetComponent<Image>();

        cardBack = Resources.Load<Sprite>("Card Back");
    }

    public void setCardImage(Sprite newImg)
    {
        cardImage = newImg;
    }

    public void showCardBack()
    {
        currentImage.sprite = cardBack;
    }

    public void showCardImage()
    {
        currentImage.sprite = cardImage;
    }

    public void removeMaterial()
    {
        currentImage.material = null;
    }

    public void setMaterial(Material newMaterial)
    {
        currentImage.material = newMaterial;
    }

    public void setTag(string tag)
    {
        this.tag = tag;
    }
}
