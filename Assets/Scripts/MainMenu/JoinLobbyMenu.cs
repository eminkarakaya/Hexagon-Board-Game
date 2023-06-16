using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager = null;
    [SerializeField] private GameObject landingPanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    
    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
        joinButton.interactable = false;

    }
    private void HandleClientConnected()
    {
        joinButton.interactable = true;
        gameObject.SetActive(false);
        landingPanel.SetActive(false);

    }   
    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;

    }
}
