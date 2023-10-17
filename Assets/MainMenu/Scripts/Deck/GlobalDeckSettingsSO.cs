using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName ="Deck/DeckSettings")]
public class GlobalDeckSettingsSO : ScriptableObjectSinleton<GlobalDeckSettingsSO>
{
    
    public int maxDeckCount = 10;
    public int deckCardCapacity = 40;
    public int deckHeroCapacity = 6;
    public int deckSameCardCapacity = 3;
    
   

    // public List<>
    




    
}
