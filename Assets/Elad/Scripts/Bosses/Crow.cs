using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using UnityEngine;

public class Crow : MonoBehaviour
{
    private Transform target;

    [Header("Speed")] [SerializeField] private float speed;


    private void Start()
    {
        target = PlayerStatus.PlayerController.gameObject.transform;
    }


    // Update is called once per frame
    void Update()
    {
        MoveTowardPlayer();
        SideHandler();
    }

    private void MoveTowardPlayer()
    {
        // Move our position a step closer to the target.
        float step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }

    private void SideHandler()
    {
        // Determine direction to the target
        Vector2 direction = target.position - transform.position;

        // Flip the sprite based on the direction to the target
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
    }
}