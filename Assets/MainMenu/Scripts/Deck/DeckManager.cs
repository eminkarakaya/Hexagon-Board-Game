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
    private const string TITLE_CARDDATA_KEY ="CardData";
    private const string TITLE_DATA_KEY ="DeckData";
    [Header("Prefabs")]
    
    [SerializeField] private GameObject uICharacterPrefab;
    [SerializeField] private GameObject deckPrefab,selectedUICharacterPrefab;
    
    [Header("Panels")]
    [SerializeField] private Canvas editDeckPanel;

    [SerializeField] private GameObject SaveOrDiscardPanel;
    /// <summary>
    /// sadece olustururken kullanıyorum. canvası kapalı bı objenın altında olusturunca 
    /// scale sı 0 oluyo bazen ondan burda olusturuyorum.
    /// </summary>

    /// <summary>
    /// test için eger acıksa ve save ypaarsam save ler sılınır.
    /// </summary>
    public bool deleteDecks;
    [Header("UI")]

    [SerializeField] private TMP_InputField deckNameInputField;
    [SerializeField] private GameObject displayUICharacter;
    [SerializeField] private Transform displayUIParent,deckParent,playParent;
    public TMP_Dropdown dropdownDeckCards,dropdownCollectionCards;

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

    [Header("Character And Deck Data")]
    [Space(10)]
    public List<CharacterData> allCharactersData;
    public List<DeckAllData> allDecksData;

    public List<UIDeck> allDecks = new List<UIDeck>();
    public List<UICharacter> allCharacters;
    [Header("Parents")]
    public Transform createdParent; 
    public Transform editCardsParent,editCardsParentTemp;
    public Transform collectionCardsParent,collectionTempCardsParent;
    public Transform selectedCardsParent;
    [Space(20)]
    public Deck selectedDeck;
    public UIDeck selectedUIDeck;
    public bool isNew = false;
    private void Start() {
        GetAppearance();
        GetAppearanceCharacterData();
        allCharacters = CreateAllCharacters(createdParent,true);
    }
    #region CreateUI
    public UICharacter CreateUICharacter(CharacterData characterData, Transform parent)
    {   
        var obj = Instantiate(uICharacterPrefab,parent);
        UICharacter uICharacter = obj.GetComponent<UICharacter>();
        uICharacter.SetCharacterData(characterData);
        uICharacter.Initialize();
        // uICharacter.SetCountText();
        return uICharacter;
    }   
    
    public List<UICharacter> CreateAllCharacters(Transform parent,bool moveable)
    {
        List<UICharacter> characterDatas = new List<UICharacter>();
        for (int i = 0; i < allCharactersData.Count; i++)
        {
            var obj = CreateUICharacter(allCharactersData[i],parent);
            obj.moveable = moveable;
            characterDatas.Add (obj);
        }
        return characterDatas;
    } 
    
    public UIDeck CreateUIDeck(DeckAllData deckAllData, Transform parent)
    {   
        var obj = Instantiate(deckPrefab,parent);
        UIDeck uIDeck = obj.GetComponent<UIDeck>();
        
        uIDeck.SetDeckData(deckAllData);
        
        return uIDeck;
    }  

    public List<UIDeck> CreateAllDecks(Transform parent)
    {
        List<UIDeck> deckData = new List<UIDeck>();
        for (int i = 0; i < allDecks.Count; i++)
        {
            deckData.Add (CreateUIDeck(allDecksData[i],parent));

        }
        return deckData;
    }

    public DeckCharacterUIData CreateSelectedUICharacter(SavedCharacterData savedCharacterData,Transform parent)
    {
        CharacterData characterData = GetCharacterData(savedCharacterData.characterID);
        SelectedUICharacter selectedUICharacter = Instantiate(selectedUICharacterPrefab,parent).GetComponent<SelectedUICharacter>();
        UICharacter uICharacter = DeckManager.Instance.GetUICharacter(characterData);
        selectedUICharacter.uICharacter = uICharacter;
        selectedUICharacter.SetData();
        selectedUICharacter.count = savedCharacterData.ownedCount;
        DeckCharacterUIData deckCharacterUIData = new DeckCharacterUIData{characterData = characterData,selectedUICharacter = selectedUICharacter,uICharacter = uICharacter,count = savedCharacterData.ownedCount};
        // uICharacter.SetCountText();
        
        return deckCharacterUIData;
    }
    public List<DeckCharacterUIData> CreateAllSelectedUICharacter(List<SavedCharacterData> savedCharacterData,Transform parent)
    {
        List<DeckCharacterUIData> deckCharacterUIData = new List<DeckCharacterUIData>();
        for (int i = 0; i < savedCharacterData.Count; i++)
        {
            deckCharacterUIData.Add(CreateSelectedUICharacter(savedCharacterData[i],parent));
        }
        

        
        return deckCharacterUIData;
    }
    public CharacterData GetCharacterData(int ID)
    {
        foreach (var item in allCharactersData)
        {
            if(item.savedCharacterData.characterID == ID)
            {
                return item;
            }
        }
        return null;
    }
    #endregion
    
    public void DoneButton()
    {
        if(deckNameInputField.text == string.Empty)
        {
            AlertManager.Instance.ShowAlert("Deste Adı Giriniz");
        }
        else
        {
            SaveOrDiscardPanel.SetActive(true);
        }
    }
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
    
    #region TranslateUI

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
    #endregion
   
    #region  BuyCanvas
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
        SaveCards();
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
    #endregion
    
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
    

    public void OpenPlayPlayersPanel()
    {
        TranslateAllDecks(playParent,true);
    }
    public void OpenCollectionDeckPanel()
    {
        TranslateAllDecks(deckParent,false);
    }

    #region  Playfab
    public void SaveAppearanceCharacterData()
    {
        List<SavedCharacterData> savedCharacterData = new List<SavedCharacterData>();
        foreach (var item in allCharactersData)
        {
            savedCharacterData.Add(item.savedCharacterData);
        }

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {TITLE_CARDDATA_KEY,JsonConvert.SerializeObject(savedCharacterData)}
            }
        };
        
        PlayFabClientAPI.UpdateUserData(request,OnDataSendCharacterData,OnErrorCharacterData);
        
    }
    void OnDataSendCharacterData(UpdateUserDataResult result)
    {
    }
    public void GetDeckDataCharacterData()
    {

    }
    public void GetAppearanceCharacterData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest{ },OnDataRecivedCharacterData,OnErrorCharacterData);
    }
    void OnDataRecivedCharacterData(GetUserDataResult result)
    {
        if(result.Data != null && result.Data.ContainsKey(TITLE_CARDDATA_KEY))
        {
            List<SavedCharacterData> deckDatas = JsonConvert.DeserializeObject<List<SavedCharacterData>>(result.Data[TITLE_CARDDATA_KEY].Value);
                // allCharacters.Clear();
            for (int i = 0; i < deckDatas.Count; i++)
            {
                allCharactersData[i].savedCharacterData = deckDatas[i];
            }
        }
        else
        {
            SaveAppearanceCharacterData();
            GetAppearanceCharacterData();
        }
    }
    void OnErrorCharacterData(PlayFabError error)
    {
    }


    public void SaveCards()
    {
       
        // allCharacters.Add(selectedDeck);
        // selectedDeck.deckData = new DeckData{characterDataId = }
        
        SaveAppearanceCharacterData();
    }

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
        allDecksData = allDecks.Select(x=>x.deckAllData).ToList();
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
                UIDeck uIDeck = CreateUIDeck(deckDatas[i],deckParent);
                allDecks.Add(uIDeck);
            }
            allDecksData = allDecks.Select(x=>x.deckAllData).ToList();
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
            
            var obj = CreateUIDeck(deckAllData,createdParent);
            CreateUIDeck(deckAllData,createdParent);
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
