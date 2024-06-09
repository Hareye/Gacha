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

        emptySlot = Resources.Load<Sprite>("Party/Empty Slot");

        createPartyFiles();
    }

    public void setCombatDisplay()
    {
        MainAnimationManager.instance.StopAllCoroutines();
        MainAnimationManager.instance.startCombatSlideIn(combatSlots, supportSlots, 0f);
    }

    public void showCombatSlideIn()
    {
        MainAnimationManager.instance.startCombatSlideIn(combatSlots, supportSlots, 1f);
        UnitManager.instance.formatUnits("Party", "C");
    }

    public void showSupportSlideIn()
    {
        MainAnimationManager.instance.startSupportSlideIn(combatSlots, supportSlots, 1f);
        UnitManager.instance.formatUnits("Party", "S");
    }

    public void loadDropdown()
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

    public void changeDropdownName(string oldName, string newName)
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
        loadParty();
    }

    public void loadParty()
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

    public void updatePartySlotUnit(int slot, PartyUnit unit, string type)
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
            }

            if (type == "C")
            {
                removeUnit(unit.cardid, combatSlots, party.combatUnits);
                addUnit(slot, unit, party.combatUnits);
            }
            else
            {
                removeUnit(unit.cardid, supportSlots, party.supportUnits);
                addUnit(slot, unit, party.supportUnits);
            }

            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine(JsonUtility.ToJson(party));
            }
        }
    }

    public void removeFromParty(string id, string type)
    {
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

                if (type == "C") removeUnit(id, combatSlots, party.combatUnits);
                else removeUnit(id, supportSlots, party.supportUnits);

                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(JsonUtility.ToJson(party));
                }
            }
        }
    }

    private void addUnit(int slot, PartyUnit unit, List<PartyUnit> units)
    {
        int slotIndex = units.FindIndex(s => s.slot == slot);

        if (slotIndex >= 0)
        {
            units[slotIndex] = unit;
        }
        else
        {
            units.Add(unit);
        }
    }

    private void removeUnit(string id, GameObject currentSlots, List<PartyUnit> units)
    {
        int index = units.FindIndex(s => s.cardid == id);

        if (index >= 0)
        {
            Debug.Log("Removing from party...");

            removeSlotImage(currentSlots, units[index].slot);
            units.RemoveAt(index);
        }
    }

    private void resetSlotImage()
    {
        foreach (Transform child in combatSlots.transform.Find("Slots"))
        {
            child.GetComponent<Image>().sprite = emptySlot;
        }

        foreach (Transform child in supportSlots.transform.Find("Slots"))
        {
            child.GetComponent<Image>().sprite = emptySlot;
        }
    }

    private void removeSlotImage(GameObject currentSlots, int slotNum)
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

            changeDropdownName(oldName, party.name);
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
