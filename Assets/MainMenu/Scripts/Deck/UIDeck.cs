using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDeck : MonoBehaviour, IPointerDownHandler , IPointerUpHandler 
{
    
    public bool isPlay;
    [Header("UI")] 
    [SerializeField] private GameObject frame;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text deckName;

    // [SerializeField] private Transform 
    public DeckAllData deckAllData;

    public void SetDeckData(DeckAllData _deckData)
    {
        deckAllData = _deckData;
        deckName.text = deckAllData.deckName;
    }
    private void Start() {
        deckName.text = deckAllData.deckName;
    }
    public void SelectEvent()
    {
        frame.SetActive(true);
    }
    public void DeselectEvent()
    {
        frame.SetActive(false);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(DeckManager.Instance.selectedUIDeck != null)
        {
            DeckManager.Instance.selectedUIDeck.DeselectEvent();
        }
        DeckManager.Instance.selectedDeck = new Deck(deckAllData.savedCharacterData);
        Deck deck = DeckManager.Instance.selectedDeck;
        deck.deckName = deckAllData.deckName;
        if(!isPlay)
        {
            DeckManager.Instance.OpenEditDeckPanel();
            DeckManager.Instance.SetDataUICharacters();
            for (int i = 0; i < deck.selectedCards.Count; i++)
            {
                deck.selectedCards[i].selectedUICharacter.count = deckAllData.savedCharacterData[i].ownedCount;
                deck.selectedCards[i].selectedUICharacter.SetCountTextUICharacter();
                deck.selectedCards[i].selectedUICharacter.Initialize();
            }
        }
        else
        {
            
            SelectEvent();

        }
        DeckManager.Instance.selectedUIDeck = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
