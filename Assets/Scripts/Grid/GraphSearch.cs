using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch
{
    public static SightResult GetRangeSightDistance(Vector3Int startPoint, int movementPoints)
    {
        int movementPointsTemp = movementPoints;
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int?> allNodes = new Dictionary<Vector3Int, Vector3Int?>();
        allNodes.Add(startPoint, null);
        nodesToVisitQueue.Enqueue(startPoint);  // startpoint ekleniyor 
        costSoFar.Add(startPoint, 0);            // suana kadarkı malıyete startpoint eklenıyor

        while (nodesToVisitQueue.Count > 0)      // 
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();
            foreach (Vector3Int neighbourPosition in HexGrid.Instance.GetNeighboursFor(currentNode))
            {
                int nodeCost = 1;
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;
                    
                if (newCost <= movementPoints)
                {
                    if (!allNodes.ContainsKey(neighbourPosition))
                    {
                        allNodes[neighbourPosition] = currentNode;
                        costSoFar[neighbourPosition] = newCost;
                        nodesToVisitQueue.Enqueue(neighbourPosition);
                    }
                    else if (costSoFar[neighbourPosition] > newCost)
                    {
                        costSoFar[neighbourPosition] = newCost;
                        allNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        return new SightResult{sightNodesDict = allNodes};
    }
    public static BFSResult BfsGetAllRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        int movementPointsTemp = movementPoints;
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, Vector3Int?> enemiesNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int?> allNodes = new Dictionary<Vector3Int, Vector3Int?>();
        allNodes.Add(startPoint, null);
        nodesToVisitQueue.Enqueue(startPoint);  // startpoint ekleniyor 
        costSoFar.Add(startPoint, 0);            // suana kadarkı malıyete startpoint eklenıyor
        visitedNodes.Add(startPoint, null);      // visited nodes e start point eklenıyor

        while (nodesToVisitQueue.Count > 0)      // 
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();
            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if (hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                    continue;
                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;
                if (newCost <= movementPoints)
                {
                    if (!allNodes.ContainsKey(neighbourPosition))
                    {
                        allNodes[neighbourPosition] = currentNode;
                        costSoFar[neighbourPosition] = newCost;
                        nodesToVisitQueue.Enqueue(neighbourPosition);
                    }
                    else if (costSoFar[neighbourPosition] > newCost)
                    {
                        costSoFar[neighbourPosition] = newCost;
                        allNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        return new BFSResult{allNodesDict2 = allNodes};
    }
    public static BFSResult BsfGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {





        int movementPointsTemp = movementPoints;
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, Vector3Int?> enemiesNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int?> allNodes = new Dictionary<Vector3Int, Vector3Int?>();
        allNodes.Add(startPoint, null);
        nodesToVisitQueue.Enqueue(startPoint);  // startpoint ekleniyor 
        costSoFar.Add(startPoint, 0);            // suana kadarkı malıyete startpoint eklenıyor
        visitedNodes.Add(startPoint, null);      // visited nodes e start point eklenıyor

        while (nodesToVisitQueue.Count > 0)      // 
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();
            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if (hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                    continue;
                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;
        
                    if (hexGrid.GetTileAt(neighbourPosition).IsEnemy())
                    {
                        continue;   
                    }
                
                if (newCost <= movementPoints)
                {
                    if (!allNodes.ContainsKey(neighbourPosition))
                    {
                        allNodes[neighbourPosition] = currentNode;
                        costSoFar[neighbourPosition] = newCost;
                        nodesToVisitQueue.Enqueue(neighbourPosition);
                    }
                    else if (costSoFar[neighbourPosition] > newCost)
                    {
                        costSoFar[neighbourPosition] = newCost;
                        allNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        Queue<Vector3Int> nodesToVisitQueue1 = new Queue<Vector3Int>();
        Dictionary<Vector3Int, int> costSoFar1 = new Dictionary<Vector3Int, int>();
        nodesToVisitQueue1.Enqueue(startPoint);
        movementPoints = movementPointsTemp;
        costSoFar1.Add(startPoint, 0);
        while (nodesToVisitQueue1.Count > 0)      // 
        {
            Vector3Int currentNode = nodesToVisitQueue1.Dequeue();
            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if(hexGrid.GetTileAt(currentNode).isVisible)
                {
                    if(hexGrid.GetTileAt(currentNode).IsEnemy())
                    {
                        continue;
                    }
                }
                if (hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                    continue;
                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar1[currentNode];
                int newCost = currentCost + nodeCost;
                if (newCost <= movementPoints)
                {
                    if(hexGrid.GetTileAt(neighbourPosition).isVisible)
                    {
                        if(!enemiesNodes.ContainsKey(neighbourPosition))
                        {
                            if (hexGrid.GetTileAt(neighbourPosition).IsEnemy())
                            {
                                enemiesNodes[neighbourPosition] = currentNode;
                                continue;
                            }
                        }
                            if (hexGrid.GetTileAt(neighbourPosition).IsEnemy())
                            {
                                // enemiesNodes[neighbourPosition] = currentNode;
                                continue;
                            }
                    }                        
                    if (!visitedNodes.ContainsKey(neighbourPosition))
                    {
                        visitedNodes[neighbourPosition] = currentNode;
                        costSoFar1[neighbourPosition] = newCost;
                        nodesToVisitQueue1.Enqueue(neighbourPosition);
                    }
                    else if (costSoFar1[neighbourPosition] > newCost)
                    {
                        costSoFar1[neighbourPosition] = newCost;
                        visitedNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        // foreach (var item in allNodes)
        // {
        //         Debug.Log(item.Key + " " + item.Value);
        // }
            foreach (var item in enemiesNodes)
            {
                // Debug.Log(item.Key + "  " + item.Value);
                allNodes.Add(item.Key, item.Value);
            }
            Debug.Log(allNodes.Keys.Count);
            foreach (var item in allNodes)
            {
                Debug.Log(item.Key + " " + item.Value);
            }




            movementPointsTemp = movementPoints;
        Dictionary<Vector3Int, Vector3Int?> visitedNodes2 = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar2 = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue2 = new Queue<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int?> allNodes2 = new Dictionary<Vector3Int, Vector3Int?>();
        allNodes2.Add(startPoint, null);
        nodesToVisitQueue2.Enqueue(startPoint);  // startpoint ekleniyor 
        costSoFar2.Add(startPoint, 0);            // suana kadarkı malıyete startpoint eklenıyor
        visitedNodes2.Add(startPoint, null);      // visited nodes e start point eklenıyor

        while (nodesToVisitQueue2.Count > 0)      // 
        {
            Vector3Int currentNode = nodesToVisitQueue2.Dequeue();
            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if (hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                    continue;
                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar2[currentNode];
                int newCost = currentCost + nodeCost;
                if (newCost <= movementPoints)
                {
                    if (!allNodes2.ContainsKey(neighbourPosition))
                    {
                        allNodes2[neighbourPosition] = currentNode;
                        costSoFar2[neighbourPosition] = newCost;
                        nodesToVisitQueue2.Enqueue(neighbourPosition);
                    }
                    else if (costSoFar2[neighbourPosition] > newCost)
                    {
                        costSoFar2[neighbourPosition] = newCost;
                        allNodes2[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        return new BFSResult { visitedNodesDict = visitedNodes, enemiesNodesDict = enemiesNodes, allNodesDict = allNodes , costDict = costSoFar1,startPoint = startPoint,allNodesDict2 = allNodes2};
    }
    public static BFSResult GetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int?> allNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, Vector3Int?> enemiesNodes = new Dictionary<Vector3Int, Vector3Int?>();
        enemiesNodes.Add(startPoint, null);
        allNodes.Add(startPoint, null);
        nodesToVisitQueue.Enqueue(startPoint);  // startpoint ekleniyor 
        costSoFar.Add(startPoint, 0);            // suana kadarkı malıyete startpoint eklenıyor

        while (nodesToVisitQueue.Count > 0)      // 
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();
            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if (hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                    continue;
                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;
                //    if (hexGrid.GetTileAt(neighbourPosition).IsEnemy())
                //         {
                //             enemiesNodes[neighbourPosition] = currentNode;
                //             continue;
                //         
                if (newCost <= movementPoints)
                {
                    if (!allNodes.ContainsKey(neighbourPosition))
                    {
                        allNodes[neighbourPosition] = currentNode;
                        costSoFar[neighbourPosition] = newCost;
                        nodesToVisitQueue.Enqueue(neighbourPosition);
                    }
                    else if (costSoFar[neighbourPosition] > newCost)
                    {
                        costSoFar[neighbourPosition] = newCost;
                        allNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        allNodes.Remove(startPoint);
        foreach (var item in allNodes)
        {
            if(HexGrid.Instance.GetTileAt (item.Key).IsEnemy())
                enemiesNodes.Add(item.Key,item.Value);
            enemiesNodes.Remove(startPoint);
        }
        return new BFSResult{rangeNodesDict = enemiesNodes};
    }
    public static List<Vector3Int> GetCloseseteHex(HexGrid hexGrid,Vector3Int current,Dictionary<Vector3Int, Vector3Int?> allNodesDict,int rangePoint)
    {
        List<Vector3Int> list = GneratePathBFS(current, allNodesDict);
        for (int i = 0; i < rangePoint; i++)
        {
            list.RemoveAt(list.Count - 1);
            
        }
        return list;
    }
    
    public static List<Vector3Int> GneratePathBFS(Vector3Int current, Dictionary<Vector3Int, Vector3Int?> visitedNodesDict)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);
        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
        
        while (visitedNodesDict[current] != null)
        {
            path.Add(visitedNodesDict[current].Value);
            current = visitedNodesDict[current].Value;
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
    // public static List<Vector3Int> GneratePathBFS(Vector3Int current, Dictionary<Vector3Int, Vector3Int?> allNodesDict,Dictionary<Vector3Int, Vector3Int?> visitedNodesDict)
    // {
    //     List<Vector3Int> path = new List<Vector3Int>();
    //     path.Add(current);
    //     HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
    //     while (allNodesDict[current] != null)
    //     {
    //         // Debug.Log(allNodesDict[current] + " qwe");
    //         if(hexGrid.GetTileAt ((Vector3Int)allNodesDict[current]).isVisible)
    //         {
    //             if(allNodesDict[current] != null)
    //             {
    //                 path.Add(allNodesDict[current].Value);
    //                 current = allNodesDict[current].Value;
    //             }
    //         }
    //         else
    //         {
    //             path.Add(allNodesDict[current].Value);
    //             current = allNodesDict[current].Value;
    //         }
    //     }
    //     path.Reverse();
    //     return path.Skip(1).ToList();
    // }
}
public struct BFSResult
{
    public Vector3Int startPoint;
    public Dictionary<Vector3Int, Vector3Int?> visitedNodesDict;
    public Dictionary<Vector3Int, Vector3Int?> enemiesNodesDict;
    public Dictionary<Vector3Int, Vector3Int?> allNodesDict;
    public Dictionary<Vector3Int, Vector3Int?> allNodesDict2;
    public Dictionary<Vector3Int, Vector3Int?> rangeNodesDict;
    public Dictionary<Vector3Int, int> costDict;
    public List<Vector3Int> GetPathEnemyGridForMovement(Vector3Int destination)
    {
        if (allNodesDict.ContainsKey(destination) == false)
            return new List<Vector3Int>();
        return GraphSearch.GneratePathBFS(destination, allNodesDict);
    }
    public int GetCost(Vector3Int pos)
    {
        if(enemiesNodesDict.ContainsKey(pos))
        {
            // costDict.Add(pos,costDict[GraphSearch.GetCloseseteHex(HexGrid.Instance,startPoint,allNodesDict,1)[GraphSearch.GetCloseseteHex(HexGrid.Instance,startPoint,allNodesDict,1).Count-1]]);

            return costDict[pos];
        }
            
        return costDict[pos];
    }
    public List<Vector3Int> GetPathEnemyGrid(Vector3Int destination, out Vector3Int? enemyGrid , int rangePoint = 1)
    {
        if (allNodesDict.ContainsKey(destination) == false)
        {
            enemyGrid = null;
            return new List<Vector3Int>();
        }
        enemyGrid = GraphSearch.GneratePathBFS(destination,allNodesDict)[0];
        return GraphSearch.GetCloseseteHex(HexGrid.Instance, destination, allNodesDict,rangePoint);
    }
    public List<Vector3Int> GetPathTo(Vector3Int destination)
    {
        if (visitedNodesDict.ContainsKey(destination) == false)
            return new List<Vector3Int>();
        return GraphSearch.GneratePathBFS(destination ,visitedNodesDict);
    }
    public Hex GetLastIndexClosestHex(int range)
    {
        List<Vector3Int> closestList =  GraphSearch.GetCloseseteHex(HexGrid.Instance,startPoint,allNodesDict,range);
        return HexGrid.Instance.GetTileAt (closestList[closestList.Count-1]);
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
public struct SightResult
{
    public Dictionary<Vector3Int, Vector3Int?> sightNodesDict;
    public IEnumerable<Vector3Int> GetRangeSight()
        => sightNodesDict.Keys;
}
public static class DrawArrow
{
    public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay(pos, direction);
       
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }
 
    public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);
       
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }
 
    public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(pos, direction);
       
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        Debug.DrawRay(pos + direction, right * arrowHeadLength);
        Debug.DrawRay(pos + direction, left * arrowHeadLength);
    }
    public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(pos, direction, color);
       
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
        Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
    }
}