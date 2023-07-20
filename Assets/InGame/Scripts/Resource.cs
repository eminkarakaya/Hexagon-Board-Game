using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum ResourceType
{
    None,
    Oil,
    Coal,
    Niter,
    Iron
}

public class Resource : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private Image resourceIcon,goldIcon;
    private Sprite sprite;
    [SerializeField] private ResourceType resourceType;
    public ResourceType ResourceType{get => resourceType; private set {resourceType = value;}}
    public int Gold;
    public GameObject prefab;
    private void Start() {
        
        prefab = FindObjectOfType<HexGrid>().GetBuildingPrefab(ResourceType,out sprite);
        if(prefab != null)
        {
            resourceIcon.sprite = sprite;
        }
        if(ResourceType == ResourceType.None)
        {
            resourceIcon.enabled = false;
            goldText.enabled = false;
            goldIcon.enabled = false;
        }
    }
    private void SetGold(int gold)
    {
        Gold = gold;
        goldText.text = Gold.ToString();
    }
    
}
