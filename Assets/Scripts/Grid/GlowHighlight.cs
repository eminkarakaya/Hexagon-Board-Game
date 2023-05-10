using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowHighlight : MonoBehaviour
{
    Dictionary<Renderer,Material[]> glowMaterialDict = new Dictionary<Renderer, Material[]>(); 
    Dictionary<Renderer,Material[]> enemyGlowMaterialDict = new Dictionary<Renderer, Material[]>(); 
    Dictionary<Renderer,Material[]> originalMaterialDict = new Dictionary<Renderer, Material[]>(); 
    Dictionary<Color,Material> cachedGlowMaterials = new Dictionary<Color, Material>(); 

    public Material glowMaterial,enemyGlowMaterial;
    private bool isGlowing = false;
    private Color validSpaceColor = Color.green;
    private Color originalGlowColor,enemyGlowColor;
    private void Awake() {
        PrepareMaterialDictionaries();
        originalGlowColor = glowMaterial.GetColor("_GlowColor");
        enemyGlowColor = enemyGlowMaterial.GetColor("_GlowColor");
    }
    private void PrepareMaterialDictionaries()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material [] originalMaterials = renderer.materials;                 // once ilk materyallerını orıgınal materıale atıyoruz
            originalMaterialDict.Add(renderer,originalMaterials);               // dict e atıyorz
            Material [] newMaterials = new Material[renderer.materials.Length]; // yenı materıal olusturuyoruz
            Material [] enemyMaterials = new Material[renderer.materials.Length];

            for (int i = 0; i < originalMaterials.Length; i++)                  
            {
                Material mat = null;
                Material enemyMat = null;
                if(cachedGlowMaterials.TryGetValue(originalMaterials[i].color,out mat) == false)
                {
                    enemyMat = new Material(enemyGlowMaterial);
                    mat = new Material(glowMaterial);
                    enemyMat.color = originalMaterials[i].color;
                    mat.color = originalMaterials[i].color;
                    cachedGlowMaterials[mat.color] = mat;
                }
                newMaterials[i] = mat;
                enemyMaterials[i] = enemyMat;

            }
            glowMaterialDict.Add(renderer,newMaterials);
            enemyGlowMaterialDict.Add(renderer,enemyMaterials);
        }
        
    }
    public void ToggleGlow()
    {
        Hex hex;
        if(TryGetComponent(out Hex hex1))
        {
            hex = hex1;
        }
        else
        {
            hex = GetComponent<Unit>().Hex;
        }
        if(hex.IsEnemy())
        {
            if(!isGlowing)
            {
                ResetGlowHighlight();
                foreach (Renderer renderer in originalMaterialDict.Keys)
                {
                    renderer.materials = enemyGlowMaterialDict[renderer];
                }
            }
            else
            {
                foreach (Renderer renderer in originalMaterialDict.Keys)
                {
                    renderer.materials = originalMaterialDict[renderer];
                }
            }
            isGlowing = !isGlowing;
        }
        else
        {
            if(!isGlowing)
            {
                ResetGlowHighlight();
                foreach (Renderer renderer in originalMaterialDict.Keys)
                {
                    renderer.materials = glowMaterialDict[renderer];
                }
            }
            else
            {
                foreach (Renderer renderer in originalMaterialDict.Keys)
                {
                    renderer.materials = originalMaterialDict[renderer];
                }
            }
            isGlowing = !isGlowing;
        }
    }
    public void ToggleGlow(bool state)
    {
        if(isGlowing == state)
            return;
        isGlowing = !state;
        ToggleGlow();
    }
    public void EnemyToggleGlow()
    {
        if(!isGlowing)
        {
            ResetGlowHighlight();
            foreach (Renderer renderer in enemyGlowMaterialDict.Keys)
            {
                renderer.materials = enemyGlowMaterialDict[renderer];
            }
        }
        else
        {
            foreach (Renderer renderer in originalMaterialDict.Keys)
            {
                renderer.materials = originalMaterialDict[renderer];
            }
        }
        isGlowing = !isGlowing;
    }
    internal void ResetGlowHighlight()
    {
        Hex hex;
        if(TryGetComponent(out Hex hex1))
        {
            hex = hex1;
        }
        else
        {
            hex = GetComponent<Unit>().Hex;
        }
        if(hex.IsEnemy())
        {
            foreach (Renderer renderer1 in enemyGlowMaterialDict.Keys)
            {
                foreach (var item in enemyGlowMaterialDict[renderer1])
                {
                    if(item != null)
                    item.SetColor("_GlowColor",enemyGlowColor);
                }
                renderer1.materials = enemyGlowMaterialDict[renderer1];
            }
        }
        else
        {
            foreach (Renderer renderer in glowMaterialDict.Keys)
            {
                foreach (var item in glowMaterialDict[renderer])
                {
                    item.SetColor("_GlowColor",originalGlowColor);
                }
                renderer.materials = glowMaterialDict[renderer];
            }
        }
    }
    internal void HighlightValidPath()
    {
        if(!isGlowing) return;

        foreach (Renderer renderer in glowMaterialDict.Keys)
        {
            foreach (var item in glowMaterialDict[renderer])
            {
                item.SetColor("_GlowColor",validSpaceColor);
            }
            renderer.materials = glowMaterialDict[renderer];
        }
    }
}
