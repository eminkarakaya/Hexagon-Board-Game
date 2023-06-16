using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private GameObject readyObj;
    private void Start() {
        
    }
    
    public void SetName(string value)
    {
        _nameText.text = value;
    }
    public void ToggleReady()
    {
        readyObj.SetActive(!readyObj.activeSelf);
    }
    
}
