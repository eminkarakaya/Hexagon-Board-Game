
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayfabManager : MonoBehaviour
{
    [SerializeField] private Toggle _rememberMeToggle;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private GameObject _nameWindow,_registerWindow;
    [SerializeField] private TMP_InputField _emailInputField,_passwordInputField,_nameInput;
    void Start()
    {
        if(PlayerPrefs.HasKey("EMAIL"))
        {
            _emailInputField.text = PlayerPrefs.GetString("EMAIL");
            _passwordInputField.text = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest{
                Email = _emailInputField.text,
                Password = _passwordInputField.text,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams{
                GetPlayerProfile = true
            }
            };
            PlayFabClientAPI.LoginWithEmailAddress(request,OnLoginSuccess,OnError);
        }
        #if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest{
                AndroidDeviceId = ReturnAndroidID(),
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams{
                GetPlayerProfile = true
            }
            };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid,OnLoginSuccessAndroid,OnAndroidError);

        #endif
        // SendLeaderBoard(123);
        // SaveAppearance();
        // GetAppearance();
        // GetTitleData();
    }
    void AndroidLogin()
    {
        #if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest{
                AndroidDeviceId = ReturnAndroidID(),
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams{
                GetPlayerProfile = true
            }
            };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid,OnLoginSuccessAndroid,OnAndroidError);
        #endif
    }
    public static string ReturnAndroidID()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        return deviceId;
    }
    public void RegisterButton()
    {
        var request = new RegisterPlayFabUserRequest{
            Email = _emailInputField.text,
            Password = _passwordInputField.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request,OnRegisterSuccess,OnError);
    }
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Register and logged in ");
        _statusText.text = "Register and logged in";
    }
    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest{
            Email = _emailInputField.text,
            Password = _passwordInputField.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams{
                GetPlayerProfile = true
            }
        };
        if(_rememberMeToggle.isOn)
        {
            PlayerPrefs.SetString("EMAIL",_emailInputField.text);
            PlayerPrefs.SetString("PASSWORD",_passwordInputField.text);
        }
        PlayFabClientAPI.LoginWithEmailAddress(request,OnLoginSuccess,OnError);
        
    }
    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login success + " + result.InfoResultPayload.UserData);
        _statusText.text = " Login success + " + result.InfoResultPayload.UserData;
        string name = null;
        if(result.InfoResultPayload.PlayerProfile != null)
            name = result.InfoResultPayload.PlayerProfile.DisplayName;  
        if(name == null)
        {
            _nameWindow.SetActive(true);
            _registerWindow.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
    void OnLoginSuccessAndroid(LoginResult result)
    {
        Debug.Log("Login success + " + result.InfoResultPayload.UserData);
        _statusText.text = " Login success + " + result.InfoResultPayload.UserData;
        string name = null;
        if(result.InfoResultPayload.PlayerProfile != null)
            name = result.InfoResultPayload.PlayerProfile.DisplayName;  
        if(name == null)
        {
            _nameWindow.SetActive(true);
            _registerWindow.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
    public void NameWindowSubmitButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest{
            DisplayName = _nameInput.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,OnDisplayNameUpdate,OnError);
        SceneManager.LoadScene(1);
    }
    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name " + result.DisplayName);
        _statusText.text = "Updated display name + " + result.DisplayName;
    }
    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = _emailInputField.text,
            TitleId = "D6B47"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request,OnPasswordReset,OnError);
    }
    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Password Reset Mail Sent Your Email");
        _statusText.text = "Password Reset Mail Sent Your Email";
    }
    
    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/create account");
        Debug.Log(error.GenerateErrorReport());
        _statusText.text = error.ErrorMessage;
    }
    void OnAndroidError(PlayFabError error)
    {
        Debug.Log("Error while logging in/create account");
        Debug.Log(error.GenerateErrorReport());
        _statusText.text = error.ErrorMessage;
    }
    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest{
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate{
                    StatisticName = "PlatformScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request,OnLeaderboardUpdate,OnError);

    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfull leaderboard sent");
    }
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest{
            StatisticName = "PlatformScore",
            StartPosition = 0,
            MaxResultsCount = 1000
        };
        PlayFabClientAPI.GetLeaderboard(request,OnLeaderboardGet,OnError);
    }
    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            Debug.Log(item.Position + " "  +item.PlayFabId + " " + item.StatValue);
        }
    }
    

    void GetTitleData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),OnTitleDataRecieved,OnError);
    }
    void OnTitleDataRecieved(GetTitleDataResult result)
    {
        if(result.Data == null || result.Data.ContainsKey("Message") == false)
        {
            Debug.Log("No message");
            return;
        }
        Debug.Log("TİTLEDATA " + result.Data["Message"]);
    }
    
}
