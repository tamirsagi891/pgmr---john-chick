using System;
using UnityEngine;

namespace Elad.Scripts
{
    public class Parallax : MonoBehaviour
    {
        private Camera _camera;

        private GameObject _player;
        private float DistanceFromPlayer => transform.position.z - _player.transform.position.z;

        private float ClippingPlane => _camera.transform.position.z +
                                        (DistanceFromPlayer > 0 ? _camera.farClipPlane : _camera.nearClipPlane);

        private float ParallaxFactor => MathF.Abs(DistanceFromPlayer / ClippingPlane);
    
        private Vector2 _originalPosition;
        private float _originalZ;


        private Vector2 Travel => (Vector2) _camera.transform.position - _originalPosition;
        private Vector2 _parallaxEffect;
        private void Awake()
        {
            _camera = Camera.main;
            
            var tempPos = transform.position;
            _originalPosition = tempPos;
            _originalZ = tempPos.z;
        }

        private void Start()
        {
            _player = PlayerStatus.player;        
        }


        void Update()
        {
            var tempPos =  _originalPosition + Travel * ParallaxFactor;
            transform.position = new Vector3(tempPos.x, tempPos.y, _originalZ);
        }
    }
}
