using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct PropertiesData
{
    public PropertiesEnum propertiesEnum;
    public PropertiesType propertiesType;
    public Sprite propertySprite;
    [TextArea]
    public string propTip;
}

[CreateAssetMenu(menuName ="Property/PropertyData")]

public class Properties : SingletonScriptable<Properties>
{
    [SerializeField] private GameObject cardPropPrefab;
    public List<PropertiesData> propertiesDatas;

    public static PropertiesData GetRegionDataByPropType(PropertiesEnum propertiesEnum)
    {
        foreach (var item in Properties.Instance.propertiesDatas)
        {
            if(item.propertiesEnum == propertiesEnum)
                return item;
        }
        return new PropertiesData();
    }
    public static GameObject CreateCardProperty(PropertiesEnum propertyType,Transform parent)
    {
        return Instantiate(Properties.Instance.cardPropPrefab,parent);
    }
}
