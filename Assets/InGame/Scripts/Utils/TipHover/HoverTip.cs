using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float horizontalOffset, verticalOffset;
    public string tipToShow;
    public float timeToWait;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        HoverTipManager.OnMouseLoseFocus();
    }
    private void ShowMessage()
    {
        HoverTipManager.OnMouseHover(tipToShow, new Vector2 (Input.mousePosition.x + horizontalOffset,Input.mousePosition.y + verticalOffset));
    }
    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        ShowMessage();
    }
}
