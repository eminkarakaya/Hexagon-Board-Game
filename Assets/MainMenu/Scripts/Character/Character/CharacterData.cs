using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PropertiesEnum
{
    MoveKill,
    TakeHostage,
    Assassin,
    
}

public enum PropertiesType
{
    Attack,
    Move,
    Death
}
[System.Serializable]
public struct PropertiesStruct
{
    public PropertiesEnum attackPropertiesEnum;
    public PropertiesType propertiesType;
}
[CreateAssetMenu(menuName ="Character/CharacterData")]
public class CharacterData : ScriptableObject 
{
    public int cost;
    public int ownedCount;
    public int capacity;
    public string charName;
    public byte level;
    public List<PropertiesStruct> Properties;
    public List<RegionType> Regions;
    public Sprite fullSprite,selectedSprite;
}
