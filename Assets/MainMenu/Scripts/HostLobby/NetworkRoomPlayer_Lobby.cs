using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private TMP_Text[] playerNameTexts;
    [SerializeField] private TMP_Text[] playerReadyTexts;
    [SerializeField] private Button startGameButton;

    [SyncVar(hook =nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook =nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;
    public bool IsLeader{set{
        isLeader = value;
        startGameButton.gameObject.SetActive(value);
    }}
    private NetworkManagerGdd room;
    public NetworkManagerGdd Room
    {
        get
        {
            if(room != null) return room;
            return room = NetworkManagerGdd.singleton as NetworkManagerGdd;
        }
        
    }
    
    public override void OnStartAuthority()
    {
        CMDSetDisplayName(PlayerNameInput.DisplayName);
        lobbyUI.SetActive(true);
    }
    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);
        // NetworkManagerGdd.OnSceneChangedEvent += CloseDisplayRoomPlayer;
        UpdateDisplay();
    }
    private void CloseDisplayRoomPlayer()
    {
        // 
    }
    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);
        // NetworkManagerGdd.OnSceneChangedEvent -= CloseDisplayRoomPlayer;
        UpdateDisplay();
    }
    public void HandleReadyStatusChanged(bool oldValue , bool newValue)
    {
        UpdateDisplay();
    }
    public void HandleDisplayNameChanged(string oldValue , string newValue)
    {
        UpdateDisplay();
    }
    private void UpdateDisplay()
    {
        if(!isOwned)
        {
            foreach (var item in Room.RoomPlayers)
            {
                if(item.isOwned)
                {
                    item.UpdateDisplay();
                    break;
                }
            }
            return;

        }
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            if(Room.RoomPlayers[i].IsReady)
            {
                playerReadyTexts[i].text = "Ready";
                playerReadyTexts[i].color = Color.green;
            }
            else
            {
                playerReadyTexts[i].text = "Not Ready";
                playerReadyTexts[i].color = Color.red;
            }
        }
    }
    public void HandleReadyToStart(bool readyToStart)
    {
        if(!isLeader) return;
        startGameButton.interactable = readyToStart;
    }
    [Command] private void CMDSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
    [Command] public void CMDReadyUp()
    {
        IsReady = !IsReady;
        Room.NotifyPlayersOnReadyState();
    }
    public void CloseCanvas()
    {
        if(TryGetComponent(out Canvas canvas) && isOwned)
        {
            Debug.Log(canvas,canvas.gameObject);
            canvas.enabled = false;
        }
    }
    
    public void StartGame()
    {
        
        CMDStartGame();
    }
    [Command] public void CMDStartGame()
    {
        if(Room.RoomPlayers[0].connectionToClient != connectionToClient) return;
        Room.StartGame();
        
    }
}
