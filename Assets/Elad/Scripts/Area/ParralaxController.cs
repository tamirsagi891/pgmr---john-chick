using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class ParralaxController : MonoBehaviour
{
    private Transform _cam;

    private Vector3 _camStartPos;

    private float _distance;

    private GameObject[] _backgrounds;

    private Material[] mat;

    private float[] backSpeed;

    private float farthestBack;

    private GameObject _player;

    [Range(0.01f, 0.05f)] [SerializeField] private float parallaxSpeed;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _cam = Camera.main.transform;
        _camStartPos = _cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        _backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            _backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = _backgrounds[i].GetComponent<Renderer>().material;
        }

        BackSpeedCalculate(backCount);
    }

    void Start()
    {
        _player = PlayerStatus.player.gameObject;
    }


    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++) // find the farthest background 
        {
            if ((_backgrounds[i].transform.position.z - _cam.position.z) > farthestBack)
            {
                farthestBack = (_backgrounds[i].transform.position.z - _cam.position.z);
            }
        }

        for (int i = 0; i < backCount; i++)
        {
            backSpeed[i] = 1 - ((_backgrounds[i].transform.position.z - _cam.position.z) / farthestBack);
        }
    }

    private void LateUpdate()
    {
        _distance = _cam.position.x - _camStartPos.x;
<<<<<<< HEAD
        // var camPos = _player.transform.position + positionOffset;
        // transform.position = new Vector3(camPos.x, camPos.y, 0);

=======
>>>>>>> 93eb5efa15efdb2ac8abcf94599c9aa76609855c
        for (int i = 0; i < _backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            mat[i].SetTextureOffset("_MainTex", new Vector2(_distance, 0) * speed);
        }
    }
}