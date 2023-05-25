using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OnHoverEffect : MonoBehaviour
{
    /// <summary>
    /// Node Mouse over effect.
    /// On mouse over: node floats up a bit and grows in size.
    /// On mouse exit: node returns to normal position and scale.
    /// </summary>
    private Vector3 _posDefault;
    private Vector3 _posOffset;
    private Vector3 _scaleDefault;
    private Vector3 _scaleOffset;
    private MeshRenderer _defaultMR, _hoverMR;
    [SerializeField] private Material _baseMat, _hoverMat, _startMat, _endMat;

    void Start()
    {
        _posDefault = transform.position;
        _posOffset = _posDefault + Vector3.up;
        _scaleDefault = transform.localScale;
        _scaleOffset = _scaleDefault + Vector3.one;
        _defaultMR = gameObject.GetComponent<MeshRenderer>();
    }
    void OnMouseOver()
    {
        if (transform.position.y <= _posOffset.y)
        {
            transform.position = _posOffset;
            transform.localScale = _scaleOffset;
            _defaultMR.material = _hoverMat;
        }
    }

    void OnMouseExit()
    {
        if (transform.position.y >= _posDefault.y)
        {
            transform.position = _posDefault;
            transform.localScale = _scaleDefault;
        }
        
        if(gameObject.CompareTag("Start Node"))
        {
            _defaultMR.material = _startMat;
        } else if (gameObject.CompareTag("End Node"))
        {
            _defaultMR.material = _endMat;
        }
        else
        {
            _defaultMR.material = _baseMat;
        }
    }

    /// <summary>
    /// Animation when blocks first spawn in
    /// </summary>
    void BobAnimation()
    {
        
    }
}
