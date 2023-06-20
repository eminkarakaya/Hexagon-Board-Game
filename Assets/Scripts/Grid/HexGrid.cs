using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class HexGrid : NetworkBehaviour
{
    public Dictionary<Vector3Int,Hex> hexTileDict = new Dictionary<Vector3Int, Hex>();
    Dictionary<Vector3Int , List<Vector3Int>> hexTileNeighboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();
    private void Start() {
        
        foreach (Hex hex in FindObjectsOfType<Hex>()) // dic e hexlerı atıyoruz
        {
            hexTileDict[hex.HexCoordinates] = hex;
        }
    }
    public void CloseVisible()
    {
        foreach (var item in hexTileDict.Values)
        {
            item.isVisible = false;
            if(item.Unit != null)
            {
                if(item.Unit.Side == Side.Me)
                    continue;
                    item.Unit.GetComponent<Sight>().HideSight(item);
            }
            else if(item.Settler != null)
            {
                if(item.Settler.Side == Side.Me)
                    continue;
                    item.Settler.GetComponent<Sight>().HideSight(item);

            }
        }
    }
    internal Vector3Int GetClosestHex(Vector3 worldPosition)
    {
        worldPosition.y = 0;
        return HexCoordinates.ConverPositionToOffset(worldPosition);
    }
    public Hex GetTileAt(Vector3Int hexCoordinates) // koordinata gore hex gonduruyo
    {
        Hex result = null;
        hexTileDict.TryGetValue(hexCoordinates,out result);
        return result;
    }
    public List<Vector3Int> GetNeighboursFor(Vector3Int hexCoordinates)
    {
        if(hexTileDict.ContainsKey(hexCoordinates) == false)
            return new List<Vector3Int>();
        if(hexTileNeighboursDict.ContainsKey(hexCoordinates))
            return hexTileNeighboursDict[hexCoordinates];
        hexTileNeighboursDict.Add(hexCoordinates,new List<Vector3Int>());
        foreach (var direction in Direction.GetDirectionList(hexCoordinates.z))
        {
            if(hexTileDict.ContainsKey(hexCoordinates + direction))
            {
                hexTileNeighboursDict[hexCoordinates].Add(hexCoordinates + direction);
            }
        }
        return hexTileNeighboursDict[hexCoordinates];
    }
}
public static class Direction
{
    public static List<Vector3Int> directionsOffsetOdd = new List<Vector3Int>
    {
        new Vector3Int(-1,0,1), // north1
        new Vector3Int(0,0,1), // north2
        new Vector3Int(1,0,0),// east
        new Vector3Int(0,0,-1),// south2
        new Vector3Int(-1,0,-1),// south1
        new Vector3Int(-1,0,0)// west
    };
    public static List<Vector3Int> directionsOffsetEven = new List<Vector3Int>
    {
        new Vector3Int(0,0,1), //north1
        new Vector3Int(1,0,1), //north2
        new Vector3Int(1,0,0), //east
        new Vector3Int(1,0,-1), //south2
        new Vector3Int(0,0,-1), //south1
        new Vector3Int(-1,0,0) //west
    };
    public static List<Vector3Int> GetDirectionList(int z)
        => z % 2 == 0 ? directionsOffsetEven : directionsOffsetOdd;
}