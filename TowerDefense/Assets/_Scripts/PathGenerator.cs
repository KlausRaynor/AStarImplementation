using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PathFinding script - A* Algorithm
/// with Heap implementation
/// </summary>
public class PathGenerator : MonoBehaviour
{
    PathRequestManager _requestManager;
    private Grid _grid;
    
    void Awake()
    {
        _requestManager = GetComponent<PathRequestManager>();
        _grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }
    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = Array.Empty<Vector3>();
        bool pathSuccess = false;
        Node startNode = _grid.NodeFromWorldPoint(startPos);
        Node targetNode = _grid.NodeFromWorldPoint(targetPos);
        
        if (startNode._walkable && targetNode._walkable)
        {
            Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (var neighbor in _grid.GetNeighbors(currentNode))
                {
                    if (!neighbor._walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor =
                        currentNode._gCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor._gCost || !openSet.Contains(neighbor))
                    {
                        neighbor._gCost = newMovementCostToNeighbor;
                        neighbor._hCost = GetDistance(neighbor, targetNode);
                        neighbor._parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
        }

        yield return null;
        if (pathSuccess)
        {
            waypoints =   RetracePath(startNode, targetNode);
        }
        _requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode._parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1]._gridX - path[i]._gridX,
                path[i - 1]._gridY - path[i]._gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i]._worldPos);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Math.Abs(nodeA._gridX - nodeB._gridX);
        int distanceY = Math.Abs(nodeA._gridY - nodeB._gridY);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
