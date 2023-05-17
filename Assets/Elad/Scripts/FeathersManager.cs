using System;
using System.Collections.Generic;
using Elad.Events;
using UnityEngine;

namespace Elad.Scripts
{
    public class FeathersManager : MonoBehaviour
    {
        public enum FeatherKind
        {
            White,
            Red,
            Blue
        }

        [SerializeField] private FeatherKind curFeatherKind = FeatherKind.White;
        [SerializeField] private Dictionary<FeatherKind, int> _featherDamageDic; 
        
        public FeatherKind CurFeatherKind
        {
            get => curFeatherKind;
            set => curFeatherKind = value;
        }

        private void OnEnable()
        {
            characterEvents.AddFeather.AddListener(AddFeather);
            characterEvents.RemoveFeather.AddListener(RemoveFeather);
        }

        private void OnDisable()
        {
            characterEvents.AddFeather.RemoveListener(AddFeather);
            characterEvents.RemoveFeather.RemoveListener(RemoveFeather);
        }

        private Dictionary<FeatherKind, int> _featherDic;

        private void Awake()
        {
            _featherDic = new Dictionary<FeatherKind, int>();
        }

        public void AddFeather(FeatherKind featherKind)
        {
            print(featherKind);
            if (_featherDic.ContainsKey(featherKind))
            {
                _featherDic[featherKind] += 1;    
            }

            else
            {
                _featherDic[featherKind] = 1;
            }
            
            
        }
        
        public void RemoveFeather(FeatherKind featherKind)
        {
            int curAmount = _featherDic[featherKind];
            if (curAmount > 0)
            {
                _featherDic[featherKind] -= 1;
            }
            
        }

        public int HowMany(FeatherKind featherKind)
        {
            int curAmount = _featherDic[featherKind];
            return curAmount;
        }

        public int ReturnCurDamage()
        {
            return _featherDamageDic[curFeatherKind];
        }
        
    }
}
