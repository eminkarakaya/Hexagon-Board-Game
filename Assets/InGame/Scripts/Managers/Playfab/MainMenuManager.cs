using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

[System.Serializable]
public class Data
{
    public int _level;
    public int _gold = 900;
    public int _health = 5;
    public int _gem = 10;
    public Data(int gold,int energy,int gem,int level)
    {
        this.SetLevel(level);
        this.SetGold(gold);
        this.SetHealth(energy);
        this.SetGem(gem);
    }
    #region GETSET
    public int GetGem()
    {
        return _gem;
    }
    public void SetGem(int value)
    {
        _gem = value;
    }
    public int GetLevel()
    {
        return _level;
    }
    public void SetLevel(int value)
    {
        _level = value;
    }
    public int GetGold()
    {
        return _gold;
    }
    public void SetGold(int value)
    {
        _gold = value;
        
    }
    public int GetHealth()
    {
        return _health;
    }
    public void SetHealth(int value)
    {
        _health = value;
        
    }
    #endregion
    public Data ReturnData()
    {
        return new Data(GetGold(),GetHealth(),GetGem(),GetLevel());
    }
}
public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    [SerializeField] private List<GameObject> UIandCameraEtc;
    int levelOffset = 1;
    public Data data;
    [SerializeField] private const string TITLE_DATA_KEY ="PlayerData";
    [SerializeField] private TextMeshProUGUI _goldText,_healthText,_gemText,_levelText;
    private void Awake() {
        instance = this;
    }
    void Start()
    {
        GetAppearance();
    }
    void OnDisable()
    {
        SaveAppearance();
    }
    public void CloseUI()
    {
        foreach (var item in UIandCameraEtc)
        {
            item.SetActive(false);
        }
    }
    public void OpenUI()
    {
        foreach (var item in UIandCameraEtc)
        {
            item.SetActive(true);
        }
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
            data = JsonConvert.DeserializeObject<Data>(result.Data[TITLE_DATA_KEY].Value);
            _goldText.text = data._gold.ToString();
            _healthText.text = data._health.ToString();
            _gemText.text = data._gem.ToString();
            _levelText.text ="Level " + data._level.ToString();
        }
        else
        {
            data = new Data(900,5,10,1);
            SaveAppearance();
            GetAppearance();
            Debug.Log("NotData");
        }
    }
    
    
    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Successfull Data send");
    }
    public void SaveAppearance()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {TITLE_DATA_KEY,JsonConvert.SerializeObject(data.ReturnData())}
            }
        };
        
        PlayFabClientAPI.UpdateUserData(request,OnDataSend,OnError);
        // SetUi();
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/create account");
        Debug.Log(error.GenerateErrorReport());
    }
    public void NewLevelButton()
    {
        SceneManager.LoadScene(data.GetLevel()+levelOffset);
    }
}