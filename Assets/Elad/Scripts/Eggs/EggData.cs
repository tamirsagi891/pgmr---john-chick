using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using UnityEngine;

[Serializable]
public class EggData
{

    public EggsManager.EggKind eggKind;
    public Vector2 moveSpeed;
    public int damage;
    public float lifeTime = 3f;
    public RigidbodyType2D rigidbodyType2D;
    public bool addPlayerVelocity = true;
    public ContactFilter2D hitFilter;

}