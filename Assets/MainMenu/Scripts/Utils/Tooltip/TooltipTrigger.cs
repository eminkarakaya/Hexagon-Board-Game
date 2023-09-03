using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string content,header;
    public UnityEvent PointerEnterEvent,PointerExitEvent;
    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent?.Invoke();
        ToopltipManager.Show(content,header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        PointerExitEvent?.Invoke();
        ToopltipManager.Hide();
    }
}
