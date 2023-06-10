using System;
using UnityEngine;

namespace Elad.Scripts
{
    [Serializable]
    public class ArrowData
    {

        public FeathersManager.FeatherKind featherKind;
        public Vector2 moveSpeed;
        public int damage;
        public float lifeTime = 3f;
        public RigidbodyType2D rigidbodyType2D;
        public bool addPlayerVelocity = true;
        public ContactFilter2D hitFilter;
    }
}
