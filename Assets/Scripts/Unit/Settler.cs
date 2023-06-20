using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Settler : NetworkBehaviour , IMovable , ISelectable 
{
    public Hex Hex { get => _hex; set{_hex = value;} }
    [SerializeField] private Hex _hex;
    public Movement Movement { get; set; }
    public MovementSystem Result { get; set; }
    public Vector3Int Position { get; set; }
    [SerializeField] Canvas canvas;
    public Canvas Canvas { get => canvas; set{canvas = value;} }
    public Outline outline { get; set; }
    public Side Side { get =>_side; set{_side = value; Debug.Log(_side);} }
    [SerializeField] private Side _side;
    private void Awake() {
        
    }
    private void Start() {
        outline = GetComponent<Outline>();
        Movement = GetComponent<Movement>();
        Result = new SettlerMovableResult(this);
    }
    public void CloseCanvas()
    {
        Canvas.gameObject.SetActive(false);
    }

    public void Deselect()
    {
        Result.HideRange(this,Movement);
        
    }
    

    public void LeftClick()
    {
        outline.enabled = true;
        // Result.ShowRange(this,Movement);
        Result.ShowRange(this,Movement);
    }

    public void OpenCanvas()
    {
        Canvas.gameObject.SetActive(true);
        
    }

    public void RightClick(Hex selectedHex)
    {
        HexGrid hexGrid =FindObjectOfType<HexGrid>();
        Result.ShowPath(selectedHex.HexCoordinates,hexGrid);
        // Result = new UnitMovableResult(this);
        Result.CalculateRange(this,hexGrid);
        Result.ShowPath(selectedHex.HexCoordinates,hexGrid);
    }

    public void RightClick2(Hex selectedHex)
    {
        Result.MoveUnit(Movement,FindObjectOfType<HexGrid>(),selectedHex);
    }
}
