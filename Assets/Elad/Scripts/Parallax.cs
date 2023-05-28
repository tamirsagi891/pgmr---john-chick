using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField]private Camera _camera;

    [SerializeField]private GameObject _player;
    private float _distanceFromPlayer => transform.position.z - _player.transform.position.z;

    private float _clippingPlane => _camera.transform.position.z +
                                    (_distanceFromPlayer > 0 ? _camera.farClipPlane : _camera.nearClipPlane);

    private float _parallaxFactor => MathF.Abs(_distanceFromPlayer / _clippingPlane);
    
    private Vector2 _originalPosition;
    private float _originalZ;


    private Vector2 _travel => (Vector2) _camera.transform.position - _originalPosition;
    private Vector2 _parallaxEffect;
    private void Awake()
    {
       
        var tempPos = transform.position;
        _originalPosition = tempPos;
        _originalZ = tempPos.z;
    }

    
    
    void Update()
    {
        var tempPos =  _originalPosition + _travel * _parallaxFactor;
        transform.position = new Vector3(tempPos.x, tempPos.y, _originalZ);
    }
}
