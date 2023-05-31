using System;
using BitStrap;
using Nemesh.Attributes;
using UnityEngine;

namespace Mechanics.Enemies
{
    [CreateAssetMenu(fileName = "Npc_Data_", menuName = "NPC/Base Data", order = 0)]
    public class NpcDataScriptable : ScriptableObject
    {
        [SerializeField]
        public string npcName;

        [SerializeField]
        [InspectorFieldName("Type Of Enemy:")]
        public NpcType type = NpcType.Ground;

        [HelpBox("This stats are the initial value - See StatsHandler for the current values")]
        [SerializeField]
        [ReadOnly(onlyInPlaymode = true)]
        [InspectorFieldName("Initial NPC Stats")]
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
        public Vector2 knockBack = Vector2.zero;

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

        [SerializeField]
        public float defense;

        [SerializeField]
        public float deBuff;

        [SerializeField]
        public AttackType type = AttackType.Regular;

        public override string ToString()
        {
            return $@"Stats:
hp: {hp}  --  def: {defense}  --  rad: {detectionRadius}
dmg: {damage}  -- deBuff: {deBuff}  -- cd: {cooldown}
ms: {movementSpeed}  -- dash: {extraDashSpeed}  -- jump: {jumpForce}
";
        }

        public NpcStats Copy()
        {
            return MemberwiseClone() as NpcStats;
        }
    }

    [Serializable]
    public enum NpcType
    {
        Ground,
        Flying
    }
}
