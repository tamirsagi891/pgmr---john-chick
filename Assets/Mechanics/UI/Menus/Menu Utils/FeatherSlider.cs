using System;
using System.Collections.Generic;
using Avrahamy;
using BitStrap;
using Elad.Scripts;
using UnityEngine;
using UnityEngine.Pool;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus.Menu_Utils
{
    public class FeatherSlider : MonoBehaviour
    {
        [SerializeField]
        private PassiveRealTimeTimer timeBetweenFeathers = new(0.5f);

        [SerializeField]
        private EndScoreManager scoreManager;
        
        [Space]
        [SerializeField]
        private GameObject featherImage;

        public LinkedPool<GameObject> Pool { get; set; }
        
        private List<GameObject> _images = new();
        private int _currentFeathers;

        protected void Awake()
        {
            Pool = new LinkedPool<GameObject>(
                InitFeather,
                GetFeather,
                ReleaseFeather,
                DestroyFeather);
        }

        private void OnEnable()
        {
            ClearFeathers();
            timeBetweenFeathers.Clear();
        }

        private void Update()
        {
            if (timeBetweenFeathers.IsSet && !timeBetweenFeathers.IsActive)
            {
                if (_images.Count < _currentFeathers)
                {
                    IncImages();
                }
                if (_images.Count >= _currentFeathers)
                {
                    timeBetweenFeathers.Clear();
                    Logger.Log("Finished Feathers Loop", Color.cyan, this);
                }
                else
                {
                    timeBetweenFeathers.Start();
                }
            }
        }

        protected void DestroyFeather(GameObject projectile)
        {
            Destroy(projectile.gameObject);
        }

        protected void ReleaseFeather(GameObject projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        protected GameObject InitFeather()
        {
            var projectile = Instantiate(featherImage, transform);
            return projectile;
        }
        protected void GetFeather(GameObject projectile)
        {
            projectile.gameObject.SetActive(true);
        }
        
        [Button]
        public void StartFeatherAnimation()
        {
            _currentFeathers = PlayerStatus.FeathersToCollectManager.CollectedFeatherAmount;
            ClearFeathers();
            timeBetweenFeathers.Start();
            Logger.Log("Started Feathers Loop", Color.green, this);

        }

        private void ClearFeathers()
        {
            foreach (var image in _images)
            {
                Pool.Release(image);
            }
            _images.Clear();
            scoreManager.CurrentDisplayed = _images.Count;
        }
    
        [Button]
        private void IncImages()
        {
            var feather = Pool.Get();
            _images.Add(feather);
            scoreManager.CurrentDisplayed = _images.Count;
        }
        
        [Button]
        private void DecImages()
        {
            if (_images.Count > 0)
            {
                var image = _images[^1];
                _images.Remove(image);
                scoreManager.CurrentDisplayed = _images.Count;
                Pool.Release(image);
            }
        }
        
        [Button]
        private void DoHugeTest(int count = 200)
        {
            _currentFeathers = count;
            ClearFeathers();
            timeBetweenFeathers.Start();
            Logger.Log("Started Feathers Test", Color.magenta, this);
        }
    }
}