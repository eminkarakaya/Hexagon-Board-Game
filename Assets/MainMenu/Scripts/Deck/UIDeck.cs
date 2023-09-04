using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDeck : MonoBehaviour
{
    [Header("UI")] 
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text deckName;
    // [SerializeField] private Transform 
    public DeckAllData deckAllData;

    public void SetDeckData(DeckAllData _deckData)
    {
        deckAllData = _deckData;
        
        deckName.text = deckAllData.deckName;
    }
    private void Start() {
        deckName.text = deckAllData.deckName;
    }
}
