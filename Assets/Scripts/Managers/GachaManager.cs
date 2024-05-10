using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using urls;

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

    private ServerManager server;
    private Endpoints endpoints = new Endpoints();

    private void Awake()
    {
        title = gachaScreen.transform.Find("Title/Text").GetComponent<TextMeshProUGUI>();
        bannerImage = gachaScreen.transform.Find("Banner Image").GetComponent<Image>();
        focusSelector = gachaScreen.transform.Find("Banner Image/Focus Selector").gameObject;
        popUpBody = gachaScreen.transform.Find("Banner Image/Pull Rates/Pop-up Info/Body").gameObject;
        pull1 = gachaScreen.transform.Find("Pull 1").GetComponent<Button>();
        pull10 = gachaScreen.transform.Find("Pull 10").GetComponent<Button>();

        server = GetComponent<ServerManager>();

        focusDropdown = focusSelector.GetComponent<TMP_Dropdown>();
        panelAnimator = popUpInfo.GetComponent<Animator>();

        loadBanner("Standard");
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
        Debug.Log("Loading standard banner...");

        title.text = "Standard";
        bannerImage.sprite = Resources.Load<Sprite>("Banners/Standard Banner Image");
        focusSelector.SetActive(false);

        // Load character rates by calling making a request to the server
        // Set up pull buttons

        StartCoroutine(loadRates("Standard"));

        pull1.onClick.RemoveAllListeners();
        pull10.onClick.RemoveAllListeners();

        pull1.onClick.AddListener(delegate { StartCoroutine(pullProcess(1, "Standard")); });
        pull10.onClick.AddListener(delegate { StartCoroutine(pullProcess(10, "Standard")); });
    }

    public void loadFocus()
    {
        Debug.Log("Loading focus banner...");

        title.text = "Focus";
        bannerImage.sprite = Resources.Load<Sprite>("Banners/Focus Banner Image");
        focusSelector.SetActive(true);

        //loadFocusAnime(focusDropdown.itemText.text);

        //pull1.onClick.RemoveAllListeners();
        //pull10.onClick.RemoveAllListeners();

        //pull1.onClick.AddListener(delegate { StartCoroutine(pullProcess(1, "Focus")); });
        //pull10.onClick.AddListener(delegate { StartCoroutine(pullProcess(10, "Focus")); });
    }

    public void loadFocusAnime(string animeName)
    {
        // Call when user selects a show from dropdown
        // Load character rates of a specific show by making a request to the server
        // Set up pull buttons

        //Debug.Log(animeName);
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

    public IEnumerator pullProcess(int pulls, string banner)
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("pulls", pulls.ToString()),
            new KeyValuePair<string, string>("banner", banner),
        };

        IEnumerator pullCoroutine = server.sendPullRequest(endpoints.gachaPull, list);
        yield return StartCoroutine(pullCoroutine);

        string status = pullCoroutine.Current as string;
    }

    public IEnumerator loadRates(string banner)
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("banner", banner),
        };

        IEnumerator loadRatesCoroutine = server.sendPullRequest(endpoints.loadRates, list);
        yield return StartCoroutine(loadRatesCoroutine);

        string status = loadRatesCoroutine.Current as string;

        Debug.Log(status);
    }
}
