using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
[System.Serializable]
public struct ResourceData{
    public GameObject prefab;
    public Sprite image;
}
public class HexGrid : NetworkBehaviour
{

    [SerializeField] private ResourceData Oil,Coal,Iron;
    public Dictionary<Vector3Int,Hex> hexTileDict = new Dictionary<Vector3Int, Hex>();
    Dictionary<Vector3Int , List<Vector3Int>> hexTileNeighboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();
    Dictionary<Hex , List<Hex>> hexTileNeighboursDictHex = new Dictionary<Hex, List<Hex>>();
    private void Start() {
        
        foreach (Hex hex in FindObjectsOfType<Hex>()) // dic e hexlerı atıyoruz
        {
            hexTileDict[hex.HexCoordinates] = hex;
        }
        SetCoastAllUnits();
    }

    public GameObject GetBuildingPrefab(ResourceType type, out Sprite image)
    {
        if(type == ResourceType.Oil)
        {
            image = Oil.image;
            return Oil.prefab;
        }
        else if(type == ResourceType.Coal)
        {
            image = Coal.image;
            return Coal.prefab;
        }
        else if(type == ResourceType.Iron)
        {
            image = Iron.image;
            return Iron.prefab;
        }
        image = null;
        return null;
    }

    public List<Hex> GetHarborHex()
    {
        List<Hex> list = new List<Hex>();
        foreach (var item in hexTileDict.Values)
        {
            if(item.isCoast  && item.Building == null)
            {
                list.Add(item);
            }
        }
        return list;
    }

    public void CloseVisible()
    {
        foreach (var item in hexTileDict.Values)
        {
            item.isVisible = false;
            if(item.Ship != null)
            {

                if(item.Ship.Side == Side.Me ||item.Ship.Side == Side.Ally)
                    continue;
                    item.Ship.GetComponent<Vision>().HideVision(item);
            }
            if(item.Unit != null)
            {
                if(item.Unit.Side == Side.Me || item.Unit.Side == Side.Ally)
                    continue;
                    item.Unit.GetComponent<Vision>().HideVision(item);
            }
            else if(item.Settler != null)
            {
                if(item.Settler.Side == Side.Me || item.Settler.Side == Side.Ally)
                    continue;
                    item.Settler.GetComponent<Vision>().HideVision(item);

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
    
    public List<Hex> GetNeighboursForHex(Hex hex)
    {
        if(hexTileDict.ContainsKey(hex.HexCoordinates) == false)
            return new List<Hex>();
        if(hexTileNeighboursDictHex.ContainsKey(hex))
            return hexTileNeighboursDictHex[hex];
        hexTileNeighboursDictHex.Add(hex,new List<Hex>());
        foreach (var direction in Direction.GetDirectionList(hex.HexCoordinates.z))
        {
            if(hexTileDict.ContainsKey(hex.HexCoordinates + direction))
            {
                hexTileNeighboursDictHex[hex].Add(GetTileAt (hex.HexCoordinates + direction));
            }
            else
            {
                hexTileNeighboursDictHex[hex].Add(null);

            }
        }
        return hexTileNeighboursDictHex[hex];
    }

    private void SetCoastAllUnits()
    {
        foreach (var item in hexTileDict)
        {
            if(GetTileAt(item.Key).IsWater()) 
                continue;
            List<Vector3Int> hexTileNeighboursDict = new List<Vector3Int>();
            hexTileNeighboursDict = GetNeighboursFor(item.Key);
            foreach (var item1  in hexTileNeighboursDict)
            {
                if(GetTileAt(item1).IsWater())
                {
                    GetTileAt(item.Key).isCoast = true;
                }
            }
        }
    }
    public void DrawBorders(Hex hex,Vector3Int currentHexPos)
    {
        Hex currentHex = GetTileAt(currentHexPos);
        List<Hex> hexes =  GetNeighboursForHex(hex);
        for (int i = 0; i < hexes.Count; i++)
        {
            
            if(hexes[i] == null || !hexes[i].isReachable)
            {
                if(hexes[i] == currentHex) continue;
                hex.OpenEdge(i,true);
            }
        }
    }
    public void DeleteBorders(Hex hex,Hex currentHex)
    {
        List<Hex> hexes =  GetNeighboursForHex(hex);
        for (int i = 0; i < hexes.Count; i++)
        {
            if(hexes[i] == null || !hexes[i].isReachable)
            {
                hex.OpenEdge(i,false);
            }
        }
    }
}
public static class Direction
{
    public static Vector3Int GetNorth(int z)
    {
        if(z% 2 == 0)
        {
            return new Vector3Int(0,0,1);
        }
        else
        {
            return new Vector3Int(-1,0,1);
        }
    }
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