using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameInput : Singleton<PlayerNameInput>
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _continueBtn;
    public string DisplayName;
    private const string PlayerPrefsNameKey = "PlayerName";
    private void Start() {
        
    }
    private void SetInputField()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
        _inputField.text = defaultName;
        SetPlayerName(defaultName);
    }
    public void SetPlayerName(string name)
    {
        _continueBtn.interactable = !string.IsNullOrEmpty(name);
    }
    public void SavePlayerName()
    {
        DisplayName = _inputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey,DisplayName);
        
    }

}
