using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using objects;

public class PartySlot : MonoBehaviour, IDropHandler
{
    private int slot;
    private string type;

    void Start()
    {
        slot = int.Parse(transform.name.Split(' ')[1]);
        type = transform.parent.parent.name.Split(' ')[0][0].ToString();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            GetComponent<Image>().sprite = eventData.pointerDrag.GetComponent<Image>().sprite;

            PartyManager.instance.updatePartyMember(slot, setUpNewUnit(eventData.pointerDrag.GetComponent<Unit>().unit), type);
        }
    }

    private PartyUnit setUpNewUnit(UnitObject unit)
    {
        PartyUnit newUnit = new PartyUnit();
        newUnit.slot = slot;
        newUnit.cardid = unit.cardid;
        newUnit.skinname = unit.skinname;
        newUnit.charactername = unit.charactername;
        newUnit.characterlevel = unit.characterlevel;
        newUnit.combattype = unit.combattype;
        newUnit.animename = unit.animename;
        newUnit.rarity = unit.rarity;
        newUnit.element = unit.element;

        return newUnit;
    }
}
