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
    public bool deleteDecks;
    [SerializeField] private TMP_InputField deckNameInputField;
    private const string TITLE_DATA_KEY ="DeckData";
    [SerializeField] private GameObject displayUICharacter;
    [SerializeField] private Transform displayUIParent,deckParent,playParent;
    [Header("RightClickCanvas")]
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text rightClickCountText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image ppImage;
    [SerializeField] private GameObject rightClickPanel;
    private UICharacterDisplay display;

    public UICharacter currentRightClickedUICharacter;
    public List<UIDeck> allDecks = new List<UIDeck>();
    public TMP_Dropdown dropdownDeckCards,dropdownCollectionCards;
    public List<UICharacter> allCharacters;
    public Transform editCardsParent,editCardsParentTemp;
    public Transform collectionCardsParent,collectionTempCardsParent;
    public Transform selectedCardsParent;
    public Deck selectedDeck;
    bool isNew = false;
    public void NewDeck()
    {
        selectedDeck = new Deck(new DeckAllData());
        isNew = true;
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
        allCharacters = GlobalDeckSettingsSO.Instance.CreateAllCharacters(editCardsParent,true);
        
        GlobalDeckSettingsSO.Instance.CreateAllCharacters(collectionCardsParent,false);
    }
    public void ListAllCharactersCollectionCards()
    {
        foreach (var item in allCharacters)
        {
            item.transform.SetParent(collectionCardsParent);   
        }
    }
    public void ListOwnedCharactersCollectionCards()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.savedCharacterData.ownedCount == 0)
            {
                item.transform.SetParent(collectionTempCardsParent);
            }
            else
            {
                item.transform.SetParent(collectionCardsParent);
            }
        }
    }
    public void ListIncomplateCharactersCollectionCards()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.savedCharacterData.ownedCount == 0)
            {
                item.transform.SetParent(collectionCardsParent);
            }
            else
            {
                item.transform.SetParent(collectionTempCardsParent);
            }
        }
    }
    public void ListAllCharacters()
    {
        foreach (var item in allCharacters)
        {
            item.transform.SetParent(editCardsParent);   
        }
    }
    public void ListOwnedCharacters()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.savedCharacterData.ownedCount == 0)
            {
                item.transform.SetParent(editCardsParentTemp);
            }
            else
            {
                item.transform.SetParent(editCardsParent);
            }
        }
    }
    public void ListIncomplateCharacters()
    {
        foreach (var item in allCharacters)
        {
            if(item.CharacterData.savedCharacterData.ownedCount == 0)
            {
                item.transform.SetParent(editCardsParent);
            }
            else
            {
                item.transform.SetParent(editCardsParentTemp);
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
        rightClickPanel.SetActive(false);
        Destroy(display.gameObject);
        GlobalDeckSettingsSO.Instance.SaveCards();
    }
    public void OpenRightclickCanvas()
    {
        rightClickPanel.SetActive(true);
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
                Debug.Log(item.deckAllData.deckName);
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
        Debug.Log("Successfull Data send");
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
        Debug.Log("Recieved user data");
        if(result.Data != null && result.Data.ContainsKey(TITLE_DATA_KEY))
        {
            List<DeckAllData> deckDatas = JsonConvert.DeserializeObject<List<DeckAllData>>(result.Data[TITLE_DATA_KEY].Value);
                allDecks.Clear();
            for (int i = 0; i < deckDatas.Count; i++)
            {
                allDecks.Add(GlobalDeckSettingsSO.Instance.CreateUIDeck(deckDatas[i],deckParent));
            }
            GlobalDeckSettingsSO.Instance.allDecks = allDecks.Select(x=>x.deckAllData).ToList();
            // CreateDeckUI();
        }
        else
        {
            SaveAppearance();
            GetAppearance();
            Debug.Log("NotData");
        }
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/create account");
        Debug.Log(error.GenerateErrorReport());
    }


    public void SaveDeck()
    {
        selectedDeck.deckName = deckNameInputField.text;
        if(isNew)
        {
            var obj = GlobalDeckSettingsSO.Instance.CreateUIDeck(new DeckAllData{savedCharacterData = selectedDeck.selectedCards.Select(x=>x.characterData.savedCharacterData).ToList(),deckName = selectedDeck.deckName},deckParent);
            // Debug.Log(obj.deckAllData.savedCharacterData.Count + " obj.deckAllData.savedCharacterData.Count");
            GlobalDeckSettingsSO.Instance.CreateUIDeck(new DeckAllData{savedCharacterData = selectedDeck.selectedCards.Select(x=>x.characterData.savedCharacterData).ToList(),deckName = selectedDeck.deckName},playParent);
            allDecks.Add(obj);            
        }        
        SaveAppearance();
        isNew = false;
    }
    #endregion
}
