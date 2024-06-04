using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private GameObject rowPrefab;

    [SerializeField]
    private GameObject unitPrefab;

    private ServerManager server;
    private Endpoints endpoints = new Endpoints();

    private GameObject homeScreen;
    private GameObject unitScreen;
    private GameObject unitInfoScreen;
    private bool unitsLoaded;

    private Color32 urColor;
    private Color32 srColor;
    private Color32 rColor;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        server = GetComponent<ServerManager>();

        homeScreen = canvas.transform.Find("Home Screen").gameObject;
        unitScreen = canvas.transform.Find("Unit Screen").gameObject;
        unitInfoScreen = canvas.transform.Find("Unit Info Screen").gameObject;
        unitsLoaded = false;
        
        urColor = new Color32(255, 247, 0, 255);
        srColor = new Color32(185, 67, 255, 255);
        rColor = new Color32(73, 130, 255, 255);
    }

    public void showScreen(string screen)
    {
        switch (screen)
        {
            case "Home":
                showHomeScreen();
                hideUnitScreen();
                break;
            case "Unit":
                if (!unitsLoaded) StartCoroutine(loadUnitsProcess());
                showUnitScreen();
                hideUnitInfoScreen();
                hideHomeScreen();
                break;
            case "Unit Info":
                showUnitInfoScreen();
                hideUnitScreen();
                break;
        }
    }

    public void setUpUnitInfo(UnitObject unit)
    {
        Sprite unitImg = Resources.Load<Sprite>("Characters/" + unit.charactername + " - " + unit.skinname);
        unitInfoScreen.transform.Find("Display/Splashart").GetComponent<Image>().sprite = unitImg;
        unitInfoScreen.transform.Find("Display/Name").GetComponent<TextMeshProUGUI>().text = unit.charactername;

        GameObject body = unitInfoScreen.transform.Find("Panel/Body").gameObject;
        GameObject stats = body.transform.Find("Stats").gameObject;
        //GameObject tags = body.transform.Find("Tags").gameObject;
        //GameObject series = body.transform.Find("Series").gameObject;

        switch (unit.rarity)
        {
            case "UR":
                stats.transform.Find("Rarity/Text").GetComponent<TextMeshProUGUI>().color = urColor;
                break;
            case "SR":
                stats.transform.Find("Rarity/Text").GetComponent<TextMeshProUGUI>().color = srColor;
                break;
            case "R":
                stats.transform.Find("Rarity/Text").GetComponent<TextMeshProUGUI>().color = rColor;
                break;
        }

        stats.transform.Find("Rarity/Text").GetComponent<TextMeshProUGUI>().text = unit.rarity;
        stats.transform.Find("Skin Name/Text").GetComponent<TextMeshProUGUI>().text = unit.skinname;
        stats.transform.Find("Combat Type/Text").GetComponent<TextMeshProUGUI>().text = unit.combattype;

        stats.transform.Find("Details/Element/Text").GetComponent<TextMeshProUGUI>().text = unit.element;
        stats.transform.Find("Details/Level Text").GetComponent<TextMeshProUGUI>().text = unit.characterlevel + " / 80";
        stats.transform.Find("Details/XP Bar").GetComponent<Slider>().value = 0f;

        stats.transform.Find("Stats/HP/Text").GetComponent<TextMeshProUGUI>().text = unit.hp.ToString();
        stats.transform.Find("Stats/DEF/Text").GetComponent<TextMeshProUGUI>().text = unit.def.ToString();
        stats.transform.Find("Stats/RES/Text").GetComponent<TextMeshProUGUI>().text = unit.res.ToString();
        stats.transform.Find("Stats/ATK/Text").GetComponent<TextMeshProUGUI>().text = unit.atk.ToString();
        stats.transform.Find("Stats/MAG/Text").GetComponent<TextMeshProUGUI>().text = unit.mag.ToString();
        stats.transform.Find("Stats/SPD/Text").GetComponent<TextMeshProUGUI>().text = unit.spd.ToString();
    }

    private IEnumerator loadUnitsProcess()
    {
        Debug.Log("Loading units...");

        // Set up loading here

        IEnumerator unitCoroutine = server.sendRequestWithAuth(endpoints.load);
        yield return StartCoroutine(unitCoroutine);

        string status = unitCoroutine.Current as string;

        UnitsObject units = JsonUtility.FromJson<UnitsObject>(status);
        setUpUnitList(units);

        unitsLoaded = true;

        // Finish loading here
    }

    private void setUpUnitList(UnitsObject units)
    {
        GameObject content = unitScreen.transform.Find("Unit List/Characters/Viewport/Content").gameObject;
        int count = 0;
        int cardsPerRow = 8;

        float startX = -752.5f;
        float deltaX = 215f;

        // Initial row
        GameObject row = Instantiate(rowPrefab, content.transform);

        foreach (UnitObject unit in units.units)
        {
            if (count == cardsPerRow)
            {
                row = Instantiate(rowPrefab, content.transform);
                count = 0;
            }

            GameObject newUnit = Instantiate(unitPrefab, row.transform);
            newUnit.GetComponent<RectTransform>().localPosition = new Vector2(startX + (deltaX * count), 0);

            Sprite unitImg = Resources.Load<Sprite>("Characters/" + unit.charactername + " - " + unit.skinname);
            newUnit.GetComponent<Image>().sprite = unitImg;
            newUnit.GetComponent<Unit>().unit = unit;

            count++;
        }
    }

    private void showHomeScreen()
    {
        homeScreen.SetActive(true);
    }

    private void hideHomeScreen()
    {
        homeScreen.SetActive(false);
    }

    private void showUnitScreen()
    {
        unitScreen.SetActive(true);
    }

    private void hideUnitScreen()
    {
        unitScreen.SetActive(false);
    }

    private void showUnitInfoScreen()
    {
        unitInfoScreen.SetActive(true);
    }

    private void hideUnitInfoScreen()
    {
        unitInfoScreen.SetActive(false);
    }
}
