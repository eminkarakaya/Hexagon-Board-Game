using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager  networkManger = null;
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby()
    {
        networkManger.StartHost();
        landingPagePanel.SetActive(false);
        
    }
}
