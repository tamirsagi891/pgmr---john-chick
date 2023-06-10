using System;
using System.Collections.Generic;
using BitStrap;
using Elad.Events;
using Elad.Save_Load_System;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Arrows
{
    
    public class FeathersToCollectManager : MonoBehaviour
    {
        static int howManyTimesCalled = 0;
        [SerializeField] [ReadOnly] private int startFeatherAmount;
        [SerializeField] [ReadOnly] private int collectedFeatherAmount;
        [SerializeField] [ReadOnly] private int currentFeathersAmount;
        [SerializeField] [ReadOnly] private float percentagesCurrentFeathersAmount;
        private int _lastFeatherId = 0;
        
        private FeatherToCollectLists _featherToCollectLists = new FeatherToCollectLists();

        [SerializeField] private bool initializeFromJason;

        public int CurrentFeathersAmount
        {
            get => currentFeathersAmount;
            set
            {
                currentFeathersAmount = value;
                PercentagesCurrentFeathersAmount = ((float)CollectedFeatherAmount / (float)startFeatherAmount) * 100;
            }
        }

        public float PercentagesCurrentFeathersAmount
        {
            get => percentagesCurrentFeathersAmount;
            set => percentagesCurrentFeathersAmount = value;
        }

        public int CollectedFeatherAmount
        {
            get => collectedFeatherAmount;
            set => collectedFeatherAmount = value;
        }


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
                // initializeFromJason = false;
            }
            
        }

        public void AddFeather(FeatherToCollect featherToCollect)
        {
            
            _featherToCollectLists.featherList.Add(featherToCollect);
            startFeatherAmount += 1;
            CurrentFeathersAmount += 1;
            featherToCollect.ID = CurrentFeathersAmount;

        }

        public void RemoveFeather(FeatherToCollect featherToCollect)
        {
            if (featherToCollect.ID != _lastFeatherId)
            {
                _lastFeatherId = featherToCollect.ID;
                CollectedFeatherAmount += 1;
                _featherToCollectLists.featherList.Remove(featherToCollect);
                CurrentFeathersAmount -= 1;
            }
            
            

        }

        public void SaveFeathersStatus()
        {
            SaveGameOnJson.CurrentSaveData.featherToCollectLists = _featherToCollectLists;
            // Logger.Log(_featherToCollectLists.featherList.Count);
        }

        public void LoadFeathersStatus()
        {
            Logger.Log("In LoadFeathersStatus");
            _featherToCollectLists = SaveGameOnJson.CurrentSaveData.featherToCollectLists;

            int currentFeathersAmountTemp = 0;
            foreach (var featherData in _featherToCollectLists.featherList)
            {
                currentFeathersAmountTemp += 1;
                featherData.gameObject.SetActive(true);
            }

            CurrentFeathersAmount = currentFeathersAmountTemp;
        }
    }

    [System.Serializable]
    public class FeatherToCollectLists
    {
        public List<FeatherToCollect> featherList = new List<FeatherToCollect>();
    }
}