using System.Collections.Generic;
using UnityEngine;

namespace Elad.Scripts
{
    [CreateAssetMenu(fileName = "ArrowData", menuName = "Player/Attacks/Arrows", order = 0)]
    public class ArrowAttackData : ScriptableObject
    {
        [SerializeField] public List<ArrowAttack> arrowAttacksList;
    }
}