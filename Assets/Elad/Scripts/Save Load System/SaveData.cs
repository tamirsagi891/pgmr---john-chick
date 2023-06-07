using System.Collections.Generic;
using Elad.Scripts;
using Elad.Scripts.Arrows;
using Elad.Scripts.Combat;
using UnityEngine;

namespace Elad.Save_Load_System
{
    [System.Serializable]
    public class SaveData
    {
        public FeatherToCollectLists featherToCollectLists;
        public PlayerSaveData playerSaveData;
    }
}
