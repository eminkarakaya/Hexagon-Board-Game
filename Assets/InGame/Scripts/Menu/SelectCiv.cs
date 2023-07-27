using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SelectCiv : MonoBehaviour
{
    public Button button;
    public int selectedCivDataIndex;
    public Image selectedImage;
    public List<CivUI> civUIs;
    public Transform parent;
    public TMP_InputField nameInputField;
    
    private void Start() {
        nameInputField.text = "Player" + Random.Range(0,1000);
        for (int i = 0; i < civUIs.Count; i++)
        {
            var obj = Instantiate(civUIs[i].gameObject,parent);            
        }
        
      
    }

}
