using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField,contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;

    public RectTransform rectTransform;
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        // if(Application.isEditor) 
        // {
        //     int headerLength = headerField.text.Length; 
        //     int contentLength = contentField.text.Length;

        //     layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        // }
        Vector2 position = Input.mousePosition;

        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        rectTransform.pivot = new Vector2(pivotX/5,pivotY/5);
        transform.position = position;

    }

    public void SetText(string content , string header = " ")
    {
        if(string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField.text = content;
        int headerLength = headerField.text.Length; 
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }
}
