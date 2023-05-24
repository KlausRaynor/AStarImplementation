using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

/// <summary>
/// PathFinding script - A* Algorithm
/// with Heap implementation
/// </summary>
public class PathGenerator : MonoBehaviour
{
    [SerializeField] private Transform _seeker, _target;
    private Grid _grid;
    
    void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
       // Stopwatch sw = new Stopwatch();
        //sw.Start();
        Node startNode = _grid.NodeFromWorldPoint(startPos);
        Node targetNode = _grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                //   sw.Stop();
              //  print("Path Found in: " + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, targetNode);
                yield break;
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
                    
                    if(!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        yield return null;
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode._parent;
        }

        path.Reverse();

        _grid.path = path;
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
