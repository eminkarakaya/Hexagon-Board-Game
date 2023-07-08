using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System;
using Steamworks;
public class PlayerInfoDisplay : NetworkBehaviour
{
    public LobbyItem lobbyItemPrefab;

    LobbyItem lobbyItem;
    [SyncVar(hook =nameof(HandleSteamIDUpdated))] private ulong steamID;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;
    public override void OnStartClient()
    {  
        if(lobbyItem == null)
            lobbyItem = Instantiate(lobbyItemPrefab,SteamNetworkManager.instance.playerPrefabParent); 
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

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
    public void SetSteamId(ulong steamID)
    {
        this.steamID = steamID;
    }
    [Command]
    public void CMDBeginGame()
    {
        TargetBeginGame();
    }
    [TargetRpc]
    public void TargetBeginGame () {
        //Additively load game scene
        SceneManager.LoadScene (1, LoadSceneMode.Additive);
    }
}
