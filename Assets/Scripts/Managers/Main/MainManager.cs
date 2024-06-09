using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using objects;
using urls;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    [SerializeField]
    private GameObject canvas;

    private GameObject homeScreen;
    private GameObject unitScreen;
    private GameObject unitInfoScreen;
    private GameObject partyScreen;

    private GameObject currentScreen;
    private string currentScreenName;
    private GameObject currentBody;
    private GameObject currentBar;

    private GameObject unitScreenList;
    private GameObject partyScreenList;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        homeScreen = canvas.transform.Find("Home Screen").gameObject;
        unitScreen = canvas.transform.Find("Unit Screen").gameObject;
        unitInfoScreen = canvas.transform.Find("Unit Info Screen").gameObject;
        partyScreen = canvas.transform.Find("Party Screen").gameObject;

        currentScreen = homeScreen;
        currentScreenName = "Home";
        currentBody = unitInfoScreen.transform.Find("Panel/Body/Stats").gameObject;
        currentBar = unitInfoScreen.transform.Find("Panel/Bar/Stats").gameObject;

        unitScreenList = unitScreen.transform.Find("Unit List").gameObject;
        partyScreenList = partyScreen.transform.Find("Unit List").gameObject;
    }

    public void showTab(string tab)
    {
        setBody(tab);
        setBar(tab);
    }

    public void showScreen(string screen)
    {
        GameObject nextScreen = canvas.transform.Find(screen + " Screen").gameObject;

        if (currentScreen == nextScreen) return;

        currentScreen.SetActive(false);

        currentScreen = nextScreen;
        currentScreenName = screen;
        currentScreen.SetActive(true);

        if (screen.Equals("Unit"))
        {
            if (!UnitManager.instance.getUnitsLoaded())
            {
                StartCoroutine(UnitManager.instance.loadUnits(screen));
            } else
            {
                UnitManager.instance.formatUnits(screen);
            }
            
            UnitManager.instance.formatCharacterList(unitScreenList.transform);
        }

        if (screen.Equals("Party"))
        {
            if (!UnitManager.instance.getUnitsLoaded())
            {
                StartCoroutine(UnitManager.instance.loadUnits(screen, "C"));
            } else
            {
                UnitManager.instance.formatUnits(screen, "C");
            }

            UnitManager.instance.formatCharacterList(partyScreenList.transform);
            PartyManager.instance.loadDropdown();
            PartyManager.instance.switchParty();
        }
    }

    public string getCurrentScreenName()
    {
        return currentScreenName;
    }

    private void setBody(string tab)
    {
        GameObject nextBody = unitInfoScreen.transform.Find("Panel/Body/" + tab).gameObject;

        if (currentBody == nextBody) return;

        Debug.Log("Showing " + tab + " tab...");

        currentBody.SetActive(false);

        currentBody = nextBody;
        currentBody.SetActive(true);
    }

    private void setBar(string tab)
    {
        GameObject nextBar = unitInfoScreen.transform.Find("Panel/Bar/" + tab).gameObject;

        if (currentBar == nextBar) return;

        Sprite selected = Resources.Load<Sprite>("Units/Bookmark Selected");
        Sprite bookmark = Resources.Load<Sprite>("Units/Bookmark");

        currentBar.transform.Find("BG").GetComponent<Image>().sprite = bookmark;
        nextBar.transform.Find("BG").GetComponent<Image>().sprite = selected;
        currentBar = nextBar;
    }
}
