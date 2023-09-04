using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DeckAllData
{
    public List<SavedCharacterData> savedCharacterData;
    public string deckName;
}

[System.Serializable]
public class DeckCharacterUIData
{
    public UICharacter uICharacter;
    public SelectedUICharacter selectedUICharacter;
    public CharacterData characterData;
    public int count;
}

[System.Serializable]
public class Deck
{
    public Deck(DeckAllData _deckData)
    {
        deckAllData = _deckData;
    }
    public DeckAllData deckAllData;
    
    public string deckName;
    public RegionType regionType;
    public List<DeckCharacterUIData> selectedCards = new List<DeckCharacterUIData>();
    public int GetCharacterCountInDeck(SelectedUICharacter selectedUICharacter)
    {
        foreach (var item in selectedCards)
        {
            
            if(item.characterData == selectedUICharacter.uICharacter.CharacterData)
            {
                return item.selectedUICharacter.count;
            }
        }
        return 0;
    }
    public void AddAnimation(SelectedUICharacter selectedUICharacter)
    {

    }
    public void AddItem(DeckCharacterUIData deckItem)
    {   
        if(deckItem.count == GlobalDeckSettingsSO.Instance.deckSameCardCapacity)
            return;
        
        for (int i = 0; i < selectedCards.Count; i++)
        {
            if(selectedCards[i].characterData == deckItem.characterData)
            {
                selectedCards[i].count ++;
                AddAnimation(deckItem.selectedUICharacter);
                selectedCards[i].selectedUICharacter.IncreaseCount();
                // for (int j = 0; j < deckAllData.savedCharacterData.Count; j++)
                // {
                //     if(deckAllData.savedCharacterData[j] == deckItem.characterData.savedCharacterData)
                //     {
                //         deckAllData.savedCharacterData[j].ownedCount ++;
                //     }
                // }
                deckItem.uICharacter.SetCountText();
                MonoBehaviour.Destroy (deckItem.selectedUICharacter.gameObject);
                return;
            }
        }
        
        deckAllData.savedCharacterData = new List<SavedCharacterData>();
        deckAllData.savedCharacterData.Add(deckItem.characterData.savedCharacterData);
        AddAnimation(deckItem.selectedUICharacter);
        selectedCards.Add(deckItem);
        deckItem.selectedUICharacter.transform.SetParent(DeckManager.Instance.selectedCardsParent);
        deckItem.count ++;
        deckItem.selectedUICharacter.IncreaseCount();
        deckItem.uICharacter.SetCountText();
        
    }
    public void DeclineItem(DeckCharacterUIData deckItem)
    {   

    }
    public void RemoveItem(SelectedUICharacter selectedUICharacter)
    {
        DeckCharacterUIData deckItem = new DeckCharacterUIData();
        foreach (var item in selectedCards)
        {
            if(item.selectedUICharacter == selectedUICharacter)
            {
                deckItem = item;
                break;
            }
        }
        if(deckItem.characterData == null)
        {
            return;
        }
        if(deckItem.count > 1)
        {
            deckItem.count --;
            deckItem.selectedUICharacter.DecreaseCount();
            deckItem.selectedUICharacter.SetCountText();
        }
        else
        {            
            deckItem.count --;
            deckItem.selectedUICharacter.DecreaseCount();
            deckItem.selectedUICharacter.SetCountText();
            selectedCards.Remove(deckItem);
            MonoBehaviour.Destroy(deckItem.selectedUICharacter.gameObject);
        }
    }
    public void RemoveItem(DeckCharacterUIData deckItem)
    {
        if(selectedCards.Contains(deckItem))
        {
            selectedCards.Remove(deckItem);
        }
    }

    
}

