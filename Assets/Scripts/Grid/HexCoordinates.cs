using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class HexCoordinates : NetworkBehaviour
{
    
    public static float xOffset = 2f, yOffset = 1,zOffset = 1.73f; // gridin buyukluklerı
    [Header("Offset coordinates")]
    [SerializeField] Vector3Int offsetCoordinates; // index
    public override void OnStartClient()
    {
        base.OnStartClient();
    }
    private void Awake() {
        offsetCoordinates = ConverPositionToOffset(transform.position);
    }
    internal Vector3Int GetHexCoords()
        => offsetCoordinates;
    public static Vector3Int ConverPositionToOffset(Vector3 position) // posizyonuna ve grıd buyuklufune gore grıde ındex verıyoruz
    {
        int x = Mathf.CeilToInt(position.x/xOffset);
        int y = Mathf.RoundToInt(position.y/yOffset);
        int z = Mathf.RoundToInt(position.z/zOffset);
        return new Vector3Int(x,y,z);
    }
}
