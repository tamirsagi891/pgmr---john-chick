using System;
using Elad.Scripts;
using Elad.Scripts.Arrows;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus.Menu_Utils
{
    public class DoorKey : MonoBehaviour
    {
        [SerializeField] private bool isStartTimeLine;
        [SerializeField] public float openPercentage = 0.6f;

        [Space] [SerializeField] private float minVisibleDistance = 12f;
        [SerializeField] private float maxVisibleDistance = 18f;

        [Space] [SerializeField] private Door myDoor;

        private Transform _playerTransform;
        private SpriteRenderer _mySprite;

        private float currentAlpha = -1;
        private bool _isOpen;

        #region Public Methods

        public void CloseDoorImmediate()
        {
            myDoor.CloseDoorImmediate();
        }

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            _playerTransform = PlayerStatus.Player.transform;
            _mySprite = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_isOpen) return;

            float distance = Vector3.Distance(_playerTransform.position, transform.position);

            if (distance <= minVisibleDistance)
            {
                // Fully visible
                SetAlpha(1);
            }
            else if (distance > maxVisibleDistance)
            {
                // Fully transparent
                SetAlpha(0);
            }
            else
            {
                // Interpolate between fully visible and fully transparent based on distance
                float alpha = 1 - (distance - minVisibleDistance) / (maxVisibleDistance - minVisibleDistance);
                SetAlpha(alpha);
            }
        }

        private void OnEnable()
        {
            FeathersToCollectManager.OnPercentageChange += CollectedFeather;
        }

        private void OnDisable()
        {
            FeathersToCollectManager.OnPercentageChange -= CollectedFeather;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Logger.Log("in key trigger");
            var startCutScene = GetComponent<EndLevelCutScene>();
            startCutScene.StartCutScene = true;
        }

        public void OpenEndUpMenu()
        {
            Logger.Log("in open end up menu");
            MenuManager.Menu.OpenEndLevelMenu();
        }

        #endregion

        #region Private Methods

        private void CollectedFeather([CanBeNull] object sender, float percent)
        {
            if (myDoor.IsDoorMoving || percent < openPercentage)
            {
                return;
            }

            Logger.Log($"Opening End Door {percent}", Color.magenta, myDoor);

            if (percent > openPercentage)
            {
                // SetAlpha(0);
                _isOpen = true;
            }

            myDoor.OpenDoor();
        }

        private void SetAlpha(float alpha)
        {
            if (Math.Abs(currentAlpha - alpha) < 0.01f) return;
            currentAlpha = alpha;
            var color = _mySprite.color;
            color.a = alpha;
            _mySprite.color = color;
        }

        #endregion
    }
}