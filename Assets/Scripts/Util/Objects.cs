using System.Collections.Generic;
using System;

namespace objects
{
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
    public class Characters
    {
        public Character[] characters;
    }

    [Serializable]
    public class Character
    {
        public string skinname;
        public string charactername;
        public string rarity;
    }

    [Serializable]
    public class UnitsObject
    {
        //public UnitObject[] units;
        public List<UnitObject> units;
    }

    [Serializable]
    public class UnitObject
    {
        public string cardid;
        public string skinname;
        public string charactername;
        public int characterlevel;
        public string combattype;
        public string animename;
        public string rarity;
        public string element;
        public string tags;
        public int hp;
        public int atk;
        public int def;
        public int mag;
        public int res;
        public int spd;
    }

    [Serializable]
    public class PartyUnit
    {
        public int slot;
        public string cardid;
        public string skinname;
        public string charactername;
        public int characterlevel;
        public string combattype;
        public string animename;
        public string rarity;
        public string element;
    }

    [Serializable]
    public class Party
    {
        public string name;
        public List<PartyUnit> combatUnits;
        public List<PartyUnit> supportUnits;

        public Party(string n)
        {
            name = n;
        }
    }

    [Serializable]
    public class SerializableList<T>
    {
        public List<T> list;

        public SerializableList()
        {
            list = new List<T>();
        }
    }
}