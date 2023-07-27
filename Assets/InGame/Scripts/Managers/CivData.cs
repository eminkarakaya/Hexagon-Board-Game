using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="InGame/Data/CivData")]
public class CivData : ScriptableObject
{
    public string civName;
    public int civType;
    public Sprite civImage; 
    public Color primaryColor,secondaryColor;
    
}
