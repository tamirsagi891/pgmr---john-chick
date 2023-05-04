using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics.Enemies
{
    [CreateAssetMenu(fileName = "Npc_Data_", menuName = "NPC/Base Data", order = 0)]
    public class NpcData : ScriptableObject
    {
        [SerializeField] 
        public string npcName;

        [SerializeField] 
        public NpcStats stats;

        public override string ToString()
        {
            return $"name: {npcName} :: stats: \n{stats}";
        }
    }

    [Serializable]
    public class NpcStats
    {
        [SerializeField] 
        public int hp;

        public override string ToString()
        {
            return $"hp: {hp}";
        }
    }
    
    
}