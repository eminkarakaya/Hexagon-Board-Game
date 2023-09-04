using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SelectedUICharacter : MonoBehaviour, IDragHandler , IPointerDownHandler , IPointerUpHandler
{
    private Transform dragParent;
    [SerializeField] private SelectedUICharacter selectedUICharacter;
    [SerializeField] private GameObject selectedGameobject;
    public UICharacter uICharacter;
    [SerializeField] private Image backgroundImage;
    public int count;
    [SerializeField] private TextMeshProUGUI countText,levelText,nameText;

    [SerializeField] private float dragOffset,currentDragOffset,moveUpOffset,currentMoveUpOffset,addConditionOffset;
    [SerializeField] private bool isCreated,isMoveUp,addCondition;
    private void Start() {
        dragParent = GameObject.Find("DragDropCanvas").transform;   
        SetData();
    }
    public void SetCountText()
    {
        uICharacter.countText.text = DeckManager.Instance.selectedDeck.GetCharacterCountInDeck(this) + "/" + uICharacter.CharacterData.savedCharacterData.ownedCount + "("+uICharacter.CharacterData.capacity+")"; 
    }
    public void IncreaseCount()
    {
        count++;
        countText.text = count.ToString();
    }
    public void DecreaseCount()
    {
        count--;
        countText.text = count.ToString();
    }

    public void SetData()
    {
        backgroundImage.sprite = uICharacter.CharacterData.selectedSprite;
        levelText.text = uICharacter.CharacterData.level.ToString();
        nameText.text = uICharacter.CharacterData.name;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        if(isMoveUp && !isCreated)
        {
            var scrollRect = GetComponentInParent<ScrollRect>();
            eventData.pointerDrag = scrollRect.gameObject;
            EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);
            
            scrollRect.OnInitializePotentialDrag(eventData);    
            scrollRect.OnBeginDrag(eventData);
        }
        if(!isCreated)
        {
            currentDragOffset += eventData.delta.x;
            currentMoveUpOffset += eventData.delta.y;

        }

            
        if( !isCreated)
        {

            if(Mathf.Abs(currentMoveUpOffset) > moveUpOffset && !isMoveUp)
            {
                isMoveUp = true;
                isCreated = false;
                currentDragOffset = 0;
                
            }
            if(Mathf.Abs(currentDragOffset) > dragOffset && !isCreated && !isMoveUp)
            { 
                selectedUICharacter = Instantiate(selectedGameobject,eventData.position,Quaternion.identity,dragParent).GetComponent<SelectedUICharacter>();
                selectedUICharacter.uICharacter = uICharacter;
                
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

    public void OnPointerDown(PointerEventData eventData)
    {
        selectedUICharacter = selectedGameobject.GetComponent<SelectedUICharacter>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isCreated = false;
        if(addCondition)
        {
            DeckManager.Instance.DeckRemoveItem(this);
            
            addCondition = false;
            Destroy(selectedUICharacter.gameObject);
            
        }
        else
        {
            Destroy(selectedUICharacter.gameObject);
        }
        currentDragOffset = 0;
        selectedUICharacter = null;
        currentMoveUpOffset = 0;
        isMoveUp = false;
    }
}
