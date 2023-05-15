using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch
{
    public static BFSResult BsfGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        GameObject arrow = GameObject.Find("Arrow");
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
                if(hexGrid.GetTileAt(currentNode).IsEnemy())
                    continue;
                if (hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                    continue;
                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar1[currentNode];
                int newCost = currentCost + nodeCost;
                if (newCost <= movementPoints)
                {
                    if(!enemiesNodes.ContainsKey(neighbourPosition))
                    {

                        if (hexGrid.GetTileAt(neighbourPosition).IsEnemy())
                        {
                            enemiesNodes[neighbourPosition] = currentNode;
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
            foreach (var item in enemiesNodes)
            {
                // Debug.Log(item.Key + "  " + item.Value);
                allNodes.Add(item.Key, item.Value);
            }
        return new BFSResult { visitedNodesDict = visitedNodes, enemiesNodesDict = enemiesNodes, allNodesDict = allNodes };
    }
    public static List<Vector3Int> GetCloseseteHex(
        HexGrid hexGrid,
        Vector3Int current,
        Dictionary<Vector3Int, Vector3Int?> allNodesDict,
        Dictionary<Vector3Int, Vector3Int?> enemyNodesDict)
    {
        List<Vector3Int> list = GneratePathBFS(current, allNodesDict);
        list.RemoveAt(list.Count - 1);
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

}
public struct BFSResult
{
    
    public Dictionary<Vector3Int, Vector3Int?> visitedNodesDict;
    public Dictionary<Vector3Int, Vector3Int?> enemiesNodesDict;
    public Dictionary<Vector3Int, Vector3Int?> allNodesDict;
    public List<Vector3Int> GetPathEnemyGridForMovement(Vector3Int destination)
    {
        if (allNodesDict.ContainsKey(destination) == false)
            return new List<Vector3Int>();
        return GraphSearch.GneratePathBFS(destination, allNodesDict);
    }
    public List<Vector3Int> GetPathEnemyGrid(Vector3Int destination, out Vector3Int? enemyGrid)
    {
        if (allNodesDict.ContainsKey(destination) == false)
        {
            enemyGrid = null;
            return new List<Vector3Int>();
        }
        enemyGrid = GraphSearch.GneratePathBFS(destination, allNodesDict)[0];
        return GraphSearch.GetCloseseteHex(HexGrid.Instance, destination, allNodesDict,enemiesNodesDict);
    }
    public List<Vector3Int> GetPathTo(Vector3Int destination)
    {
        if (visitedNodesDict.ContainsKey(destination) == false)
            return new List<Vector3Int>();
        return GraphSearch.GneratePathBFS(destination, visitedNodesDict);
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
