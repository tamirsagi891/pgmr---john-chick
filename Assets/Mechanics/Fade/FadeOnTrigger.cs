using System;
using BitStrap;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Mechanics.Fade
{
    /// <summary>
    /// Set fade effect to tall objects that hide the player
    /// </summary>
    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(CompositeCollider2D))]
    public class FadeOnTrigger : MonoBehaviour
    {
        #region Inspector

        [Header("Hide Tilemap")]
        [SerializeField]
        private bool setRendererEnabledOnStart = true;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The alpha value to go to")]
        private float transparency = 0.5f;

        [SerializeField]
        [Tooltip("Time to reach the transparency target")]
        private float fadeTime = 0.5f;

        [SerializeField]
        [TagSelector]
        [Tooltip("The tag used by the player")]
        private string playerTag = "Player";

        #endregion

        #region Private Fields

        private float _t;
        private int _direction = 1;
        private bool _notActive = true;
        private Color _normalColor;
        private Color _fadeColor;
        private Tilemap _myTilemap;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            if (_myTilemap == null)
            {
                _myTilemap = GetComponentInParent<Tilemap>();
            }

            _normalColor = _myTilemap.color;
            _fadeColor = new Color(_normalColor.r, _normalColor.g, _normalColor.b, transparency);
        }

        private void Start()
        {
            if (setRendererEnabledOnStart && TryGetComponent(out TilemapRenderer tilemapRenderer))
            {
                tilemapRenderer.enabled = true;
            }
        }

        private void Update()
        {
            if (_notActive)
            {
                return;
            }

            _t += _direction * Time.deltaTime / fadeTime;
            _t = Mathf.Clamp(_t, 0, 1);
            _myTilemap.color = Color.Lerp(_normalColor, _fadeColor, _t);
            _notActive = _t >= 1 || _t <= 0;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Player"))
            {
                return;
            }

            AudioManager.instance.PlayOneShot(FMODEvents.instance.caveAppear, transform.position);
            _direction = 1;
            _notActive = false;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag))
            {
                return;
            }
    
            AudioManager.instance.PlayOneShot(FMODEvents.instance.caveAppear, transform.position);
            _direction = -1;
            _notActive = false;
        }

        #endregion

        #region BitStrap

        [Button]
        private void ManualTrigger()
        {
            _direction = -_direction;
            _notActive = false;
        }

        #endregion
    }
}