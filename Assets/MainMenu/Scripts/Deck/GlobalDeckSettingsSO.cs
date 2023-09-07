using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName ="Deck/DeckSettings")]
public class GlobalDeckSettingsSO : ScriptableObjectSinleton<GlobalDeckSettingsSO>
{
    private const string TITLE_DATA_KEY ="CardData";
    [SerializeField] private GameObject uICharacterPrefab,deckPrefab,selectedUICharacterPrefab;
    public List<CharacterData> allCharacters;
    public List<DeckAllData> allDecks;
    public int maxDeckCount = 10;
    public int deckCardCapacity = 40;
    public int deckHeroCapacity = 6;
    public int deckSameCardCapacity = 3;
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
        for (int i = 0; i < allCharacters.Count; i++)
        {
            Debug.Log("createUICharacter");
            var obj = CreateUICharacter(allCharacters[i],parent);
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
            deckData.Add (CreateUIDeck(allDecks[i],parent));

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
        foreach (var item in allCharacters)
        {
            if(item.savedCharacterData.characterID == ID)
            {
                return item;
            }
        }
        return null;
    }

    // public List<>
    




    public void SaveAppearance()
    {
        List<SavedCharacterData> savedCharacterData = new List<SavedCharacterData>();
        foreach (var item in allCharacters)
        {
            savedCharacterData.Add(item.savedCharacterData);
        }

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {TITLE_DATA_KEY,JsonConvert.SerializeObject(savedCharacterData)}
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
            List<SavedCharacterData> deckDatas = JsonConvert.DeserializeObject<List<SavedCharacterData>>(result.Data[TITLE_DATA_KEY].Value);
                // allCharacters.Clear();
            for (int i = 0; i < deckDatas.Count; i++)
            {
                allCharacters[i].savedCharacterData = deckDatas[i];
            }
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


    public void SaveCards()
    {
       
        // allCharacters.Add(selectedDeck);
        // selectedDeck.deckData = new DeckData{characterDataId = }
        
        SaveAppearance();
    }
}
