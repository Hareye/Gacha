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
        public UnitObject[] units;
    }

    [Serializable]
    public class UnitObject
    {
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
}