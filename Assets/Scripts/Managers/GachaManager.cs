using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using urls;

[Serializable]
public class StandardRates
{
    public float rRates;
    public float srRates;
    public float urRates;
    public float rCharacterRates;
    public float srCharacterRates;
    public float urCharacterRates;
    public Character[] characters;
}

[Serializable]
public class Character
{
    public string charactername;
    public string rarity;
}

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
    private Animator panelAnimator;
    private CanvasGroup canvasGroup;

    private TextMeshProUGUI rarityRates;
    private TextMeshProUGUI characterRates;

    private Button pull1;
    private Button pull10;

    private bool ratesVisible = false;

    private ServerManager server;
    private Endpoints endpoints = new Endpoints();

    private void Awake()
    {
        server = GetComponent<ServerManager>();

        title = gachaScreen.transform.Find("Title/Text").GetComponent<TextMeshProUGUI>();
        bannerImage = gachaScreen.transform.Find("Banner Image").GetComponent<Image>();
        focusSelector = gachaScreen.transform.Find("Banner Image/Focus Selector").gameObject;
        popUpBody = gachaScreen.transform.Find("Banner Image/Pull Rates/Pop-up Info/Body").gameObject;
        pull1 = gachaScreen.transform.Find("Pull 1").GetComponent<Button>();
        pull10 = gachaScreen.transform.Find("Pull 10").GetComponent<Button>();
        canvasGroup = popUpInfo.GetComponent<CanvasGroup>();

        focusDropdown = focusSelector.GetComponent<TMP_Dropdown>();
        panelAnimator = popUpInfo.GetComponent<Animator>();

        loadBanner("Standard");
    }

    public void loadBanner(string banner)
    {
        Debug.Log("Loading " + banner + " banner...");
        setUpBanner(banner);
    }

    public void togglePopUp()
    {
        ratesVisible = !ratesVisible;

        if (ratesVisible)
        {
            panelAnimator.Play("Rates_FadeIn");
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            focusDropdown.enabled = false;

            if (title.text.Equals("Standard"))
            {
                StartCoroutine(loadRatesProcess("Standard"));
            }
            else if (title.text.Equals("Focus"))
            {
                StartCoroutine(loadRatesProcess("Focus", focusDropdown.captionText.text));
            }
        }
        else
        {
            panelAnimator.Play("Rates_FadeOut");
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            focusDropdown.enabled = true;
        }
    }

    private void setUpBanner(string banner)
    {
        if (ratesVisible)
        {
            hideRatesOnBannerLoad();
        }

        // Disable button until everything has loaded
        pull1.gameObject.GetComponent<Button>().interactable = false;
        pull10.gameObject.GetComponent<Button>().interactable = false;

        pull1.onClick.RemoveAllListeners();
        pull10.onClick.RemoveAllListeners();

        if (banner.Equals("Standard"))
        {
            title.text = "Standard";
            bannerImage.sprite = Resources.Load<Sprite>("Banners/Standard Banner Image");
            focusSelector.SetActive(false);

            pull1.onClick.AddListener(delegate { StartCoroutine(pullProcess(1, "Standard")); });
            pull10.onClick.AddListener(delegate { StartCoroutine(pullProcess(10, "Standard")); });
        } else if (banner.Equals("Focus"))
        {
            title.text = "Focus";
            bannerImage.sprite = Resources.Load<Sprite>("Banners/Focus Banner Image");
            focusSelector.SetActive(true);

            pull1.onClick.AddListener(delegate { StartCoroutine(pullProcess(1, "Focus")); });
            pull10.onClick.AddListener(delegate { StartCoroutine(pullProcess(10, "Focus")); });
        }

        // Enable button after everything has loaded
        pull1.gameObject.GetComponent<Button>().interactable = true;
        pull10.gameObject.GetComponent<Button>().interactable = true;
    }

    private void hideRatesOnBannerLoad()
    {
        ratesVisible = !ratesVisible;

        panelAnimator.Play("Rates_FadeOut");
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        focusDropdown.enabled = true;
    }

    private IEnumerator pullProcess(int pulls, string banner)
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("pulls", pulls.ToString()),
            new KeyValuePair<string, string>("banner", banner),
        };

        IEnumerator pullCoroutine = server.sendRequestWithAuth(endpoints.gachaPull, list);
        yield return StartCoroutine(pullCoroutine);

        string status = pullCoroutine.Current as string;
    }

    private IEnumerator loadRatesProcess(string banner, string anime = "N/A")
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("banner", banner),
            new KeyValuePair<string, string>("anime", anime),
        };

        // Set up loading here

        IEnumerator loadRatesCoroutine = server.sendRequestWithAuth(endpoints.loadRates, list);
        yield return StartCoroutine(loadRatesCoroutine);

        string status = loadRatesCoroutine.Current as string;

        rarityRates = gachaScreen.transform.Find("Banner Image/Pull Rates/Pop-up Info/Body/Rarity Rates").GetComponent<TextMeshProUGUI>();
        characterRates = gachaScreen.transform.Find("Banner Image/Pull Rates/Pop-up Info/Body/Scroll/Viewport/Character Rates").GetComponent<TextMeshProUGUI>();

        StandardRates rates = JsonUtility.FromJson<StandardRates>(status);

        rarityRates.text = "<color=\"red\">UR: <color=\"white\"> \t " + rates.urRates + "% \n" +
                           "<color=\"red\">SR: <color=\"white\"> \t " + rates.srRates + "% \n" +
                           "<color=\"red\">R: <color=\"white\"> \t " + rates.rRates + "%";

        string urText = "<u>UR:</u> \n";
        string srText = "\n<u>SR:</u> \n";
        string rText = "\n<u>R:</u> \n";

        foreach(Character character in rates.characters)
        {
            if (character.rarity.Equals("UR"))
            {
                urText += character.charactername + " (" + rates.urCharacterRates + "%) \n";
            } else if (character.rarity.Equals("SR"))
            {
                srText += character.charactername + " (" + rates.srCharacterRates + "%) \n";
            } else
            {
                rText += character.charactername + " (" + rates.rCharacterRates + "%) \n";
            }
        }

        characterRates.text = urText + srText + rText;

        // Finish loading here
    }
}
