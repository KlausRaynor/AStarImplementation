using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed = 20;
    private Vector3[] _path;
    private int _targetIndex;

    void Start()
    {
        PathRequestManager.RequestPath(transform.position, _target.position, OnPathFound);
    }

    private void OnPathFound(Vector3[] newPath, bool pathSuccessfullyFound)
    {
        if (pathSuccessfullyFound)
        {
            _path = newPath;
            _targetIndex = 0;
            StopCoroutine(nameof(FollowPath));
            StartCoroutine(nameof(FollowPath));
        }
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = _path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                _targetIndex++;
                if(_targetIndex >= _path.Length)
                {
                    yield break;
                }
                currentWaypoint = _path[_targetIndex];
            }
            
            transform.position = Vector3.MoveTowards(transform.position, 
                currentWaypoint, _speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (_path != null)
        {
            for (int i = _targetIndex; i < _path.Length; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawCube(_path[i], Vector3.one);

                if (i == _targetIndex)
                {
                    Gizmos.DrawLine(transform.position, _path[i]);
                }
                else
                {
                    Gizmos.DrawLine(_path[i - 1], _path[i]);
                }
            }
        }
    }
}
