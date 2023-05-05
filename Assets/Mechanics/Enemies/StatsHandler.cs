using System;
using Avrahamy.Math;
using BitStrap;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Stats Handler", -1)]
    [RequireComponent(typeof(BaseNpc))]
    [DisallowMultipleComponent]
    public class StatsHandler : MonoBehaviour
    {
        #region Inspector
        
        [HelpBox("This are Play only - and are initialized from the scriptable object!")]
        [Header("Current Stats")]
        [SerializeField]
        [ReadOnly(onlyInEditor=true)]
        private NpcStats currentStats;

        #endregion

        #region Private Fields

        private BaseNpc _myNpc;
        private NpcStats _initialStats;

        public NpcStats CurrentStats
        {
            get => currentStats;
            set => currentStats = value;
        }

        #endregion
        
        #region MonoBehaviour

        private void Awake()
        {
            _myNpc = GetComponent<BaseNpc>();
            _initialStats = _myNpc.NpcData.stats;
            ResetStats();
        }
        
        #endregion

        #region Public Methods

        [Button]
        public void ResetStats()
        {
            currentStats = _initialStats.Copy();
        }

        #endregion
    }
}
