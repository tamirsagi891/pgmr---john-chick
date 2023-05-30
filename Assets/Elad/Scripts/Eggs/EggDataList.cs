using System.Collections.Generic;
using UnityEngine;

namespace Elad.Scripts
{
    [CreateAssetMenu(fileName = "EggData", menuName = "Player/Attacks/Eggs", order = 1)]
    public class EggDataList : ScriptableObject
    {
        [SerializeField] public List<EggData> eggDataList;
    }
}

