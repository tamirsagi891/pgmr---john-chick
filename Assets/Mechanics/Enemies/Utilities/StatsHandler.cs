using BitStrap;
using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Stats Handler", -1)]
    [DisallowMultipleComponent]
    public class StatsHandler : MonoBehaviour
    {

        #region Inspector

        [HelpBox("This are Play only - and are initialized from the scriptable object!")]
        [Header("Current Stats")]
        [SerializeField]
        [ReadOnly(onlyInEditor = true)]
        private NpcStats currentStats;

        #endregion

        #region Public Properties

        public float Hp
        {
            get => currentStats.hp;
            set => currentStats.hp = Mathf.Max(0f, value);
        }

        #endregion

        #region Private Fields

        private BaseNpc _myNpc;
        private NpcStats _initialStats;

        public NpcStats CurrentStats => currentStats;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _myNpc = GetComponent<BaseNpc>();
            _initialStats = _myNpc.NpcDataScriptable.stats;
            RestoreInitialStats();
        }

        #endregion

        #region Public Methods

        [Button]
        public void RestoreInitialStats()
        {
            currentStats = _initialStats.Copy();
        }

        public float TakeDamage(float dmgTaken)
        {
            Hp -= dmgTaken - CurrentStats.defense + CurrentStats.deBuff;
            return Hp;
        }

        public void ApplyDamageMultiplier(float multiplier)
        {
            currentStats.hp *= multiplier;
            currentStats.damage *= multiplier;
            currentStats.defense *= multiplier;
        }

        #endregion

    }
}
