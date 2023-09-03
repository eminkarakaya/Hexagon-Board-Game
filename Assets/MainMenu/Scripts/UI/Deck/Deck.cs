using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckItem
{
    public UICharacter uICharacter;
    public SelectedUICharacter selectedUICharacter;
    public CharacterData characterData;
    public int count;
}
public class Decks 
{
    public List<Deck> allDecks = new List<Deck>();
}
[System.Serializable]
public class Deck
{
    public string deckName;
    public RegionType regionType;
    public List<DeckItem> selectedCards = new List<DeckItem>();
    public int GetCharacterCountInDeck(SelectedUICharacter selectedUICharacter)
    {
        foreach (var item in selectedCards)
        {
            // Debug.Log(item );
            // Debug.Log(item.characterData);
            // Debug.Log(selectedUICharacter);
            // Debug.Log(selectedUICharacter.uICharacter);
            // Debug.Log(selectedUICharacter.uICharacter.CharacterData);
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
    public void AddItem(DeckItem deckItem)
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
    public void DeclineItem(DeckItem deckItem)
    {   

    }
    public void RemoveItem(SelectedUICharacter selectedUICharacter)
    {
        DeckItem deckItem = new DeckItem();
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
    public void RemoveItem(DeckItem deckItem)
    {
        if(selectedCards.Contains(deckItem))
        {
            selectedCards.Remove(deckItem);
        }
    }
}

