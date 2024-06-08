using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using objects;

public class PartyManager : MonoBehaviour
{
    public static PartyManager instance;

    [SerializeField]
    private GameObject canvas;

    private GameObject partyScreen;
    private TMP_InputField partyTitle;
    private TMP_Dropdown dropdown;

    private GameObject combatSlots;
    private GameObject supportSlots;
    private GameObject currentSlots;

    private Sprite emptySlot;

    private int totalParties = 5;
    private int currentParty;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        partyScreen = canvas.transform.Find("Party Screen").gameObject;
        partyTitle = partyScreen.transform.Find("Party Display/Title").GetComponent<TMP_InputField>();
        dropdown = partyScreen.transform.Find("Party Display/Party Dropdown").GetComponent<TMP_Dropdown>();

        combatSlots = partyScreen.transform.Find("Party Display/Combat Slots").gameObject;
        supportSlots = partyScreen.transform.Find("Party Display/Support Slots").gameObject;
        currentSlots = combatSlots.transform.Find("Slots").gameObject;

        emptySlot = Resources.Load<Sprite>("Party/Empty Slot");

        createPartyFiles();
    }

    public void showCombatSlideIn()
    {
        MainAnimationManager.instance.startCombatSlideIn(combatSlots, supportSlots, 1f);
        UnitManager.instance.setUpUnitList("Party", "C");
        currentSlots = combatSlots.transform.Find("Slots").gameObject;
    }

    public void showSupportSlideIn()
    {
        MainAnimationManager.instance.startSupportSlideIn(combatSlots, supportSlots, 1f);
        UnitManager.instance.setUpUnitList("Party", "S");
        currentSlots = supportSlots.transform.Find("Slots").gameObject;
    }

    public void setUpDropdown()
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        for (int i = 1; i <= totalParties; i++)
        {
            FileInfo fi = new FileInfo(Application.dataPath + "/Resources/Data/Party" + i + ".txt");

            if (fi.Exists)
            {
                FileStream fs = fi.Open(FileMode.Open, FileAccess.Read);
                Party party;

                using (StreamReader sr = new StreamReader(fs))
                {
                    party = JsonUtility.FromJson<Party>(sr.ReadToEnd());
                }

                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = party.name;

                options.Add(option);
            }
        }

        dropdown.options = options;
    }

    public void setUpDropdown(string oldName, string newName)
    {
        List<TMP_Dropdown.OptionData> options = dropdown.options;
        int index = options.FindIndex(s => s.text == oldName);
        options[index].text = newName;

        dropdown.options = options;
    }

    public void switchParty()
    {
        Debug.Log("Loading party...");

        partyTitle.text = dropdown.captionText.text;
        currentParty = dropdown.value + 1;

        partyTitle.onEndEdit.RemoveAllListeners();
        partyTitle.onEndEdit.AddListener(delegate { changePartyName(dropdown.value + 1); });

        resetSlotImage();
        setUpPartySlots();
    }

    public void setUpPartySlots()
    {
        FileInfo fi = new FileInfo(Application.dataPath + "/Resources/Data/Party" + currentParty + ".txt");

        if (fi.Exists)
        {
            FileStream fs = fi.Open(FileMode.Open, FileAccess.Read);
            Party party;

            using (StreamReader sr = new StreamReader(fs))
            {
                party = JsonUtility.FromJson<Party>(sr.ReadToEnd());
            }

            foreach (PartyUnit unit in party.combatUnits)
            {
                GameObject slot = combatSlots.transform.Find("Slots/Slot " + unit.slot).gameObject;
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Characters/" + unit.charactername + " - " + unit.skinname);
            }

            foreach (PartyUnit unit in party.supportUnits)
            {
                GameObject slot = supportSlots.transform.Find("Slots/Slot " + unit.slot).gameObject;
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Characters/" + unit.charactername + " - " + unit.skinname);
            }
        }
    }

    public void updatePartyMember(int slot, PartyUnit unit, string type)
    {
        FileInfo fi = new FileInfo(Application.dataPath + "/Resources/Data/Party" + currentParty + ".txt");

        if (fi.Exists)
        {
            FileStream fs = fi.Open(FileMode.Open, FileAccess.Read);
            List<PartyUnit> units;
            Party party;

            using (StreamReader sr = new StreamReader(fs))
            {
                party = JsonUtility.FromJson<Party>(sr.ReadToEnd());

                if (type == "C") units = party.combatUnits;
                else units = party.supportUnits;

                int currentSlotIndex = units.FindIndex(s => s.cardid == unit.cardid);

                if (currentSlotIndex >= 0)
                {
                    removeSlotImage(units[currentSlotIndex].slot);
                    units.RemoveAt(currentSlotIndex);
                }

                int slotIndex = units.FindIndex(s => s.slot == slot);

                if (slotIndex >= 0)
                {
                    units[slotIndex] = unit;
                } else {
                    units.Add(unit);
                }
            }

            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine(JsonUtility.ToJson(party));
            }
        }
    }

    private void resetSlotImage()
    {
        foreach (Transform child in currentSlots.transform)
        {
            child.GetComponent<Image>().sprite = emptySlot;
        }
    }

    private void removeSlotImage(int slotNum)
    {
        GameObject slot = currentSlots.transform.Find("Slot " + slotNum).gameObject;
        slot.GetComponent<Image>().sprite = emptySlot;
    }

    private void changePartyName(int partyNum)
    {
        FileInfo fi = new FileInfo(Application.dataPath + "/Resources/Data/Party" + partyNum + ".txt");

        if (fi.Exists)
        {
            Debug.Log("Changing party name...");

            FileStream fs = fi.Open(FileMode.Open, FileAccess.Read);
            Party party;
            string oldName;

            using (StreamReader sr = new StreamReader(fs))
            {
                party = JsonUtility.FromJson<Party>(sr.ReadToEnd());
                oldName = party.name;
                party.name = partyTitle.text;
            }

            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine(JsonUtility.ToJson(party));
            }

            setUpDropdown(oldName, party.name);
        }
    }

    private void createPartyFiles()
    {
        for (int i = 1; i <= totalParties; i++)
        {
            FileInfo fi = new FileInfo(Application.dataPath + "/Resources/Data/Party" + i + ".txt");

            if (!fi.Exists)
            {
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(JsonUtility.ToJson(new Party("Party " + i)));
                }
            }
        }
    }
}
