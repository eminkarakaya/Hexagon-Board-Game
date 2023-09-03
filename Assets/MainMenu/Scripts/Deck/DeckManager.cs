using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckManager : Singleton<DeckManager>
{


    [Header("RightClickCanvas")]
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text rightClickCountText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image ppImage;
    [SerializeField] private GameObject rightClickPanel;

    public UICharacter currentRightClickedUICharacter;

    public TMP_Dropdown dropdown;
    public List<UICharacter> allCharacters;
    public Transform cardsParent,tempCardsParent;
    public Transform selectedCardsParent;
    public Deck selectedDeck;
    public void NewDeck()
    {
        selectedDeck = new Deck();
    }
    public void DeckAddItem(DeckItem deckItem)
    {
        selectedDeck.AddItem(deckItem);
    }
    public void DeckRemoveItem(SelectedUICharacter selectedUICharacter)
    {
        selectedDeck.RemoveItem(selectedUICharacter);
    }
    public void ResetAllDeck()
    {
        allCharacters = GlobalDeckSettingsSO.Instance.CreateAllCharacters(cardsParent);
    }
    private void Start() {
        ResetAllDeck();
    }
    public void ListAllCharacters()
    {
        foreach (var item in allCharacters)
        {
            item.transform.SetParent(cardsParent);   
        }
    }
    public void ListOwnedCharacters()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.ownedCount == 0)
            {
                item.transform.SetParent(tempCardsParent);
            }
            else
            {
                item.transform.SetParent(cardsParent);
            }
        }
    }
    public void ListIncomplateCharacters()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.ownedCount == 0)
            {
                item.transform.SetParent(cardsParent);
            }
            else
            {
                item.transform.SetParent(tempCardsParent);
            }
        }
    }
    // 0 = allcards  , 1 = owned , 2 = incomplate  
    public void OnToggleChanged()
    {
        if(dropdown.value == 0)
        {
            ListAllCharacters();
        }
        else if(dropdown.value == 1)
        {
            ListOwnedCharacters();
        }
        else if(dropdown.value == 2)
        {
            ListIncomplateCharacters();
        }
    }
    public void BuyButton()
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(()=> currentRightClickedUICharacter.Buy());
        buyButton.onClick.AddListener(()=> RightClickCanvasSetData());
    }
    public void OpenRightclickCanvas()
    {
        rightClickPanel.SetActive(true);
        
        RightClickCanvasSetData();
    }
    public void RightClickCanvasSetData()
    {
        SetRightClickImage(currentRightClickedUICharacter.CharacterData.fullSprite);
        SetRightClickCountText(currentRightClickedUICharacter.CharacterData);
        SetRightClickCostText(currentRightClickedUICharacter.CharacterData);
        currentRightClickedUICharacter.SetCountText();
    }
    public void SetRightClickImage(Sprite sprite)
    {
        ppImage.sprite = sprite;
    }
    public void SetRightClickCountText(CharacterData characterData)
    {
        rightClickCountText.text = characterData.ownedCount +  "/" +characterData.capacity;
    }
    public void SetRightClickCostText(CharacterData characterData)
    {
        costText.text = characterData.cost.ToString();
    }
}
