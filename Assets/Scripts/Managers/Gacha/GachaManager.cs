using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using objects;
using urls;

public class GachaManager : MonoBehaviour
{
    public static GachaManager instance;

    [SerializeField]
    private GameObject panel;

    private Image background;

    private GameObject gachaScreen;
    private GameObject portalContainer;
    private GameObject portalScreen;
    private GameObject resultScreen;

    private TextMeshProUGUI title;
    private Image bannerImage;
    private GameObject focusSelector;
    private TMP_Dropdown focusDropdown;

    private GameObject popUpInfo;
    private GameObject popUpBody;
    private Animator popUpAnimator;
    private CanvasGroup popUpCanvasGroup;

    private GameObject cards;
    private Material urGlow;
    private Material srGlow;
    private Material rGlow;

    private TextMeshProUGUI rarityRates;
    private TextMeshProUGUI characterRates;

    private Button pull1;
    private Button pull10;

    private bool ratesVisible = false;
    private int pulls;

    private ServerManager server;
    private Endpoints endpoints = new Endpoints();

    private byte originalBgColor;
    private byte bgDarkened;
    private string originalPortalTag;
    private string originalResultTag;
    private Vector2 originalPortalScale;

    private void Awake()
    {
        instance = this;

        server = GetComponent<ServerManager>();

        originalBgColor = 200;
        bgDarkened = 75;
        originalPortalTag = "Skip Portal";
        originalResultTag = "Skip Result";
        originalPortalScale = new Vector2(1, 1);

        background = panel.transform.Find("../BG").GetComponent<Image>();
        background.color = new Color32(originalBgColor, originalBgColor, originalBgColor, 255);

        gachaScreen = panel.transform.Find("Gacha Screen").gameObject;
        portalContainer = panel.transform.Find("Portal Container").gameObject;
        portalScreen = portalContainer.transform.Find("Portal Screen").gameObject;
        resultScreen = portalContainer.transform.Find("Result Screen").gameObject;

        title = gachaScreen.transform.Find("Title/Text").GetComponent<TextMeshProUGUI>();
        bannerImage = gachaScreen.transform.Find("Banner Image").GetComponent<Image>();
        focusSelector = gachaScreen.transform.Find("Banner Image/Focus Selector").gameObject;
        focusDropdown = focusSelector.GetComponent<TMP_Dropdown>();

        popUpInfo = gachaScreen.transform.Find("Banner Image/Pull Rates/Pop-up Info").gameObject;
        popUpAnimator = popUpInfo.GetComponent<Animator>();
        popUpBody = gachaScreen.transform.Find("Banner Image/Pull Rates/Pop-up Info/Body").gameObject;
        popUpCanvasGroup = popUpInfo.GetComponent<CanvasGroup>();

        cards = resultScreen.transform.Find("Cards").gameObject;
        urGlow = Resources.Load<Material>("Shaders/Gacha/UR Glow");
        srGlow = Resources.Load<Material>("Shaders/Gacha/SR Glow");
        rGlow = Resources.Load<Material>("Shaders/Gacha/R Glow");

        pull1 = gachaScreen.transform.Find("Pull 1").GetComponent<Button>();
        pull10 = gachaScreen.transform.Find("Pull 10").GetComponent<Button>();

        originalPortalTag = "Skip Portal";

        loadBanner("Standard");
    }

    /*****************************************
     * 
     * PUBLIC METHODS
     * 
     *****************************************/

    // Called by UI buttons to load banners
    public void loadBanner(string banner)
    {
        Debug.Log("Loading " + banner + " banner...");
        setUpBanner(banner);
    }

    // Toggle pop-up displaying rates of each character
    public void togglePopUp()
    {
        ratesVisible = !ratesVisible;

        if (ratesVisible)
        {
            popUpAnimator.Play("Rates_FadeIn");
            setCanvasGroup(popUpCanvasGroup, true);

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
            popUpAnimator.Play("Rates_FadeOut");
            setCanvasGroup(popUpCanvasGroup, false);

            focusDropdown.enabled = true;
        }
    }

    // Button to return to gacha screen from results screen
    public void returnToGachaScreen()
    {
        // Reset the view back to the gacha screen, and reset the cards
        setCanvasGroup(gachaScreen.GetComponent<CanvasGroup>(), 1, true);
        setCanvasGroup(resultScreen.GetComponent<CanvasGroup>(), 0, false);
        resetCards();

        // Reset the values manipulated by the script animations
        background.color = new Color32(originalBgColor, originalBgColor, originalBgColor, 255);
        portalScreen.transform.Find("Portal").transform.localScale = originalPortalScale;
        portalScreen.tag = originalPortalTag;
        resultScreen.tag = originalResultTag;
    }

    public IEnumerator showPortalAnimation()
    {
        // Enable collider on portal screen for detecting clicks on portal screen
        portalScreen.GetComponent<BoxCollider>().enabled = true;

        // Start darken background and fade in portal animation
        GachaAnimationManager.instance.startDarken(background, bgDarkened, 1);
        GachaAnimationManager.instance.startPortalFadeIn(1);
        yield return new WaitForSeconds(1f);

        // Set up tag to start the pull animation when user clicks on portal
        portalScreen.tag = "Portal";
    }

    public void skipPortalAnimation()
    {
        // Skip darken background and fade in portal animation
        background.color = new Color32(bgDarkened, bgDarkened, bgDarkened, 255);
        GachaAnimationManager.instance.portalMat.SetFloat("_Alpha", 1);

        // Set up tag to start the pull animation when user clicks on portal
        portalScreen.tag = "Portal";
    }

    public IEnumerator showZoomAnimation()
    {
        // Set up tag to skip zoom animation if user clicks again during portal animation
        portalScreen.tag = "Skip Zoom";

        // Start zoom in and fade out portal animation
        GachaAnimationManager.instance.startZoomIn(portalScreen.transform.Find("Portal").transform, 10, 2);
        GachaAnimationManager.instance.startPortalFadeOut(2);
        yield return new WaitForSeconds(2f);

        // Start results animation and enable collider on results screen for detecting clicks on results screen
        resultScreen.GetComponent<BoxCollider>().enabled = true;
        GachaAnimationManager.instance.startAnimation(showResultsAnimation());
    }

    public void skipZoomAnimation()
    {
        // Skip zoom in and fade out animation
        GachaAnimationManager.instance.portalMat.SetFloat("_Alpha", 0);
        resultScreen.GetComponent<CanvasGroup>().alpha = 1;

        // Continue showing results animation and enable collider on results screen for detecting clicks on results screen
        resultScreen.GetComponent<BoxCollider>().enabled = true;
        GachaAnimationManager.instance.startAnimation(showResultsAnimation());
    }

    public IEnumerator showResultsAnimation()
    {
        // Hide the portal screen and disable portal screen collider that detects for clicks on portal screen
        portalScreen.GetComponent<BoxCollider>().enabled = false;

        // Start fade in results screen animation
        GachaAnimationManager.instance.startFadeIn(resultScreen.GetComponent<CanvasGroup>(), 2);
        yield return new WaitForSeconds(2f);

        // Start showing cards animation and set up tag to skip card animation if user clicks during cards animation
        resultScreen.tag = "Skip Card";
        GachaAnimationManager.instance.startAnimation(showCardsAnimation());
    }

    public void skipResultsAnimation()
    {
        // Skip fade in results screen animation
        resultScreen.GetComponent<CanvasGroup>().alpha = 1;
        resultScreen.tag = "Skip Card";

        // Continue showing cards animation
        GachaAnimationManager.instance.startAnimation(showCardsAnimation());
    }

    public IEnumerator showCardsAnimation()
    {
        // Start fade in cards animation
        GachaAnimationManager.instance.startCardFadeIn(cards, pulls);
        setCanvasGroup(resultScreen.GetComponent<CanvasGroup>(), true);
        yield return new WaitForSeconds(5f);

        // Disable collider on result screen for detecting clicks for skipping animations and enable card colliders
        resultScreen.GetComponent<BoxCollider>().enabled = false;

        for (int i = 0; i < pulls; i++)
        {
            cards.transform.Find("Card" + i).GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void skipCardsAnimation()
    {
        // Disable collider on result screen for detecting clicks for skipping animations
        resultScreen.GetComponent<BoxCollider>().enabled = false;

        // Skip fade in cards animation
        for (int i = 0; i < pulls; i++)
        {
            Transform card = cards.transform.Find("Card" + i);
            card.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            card.GetComponent<BoxCollider>().enabled = true;
        }
    }

    /*****************************************
     * 
     * PRIVATE METHODS
     * 
     *****************************************/

    private void setUpBanner(string banner)
    {
        if (ratesVisible)
        {
            hideRatesOnBannerLoad(banner);
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

            pull1.onClick.AddListener(delegate { StartCoroutine(pullProcess(1, "Focus", focusDropdown.captionText.text)); });
            pull10.onClick.AddListener(delegate { StartCoroutine(pullProcess(10, "Focus", focusDropdown.captionText.text)); });
        }

        // Enable button after everything has loaded
        pull1.gameObject.GetComponent<Button>().interactable = true;
        pull10.gameObject.GetComponent<Button>().interactable = true;
    }

    private void hideRatesOnBannerLoad(string banner)
    {
        ratesVisible = !ratesVisible;

        popUpAnimator.Play("Rates_FadeOut");
        setCanvasGroup(popUpCanvasGroup, false);

        if (banner.Equals("Focus"))
        {
            focusDropdown.enabled = true;
        }
    }

    private IEnumerator pullProcess(int pulls, string banner, string anime = "N/A")
    {
        this.pulls = pulls;

        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("pulls", pulls.ToString()),
            new KeyValuePair<string, string>("banner", banner),
            new KeyValuePair<string, string>("anime", anime),
        };

        // Set up loading here

        IEnumerator pullCoroutine = server.sendRequestWithAuth(endpoints.gachaPull, list);
        yield return StartCoroutine(pullCoroutine);

        string status = pullCoroutine.Current as string;

        Characters characters = JsonUtility.FromJson<Characters>(status);
        setUpPullResults(characters);

        // Hide gacha screen
        setCanvasGroup(gachaScreen.GetComponent<CanvasGroup>(), 0, false);

        // Begin portal animation for showing portal screen
        GachaAnimationManager.instance.startAnimation(showPortalAnimation());

        // Finish loading here
    }

    private void resetCards()
    {
        for (int i = 0; i < 10; i++)
        {
            Card card = cards.transform.Find("Card" + i).GetComponent<Card>();
            card.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            card.GetComponent<BoxCollider>().enabled = false;
            card.showCardBack();
            card.setTag("Untagged");
        }
    }

    private void setUpPullResults(Characters characters)
    {
        for (int i = 0; i < characters.characters.Length; i++)
        {
            GameObject obj = cards.transform.Find("Card" + i).gameObject;

            Card card = obj.GetComponent<Card>();
            Sprite characterImg = Resources.Load<Sprite>("Characters/" + characters.characters[i].charactername + " - " + characters.characters[i].skinname);

            card.setCardImage(characterImg);
            card.setTag("Card");

            switch (characters.characters[i].rarity)
            {
                case "UR":
                    card.setMaterial(urGlow);
                    break;
                case "SR":
                    card.setMaterial(srGlow);
                    break;
                case "R":
                    card.setMaterial(rGlow);
                    break;
            }
        }
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

        rarityRates = popUpBody.transform.Find("Rarity Rates").GetComponent<TextMeshProUGUI>();
        characterRates = popUpBody.transform.Find("Scroll/Viewport/Character Rates").GetComponent<TextMeshProUGUI>();

        StandardRates rates = JsonUtility.FromJson<StandardRates>(status);

        rarityRates.text = "<color=\"red\">UR: <color=\"white\"> \t " + rates.urRates + "% \n" +
                           "<color=\"red\">SR: <color=\"white\"> \t " + rates.srRates + "% \n" +
                           "<color=\"red\">R: <color=\"white\"> \t " + rates.rRates + "%";

        string urText = "<u>UR:</u> \n";
        string srText = "\n<u>SR:</u> \n";
        string rText = "\n<u>R:</u> \n";

        foreach(Character character in rates.characters)
        {
            switch(character.rarity)
            {
                case "UR":
                    urText += character.charactername + " (" + rates.urCharacterRates + "%) \n";
                    break;
                case "SR":
                    srText += character.charactername + " (" + rates.srCharacterRates + "%) \n";
                    break;
                case "R":
                    rText += character.charactername + " (" + rates.rCharacterRates + "%) \n";
                    break;
            }
        }

        characterRates.text = urText + srText + rText;

        // Finish loading here
    }

    /*****************************************
     * 
     * HELPER METHODS
     * 
     *****************************************/
    private void setCanvasGroup(CanvasGroup cg, bool b)
    {
        cg.interactable = b;
        cg.blocksRaycasts = b;
    }

    private void setCanvasGroup(CanvasGroup cg, float a, bool b)
    {
        cg.alpha = a;
        cg.interactable = b;
        cg.blocksRaycasts = b;
    }
}
