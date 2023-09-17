using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button continueButton;
    public static string DisplayName {get; private set;}
    private const string PlayerPrefsNameKey = "PlayerName";
    private void Start() {
        SetUpInputField();
    }
    public void SetUpInputField()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        nameInputField.text = defaultName;
    }
    public void SetPlayerName(string name)
    {
        // continueButton.interactable = !string.IsNullOrEmpty(name);
    }
    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey,DisplayName);
    }

}
