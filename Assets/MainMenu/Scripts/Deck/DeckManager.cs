using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System.Linq;

public class DeckManager : Singleton<DeckManager>
{   
    /// <summary>
    /// sadece olustururken kullanıyorum. canvası kapalı bı objenın altında olusturunca 
    /// scale sı 0 oluyo bazen ondan burda olusturuyorum.
    /// </summary>
    public Transform createdParent; 

    /// <summary>
    /// test için eger acıksa ve save ypaarsam save ler sılınır.
    /// </summary>
    public bool deleteDecks;
    [SerializeField] private TMP_InputField deckNameInputField;
    private const string TITLE_DATA_KEY ="DeckData";
    [SerializeField] private GameObject displayUICharacter;
    [SerializeField] private Transform displayUIParent,deckParent,playParent;

    /// <summary>
    /// buy canvası ve display UIChaaacter bulunur.
    /// </summary>
    [Header("RightClickCanvas")]
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text rightClickCountText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image ppImage;
    [SerializeField] private Canvas rightClickPanel;
    private UICharacterDisplay display;

    public UICharacter currentRightClickedUICharacter;

    [Space(10)]
    public List<UIDeck> allDecks = new List<UIDeck>();
    public TMP_Dropdown dropdownDeckCards,dropdownCollectionCards;
    public List<UICharacter> allCharacters;
    public Transform editCardsParent,editCardsParentTemp;
    public Transform collectionCardsParent,collectionTempCardsParent;
    public Transform selectedCardsParent;
    public Deck selectedDeck;
    public UIDeck selectedUIDeck;
    [SerializeField] private Canvas editDeckPanel;
    public bool isNew = false;
    public void NewDeck()
    {
        selectedDeck = new Deck();
        isNew = true;
        TranslateAllCharacters(editCardsParent,true);
    }
    public void SetDataUICharacters()
    {
        foreach (var item in allCharacters)
        {
            item.SetCountText();
        }
    }
    public UICharacter GetUICharacter(CharacterData characterData)
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData == characterData)
            {
                return item;
            }
        }
        return null;
    }
    public void DeckAddItem(DeckCharacterUIData deckItem)
    {
        selectedDeck.AddItem(deckItem);
    }
    public void DeckRemoveItem(SelectedUICharacter selectedUICharacter)
    {
        selectedDeck.RemoveItem(selectedUICharacter);
    }
    private void Start() {
        GetAppearance();
        GlobalDeckSettingsSO.Instance.GetAppearance();
        allCharacters = GlobalDeckSettingsSO.Instance.CreateAllCharacters(createdParent,true);
    }
    public void ListAllCharactersCollectionCards()
    {
        TranslateAllCharacters(collectionCardsParent,false);
        // foreach (var item in allCharacters)
        // {
        //     item.transform.SetParent(collectionCardsParent);   
        // }
    }
    public void ListOwnedCharactersCollectionCards()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.savedCharacterData.ownedCount == 0)
            {
                TranslateCharacter(collectionTempCardsParent,false,item);
                // item.transform.SetParent(collectionTempCardsParent);
                
            }
            else
            {
                TranslateCharacter(collectionCardsParent,false,item);
                // item.transform.SetParent(collectionCardsParent);
            }
        }
    }
    public void ListIncomplateCharactersCollectionCards()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.savedCharacterData.ownedCount == 0)
            {
                TranslateCharacter(collectionCardsParent,false,item);
                // item.transform.SetParent(collectionCardsParent);
            }
            else
            {
                TranslateCharacter(collectionTempCardsParent,false,item);
                // item.transform.SetParent(collectionTempCardsParent);
            }
        }
    }
    public void ListAllCharacters()
    {
        TranslateAllCharacters(editCardsParent,true);
        // foreach (var item in allCharacters)
        // {
        //     item.transform.SetParent(editCardsParent);   
        // }
    }
    public void ListOwnedCharacters()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.savedCharacterData.ownedCount == 0)
            {
                TranslateCharacter(editCardsParentTemp,true,item);
                // item.transform.SetParent(editCardsParentTemp);
            }
            else
            {
                TranslateCharacter(editCardsParent,true,item);
                // item.transform.SetParent(editCardsParent);
            }
        }
    }
    public void ListIncomplateCharacters()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.savedCharacterData.ownedCount == 0)
            {
                TranslateCharacter(editCardsParent,true,item);
                // item.transform.SetParent(editCardsParent);
            }
            else
            {
                TranslateCharacter(editCardsParentTemp,true,item);
                // item.transform.SetParent(editCardsParentTemp);
            }
        }
    }
    // 0 = allcards  , 1 = owned , 2 = incomplate  
    public void OnToggleChangedCollection()
    {
        if(dropdownCollectionCards.value == 0)
        {
            ListAllCharactersCollectionCards();
        }
        else if(dropdownCollectionCards.value == 1)
        {
            ListOwnedCharactersCollectionCards();
        }
        else if(dropdownCollectionCards.value == 2)
        {
            ListIncomplateCharactersCollectionCards();
        }
    }
    public void OnToggleChanged()
    {
        if(dropdownDeckCards.value == 0)
        {
            ListAllCharacters();
        }
        else if(dropdownDeckCards.value == 1)
        {
            ListOwnedCharacters();
        }
        else if(dropdownDeckCards.value == 2)
        {
            ListIncomplateCharacters();
        }
    }
    public void Buy()
    {
        currentRightClickedUICharacter.Buy();
        RightClickCanvasSetDataDisplay(display);
    }
    public void BuyButton()
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(()=> Buy());
        buyButton.onClick.AddListener(()=> RightClickCanvasSetData());
    }
    public void CloseRightclickCanvas()
    {
        rightClickPanel.enabled = false;
        Destroy(display.gameObject);
        GlobalDeckSettingsSO.Instance.SaveCards();
    }
    public void OpenRightclickCanvas()
    {
        rightClickPanel.enabled = true;
        display = CreateUICharacterDisplay();
        RightClickCanvasSetData();
        RightClickCanvasSetDataDisplay(display);
    }
    public void RightClickCanvasSetDataDisplay(UICharacterDisplay display)
    {
        display.SetCountText();
    }
    public void RightClickCanvasSetData()
    {
        SetRightClickImage(currentRightClickedUICharacter.CharacterData.fullSprite);
        SetRightClickCountText(currentRightClickedUICharacter.CharacterData);
        SetRightClickCostText(currentRightClickedUICharacter.CharacterData);
        // currentRightClickedUICharacter.SetCountText();
    }
    public void SetRightClickImage(Sprite sprite)
    {
        ppImage.sprite = sprite;
    }
    public void SetRightClickCountText(CharacterData characterData)
    {
        rightClickCountText.text = characterData.savedCharacterData.ownedCount +  "/" +characterData.capacity;
    }
    public void SetRightClickCostText(CharacterData characterData)
    {
        costText.text = characterData.cost.ToString();
    }
    public UICharacterDisplay CreateUICharacterDisplay()
    {
        var obj = Instantiate(displayUICharacter,displayUIParent);
        UICharacterDisplay display = obj.GetComponent<UICharacterDisplay>();
        display.CharacterData = currentRightClickedUICharacter.CharacterData;
        return display;
    }
    public void OpenEditDeckPanel()
    {
        editDeckPanel.enabled = true;
        deckNameInputField.text = selectedDeck.deckName;
        TranslateAllCharacters(editCardsParent,true);
    }
    public void CloseEditDeckPanel()
    {
        editDeckPanel.enabled = false;
    }

    public void DiscardDeck()
    {
        for (int i = 0; i < selectedDeck.selectedCards.Count; i++)
        {
            Destroy (selectedDeck.selectedCards[i].selectedUICharacter.gameObject);
        }
        selectedDeck = null;
        CloseEditDeckPanel();
    }

    public void OpenCardPanel()
    {
        OnToggleChangedCollection();
        // TranslateAllCharacters(collectionCardsParent,false);
    }
    public void TranslateCharacter(Transform parent,bool moveable,UICharacter uICharacter)
    {
        uICharacter.transform.SetParent(parent);
        uICharacter.moveable = moveable;
        uICharacter.SetCountText();
        uICharacter.transform.localScale = Vector3.one;
    }
    public void TranslateAllCharacters(Transform parent,bool moveable)
    {
        foreach (var item in allCharacters)
        {
            TranslateCharacter(parent,moveable,item);
        }
    }
    public void TranslateAllDecks(Transform parent,bool isplay)
    {
        foreach (var item in allDecks)
        {
            item.transform.SetParent(parent);
            item.isPlay = isplay;
            item.transform.localScale = Vector3.one;
            if (selectedUIDeck != null)
            {
                selectedUIDeck.DeselectEvent();
                selectedDeck = null;
            }
        }
    }

    public void OpenPlayPlayersPanel()
    {
        TranslateAllDecks(playParent,true);
    }
    public void OpenCollectionDeckPanel()
    {
        TranslateAllDecks(deckParent,false);
    }

    #region  Playfab


    [ContextMenu("Save")]
    public void SaveAppearance()
    {
        List<DeckAllData> deckDatas = new List<DeckAllData>();
        // deck.selectedcards
        if(!deleteDecks)
        {
            foreach (var item in allDecks)
            {
                DeckAllData deckAllData = new DeckAllData{savedCharacterData = new List<SavedCharacterData>()};
                deckAllData.savedCharacterData = item.deckAllData.savedCharacterData;
                deckAllData.deckName = item.deckAllData.deckName;
                deckDatas.Add(deckAllData);
                
            }
        }
        GlobalDeckSettingsSO.Instance.allDecks = allDecks.Select(x=>x.deckAllData).ToList();
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {TITLE_DATA_KEY,JsonConvert.SerializeObject(deckDatas)}
            }
        };
        
        PlayFabClientAPI.UpdateUserData(request,OnDataSend,OnError);
        
    }
    void OnDataSend(UpdateUserDataResult result)
    {
    }
    public void GetDeckData()
    {

    }
    public void GetAppearance()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest{ },OnDataRecived,OnError);
    }
    void OnDataRecived(GetUserDataResult result)
    {
        if(result.Data != null && result.Data.ContainsKey(TITLE_DATA_KEY))
        {
            List<DeckAllData> deckDatas = JsonConvert.DeserializeObject<List<DeckAllData>>(result.Data[TITLE_DATA_KEY].Value);
                allDecks.Clear();
            for (int i = 0; i < deckDatas.Count; i++)
            {
                UIDeck uIDeck = GlobalDeckSettingsSO.Instance.CreateUIDeck(deckDatas[i],deckParent);
                allDecks.Add(uIDeck);
            }
            GlobalDeckSettingsSO.Instance.allDecks = allDecks.Select(x=>x.deckAllData).ToList();
            // CreateDeckUI();
        }
        else
        {
            SaveAppearance();
            GetAppearance();
        }
    }
    void OnError(PlayFabError error)
    {
    }


    public void SaveDeck()
    {
        selectedDeck.deckName = deckNameInputField.text;
        if(isNew)
        {
            DeckAllData deckAllData = new DeckAllData();
            deckAllData.savedCharacterData = new List<SavedCharacterData>();
            for (int i = 0; i < selectedDeck.selectedCards.Count; i++)
            {
                deckAllData.savedCharacterData.Add(new SavedCharacterData{characterID = selectedDeck.selectedCards[i].characterData.savedCharacterData.characterID,ownedCount = selectedDeck.selectedCards[i].count});
            }
            deckAllData.deckName = selectedDeck.deckName;
            
            var obj = GlobalDeckSettingsSO.Instance.CreateUIDeck(deckAllData,createdParent);
            GlobalDeckSettingsSO.Instance.CreateUIDeck(deckAllData,createdParent);
            allDecks.Add(obj);            
        }       
        else
        {
            DeckAllData deckAllData = new DeckAllData();
            deckAllData.savedCharacterData = new List<SavedCharacterData>();
            for (int i = 0; i < selectedDeck.selectedCards.Count; i++)
            {
                deckAllData.savedCharacterData.Add(new SavedCharacterData{characterID = selectedDeck.selectedCards[i].characterData.savedCharacterData.characterID,ownedCount = selectedDeck.selectedCards[i].count});
            }
            deckAllData.deckName = selectedDeck.deckName;
            selectedUIDeck.SetDeckData(deckAllData);
        } 
        SaveAppearance();
        isNew = false;
        CloseEditDeckPanel();
        for (int i = 0; i < selectedDeck.selectedCards.Count; i++)
        {
            Destroy (selectedDeck.selectedCards[i].selectedUICharacter.gameObject);
        }
        selectedDeck = null;
    }
    

    #endregion
}
