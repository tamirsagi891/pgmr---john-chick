using System;
using BitStrap;
using Elad.Scripts;
using Elad.Scripts.Arrows;
using Mechanics.UI.Menus.Menu_Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/End Level Menu")]
    public class EndLevelMenu : BaseMenuController
    {
        [SerializeField]
        [RequiredReference]
        private FeatherSlider featherSlider;

        [SerializeField]
        private MenuCounter deathCounter;

        [SerializeField]
        private MenuCounter timeCounter;

        private int Total => PlayerStatus.FeathersToCollectManager.StartFeatherAmount;
        
        public override void OpenMenu()
        {
            base.OpenMenu();
            featherSlider.StartFeatherAnimation();
            deathCounter.Count = $"{PlayerStatus.PlayerDamageable.DeathAmounts}";
            Logger.Log("ASK ELAD TO ADD DEATH HERE", Color.red, this);
            TimeSpan time = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
            timeCounter.Count = $"{time:m\\:ss\\.ff}";
        }
    }

    [Serializable]
    public struct LevelScore
    {
        public int deathCount;
        public float completionTime;
        public TimeSpan timeSpan;
        public int feathersCollected;
        public int totalFeathers;
    }
}