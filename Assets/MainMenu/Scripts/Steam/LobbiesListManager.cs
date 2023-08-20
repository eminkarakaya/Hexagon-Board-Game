using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LobbiesListManager : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject loaderCanvasGO;
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

    public async void LoadScene(string sceneName,System.Action action)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        loaderCanvasGO.SetActive(true);
        do
        {
            await Task.Delay(100);
            progressBar.fillAmount = scene.progress;
            
        } while (scene.progress< .9f);

        scene.allowSceneActivation = true;
        loaderCanvasGO.SetActive(false);
        action?.Invoke();
    }
}
