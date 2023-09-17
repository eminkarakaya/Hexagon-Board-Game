using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerGdd networkManagerGdd;

    [Header("uı")]
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private Button joinButton;

    private void OnEnable() {
        NetworkManagerGdd.OnClientConnected += HandleClientConnected;
        NetworkManagerGdd.OnClientDisconnected += HandleClientDisconnected;
    }
    private void OnDisable() {
        NetworkManagerGdd.OnClientConnected -= HandleClientConnected;
        NetworkManagerGdd.OnClientDisconnected -= HandleClientDisconnected;
    }
    public void JoinLobby()
    {
        string ipAddress = "localhost";
        // string ipAddress = ipAddressInputField.text;

        // networkManagerGdd.networkAddress = ipAddress;
        networkManagerGdd.StartClient();
        Debug.Log("start");
        joinButton.interactable = false;
    }
    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }
    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
