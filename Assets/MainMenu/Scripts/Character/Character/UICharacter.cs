using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UICharacter : MonoBehaviour,IDragHandler ,IPointerDownHandler , IPointerUpHandler 
{

    public bool moveable = false;

    [Header("UI")]
    [SerializeField] private Image backGroundImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] public TMP_Text countText;
    [Space(20)]

    [SerializeField] private GameObject selectedGameobject;
    [SerializeField] private SelectedUICharacter selectedUICharacter;
    private int level;
    [SerializeField] private LevelTip levelTip;
    public CharacterData CharacterData{get =>characterData;}
    [SerializeField] private CharacterData characterData;
    [SerializeField] private List<RegionType> regionTypes;
    [SerializeField] private List<PropertiesEnum> propertiesType;
    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private Transform regionParent,propParent;
    private Transform dragParent;
    
    [SerializeField] private float dragOffset,currentDragOffset,moveUpOffset,currentMoveUpOffset,addConditionOffset;
    [SerializeField] private bool isCreated,isMoveUp,addCondition;

    private void Start() {
        dragParent = GameObject.Find("DragDropCanvas").transform;        
        regionTypes = characterData.Regions;
        foreach (var item in regionTypes)
        {
            var go = Region.CreateCardRegion(item,regionParent);
            RegionTip regionTip = go.GetComponent<RegionTip>();
            regionTip.regionType = item;
            regionTip.InitializeTip();
        }
        propertiesType = characterData.Properties.Select(x=>x.attackPropertiesEnum).ToList();
        foreach (var item in propertiesType)
        {
            if(item == PropertiesEnum.MoveKill) continue;
            var go = Properties.CreateCardProperty(item,propParent);
            PropertyTip propertyTip = go.GetComponent<PropertyTip>();
            propertyTip.propertiesEnum = item;
            propertyTip.InitializeTip();
        }
        levelTip.level = CharacterData.level;
        backGroundImage.sprite = characterData.fullSprite;
        level = characterData.level;
        levelText.text = level.ToString();
        SetCountText();
        
        
    }
    
    public void SetCountText()
    {
        if(moveable)
        {
            countText.text = DeckManager.Instance.selectedDeck.GetCharacterCountInDeck(selectedUICharacter) + "/" + characterData.savedCharacterData.ownedCount + "("+characterData.capacity+")"; 
        }
        else
        {
            countText.text = characterData.savedCharacterData.ownedCount + "/"+ characterData.capacity; 

        }
    }
    
    public void Buy(int count = 1)
    {
        characterData.savedCharacterData.ownedCount ++;
        SetCountText();
    }
    
    
    public void SetCharacterData(CharacterData _characterData)
    {
        characterData = _characterData;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(!moveable) 
        {
            var scrollRect = GetComponentInParent<ScrollRect>();
            eventData.pointerDrag = scrollRect.gameObject;
            EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);
            
            scrollRect.OnInitializePotentialDrag(eventData);    
            scrollRect.OnBeginDrag(eventData);
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(isMoveUp && !isCreated)
            {
                var scrollRect = GetComponentInParent<ScrollRect>();
                eventData.pointerDrag = scrollRect.gameObject;
                EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);
                
                scrollRect.OnInitializePotentialDrag(eventData);    
                scrollRect.OnBeginDrag(eventData);
            }
            if(selectedUICharacter != null)
            {
                currentDragOffset += eventData.delta.x;
                currentMoveUpOffset += eventData.delta.y;

            }

                
            if(selectedUICharacter != null)
            {
                if( !isCreated)
                {

                    if(Mathf.Abs(currentMoveUpOffset) > moveUpOffset && !isMoveUp)
                    {
                        isMoveUp = true;
                        isCreated = false;
                        currentDragOffset = 0;
                        selectedUICharacter = null;
                        
                    }
                    if(Mathf.Abs(currentDragOffset) > dragOffset && !isCreated && !isMoveUp)
                    { 
                        selectedUICharacter = Instantiate(selectedGameobject,eventData.position,Quaternion.identity,dragParent).GetComponent<SelectedUICharacter>();
                        selectedUICharacter.uICharacter = this;
                        
                        isCreated = true;
                    }
                }
                else
                {
                    selectedUICharacter.gameObject.transform.position = eventData.position; 
                    if(currentDragOffset < addConditionOffset)
                    {
                        addCondition = true;
                    }
                    else
                    {
                        addCondition = false;
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(!moveable) return;
            selectedUICharacter = selectedGameobject.GetComponent<SelectedUICharacter>();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            DeckManager.Instance.currentRightClickedUICharacter = this;
            DeckManager.Instance.OpenRightclickCanvas();
            DeckManager.Instance.BuyButton();
            
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!moveable) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(addCondition)
            {
                DeckManager.Instance.DeckAddItem(new DeckCharacterUIData{uICharacter = this,selectedUICharacter =this.selectedUICharacter,characterData = characterData});
                addCondition = false;
                
            }
            else
            {
                if(isCreated)
                    Destroy(selectedUICharacter.gameObject);
            }
                

            
            isCreated = false;
            currentDragOffset = 0;
            selectedUICharacter = null;
            currentMoveUpOffset = 0;
            isMoveUp = false;
        }
    }

   
}
