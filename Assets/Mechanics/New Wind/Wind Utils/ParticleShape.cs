using System.Collections;
using System.Collections.Generic;
using Avrahamy;
using UnityEngine;

public class ParticleShape : OptimizedBehaviour
{
    [SerializeField]
    private Vector2 sizeFactor = Vector2.one;

    public Vector2 SizeFactor
    {
        get => sizeFactor;
        set => sizeFactor = value;
    }

    public void SetSize(Vector2 newSize)
    {
        transform.localScale = newSize * SizeFactor;
    }
    
#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(0.41f, 0.2f, 0.38f, 0.33f);
        var transform1 = transform;
        Gizmos.DrawCube(transform1.position, transform1.localScale);
    }
#endif
}
