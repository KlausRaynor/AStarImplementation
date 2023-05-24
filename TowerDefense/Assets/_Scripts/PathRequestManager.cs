using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    
    Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
    PathRequest _currentPathRequest;
    
    static PathRequestManager _instance;
    PathGenerator _pathGenerator;
    private bool _isProcessingPath;

    void Awake()
    {
        _instance = this;
        _pathGenerator = GetComponent<PathGenerator>();
    }
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        _instance._pathRequestQueue.Enqueue(newRequest);
        _instance.TryProecessNext();
    }

    private void TryProecessNext()
    {
        if (!_isProcessingPath && _pathRequestQueue.Count > 0)
        {
            _currentPathRequest = _pathRequestQueue.Dequeue();
            _isProcessingPath = true;
            _pathGenerator.StartFindPath(_currentPathRequest.pathStart, _currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        _currentPathRequest.callback(path, success);
        _isProcessingPath = false;
        TryProecessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.callback = callback;
        }
    }
}
