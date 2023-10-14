using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterDisplay : MonoBehaviour
{
    [SerializeField] private Image backGroundImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] public TMP_Text countText;
    [SerializeField] private Transform regionParent,propParent;
    public CharacterData CharacterData;
    [SerializeField] private List<RegionType> regionTypes;
    [SerializeField] private List<PropertiesEnum> propertiesType;
    private int level;
    [SerializeField] private LevelTip levelTip;
    private void Start() {
        regionTypes = CharacterData.Regions;
        foreach (var item in regionTypes)
        {
            var go = Region.CreateCardRegion(item,regionParent);
            RegionTip regionTip = go.GetComponent<RegionTip>();
            regionTip.regionType = item;
            regionTip.InitializeTip();
        }
        propertiesType = CharacterData.Properties.Select(x=>x.attackPropertiesEnum).ToList();
        foreach (var item in propertiesType)
        {
            if(item == PropertiesEnum.Default) continue;
            var go = Properties.CreateCardProperty(item,propParent);
            PropertyTip propertyTip = go.GetComponent<PropertyTip>();
            propertyTip.propertiesEnum = item;
            propertyTip.InitializeTip();
        }
        levelTip.level = CharacterData.level;
        backGroundImage.sprite = CharacterData.fullSprite;
        level = CharacterData.level;
        levelText.text = level.ToString();
        SetCountText();
    }
    public void SetCountText()
    {
        countText.text =  CharacterData.savedCharacterData.ownedCount + "/"+CharacterData.capacity; 
    }
    
}
