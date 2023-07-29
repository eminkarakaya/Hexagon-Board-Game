using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SelectCiv : MonoBehaviour
{
    public TMP_Text currentTeamText;
    public Button button;
    public int selectedCivDataIndex;
    public Image selectedImage;
    public List<CivUI> civUIs;
    public Transform parent;
    public TMP_InputField nameInputField;
    public int team;
    
    private void Start() {
        SetTeam(1);
        nameInputField.text = "Player" + Random.Range(0,1000);
        for (int i = 0; i < civUIs.Count; i++)
        {
            var obj = Instantiate(civUIs[i].gameObject,parent);            
        }
    }
    
    public void SetTeam(int team)
    {
        this.team = team;
        currentTeamText.text = "Team " + team.ToString();
    }
}
