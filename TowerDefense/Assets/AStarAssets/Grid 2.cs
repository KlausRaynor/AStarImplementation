using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private LayerMask _unwalkableMask;
    [SerializeField] private Vector2 _gridWorldSize;
    [SerializeField] private float _nodeRadius;
    [SerializeField] private Transform _player;
    private Node[,] grid;
    private float _nodeDiameter;
    private int gridSizeX, gridSizeY;
    public bool onlyDisplayPathGizmos;
    public int MaxSize => gridSizeX * gridSizeY;
    void Awake()
    {
        _nodeDiameter = _nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeDiameter);
        gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);

        CreateGrid();
    }
    

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * _gridWorldSize.x / 2 
            - Vector3.forward * _gridWorldSize.y / 2;
        for (var x = 0; x < gridSizeX; x++)
        {
            for (var y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + _nodeRadius)
                                                     + Vector3.forward * (y * _nodeDiameter + _nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, _nodeRadius, _unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }
/// <summary>
/// takes given node and returns a node list of all its neighbors. 
/// </summary>
/// <param name="node"></param>
/// <returns></returns>
    public List<Node> GetNeighbors(Node node)
    {
        var neighbors = new List<Node>();

        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                var checkX = node._gridX + x;
                var checkY = node._gridY + y;

                if (checkX >= 0 && checkX < gridSizeX &&
                    checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }
/// <summary>
/// Match Node with given Vector3 pos in world
/// </summary>
/// <param name="worldPosition"></param>
/// <returns>grid[x,y]</returns>
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        var percentX = (worldPosition.x / _gridWorldSize.x) + 0.5f; //optimized
        var percentY = (worldPosition.z / _gridWorldSize.y) + 0.5f;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        var x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        var y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x,y];

    }

public List<Node> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));
        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(n._worldPos, Vector3.one * (_nodeDiameter - 0.1f));
                }
            }
        } else
        {
            if (grid != null)
            {
                Node playerNode = NodeFromWorldPoint(_player.position);
                foreach(Node n in grid)
                {
                    Gizmos.color = (n._walkable) ? Color.white : Color.red;

                    if (playerNode == n)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    Gizmos.DrawCube(n._worldPos, Vector3.one * (_nodeDiameter - 0.1f));
                }
            }
        }
        
    }
}
