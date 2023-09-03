using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Deck/DeckSettings")]
public class GlobalDeckSettingsSO : ScriptableObjectSinleton<GlobalDeckSettingsSO>
{

    [SerializeField] private GameObject uICharacterPrefab;
    public List<CharacterData> allCharacters;
    public int maxDeckCount = 10;
    public int deckCardCapacity = 40;
    public int deckHeroCapacity = 6;
    public int deckSameCardCapacity = 3;
    public UICharacter CreateUICharacter(CharacterData characterData, Transform parent)
    {   
        var obj = Instantiate(uICharacterPrefab,parent);
        UICharacter uICharacter = obj.GetComponent<UICharacter>();
        uICharacter.SetCharacterData(characterData);
        return uICharacter;
    }   
    
    public List<UICharacter> CreateAllCharacters(Transform parent)
    {
        List<UICharacter> characterDatas = new List<UICharacter>();
        for (int i = 0; i < allCharacters.Count; i++)
        {
            characterDatas.Add (CreateUICharacter(allCharacters[i],parent));
        }
        return characterDatas;
    } 
}
