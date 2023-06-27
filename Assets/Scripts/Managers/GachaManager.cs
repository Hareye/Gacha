using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GachaManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gachaScreen;

    [SerializeField]
    private GameObject popUpInfo;

    private TextMeshProUGUI title;
    private Image bannerImage;
    private GameObject focusSelector;
    private GameObject popUpBody;
    private TMP_Dropdown focusDropdown;

    private Button pull1;
    private Button pull10;

    private bool visible = false;
    private Animator panelAnimator;

    private void Awake()
    {
        title = gachaScreen.transform.Find("Title/Text").GetComponent<TextMeshProUGUI>();
        bannerImage = gachaScreen.transform.Find("Banner Image").GetComponent<Image>();
        focusSelector = gachaScreen.transform.Find("Banner Image/Focus Selector").gameObject;
        popUpBody = gachaScreen.transform.Find("Banner Image/Pull Rates/Pop-up Info/Body").gameObject;
        pull1 = gachaScreen.transform.Find("Pull 1").GetComponent<Button>();
        pull10 = gachaScreen.transform.Find("Pull 10").GetComponent<Button>();

        focusDropdown = focusSelector.GetComponent<TMP_Dropdown>();
        panelAnimator = popUpInfo.GetComponent<Animator>();
    }

    public void loadBanner(string banner)
    {
        if (banner.Equals("Standard"))
        {
            loadStandard();
        } else if (banner.Equals("Focus"))
        {
            loadFocus();
        }
    }

    public void loadStandard()
    {
        title.text = "Standard";
        bannerImage.sprite = Resources.Load<Sprite>("Banners/Standard Banner Image");
        focusSelector.SetActive(false);

        // Load character rates by calling making a request to the server
        // Set up pull buttons
    }

    public void loadFocus()
    {
        title.text = "Focus";
        bannerImage.sprite = Resources.Load<Sprite>("Banners/Focus Banner Image");
        focusSelector.SetActive(true);

        loadFocusAnime(focusDropdown.itemText.text);
    }

    public void loadFocusAnime(string animeName)
    {
        // Call when user selects a show from dropdown
        // Load character rates of a specific show by making a request to the server
        // Set up pull buttons
    }

    public void togglePopUp()
    {
        visible = !visible;

        if (visible == true)
        {
            panelAnimator.Play("Rates_FadeIn");
        }
        else
        {
            panelAnimator.Play("Rates_FadeOut");
        }
    }
}
