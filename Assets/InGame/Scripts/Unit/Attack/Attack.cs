using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.Events;
using System;


public abstract class Attack : NetworkBehaviour
{   
    public List<PropertiesStruct> PropertiesEnumList;
    [SerializeField] private TMP_Text attackText;
    public float attackTime;
    Dictionary<Renderer,Material[]> transparentMaterialDict = new Dictionary<Renderer, Material[]>(); 
    Dictionary<Renderer,Material[]> originalMaterialDict = new Dictionary<Renderer, Material[]>(); 
    Dictionary<Color,Material> cachedGlowMaterials = new Dictionary<Color, Material>(); 
    [SerializeField] protected NetworkAnimator networkAnimator;
    public Color originalColor;
    public Material transparentMaterial,originalMaterial;
    public UnityEvent killEvent;
    protected IAttackable attackable;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] protected int _damagePower;
    public UnityEvent AttackEvent;
    public System.Action<Hex> KillEvent;
    protected int DamagePower { get; set; }
    public int range = 1;
    private void Awake() {
    }
    private void OnValidate() {
        
        networkAnimator = GetComponent<NetworkAnimator>();
    }
    private void Start() {
        PropertiesEnumList = attackable.attackProperties;
        attackable = GetComponent<IAttackable>();
        PrepareMaterialDictionaries();
        originalColor = transparentMaterial.GetColor("_Color");
        SetDamage(_damagePower);
    }
    protected IEnumerator CloseTransparentMaterial(float dur)
    {
        Material mat = new Material(transparentMaterial);
        ResetGlowHighlight();
        Color color = mat.color;
        color.a = 0f;
        float timeElapsed = 0f;
        while(timeElapsed < dur)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / dur;
            color.a = Mathf.Lerp(0f,1f,lerpStep);
            foreach (Renderer renderer in originalMaterialDict.Keys)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.material.SetColor("_BaseColor",color);
                }
            }
            yield return null;
        }
        
        foreach (Renderer renderer in originalMaterialDict.Keys)
        {
            renderer.materials = originalMaterialDict[renderer];
        }
    }
    protected IEnumerator OpenTransparentMaterial(float dur)
    {
        ResetGlowHighlight();
        foreach (Renderer renderer in originalMaterialDict.Keys)
        {
            renderer.materials = transparentMaterialDict[renderer];
        }
        Material mat = new Material(transparentMaterial);
        Color color = mat.color;
        color.a = 0f;
        float timeElapsed = 0f;
        while(timeElapsed < dur)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / dur;
            color.a = Mathf.Lerp(1f,0f,lerpStep);
            foreach (Renderer renderer in originalMaterialDict.Keys)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.material.SetColor("_BaseColor",color);
                }
            }
            yield return null;
        }
    }
    private void PrepareMaterialDictionaries()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material [] originalMaterials = renderer.materials;              // once ilk materyallerını orıgınal materıale atıyoruz
            originalMaterialDict.Add(renderer,originalMaterials);               // dict e atıyorz
            Material [] newMaterials = new Material[renderer.materials.Length]; // yenı materıal olusturuyoruz

            for (int i = 0; i < originalMaterials.Length; i++)                  
            {
                Material mat = null;
                // Material rangeMat = null;
                if(cachedGlowMaterials.TryGetValue(originalMaterials[i].color,out mat) == false)
                {
                    // rangeMat = new Material(rangeGlowMaterial);
                    mat = new Material(transparentMaterial);
                    mat.color = originalMaterials[i].color;
                    
                    cachedGlowMaterials[mat.color] = mat;
                }
                newMaterials[i] = mat;

            }
            transparentMaterialDict.Add(renderer,newMaterials);
        }
        
    }
    internal void ResetGlowHighlight()
    {   
        foreach (Renderer renderer in transparentMaterialDict.Keys)
        {
            foreach (var item in transparentMaterialDict[renderer])
            {
                item.SetColor("_GlowColor",originalColor);
            }
            renderer.materials = transparentMaterialDict[renderer];
        }
    }
    void SetDamage(int value)
    {
        _damagePower = value;
        _damageText.text = _damagePower.ToString();
    }

    
    public abstract IEnumerator AttackUnit(IDamagable damagable, IAttackable attackable,float movementDuration);
    // [Command]
    // protected virtual void CMDInflictDamage(HP hp)
    // {
    //     InflictDamage(hp);
    // } 

    protected void InflictDamage(HP hp)
    {
        
    }

}
