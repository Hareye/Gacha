using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using objects;
using urls;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private GameObject rowPrefab;

    [SerializeField]
    private GameObject unitPrefab;

    [SerializeField]
    private GameObject unitPartyPrefab;

    private ServerManager server;
    private Endpoints endpoints = new Endpoints();

    private GameObject unitScreen;
    private GameObject unitInfoScreen;
    private GameObject unitBar;
    private GameObject characters;


    private Color32 urColor;
    private Color32 srColor;
    private Color32 rColor;

    private UnitsObject units;
    private bool unitsLoaded;
    private bool viewMode;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        server = GetComponent<ServerManager>();

        unitScreen = canvas.transform.Find("Unit Screen").gameObject;
        unitInfoScreen = canvas.transform.Find("Unit Info Screen").gameObject;
        unitBar = unitScreen.transform.Find("Unit Bar").gameObject;

        characters = unitScreen.transform.Find("Unit List/Characters").gameObject;

        urColor = new Color32(255, 247, 0, 255);
        srColor = new Color32(185, 67, 255, 255);
        rColor = new Color32(73, 130, 255, 255);

        unitsLoaded = false;
        viewMode = true;
    }

    public void toggleMode()
    {
        viewMode = !viewMode;

        if (viewMode) unitBar.transform.Find("Mode/Text").GetComponent<TextMeshProUGUI>().text = "Viewing";
        else unitBar.transform.Find("Mode/Text").GetComponent<TextMeshProUGUI>().text = "Dismissing";
    }

    public void setUpCharacters(Transform transform, int width)
    {
        Debug.Log("Setting up characters...");

        RectTransform characterTransform = characters.transform.GetComponent<RectTransform>();
        RectTransform contentTransform = characters.transform.Find("Viewport/Content").GetComponent<RectTransform>();

        characters.transform.SetParent(transform);
        characterTransform.sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, characterTransform.sizeDelta.y);
        contentTransform.sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, characterTransform.sizeDelta.y);

        characterTransform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void setUpUnitInfo(UnitObject unit)
    {
        Sprite unitImg = Resources.Load<Sprite>("Characters/" + unit.charactername + " - " + unit.skinname);
        unitInfoScreen.transform.Find("Display/Splashart").GetComponent<Image>().sprite = unitImg;
        unitInfoScreen.transform.Find("Display/Name").GetComponent<TextMeshProUGUI>().text = unit.charactername;

        GameObject body = unitInfoScreen.transform.Find("Panel/Body").gameObject;
        GameObject stats = body.transform.Find("Stats").gameObject;

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

    public void setUpUnitList(string screen, string type = null)
    {
        if (screen.Equals("Unit"))
        {
            int cardsPerRow = 8;
            float startX = -752.5f;

            modifyUnitList(cardsPerRow, startX, unitPrefab, type);
        }

        if (screen.Equals("Party"))
        {
            int cardsPerRow = 5;
            float startX = -430f;

            modifyUnitList(cardsPerRow, startX, unitPartyPrefab, type);
        }
    }

    public IEnumerator loadUnits(string screen, string type = null)
    {
        yield return StartCoroutine(loadUnitsProcess());
        setUpUnitList(screen, type);
    }

    public bool getUnitsLoaded()
    {
        return unitsLoaded;
    }

    private void modifyUnitList(int cardsPerRow, float startX, GameObject unitPrefab, string type = null)
    {
        GameObject content = characters.transform.Find("Viewport/Content").gameObject;
        int rowWidth = (cardsPerRow * 185) + ((cardsPerRow + 1) * 30);

        foreach (RectTransform rowTransform in content.transform)
        {
            Destroy(rowTransform.gameObject);
        }

        int count = 0;
        float deltaX = 215f;

        // Initial row
        GameObject row = Instantiate(rowPrefab, content.transform);
        row.GetComponent<RectTransform>().sizeDelta = new Vector2(rowWidth, row.GetComponent<RectTransform>().sizeDelta.y);

        foreach (UnitObject unit in units.units)
        {
            if (type != null)
            {
                if (unit.combattype != type) continue;
            }

            if (count == cardsPerRow)
            {
                row = Instantiate(rowPrefab, content.transform);
                row.GetComponent<RectTransform>().sizeDelta = new Vector2(rowWidth, row.GetComponent<RectTransform>().sizeDelta.y);
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

    private IEnumerator loadUnitsProcess()
    {
        Debug.Log("Loading units...");

        // Set up loading here

        IEnumerator unitCoroutine = server.sendRequestWithAuth(endpoints.load);
        yield return StartCoroutine(unitCoroutine);

        string status = unitCoroutine.Current as string;

        units = JsonUtility.FromJson<UnitsObject>(status);
        unitsLoaded = true;

        // Finish loading here
    }
}
