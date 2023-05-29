using System;
using System.Collections.Generic;
using BitStrap;
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

        [SerializeField] [InlineScriptableObject]
        private ArrowDataList arrowList;

        private List<ArrowData> arrowAttacksList => arrowList.arrowAttacksList;
        // [SerializeField] private List<ArrowData> arrowAttacksList;
        
        private Dictionary<FeatherKind, ArrowData> _arrowDataDic;

        public FeatherKind CurFeatherKind
        {
            get => curFeatherKind;
            set
            {
                curFeatherKind = value;
                PlayerStatus.CurrentArrowDataData = _arrowDataDic[value];
            }
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

        private Dictionary<FeatherKind, int> _featherAmountDic;

        private void Awake()
        {
            DictionaryInit();
            SetCurrentFeather(_arrowDataDic[FeatherKind.White]);
        }

        private void DictionaryInit()
        {
            _arrowDataDic = new Dictionary<FeatherKind, ArrowData>();
            foreach (var arrow in arrowAttacksList)
            {
                _arrowDataDic[arrow.featherKind] = arrow;
            }
        }
        
        public void AddFeather(FeatherKind featherKind)
        {
            
            if (_featherAmountDic.ContainsKey(featherKind))
            {
                _featherAmountDic[featherKind] += 1;    
            }

            else
            {
                _featherAmountDic[featherKind] = 1;
            }
            
            
        }
        
        public void RemoveFeather(FeatherKind featherKind)
        {
            int curAmount = _featherAmountDic[featherKind];
            if (curAmount > 0)
            {
                _featherAmountDic[featherKind] -= 1;
            }
            
        }

        public int HowMany(FeatherKind featherKind)
        {
            int curAmount = _featherAmountDic[featherKind];
            return curAmount;
        }

        public int ReturnCurDamage()
        {
            return _arrowDataDic[curFeatherKind].damage;
        }

        private void SetCurrentFeather(ArrowData curArrowDataData)
        {
            PlayerStatus.CurrentArrowDataData = curArrowDataData;
        }
        
        
    }
}
