using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch 
{
    public static BFSResult BsfGetRange(HexGrid hexGrid,Vector3Int startPoint, int movementPoints)
    {
        int movementPointsTemp = movementPoints;
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>(); 
        Dictionary<Vector3Int, Vector3Int?> enemiesNodes = new Dictionary<Vector3Int, Vector3Int?>(); 
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>(); 

        nodesToVisitQueue.Enqueue(startPoint);  // startpoint ekleniyor 
        costSoFar.Add(startPoint,0);            // suana kadarkı malıyete startpoint eklenıyor
        visitedNodes.Add(startPoint,null);      // visited nodes e start point eklenıyor
        
        while(nodesToVisitQueue.Count > 0)      // 
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();       
            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if(hexGrid.GetTileAt(neighbourPosition).IsObstacle() ) 
                    continue;
                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;
                if(newCost <= movementPoints)
                {
                    if(hexGrid.GetTileAt(neighbourPosition).IsEnemy())
                    {
                        enemiesNodes[neighbourPosition] = currentNode;
                        continue;
                    }
                    if(!visitedNodes.ContainsKey(neighbourPosition))
                    {
                        visitedNodes[neighbourPosition] = currentNode;
                        costSoFar[neighbourPosition] = newCost;
                        nodesToVisitQueue.Enqueue(neighbourPosition);
                    }
                    else if(costSoFar[neighbourPosition] >  newCost)
                    {
                        costSoFar[neighbourPosition] = newCost;
                        visitedNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        Queue<Vector3Int> nodesToVisitQueue1 = new Queue<Vector3Int>(); 
        Dictionary<Vector3Int, Vector3Int?> allNodes = new Dictionary<Vector3Int, Vector3Int?>(); 
        Dictionary<Vector3Int, int> costSoFar1 = new Dictionary<Vector3Int, int>();
        allNodes.Add(startPoint,null);
        nodesToVisitQueue1.Enqueue(startPoint);
        movementPoints = movementPointsTemp;
        costSoFar1.Add(startPoint,0);  
        while(nodesToVisitQueue1.Count > 0)      // 
        {
            Vector3Int currentNode = nodesToVisitQueue1.Dequeue();       
            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if(hexGrid.GetTileAt(neighbourPosition).IsObstacle() ) 
                    continue;
                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar1[currentNode];
                int newCost = currentCost + nodeCost;
                if(newCost <= movementPoints)
                {
                    
                    if(!allNodes.ContainsKey(neighbourPosition))
                    {
                        allNodes[neighbourPosition] = currentNode;
                        costSoFar1[neighbourPosition] = newCost;
                        nodesToVisitQueue1.Enqueue(neighbourPosition);
                    }
                    else if(costSoFar1[neighbourPosition] >  newCost)
                    {
                        costSoFar1[neighbourPosition] = newCost;
                        allNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        return new BFSResult{visitedNodesDict = visitedNodes,enemiesNodesDict = enemiesNodes,allNodesDict = allNodes};
    }
    public static List<Vector3Int> GetCloseseteHex(Dictionary<Vector3Int,Vector3Int?> visitedNodesDict,Vector3Int current)
    {
        List<Vector3Int> list = GneratePathBFS(current,visitedNodesDict);
        for (int i = 0; i < list.Count; i++)
        {
            // Debug.Log(list[i],HexGrid.Instance.GetTileAt (list[i]));
        }
        list.RemoveAt(list.Count-1);
        return list;
    }
    public static List<Vector3Int> GneratePathBFS(Vector3Int current,Dictionary<Vector3Int,Vector3Int?> visitedNodesDict)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);
        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
        while(visitedNodesDict[current] != null)
        {
            // Debug.Log(visitedNodesDict[current].Value + "  path " ,  hexGrid.GetTileAt(current));
            path.Add(visitedNodesDict[current].Value);
            current = visitedNodesDict[current].Value;
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
    
}
public struct BFSResult
{
    public Dictionary<Vector3Int,Vector3Int?> visitedNodesDict;
    public Dictionary<Vector3Int,Vector3Int?> enemiesNodesDict;
    public Dictionary<Vector3Int,Vector3Int?> allNodesDict;
    public List<Vector3Int> GetPathEnemyGrid(Vector3Int destination)
    {
        if(allNodesDict.ContainsKey(destination) == false)
            return new List<Vector3Int>();
        return GraphSearch.GneratePathBFS(destination,allNodesDict);
    }
    public List<Vector3Int> GetPathTo(Vector3Int destination)
    {
        if(visitedNodesDict.ContainsKey(destination) == false)
            return new List<Vector3Int>();
        return GraphSearch.GneratePathBFS(destination,visitedNodesDict);
    }
    public bool IsHecPositionInRange(Vector3Int position)
    {
        return allNodesDict.ContainsKey(position);
    }
    public IEnumerable<Vector3Int> GetRangePositions()
        => visitedNodesDict.Keys;
    public IEnumerable<Vector3Int> GetRangeEnemiesPositions()
        => enemiesNodesDict.Keys;
    public IEnumerable<Vector3Int> GetRangeAllPositions()
        => allNodesDict.Keys;
    
}
