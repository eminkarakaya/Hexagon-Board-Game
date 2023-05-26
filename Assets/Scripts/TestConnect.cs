using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class TestConnect : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI playerCount;
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("connecting to server");
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {

        Debug.Log("connected to server");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {

        PhotonNetwork.JoinOrCreateRoom("oda",new RoomOptions{MaxPlayers = 2,IsOpen = true,IsVisible = true,},TypedLobby.Default);
        Debug.Log("connected to lobbyu");
        base.OnJoinedLobby();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("connected to room");
        base.OnJoinedRoom();
        playerCount.text = PhotonNetwork.CountOfPlayersInRooms.ToString();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        // Debug.Log("Disconnect " + cause);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
