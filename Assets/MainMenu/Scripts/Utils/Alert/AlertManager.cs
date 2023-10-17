using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public enum AlertType
{
    a,b,c
}
public class AlertManager : Singleton<AlertManager>
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float duration;
    [SerializeField] private GameObject alertPrefab;

    [SerializeField] private TMP_Text alertText;
    [SerializeField] private float yOffset;
    [SerializeField] private Transform parentAlert;
   
    public void AlertAnimation()
    {

        Vector3 pos = new Vector3(((Screen.width/2)-yOffset) ,Screen.height/2);
        var obj = Instantiate(alertPrefab,pos,Quaternion.identity,parentAlert);
        fadeImage = obj.GetComponent<Image>();
        alertText = obj.transform.GetChild(0).GetComponent<TMP_Text>();
        
        obj.transform.DOMove(new Vector3(pos.x,pos.y + yOffset,pos.z)  ,duration);
        alertText.DOFade(0,duration);
        fadeImage.DOFade(0,duration).OnComplete(()=>{
                Destroy(obj,.1f);
            });
        Debug.Log(obj,obj);
    }
    public void ShowAlert(string alertContent,AlertType alertType = AlertType.a)
    {
        AlertAnimation();
        alertText.text = alertContent;
    }
}
