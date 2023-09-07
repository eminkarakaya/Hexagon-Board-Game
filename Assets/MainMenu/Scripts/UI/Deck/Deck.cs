using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Deck()
    {
        
    }
    public Deck(List<DeckCharacterUIData> selectedCards)
    {
        this.selectedCards = selectedCards;
    }
    public Deck(List<SavedCharacterData> savedCharacterData)
    {
        selectedCards = GlobalDeckSettingsSO.Instance.CreateAllSelectedUICharacter(savedCharacterData,DeckManager.Instance.selectedCardsParent);
        foreach (var item in selectedCards)
        {
            item.selectedUICharacter.SetCountText();
        }
        // deck datalar atanıcak
        
    }
    public string deckName;
    
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
                deckItem.uICharacter.SetCountText();
                MonoBehaviour.Destroy (deckItem.selectedUICharacter.gameObject);
                return;
            }
        }
        
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
            deckItem.selectedUICharacter.SetCountTextUICharacter();
        }
        else
        {            
            deckItem.count --;
            deckItem.selectedUICharacter.DecreaseCount();
            deckItem.selectedUICharacter.SetCountTextUICharacter();
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
    public SelectedUICharacter GetSelectedUICharacter(CharacterData characterData)
    {
        foreach (var item in selectedCards)
        {
            if(item.characterData == characterData)
            {
                return item.selectedUICharacter;
            }
        }
        return null;
    }
    
}

