using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundGenerator : MonoBehaviour
{

    [SerializeField] private GameObject _wallNode;
    [SerializeField] private GameObject _pathNode;
    [SerializeField, Range(2, 100)] private int _width, _depth;
    [SerializeField] private Transform _camera;
    [SerializeField] private Material _startMat, _endMat, _pathMat;
    public static GroundGenerator Instance;
    private GameObject _startNode, _endNode;
    private static List<GameObject> nodeList;
    private static GameObject[,] _nodeArray;
    private int _scaledDepth, _scaledWidth;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        nodeList = new List<GameObject>();
        
        _scaledDepth = _depth *  (int)_wallNode.transform.localScale.z;
        _scaledWidth = _width * (int)_wallNode.transform.localScale.x;
        _nodeArray = new GameObject[_width,_depth];
        
        GenerateGrid();
        GenerateStartAndEndNode();
        GenerateObstacles();
      //  GeneratePath();
        _camera.transform.position =
            new Vector3((float)_scaledDepth / 2 - 2f, (float)(_scaledDepth + _scaledWidth) / 2 - 2f, (float)_scaledDepth / 2 - 2f);
        PrintList();
    }
    
    /// <summary>
    /// Generates the grid of squares. Depth and width scaled with Prefab dimensions (default is 4x1x4).
    /// </summary>

    public void GenerateGrid()
    {
        for (var x = 0; x < _scaledWidth; x++)
        {
            for (var z = 0; z < _scaledDepth;z++)
            {
                var nodePos = new Vector3(x, 0, z);
                
                var newNode = Instantiate(_wallNode, nodePos, Quaternion.identity);
                newNode.name = $"Node: {x}, {z}";
                newNode.transform.SetParent(gameObject.transform);
                nodeList.Add(newNode);
                z += 4;
            }

            x += 4;
        }
    }
    
    /// <summary> *** GenerateStartAndEndNode ***
    /// Grabs 1 random node from first 2 rows and assigns it as a startNode
    /// Grabs 1 random node from last 2 rows and assigns it as an endNode
    /// Replaces Material with Start/End Node mat.
    /// Replaces tag with Start/End Node tag.
    /// </summary>
    public void GenerateStartAndEndNode()
    {
        int startNodeEndRange = (_scaledWidth / 5) * 2;
        int endNodeStartRange = nodeList.Count - startNodeEndRange;
        
        _startNode = nodeList.ElementAt(Random.Range(0, startNodeEndRange));
        _endNode = nodeList.ElementAt(Random.Range(endNodeStartRange, nodeList.Count));

        MeshRenderer startNodeMR = _startNode.GetComponent<MeshRenderer>();
        MeshRenderer endNodeMR = _endNode.GetComponent<MeshRenderer>();
        _startNode.name = "Start Node";
        _startNode.tag = "Start Node";
        _endNode.name = "End Node";
        _endNode.tag = "End Node";

        startNodeMR.material = _startMat;
        endNodeMR.material = _endMat;
    }
/// <summary>
/// Generates terrain nodes that cannot be traversed. 
/// </summary>
    private void GenerateObstacles()
    {
 
    }


    private void CameraLerp()
    {
        Vector3 camDefaultPos = _camera.transform.position;
        var speed = new Vector3(1, 1, 1);
        Vector3 camStartPos = 
            new Vector3((float)_scaledDepth/2 - 2f, (float)(_scaledDepth + _scaledWidth)/2 - 2f, (float)_scaledDepth/2 - 2f);
        _camera.transform.position = Vector3.SmoothDamp(camDefaultPos, camStartPos, ref speed, Time.deltaTime);
    }
    private void PrintList()
    {
        foreach(GameObject item in nodeList)
            if(item != null)
            {
                print(item.name);
            }
            else
            {
                print("Item is null");
            }
        
        print("NodeList Length: " + nodeList.Count);
    }
}
