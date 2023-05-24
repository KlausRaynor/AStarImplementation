using System.Collections;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool _walkable;
    public Vector3 _worldPos;
    public Node _parent;
    public int _gridX;
    public int _gridY;
    public int _gCost;
    public int _hCost;
    private int _heapIndex;

    public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        _walkable = walkable;
        _worldPos = worldPos;
        _gridX = gridX;
        _gridY = gridY;
    }

    public int FCost => _gCost + _hCost;

    public int HeapIndex
    {
        get
        {
            return _heapIndex;
        } 
        set
        {
            _heapIndex = value;
        }
    }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
            compare = _hCost.CompareTo(other._hCost);
        return -compare;
    }
    
}
