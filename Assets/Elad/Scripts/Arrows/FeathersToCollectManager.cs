using System;
using System.Collections.Generic;
using Elad.Events;
using Elad.Save_Load_System;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Arrows
{
    public class FeathersToCollectManager : MonoBehaviour
    {
        [SerializeField] private GameObject Feather;
        private FeatherToCollectLists _featherToCollectLists = new FeatherToCollectLists();

        [SerializeField] private bool initializeFromJason;


        private void OnEnable()
        {
            characterEvents.FunctionsSave.AddListener(SaveFeathersStatus);
            characterEvents.FunctionsLoad.AddListener(LoadFeathersStatus);
        }

        private void OnDisable()
        {
            characterEvents.FunctionsSave.RemoveListener(SaveFeathersStatus);
            characterEvents.FunctionsLoad.RemoveListener(LoadFeathersStatus);
        }

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
                initializeFromJason = false;
            }
        }

        public void AddFeather(FeatherToCollect featherToCollect)
        {
            _featherToCollectLists.featherList.Add(featherToCollect);
        }

        public void RemoveFeather(FeatherToCollect featherToCollect)
        {
            _featherToCollectLists.featherList.Remove(featherToCollect);
        }

        public void SaveFeathersStatus()
        {
            SaveGameOnJson.CurrentSaveData.featherToCollectLists = _featherToCollectLists;
        }

        public void LoadFeathersStatus()
        {
            _featherToCollectLists = SaveGameOnJson.CurrentSaveData.featherToCollectLists;

            foreach (var featherData in _featherToCollectLists.featherList)
            {
                featherData.gameObject.SetActive(true);
            }
        }
    }

    [System.Serializable]
    public class FeatherToCollectLists
    {
        public List<FeatherToCollect> featherList = new List<FeatherToCollect>();
    }
}