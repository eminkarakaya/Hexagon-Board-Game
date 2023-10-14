using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
public enum PropertiesEnum
{
    Default,
    TakeHostage,
    Assassin,
    
}

[System.Serializable]
public struct PropertiesStruct
{
    public PropertiesEnum attackPropertiesEnum;
}

[System.Serializable]
public class SavedCharacterData
{
    public int characterID;
    public int ownedCount;
    
}


[CreateAssetMenu(menuName ="Character/CharacterData")]
public class CharacterData : ScriptableObject 
{
    
    public SavedCharacterData savedCharacterData = new SavedCharacterData();
    public int cost;
    public int capacity;
    public string charName;
    public byte level;
    public List<PropertiesStruct> Properties;
    public List<RegionType> Regions;
    public Sprite fullSprite,selectedSprite;

    public void Buy()
    {

    }




    
}
