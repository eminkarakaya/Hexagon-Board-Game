using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mirror;
using TMPro;
using Steamworks;
using UnityEngine.SceneManagement;
using System;

public class PlayerManagerLobby : NetworkBehaviour
{
    [SerializeField] PlayerInfoDisplay lobbyPrefab;
    public PlayerInfoDisplay lobby;
    private PlayerManager playerManager;
    [SerializeField] private GameObject playerManagerPrefab;
    public LobbyItem lobbyItemPrefab;
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;
    [SyncVar(hook =nameof(HandleSteamIDUpdated))] private ulong steamID;
    LobbyItem lobbyItem;
     private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if(callback.m_steamID.m_SteamID != steamID) return;
        lobbyItem.profileImage.texture = GetSteamImageAsTexture(callback.m_iImage);
    }

    private void HandleSteamIDUpdated(ulong oldId,ulong newId)
    {
        if(lobbyItem == null)
        {
            lobbyItem = Instantiate(lobbyItemPrefab,SteamNetworkManager.instance.playerPrefabParent);

        }
        var cSteamID = new CSteamID(newId);
        lobbyItem.displayNameText.text = SteamFriends.GetFriendPersonaName(cSteamID);
        int imageId = SteamFriends.GetLargeFriendAvatar(cSteamID);
        if(imageId == -1) return;
        lobbyItem.profileImage.texture =  GetSteamImageAsTexture(imageId);
    }
    public void SetSteamId(ulong steamID)
    {
        this.steamID = steamID;
    }
    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(iImage,out uint width,out uint height);
        if(isValid)
        {
            byte[] image = new byte[width*height*4];
            isValid = SteamUtils.GetImageRGBA(iImage,image,(int)(width*height*4));
            if(isValid)
            {
                texture = new Texture2D((int)width,(int)height,TextureFormat.RGBA32,false,true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        return texture;
    }
     [ClientRpc] private void RPCSetParentLobby()
    {
        lobby.gameObject.SetActive(true);
        lobby.transform.SetParent(SteamNetworkManager.instance.playerPrefabParent);
    }
    [Command] public void CreateLobbyItem()
    {

        GameObject _lobby = Instantiate(lobbyPrefab).gameObject;
        NetworkServer.Spawn(_lobby,connectionToClient);
        lobby = _lobby.GetComponent<PlayerInfoDisplay>();
        RPCSetParentLobby();
    }
    [Command]
    public void CMDBeginGame()
    {
        TargetBeginGame();
    }
    [TargetRpc]
    public void TargetBeginGame () {
        // Additively load game scene
        SceneManager.LoadScene (1,LoadSceneMode.Additive);
        
        lobby.gameObject.SetActive(false);
        playerManager =  Instantiate(playerManagerPrefab).GetComponent<PlayerManager>();
        NetworkServer.Spawn(playerManager.gameObject,this.gameObject);
        
        StartCoroutine ( playerManager.StartGame());
        // StartCoroutine(playerManager.wait());
    }

}
