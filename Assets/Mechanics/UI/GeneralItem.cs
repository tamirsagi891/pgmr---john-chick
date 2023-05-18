using System;
using UnityEngine;
using UnityEngine.Events;
using Logger = Nemesh.Logger;

namespace Mechanics.UI
{
    [Serializable]
    public class GeneralItem
    {
        public string name;

        [TextArea]
        public string description;

        public Sprite sprite;

        public UnityEvent<GeneralItem> onItemUse; 

        public GeneralItem()
        {
            name = "item";
            description = "Description";
        }

        public void UseItem()
        {
            Logger.Log($"Used Item {name}");
            onItemUse.Invoke(this);
        }
    }
}