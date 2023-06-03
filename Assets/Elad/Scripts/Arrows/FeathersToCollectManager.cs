using System;
using System.Collections.Generic;
using Elad.Save_Load_System;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Arrows
{
    public class FeathersToCollectManager : MonoBehaviour
    {
        [SerializeField] private GameObject Feather;
        private FeathersToCollectData _feathersToCollectData = new FeathersToCollectData();

        [SerializeField] private bool initializeFromJason;

        private void Awake()
        {
            PlayerStatus.FeathersToCollectManager = this;
            PlayerStatus.InitializeFromJason = initializeFromJason;
        }

        private void Start()
        {
            if (initializeFromJason)
            {
                LoadFeathersStatus();
            }
        }

        public void Add(Feather feather)
        {
            _feathersToCollectData.FeatherList.Add(feather);
        }

        public void Remove(Feather feather)
        {
            _feathersToCollectData.FeatherList.Remove(feather);
        }

        public void SaveFeathersStatus()
        {
            SaveGameManager.CurrentSaveData.feathersToCollectData = _feathersToCollectData;
        }

        public void LoadFeathersStatus()
        {
            var temp = SaveGameManager.CurrentSaveData.feathersToCollectData;
            foreach (var featherData in temp.FeatherList)
            {
                
                    Instantiate(Feather, featherData.Position, Quaternion.identity);
            }

            _feathersToCollectData = temp;

        }
    }

    [System.Serializable]
    public class FeathersToCollectData
    {
        public List<Feather> FeatherList = new List<Feather>();
    }
}