using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    private AreaEffector2D _areaEffector2D;
    [SerializeField] private int wantedForce = 100;
    [SerializeField] private int wantedDrag = 100;
    private bool _insideCollider;
    private CharacterJump _characterJump;


    [SerializeField] private float powerReturnTime = 0.3f;
    [SerializeField] private float powerReturnTimer;
    private bool _canWork = true;
    private bool _working = true;
    
    private void Awake()
    {
        _areaEffector2D = GetComponent<AreaEffector2D>();
        _areaEffector2D.forceMagnitude = 0;
        _areaEffector2D.drag = 0;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagStrings.playerTag))
        {
            if (!_characterJump)
            {
                _characterJump = other.GetComponent<CharacterJump>();
            }

            _insideCollider = true;
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(TagStrings.playerTag))
        {
            _insideCollider = false;
            _canWork = false;
            powerReturnTimer = powerReturnTime;
        }
    }


    private void Update()
    {
        if (!_working &&_canWork && _insideCollider && _characterJump && _characterJump.IsGliding)
        {
            _areaEffector2D.forceMagnitude = wantedForce;
            _areaEffector2D.drag = wantedDrag;
            _working = true;
        }
        
        else
        {
            if (_working)
            {
                _areaEffector2D.forceMagnitude = 0;
                _areaEffector2D.drag = 0;
                _working = false;
            }
           
        }

        if (!_working)
        {
            if (powerReturnTimer > 0)
            {
                powerReturnTimer -= Time.deltaTime;
                if (powerReturnTimer <= 0)
                {
                    powerReturnTimer = 0;
                    _canWork = true;
                }
            }
        }
        
        
    }
}