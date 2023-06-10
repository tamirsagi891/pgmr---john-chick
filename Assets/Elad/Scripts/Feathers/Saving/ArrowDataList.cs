using System.Collections.Generic;
using UnityEngine;

namespace Elad.Scripts
{
    [CreateAssetMenu(fileName = "ArrowData", menuName = "Player/Attacks/Arrows", order = 0)]
    public class ArrowDataList : ScriptableObject
    {
        [SerializeField] public List<ArrowData> arrowAttacksList;
    }
}