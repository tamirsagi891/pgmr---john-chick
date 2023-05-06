using System;
using BitStrap;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics.Enemies
{
    [CreateAssetMenu(fileName = "Npc_Data_", menuName = "NPC/Base Data", order = 0)]
    public class NpcData : ScriptableObject
    {
        [SerializeField] 
        public string npcName;

        [HelpBox("This stats are the initial value - See StatsHandler for the current values")]
        [SerializeField]
        [ReadOnly(onlyInPlaymode = true)]
        public NpcStats stats;

        [Space]
        [SerializeField]
        public Sprite characterIcon;

        public override string ToString()
        {
            return $"name: {npcName} :: stats: \n{stats}";
        }
    }

    [Serializable]
    public class NpcStats
    {
        [SerializeField] 
        public float hp;

        [SerializeField]
        [Min(0)]
        public float detectionRadius = 10f;

        [SerializeField]
        public float damage = 3f;

        [SerializeField]
        [Min(0)]
        public float cooldown = 2f;

        [SerializeField]
        [Min(0)]
        public float movementSpeed = 1f;

        [SerializeField]
        [Min(0)]
        public float jumpForce = 40f;

        [SerializeField]
        [Min(0)]
        public float extraDashSpeed = 10f;

        public override string ToString()
        {
            return $"hp: {hp}" +
                   $"rad: {detectionRadius}" +
                   $"dmg: {damage}" +
                   $"cd: {cooldown}" +
                   $"ms: {movementSpeed}";
        }

        public NpcStats Copy()
        {
            return MemberwiseClone() as NpcStats;
        }
    }
    
    
}