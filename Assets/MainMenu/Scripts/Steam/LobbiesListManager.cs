using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;
    private void Awake() {
        instance = this;
    }
    public GameObject lobbiesMenu,lobbyDataItemPrefab,lobbyListContent;
    public GameObject lobbiesButton,hostButton;
    public List<GameObject> listOfLobbies = new List<GameObject>();
    public void DestroyLobbies()
    {
        foreach (var item in listOfLobbies)
        {
            Destroy(item);
        }
        listOfLobbies.Clear();
    }
    public void DisplayLobbies(List<CSteamID> lobbyIDs,LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            if(lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);
                createdItem.GetComponent<LobbyData>().lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID,"name");
                createdItem.GetComponent<LobbyData>().SetLobbyData();
                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;
                listOfLobbies.Add(createdItem);

            }
        }
    }
    public void GetListOfLobbies()
    {
        lobbiesButton.SetActive(false);
        hostButton.SetActive(false);
        lobbiesMenu.SetActive(true);
        CreateLobbySteam.instance.GetLobbiesList();
    }    
}
