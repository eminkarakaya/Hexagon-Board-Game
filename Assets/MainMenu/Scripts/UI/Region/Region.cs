using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RegionType
{
    Noxus,
    Demacia,
    Ionia,
    Shurima,
    BandleCity,
    Freljord,
    ShadowIsland

}
[System.Serializable]
public struct RegionData
{
    public RegionType regionType;
    public GameObject regionTip;
    public string regionDescription;
    public Sprite regionSprite;
    public void SetRegionData()
    {
        
    }
}

[CreateAssetMenu(menuName ="Data/RegionData")]
public class Region : SingletonScriptable<Region> 
{
    [SerializeField] private GameObject cardRegionPrefab;
    [SerializeField] private List<RegionData> regionDatas;
    public static RegionData GetRegionDataByRegionType(RegionType regionType)
    {
        foreach (var item in Region.Instance.regionDatas)
        {
            if(item.regionType == regionType)
                return item;
        }
        return new RegionData();
    }
    public static GameObject CreateCardRegion(RegionType regionType,Transform parent)
    {
        return Instantiate(Region.Instance.cardRegionPrefab,parent);
    }

    
}
